using System;
using Creatures.Collision;
using Creatures.Parts;
using JetBrains.Annotations;
using NaughtyAttributes;
using Player;
using RamenSea.Foundation3D.Extensions;
using Systems;
using UnityEngine;
using UnityEngine.Animations.Rigging;


namespace Creatures {
    
    public class BaseCreature: MonoBehaviour {
        public const int MAX_HEALTH = 10_000;
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
        public BaseBodyPart bodyPart { get; set; }
        public BaseCreaturePart legPart => this.bodyPart?.attachedLegPart;
        public BaseCreaturePart headPart => this.bodyPart?.attachedHeadPart;
        public BaseCreaturePart armPart => this.bodyPart?.attachedArmsPart;

        public WaterInfo waterInfo;
        
        /*
         * States
         */
        public Vector3 moveDirection;
        public bool isOnGround;
        public bool landedOnGroundThisFrame;
        public float timeInAirLast;
        public float timeOutOfWaterLast;
        public bool doLegAction;
        public bool doHeadAction;
        public bool doArmsAction;
        public float health;

        [NonSerialized] public bool isPlayer = false;
        [NonSerialized] private float currentRotation;
        [SerializeField] protected Vector3 goalVelocity;
        [SerializeField] protected float jumpRecharge = 0;
        [SerializeField] protected float flapRechargeTimer = 0;
        [SerializeField] protected float flapTimer = 0;
        [SerializeField] protected bool isFlapping = false;
        [SerializeField] protected bool isJumping = false;
        [SerializeField] protected float jumpTimer = 0f;

        public Vector3 gravity;
        
