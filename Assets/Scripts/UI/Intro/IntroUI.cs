using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI.Intro {
    public class IntroUI: MonoBehaviour {

        public TMP_Text text;
        public float timeTilStart = 1f;
        public float perLetterSpeed = 1f;

        public int storyIndex = 0;
        public string[] storyLines;

        private void Start() {
            
            this.PlayNextStoryLine();
        }


        public void PlayNextStoryLine() {
            if (this.storyIndex >= this.storyLines.Length) {
                SceneManager.LoadScene("MainLevel");
                return;
            }
            var line = this.storyLines[this.storyIndex];
            this.text.DOKill();
            this.text.text = "";
            this.text.DOText(line, this.perLetterSpeed * line.Length);

            storyIndex++;
        }

        public void Skip() {
            this.PlayNextStoryLine();
        }
    }
}