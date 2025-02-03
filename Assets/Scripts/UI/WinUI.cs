using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI {
    public class WinUI: MonoBehaviour {
        public float fadeIn = 1f;
        public Image backgroundImage;
        public RawImage pepeImage;
        public TMP_Text winText;
        public TMP_Text creditsText;


        public void Show() {
            var color = this.backgroundImage.color;
            this.backgroundImage.color = Color.clear;
            this.backgroundImage.DOColor(color, this.fadeIn);
            color = this.pepeImage.color;
            this.pepeImage.color = Color.clear;
            this.pepeImage.DOColor(color, this.fadeIn);
            color = this.winText.color;
            this.winText.color = Color.clear;
            this.winText.DOColor(color, this.fadeIn);
            color = this.creditsText.color;
            this.creditsText.color = Color.clear;
            this.creditsText.DOColor(color, this.fadeIn);
        }
        public void BeginAgain() {
            SceneManager.LoadScene("MainLevel");
        }
        public void KeepPlaying() {
            this.gameObject.SetActive(false);
        }
    }
}