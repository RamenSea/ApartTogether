using System;
using Creatures;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

namespace Systems {
    public class RiverLevel: MonoBehaviour {
        
        public static RiverLevel Instance;
        
        
        public GameObject dam;
        public GameObject river;
        public ParticleSystem[] damExplosion;
        public SpawnInCreature[] riverSpawns;
        public float riverRiseTime;
        public Vector3 riverPosition;
        public GameObject forestSpawn;
        public GameObject forestPushButton;
        
        private void Awake() {
            Instance = this;
            for (var i = 0; i < this.damExplosion.Length; i++) {
                this.damExplosion[i].gameObject.SetActive(false);
            }
            
            this.riverPosition = this.river.transform.position;
            var p = this.riverPosition;
            p.y -= 3.94f;
            this.river.transform.position = p;
        }

        private void Start() {
            if (TheSystem.Get().save.hasUnDammedRiver == false) {
                for (var i = 0; i < this.riverSpawns.Length; i++) {
                    this.riverSpawns[i].Spawn();
                }
            }
        }

        private void OnDestroy() {
            if (Instance == this) {
                Instance = null;
            }
        }


        [Button("Test blow up dam")]
        public async void TestBlowUpDam() {
            this.UnDamRiver(false);
        }

        public async void UnDamRiver(bool instant) {
            this.dam.gameObject.SetActive(false);
            this.river.gameObject.SetActive(true);

            if (instant) {
                this.river.transform.position = this.riverPosition;
                this.forestSpawn.gameObject.SetActive(true);
                this.forestPushButton.gameObject.SetActive(false);
                return;
            }
            
            for (var i = 0; i < this.damExplosion.Length; i++) {
                this.damExplosion[i].gameObject.SetActive(true);
                this.damExplosion[i].Stop();
                this.damExplosion[i].Play();
            }
            
            this.river.transform.DOMove(this.riverPosition, this.riverRiseTime);
            
            await UniTask.Delay(TimeSpan.FromSeconds(0.3f));
            this.forestSpawn.gameObject.SetActive(true);
            this.forestPushButton.gameObject.SetActive(false);
        }
    }
}