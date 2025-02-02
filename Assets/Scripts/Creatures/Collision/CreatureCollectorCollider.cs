using System.Collections.Generic;
using Player;
using RamenSea.Foundation3D.Extensions;
using Systems;
using Unity.VisualScripting;
using UnityEngine;

namespace Creatures.Collision {
    public class CreatureCollectorCollider: CreatureCollisionDetection {
        public SphereCollider sphereCollider;

        public List<BaseCreature> targets;

        private void Awake() {
            this.targets = new();
        }

        public void SetValues(float range, int mask) {
            this.sphereCollider.radius = range * 0.5f;
            // this.sphereCollider.includeLayers = mask;
            // this.sphereCollider.excludeLayers = ~this.sphereCollider.includeLayers;
        }
        public void AddTarget(BaseCreature creature) {
            if (!this.targets.Contains(creature)) {
                this.targets.Add(creature);
            }
        }
        public void RemoveTarget(BaseCreature creature) {
            if (this.targets.Contains(creature)) {
                this.targets.Remove(creature);
            }
        }
        public override void OnTriggerEnter(Collider other) {
            if (other.gameObject.CompareTag(GameTags.Creature)) {
                var detector = other.gameObject.GetComponent<CreatureCollider>();
                this.AddTarget(detector.creature);
            }
            base.OnTriggerEnter(other);
        }
        public override void OnTriggerExit(Collider other) {
            if (other.gameObject.CompareTag(GameTags.Creature)) {
                var detector = other.gameObject.GetComponent<CreatureCollider>();
                this.RemoveTarget(detector.creature);
            }
            base.OnTriggerExit(other);
        }
    }
}