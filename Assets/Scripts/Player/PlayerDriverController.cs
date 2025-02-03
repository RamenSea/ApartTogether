using System;
using Cinemachine;
using Creatures;
using Creatures.Collision;
using Creatures.Parts;
using RamenSea.Foundation3D.Extensions;
using Systems;
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
        
        [SerializeField] public Vector3 trackingBirdDamping = new Vector3(4f, 4f, 4f);
        [SerializeField] public Vector3 trackingBirdRigShoulderOffset = new Vector3(0.79f, 3.86f, 0f);
        [SerializeField] public float trackingBirdVerticalAimArm = 0.17f;
        [SerializeField] public float trackingBirdCameraDistance = 2.6f;
        [SerializeField] public Vector3 trackingBirdAimOffset = new Vector3(-1.48f, 0f, 0f);
        
        [SerializeField] public Vector3 trackingPlayerDamping = new Vector3(0f,0f,0f);
        [SerializeField] public Vector3 trackingPlayerRigShoulderOffset = new Vector3(0f,0f,0f);
        [SerializeField] public float trackingPlayerVerticalAimArm = 0.17f;
        [SerializeField] public float trackingPlayerCameraDistance = 0f;
        [SerializeField] public Vector3 trackingPlayerAimOffset = new Vector3(0f,0f,0f);

        private void Awake() {
            PlayerDriverController.Instance = this;
        }

        public void SpawnInTest() {
            this.creature = this.playerSpawn.Spawn(true);
            creature.gameObject.name = "Player Creature";
            this.PlayerCreatureDidChangeParts();
            this.SetCameraAfterSpawn();
        }

        public void SpawnIn(SpawnPointActivation at) {
            var save = TheSystem.Get().save;
            if (save.hasCollectedPepeBody)
                this.playerSpawn.bodyPart = PartId.PepeBody;
            if (save.hasCollectedPepeHead)
                this.playerSpawn.headPart = PartId.PepeHead;
            if (save.hasCollectedPepeLegs)
                this.playerSpawn.legPart = PartId.PepeLegs;
            if (save.hasCollectedPepeWings)
                this.playerSpawn.armPart = PartId.PepeWings;
            
            this.transform.SetPositionAndRotation(at.spawnPoint);
            at.spawnAnimation.gameObject.SetActive(true);
            at.spawnAnimation.Stop();
            at.spawnAnimation.Play();
            this.creature = this.playerSpawn.Spawn(true);
            creature.gameObject.name = "Player Creature";
            this.PlayerCreatureDidChangeParts();
            this.SetCameraAfterSpawn();
        }

        public void SetCameraForBird() {
            this.cameraController.virtualCamera.LookAt = GameRunner.Instance.annoyingBird.transform;
            this.cameraController.virtualCamera.Follow = GameRunner.Instance.annoyingBird.transform;
            var cameraBody = this.cameraController.virtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
            cameraBody.Damping = this.trackingBirdDamping;
            cameraBody.ShoulderOffset = this.trackingBirdRigShoulderOffset;
            cameraBody.VerticalArmLength = this.trackingBirdVerticalAimArm;
            cameraBody.CameraDistance = this.trackingBirdCameraDistance;

            var cameraAim = this.cameraController.virtualCamera.GetCinemachineComponent<CinemachineComposer>();
            cameraAim.m_TrackedObjectOffset = this.trackingBirdAimOffset;
        }
        public void SetCameraForHints() {
            var cameraBody = this.cameraController.virtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
            cameraBody.Damping = this.trackingPlayerDamping;
            cameraBody.ShoulderOffset = this.trackingPlayerRigShoulderOffset;
            cameraBody.VerticalArmLength = this.trackingPlayerVerticalAimArm;
            cameraBody.CameraDistance = this.trackingPlayerCameraDistance;
            cameraBody.CameraCollisionFilter = 0;

            var cameraAim = this.cameraController.virtualCamera.GetCinemachineComponent<CinemachineComposer>();
            cameraAim.m_TrackedObjectOffset = this.trackingPlayerAimOffset;
        }
        public void SetCameraAfterSpawn() {
            CameraController.Instance.virtualCamera.Follow = this.transform;
            CameraController.Instance.virtualCamera.LookAt = this.cameraAimAt;
            var cameraBody = this.cameraController.virtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
            cameraBody.Damping = this.trackingPlayerDamping;
            cameraBody.ShoulderOffset = this.trackingPlayerRigShoulderOffset;
            cameraBody.VerticalArmLength = this.trackingPlayerVerticalAimArm;
            cameraBody.CameraDistance = this.trackingPlayerCameraDistance;
            cameraBody.CameraCollisionFilter = 1 << CreatureManager.Instance.groundMask;

            var cameraAim = this.cameraController.virtualCamera.GetCinemachineComponent<CinemachineComposer>();
            cameraAim.m_TrackedObjectOffset = this.trackingPlayerAimOffset;
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
            if (this.creature == null) {
                return;
            }
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