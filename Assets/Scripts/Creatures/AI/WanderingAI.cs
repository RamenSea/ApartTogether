using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Creatures.AI {
    public class WanderingAI: BaseAIAgent {

        public float minWalkTime = 1f;
        public float maxWalkTime = 1f;
        public float minSitTime = 1f;
        public float maxSitTime = 1f;

        public Vector3 direction;
        public float wanderingTime = 0f;
        public bool isWandering = true;
        private void Start() {
        }

        private void Update() {
            if (!this.hasStarted) {
                return;
            }
            if (this.creature == null || this.creature.isDead) {
                return;
            }

            this.wanderingTime -= Time.deltaTime;
            if (this.wanderingTime <= 0) {
                this.isWandering = !this.isWandering;

                if (this.isWandering) {
                    this.direction = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
                    this.creature.moveDirection = this.direction;
                    this.wanderingTime = Random.Range(this.minWalkTime, this.maxWalkTime);
                } else {
                    this.creature.moveDirection = Vector3.zero;
                    this.wanderingTime = Random.Range(this.minSitTime, this.maxSitTime);
                }
            }
        }
    }
}