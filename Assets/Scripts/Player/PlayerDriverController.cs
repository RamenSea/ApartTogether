using System;
using Creatures;
using RamenSea.Foundation3D.Extensions;
using UnityEngine;

namespace Player {
    public class PlayerDriverController: MonoBehaviour {
        [SerializeField] private BaseCreature creature;
        [SerializeField] private PlayerInputController inputController;
        [SerializeField] private CameraController cameraController;

        private void Update() {
            var yInput = this.cameraController.transform.forward * this.inputController.moveInput.y;
            var xInput = this.cameraController.transform.right * this.inputController.moveInput.x;
            var inputGuide = yInput + xInput;
            inputGuide = inputGuide.normalized;
            // inputGuide = new Vector3(0,0,1f);
            this.creature.moveDirection = inputGuide;
        }
    }
}