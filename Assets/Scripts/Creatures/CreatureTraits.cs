using System;
using Creatures.Parts;

namespace Creatures {
    
    [Serializable]
    public struct CreatureTraits {
        public float weight;
        public float maxSpeed;
        public float acceleration;
        public float height;
        // 2000 based off of 10 weight is good
        public float heightSpringForce;
        // 2000 based off of 10 weight is good
        public float heightSpringDamper;
        // 2000 based off of 10 weight is good
        public float uprightSpringStrength;
        // 200 
        public float uprightSpringDamper;
        public float rotationSpeedMin;
        public float rotationSpeedDampener;
        public float jumpPower;
    }


    public static class CreatureTraitHelper {
        public static CreatureTraits CreateTraits(bool isPlayer, BaseCreature creature) {
            var traits = creature.bodyPart?.traits ?? new CreatureTraits();
            //base traits come from the body

            if (creature.headPart != null) {
                var otherTraits = creature.headPart.traits;
                traits = BasicAddition(traits, otherTraits, creature.headPart.slotType);
            }
            if (creature.armPart != null) {
                var otherTraits = creature.armPart.traits;
                traits = BasicAddition(traits, otherTraits, creature.armPart.slotType);
            }
            if (creature.legPart != null) {
                var otherTraits = creature.legPart.traits;
                traits = BasicAddition(traits, otherTraits, creature.legPart.slotType);
            }

            var uprightPerWeight = 200;
            traits.heightSpringForce = uprightPerWeight * traits.weight;
            traits.heightSpringDamper = traits.heightSpringForce * 0.1f;
            return traits;
        }
        
        public static CreatureTraits BasicAddition(CreatureTraits bodyTrait, CreatureTraits otherTrait, PartSlotType partSlotType) {
            var traits = bodyTrait;
            traits.weight += otherTrait.weight;
            traits.maxSpeed += otherTrait.maxSpeed;
            traits.acceleration += otherTrait.acceleration;
            traits.height += otherTrait.height;
            traits.rotationSpeedMin += otherTrait.rotationSpeedMin;
            traits.rotationSpeedDampener += otherTrait.rotationSpeedDampener;
            traits.jumpPower += otherTrait.jumpPower;
            return traits;
        }
    }
}