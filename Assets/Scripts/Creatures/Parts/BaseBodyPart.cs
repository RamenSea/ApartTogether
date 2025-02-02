using System;
using Creatures.Parts.Limbs;
using Systems;
using Unity.VisualScripting;
using UnityEngine;

namespace Creatures.Parts {
    public class BaseBodyPart: BaseCreaturePart {
        
        [SerializeField] public BaseBodyLimb bodyLimb;
        
        [NonSerialized] public BaseCreaturePart attachedHeadPart = null;
        [NonSerialized] public BaseCreaturePart attachedLegPart = null;
        [NonSerialized] public BaseCreaturePart attachedArmsPart = null;

        public void AttachBody(BaseCreature creature) {
            this.creature = creature;
            this.bodyLimb.creatureCollider.creature = creature;

            if (creature.isPlayer) {
                this.bodyLimb.creatureCollider.gameObject.layer = CreatureManager.Instance.playerMask;
            } else {
                this.bodyLimb.creatureCollider.gameObject.layer = CreatureManager.Instance.normalCreatureMask;
            }
        }
        public void AttachPart(BaseCreaturePart part) {
            Transform worldPartsTransform = null; // todo
            switch (part.slotType) {
                case PartSlotType.Head:
                    if (this.attachedHeadPart != null) {
                        this.attachedHeadPart.transform.SetParent(worldPartsTransform);
                        this.attachedHeadPart.OnDeattachToBody(true);
                    }

                    this.attachedHeadPart = part;
                    part.OnAttachToBody(this, new []{this.bodyLimb.headAttachPoint});
                    break;
                case PartSlotType.Legs:
                    if (this.attachedLegPart != null) {
                        this.attachedLegPart.transform.SetParent(worldPartsTransform);
                        this.attachedLegPart.OnDeattachToBody(true);
                    }

                    this.attachedLegPart = part;
                    part.OnAttachToBody(this, this.bodyLimb.legsAttachPoint);
                    break;
                case PartSlotType.Arms:
                    if (this.attachedArmsPart != null) {
                        this.attachedArmsPart.transform.SetParent(worldPartsTransform);
                        this.attachedArmsPart.OnDeattachToBody(true);
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