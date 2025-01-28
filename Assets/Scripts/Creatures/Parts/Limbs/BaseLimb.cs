using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Creatures.Parts {
    public class BaseLimb: MonoBehaviour {
        public CreatureInterface creatureInterface;
        public Transform[] bones;
        public Rig[] rigs;
        public Collider[] colliders;

        public bool isAttachedToCreature = false;
        public void GameUpdate(float deltaTime) {
            this.OnGameUpdate(deltaTime);
        }
        protected virtual void OnGameUpdate(float deltaTime) { }
        public void PhysicsUpdate(float deltaTime) {
            this.OnPhysicsUpdate(deltaTime);
        }
        protected virtual void OnPhysicsUpdate(float deltaTime) { }
        
        
        public virtual void OnDroppedLimb() {
            this.isAttachedToCreature = false;
            this.creatureInterface = null;
        }
        public virtual void OnDroppedLimbDestroy() {
            this.isAttachedToCreature = false;
            this.creatureInterface = null;
            
        }
        public virtual void OnAttachToBody(BaseBodyPart bodyPart, Transform toPoints) {
            this.transform.localPosition = Vector3.zero;
            this.transform.localRotation = Quaternion.identity;
            this.isAttachedToCreature = true;
        }
        public virtual void OnDeattachBody() {
            this.creatureInterface = null;
            this.isAttachedToCreature = false;
            this.creatureInterface = null;
        }

        public void ChangeColliderActivations(bool shouldActivate, LayerMask layerMask) {
            for (var i = 0; i < this.colliders.Length; i++) {
                this.colliders[i].enabled = shouldActivate;
                this.colliders[i].gameObject.layer = layerMask;
            }
        }
    }
}