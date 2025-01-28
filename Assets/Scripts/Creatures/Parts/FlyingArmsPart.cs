using UnityEngine;

namespace Creatures.Parts {
    public class FlyingArmsPart: BaseArmPart {
        
        private float timeTilFlap = 0;
        private float flapTimer = 0;
        private bool flapping = false;
        protected override void OnPhysicsUpdate(float deltaTime) {
            base.OnPhysicsUpdate(deltaTime);

        }
        //
        // protected void PerformBasicMovement(float deltaTime) {
        //     // if (!this.isOnGround) {
        //     //     return;
        //     // }
        //     var worldDirection = this.creatureInterface.moveDirection;
        //     var targetGoalVelocity = worldDirection * this.creatureInterface.compiledTraits.maxSpeed;
        //     this.goalVelocity = Vector3.MoveTowards(this.goalVelocity, targetGoalVelocity, deltaTime * this.creatureInterface.compiledTraits.acceleration);
        //
        //     var accelNeeded = (this.goalVelocity - this.creatureInterface.rb.linearVelocity) / deltaTime;
        //     this.creatureInterface.rb.AddForce(Vector3.Scale(accelNeeded, new Vector3(1,0,1)));
        // }
    }
}