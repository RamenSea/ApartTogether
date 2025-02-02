using Player;
using RamenSea.Foundation3D.Extensions;
using UnityEngine;

namespace Creatures.AI {
    public class ChasePlayerAI: BaseAIAgent {
        public float range;
        public bool isChasing = false;
        private void Update() {
            if (!this.hasStarted) {
                return;
            }
            if (this.creature == null || this.creature.isDead || PlayerDriverController.Instance.creature == null || PlayerDriverController.Instance.creature.isDead) {
                return;
            }

            var distance
                = this.creature.transform.position.Distance(PlayerDriverController.Instance.creature.transform.position);
            var shouldChase = distance <= range;

            if (this.isChasing != shouldChase) {
                this.isChasing = shouldChase;
                for (var i = 0; i < this.creature.agents.Count; i++) {
                    this.creature.agents[i].chasingAIIsRunning = this.isChasing;
                }
            }
            if (this.isChasing) {
                var directionToPlayer
                    = this.creature.transform.position.Direction(PlayerDriverController.Instance.creature.transform.position);
                this.creature.moveDirection = directionToPlayer;
            } else {
                this.creature.moveDirection = Vector3.zero;
            }
        }
    }
}