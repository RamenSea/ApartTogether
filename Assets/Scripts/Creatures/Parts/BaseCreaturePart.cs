using System;
using RamenSea.Foundation3D.Extensions;
using Systems;
using UnityEngine;

namespace Creatures.Parts {
    public enum PartSlotType {
        Body,
        Legs,
        Head,
        Arms,
    }
    public enum PartId {
        DogHead,
        DogBody,
        DogLegs,
    }
    public class BaseCreaturePart: MonoBehaviour {
        public virtual PartSlotType slotType => PartSlotType.Body;

        [SerializeField] public BaseLimb limbPrefab; // only if u need it

        [SerializeField] protected PartId _partId;
        public PartId partId => _partId;

        public BaseLimb[] limbs;

        public bool isAttached = false;
        public virtual void OnAttachToBody(BaseBodyPart bodyPart, Transform[] toPoints) {
            this.isAttached = true;
            for (var i = toPoints.Length; i < this.limbs.Length; i++) {
                this.limbs[i].OnDroppedLimbDestroy();
                this.limbs[i].Destroy();
            }
            var updatedLimbs = new BaseLimb[toPoints.Length];
            for (var i = 0; i < updatedLimbs.Length; i++) {
                BaseLimb limb;
                if (i < this.limbs.Length) {
                    limb = this.limbs[i];
                } else {
                    limb = this.limbPrefab.Instantiate(toPoints[i]);
                }
                updatedLimbs[i] = limb;
                limb.OnAttachToBody(bodyPart, toPoints[i]);
            }
            this.limbs = updatedLimbs;
        }
        public virtual void OnDeattachToBody(bool isDropped) {
            this.isAttached = false;
            for (var i = 1; i < this.limbs.Length; i++) {
                this.limbs[i].OnDeattachToBody();
                this.limbs[i].Destroy();
            }

            if (isDropped) {
                for (var i = 1; i < this.limbs.Length; i++) {
                    this.limbs[i].OnDeattachToBody();
                    this.limbs[i].Destroy();
                }
                
                BaseLimb limb;
                if (this.limbs.Length > 0) {
                    limb = this.limbs[0];
                } else {
                    limb = this.limbPrefab.Instantiate(this.transform);
                }
                limb.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                var collector = WorldPartCollector.Instance.Get();
                this.limbs = new BaseLimb[1] {limb};
                this.limbs[0] = limb;
                collector.AttachPart(this);
                limb.OnDroppedLimb();
            } else {
                for (var i = 0; i < this.limbs.Length; i++) {
                    this.limbs[i].OnDeattachToBody();
                    this.limbs[i].Destroy();
                }
            }
        }
        public void GameUpdate(float deltaTime) {
            this.OnGameUpdate(deltaTime);
            for (var i = 0; i < this.limbs.Length; i++) {
                this.limbs[i].GameUpdate(deltaTime);
            }
        }
        protected virtual void OnGameUpdate(float deltaTime) { }
        public void PhysicsUpdate(float deltaTime) {
            this.OnPhysicsUpdate(deltaTime);
            for (var i = 0; i < this.limbs.Length; i++) {
                this.limbs[i].PhysicsUpdate(deltaTime);
            }
        }
        protected virtual void OnPhysicsUpdate(float deltaTime) { }
    }
}