using System;
using RamenSea.Foundation3D.Extensions;
using UnityEngine;

namespace Creatures.AI {
    public class AnnoyingBirdAI: BaseAIAgent {

        public Transform[] path;
        
        public Transform target;
        public float moveSpeed = 1;
        public float turnSpeed = 60;
        public override void StartAI() {
            base.StartAI();
            
        }

        private void FixedUpdate() {
            if (!this.hasStarted || this.creature == null || this.creature.isDead) {
                return;
            }
            if (this.target == null) {
                this.SetNextTarget();
            }
            var distance = this.creature.rb.position.Distance(this.target.position);
            if (distance <= 0.001f) {
                this.SetNextTarget();
            }

            this.creature.doLegAction = true;
            var direction = this.creature.rb.position.Direction(this.target.position);
            var position = Vector3.MoveTowards(this.creature.rb.position, this.target.position, moveSpeed * Time.fixedDeltaTime);
            this.creature.rb.MovePosition(position);
            this.creature.rb.MoveRotation(Quaternion.RotateTowards(this.creature.rb.rotation, Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z)), this.turnSpeed * Time.fixedDeltaTime));
        }

        public void SetNextTarget() {
            if (this.target == null) {
                this.target = this.path[0];
            } else {
                var index = 0;
                for (int i = 0; i < this.path.Length; i++) {
                    if (this.path[i] == this.target) {
                        index = i + 1;
                        break;
                    }
                }
                if (index < this.path.Length) {
                    this.target = this.path[index];
                } else {
                    this.target = this.path[0];
                }
            }
        }
        public override void PostStart() {
            base.PostStart();

            var t = this.creature.compiledTraits;
            var traits = new CreatureTraits();
            traits.effectsGravity = 0f;
            traits.flapFlightPower = -t.flapFlightPower;
            traits.jumpPower = -t.jumpPower;
            traits.flapDuration = -t.flapDuration + 1f;
            this.creature.additionalTraits = traits;
            this.creature.FinishSettingParts(false);
        }
    }
}