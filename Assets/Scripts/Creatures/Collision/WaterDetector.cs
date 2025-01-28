using System;
using UnityEngine;

namespace Creatures.Collision {
    public class WaterDetector: MonoBehaviour {
        public WaterCreatureInfo info;
        public BoxCollider collider;

        private void OnDestroy() {
            this.info = null;
        }
    }
}