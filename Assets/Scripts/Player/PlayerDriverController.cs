using System;
using Creatures;
using RamenSea.Foundation3D.Extensions;
using UnityEngine;

namespace Player {
    public class PlayerDriverController: MonoBehaviour {
        public static PlayerDriverController Instance { get; private set; }
        
        public BaseCreature creature;
        [SerializeField] public PlayerInputController inputController;
        [SerializeField] public CameraController cameraController;
        [SerializeField] public SpawnInCreature playerSpawn;

        private void Awake() {
            PlayerDriverController.Instance = this;
        }

        private void Start() {
            this.creature = this.playerSpawn.Spawn();
            creature.gameObject.name = "Player Creature";
            CameraController.Instance.virtualCamera.Follow = this.creature.bodyPart?.followPoint;
        }

        private void Update() {
            this.transform.SetPositionAndRotation(this.creature.transform);
            var yInput = this.cameraController.transform.forward * this.inputController.moveInput.y;
            var xInput = this.cameraController.transform.right * this.inputController.moveInput.x;
            var inputGuide = yInput + xInput;
            inputGuide = inputGuide.normalized;
            // inputGuide = new Vector3(0,0,1f);
            this.creature.moveDirection = inputGuide;
            this.creature.doLegAction = this.inputController.didPressLegAction;
            this.creature.doArmsAction = this.inputController.didPressArmAction;
            this.creature.doHeadAction = this.inputController.didPressHeadAction;
        }

        private void OnDestroy() {
            if (PlayerDriverController.Instance == this) {
                PlayerDriverController.Instance = null;
            }
        }
    }
}