using System;
using Creatures.Parts.Limbs;
using Player.PickUp;
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
        None = 0,
        
        DogHead = 10_1,
        DogBody = 10_2,
        DogLegs = 10_3,
        
        CannonArms = 11_1,
        
        BeetleHead = 21_1,
        BeetleBody = 21_2,
        BeetleLegs = 21_3,
        
        BirdArm = 31_4,
        
        CowHead = 41_1,
        CowBody = 41_2,
        CowLegs = 41_3,
        
        FrogHead = 51_1,
        FrogBody = 51_2,
        FrogLegs = 51_3,
        
        BunHead = 61_1,
        BunBody = 61_2,
        BunLegs = 61_3,
        
        SpiderHead = 71_1,
        SpiderBody = 71_2,
        SpiderLegs = 71_3,
        
        FishHead = 81_1,
        FishBody = 81_2,
        FishLegs = 81_3,
    }
    public class BaseCreaturePart: MonoBehaviour {
        public BaseCreature creature;
        
        public PartSlotType slotType = PartSlotType.Body;

        [SerializeField] public BaseLimb limbPrefab; // only if u need it
        [SerializeField] protected bool deactivateCollidersWhileAttached = true;

        [SerializeField] protected PartId _partId;
        [SerializeField] protected CreatureTraits _traits;
        public CreatureTraits traits => this._traits;
        public PartId partId => _partId;

        public BaseLimb[] limbs;

        public bool isAttached = false;

        protected BaseBodyPart attachedToBody;

        [NonSerialized] public bool isPlayer = false;
        public virtual void OnAttachToBody(BaseBodyPart bodyPart, LimbAttachPoint[] toPoints) {
            if (this.attachedToBody != null) {
                this.attachedToBody?.PartWillDeattach(this);
            }
            this.attachedToBody = bodyPart;
            this.creature = bodyPart.creature;
            this.isAttached = true;
            this.transform.SetParent(this.creature.transform);
            this.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            
            for (var i = toPoints.Length; i < this.limbs.Length; i++) {
                this.limbs[i].OnDroppedLimbDestroy();
                this.limbs[i].gameObject.Destroy();
            }
            var updatedLimbs = new BaseLimb[toPoints.Length];
            for (var i = 0; i < updatedLimbs.Length; i++) {
                BaseLimb limb;
                if (i < this.limbs.Length) {
                    limb = this.limbs[i];
                    limb.transform.SetParent(toPoints[i].transform);
                } else {
                    limb = this.limbPrefab.Instantiate(toPoints[i].transform);
                }
                limb.creature = this.creature;
                updatedLimbs[i] = limb;
                limb.OnAttachToBody(bodyPart, toPoints[i]);
            }
            this.limbs = updatedLimbs;

            var layerMaskToUse = CreatureManager.Instance.normalCreatureMask;
            for (var i = 0; i < this.limbs.Length; i++) {
                this.limbs[i].ChangeColliderActivations(!this.deactivateCollidersWhileAttached, layerMaskToUse);
            }
        }
        public virtual void OnDeattachToBody(bool isDropped) {
            this.isPlayer = false;
            this.attachedToBody?.PartWillDeattach(this);
            this.attachedToBody = null;
            this.isAttached = false;
            this.creature = null;
            this.transform.SetParent(null); //todo deattch
            for (var i = 1; i < this.limbs.Length; i++) {
                this.limbs[i].OnDeattachBody();
                this.limbs[i].gameObject.Destroy();
            }

            if (isDropped) {
                for (var i = 0; i < this.limbs.Length; i++) {
                    this.limbs[i].OnDeattachBody();
                    if (i > 0) {
                        this.limbs[i].gameObject.Destroy();
                    }
                }
                
                BaseLimb limb;
                if (this.limbs.Length > 0) {
                    limb = this.limbs[0];
                    limb.transform.SetParent(this.transform);
                } else {
                    limb = this.limbPrefab.Instantiate(this.transform);
                }
                this.limbs = new[] {limb};
            } else {
                for (var i = 0; i < this.limbs.Length; i++) {
                    this.limbs[i].OnDeattachBody();
                    this.limbs[i].gameObject.Destroy();
                }
            }
        }

        public virtual void OnAttachToWorldContainer(PartPickUpHolder holder) {
            var layerMaskToUse = CreatureManager.Instance.worldPartsMask;
            for (var i = 0; i < this.limbs.Length; i++) {
                this.limbs[i].ChangeColliderActivations(true, layerMaskToUse);
            }
        }

        public virtual void OnDeattachToWorldContainer() {
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