using System;
using System.Collections.Generic;
using Creatures;
using Creatures.Parts;
using NaughtyAttributes;
using RamenSea.Foundation3D.Components.Recyclers;
using UnityEngine;

namespace Systems {
    public class CreaturePartIndex: MonoBehaviour {

        public static CreaturePartIndex Instance;
        
        public KeyedPrefabRecycler<PartId, BaseCreaturePart> recycler;

        [SerializeField] private BaseCreaturePart[] allParts;
        [SerializeField] public BaseCreature basePrefab;

        private void Awake() {
            CreaturePartIndex.Instance = this;
            var indexedParts = new Dictionary<PartId, BaseCreaturePart>();
            if (this.allParts != null) {
                for (var i = 0; i < this.allParts.Length; i++) {
                    var part = this.allParts[i];
#if UNITY_EDITOR
                    if (indexedParts.ContainsKey(part.partId)) {
                        Debug.LogError($"Recycler contains duplicate parts ${part.partId}");
                    }
#endif
                    indexedParts[part.partId] = part;
                }
            }
            this.recycler = new KeyedPrefabRecycler<PartId, BaseCreaturePart>(indexedParts, this.transform);
        }

        private void OnDestroy() {
            if (CreaturePartIndex.Instance == this) {
                CreaturePartIndex.Instance = null;
            }
        }

#if UNITY_EDITOR
        [Button("Update prefabs")]
        public void UpdatePrefabs() {
            this.allParts = Resources.LoadAll<BaseCreaturePart>("Prefabs/Parts");
        }
#endif
    }
}