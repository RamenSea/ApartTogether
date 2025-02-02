using System;
using Creatures.Collision;
using UnityEngine;

namespace Creatures.AI {
    public class EffectsStatsAI: BaseAIAgent {

        public CreatureTraits traits;
        private void Start() {
        }

        public override void PostStart() {
            base.PostStart();

            this.creature.additionalTraits = this.traits;
            this.creature.FinishSettingParts(false);
        }
    }
}