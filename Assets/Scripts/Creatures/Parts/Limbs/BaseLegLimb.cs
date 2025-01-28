using System;
using RamenSea.Foundation3D.Extensions;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Creatures.Parts.Limbs {
    public class BaseLegLimb: BaseLimb {
        public Transform target;
        public float stepLength;
        public float stepHeight;
        public float findGroundHeight;
        public float minStepSpeed;
        public float offAngleTrigger;
        public float speedScale;
        public float stepVariance = 0.1f;
        public AnimationCurve stepHeightCurve;
        
        private Vector3 finalTargetPosition;
        private Vector3 currentTargetPosition;
        private Vector3 lastPositionForSpeed;
        private Vector3 lastPositionForStep;
        private Vector2 lastForward;
        private float lastStepLength;
        private float currentStepLength;
        
        private void Start() {
            this.finalTargetPosition = this.target.position;
            this.currentTargetPosition = this.finalTargetPosition;
            this.lastPositionForSpeed = this.transform.position;
            this.lastPositionForSpeed.y = 0;
            this.SetStepLength();
            this.lastStepLength = this.currentStepLength;
        }

        private void SetStepLength() {
            this.lastStepLength = this.currentStepLength;
            this.currentStepLength = this.stepLength + Random.Range(-this.stepVariance, this.stepVariance) * this.stepLength;
        }
        private void SetStepPosition(Vector3 targetPosition) {
            var forward = this.creature.transform.forward;
            this.finalTargetPosition = targetPosition;
            this.lastForward = new Vector2(forward.x, forward.z);
            this.SetStepLength();
        }
        public void ScrambleIdleStep(bool goingForward) {
            this.SetStepLength();
            var forward = this.transform.forward;
            forward.y = 0;
            if (!goingForward) {
                forward *= -1f;
            }

            var usingStepLength = this.currentStepLength * 0.5f;
            if (Physics.SphereCast(this.transform.position + forward * usingStepLength, 0.05f, new Vector3(forward.x, -1f, forward.y), out RaycastHit hit, this.findGroundHeight, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore)) {
                this.SetStepPosition(hit.point);
            }
        }
        protected override void OnGameUpdate(float deltaTime) {
            var currentPositionForSpeed = this.transform.position;
            currentPositionForSpeed.y = 0;
            var speed = this.lastPositionForSpeed.Distance(currentPositionForSpeed);
            this.lastPositionForSpeed = currentPositionForSpeed;
            var forward = this.transform.forward;
            forward.y = 0;
            if (Physics.SphereCast(this.transform.position + forward * (this.currentStepLength), 0.05f, new Vector3(forward.x, -1f, forward.y), out RaycastHit hit, this.findGroundHeight, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore)) {
                var distance = this.finalTargetPosition.Distance(hit.point);
                
                if (distance> this.currentStepLength * 2) {
                    this.SetStepPosition(hit.point);
                } else {
                    var angleDiff = Mathf.Abs(this.lastForward.Angle() - new Vector2(forward.x, forward.z).Angle());
                    if (angleDiff > this.offAngleTrigger) {
                        this.SetStepPosition(hit.point);
                    }
                }
            }

            var takingStep = this.currentTargetPosition - this.finalTargetPosition;
            takingStep = takingStep.Abs();
            if (takingStep.magnitude > 0.001f) {
                var actualStepLength = this.lastStepLength * 2.0f;
                var steppingSpeed = Mathf.Max(this.minStepSpeed * Time.deltaTime, speed * speedScale);
                // steppingSpeed = this.minStepSpeed * Time.deltaTime;
                this.currentTargetPosition = Vector3.MoveTowards(this.currentTargetPosition, this.finalTargetPosition, steppingSpeed);
                var distance = this.currentTargetPosition.Distance(this.finalTargetPosition);
                var workingPosition = this.currentTargetPosition;
                var progressOnStep = distance / actualStepLength;
                workingPosition.y += this.stepHeightCurve.Evaluate(progressOnStep) * this.stepHeight;
                this.target.position = workingPosition;
            } else {
                this.currentTargetPosition = this.finalTargetPosition;
                this.target.position = this.currentTargetPosition;
            }
        }

        private void OnDrawGizmos() {
            if (this.creature != null) {
                var forward = this.creature.transform.forward;
                forward.y = 0;
                
                if (Physics.SphereCast(this.transform.position + forward * (this.currentStepLength), 0.05f, new Vector3(forward.x, -1f, forward.y), out RaycastHit hit, this.findGroundHeight, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore)) {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawWireSphere(hit.point, 0.05f);
                }
            }
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(this.finalTargetPosition, 0.03f);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(this.currentTargetPosition, 0.02f);

        }
    }
}