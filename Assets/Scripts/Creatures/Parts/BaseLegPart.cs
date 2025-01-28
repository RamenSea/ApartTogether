using Creatures.Parts.Limbs;
using UnityEngine;

namespace Creatures.Parts {
    public class BaseLegPart: BaseCreaturePart {
        public override PartSlotType slotType => PartSlotType.Legs;

        protected Vector3 goalVelocity;

        protected float jumpRecharge = 0;

        protected override void OnGameUpdate(float deltaTime) {
            base.OnGameUpdate(deltaTime);

            if (this.creatureInterface.landedOnGroundThisFrame) {
                this.ScrambleSteps();
            }
        }

        protected override void OnPhysicsUpdate(float deltaTime) {
            base.OnPhysicsUpdate(deltaTime);
            
            this.PerformBasicMovement(deltaTime);
            this.PerformBasicJumpCheck(deltaTime);
        }

        protected void PerformBasicMovement(float deltaTime) {
            // if (!this.isOnGround) {
            //     return;
            // }
            var worldDirection = this.creatureInterface.moveDirection;
            var targetGoalVelocity = worldDirection * this.creatureInterface.compiledTraits.maxSpeed;
            this.goalVelocity = Vector3.MoveTowards(this.goalVelocity, targetGoalVelocity, deltaTime * this.creatureInterface.compiledTraits.acceleration);
        
            var accelNeeded = (this.goalVelocity - this.creatureInterface.rb.linearVelocity) / deltaTime;
            this.creatureInterface.rb.AddForce(Vector3.Scale(accelNeeded, new Vector3(1,0,1)));
        }

        public override void OnAttachToBody(BaseBodyPart bodyPart, Transform[] toPoints) {
            base.OnAttachToBody(bodyPart, toPoints);
            this.ScrambleSteps();
        }
        protected void PerformBasicJumpCheck(float deltaTime) {
            if (this.jumpRecharge > 0) {
                this.jumpRecharge -= Time.deltaTime;
            }
            if (this.creatureInterface.doLegAction && this.creatureInterface.isOnGround && this.jumpRecharge <= 0.0f) {
                this.jumpRecharge = 0.2f;
                this.creatureInterface.rb.AddForce(Vector3.up * this.creatureInterface.compiledTraits.jumpPower);
            }
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