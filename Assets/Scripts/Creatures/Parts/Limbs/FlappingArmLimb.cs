using UnityEngine;

namespace Creatures.Parts.Limbs {
    public class FlappingArmLimb: BaseLimb {
        public Transform pivotPoint;
        public float flapBy;


        protected override void OnGameUpdate(float deltaTime) {
            base.OnGameUpdate(deltaTime);

            if (this.creature.isFlapping) {
                var percentFlap = (this.creature.compiledTraits.flapDuration - this.creature.flapTimer) / this.creature.compiledTraits.flapDuration;
                percentFlap = this.creature.flapCurve.Evaluate(percentFlap);
                this.pivotPoint.localRotation = Quaternion.Lerp(Quaternion.identity, Quaternion.Euler(this.flapBy, 0, 0), percentFlap);
            } else {
                var percent = (BaseCreature.FLAP_RECHARGE - this.creature.flapRechargeTimer) / BaseCreature.FLAP_RECHARGE;
                percent = this.creature.flapCurve.Evaluate(percent);
                this.pivotPoint.localRotation = Quaternion.Lerp(Quaternion.Euler(this.flapBy, 0, 0), Quaternion.identity, percent);
            }
        }
    }
}