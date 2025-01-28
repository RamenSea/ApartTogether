using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Creatures.Parts {
    public class BaseLimb: MonoBehaviour {
        public Transform[] bones;
        public Rig[] rigs;
        public Collider[] colliders;
        
        public void GameUpdate(float deltaTime) {
            this.OnGameUpdate(deltaTime);
        }
        protected virtual void OnGameUpdate(float deltaTime) { }
        public void PhysicsUpdate(float deltaTime) {
            this.OnPhysicsUpdate(deltaTime);
        }
        protected virtual void OnPhysicsUpdate(float deltaTime) { }
        
        
        public virtual void OnDroppedLimb() {
            
        }
        public virtual void OnDroppedLimbDestroy() {
            
        }
        public virtual void OnAttachToBody(BaseBodyPart bodyPart, Transform toPoints) {
            this.transform.localPosition = Vector3.zero;
            this.transform.localRotation = Quaternion.identity;
        }
        public virtual void OnDeattachToBody() {
            
        }
    }
}