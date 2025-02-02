using Creatures.Collision;
using RamenSea.Foundation3D.Components.Audio;
using UnityEngine;

namespace Creatures.Parts {
    public class BaseHeadPart: BaseCreaturePart {

        public bool hasBiteAttack;
        public float biteCooldown;
        public float biteDamage;
        public float bitePushBack;
        public CreatureCollectorCollider collectorCollider;
        public VariationAudioSource biteSound;
        public ParticleSystem biteEffect;

        public float biteTimer;
        public bool CanAndReadyToBite() => this.collectorCollider.targets.Count > 0 && this.biteTimer <= 0f;
        
        protected override void OnGameUpdate(float deltaTime) {
            base.OnGameUpdate(deltaTime);

            if (this.hasBiteAttack) {
                if (this.biteTimer <= 0f && this.creature.doHeadAction) {
                    
                    this.biteSound.Play();
                    this.biteEffect.Stop();
                    this.biteEffect.Play();
                    
                    for (var i = 0; i < this.collectorCollider.targets.Count; i++) {
                        if (this.collectorCollider.targets[i].isDead) {
                            return;
                        }
                        this.collectorCollider.targets[i].TakeDamage(new DealDamage() {
                            amount = this.biteDamage,
                            damageType = DamageType.Direct,
                            fromLocation = this.transform.position,
                        });
                        this.collectorCollider.targets[i].rb.AddExplosionForce(this.bitePushBack, this.transform.position, this.collectorCollider.sphereCollider.radius, 0.3f);
                    }

                    this.biteTimer = this.biteCooldown;
                } else {
                    this.biteTimer -= deltaTime;
                }
            }
        }
    }
}