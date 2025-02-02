using System;
using Creatures;
using Creatures.Collision;
using Creatures.Parts;
using RamenSea.Foundation3D.Components.Recyclers;
using UnityEngine;
using RamenSea.Foundation3D.Extensions;
using Systems;

namespace Player.PickUp {
    public class PartPickUpHolder: MonoBehaviour, IRecyclableObject, ICreatureCollisionDetectionListener {
        public BaseCreaturePart heldPart;
        public CreatureCollisionDetection playerDetection;
        public GameObject selectedGo;
        public bool isSelected = false;
        private void Start() {
            this.playerDetection.listener = this;
        }

        [SerializeField] private Rigidbody rb;
        public void AttachPart(BaseCreaturePart part, BaseLimb limb) {
            this.selectedGo.SetActive(false);
            this.heldPart = part;
            this.transform.SetPositionAndRotation(limb.transform);
            var pos = limb.transform.position;
            this.transform.position = pos;
            limb.transform.SetParent(this.transform);
            limb.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            limb.OnAttachToWorldContainer();
            this.rb.mass = part.traits.weight;
        }

        public void OnPickUp() {
            this.heldPart = null;
            this.recycler.Recycle(this);
            this.selectedGo.SetActive(false);
        }

        public IRecycler recycler { get; set; }

        public void OnCreatureTriggerEnter(BaseCreature creature) {
            WorldPartCollector.Instance.OnPartWithinRange(this);
            this.selectedGo.SetActive(true);
        }
        public void OnCreatureTriggerExit(BaseCreature creature) {
            WorldPartCollector.Instance.OnPartLeft(this);
            this.selectedGo.SetActive(false);
        }
    }
}