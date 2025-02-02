using System;
using Cinemachine;
using UnityEngine;

namespace Player {
    public class CameraController: MonoBehaviour {
        public static CameraController Instance { get; private set; }
        public CinemachineVirtualCamera virtualCamera;
        public Camera mainCamera;

        private void Awake() {
            Instance = this;
        }

        private void OnDestroy() {
            if (Instance == this) Instance = null;
        }
    }
}