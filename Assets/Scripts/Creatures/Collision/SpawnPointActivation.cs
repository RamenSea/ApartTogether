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

        public bool isActiveSpawn = false;
        private void Start() {
            this.detection.listener = this;
            this.spawnRenderer.materials[1] = this.notSetMaterial;
            this.spawnRenderer.materials = this.spawnRenderer.materials;
            this.spawnCanSetAnimation.gameObject.SetActive(false);
            this.spawnCanSetAnimation.Play();
        }

        public void SetSpawnActive(bool isActive) {
            isActiveSpawn = isActive;
            this.spawnRenderer.materials[1] = isActive ? this.setMaterial : this.notSetMaterial;

            this.spawnRenderer.materials = this.spawnRenderer.materials;
            if (!isActive) {
                this.spawnCanSetAnimation.gameObject.SetActive(true);
                this.spawnCanSetAnimation.Play();
            }
        }

        public void OnCreatureTriggerEnter(BaseCreature creature) {
            Debug.Log(this.isActiveSpawn);
            if (this.isActiveSpawn) {
                return;
            }
            this.spawnUpdateAnimation.gameObject.SetActive(true);
            this.spawnUpdateAnimation.Stop();
            this.spawnUpdateAnimation.Play();
            this.spawnCanSetAnimation.gameObject.SetActive(false);
            this.spawnCanSetAnimation.Stop();
            GameRunner.Instance.UpdateSpawnPoint(this.spawnPointId);
        }
    }
}