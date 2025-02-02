using System;
using UnityEngine;

namespace Systems {
    public class RiverLevel: MonoBehaviour {
        
        public static RiverLevel Instance;
        
        private void Awake() {
            Instance = this;
        }

        private void OnDestroy() {
            if (Instance == this) {
                Instance = null;
            }
        }
    }
}