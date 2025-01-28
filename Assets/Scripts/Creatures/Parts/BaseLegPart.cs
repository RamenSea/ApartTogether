using UnityEngine;

namespace Creatures.Parts {
    public class BaseLegPart: BaseCreaturePart {
        public override PartSlotType slotType => PartSlotType.Legs;

        protected Vector3 goalVelocity;

        protected override void OnPhysicsUpdate(float deltaTime) {
            base.OnPhysicsUpdate(deltaTime);
            
            this.PerformBasicMovement(deltaTime);
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
    }
}