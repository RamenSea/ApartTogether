using System;
using Creatures;
using Creatures.Parts;
using Creatures.Parts.Limbs;
using Player;
using Systems;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class MainGameUI: MonoBehaviour {
        
        [SerializeField] private Image _healthBar;
        [SerializeField] private Button jumpButton;
        [SerializeField] private RectTransform jumpButtonRect;
        [SerializeField] private Button biteButton;
        [SerializeField] private RectTransform biteButtonRect;
        [SerializeField] private Button interactButton;
        [SerializeField] private RectTransform interactButtonRect;
        [SerializeField] private Button shootButton;
        [SerializeField] private RectTransform shootButtonRect;
        
        private void Update() {
            if (PlayerDriverController.Instance.creature == null) {
                return;
            }
            var hpPercent = PlayerDriverController.Instance.creature.health / BaseCreature.MAX_HEALTH;
            this._healthBar.rectTransform.anchorMax = new Vector2(hpPercent, this._healthBar.rectTransform.anchorMax.y);
            
            var armPart = PlayerDriverController.Instance.creature.armPart;
            if (armPart != null && armPart.limbs.Length > 0 && armPart.limbs[0] is ProjectileLimb l) {
                this.shootButton.interactable = true;

                var percent = 1f;
                if (l.fireCooldown > 0f) {
                    percent = (l.fireRate - l.fireCooldown) / l.fireRate;
                }
                percent = 1f - percent;
                
                var shootOffset = this.shootButton.image.rectTransform.offsetMax;
                shootOffset.y = -percent * this.shootButtonRect.rect.height;
                this.shootButton.image.rectTransform.offsetMax = shootOffset;

            } else {
                this.shootButton.interactable = false;
            }
            
            var headPart = PlayerDriverController.Instance.creature.headPart;
            if (headPart != null && headPart is BaseHeadPart castedHead && castedHead.hasBiteAttack) {
                this.biteButton.interactable = true;

                var percent = 1f;
                if (castedHead.biteTimer > 0f) {
                    percent = (castedHead.biteCooldown - castedHead.biteTimer) / castedHead.biteCooldown;
                }
                percent = 1f - percent;

                var shootOffset = this.biteButton.image.rectTransform.offsetMax;
                shootOffset.y = -percent * this.biteButtonRect.rect.height;
                this.biteButton.image.rectTransform.offsetMax = shootOffset;

            } else {
                this.biteButton.interactable = false;
            }

            this.interactButton.interactable = WorldPartCollector.Instance.CanInteract();

        }

        public void ShowHint() {
            GameRunner.Instance.hint.ShowHint();
        }
        
    }
}