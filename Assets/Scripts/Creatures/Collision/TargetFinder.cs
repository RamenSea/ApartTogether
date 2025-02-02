using System.Collections.Generic;
using Player;
using RamenSea.Foundation3D.Extensions;
using Systems;
using UnityEngine;

namespace Creatures.Collision {
    public class TargetFinder: MonoBehaviour {
        public Transform measureFrom;
        public SphereCollider sphereCollider;


        public BaseCreature bestTarget;
        public List<BaseCreature> targets;

        public float findBestTargetTimer = 1f;
        private void Awake() {
            this.targets = new();
        }

        private void Update() {
            this.findBestTargetTimer -= Time.deltaTime;
            if (this.findBestTargetTimer < 0f) {
                this.findBestTargetTimer = 1f;
            }
        }
        public void FindBestTarget() {
            this.bestTarget = null;
            var distanceForTarget = 10000000f;
            for (var i = 0; i < this.targets.Count; i++) {
                var d = this.targets[i].transform.position.Distance(this.measureFrom.position);

                if (this.bestTarget == null || d < distanceForTarget) {
                    this.bestTarget = this.targets[i];
                    distanceForTarget = d;
                }
            }
        }
        public void SetValues(float range, bool isPlayer) {
            this.sphereCollider.radius = range * 0.5f;
            this.sphereCollider.includeLayers = 1 << (isPlayer ? CreatureManager.Instance.normalCreatureMask : CreatureManager.Instance.playerMask);
            this.sphereCollider.excludeLayers = ~this.sphereCollider.includeLayers;
            this.transform.localPosition = new Vector3(0, 0, range * 0.5f);
        }
        public void AddTarget(BaseCreature creature) {
            if (!this.targets.Contains(creature)) {
                this.targets.Add(creature);
                this.FindBestTarget();
            }
        }
        public void RemoveTarget(BaseCreature creature) {
            if (this.targets.Contains(creature)) {
                this.targets.Remove(creature);
                this.FindBestTarget();
            }
        }
        private void OnTriggerEnter(Collider other) {
            if (other.gameObject.CompareTag(GameTags.Creature)) {
                var detector = other.gameObject.GetComponent<CreatureCollider>();
                this.AddTarget(detector.creature);
            }
        }
        private void OnTriggerExit(Collider other) {
            if (other.gameObject.CompareTag(GameTags.Creature)) {
                var detector = other.gameObject.GetComponent<CreatureCollider>();
                this.RemoveTarget(detector.creature);
            }
        }
    }
}