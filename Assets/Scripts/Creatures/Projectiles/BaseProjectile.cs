using System;
using Creatures.Collision;
using RamenSea.Foundation3D.Components.Recyclers;
using UnityEngine;

namespace Creatures.Projectiles {
    public interface IBaseProjectileListener {
        public void DidHitCreature(BaseProjectile projectile, BaseCreature creature);
        public void DidHitOther(BaseProjectile projectile, Collider other);
    }
    public class BaseProjectile: RecyclableObject, ICreatureCollisionDetectionListener {
        public IBaseProjectileListener listener;
        
        public float speed;
        public float lifetimer;
        [SerializeField] protected bool isFired;
        [SerializeField] protected bool hasExploded;
        [SerializeField] protected CreatureCollisionDetection collisionDetection;
        [SerializeField] protected float longestLifeTime = 10f;

        private void Start() {
            this.collisionDetection.listener = this;
        }

        private void Update() {
            this.lifetimer += Time.deltaTime;
            if (this.lifetimer >= this.longestLifeTime) {
                this.hasExploded = true;
                this.recycler.Recycle(this);
            }
        }

        private void FixedUpdate() {
            if (!this.isFired || this.hasExploded) {
                return;
            }
            
            this.transform.position += this.transform.forward * (this.speed * Time.fixedDeltaTime);
        }

        public void FireStraight(Vector3 from, Vector3 direction) {
            this.lifetimer = 0f;
            this.isFired = true;
            this.hasExploded = false;
            this.transform.position = from;
            this.transform.rotation = Quaternion.LookRotation(direction);
        }

        public virtual void Explode() {
            this.hasExploded = true;
            this.recycler.Recycle(this);
        }

        public void OnCreatureCollisionEnter(BaseCreature creature, UnityEngine.Collision collision) {
            this.listener?.DidHitCreature(this, creature);
        }
        public void OnCreatureTriggerEnter(BaseCreature creature) {
            this.listener?.DidHitCreature(this, creature);
        }
        public void OnOtherCollisionEnter(UnityEngine.Collision collision) {
        }
        public void OnOtherTriggerEnter(Collider other) {
        }
    }
}