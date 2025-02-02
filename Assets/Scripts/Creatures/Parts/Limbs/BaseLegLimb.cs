using System;
using Player;
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
        public float biasCreatureForward = 0f;
        public AnimationCurve stepHeightCurve;
        public float leftForwardTilt = 90f;
        public float rightForwardTilt = 90f;
        public float brokenLinkCheckPercent = 1f;
        public bool useForwardTilt = false;
        
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

        private bool hasSetLastMovingPlatformPosition = false;
        private Vector3 lastMovingPlatformPosition;
        // public Vector3 GetPlatformOffset() {
        //     if (this.creature.movingPlatformTransform != null) {
        //         return this.creature.movingPlatformTransform.position - this.platformPositionAtStep;
        //     }
        //     return Vector3.zero;
        // }
        private void SetStepLength() {
            this.lastStepLength = this.currentStepLength;
            this.currentStepLength = this.stepLength + Random.Range(-this.stepVariance, this.stepVariance) * this.stepLength;
        }
        private void SetStepPosition(Vector3 targetPosition) {
            if (this.creature.movingPlatformTransform != null) {
                this.lastMovingPlatformPosition = this.creature.movingPlatformTransform.position;
                this.hasSetLastMovingPlatformPosition = true;
            } else {
                this.hasSetLastMovingPlatformPosition = false;
            }
            
            var forward = this.GetStepForward();
            
            this.currentTargetPosition = this.finalTargetPosition;
            this.target.position = this.currentTargetPosition;
            
            this.finalTargetPosition = targetPosition;
            this.lastForward = new Vector2(forward.x, forward.z);
            this.actualStepLenght = this.transform.position.Distance(this.finalTargetPosition);
            this.SetStepLength();
        }

        private float actualStepLenght;
        private Vector3 GetStepForward(bool goingForward = false) {
            var myForward = this.transform.forward;
            myForward.y = 0;
            if (this.useForwardTilt) {
                var tilt = this.leftForwardTilt;
                if (!this.attachPoint.isLeft) {
                    tilt = this.rightForwardTilt;
                }
                var updatedDir = tilt.DegreeToDirection();
                var dir = this.transform.TransformDirection(new Vector3(updatedDir.x, 0, updatedDir.y));
                myForward.x += updatedDir.x;
                myForward.z += updatedDir.y;
                myForward = myForward.normalized;
                myForward = dir;
            }
            
            var creatureForward = this.creature.transform.forward;
            creatureForward.y = 0f;
            if (!goingForward) {
                creatureForward *= 1f;
            }

            var amountMyForward = 1.0f - this.biasCreatureForward;
            var f = myForward * amountMyForward + creatureForward * this.biasCreatureForward;
            return f.normalized;
        }
        public void ScrambleIdleStep(bool goingForward) {
            this.SetStepLength();
            var forward = this.GetStepForward(goingForward);
            forward.y = 0;
            var usingStepLength = this.currentStepLength * 0.5f;
            if (Physics.Raycast(this.transform.position + forward * usingStepLength, Vector3.down, out RaycastHit hit, this.findGroundHeight, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore)) {
                this.SetStepPosition(hit.point);
            }
        }
        protected override void OnGameUpdate(float deltaTime) {
            var movingPlatformAddition = Vector3.zero;
            if (this.creature.movingPlatformTransform != null) {
                if (this.hasSetLastMovingPlatformPosition) {
                    movingPlatformAddition = this.creature.movingPlatformTransform.position - this.lastMovingPlatformPosition;
                }

                this.currentTargetPosition += movingPlatformAddition;
                this.finalTargetPosition += movingPlatformAddition;
                this.hasSetLastMovingPlatformPosition = true;
                this.lastMovingPlatformPosition = this.creature.movingPlatformTransform.position;
            } else if (this.hasSetLastMovingPlatformPosition) {
                this.hasSetLastMovingPlatformPosition = false;
            }
            
            var currentPositionForSpeed = this.transform.position;
            currentPositionForSpeed.y = 0;
            var speed = this.creature.rb.linearVelocity.magnitude * deltaTime;
            this.lastPositionForSpeed = currentPositionForSpeed;
            var forward = this.GetStepForward();
            forward.y = 0;
            // if (PlayerDriverController.Instance.inputController.moveInput.x < -0.001f) {
            //     forward *= -1;
            // }
            //

            if (!this.creature.isOnGround) {
                forward = Vector3.zero;
            }
            var currentActualStepDistance = this.transform.position.Distance(this.finalTargetPosition);
            if (Physics.Raycast(this.transform.position + forward * this.currentStepLength, Vector3.down, out RaycastHit hit, this.findGroundHeight, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore)) {
                var distance = this.finalTargetPosition.Distance(hit.point);
                
                if (distance> this.currentStepLength * 2) {
                    this.SetStepPosition(hit.point);
                } else if (currentActualStepDistance > this.actualStepLenght * this.brokenLinkCheckPercent) {
                    // Debug.Log("broken position");
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
                var forward = this.GetStepForward();
                forward.y = 0;
                
                // if (!this.attachPoint.isLeft) {
                //     forward *= -1f;
                // }
                //
                // var usingStepLength = this.currentStepLength * 0.5f;
                // if (Physics.Raycast(this.transform.position + forward * usingStepLength, Vector3.down, out RaycastHit hit, this.findGroundHeight, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore)) {
                //     Gizmos.DrawWireSphere(hit.point, 0.05f);
                // }
                if (Physics.Raycast(this.transform.position + forward * this.currentStepLength, Vector3.down, out RaycastHit hit, this.findGroundHeight, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore)) {
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