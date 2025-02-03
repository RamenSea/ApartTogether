using Creatures.Parts.Limbs;
using Systems;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Creatures.Parts {
    public class BaseLimb: MonoBehaviour {
        public BaseCreature creature;
        public Rig[] rigs;
        public Collider[] colliders;
        public LimbAttachPoint attachPoint;
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
            this.creature = null;
            this.attachPoint = null;
        }
        public virtual void OnDroppedLimbDestroy() {
            this.isAttachedToCreature = false;
            this.creature = null;
            this.attachPoint = null;
        }
        public virtual void OnAttachToBody(BaseBodyPart bodyPart, LimbAttachPoint toPoint) {
            this.transform.localPosition = Vector3.zero;
            this.transform.localRotation = Quaternion.identity;
            this.isAttachedToCreature = true;
            this.attachPoint = toPoint;
        }
        public virtual void OnDeattachBody() {
            this.transform.SetParent(null);
            this.creature = null;
            this.attachPoint = null;
            this.isAttachedToCreature = false;
        }

        public virtual void OnAttachToWorldContainer() {
            var layerMaskToUse = CreatureManager.Instance.worldPartsMask;
            this.ChangeColliderActivations(true, layerMaskToUse);
        }
        public void ChangeColliderActivations(bool shouldActivate, LayerMask layerMask) {
            for (var i = 0; i < this.colliders.Length; i++) {
                this.colliders[i].enabled = shouldActivate;
                this.colliders[i].gameObject.layer = layerMask;
            }
        }
    }
}