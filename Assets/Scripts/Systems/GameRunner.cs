using System;
using Audio;
using Creatures;
using Creatures.Collision;
using Creatures.Parts;
using Cysharp.Threading.Tasks;
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
        public WinUI winUI;

        public RiverLevel riverLevel;
        public Transform pepeLegsLookAtLocation;
        public Transform pepeLegsCameraLocation;
        public GameObject pepeLegsCollection;
        public Transform pepeBodyLookAtLocation;
        public Transform pepeBodyCameraLocation;
        public GameObject pepeBodyCollection;
        public Transform pepeHeadLookAtLocation;
        public Transform pepeHeadCameraLocation;
        public GameObject pepeHeadCollection;
        public Transform pepeWingsLookAtLocation;
        public Transform pepeWingsCameraLocation;
        public GameObject pepeWingsCollection;
        public SpawnPointActivation[] spawns;
        public SpawnInCreature annoyingBirdSpawn;
        public BaseCreature annoyingBird;
        public HintingShowingSystem hint;
        public Transform hiddenDoor;
        public float moveHiddenDoorBy;

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
            
            if (save.hasCollectedPepeLegs && save.hasCollectedPepeHead && save.hasCollectedPepeBody) {
                var pos = this.hiddenDoor.position;
                pos.y += this.moveHiddenDoorBy;
                this.hiddenDoor.position = pos;
            }
            
        }

        private void Start() {
            this.annoyingBird = this.annoyingBirdSpawn.Spawn();
            
            this.deathUI.gameObject.SetActive(false);
            this.startUI.gameObject.SetActive(false);
            this.gameUI.gameObject.SetActive(false);
            var save = TheSystem.Get().save;

            if (save.hasUnDammedRiver) {
                this.riverLevel.UnDamRiver(true);
            }

            var spawnId = TheSystem.Get().keyStore.GetString("spawn_point");
            for (var i = 0; i < this.spawns.Length; i++) {
                var isActive = this.spawns[i].spawnPointId.Equals(spawnId);
                if (spawnId.Length == 0 && i == 0) {
                    isActive = true;
                }
                this.spawns[i].SetSpawnActive(isActive);
            }
            
            if (TheSystem.Get().startGameImmediately) {
                this.SpawnPlayerAndThenShowHints();
            } else {
                this.startUI.gameObject.SetActive(false);
                PlayerDriverController.Instance.SetCameraForBird();
                this.startUI.Show();
            }
        }

        private void Update() {
            if (PlayerDriverController.Instance.creature != null && !PlayerDriverController.Instance.creature.isDead &&
                this.annoyingBird != null && this.annoyingBird.isDead) {
                this.winUI.gameObject.SetActive(true);
            }
        }

        [Button("Test spawn")]
        public void TestSpawn() {
            this.startUI.gameObject.SetActive(false);
            this.gameUI.gameObject.SetActive(true);
            PlayerDriverController.Instance.SpawnInTest();
        }
        public void SpawnPlayer() {
            MusicPlayer.Instance.SpawningIn();
            this.startUI.gameObject.SetActive(false);
            var spawnId = TheSystem.Get().keyStore.GetString("spawn_point");
            SpawnPointActivation spawn = this.spawns[0];
            for (var i = 0; i < this.spawns.Length; i++) {
                if (this.spawns[i].spawnPointId.Equals(spawnId)) {
                    spawn = this.spawns[i];
                }
            }
            
            this.gameUI.gameObject.SetActive(true);
            PlayerDriverController.Instance.SpawnIn(spawn);
        }

        public async void SpawnPlayerAndThenShowHints() {
            TheSystem.Get().startGameImmediately = false;
            this.SpawnPlayer();
            // await UniTask.Delay(TimeSpan.FromSeconds(15f));
            // this.hint.ShowHint();
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
            
            if (save.hasCollectedPepeLegs && save.hasCollectedPepeHead && save.hasCollectedPepeBody) {
                var pos = this.hiddenDoor.position;
                pos.y += this.moveHiddenDoorBy;
                this.hiddenDoor.position = pos;
            }
        }

        public void UpdateSpawnPoint(string spawnPointId) {
            
            TheSystem.Get().keyStore.Set("spawn_point", spawnPointId);
            for (var i = 0; i < this.spawns.Length; i++) {
                var isActive = this.spawns[i].spawnPointId.Equals(spawnPointId);
                if (spawnPointId.Length == 0 && i == 0) {
                    isActive = true;
                }

                if (isActive) {
                    continue;
                }
                this.spawns[i].SetSpawnActive(false);
            }
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