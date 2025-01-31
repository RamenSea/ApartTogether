using System;
using Creatures.Parts;

namespace Creatures {
    
    [Serializable]
    public struct CreatureTraits {
        public float height;
        public float weight;
        
        // move speed related
        public float maxSpeedGround;
        public float accelerationGround;
        public float decelerationGround;
        public float maxSpeedAir;
        public float accelerationAir;
        public float decelerationAir;
        public float maxSpeedWater;
        public float accelerationWater;
        public float decelerationWater;
        public float effectsGravity;
        public float effectsWaterGravity;
        
        public float rotationSpeedMin;
        public float rotationSpeedDampener;
        
        public float jumpPower;
        public float jumpPowerHold;
        public bool enableFlapFlight;
        public float flapFlightPower;
        public float flapDuration;
        
        // All post calculated
        // 2000 based off of 10 weight is good
        public float heightSpringForce;
        // 2000 based off of 10 weight is good
        public float heightSpringDamper;
        // 2000 based off of 10 weight is good
        public float uprightSpringStrength;
        // 200 
        public float uprightSpringDamper;
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

            var uprightPerWeight = 200f;
            traits.heightSpringForce = uprightPerWeight * traits.weight;
            traits.heightSpringDamper = traits.heightSpringForce * 0.1f;
            var rotationFixPerWeight = 200f;
            traits.uprightSpringStrength = rotationFixPerWeight * traits.weight;
            traits.uprightSpringDamper = traits.uprightSpringStrength * 0.1f;
            return traits;
        }
        
        public static CreatureTraits BasicAddition(CreatureTraits bodyTrait, CreatureTraits otherTrait, PartSlotType partSlotType) {
            var traits = bodyTrait;
            traits.weight += otherTrait.weight;
            traits.height += otherTrait.height;

            traits.maxSpeedGround += otherTrait.maxSpeedGround;
            traits.accelerationGround += otherTrait.accelerationGround;
            traits.decelerationGround += otherTrait.decelerationGround;
            
            traits.maxSpeedWater += otherTrait.maxSpeedWater;
            traits.accelerationWater += otherTrait.accelerationWater;
            traits.decelerationWater += otherTrait.decelerationWater;
            
            traits.maxSpeedAir += otherTrait.maxSpeedAir;
            traits.accelerationAir += otherTrait.accelerationAir;
            traits.decelerationAir += otherTrait.decelerationAir;
            
            traits.rotationSpeedMin += otherTrait.rotationSpeedMin;
            traits.rotationSpeedDampener += otherTrait.rotationSpeedDampener;
            traits.jumpPower += otherTrait.jumpPower;
            traits.jumpPowerHold += otherTrait.jumpPowerHold;
            
            traits.effectsGravity *= otherTrait.effectsGravity;
            traits.effectsWaterGravity *= otherTrait.effectsWaterGravity;

            if (otherTrait.enableFlapFlight) {
                traits.enableFlapFlight = true;
            }
            traits.flapFlightPower += otherTrait.flapFlightPower;
            traits.flapDuration += otherTrait.flapDuration;
            return traits;
        }
    }
}