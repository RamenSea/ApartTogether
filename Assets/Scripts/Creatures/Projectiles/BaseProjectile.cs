using System;
using Creatures.Collision;
using JetBrains.Annotations;
using NaughtyAttributes;
using RamenSea.Foundation3D.Components.Recyclers;
using RamenSea.Foundation3D.Components.Audio;
using UnityEngine;

namespace Creatures.Projectiles {
    public interface IBaseProjectileListener {
        public void DidHit(BaseProjectile projectile, [CanBeNull] BaseCreature creature);
    }
    public class BaseProjectile: RecyclableObject, ICreatureCollisionDetectionListener {
        public IBaseProjectileListener listener;
        
        public float speed;
        public float lifetimer;
        [SerializeField] protected bool isFired;
        [SerializeField] protected bool hasExploded;
        [SerializeField] protected CreatureCollisionDetection collisionDetection;
        [SerializeField] protected float longestLifeTime = 10f;
        [SerializeField] protected float timeTilRecycle = 0.01f;

        private float recycleTimer;
        public CreatureCollectorCollider collectorCollider;
        public GameObject model;
        public ParticleSystem explosionSystem;
        public VariationAudioSource audioSource;
        public Vector3 bulletGravity;

        public float timeTilActivateCollision;
        private void Start() {
            this.collisionDetection.listener = this;
        }

        private void Update() {
            this.lifetimer += Time.deltaTime;
            if (this.lifetimer >= this.longestLifeTime) {
                this.hasExploded = true;
                this.recycler.Recycle(this);
            }

            if (this.recycleTimer > 0) {
                this.recycleTimer -= Time.deltaTime;
                if (this.recycleTimer <= 0) {
                    this.recycler.Recycle(this);
                    this.collectorCollider.targets.Clear();
                    if (this.explosionSystem != null) {
                        this.explosionSystem.gameObject.SetActive(false);
                    }
                }
            }
            
            if (this.timeTilActivateCollision > 0f) {
                this.timeTilActivateCollision -= Time.deltaTime;
                if (this.timeTilActivateCollision <= 0f) {
                    if (this.collectorCollider != null && this.collectorCollider.targets.Count > 0) {
                        this.listener?.DidHit(this, this.collectorCollider.targets[0]);
                    }
                }
            }
        }

        private void FixedUpdate() {
            if (!this.isFired || this.hasExploded) {
                return;
            }
            
            this.transform.position += this.transform.forward * (this.speed * Time.fixedDeltaTime);
            this.transform.position += this.bulletGravity * Time.fixedDeltaTime;
        }

        public void FireStraight(Vector3 from, Vector3 direction, float timeTilActivateCollision, Vector3 bulletGravity) {
            this.timeTilActivateCollision = timeTilActivateCollision;
            this.model.SetActive(true);
            this.lifetimer = 0f;
            this.isFired = true;
            this.hasExploded = false;
            this.transform.position = from;
            this.transform.rotation = Quaternion.LookRotation(direction);
            this.collisionDetection.gameObject.SetActive(true);
            if (this.collectorCollider != null) {
                this.collectorCollider.targets.Clear();
            }

            this.bulletGravity = bulletGravity;
        }

        [Button("explode")]
        public virtual void Explode() {
            this.hasExploded = true;
            this.model.SetActive(false);
            this.recycleTimer = this.timeTilRecycle;
            this.collisionDetection.gameObject.SetActive(false);
            
            if (this.explosionSystem != null) {
                this.explosionSystem.gameObject.SetActive(true);
                this.explosionSystem.Clear();
                this.explosionSystem.Play();
            }
            if (this.audioSource != null) {
                this.audioSource.Play();
            }
        }

        public void OnCreatureCollisionEnter(BaseCreature creature, UnityEngine.Collision collision) {
            if (this.timeTilActivateCollision > 0f) {
                return;
            }
            this.listener?.DidHit(this, creature);
        }
        public void OnCreatureTriggerEnter(BaseCreature creature) {
            if (this.timeTilActivateCollision > 0f) {
                return;
            }
            this.listener?.DidHit(this, creature);
        }
        public void OnOtherCollisionEnter(UnityEngine.Collision collision) {
            this.listener?.DidHit(this, null);
        }
        public void OnOtherTriggerEnter(Collider other) {
            this.listener?.DidHit(this, null);
        }
    }
}