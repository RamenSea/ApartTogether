using Creatures.Collision;
using UnityEngine;

namespace Creatures.Parts.Limbs {
    public class BaseBodyLimb: BaseLimb {
        public LimbAttachPoint headAttachPoint;
        public LimbAttachPoint[] armsAttachPoint;
        public LimbAttachPoint[] legsAttachPoint;
        public CreatureCollider creatureCollider;
    }
}