using System;
using UnityEngine;

namespace Creatures.Collision {
    public class WaterTrigger: MonoBehaviour {
        private void OnTriggerEnter(Collider other) {
            if (other.gameObject.CompareTag(GameTags.Creature)) {
                var detector = other.gameObject.GetComponent<WaterDetector>();
                detector.info.OnDetectWater(other, detector);
            }
        }
        private void OnTriggerExit(Collider other) {
            if (other.gameObject.CompareTag(GameTags.Creature)) {
                var detector = other.gameObject.GetComponent<WaterDetector>();
                detector.info.OnRemoveDetectWater(other, detector);
            }
        }
    }
}