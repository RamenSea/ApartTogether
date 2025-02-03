using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using RamenSea.Foundation.Extensions;
using Systems;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = System.Random;

namespace UI {
    public class StartGameUI: MonoBehaviour {

        public Image backgroundImage;
        public TMP_Text titleText;
        public Button startGameButton;
        public Button newSaveButton;
        
        public float fadeInBackground = 2f;
        public float fadeInTitle = 4f;

        // public string[] deathPhrases;
        // public string[] tips;

        private void Start() {
        }
        
        

        public async void Show() {
            this.gameObject.SetActive(true);
            var random = new Random();
            // this.deathText.text = this.deathPhrases.RandomElement(random);
            // this.tipsText.text = this.tips.RandomElement(random);
            //
            var rollingDelay = 0f;
            var color = this.backgroundImage.color;
            this.backgroundImage.color = Color.clear;
            this.backgroundImage.DOColor(color, this.fadeInBackground);
            // color = this.deathText.color;
            // this.deathText.color = Color.clear;
            // rollingDelay += this.fadeInBackground * 0.5f;
            // this.deathText.DOColor(color, this.fadeInText).SetDelay(rollingDelay);
            // color = this.tipsText.color;
            // this.tipsText.color = Color.clear;
            // rollingDelay += this.fadeInText * 0.7f;
            // this.tipsText.DOColor(color, this.fadeInTip).SetDelay(rollingDelay);

            await UniTask.Delay(TimeSpan.FromSeconds(rollingDelay));
        }

        public void StartGame() {
            if (TheSystem.Get().keyStore.GetBool("played_before", false) == false) {
                TheSystem.Get().keyStore.Set("played_before", true);
                TheSystem.Get().startGameImmediately = true;
                SceneManager.LoadScene("Intro");
            } else {
                GameRunner.Instance.SpawnPlayer();
            }
        }
        public void NewSave() {
            TheSystem.Get().ClearSave();
            TheSystem.Get().startGameImmediately = true;
            SceneManager.LoadScene("Intro");
        }
        public void PlayIntro() {
            SceneManager.LoadScene("Intro");
        }
    }
}