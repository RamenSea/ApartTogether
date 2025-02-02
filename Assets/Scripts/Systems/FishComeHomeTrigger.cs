using System;
using Creatures;
using Creatures.Collision;
using Creatures.Parts;
using Player;
using UnityEngine;

namespace Systems {
    public class FishComeHomeTrigger: MonoBehaviour, ICreatureCollisionDetectionListener {

        public GameObject pepePart;
        public CreatureCollisionDetection collisionDetection;
        private void Start() {
            this.collisionDetection.listener = this;
        }


        public void OnCreatureTriggerEnter(BaseCreature creature) {
            if (creature.isPlayer && this.pepePart != null) {
                if (creature.bodyPart.partId == PartId.FishBody && creature.headPart.partId == PartId.FishHead &&
                    creature.legPart.partId == PartId.FishLegs && !TheSystem.Get().save.hasCollectedPepeBody) {
                    pepePart.gameObject.SetActive(true);
                }
            }
        }
    }
}