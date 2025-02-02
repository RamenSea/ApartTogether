using Player;
using RamenSea.Foundation3D.Extensions;
using UnityEngine;

namespace Creatures.AI {
    public class ChasePlayerAI: BaseAIAgent {
        public float range;
        private void Update() {
            if (!this.hasStarted) {
                return;
            }
            if (this.creature == null || this.creature.isDead) {
                return;
            }

            var distance
                = this.creature.transform.position.Distance(PlayerDriverController.Instance.creature.transform.position);
            if (distance <= this.range) {
                var directionToPlayer
                    = this.creature.transform.position.Direction(PlayerDriverController.Instance.creature.transform.position);
                this.creature.moveDirection = directionToPlayer;
            } else {
                this.creature.moveDirection = Vector3.zero;
            }
        }
    }
}