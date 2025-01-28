using System;
using UnityEngine;

namespace Player {
    public class PlayerInputController: MonoBehaviour {
        private InputAsset input;
        public Vector2 moveInput;
        public bool didPressLegAction;
        public bool didPressArmAction;
        public bool didPressHeadAction;
        public bool didPressInteract;
        public bool didPressInteractThisTurn;
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
            this.didPressInteractThisTurn = this.input.Player.Interact.WasPressedThisFrame();
        }
        private void FixedUpdate() {
            if (this.updateInput) {
                this.updateInput = false;
                this.moveInput = this.input.Player.Move.ReadValue<Vector2>();
                this.didPressLegAction = this.input.Player.Jump.ReadValue<float>() > 0.5f;
                this.didPressArmAction = this.input.Player.Attack.ReadValue<float>() > 0.5f;
                this.didPressHeadAction = this.input.Player.Crouch.ReadValue<float>() > 0.5f;
                this.didPressInteract = this.input.Player.Interact.ReadValue<float>() > 0.5f;
            }
        }
    }
}