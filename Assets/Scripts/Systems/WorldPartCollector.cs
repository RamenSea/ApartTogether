using System;
using System.Collections.Generic;
using Player;
using Player.PickUp;
using RamenSea.Foundation3D.Components.Recyclers;
using UnityEngine;

namespace Systems {
    public class WorldPartCollector: RecyclerBehavior<PartPickUpHolder> {
        public static WorldPartCollector Instance { get; private set; }


        public List<PartPickUpHolder> partsWithinRange;
        protected override void Awake() {
            base.Awake();
            WorldPartCollector.Instance = this; // bad;
            this.partsWithinRange = new();
        }

        private void Update() {
            if (PlayerDriverController.Instance.inputController.didPressInteractThisTurn &&
                this.partsWithinRange.Count > 0) {
                var pickUpHolder = this.partsWithinRange[0];
                if (pickUpHolder.heldPart != null) {
                    var part = pickUpHolder.heldPart;
                    if (this.partsWithinRange.Contains(pickUpHolder)) {
                        this.partsWithinRange.Remove(pickUpHolder);
                    }
                    pickUpHolder.OnPickUp();
                    part.OnDeattachToWorldContainer();
                    PlayerDriverController.Instance.creature.SetCreaturePart(part);
                    PlayerDriverController.Instance.creature.FinishSettingParts(false);
                }
            }
        }

        public void OnPartWithinRange(PartPickUpHolder holder) {
            if (!this.partsWithinRange.Contains(holder)) {
                this.partsWithinRange.Add(holder);
            }
        }
        public void OnPartLeft(PartPickUpHolder holder) {
            if (this.partsWithinRange.Contains(holder)) {
                this.partsWithinRange.Remove(holder);
            }
        }
        private void OnDestroy() {
            if (WorldPartCollector.Instance == this) {
                WorldPartCollector.Instance = null;
            }
        }
    }
}