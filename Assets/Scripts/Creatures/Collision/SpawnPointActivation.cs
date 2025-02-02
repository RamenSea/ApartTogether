using System;
using Systems;
using UnityEngine;

namespace Creatures.Collision {
    public class SpawnPointActivation: MonoBehaviour, ICreatureCollisionDetectionListener {
        public string spawnPointId;

        public ParticleSystem spawnAnimation;
        public ParticleSystem spawnUpdateAnimation;
        public ParticleSystem spawnCanSetAnimation;
        public Transform spawnPoint;
        public CreatureCollisionDetection detection;
        public MeshRenderer spawnRenderer;
        public Material setMaterial;
        public Material notSetMaterial;

        private void Start() {
            this.detection.listener = this;
            this.spawnRenderer.materials[1] = this.notSetMaterial;
            this.spawnCanSetAnimation.gameObject.SetActive(false);
            this.spawnCanSetAnimation.Play();
        }

        public void SetActivation(bool isActive) {
            this.spawnRenderer.materials[1] = isActive ? this.setMaterial : this.notSetMaterial;

            if (!isActive) {
                this.spawnCanSetAnimation.gameObject.SetActive(false);
                this.spawnCanSetAnimation.Play();
            }
        }

        private void OnTriggerEnter(Collider other) {
            this.spawnUpdateAnimation.gameObject.SetActive(true);
            this.spawnUpdateAnimation.Stop();
            this.spawnUpdateAnimation.Play();
            this.spawnCanSetAnimation.gameObject.SetActive(false);
            GameRunner.Instance.UpdateSpawnPoint(this.spawnPointId);
        }
    }
}