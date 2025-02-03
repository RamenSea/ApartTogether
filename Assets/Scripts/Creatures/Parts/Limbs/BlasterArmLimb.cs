using System;
using Creatures.Collision;
using RamenSea.Foundation3D.Extensions;
using Systems;
using UnityEngine;

namespace Creatures.Parts.Limbs {
    public class BlasterArmLimb: ProjectileLimb {

        public Transform defaultTarget;
        public int lookingForType;

        public float timeTilLookForNewTarget;
        public BaseCreature targetCreature;
        public Transform targeter;
        public Transform turretAimer;

        public float armRange;
        public float range;
        public float targetSpeed;
        public float rotationSpeed;
        public float minHeight;
        
        private Collider[] cachedColliderArray;

        protected override void Awake() {
            base.Awake();
            this.cachedColliderArray = new Collider[20];
        }

        public override void OnAttachToBody(BaseBodyPart bodyPart, LimbAttachPoint toPoints) {
            base.OnAttachToBody(bodyPart, toPoints);
            if (this.creature.isPlayer) {
                this.lookingForType = 1 << CreatureManager.Instance.normalCreatureMask;
            } else {
                this.lookingForType = 1 << CreatureManager.Instance.playerMask;
            }
        }

        public override bool HasTarget() {
            return this.targetCreature != null;
        }

        public void FindTargets() {
            this.targetCreature = null;
            var bestDistance = 100000000f;
            Physics.OverlapSphereNonAlloc(this.transform.position, this.range, this.cachedColliderArray, this.lookingForType, QueryTriggerInteraction.Ignore);
            for (var i = 0; i < this.cachedColliderArray.Length; i++) {
                var collider = this.cachedColliderArray[i];
                if (collider == null) {
                    break;
                }
                var creatureCollider = collider.GetComponent<CreatureCollider>();
                if (creatureCollider.creature != null && !creatureCollider.creature.isDead) {
                    var distance = Vector3.Distance(this.transform.position, creatureCollider.creature.transform.position);
                    if (this.targetCreature == null || bestDistance < distance) {
                        bestDistance = distance;
                        this.targetCreature = creatureCollider.creature;
                    }
                    this.targetCreature = creatureCollider.creature;
                    break;
                }
            }
        }

        public void CheckDistance() {
            if (this.targetCreature != null) {
                var checkDistance = this.transform.position.Distance(this.targetCreature.transform.position);
                if (checkDistance > this.range) {
                    this.targetCreature = null;
                }
            }
        }
        protected override void OnGameUpdate(float deltaTime) {
            base.OnGameUpdate(deltaTime);
            this.timeTilLookForNewTarget -= deltaTime;
            if (this.timeTilLookForNewTarget < 0) {
                this.timeTilLookForNewTarget = 0.5f;
                this.FindTargets();
            }
            this.CheckDistance();

            var targetTransform = this.defaultTarget;
            if (this.targetCreature != null) {
                targetTransform = this.targetCreature.transform;
            } else {
                this.targetCreature = null;
            }

            var bestPosition = Vector3.MoveTowards(this.transform.position, targetTransform.position, this.armRange);
            if (bestPosition.y < this.transform.position.y) {
                bestPosition.y = this.transform.position.y;
            }
            var distance = this.targeter.position.Distance(bestPosition);
            if (distance > 0.001f) {
                this.targeter.position = Vector3.MoveTowards(this.targeter.position, bestPosition, this.targetSpeed * deltaTime);
            }
            this.turretAimer.LookAt(targetTransform.position);
        }
    }
}