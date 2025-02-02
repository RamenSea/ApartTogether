using System;
using Creatures;
using Creatures.Collision;
using Player;
using UnityEngine;

namespace Systems {
    public class PressDownButton: MonoBehaviour, ICreatureCollisionDetectionListener {
        public CreatureCollisionDetection collisionDetection;
        public Rigidbody rb;
        public float heightSpringForce;
        public float heightSpringDamper;
        public float height;
        public float forceMultiplier;
        public float triggerPoint;
        public bool hasPlayer;

        private void Start() {
            this.collisionDetection.listener = this;
        }

        private void Update() {
            if (this.transform.localPosition.y < this.triggerPoint) {
                Debug.Log("damn triggerd");
                GameRunner.Instance.BlowUpDam();
            }
        }

        private void FixedUpdate() {
            var mask = 1 << CreatureManager.Instance.groundMask;
            var usingDown = Vector3.down;
            RaycastHit hit;
            if (Physics.Raycast(this.transform.position, Vector3.down, out hit, height, ~mask, QueryTriggerInteraction.Ignore)) {
                var velocity = rb.linearVelocity;
                var rayDir = usingDown;


                
                var rayDirVel = Vector3.Dot(rayDir, velocity);
                var relVel = rayDirVel;
                var x = hit.distance - this.height;
                var springForce = (x * this.heightSpringForce) -
                    (relVel * this.heightSpringDamper);
                
                this.rb.AddForce(rayDir * springForce);
            }

            if (this.hasPlayer) {
                this.rb.AddForce(Vector3.down * this.forceMultiplier * Time.fixedDeltaTime * PlayerDriverController.Instance.creature.rb.mass);
            }
        }

        public void OnCreatureTriggerEnter(BaseCreature creature) {
            if (creature.isPlayer) {
                this.hasPlayer = true;
                if (creature.rb.mass >= 20) {
                    GameRunner.Instance.BlowUpDam();
                }
            }
        }

        public void OnCreatureTriggerExit(BaseCreature creature) {
            if (creature.isPlayer) {
                this.hasPlayer = false;
            }
        }
    }
}