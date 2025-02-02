using System;
using System.Collections.Generic;
using Creatures.AI;
using Creatures.Collision;
using Creatures.Parts;
using JetBrains.Annotations;
using NaughtyAttributes;
using Player;
using RamenSea.Foundation.Extensions;
using RamenSea.Foundation3D.Extensions;
using Systems;
using UnityEngine;
using UnityEngine.Animations.Rigging;


namespace Creatures {
    
    public class BaseCreature: MonoBehaviour {
        public const int MAX_HEALTH = 10_000;
        public const float FLAP_RECHARGE = 0.2f;
        public static Quaternion ShortestRotation(Quaternion to, Quaternion from) {
            if (Quaternion.Dot(to, from) < 0) {
                return to * Quaternion.Inverse(Multiply(from, -1));
            } else {
                return to * Quaternion.Inverse(from);
            }
        }
        public static Quaternion Multiply(Quaternion input, float scalar) {
            return new Quaternion(input.x * scalar, input.y * scalar, input.z * scalar, input.w * scalar);
        }
        
        [SerializeField] protected CreatureTraits _compiledTraits; //isn't actually serialized
        [SerializeField] private Rigidbody _rb;
        public Rigidbody rb => _rb;
        public RigBuilder rigBuilder;
        public CreatureTraits compiledTraits => this._compiledTraits;
        public CreatureTraits? additionalTraits;
        public BaseBodyPart bodyPart { get; set; }
        public BaseCreaturePart legPart => this.bodyPart?.attachedLegPart;
        public BaseCreaturePart headPart => this.bodyPart?.attachedHeadPart;
        public BaseCreaturePart armPart => this.bodyPart?.attachedArmsPart;

        public WaterInfo waterInfo;
        public Transform movingPlatformTransform;
        public List<BaseLimb> attachedLimbs;
        public List<BaseAIAgent> agents;
        public ParticleSystem jumpOnEffect;

        /*
         * States
         */
        public Vector3 moveDirection;
        public Vector3 moveInput;
        public bool isOnGround;
        public bool landedOnGroundThisFrame;
        public float timeInAirLast;
        public float timeOutOfWaterLast;
        public bool doLegAction;
        public bool doHeadAction;
        public bool doArmsAction;
        public float health;
        public bool shouldLog = false;
        public bool isDead => health <= 0;

        [NonSerialized] public bool isPlayer = false;
        [SerializeField] public Vector3 goalVelocity;
        [SerializeField] public float jumpRecharge = 0;
        [SerializeField] public float flapRechargeTimer = 0;
        [SerializeField] public float flapTimer = 0;
        [SerializeField] public bool isFlapping = false;
        [SerializeField] public bool isJumping = false;
        [SerializeField] public float jumpTimer = 0f;
        [SerializeField] public float jumpSavingGrace = 0.1f;
        [SerializeField] public AnimationCurve jumpCurve;
        [SerializeField] public AnimationCurve flapCurve;

        public Vector3 gravity;
        
        private bool wasOnGroundLastFrame = false;
        private Vector3[] raycastPositions;

        private void Awake() {
            this.attachedLimbs = new();
            this.agents = new();
            this.health = MAX_HEALTH;
        }

        private void Update() {
            if (this.isDead) {
                return;
            }
            this.raycastPositions[0] = this.transform.position;
            for (var i = 0; i < this.bodyPart.bodyLimb.legsAttachPoint.Length; i++) {
                this.raycastPositions[i + 1] = this.bodyPart.bodyLimb.legsAttachPoint[i].transform.position;
                this.raycastPositions[i + 1].y = this.raycastPositions[0].y;
            }
            for (var i = 0; i < this.raycastPositions.Length; i++) {
                Debug.DrawLine(this.raycastPositions[i], this.raycastPositions[i] + (this.compiledTraits.height * Vector3.down), Color.red);
            }

            this.landedOnGroundThisFrame = false;
            if (this.isOnGround && this.wasOnGroundLastFrame != this.isOnGround) {
                this.landedOnGroundThisFrame = true;
            }
            this.wasOnGroundLastFrame = this.isOnGround;

            this.bodyPart?.GameUpdate(Time.deltaTime);
            this.headPart?.GameUpdate(Time.deltaTime);
            this.armPart?.GameUpdate(Time.deltaTime);
            this.legPart?.GameUpdate(Time.deltaTime);

            if (this.waterInfo.isSwimming) {
                this.timeOutOfWaterLast = 0;
                this.gravity = Physics.gravity * this.compiledTraits.effectsWaterGravity;
            } else {
                this.timeOutOfWaterLast += Time.deltaTime;
                this.gravity = Physics.gravity * this.compiledTraits.effectsGravity;
            }

            if (this.waterInfo.isSwimming && this.compiledTraits.inWaterDamage > 0.001f) {
                this.TakeDamage(new DealDamage() {
                    amount = this.compiledTraits.inWaterDamage * Time.deltaTime,
                    damageType = DamageType.InWater,
                });
            } else if (!this.waterInfo.isSwimming && (this.timeOutOfWaterLast > 3f || this.isOnGround) && this.compiledTraits.outOfWaterDamage > 0.001f) {
                this.TakeDamage(new DealDamage() {
                    amount = this.compiledTraits.outOfWaterDamage * Time.deltaTime,
                    damageType = DamageType.OutOfWater,
                });
            }
        }

