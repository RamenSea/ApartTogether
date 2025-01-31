using UnityEngine;

namespace Creatures.Parts.Limbs {
    public class SpinningHeadLimb: BaseHeadLimb {
        [SerializeField] private Transform propellerHead;
        [SerializeField] private float speedToRotation;
        [SerializeField] private float lastHeight;


        public override void OnAttachToBody(BaseBodyPart bodyPart, LimbAttachPoint toPoint) {
            base.OnAttachToBody(bodyPart, toPoint);
            this.lastHeight = this.transform.position.y;
        }

        protected override void OnGameUpdate(float deltaTime) {
            base.OnGameUpdate(deltaTime);
            
            var diff = Mathf.Abs(this.transform.position.y - this.lastHeight);
            this.lastHeight = this.transform.position.y;

            var rotation = diff * this.speedToRotation * this.speedToRotation * this.speedToRotation * deltaTime;
            this.propellerHead.Rotate(0, rotation, 0);
        }
    }
}