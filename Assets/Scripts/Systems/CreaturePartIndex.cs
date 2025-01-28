using System;
using System.Collections.Generic;
using Creatures.Parts;
using NaughtyAttributes;
using RamenSea.Foundation3D.Components.Recyclers;
using UnityEngine;

namespace Systems {
    public class CreaturePartIndex: MonoBehaviour {

        public KeyedPrefabRecycler<PartId, BaseCreaturePart> recycler;

        [SerializeField] private BaseCreaturePart[] allParts;

        private void Awake() {
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
        
        
#if UNITY_EDITOR
        [Button("Update prefabs")]
        public void UpdatePrefabs() {
            this.allParts = Resources.LoadAll<BaseCreaturePart>("Prefabs/Parts");
        }
#endif
    }
}