using UnityEngine;

namespace Creatures {
    public enum PartSlotType {
        Body,
        Legs,
        Head,
        Arms,
    }
    public class BaseCreaturePart: MonoBehaviour {
        [SerializeField] protected PartSlotType _slotType;
        public PartSlotType slotType => _slotType;





        public void PhysicsUpdate(float deltaTime) {
            this.OnPhysicsUpdate(deltaTime);
        }
        protected virtual void OnPhysicsUpdate(float deltaTime) { }
    }
}