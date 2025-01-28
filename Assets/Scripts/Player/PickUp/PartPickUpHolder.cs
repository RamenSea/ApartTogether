using Creatures.Parts;
using RamenSea.Foundation3D.Components.Recyclers;
using UnityEngine;
using RamenSea.Foundation3D.Extensions;

namespace Player.PickUp {
    public class PartPickUpHolder: MonoBehaviour, IRecyclableObject {
        public BaseCreaturePart heldPart;

        public void AttachPart(BaseCreaturePart part) {
            this.heldPart = part;
            this.transform.SetPositionAndRotation(part.transform);
            part.transform.SetParent(this.transform);
            part.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }

        public IRecycler recycler { get; set; }
    }
}