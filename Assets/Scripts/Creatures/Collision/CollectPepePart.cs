using System;
using Creatures.Parts;
using RamenSea.Foundation3D.Extensions;
using Systems;
using UnityEngine;

namespace Creatures.Collision {
    public class CollectPepePart: MonoBehaviour, ICreatureCollisionDetectionListener {
        public CreatureCollisionDetection creatureCollisionDetection;
        public GameObject visiblePartGO;
        public ParticleSystem particleSystem;
        public PartId pardId;
        public bool wasPickedUpThisSession;
        public PlayAudio audioPlay;

        private void Start() {
            this.creatureCollisionDetection.listener = this;
        }

        public void OnCreatureTriggerEnter(BaseCreature creature) {
            if (this.wasPickedUpThisSession) {
                return;
            }
            if (creature.isPlayer) {
                this.wasPickedUpThisSession = true;
                this.creatureCollisionDetection.enabled = false;
                this.particleSystem.gameObject.SetActive(true);
                this.particleSystem.Stop();
                this.particleSystem.Play();
                audioPlay.Play();
                visiblePartGO.SetActive(false);
                GameRunner.Instance.PickedUpPepePart(this.pardId);
            }
        }
    }
}