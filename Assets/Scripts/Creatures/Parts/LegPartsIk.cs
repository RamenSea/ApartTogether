using System;
using RamenSea.Foundation3D.Extensions;
using UnityEngine;

namespace Creatures.Parts {
    public class LegPartsIk: PartsIKCollection {
        public Transform shootStepFrom;
        public Rigidbody baseRigidbody;
        public Transform forwardFrom;
        public Transform target;
        public float stepLength;
        public float stepHeight;
        public float findGroundHeight;
        public float minStepSpeed;
        
        private Vector3 finalTargetPosition;
        private Vector3 currentTargetPosition;
        private Vector3 lastPositionForSpeed;
        private Vector3 lastPositionForStep;

        private void Start() {
            this.finalTargetPosition = this.target.position;
            this.currentTargetPosition = this.finalTargetPosition;
            this.lastPositionForSpeed = this.transform.position;
            this.lastPositionForSpeed.y = 0;
        }

        private void Update() {
            var currentPositionForSpeed = this.transform.position;
            currentPositionForSpeed.y = 0;
            var speed = this.lastPositionForSpeed.Distance(currentPositionForSpeed);
            this.lastPositionForSpeed = currentPositionForSpeed;
            var forward = this.forwardFrom.forward;
            // forward = this.baseRigidbody.linearVelocity.normalized;
            forward.y = 0;
            if (Physics.Raycast(this.transform.position + forward * (this.stepLength), Vector3.down, out RaycastHit hit, this.findGroundHeight)) {
                if (this.finalTargetPosition.Distance(hit.point) > this.stepLength * 2) {
                    
                    var takingStep2 = this.currentTargetPosition - this.finalTargetPosition;
                    takingStep2 = takingStep2.Abs();
                    if (takingStep2.magnitude > 0.001f) {
                        
                    }

                    // this.currentTargetPosition = this.finalTargetPosition;
                    // this.target.position = this.currentTargetPosition;
                    this.finalTargetPosition = hit.point;
                }
            }

            var takingStep = this.currentTargetPosition - this.finalTargetPosition;
            takingStep = takingStep.Abs();
            if (takingStep.magnitude > 0.001f) {
                var steppingSpeed = Mathf.Max(this.minStepSpeed * Time.deltaTime, speed);
                // steppingSpeed = this.minStepSpeed * Time.deltaTime;
                Debug.Log(Mathf.Approximately(steppingSpeed, speed));
                Debug.Log(steppingSpeed);
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
                if (Physics.Raycast(this.transform.position + forward * this.stepLength, Vector3.down, out RaycastHit hit, this.findGroundHeight)) {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawWireSphere(hit.point, 0.03f);
                }
            }
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(this.finalTargetPosition, 0.03f);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(this.currentTargetPosition, 0.02f);

        }
    }
}