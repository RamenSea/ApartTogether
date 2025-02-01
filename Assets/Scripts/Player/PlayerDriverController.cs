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
        [SerializeField] public Transform cameraAimAt;
        [SerializeField] public float aimHeightOffsetForHeight;
        [SerializeField] public Vector3 defaultCameraOffset;
        [SerializeField] public Vector2 cameraMoveEffect;
        [SerializeField] public float cameraMaxMove;

        private void Awake() {
            PlayerDriverController.Instance = this;
        }

        private void Start() {
            this.creature = this.playerSpawn.Spawn(true);
            creature.gameObject.name = "Player Creature";
            CameraController.Instance.virtualCamera.Follow = this.transform;
            CameraController.Instance.virtualCamera.LookAt = this.cameraAimAt;
            this.PlayerCreatureDidChangeParts();
        }

        public void PlayerCreatureDidChangeParts() {
            if (this.creature == null) {
                return;
            }
            var height = this.creature.compiledTraits.height * this.aimHeightOffsetForHeight;
            var offset = this.defaultCameraOffset;
            offset.y += height;
            var target = offset;
            target.y += this.inputController.cameraInput.y * this.cameraMoveEffect.y;
            target.x += this.inputController.cameraInput.x * this.cameraMoveEffect.x;
            offset = Vector3.MoveTowards(offset, target,this.cameraMaxMove);
            this.cameraAimAt.localPosition = offset;
            var pos = this.cameraAimAt.position;
            
            var y = PlayerDriverController.Instance.cameraController.transform.eulerAngles.y;
            var e = this.transform.eulerAngles;
            e.y = y;
            this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, Quaternion.Euler(e), this.creature.compiledTraits.rotationSpeedMin * Time.deltaTime);
            this.cameraAimAt.position = pos;
        }

        private void Update() {
            this.transform.position = this.creature.transform.position;
            var yInput = this.cameraController.transform.forward * this.inputController.moveInput.y;
            var xInput = this.cameraController.transform.right * this.inputController.moveInput.x;
            var inputGuide = yInput + xInput;
            inputGuide = inputGuide.normalized;
            // inputGuide = new Vector3(0,0,1f);
            this.creature.moveDirection = inputGuide;
            this.creature.moveInput = this.inputController.moveInput;
            this.creature.doLegAction = this.inputController.didPressLegAction;
            this.creature.doArmsAction = this.inputController.didPressArmAction;
            this.creature.doHeadAction = this.inputController.didPressHeadAction;
            
            this.PlayerCreatureDidChangeParts();
        }

        private void OnDestroy() {
            if (PlayerDriverController.Instance == this) {
                PlayerDriverController.Instance = null;
            }
        }
    }
}