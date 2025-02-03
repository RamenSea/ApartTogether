

using System;
using Systems;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Creatures.AI {
    public class PlayAudioClip: BaseAIAgent {
        public float timeTilPlay = 0f;

        public float minTimeTilPlay = 5f;
        public float maxTimeTilPlay = 5f;
        public float minStartTilPlay = 5f;
        public float maxStartTilPlay = 5f;


        public PlayAudio audio;
        private void Start() {
            this.timeTilPlay = Random.Range(this.minStartTilPlay, this.maxStartTilPlay);
        }

        private void Update() {
            if (!this.hasStarted) {
                return;
            }
            if (this.creature == null || this.creature.isDead) {
                return;
            }

            this.timeTilPlay -= Time.deltaTime;
            if (this.timeTilPlay <= 0) {
                this.timeTilPlay = Random.Range(this.minTimeTilPlay, this.maxTimeTilPlay);
                this.audio.Play();
            }
        }
    }
}