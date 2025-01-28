using Creatures;
using Creatures.Parts;
using NaughtyAttributes;
using RamenSea.Foundation3D.Extensions;
using UnityEngine;

namespace Systems {
    public class CreatureManager: MonoBehaviour {

        public static CreatureManager Instance;
        
        [Layer] public int normalCreatureMask;
        [Layer] public int worldPartsMask;
        

        private void Awake() {
            CreatureManager.Instance = this;
        }

        public void DropPart(BaseCreaturePart part) {
            var partCollector = WorldPartCollector.Instance;
            var collector = partCollector.Get();
            part.OnDeattachToBody(true);
            collector.AttachPart(part);
        }

        public void CreatureDidDie(BaseCreature creature) {
            var partCollector = WorldPartCollector.Instance;
            if (creature.armPart != null) {
                var part = creature.armPart;
                var collector = partCollector.Get();
                part.OnDeattachToBody(true);
                collector.AttachPart(part);
            }
            if (creature.legPart != null) {
                var part = creature.legPart;
                var collector = partCollector.Get();
                part.OnDeattachToBody(true);
                collector.AttachPart(part);
            }
            if (creature.headPart != null) {
                var part = creature.headPart;
                var collector = partCollector.Get();
                part.OnDeattachToBody(true);
                collector.AttachPart(part);
            }
            if (creature.bodyPart != null) {
                var part = creature.bodyPart;
                creature.bodyPart = null;
                var collector = partCollector.Get();
                part.OnDeattachToBody(true);
                collector.AttachPart(part);
            }
            
            creature.gameObject.Destroy();
        }
        private void OnDestroy() {
            if (CreatureManager.Instance == this) {
                CreatureManager.Instance = null;
            }
        }
        
    }
}