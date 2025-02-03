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
        public BaseLimb limb;
        public CreatureCollisionDetection playerDetection;
        public GameObject selectedGo;
        [SerializeField] private Rigidbody rb;

        private void Start() {
            this.playerDetection.listener = this;
        }

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
            if (this.limb != null) {
                this.limb.transform.SetParent(null);
            }
            this.limb = null;
            this.heldPart = null;
            this.recycler.Recycle(this);
            this.selectedGo.SetActive(false);
        }

        public IRecycler recycler { get; set; }

        public void OnCreatureTriggerEnter(BaseCreature creature) {
            WorldPartCollector.Instance.OnPartWithinRange(this);
        }
        public void OnCreatureTriggerExit(BaseCreature creature) {
            WorldPartCollector.Instance.OnPartLeft(this);
        }
    }
}