        public BaseCreaturePart GetSocket(PartSlotType slotType) {
            switch (slotType) {
                case PartSlotType.Head:
                    return this.headPart;
                case PartSlotType.Body:
                    return this.bodyPart;
                case PartSlotType.Arms:
                    return this.armPart;
                case PartSlotType.Legs:
                    return this.legPart;
            }

            return null;
        }
        public void SetCreaturePart(BaseCreaturePart creaturePart) {
            if (this.isDead) {
                return;
            }
            creaturePart.creature = this;
            
            switch (creaturePart.slotType) {
                case PartSlotType.Body: {
                    var oldBody = this.bodyPart;
                    this.bodyPart = creaturePart as BaseBodyPart;
                    this.bodyPart.transform.SetParent(this.transform);
                    this.bodyPart.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                    this.bodyPart.AttachBody(this);
                    if (oldBody?.attachedArmsPart != null) {
                        this.bodyPart.AttachPart(oldBody.attachedArmsPart);
                    }
                    if (oldBody?.attachedLegPart != null) {
                        this.bodyPart.AttachPart(oldBody.attachedLegPart);
                    }
                    if (oldBody?.attachedHeadPart != null) {
                        this.bodyPart.AttachPart(oldBody.attachedHeadPart);
                    }

                    if (oldBody != null) {
                        WorldPartCollector.Instance.DropPart(oldBody);
                    }
                    break;
                }
                default: {
                    if (this.bodyPart == null) {
                        Debug.LogError("You can't build a creature without a body");
                        return;
                    }
                    var oldBodyPart = this.GetSocket(creaturePart.slotType);
                    if (oldBodyPart != null) {
                        WorldPartCollector.Instance.DropPart(oldBodyPart);
                    }
                    this.bodyPart.AttachPart(creaturePart);
                    break;
                }
            }
        }
        public void FinishSettingParts(bool resetCounters) {
            if (this.isDead) {
                return;
            }
            this._compiledTraits = CreatureTraitHelper.CreateTraits(this.isPlayer, this);
            if (this.additionalTraits != null) {
                this._compiledTraits = CreatureTraitHelper.BasicAddition(this._compiledTraits, this.additionalTraits.Value, PartSlotType.Arms);
            }

            this.raycastPositions = new Vector3[1 + this.bodyPart.bodyLimb.legsAttachPoint.Length];
            this.rb.mass = this.compiledTraits.weight;
            this.attachedLimbs.Clear();
            
            // build rig graph
            this.rigBuilder.layers.Clear();
            if (this.bodyPart != null) {
                this.attachedLimbs.AddRange(this.bodyPart.limbs);
                for (var i = 0; i < this.bodyPart.limbs.Length; i++) {
                    for (var limbIndex = 0; limbIndex < this.bodyPart.limbs[i].rigs.Length; limbIndex++) {
                        this.rigBuilder.layers.Add(new RigLayer(this.bodyPart.limbs[i].rigs[limbIndex]));
                    }
                }
            }
            if (this.legPart != null) {
                this.attachedLimbs.AddRange(this.legPart.limbs);
                for (var i = 0; i < this.legPart.limbs.Length; i++) {
                    for (var limbIndex = 0; limbIndex < this.legPart.limbs[i].rigs.Length; limbIndex++) {
                        this.rigBuilder.layers.Add(new RigLayer(this.legPart.limbs[i].rigs[limbIndex]));
                    }
                }
            }
            if (this.armPart != null) {
                this.attachedLimbs.AddRange(this.armPart.limbs);
                for (var i = 0; i < this.armPart.limbs.Length; i++) {
                    for (var limbIndex = 0; limbIndex < this.armPart.limbs[i].rigs.Length; limbIndex++) {
                        this.rigBuilder.layers.Add(new RigLayer(this.armPart.limbs[i].rigs[limbIndex]));
                    }
                }
            }
            if (this.headPart != null) {
                this.attachedLimbs.AddRange(this.headPart.limbs);
                for (var i = 0; i < this.headPart.limbs.Length; i++) {
                    for (var limbIndex = 0; limbIndex < this.headPart.limbs[i].rigs.Length; limbIndex++) {
                        this.rigBuilder.layers.Add(new RigLayer(this.headPart.limbs[i].rigs[limbIndex]));
                    }
                }
            }
            this.rigBuilder.Build();

            if (resetCounters) {
                this.moveDirection = Vector3.zero;
                this.isOnGround = false;
                this.wasOnGroundLastFrame = false;
                this.landedOnGroundThisFrame = false;
                this.isJumping = false;
                this.isFlapping = false;
                this.doLegAction = false;
                this.doHeadAction = false;
                this.doArmsAction = false;
                this.timeInAirLast = 0f;
                this.timeOutOfWaterLast = 0f;
                this.jumpTimer = 0f;
                this.health = MAX_HEALTH;
            }
            
            this.waterInfo.SetColliders(this);

            if (this.isPlayer) {
                PlayerDriverController.Instance.PlayerCreatureDidChangeParts();
            }
        }
        private void FixedUpdate() {
            if (this.isDead) {
                return;
            }
            this.PhysicsUpdate(Time.fixedDeltaTime);
        }
        
