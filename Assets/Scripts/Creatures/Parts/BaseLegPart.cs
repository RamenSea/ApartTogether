using Creatures.Parts.Limbs;
using UnityEngine;

namespace Creatures.Parts {
    public class BaseLegPart: BaseCreaturePart {

        protected override void OnGameUpdate(float deltaTime) {
            base.OnGameUpdate(deltaTime);

            if (this.creature.landedOnGroundThisFrame) {
                this.ScrambleSteps();
            }
        }

        public override void OnAttachToBody(BaseBodyPart bodyPart, LimbAttachPoint[] toPoints) {
            base.OnAttachToBody(bodyPart, toPoints);
            this.ScrambleSteps();
        }

        public void ScrambleSteps() {
            for (var i = 0; i < this.limbs.Length; i++) {
                var limb = this.limbs[i];
                if (limb is BaseLegLimb leg) {
                    var isForward = i % 2 == 0;
                    leg.ScrambleIdleStep(isForward);
                }
            }
        }
    }
}