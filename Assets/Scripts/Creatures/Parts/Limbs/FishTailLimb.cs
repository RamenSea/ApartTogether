using System;
using UnityEngine;

namespace Creatures.Parts.Limbs {
    public class FishTailLimb: BaseLimb {
        public Transform tailPivot;

        public float rotationAmount;
        public float rotationSpeed;

        public bool rotatingDirection = false;
        public float rotationCount = 0f;


        public override void OnAttachToBody(BaseBodyPart bodyPart, LimbAttachPoint toPoint) {
            base.OnAttachToBody(bodyPart, toPoint);
            this.rotationCount = 0f;
            this.rotatingDirection = false;
        }

        private void Update() {

            var localEulerAngles = this.tailPivot.localEulerAngles;
            if (this.rotationCount > this.rotationAmount) {
                this.rotatingDirection = false;
            } else if (this.rotationCount < -this.rotationAmount) {
                this.rotatingDirection = true;
            } 

            if (rotatingDirection) {
                localEulerAngles.y += rotationSpeed * Time.deltaTime;
                this.rotationCount += rotationSpeed * Time.deltaTime;
            } else {
                localEulerAngles.y -= rotationSpeed * Time.deltaTime;
                this.rotationCount -= rotationSpeed * Time.deltaTime;
            }
            this.tailPivot.localEulerAngles = localEulerAngles;
        }
    }
}