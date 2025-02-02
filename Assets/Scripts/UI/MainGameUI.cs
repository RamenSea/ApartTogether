using System;
using Creatures;
using Player;
using Systems;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class MainGameUI: MonoBehaviour {
        
        [SerializeField] private Image _healthBar;
        
        private void Update() {


            if (PlayerDriverController.Instance.creature != null) {
                var hpPercent = PlayerDriverController.Instance.creature.health / BaseCreature.MAX_HEALTH;
                this._healthBar.rectTransform.anchorMax = new Vector2(hpPercent, this._healthBar.rectTransform.anchorMax.y);
            }
            
        }

        public void ShowHint() {
            GameRunner.Instance.hint.ShowHint();
        }
    }
}