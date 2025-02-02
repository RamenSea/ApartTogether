using System;
using Creatures.Collision;
using RamenSea.Foundation3D.Extensions;
using Systems;
using UnityEngine;

namespace Creatures.Parts.Limbs {
    public class TurretAimLimb: ProjectileLimb {

        public BaseCreature targetCreature;
        public Transform turretAimer;
        public TargetFinder targetFinder;

        public float range;
        public float rotationSpeed;

        public override void OnAttachToBody(BaseBodyPart bodyPart, LimbAttachPoint toPoints) {
            base.OnAttachToBody(bodyPart, toPoints);
            this.targetFinder.enabled = true;
            this.targetFinder.targets.Clear();
            this.targetFinder.bestTarget = null;
            this.targetFinder.SetValues(this.range, this.creature.isPlayer);
        }

        public override void OnDeattachBody() {
            base.OnDeattachBody();
            this.targetFinder.targets.Clear();
            this.targetFinder.bestTarget = null;
            this.targetFinder.enabled = false;
        }

        public override bool HasTarget() {
            return this.targetFinder.bestTarget != null;
        }

        protected override void OnGameUpdate(float deltaTime) {
            base.OnGameUpdate(deltaTime);

            Quaternion targetRotation = Quaternion.identity;
            bool useLocal = true;
            if (this.targetFinder.bestTarget != null) {
                var direction = this.turretAimer.position.Direction(this.targetFinder.bestTarget.transform.position);
                targetRotation = Quaternion.LookRotation(direction);
                useLocal = false;
            }

            if (useLocal) {
                this.turretAimer.localRotation = Quaternion.RotateTowards(this.turretAimer.localRotation, targetRotation, rotationSpeed * deltaTime);
            } else {
                this.turretAimer.rotation = Quaternion.RotateTowards(this.turretAimer.rotation, targetRotation, rotationSpeed * deltaTime);
            }
        }
    }
}