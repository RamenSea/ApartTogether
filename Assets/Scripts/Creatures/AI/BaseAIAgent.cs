using NaughtyAttributes;
using UnityEngine;

namespace Creatures.AI {
    public class BaseAIAgent: MonoBehaviour {
        public BaseCreature creature;

        public bool hasStarted = false;
        public bool chasingAIIsRunning = false;

        [Button("start AI")]
        public virtual void StartAI() {
            this.hasStarted = true;
        }
        public virtual void PostStart() {
        }
    }
}