using System;
using Creatures.Parts.Limbs;
using Systems;
using UnityEngine;

namespace Creatures.Parts {
    public class BaseBodyPart: BaseCreaturePart {
        public override PartSlotType slotType => PartSlotType.Body;
        
        public Transform followPoint;

        [SerializeField] protected BaseBodyLimb bodyLimb;
        
        [NonSerialized] public BaseHeadPart attachedHeadPart = null;
        [NonSerialized] public BaseLegPart attachedLegPart = null;
        [NonSerialized] public BaseArmPart attachedArmsPart = null;

        public void AttachBody(BaseCreature creature) {
            this.creatureInterface = creature;
            this.bodyLimb.creatureCollider.creature = creature;
        }
        public void AttachPart(BaseCreaturePart part) {
            Transform worldPartsTransform = null; // todo
            switch (part.slotType) {
                case PartSlotType.Head:
                    if (this.attachedHeadPart != null) {
                        this.attachedHeadPart.transform.SetParent(worldPartsTransform);
                        this.attachedHeadPart.OnDeattachToBody(true);
                    }

                    this.attachedHeadPart = part as BaseHeadPart;
                    part.OnAttachToBody(this, new Transform[]{this.bodyLimb.headAttachPoint});
                    break;
                case PartSlotType.Legs:
                    if (this.attachedLegPart != null) {
                        this.attachedLegPart.transform.SetParent(worldPartsTransform);
                        this.attachedLegPart.OnDeattachToBody(true);
                    }

                    this.attachedLegPart = part as BaseLegPart;
                    part.OnAttachToBody(this, this.bodyLimb.legsAttachPoint);
                    break;
                case PartSlotType.Arms:
                    if (this.attachedArmsPart != null) {
                        this.attachedArmsPart.transform.SetParent(worldPartsTransform);
                        this.attachedArmsPart.OnDeattachToBody(true);
                    }

                    this.attachedArmsPart = part as BaseArmPart;
                    part.OnAttachToBody(this, this.bodyLimb.armsAttachPoint);
                    break;
                case PartSlotType.Body:
                    //todo transfer over parts, attach to main GO
                    break;
            }
        }
    }
}