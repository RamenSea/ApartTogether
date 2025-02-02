using System;
using UnityEngine;

namespace Creatures.AI {
    public class AIAgentRunForward: BaseAIAgent {
        public Vector3 direction;

        private void Update() {
            if (this.creature == null) {
                return;
            }
            
            if (this.chasingAIIsRunning) {
                return;
            }
            this.creature.moveDirection = this.direction;
        }
    }
}