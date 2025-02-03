using Creatures.Projectiles;
using JetBrains.Annotations;
using RamenSea.Foundation3D.Components.Recyclers;
using UnityEngine;

namespace Creatures.Parts.Limbs {
    public class ProjectileLimb: BaseLimb, IBaseProjectileListener{
        
        [SerializeField] public BaseProjectile projectilePrefab;
        [SerializeField] public float fireRate;
        [SerializeField] public Transform targetPivot;
        [SerializeField] public Transform fireFrom;
        [SerializeField] public float projectileSpeed;
        [SerializeField] public float damageAmount;
        [SerializeField] public float pushForce;
        [SerializeField] public float pushRadius;
        [SerializeField] public float bulletActivateTime;
        [SerializeField] public Vector3 bulletGravity;

        public float fireCooldown;
        
        public PrefabRecycler<BaseProjectile> prefabRecycler;

        protected virtual void Awake() {
            this.prefabRecycler = new PrefabRecycler<BaseProjectile>(this.projectilePrefab);
        }

        public override void OnAttachToBody(BaseBodyPart bodyPart, LimbAttachPoint toPoints) {
            base.OnAttachToBody(bodyPart, toPoints);
        }

        public virtual bool HasTarget() => true;
        protected virtual bool ShouldFire() => this.creature?.doArmsAction ?? false;
        protected override void OnGameUpdate(float deltaTime) {
            base.OnGameUpdate(deltaTime);

            if (this.fireCooldown > 0) {
                this.fireCooldown -= deltaTime;
            }

            if (this.fireCooldown <= 0 && this.ShouldFire()) {
                this.Fire();
            }
        }

        protected void FaceCannon(Vector3 towards) {
            this.targetPivot.LookAt(towards);
        }

        protected virtual void Fire() {
            this.fireCooldown = this.fireRate;
            var projectile = this.prefabRecycler.Get();
            projectile.recycler = this.prefabRecycler;
            projectile.listener = this;
            projectile.speed = this.projectileSpeed;
            projectile.FireStraight(this.fireFrom.position, this.fireFrom.forward, this.bulletActivateTime, this.bulletGravity);
        }

        public void DidHit(BaseProjectile projectile, [CanBeNull] BaseCreature creature) {
            var damage = new DealDamage() {
                damageType = DamageType.Direct,
                fromLocation = projectile.transform.position,
                amount = this.damageAmount,
            };
            Debug.Log($"HITTING Player${creature?.gameObject.name}");
            if (projectile.collectorCollider != null) {
                for (var i = 0; i < projectile.collectorCollider.targets.Count; i++) {
                    var c = projectile.collectorCollider.targets[i];
                    c.TakeDamage(damage);
                    if (this.pushForce > 0.001f) {
                        c.rb.AddExplosionForce(this.pushForce, projectile.transform.position, this.pushRadius, 0.4f);
                    }
                }
            } else if (creature != null) {
                Debug.Log($"HITTING Player${creature.gameObject.name}");
                if (this.pushForce > 0.001f) {
                    creature.rb.AddExplosionForce(this.pushForce, projectile.transform.position, this.pushRadius, 0.4f);
                }
                creature.TakeDamage(damage);
            }
            projectile.Explode();
        }

    }
}