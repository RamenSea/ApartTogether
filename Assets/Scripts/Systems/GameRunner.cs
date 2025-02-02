using System;
using UI;
using UnityEngine;

namespace Systems {
    public class GameRunner: MonoBehaviour {
        public static GameRunner Instance { get; private set; }
        public DeathUI deathUI;


        public Transform pepeLegs;
        public Transform pepeBody;
        public Transform pepeHead;
        public Transform pepeWings;
        
        private void Awake() {
            GameRunner.Instance = this;
        }

        private void Start() {
            this.deathUI.gameObject.SetActive(false);
        }

        public void PlayerDidDie() {
            this.deathUI.Show();
        }

        private void OnDestroy() {
            if (Instance == this) {
                Instance = null;
            }
        }
    }
}