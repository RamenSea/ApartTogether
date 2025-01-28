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
                var headTraits = creature.headPart.traits;
                traits = BasicAddition(traits, headTraits, creature.headPart.slotType);
            }
            if (creature.armPart != null) {
                var headTraits = creature.headPart.traits;
                traits = BasicAddition(traits, headTraits, creature.headPart.slotType);
            }
            if (creature.headPart != null) {
                var headTraits = creature.headPart.traits;
                traits = BasicAddition(traits, headTraits, creature.headPart.slotType);
            }

            return traits;
        }
        
        public static CreatureTraits BasicAddition(CreatureTraits bodyTrait, CreatureTraits otherTrait, PartSlotType partSlotType) {
            var traits = bodyTrait;
            return traits;
        }
    }
}