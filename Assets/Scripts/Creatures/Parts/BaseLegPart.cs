using Creatures.Parts.Limbs;
using NaughtyAttributes;
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

        [Button("Scramble Steps")]
        public void ScrambleSteps() {
            for (var i = 0; i < this.limbs.Length; i++) {
                var limb = this.limbs[i];
                if (limb is BaseLegLimb leg) {
                    leg.ScrambleIdleStep(!limb.attachPoint.isLeft);
                }
            }
        }
    }
}