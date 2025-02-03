using System;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using Player;
using TMPro;
using UnityEngine;

namespace Systems {

    public struct Hint {
        public Transform transform;
        public Transform lookAt;
        public string message;
    }
    public class HintingShowingSystem: MonoBehaviour {

        public TMP_Text hintText;
        public float timeOnEachHint = 5f;
        public float textTime = 5f;
        public float hintTimer = 0f;
        public int hintIndex = 0;

        public List<Hint> hints;

        public Hint hint;
        public bool showingHints = false;
        private void Awake() {
            this.hints = new List<Hint>();
            this.hintText.text = "";
            this.hintText.gameObject.SetActive(false);
        }

        private void Update() {
            if (this.showingHints == false) return;
            
            this.hintTimer -= Time.deltaTime;
            if (this.hintTimer <= 0f) {
                this.ShowNextHint();
            }
        }

        [Button("Show hints")]
        public async void ShowHint() {
            if (this.showingHints) {
                this.ShowNextHint();
                return;
            }

            PlayerDriverController.Instance.SetCameraForHints();
            this.hintText.gameObject.SetActive(true);
            this.showingHints = true;
            this.hintIndex = -1;
            this.ShowNextHint();
        }

        public void StopShowingHints() {
            this.hintText.DOKill();
            this.hintText.text = "";
            this.hintText.gameObject.SetActive(false);
            this.showingHints = false;
            if (PlayerDriverController.Instance.creature != null) {
                PlayerDriverController.Instance.SetCameraAfterSpawn();
            } else {
                PlayerDriverController.Instance.SetCameraForBird();
            }
        }

        [Button("Show next hint")]
        public void ShowNextHint() {
            this.hintText.DOKill();
            this.hintText.text = "";
            var save = TheSystem.Get().save;
            this.hintIndex += 1;
            this.hints.Clear();
            this.hintTimer = this.timeOnEachHint;

            if (!save.hasCollectedPepeHead) {
                this.hints.Add(new Hint() {
                    transform = GameRunner.Instance.pepeHeadCameraLocation,
                    lookAt = GameRunner.Instance.pepeHeadLookAtLocation,
                    message = "A head at the top of a bar"
                });
            }
            if (!save.hasCollectedPepeLegs) {
                this.hints.Add(new Hint() {
                    transform = GameRunner.Instance.pepeLegsCameraLocation,
                    lookAt = GameRunner.Instance.pepeLegsLookAtLocation,
                    message = "Climb the mountain"
                });
            }
            if (!save.hasCollectedPepeBody) {
                this.hints.Add(new Hint() {
                    transform = GameRunner.Instance.pepeBodyCameraLocation,
                    lookAt = GameRunner.Instance.pepeBodyLookAtLocation,
                    message = "Return a fish to its pond"
                });
            }
            if (!save.hasCollectedPepeWings) {
                var otherHintsSoFar = this.hints.Count > 0;
                this.hints.Add(new Hint() {
                    transform = GameRunner.Instance.pepeWingsCameraLocation,
                    lookAt = GameRunner.Instance.pepeWingsLookAtLocation,
                    message = otherHintsSoFar ? "He needs more parts before he can get his wings" : "His wings await!",
                });
            }
            this.hints.Add(new Hint() {
                transform = GameRunner.Instance.annoyingBird.transform,
                lookAt = GameRunner.Instance.annoyingBird.transform,
                message = "End this annoying bird so we can get out of here",
            });

            if (this.hintIndex >= this.hints.Count) {
                this.StopShowingHints();
            } else {
                this.hint = this.hints[this.hintIndex];
                this.hintText.text = "";
                this.hintText.DOText(this.hint.message, this.textTime);
                CameraController.Instance.virtualCamera.Follow = this.hint.transform;
                CameraController.Instance.virtualCamera.LookAt = this.hint.lookAt;
            }

        }
    }
}