        public void HandleGravity(float deltaTime) {
            if (this.isDead) {
                return;
            }
            // var usingDown = Vector3.down;
            // this.raycastPositions[0] = this.transform.position;
            // for (var i = 0; i < this.bodyPart.bodyLimb.legsAttachPoint.Length; i++) {
            //     this.raycastPositions[i + 1] = this.bodyPart.bodyLimb.legsAttachPoint[i].transform.position;
            //     this.raycastPositions[i + 1].y = this.raycastPositions[0].y;
            // }
            // RaycastHit hit = new RaycastHit();
            // RaycastHit usingHit = new RaycastHit();
            // bool hasHit = false;
            // bool hitIsPlatform = false;
            // for (var i = 0; i < this.raycastPositions.Length; i++) {
            //     if (Physics.Raycast(this.raycastPositions[i], Vector3.down, out hit, this.compiledTraits.height, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore)) {
            //         var currentHitIsPlatform = hit.collider.gameObject.CompareTag(GameTags.MovingPlatform);
            //         if (!hasHit) {
            //             usingHit = hit;
            //             hitIsPlatform = currentHitIsPlatform;
            //         } else if (hit.rigidbody != null && usingHit.rigidbody == null) {
            //             usingHit = hit;
            //         }
            //         hasHit = true;
            //
            //         if (hitIsPlatform) {
            //             break;
            //         }
            //     }
            // }
            //
            var wasOnGround = this.isOnGround;
            var usingDown = Vector3.down;
            RaycastHit hit;
            if (Physics.Raycast(this.transform.position, Vector3.down, out hit, this.compiledTraits.height, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore)) {
                this.isOnGround = true;
                var velocity = rb.linearVelocity;
                var rayDir = usingDown;
                // var downDir = Vector3.down;
                // var rayDir = this.transform.TransformDirection(downDir);

                var otherVel = Vector3.zero;
                Rigidbody hitBody = hit.rigidbody;
                if (hitBody) {
                    otherVel = hitBody.linearVelocity;
                    if (this.shouldLog) {
                        Debug.Log($"Jumped on ${hitBody.gameObject.name}");
                    }
                }
                if (hit.collider != null && hit.collider.gameObject.CompareTag(GameTags.Creature)) {
                    var creature = hit.collider.gameObject.GetComponent<CreatureCollider>().creature;
                    if (creature != null && !creature.isDead) {
                        if (creature.jumpOnEffect.isStopped) {
                            creature.jumpOnEffect.Stop();
                            creature.jumpOnEffect.Play();
                            creature.jumpOnEffect.transform.position = hit.point;
                        }

                        var damage = (this._compiledTraits.jumpDamage + this.rb.mass * 200f) * deltaTime;
                        creature.rb.AddExplosionForce(this._compiledTraits.jumpPushEffect, hit.point, 0.1f);
                        creature.TakeDamage(new DealDamage() {
                            amount = damage,
                            damageType = DamageType.Direct,
                            fromLocation = this.transform.position,
                        });
                    }
                }
                
                if (hit.collider != null && hit.collider.gameObject.CompareTag(GameTags.MovingPlatform) && this.movingPlatformTransform != hit.collider.transform) {
                    this.movingPlatformTransform = hit.collider.transform;
                    this.transform.parent = this.movingPlatformTransform;
                }
                
                var rayDirVel = Vector3.Dot(rayDir, velocity);
                var otherDirVel = Vector3.Dot(rayDir, otherVel);
                var relVel = rayDirVel - otherDirVel;
                var x = hit.distance - this.compiledTraits.height;
                var springForce = (x * this.compiledTraits.heightSpringForce) -
                    (relVel * this.compiledTraits.heightSpringDamper);
                
                this.rb.AddForce(rayDir * springForce);
            } else {
                if (this.movingPlatformTransform != null) {
                    this.transform.SetParent(null);
                    this.movingPlatformTransform = null;
                }
                this.isOnGround = false;
            }

            if (wasOnGround != this.isOnGround) {
                this.timeInAirLast = 0f;
            }
            if (!this.isOnGround) {
                this.timeInAirLast += deltaTime;
            }
            
            this.rb.AddForce(this.gravity * deltaTime, ForceMode.VelocityChange);
        }

