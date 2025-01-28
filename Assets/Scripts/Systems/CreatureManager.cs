using Creatures;
using RamenSea.Foundation3D.Extensions;
using UnityEngine;

namespace Systems {
    public class CreatureManager: MonoBehaviour {

        public static CreatureManager Instance;
        

        private void Awake() {
            CreatureManager.Instance = this;
        }

        public void CreatureDidDie(BaseCreature creature) {
            var partCollector = WorldPartCollector.Instance;
            if (creature.armPart != null) {
                var collector = partCollector.Get();
                collector.AttachPart(creature.armPart);
            }
            if (creature.legPart != null) {
                var collector = partCollector.Get();
                collector.AttachPart(creature.legPart);
            }
            if (creature.headPart != null) {
                var collector = partCollector.Get();
                collector.AttachPart(creature.headPart);
            }
            if (creature.bodyPart != null) {
                var collector = partCollector.Get();
                collector.AttachPart(creature.bodyPart);
            }
            
            creature.Destroy();
        }
        private void OnDestroy() {
            if (CreatureManager.Instance == this) {
                CreatureManager.Instance = null;
            }
        }
        
    }
}