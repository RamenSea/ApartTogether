using Creatures.Projectiles;
using RamenSea.Foundation3D.Components.Recyclers;
using UnityEngine;

namespace Creatures.Parts.Limbs {
    public class ProjectileLimb: BaseLimb, IBaseProjectileListener{
        
        [SerializeField] protected BaseProjectile projectilePrefab;
        [SerializeField] protected float fireRate;
        [SerializeField] protected Transform targetPivot;
        [SerializeField] protected Transform fireFrom;
        [SerializeField] protected float projectileSpeed;
        [SerializeField] protected int damageAmount;

        protected float fireCooldown;
        
        protected PrefabRecycler<BaseProjectile> prefabRecycler;

        protected virtual void Awake() {
            this.prefabRecycler = new PrefabRecycler<BaseProjectile>(this.projectilePrefab);
        }

        public override void OnAttachToBody(BaseBodyPart bodyPart, LimbAttachPoint toPoints) {
            base.OnAttachToBody(bodyPart, toPoints);
            this.FaceCannon(toPoints.transform.forward);
        }

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
            projectile.FireStraight(this.fireFrom.position, this.fireFrom.forward);
        }

        public void DidHitCreature(BaseProjectile projectile, BaseCreature creature) {
            this.prefabRecycler.Recycle(projectile);
            creature.TakeDamage(new DealDamage(){ damageType = DamageType.Direct, amount = this.damageAmount});
        }

        public void DidHitOther(BaseProjectile projectile, Collider other) {
            this.prefabRecycler.Recycle(projectile);
        }
    }
}