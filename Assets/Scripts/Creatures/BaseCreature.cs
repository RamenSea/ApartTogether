using System;
using System.Collections.Generic;
using Creatures.Parts;
using JetBrains.Annotations;
using Player;
using RamenSea.Foundation.Extensions;
using RamenSea.Foundation3D.Extensions;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations.Rigging;


namespace Creatures {
    public interface CreatureInterface {
        public Rigidbody rb { get; }
        public CreatureTraits compiledTraits { get; }
        [CanBeNull] public BaseBodyPart bodyPart { get; }
        [CanBeNull] public BaseLegPart legPart { get; }
        [CanBeNull] public BaseHeadPart headPart { get; }
        [CanBeNull] public BaseArmPart armPart { get; }
        public bool isOnGround { get; }
        public bool tryJumping { get; }
        public Vector3 moveDirection { get; }
    }
    
    public class BaseCreature: MonoBehaviour, CreatureInterface {
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
        
        protected CreatureTraits _compiledTraits;
        public Rigidbody rb { get; set; }
        public RigBuilder rigBuilder { get; set; }
        public CreatureTraits compiledTraits => this._compiledTraits;
        public BaseBodyPart bodyPart { get; set; }
        public BaseLegPart legPart => this.bodyPart?.attachedLegPart;
        public BaseHeadPart headPart => this.bodyPart?.attachedHeadPart;
        public BaseArmPart armPart => this.bodyPart?.attachedArmsPart;
        
        [SerializeField] protected float jumpRecharge;

        public Vector3 moveDirection { get; set; }
        public bool tryJumping { get; set; }
        public bool isOnGround { get; set; }

        [NonSerialized] public bool isPlayer = false;
        [NonSerialized] private float currentRotation;

        
        private void Start() {
            this.rb = GetComponent<Rigidbody>();
            this.rigBuilder = GetComponent<RigBuilder>();
        }

        private void Update() {
            Debug.DrawLine(this.transform.position, this.transform.position + (this.compiledTraits.height * Vector3.down), Color.red);

            if (this.jumpRecharge > 0) {
                this.jumpRecharge -= Time.deltaTime;
            }
            if (this.tryJumping && this.isOnGround && this.jumpRecharge <= 0.0f) {
                this.jumpRecharge = 0.2f;
                this.rb.AddForce(Vector3.up * this.compiledTraits.jumpPower);
            }
            
            this.CorrectRotation(Time.deltaTime);

        }

        public void SetCreaturePart(BaseCreaturePart bodyPart) {
            bodyPart.creatureInterface = this;
        }
        public void FinishSettingParts(bool resetCounters) {
            this._compiledTraits = CreatureTraitHelper.CreateTraits(this.isPlayer, this);
            
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
        }
        private void FixedUpdate() {
            this.PhysicsUpdate(Time.fixedDeltaTime);
        }

        public void HandleGravity(float deltaTime) {
            var usingDown = Vector3.down;
            RaycastHit hit;
            if (Physics.Raycast(this.transform.position, transform.TransformDirection(usingDown), out hit, this.compiledTraits.height)) {
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
        }

        public void CorrectRotation(float deltaTime) {
            // var upRightRotation = Quaternion.Euler(new Vector3(0,this.inputController.moveInput.Angle(),0)); //todo
            // var upRightRotation = Quaternion.identity; //todo
            // var toGoal = BaseCreature.ShortestRotation(upRightRotation, this.transform.rotation);
            // print(toGoal.eulerAngles.magnitude);
            //
            // Vector3 rotAxis;
            // float rotDegree;
            // toGoal.ToAngleAxis(out rotDegree, out rotAxis);
            // rotAxis.Normalize();
            //
            // float rotRadians = rotDegree * Mathf.Deg2Rad;
            //
            // this.rb.AddTorque((rotAxis * (rotRadians * this.compiledTraits.uprightSpringStrength)) - (this.rb.angularVelocity * this.compiledTraits.uprightSpringDamper));
            
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

            this.bodyPart?.PhysicsUpdate(deltaTime);
            this.headPart?.PhysicsUpdate(deltaTime);
            this.armPart?.PhysicsUpdate(deltaTime);
            this.legPart?.PhysicsUpdate(deltaTime);
        }
    }
}