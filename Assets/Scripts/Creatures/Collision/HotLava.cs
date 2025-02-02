using System;
using UnityEngine;

namespace Creatures.Collision {

    public class HotLava: MonoBehaviour, ICreatureCollisionDetectionListener {
        public CreatureCollisionDetection detection;

        private void Start() {
            this.detection.listener = this;
        }

        public void OnCreatureTriggerEnter(BaseCreature creature) {
            if (creature != null && !creature.isDead) {
                creature.TakeDamage(new DealDamage() {
                    amount = 1000000,
                    damageType = DamageType.Direct,
                    fromLocation = Vector3.zero
                });
            }
        }
    }
}