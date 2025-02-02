using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Creatures.AI {
    public class SittingJumpingAI: BaseAIAgent {

        public float jumpWaitTimer = 0f;
        public float jumpHoldTimer = 0f;
        public float timeTilWaitToJump = 0f;
        public float timeTilWaitToJumpVariation = 0f;
        public float timeToStart = 0f;
        public float timeToStartVariation = 0f;
        public float timeToHoldJump = 0f;
        public float timeToHoldJumpVariation = 0f;
        public bool listenForGround = true;

        private void Start() {
            this.jumpWaitTimer = this.timeToStart + Random.Range(0f, this.timeToStartVariation);
        }

        private void Update() {
            if (!this.hasStarted) {
                return;
            }
            if (this.creature == null || this.creature.isDead) {
                return;
            }

            if (this.jumpWaitTimer > 0f) {
                this.jumpWaitTimer -= Time.deltaTime;
                
                this.creature.doLegAction = false;
                if (this.jumpWaitTimer > 0f) {
                    return;
                }

                this.jumpHoldTimer = this.timeToHoldJump + Random.Range(0f, this.timeToHoldJumpVariation);
            }

            this.jumpHoldTimer -= Time.deltaTime;
            if (this.jumpHoldTimer >= 0f) {
                this.creature.doLegAction = true;
            } else if (this.listenForGround && this.creature.isOnGround) {
                this.jumpWaitTimer = this.timeTilWaitToJump + Random.Range(0f, this.timeTilWaitToJumpVariation);
            } else if (!this.listenForGround) {
                this.jumpWaitTimer = this.timeTilWaitToJump + Random.Range(0f, this.timeTilWaitToJumpVariation);
            }
        }
    }
}