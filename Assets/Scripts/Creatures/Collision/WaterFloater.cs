using System;
using UnityEngine;

namespace Creatures.Collision {
    public class WaterFloater: MonoBehaviour {
        public WaterInfo waterInfo;
        public Vector3 swimmingGravity;
        public Vector3 touchingGravity;
        public Rigidbody rb;

        public Vector3 gravity;
        public float gravityReduce;
        private void Update() {

            if (this.waterInfo.isSwimming) {
                this.gravity = this.swimmingGravity;
            } else if (this.waterInfo.isTouchingWater) {
                this.gravity = this.touchingGravity;
            } else {
                this.gravity = Physics.gravity * this.gravityReduce;
            }
        }

        private void FixedUpdate() {
            this.rb.AddForce(this.gravity * Time.fixedDeltaTime, ForceMode.VelocityChange);
        }
    }
}