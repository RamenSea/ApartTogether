using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using RamenSea.Foundation.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = System.Random;

namespace UI {
    public class DeathUI: MonoBehaviour {

        public Image backgroundImage;
        public TMP_Text deathText;
        public TMP_Text tipsText;
        public Button restartButton;
        
        public float fadeInBackground = 0.3f;
        public float fadeInText = 0.3f;
        public float fadeInTip = 0.3f;

        public string[] deathPhrases;
        public string[] tips;

        private void Start() {
            this.restartButton.gameObject.SetActive(false);
        }

        public async void Show() {
            this.gameObject.SetActive(true);
            var random = new Random();
            this.deathText.text = this.deathPhrases.RandomElement(random);
            this.tipsText.text = this.tips.RandomElement(random);
            
            var rollingDelay = 0f;
            var color = this.backgroundImage.color;
            this.backgroundImage.color = Color.clear;
            this.backgroundImage.DOColor(color, this.fadeInBackground);
            color = this.deathText.color;
            this.deathText.color = Color.clear;
            rollingDelay += this.fadeInBackground * 0.5f;
            this.deathText.DOColor(color, this.fadeInText).SetDelay(rollingDelay);
            color = this.tipsText.color;
            this.tipsText.color = Color.clear;
            rollingDelay += this.fadeInText * 0.7f;
            this.tipsText.DOColor(color, this.fadeInTip).SetDelay(rollingDelay);

            await UniTask.Delay(TimeSpan.FromSeconds(rollingDelay));
            this.restartButton.gameObject.SetActive(true);
        }

        public void OnRestart() {
            SceneManager.LoadScene("MainLevel");
        }
    }
}