using System.Collections.Generic;
using Creatures.Parts;
using Creatures.Parts.Limbs;

namespace Creatures.AI {
    public class BiteAI: BaseAIAgent {

        private BaseHeadPart headPart;

        public override void StartAI() {
            base.StartAI();
            this.headPart = this.creature.headPart as BaseHeadPart;
        }
        private void Update() {
            if (!this.hasStarted) {
                return;
            }
            if (this.creature == null || this.creature.isDead || this.headPart == null) {
                return;
            }

            this.creature.doHeadAction = false;
            if (this.headPart.CanAndReadyToBite()) {
                this.creature.doHeadAction = true;
            }
        }
    }
}