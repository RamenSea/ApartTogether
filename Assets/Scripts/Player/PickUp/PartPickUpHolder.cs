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
        public void AttachPart(BaseCreaturePart part) {
            this.selectedGo.SetActive(false);
            this.heldPart = part;
            this.transform.SetPositionAndRotation(part.transform);
            var pos = part.transform.position;
            pos.y += 0.5f;
            this.transform.position = pos;
            part.transform.SetParent(this.transform);
            part.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            part.OnAttachToWorldContainer(this);
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