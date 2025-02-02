using System;
using Creatures;
using Creatures.Collision;
using Creatures.Parts;
using NaughtyAttributes;
using Player;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Systems {
    public class GameRunner: MonoBehaviour {
        public static GameRunner Instance { get; private set; }
        public DeathUI deathUI;
        public StartGameUI startUI;
        public MainGameUI gameUI;

        public RiverLevel riverLevel;
        public Transform pepeLegsLookAtLocation;
        public GameObject pepeLegsCollection;
        public Transform pepeBodyLookAtLocation;
        public GameObject pepeBodyCollection;
        public Transform pepeHeadLookAtLocation;
        public GameObject pepeHeadCollection;
        public Transform pepeWingsLookAtLocation;
        public GameObject pepeWingsCollection;
        public SpawnPointActivation[] spawns;
        public SpawnInCreature annoyingBirdSpawn;
        public BaseCreature annoyingBird;

        private void Awake() {
            GameRunner.Instance = this;
            var save = TheSystem.Get().save;

            this.pepeBodyCollection.SetActive(false); // starts deactivated 

            if (save.hasCollectedPepeLegs) {
                this.pepeLegsCollection.SetActive(false);
            }
            if (save.hasCollectedPepeHead) {
                this.pepeHeadCollection.SetActive(false);
            }
            if (save.hasCollectedPepeWings) {
                this.pepeWingsCollection.SetActive(false);
            }

            this.annoyingBird = this.annoyingBirdSpawn.Spawn();
        }

        private void Start() {
            this.deathUI.gameObject.SetActive(false);
            this.startUI.gameObject.SetActive(true);
            this.gameUI.gameObject.SetActive(false);
            var save = TheSystem.Get().save;

            if (save.hasUnDammedRiver) {
                this.riverLevel.UnDamRiver(true);
            }

            
            if (TheSystem.Get().startGameImmediately) {
                TheSystem.Get().startGameImmediately = false;
                this.SpawnPlayer();
            } else {
                this.startUI.Show();
            }
            
        }

        public void SpawnPlayer() {
            this.startUI.gameObject.SetActive(false);
            var spawnId = TheSystem.Get().keyStore.GetString("spawn_point");
            SpawnPointActivation spawn = this.spawns[0];
            for (var i = 0; i < this.spawns.Length; i++) {
                if (this.spawns[i].spawnPointId == spawnId) {
                    spawn = this.spawns[i];
                }
            }
            
            this.gameUI.gameObject.SetActive(true);
            PlayerDriverController.Instance.SpawnIn(spawn);
        }

        public void PlayerDidDie() {
            this.gameUI.gameObject.SetActive(false);
            this.deathUI.Show();
        }

        public void BlowUpDam() {
            var save = TheSystem.Get().save;
            save.hasUnDammedRiver = true;
            TheSystem.Get().UpdateSave();
            this.riverLevel.UnDamRiver(false);
        }
        public void PickedUpPepePart(PartId partId) {
            var save = TheSystem.Get().save;
            switch (partId) {
                case PartId.PepeHead: {
                    save.hasCollectedPepeHead = true;
                    break;
                }
                case PartId.PepeBody: {
                    save.hasCollectedPepeBody = true;
                    break;
                }
                case PartId.PepeLegs: {
                    save.hasCollectedPepeLegs = true;
                    break;
                }
                case PartId.PepeWings: {
                    save.hasCollectedPepeWings = true;
                    break;
                }
            }
            TheSystem.Get().UpdateSave();

            var part = CreaturePartIndex.Instance.recycler.Get(partId);
            PlayerDriverController.Instance.creature.SetCreaturePart(part);
            PlayerDriverController.Instance.creature.FinishSettingParts(false);
        }

        public void UpdateSpawnPoint(string spawnPointId) {
            
            TheSystem.Get().keyStore.Set("spawn_point", spawnPointId);
        }
        private void OnDestroy() {
            if (Instance == this) {
                Instance = null;
            }
        }

        [Button("Clear save")]
        public void ClearSave() {
            TheSystem.Get().ClearSave();
        }
        [Button("Test save")]
        public void GetSave() {
            TheSystem.Get().ClearSave();
            var save = TheSystem.Get().save;
            save.hasUnDammedRiver = true;
            TheSystem.Get().UpdateSave();
        }
    }
}