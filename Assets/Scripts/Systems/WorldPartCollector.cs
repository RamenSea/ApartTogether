using System;
using System.Collections.Generic;
using Creatures.Parts;
using Player;
using Player.PickUp;
using RamenSea.Foundation3D.Components.Recyclers;
using RamenSea.Foundation3D.Extensions;
using UnityEngine;

namespace Systems {
    public class WorldPartCollector: RecyclerBehavior<PartPickUpHolder> {
        public static WorldPartCollector Instance { get; private set; }
        public List<PartPickUpHolder> partsWithinRange;
        public List<PartPickUpHolder> parts;
        public PartPickUpHolder highlightedPart;
        public float minDistanceForInteraction;
        
        protected override void Awake() {
            base.Awake();
            WorldPartCollector.Instance = this; // bad;
            this.partsWithinRange = new();
            this.parts = new();
        }

        private void Update() {
            if (PlayerDriverController.Instance.inputController.didPressInteractThisTurn &&
                this.highlightedPart != null) {
                if (this.highlightedPart.heldPart != null) {
                    var part = this.highlightedPart.heldPart;

                    var i = 0;
                    while (i < this.parts.Count) {
                        var holder = this.parts[i];
                        if (holder.heldPart == part) {
                            this.parts.RemoveAt(i);
                            holder.selectedGo.SetActive(false);
                            this.Recycle(holder);
                            continue;
                        }

                        i++;
                    }
                    this.highlightedPart.OnPickUp();
                    part.OnDeattachToWorldContainer();
                    PlayerDriverController.Instance.creature.SetCreaturePart(part);
                    PlayerDriverController.Instance.creature.FinishSettingParts(false);
                }
            }
            
            this.CheckHighlight();
        }

        public void DropPart(BaseCreaturePart part) {
            part.OnDeattachToBody(true);
            for (var i = 0; i < part.limbs.Length; i++) {
                var limb = part.limbs[i];
                var collector = this.Get();
                this.parts.Add(collector);
                collector.AttachPart(part, limb);
            }
        }
        public void CheckHighlight() {
            if (this.highlightedPart != null) {
                this.highlightedPart.selectedGo.SetActive(false);
                this.highlightedPart = null;
            }

            var currentDistance = 10000000f;
            for (var i = 0; i < this.partsWithinRange.Count; i++)
            {
                var part = this.partsWithinRange[i];
                var distance = part.transform.position.Distance(PlayerDriverController.Instance.creature.transform.position);
                if (distance > this.minDistanceForInteraction) {
                    continue;
                }

                if (this.highlightedPart == null || distance < currentDistance) {
                    currentDistance = distance;
                    this.highlightedPart = part;
                }
            }
            
            if (this.highlightedPart != null) {
                this.highlightedPart.selectedGo.SetActive(true);
            }
        }
        public void OnPartWithinRange(PartPickUpHolder holder) {
            if (!this.partsWithinRange.Contains(holder)) {
                this.partsWithinRange.Add(holder);
            }
            this.CheckHighlight();
        }
        public void OnPartLeft(PartPickUpHolder holder) {
            if (this.partsWithinRange.Contains(holder)) {
                holder.selectedGo.SetActive(false);
                this.partsWithinRange.Remove(holder);
            }
            this.CheckHighlight();
        }
        private void OnDestroy() {
            if (WorldPartCollector.Instance == this) {
                WorldPartCollector.Instance = null;
            }
        }
    }
}