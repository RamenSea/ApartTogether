using Player.PickUp;
using RamenSea.Foundation3D.Components.Recyclers;
using UnityEngine;

namespace Systems {
    public class WorldPartCollector: RecyclerBehavior<PartPickUpHolder> {
        public static WorldPartCollector Instance { get; private set; }
        protected override void Awake() {
            base.Awake();
            WorldPartCollector.Instance = this; // bad;
        }
        
        private void OnDestroy() {
            if (WorldPartCollector.Instance == this) {
                WorldPartCollector.Instance = null;
            }
        }
    }
}