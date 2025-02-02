using System;
using Creatures.Collision;
using UnityEngine;

namespace Creatures.AI {
    public class AreaTriggerAI: BaseAIAgent, ICreatureCollisionDetectionListener {
        public CreatureCollisionDetection collisionDetection;

        private void Start() {
            this.collisionDetection.listener = this;
        }

        public override void PostStart() {
            base.PostStart();
            this.collisionDetection.gameObject.SetActive(true);
            for (var i = 0; i < this.creature.agents.Count; i++) {
                this.creature.agents[i].hasStarted = false;
            }
        }

        public void TriggerStart() {
            for (var i = 0; i < this.creature.agents.Count; i++) {
                Debug.Log($"{this.creature.agents[i].name} is asdf");
                this.creature.agents[i].hasStarted = true;
            }
        }
        public void OnCreatureTriggerEnter(BaseCreature creature) {
            if (creature.isPlayer && !this.creature.isDead && this.hasStarted == false) {
                this.TriggerStart();
            }
        }
    }
}