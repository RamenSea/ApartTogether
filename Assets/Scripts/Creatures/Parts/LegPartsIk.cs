using System;
using RamenSea.Foundation3D.Extensions;
using UnityEngine;

namespace Creatures.Parts {
    public class LegPartsIk: PartsIKCollection {
        public Transform forwardFrom;
        public Transform target;
        public float stepLength;
        public float stepHeight;
        public float findGroundHeight;
        public float minStepSpeed;
        public float offAngleTrigger;
        public float speedScale;
        
        private Vector3 finalTargetPosition;
        private Vector3 currentTargetPosition;
        private Vector3 lastPositionForSpeed;
        private Vector3 lastPositionForStep;
        private Vector2 lastForward;

        private void Start() {
            this.finalTargetPosition = this.target.position;
            this.currentTargetPosition = this.finalTargetPosition;
            this.lastPositionForSpeed = this.transform.position;
            this.lastPositionForSpeed.y = 0;
        }

        private void SetStepPosition(Vector3 targetPosition) {

            var takingStep = this.currentTargetPosition - this.finalTargetPosition;
            takingStep = takingStep.Abs();
            if (takingStep.magnitude > 0.001f) {
                
                Debug.Log(takingStep.magnitude);
                
            }

            var forward = this.forwardFrom.forward;
            this.finalTargetPosition = targetPosition;
            this.lastForward = new Vector2(forward.x, forward.z);
        }
        private void Update() {
            var currentPositionForSpeed = this.transform.position;
            currentPositionForSpeed.y = 0;
            var speed = this.lastPositionForSpeed.Distance(currentPositionForSpeed);
            this.lastPositionForSpeed = currentPositionForSpeed;
            // forward = this.baseRigidbody.linearVelocity.normalized;
            var forward = this.forwardFrom.forward;
            forward.y = 0;
            if (Physics.SphereCast(this.transform.position + forward * (this.stepLength), 0.05f, new Vector3(forward.x, -1f, forward.y), out RaycastHit hit, this.findGroundHeight)) {
                var distance = this.finalTargetPosition.Distance(hit.point);
                // if (this.isDown && distance > this.stepLength * 2) {
                //     
                //     Debug.Log("Distance step");
                //     this.SetStepPosition(hit.point);
                // } else if (distance > this.stepLength * 2)
                //
                //
                if (distance> this.stepLength * 2) {
                    this.SetStepPosition(hit.point);
                } else {
                    var angleDiff = Mathf.Abs(this.lastForward.Angle() - new Vector2(forward.x, forward.z).Angle());
                    if (angleDiff > this.offAngleTrigger) {
                        Debug.Log("STEP");
                        Debug.Log(angleDiff);
                        this.SetStepPosition(hit.point);
                    }

                }

            }

            var takingStep = this.currentTargetPosition - this.finalTargetPosition;
            takingStep = takingStep.Abs();
            if (takingStep.magnitude > 0.001f) {
                var steppingSpeed = Mathf.Max(this.minStepSpeed * Time.deltaTime, speed * speedScale);
                // steppingSpeed = this.minStepSpeed * Time.deltaTime;
                this.currentTargetPosition = Vector3.MoveTowards(this.currentTargetPosition, this.finalTargetPosition, steppingSpeed);
                var distance = this.currentTargetPosition.Distance(this.finalTargetPosition);
                var workingPosition = this.currentTargetPosition;
                workingPosition.y += Mathf.Sin(distance / this.stepLength * Mathf.PI) * this.stepHeight;
                this.target.position = workingPosition;
            } else {
                this.currentTargetPosition = this.finalTargetPosition;
                this.target.position = this.currentTargetPosition;
            }
            
        }

        private void OnDrawGizmos() {
            if (this.forwardFrom != null) {
                var forward = this.forwardFrom.forward;
                forward.y = 0;
                
                if (Physics.SphereCast(this.transform.position + forward * (this.stepLength), 0.05f, new Vector3(forward.x, -1f, forward.y), out RaycastHit hit, this.findGroundHeight)) {
                // if (Physics.Raycast(this.transform.position + forward * this.stepLength, Vector3.down, out RaycastHit hit, this.findGroundHeight)) {
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