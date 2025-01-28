using System;
using System.Collections.Generic;
using UnityEngine;

namespace Creatures.Collision {
    public class WaterCreatureInfo: MonoBehaviour {
        public bool isTouchingWater;
        public bool startTouchingWater;
        public bool isSwimming;
        public bool startSwimming;
        
        [SerializeField] private WaterDetector swimmingDetector;
        [SerializeField] private WaterDetector waterDetector;

        private List<Collider> swimmingColliders;
        private List<Collider> waterColliders;

        private void Awake() {
            this.swimmingColliders = new();
            this.waterColliders = new();
        }

        private void Update() {
            var wasSwimming = this.isSwimming;
            var wasTouchingWater = this.isTouchingWater;
            
            this.isSwimming = this.swimmingColliders.Count > 0;
            this.isTouchingWater = this.waterColliders.Count > 0 || this.isSwimming;

            if (this.isSwimming && !wasSwimming) {
                this.startSwimming = true;
            } else {
                this.startSwimming = false;
            }
            if (this.isTouchingWater && !wasTouchingWater) {
                this.startTouchingWater = true;
            } else {
                this.startTouchingWater = false;
            }
        }

        public void SetColliders(BaseCreature creature) {
            var size = creature.bodyPart.bodyLimb.creatureCollider.collider.bounds.size;
            var h = size.y;
            size.y = creature.compiledTraits.height;
            this.waterDetector.collider.size = size;
            size.y = h * 0.8f;
            this.swimmingDetector.collider.size = size;
        }

        public void OnDetectWater(Collider waterCollider, WaterDetector waterDetector) {
            Debug.Log("OnDetectWater");
            var l = this.swimmingColliders;
            if (waterDetector == this.waterDetector) {
                l = this.waterColliders;
            }

            if (!l.Contains(waterCollider)) {
                l.Add(waterCollider);
            }
        }
        public void OnRemoveDetectWater(Collider waterCollider, WaterDetector waterDetector) {
            var l = this.swimmingColliders;
            if (waterDetector == this.waterDetector) {
                l = this.waterColliders;
            }

            if (l.Contains(waterCollider)) {
                l.Remove(waterCollider);
            }
        }
    }
}