        private bool wasOnGroundLastFrame = false;
        private void Update() {
            Debug.DrawLine(this.transform.position, this.transform.position + (this.compiledTraits.height * Vector3.down), Color.red);

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
            } else if (!this.waterInfo.isSwimming && (this.timeOutOfWaterLast > 1f|| this.isOnGround) && this.compiledTraits.outOfWaterDamage > 0.001f) {
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
            creaturePart.creature = this;
            
            switch (creaturePart.slotType) {
                case PartSlotType.Body: {
                    var oldBody = this.bodyPart;
                    this.bodyPart = creaturePart as BaseBodyPart;
                    this.bodyPart.transform.SetParent(this.transform);
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

                    if (this.isPlayer) {
                        PlayerDriverController.Instance.cameraController.virtualCamera.Follow = this.bodyPart.followPoint;
                    }
                    if (oldBody != null) {
                        CreatureManager.Instance.DropPart(oldBody);
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
                        CreatureManager.Instance.DropPart(oldBodyPart);
                    }
                    this.bodyPart.AttachPart(creaturePart);
                    break;
                }
            }
        }
        public void FinishSettingParts(bool resetCounters) {
            this._compiledTraits = CreatureTraitHelper.CreateTraits(this.isPlayer, this);

            this.rb.mass = this.compiledTraits.weight;
            
            // build rig graph
            this.rigBuilder.layers.Clear();
            if (this.bodyPart != null) {
                for (var i = 0; i < this.bodyPart.limbs.Length; i++) {
                    for (var limbIndex = 0; limbIndex < this.bodyPart.limbs[i].rigs.Length; limbIndex++) {
                        this.rigBuilder.layers.Add(new RigLayer(this.bodyPart.limbs[i].rigs[limbIndex]));
                    }
                }
            }
            if (this.legPart != null) {
                for (var i = 0; i < this.legPart.limbs.Length; i++) {
                    for (var limbIndex = 0; limbIndex < this.legPart.limbs[i].rigs.Length; limbIndex++) {
                        this.rigBuilder.layers.Add(new RigLayer(this.legPart.limbs[i].rigs[limbIndex]));
                    }
                }
            }
            if (this.armPart != null) {
                for (var i = 0; i < this.armPart.limbs.Length; i++) {
                    for (var limbIndex = 0; limbIndex < this.armPart.limbs[i].rigs.Length; limbIndex++) {
                        this.rigBuilder.layers.Add(new RigLayer(this.armPart.limbs[i].rigs[limbIndex]));
                    }
                }
            }
            if (this.headPart != null) {
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
                this.jumpTimer = 0f;
                this.health = MAX_HEALTH;
            }
            
            this.waterInfo.SetColliders(this);
        }
        private void FixedUpdate() {
            this.PhysicsUpdate(Time.fixedDeltaTime);
        }
        public void HandleGravity(float deltaTime) {
            var wasOnGround = this.isOnGround;
            var usingDown = Vector3.down;
            RaycastHit hit;
            if (Physics.Raycast(this.transform.position, transform.TransformDirection(usingDown), out hit, this.compiledTraits.height, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore)) {
                this.isOnGround = true;
                var velocity = rb.linearVelocity;
                var rayDir = usingDown;
                // var downDir = Vector3.down;
                // var rayDir = this.transform.TransformDirection(downDir);

                var otherVel = Vector3.zero;
                Rigidbody hitBody = hit.rigidbody;
                if (hitBody) {
                    otherVel = hitBody.linearVelocity;
                }
                
                var rayDirVel = Vector3.Dot(rayDir, velocity);
                var otherDirVel = Vector3.Dot(rayDir, otherVel);
                var relVel = rayDirVel - otherDirVel;
                var x = hit.distance - this.compiledTraits.height;
                var springForce = (x * this.compiledTraits.heightSpringForce) -
                    (relVel * this.compiledTraits.heightSpringDamper);
                
                this.rb.AddForce(rayDir * springForce);
            } else {
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
            // var rot = Quaternion.FromToRotation(transform.up, Vector3.up);
            // rb.AddTorque(new Vector3(rot.x, rot.y, rot.z) * (this.compiledTraits.uprightSpringStrength * deltaTime));
            // var moveDirectionAngle = new Vector2(this.moveDirection.x, this.moveDirection.z);
            // var upRightRotation = Quaternion.Euler(new Vector3(0, moveDirectionAngle.Angle(true), 0)); //todo
            // // var upRightRotation = Quaternion.identity; //todo
            // var toGoal = BaseCreature.ShortestRotation(upRightRotation, this.rb.rotation);
            // //
            // Vector3 rotAxis;
            // float rotDegree;
            // toGoal.ToAngleAxis(out rotDegree, out rotAxis);
            // rotAxis.Normalize();
            //
            // float rotRadians = rotDegree * Mathf.Deg2Rad;
            //
            // this.rb.AddTorque(
            //     (rotAxis * (rotRadians *
            //         this.compiledTraits
            //             .uprightSpringStrength)) - (this.rb.angularVelocity * this.compiledTraits.uprightSpringDamper));
            //
            var currentVelocity = this.rb.linearVelocity;
            var currentVelocityVector2 = new Vector2(currentVelocity.x, currentVelocity.z);
            var speed = currentVelocityVector2.magnitude;
            if (speed > 0.01f) {
                var targetAngle = currentVelocityVector2.normalized;
                var rotationSpeed = speed * this.compiledTraits.rotationSpeedDampener;
                // rotationSpeed = Mathf.Max(rotationSpeed, );
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(new Vector3(currentVelocityVector2.x, 0, currentVelocityVector2.y)), this.compiledTraits.rotationSpeedMin * deltaTime);
            }
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
            
            var usingAccel = Mathf.Approximately(usingDirection.x, 0f) ? deceleration : acceleration;
            if (usingDirection.x < -0.001f) {
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
            var inSpaceThatCanJump = this.isOnGround || this.waterInfo.isSwimming;
            
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
                this.rb.AddForce(Vector3.up * (this.compiledTraits.jumpPower * timeAmount));
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
                this.flapRechargeTimer = 0.2f;
            }
        }
        public void TakeDamage(DealDamage damage) {
            Debug.Log("Took damage");
            this.health -= damage.amount;
            if (this.health <= 0) {
                this.health = 0;
                this.Die();
            }
        }
        
        [Button("Test death")]
        public void Die() {
            Debug.Log("Died");
            CreatureManager.Instance.CreatureDidDie(this);
        }
    }

    public struct DealDamage {
        public float amount;
        public DamageType damageType;
    }
    public enum DamageType {
        Direct,
        OutOfWater,
        InWater,
    }
}