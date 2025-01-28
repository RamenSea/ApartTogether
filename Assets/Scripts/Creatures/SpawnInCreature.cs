using Creatures.Parts;
using RamenSea.Foundation3D.Extensions;
using Systems;
using UnityEngine;

namespace Creatures {
    public class SpawnInCreature : MonoBehaviour {
        public PartId bodyPart = PartId.None;
        public PartId legPart = PartId.None;
        public PartId armPart = PartId.None;
        public PartId headPart = PartId.None;

        public bool snapToGround = true;
        public bool autoSpawnOnStart = true;

        private void Start() {
            if (this.autoSpawnOnStart) {
                this.Spawn();
            }
        }

        public BaseCreature Spawn() {
            if (this.bodyPart == PartId.None) {
                Debug.LogWarning("Body part not specified");
                return null;
            }

            var partSystem = CreaturePartIndex.Instance;
            
            var creatureController = partSystem.basePrefab.Instantiate();

            var body = partSystem.recycler.Get(this.bodyPart);
            creatureController.SetCreaturePart(body);
            if (this.headPart != PartId.None) {
                var part = partSystem.recycler.Get(this.headPart);
                creatureController.SetCreaturePart(part);
            }

            if (this.legPart != PartId.None) {
                var part = partSystem.recycler.Get(this.legPart);
                creatureController.SetCreaturePart(part);
            }

            if (this.armPart != PartId.None) {
                var part = partSystem.recycler.Get(this.armPart);
                creatureController.SetCreaturePart(part);
            }

            creatureController.FinishSettingParts(true);
            if (this.snapToGround) {
                if (Physics.SphereCast(this.transform.position, 0.2f, Vector3.down, out RaycastHit hit, 50, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore)) {
                    var ground = hit.point;
                    ground.y += creatureController.compiledTraits.height;
                    creatureController.transform.SetPositionAndRotation(ground, this.transform.rotation);
                } else {
                    creatureController.transform.SetPositionAndRotation(this.transform);
                }
            } else {
                creatureController.transform.SetPositionAndRotation(this.transform);
            }

            return creatureController;
        }
    }
}