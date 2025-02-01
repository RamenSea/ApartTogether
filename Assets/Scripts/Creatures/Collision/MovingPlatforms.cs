using System;
using RamenSea.Foundation3D.Extensions;
using UnityEngine;

namespace Creatures.Collision {
    public class MovingPlatforms: MonoBehaviour {
        public float timeSitAtStart;
        public float timeSitAtEnd;
        public float waitingTime;
        public float timer;
        public float timeToMove;

        public Vector3 originalPosition;
        public Vector3 moveTo;


        public bool isMovingTo;
        private void Start() {
            this.originalPosition = this.transform.localPosition;
        }

        private void Update() {
            if (this.waitingTime > 0) {
                this.waitingTime -= Time.deltaTime;
                if (this.waitingTime > 0) {
                    return;
                }
            }
            
            this.timer += Time.deltaTime;
            var fromDestination = this.originalPosition;
            var targetDestination = this.moveTo;
            if (isMovingTo) {
                fromDestination = this.moveTo;
                targetDestination = this.originalPosition;
            }
            this.transform.localPosition = Vector3.Lerp(fromDestination, targetDestination, this.timer / this.timeToMove);

            if (this.timer >= this.timeToMove) {
                this.isMovingTo = !this.isMovingTo;
                this.waitingTime = this.isMovingTo ? this.timeSitAtEnd : this.timeSitAtStart;
                this.timer = 0f;
            }
        }

    }
    
}