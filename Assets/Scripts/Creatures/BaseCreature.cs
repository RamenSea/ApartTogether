using System;
using Player;
using UnityEngine;


namespace Creatures {
    [Serializable]
    public struct CreatureTraits {
        public float maxSpeed;
        public float acceleration;
        public float moveForceScale;
        public float height;
        public float heightSpringForce;
        public float heightSpringDamper;
    }
    public class BaseCreature: MonoBehaviour {
        [SerializeField] protected Rigidbody rb;
        [SerializeField] protected CreatureTraits compiledTraits;
        [SerializeField] protected PlayerInputController inputController;
        
        [SerializeField] protected Vector3 goalVelocity;

        
        private void Update() {
            Debug.DrawLine(this.transform.position, this.transform.position + (this.compiledTraits.height * Vector3.down), Color.red);
        }

        private void FixedUpdate() {
            this.PhysicsUpdate(Time.fixedDeltaTime);
        }

        public void HandleGravity(float deltaTime) {
            RaycastHit hit;
            if (Physics.Raycast(this.transform.position, transform.TransformDirection(Vector3.down), out hit, this.compiledTraits.height)) {
                
                var downDir = Vector3.down;
                var velocity = rb.linearVelocity;
                var rayDir = this.transform.TransformDirection(downDir);

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
                
            }
        }

        public void HandleMove(float deltaTime) {
            var inputDirection = this.inputController.moveInput;
            var worldDirection = new Vector3(inputDirection.x, 0, inputDirection.y);
            var targetGoalVelocity = worldDirection * this.compiledTraits.maxSpeed;
            this.goalVelocity = Vector3.MoveTowards(this.goalVelocity, targetGoalVelocity, deltaTime * this.compiledTraits.acceleration);
            
            var accelNeeded = (this.goalVelocity - this.rb.linearVelocity) / deltaTime;
            this.rb.AddForce(Vector3.Scale(accelNeeded, new Vector3(1,0,1)));
        }
        public void PhysicsUpdate(float deltaTime) {
            this.HandleGravity(deltaTime);
            this.HandleMove(deltaTime);
        }
    }
}