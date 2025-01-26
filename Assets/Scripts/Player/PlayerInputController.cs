using System;
using UnityEngine;

namespace Player {
    public class PlayerInputController: MonoBehaviour {
        private InputAsset input;
        public Vector2 moveInput;
        private bool updateInput = true;
        private void Awake() {
            this.input = new InputAsset();
        }
        private void OnEnable() {
            this.input.Enable();
        }
        private void OnDisable() {
            this.input.Disable();
        }
        private void Update() {
            this.updateInput = true;
        }
        private void FixedUpdate() {
            if (this.updateInput) {
                this.updateInput = false;
                this.moveInput = this.input.Player.Move.ReadValue<Vector2>();
            }
        }
    }
}