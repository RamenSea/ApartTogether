using System;
using UnityEngine;


namespace Creatures {
    [Serializable]
    public struct CreatureTraits {
        public float maxSpeed;
        public float acceleration;
        public float height;
        public float heightSpringForce;
        public float heightSpringDamper;
    }
    public class BaseCreature: MonoBehaviour {
        [SerializeField] protected Rigidbody rb;
        [SerializeField] protected CreatureTraits compiledTraits;

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
                
                Debug.DrawLine(this.transform.position, this.transform.position + (rayDir * springForce), Color.green);
                this.rb.AddForce(rayDir * springForce);
            }
        }
        public void PhysicsUpdate(float deltaTime) {
            this.HandleGravity(deltaTime);
            var direction = new Vector2(0.5f, 0.5f);
        }
    }
}