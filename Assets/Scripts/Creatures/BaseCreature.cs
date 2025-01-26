using System;
using Player;
using UnityEditor;
using UnityEngine;


namespace Creatures {
    
    [Serializable]
    public struct CreatureTraits {
        public float maxSpeed;
        public float acceleration;
        public float height;
        public float heightSpringForce;
        public float heightSpringDamper;
        public float uprightSpringStrength;
        public float uprightSpringDamper;
        public float jumpPower;
    }
    public class BaseCreature: MonoBehaviour {
        public static Quaternion ShortestRotation(Quaternion to, Quaternion from)
        {
            if (Quaternion.Dot(to, from) < 0)
            {
                return to * Quaternion.Inverse(Multiply(from, -1));
            }

            else return to * Quaternion.Inverse(from);
        }



        public static Quaternion Multiply(Quaternion input, float scalar)
        {
            return new Quaternion(input.x * scalar, input.y * scalar, input.z * scalar, input.w * scalar);
        }
        
        [SerializeField] protected Rigidbody rb;
        [SerializeField] protected CreatureTraits compiledTraits;
        [SerializeField] protected PlayerInputController inputController;
        
        [SerializeField] protected Vector3 goalVelocity;
        [SerializeField] protected bool isOnGround;
        [SerializeField] protected float jumpRecharge;

        
        private void Update() {
            Debug.DrawLine(this.transform.position, this.transform.position + (this.compiledTraits.height * Vector3.down), Color.red);

            if (this.jumpRecharge > 0) {
                this.jumpRecharge -= Time.deltaTime;
            }
            if (this.inputController.didPressJump && this.isOnGround && this.jumpRecharge <= 0.0f) {
                this.jumpRecharge = 0.2f;
                this.rb.AddForce(Vector3.up * this.compiledTraits.jumpPower);
            }
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
            var upRightRotation = Quaternion.identity; //todo
            var toGoal = BaseCreature.ShortestRotation(upRightRotation, this.transform.rotation);

            Vector3 rotAxis;
            float rotDegree;
            toGoal.ToAngleAxis(out rotDegree, out rotAxis);
            rotAxis.Normalize();
            
            float rotRadians = rotDegree * Mathf.Deg2Rad;
            
            this.rb.AddTorque((rotAxis * (rotRadians * this.compiledTraits.uprightSpringStrength)) - (this.rb.angularVelocity * this.compiledTraits.uprightSpringDamper));
        }
        public void HandleMove(float deltaTime) {
            // if (!this.isOnGround) {
            //     return;
            // }
            var inputDirection = this.inputController.moveInput;
            var worldDirection = new Vector3(inputDirection.x, 0, inputDirection.y);
            var targetGoalVelocity = worldDirection * this.compiledTraits.maxSpeed;
            this.goalVelocity = Vector3.MoveTowards(this.goalVelocity, targetGoalVelocity, deltaTime * this.compiledTraits.acceleration);
            
            var accelNeeded = (this.goalVelocity - this.rb.linearVelocity) / deltaTime;
            this.rb.AddForce(Vector3.Scale(accelNeeded, new Vector3(1,0,1)));
        }
        public void PhysicsUpdate(float deltaTime) {
            this.HandleGravity(deltaTime);
            this.CorrectRotation(deltaTime);
            this.HandleMove(deltaTime);
        }
    }
}