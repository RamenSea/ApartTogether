using System;
using Player;
using RamenSea.Foundation3D.Extensions;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Creatures.AI {
    public class RotateTowardsPlayerAI: BaseAIAgent {

        public float distance;
        private void Start() {
        }

        private void Update() {
            if (!this.hasStarted) {
                return;
            }
            if (this.creature == null || this.creature.isDead || PlayerDriverController.Instance.creature == null || PlayerDriverController.Instance.creature.isDead) {
                return;
            }

            if (this.chasingAIIsRunning) {
                return;
            }

            var shouldRotateTowardsPlayer = this.creature.transform.position.Distance(PlayerDriverController.Instance.creature.transform.position) <= this.distance;
            if (shouldRotateTowardsPlayer) {
                var towardPlayer = this.creature.transform.position.Direction(PlayerDriverController.Instance.creature.transform.position);
                this.creature.rb.MoveRotation(Quaternion.Slerp(this.creature.rb.rotation, Quaternion.LookRotation(new Vector3(towardPlayer.x, 0, towardPlayer.z)), this.creature.compiledTraits.rotationSpeedMin * Time.deltaTime));
            }
        }
    }
}