        public void CorrectRotation(float deltaTime) {
            // if (this.isPlayer) {
            //     var y = this.transform
            //     var e = this.rb.rotation.eulerAngles;
            //     e.y = y;
            //     this.transform.rotation = PlayerDriverController.Instance.transform.rotation;
            //
            //     this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, Quaternion.Euler(e),
            //                                                        this.compiledTraits.rotationSpeedMin * deltaTime);
            //                                                        this.rb.MoveRotation();
            // } else {
                var currentVelocity = this.rb.linearVelocity;
                var currentVelocityVector2 = new Vector2(currentVelocity.x, currentVelocity.z);
                var speed = currentVelocityVector2.magnitude;
                if (speed > 0.1f) {
                    this.rb.MoveRotation(Quaternion.RotateTowards(this.rb.rotation, Quaternion.LookRotation(new Vector3(currentVelocityVector2.x, 0, currentVelocityVector2.y)), this.compiledTraits.rotationSpeedMin * deltaTime));
                }
            // }
        }
        public void PhysicsUpdate(float deltaTime) {
            this.HandleGravity(deltaTime);
            this.PerformBasicMovement(deltaTime);
            this.PerformBasicJumpCheck(deltaTime);
            this.PerformBasicFlapCheck(deltaTime);
            this.CorrectRotation(deltaTime);
            
            this.bodyPart?.PhysicsUpdate(deltaTime);
            this.headPart?.PhysicsUpdate(deltaTime);
            this.armPart?.PhysicsUpdate(deltaTime);
            this.legPart?.PhysicsUpdate(deltaTime);
        }


