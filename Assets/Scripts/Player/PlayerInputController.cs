using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player {
    public class PlayerInputController: MonoBehaviour {
        public PlayerInput playerInput;
        public Vector2 moveInput;
        public Vector2 cameraInput;
        public bool didPressLegAction;
        public bool didPressArmAction;
        public bool didPressHeadAction;
        public bool didPressInteract;
        public bool didPressInteractThisTurn;
        private bool updateInput = true;
        
        
        private InputAction moveInputAction;
        private InputAction cameraInputAction;
        private InputAction legActionInputAction;
        private InputAction armActionInputAction;
        private InputAction headActionInputAction;
        private InputAction interactActionInputAction;

        private void Start() {
            this.moveInputAction = this.playerInput.actions["Move"];
            this.cameraInputAction = this.playerInput.actions["Look"];
            this.legActionInputAction = this.playerInput.actions["Jump"];
            this.armActionInputAction = this.playerInput.actions["Attack"];
            this.headActionInputAction = this.playerInput.actions["Crouch"];
            this.interactActionInputAction = this.playerInput.actions["Interact"];
        }

        private void Update() {
            this.updateInput = true;
            this.didPressInteractThisTurn = this.interactActionInputAction.WasPressedThisFrame();
        }
        private void FixedUpdate() {
            if (this.updateInput) {
                this.updateInput = false;
                this.moveInput = this.moveInputAction.ReadValue<Vector2>();
                this.cameraInput = this.cameraInputAction.ReadValue<Vector2>();
                
                this.didPressLegAction = this.legActionInputAction.ReadValue<float>() > 0.5f;
                this.didPressArmAction = this.armActionInputAction.ReadValue<float>() > 0.5f;
                this.didPressHeadAction = this.headActionInputAction.ReadValue<float>() > 0.5f;
                this.didPressInteract = this.interactActionInputAction.ReadValue<float>() > 0.5f;
            }
        }
    }
}