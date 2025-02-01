using System;
using RamenSea.Foundation3D.Extensions;
using UnityEngine;

namespace Creatures.Collision {
    public class MovingPlatforms: MonoBehaviour {
        public Vector3 moveBy;
        public float speed;
        
        public Vector3 originalPosition;


        public bool isMovingTo;
        private void Start() {
            this.originalPosition = this.transform.localPosition;
        }

        private void Update() {

            var targetDestination = this.originalPosition;
            if (isMovingTo) {
                targetDestination += this.moveBy;
            }
            this.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, targetDestination, this.speed * Time.deltaTime);

            if (this.transform.localPosition.Distance(targetDestination) <= 0.0001f) {
                this.isMovingTo = !this.isMovingTo;
            }
        }
    }
}