        protected void PerformBasicMovement(float deltaTime) {
            var usingDirection = this.moveDirection;

            var maxSpeed = this.compiledTraits.maxSpeedGround;
            var acceleration = this.compiledTraits.accelerationGround;
            var deceleration = this.compiledTraits.decelerationGround;
            if (this.waterInfo.isSwimming && !this.isOnGround) {
                maxSpeed = this.compiledTraits.maxSpeedWater; 
                acceleration = this.compiledTraits.accelerationWater;
                deceleration = this.compiledTraits.decelerationWater;
            } else if (!this.isOnGround) {
                maxSpeed = this.compiledTraits.maxSpeedAir; 
                acceleration = this.compiledTraits.accelerationAir;
                deceleration = this.compiledTraits.decelerationAir;
            }
            
            var usingAccel = Mathf.Approximately(this.moveInput.x, 0f) ? deceleration : acceleration;
            if (this.moveInput.x < -0.001f) {
                usingAccel *= 0.5f;
            }
            if (Mathf.Approximately(usingAccel, 0f)) {
                usingDirection.x = 0f;
                usingAccel = deceleration;
            }
            
            var targetGoalVelocity = usingDirection * maxSpeed;
            this.goalVelocity = Vector3.MoveTowards(this.goalVelocity, targetGoalVelocity, deltaTime * usingAccel);
            var accelNeeded = (this.goalVelocity - this.rb.linearVelocity) / deltaTime;
            this.rb.AddForce(Vector3.Scale(accelNeeded, new Vector3(1,0,1)));
        }
        protected void PerformBasicJumpCheck(float deltaTime) {
            if (this.jumpRecharge > 0) {
                this.jumpRecharge -= Time.deltaTime;
            }

            var wasJumping = this.isJumping;
            var inSpaceThatCanJump = (this.isOnGround || this.timeInAirLast < this.jumpSavingGrace) || this.waterInfo.isSwimming;
            
            if (!this.isJumping && this.doLegAction && inSpaceThatCanJump && this.jumpRecharge <= 0.0f) {
                this.isJumping = true;
                this.jumpTimer = this.compiledTraits.jumpPowerHold;
            }

            if (this.isJumping && !this.doLegAction) {
                this.isJumping = false;
            }

            if (this.isJumping) {
                var timeAmount = deltaTime;
                this.jumpTimer -= deltaTime;
                if (this.jumpTimer <= 0) {
                    this.isJumping = false;
                    timeAmount = deltaTime + this.jumpTimer;
                }

                var force = this.jumpCurve.Evaluate(this._compiledTraits.jumpPowerHold /
                                                    this._compiledTraits.jumpPowerHold); // curve in inversed fuck it
                force = force.Clamp01() * this.compiledTraits.jumpPower;
                this.rb.AddForce(Vector3.up * (force * timeAmount));
            }

            if (wasJumping && !this.isJumping) {
                this.jumpTimer = 0;
                this.jumpRecharge = 0.2f;
            }
        }
        
        protected void PerformBasicFlapCheck(float deltaTime) {
            if (this.flapRechargeTimer > 0) {
                this.flapRechargeTimer -= Time.deltaTime;
            }

            var wasFlapping = this.isFlapping;

            if (this.compiledTraits.enableFlapFlight && !this.waterInfo.isSwimming && !this.isFlapping && this.flapRechargeTimer <= 0 && this.doLegAction) {
                this.isFlapping = true;
                this.flapTimer = this.compiledTraits.flapDuration;
            }

            if (this.isFlapping && !this.doLegAction) {
                this.isFlapping = false;
            }
            if (this.isFlapping) {
                var timeAmount = deltaTime;
                this.flapTimer -= deltaTime;
                if (this.flapTimer <= 0) {
                    this.isFlapping = false;
                    timeAmount = deltaTime + this.flapTimer;
                }
                this.rb.AddForce(Vector3.up * (this.compiledTraits.flapFlightPower * timeAmount));
            }

            if (wasFlapping && !this.isFlapping) {
                this.flapTimer = 0;
                this.flapRechargeTimer = FLAP_RECHARGE;
            }
        }
        public void TakeDamage(DealDamage damage) {
            if (this.shouldLog) {
                Debug.Log("Took damage");
            }
            this.health -= damage.amount;
            if (this.health <= 0) {
                this.health = 0;
                this.TriggerDeath();
            }
        }
        
        [Button("Test death")]
        public void TriggerDeath() {
            CreatureManager.Instance.CreatureDidDie(this);
        }
        public void OnDeath() {
            this.health = 0;
            this.rb.isKinematic = true;
            this.rb.linearVelocity = Vector3.zero;
            this.rb.angularVelocity = Vector3.zero;
            for (var i = 0; i < this.agents.Count; i++) {
                this.agents[i].StopAI();
            }
        }
    }

    public struct DealDamage {
        public float amount;
        public Vector3 fromLocation;
        public DamageType damageType;
    }
    public enum DamageType {
        Direct,
        OutOfWater,
        InWater,
    }
}