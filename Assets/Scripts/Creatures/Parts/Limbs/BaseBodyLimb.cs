using Creatures.Collision;
using UnityEngine;

namespace Creatures.Parts.Limbs {
    public class BaseBodyLimb: BaseLimb {
        public Transform headAttachPoint;
        public Transform[] armsAttachPoint;
        public Transform[] legsAttachPoint;
        public CreatureCollider creatureCollider;
    }
}