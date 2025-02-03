using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Audio {
    public class MusicPlayer: MonoBehaviour {

        public static MusicPlayer Instance;
        public AudioSource introMusic;
        public AudioSource gameMusic;


        private void Awake() {
            Instance = this;
        }

        private void Start() {
            
        }

        public async void SpawningIn() {
            var v = this.gameMusic.volume;
            this.introMusic.DOFade(0f, 5f);
            await UniTask.Delay(TimeSpan.FromSeconds(4f));

            this.gameMusic.volume = 0f;
            this.gameMusic.Play();
            this.gameMusic.DOFade(v, 5f);

            await UniTask.Delay(TimeSpan.FromSeconds(1f));
            this.introMusic.Stop();
        }

        private void OnDestroy() {
            if (Instance == this) {
                Instance = null;
            }
        }
    }
}