using System;
using System.Collections.Generic;
using Creatures.Parts.Limbs;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Creatures.AI {
    public class ShootingAI: BaseAIAgent {
        public float timeTilTryToShoot = 0f;
        public float timer = 0f;
        public float timerHoldShooting = 0.1f;
        public float holdShootingFor = 0.1f;

        public List<ProjectileLimb> limbs;
        public override void StartAI() {
            base.StartAI();

            this.limbs = new();
            for (var i = 0; i < this.creature.attachedLimbs.Count; i++) {
                if (this.creature.attachedLimbs[i] is ProjectileLimb l) {
                    this.limbs.Add(l);
                }
            }
        }

        public bool ShouldFire() {
            for (var i = 0; i < this.limbs.Count; i++) {
                if (this.limbs[i].HasTarget()) {
                    return true;
                }
            }

            return false;
        }

        private void Update() {
            if (!this.hasStarted) {
                return;
            }
            if (this.creature == null || this.creature.isDead) {
                return;
            }
            
            this.creature.doArmsAction = false;
            this.timer -= Time.deltaTime;
            if (this.timerHoldShooting > 0f) {
                this.timerHoldShooting -= Time.deltaTime;
                this.creature.doArmsAction = this.ShouldFire();
                if (this.timerHoldShooting <= 0) {
                    this.timer = this.timeTilTryToShoot;
                    this.creature.doArmsAction = false;
                }
            } else if (this.timer <= 0f) {
                this.timerHoldShooting = this.holdShootingFor;
                this.creature.doArmsAction = this.ShouldFire();
            }
        }
    }
}