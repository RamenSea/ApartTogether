using System;
using Creatures.Parts.Limbs;
using NaughtyAttributes;
using RamenSea.Foundation3D.Extensions;
using Systems;
using Unity.VisualScripting;
using UnityEngine;

namespace Creatures.Parts {
    public class BaseBodyPart: BaseCreaturePart {
        
        [SerializeField] public BaseBodyLimb bodyLimb;
        
        public BaseCreaturePart attachedHeadPart = null;
        public BaseCreaturePart attachedLegPart = null;
        public BaseCreaturePart attachedArmsPart = null;

        public void AttachBody(BaseCreature creature) {
            this.creature = creature;
            this.bodyLimb.transform.SetParent(this.transform);
            this.bodyLimb.creatureCollider.creature = creature;
            this.bodyLimb.creatureCollider.collider.enabled = true;
            
            if (creature.isPlayer) {
                this.bodyLimb.creatureCollider.gameObject.layer = CreatureManager.Instance.playerMask;
            } else {
                this.bodyLimb.creatureCollider.gameObject.layer = CreatureManager.Instance.normalCreatureMask;
            }
            
            this.bodyLimb.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            this.bodyLimb.isAttachedToCreature = true;
        }
        public void AttachPart(BaseCreaturePart part) {
            Transform worldPartsTransform = null; // todo
            switch (part.slotType) {
                case PartSlotType.Head:
                    if (this.attachedHeadPart != null) {
                        var removePart = this.attachedHeadPart;
                        this.attachedHeadPart = null;
                        removePart.transform.SetParent(worldPartsTransform);
                        removePart.OnDeattachToBody(true);
                        WorldPartCollector.Instance.DropPart(removePart);
                    }

                    this.attachedHeadPart = part;
                    part.OnAttachToBody(this, new []{this.bodyLimb.headAttachPoint});
                    break;
                case PartSlotType.Legs:
                    if (this.attachedLegPart != null) {
                        var removePart = this.attachedLegPart;
                        this.attachedLegPart = null;
                        removePart.transform.SetParent(worldPartsTransform);
                        removePart.OnDeattachToBody(true);
                        WorldPartCollector.Instance.DropPart(removePart);
                    }

                    this.attachedLegPart = part;
                    part.OnAttachToBody(this, this.bodyLimb.legsAttachPoint);
                    break;
                case PartSlotType.Arms:
                    if (this.attachedArmsPart != null) {
                        var removePart = this.attachedArmsPart;
                        this.attachedArmsPart = null;
                        removePart.transform.SetParent(worldPartsTransform);
                        removePart.OnDeattachToBody(true);
                        WorldPartCollector.Instance.DropPart(removePart);
                    }

                    this.attachedArmsPart = part;
                    part.OnAttachToBody(this, this.bodyLimb.armsAttachPoint);
                    break;
                case PartSlotType.Body:
                    //todo transfer over parts, attach to main GO
                    break;
            }
        }

        public override void OnDeattachToBody(bool isDropped) {
            base.OnDeattachToBody(isDropped);
            this.bodyLimb.creatureCollider.creature = null;
            this.bodyLimb.creatureCollider.collider.enabled = false;
        }

        public void PartWillDeattach(BaseCreaturePart part) {
            if (part == this.attachedHeadPart) {
                this.attachedHeadPart = null;
            }
            if (part == this.attachedLegPart) {
                this.attachedLegPart = null;
            }
            if (part == this.attachedArmsPart) {
                this.attachedArmsPart = null;
            }
        }
    }
}