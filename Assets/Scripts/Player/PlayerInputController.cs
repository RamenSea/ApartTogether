using System;
using UnityEngine;

namespace Player {
    public class PlayerInputController: MonoBehaviour {
        private InputAsset input;
        public Vector2 moveInput;
        public bool didPressLegAction;
        public bool didPressArmAction;
        public bool didPressHeadAction;
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
                this.didPressLegAction = this.input.Player.Jump.ReadValue<float>() > 0.5f;
                this.didPressArmAction = this.input.Player.Attack.ReadValue<float>() > 0.5f;
                this.didPressHeadAction = this.input.Player.Crouch.ReadValue<float>() > 0.5f;
            }
        }
    }
}