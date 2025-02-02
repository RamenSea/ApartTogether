using Creatures;
using Creatures.Parts;
using NaughtyAttributes;
using RamenSea.Foundation3D.Extensions;
using UnityEngine;

namespace Systems {
    public class CreatureManager: MonoBehaviour {

        public static CreatureManager Instance;
        
        [Layer] public int playerMask;
        [Layer] public int defaultLayerMask;
        [Layer] public int normalCreatureMask;
        [Layer] public int worldPartsMask;
        [Layer] public int groundMask;
        

        private void Awake() {
            CreatureManager.Instance = this;
        }
        public void CreatureDidDie(BaseCreature creature) {
            var partCollector = WorldPartCollector.Instance;
            if (creature.armPart != null) {
                var part = creature.armPart;
                partCollector.DropPart(part);
            }
            if (creature.legPart != null) {
                var part = creature.legPart;
                partCollector.DropPart(part);
            }
            if (creature.headPart != null) {
                var part = creature.headPart;
                partCollector.DropPart(part);
            }
            if (creature.bodyPart != null) {
                var part = creature.bodyPart;
                partCollector.DropPart(part);
            }
            creature.OnDeath();

            if (creature.isPlayer) {
                GameRunner.Instance.PlayerDidDie();
            }
        }
        private void OnDestroy() {
            if (CreatureManager.Instance == this) {
                CreatureManager.Instance = null;
            }
        }
        
    }
}