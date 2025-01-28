using System;
using Creatures.Parts.Limbs;
using UnityEngine;

namespace Creatures.Parts {
    public class BaseBodyPart: BaseCreaturePart {
        public override PartSlotType slotType => PartSlotType.Body;

        [SerializeField] protected BaseBodyLimb bodyLimb;
        
        [NonSerialized] public BaseCreaturePart attachedHeadPart = null;
        [NonSerialized] public BaseCreaturePart attachedLegPart = null;
        [NonSerialized] public BaseCreaturePart attachedArmsPart = null;

        public void AttachPart(BaseCreaturePart part) {
            Transform worldPartsTransform = null;
            switch (part.slotType) {
                case PartSlotType.Head:
                    if (this.attachedHeadPart == null) {
                        this.attachedHeadPart.transform.SetParent(worldPartsTransform);
                        this.attachedHeadPart.OnDeattachToBody(true);
                    }

                    this.attachedHeadPart = part;
                    part.OnAttachToBody(this, new Transform[]{this.bodyLimb.headAttachPoint});
                    break;
                case PartSlotType.Legs:
                    if (this.attachedLegPart == null) {
                        this.attachedLegPart.transform.SetParent(worldPartsTransform);
                        this.attachedLegPart.OnDeattachToBody(true);
                    }

                    this.attachedLegPart = part;
                    part.OnAttachToBody(this, this.bodyLimb.legsAttachPoint);
                    break;
                case PartSlotType.Arms:
                    if (this.attachedArmsPart == null) {
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
    }
}