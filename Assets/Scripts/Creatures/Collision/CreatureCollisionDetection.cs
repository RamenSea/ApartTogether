using System;
using UnityEngine;

namespace Creatures.Collision {

    public interface ICreatureCollisionDetectionListener {
        public void OnCreatureCollisionEnter(BaseCreature creature, UnityEngine.Collision collision) { }
        public void OnOtherCollisionEnter(UnityEngine.Collision collision) { }
        public void OnCreatureCollisionExit(BaseCreature creature, UnityEngine.Collision collision) { }
        public void OnCreatureTriggerEnter(BaseCreature creature) { }
        public void OnCreatureTriggerExit(BaseCreature creature) { }
        public void OnOtherTriggerEnter(Collider other) { }
    }
    public class CreatureCollisionDetection: MonoBehaviour {
        public ICreatureCollisionDetectionListener listener;
        
        public virtual void OnCollisionEnter(UnityEngine.Collision other) {
            if (other.gameObject.CompareTag(GameTags.Creature)) {
                var creatureCollider = other.gameObject.GetComponent<CreatureCollider>();
                this.listener?.OnCreatureCollisionEnter(creatureCollider.creature, other);
            } else {
                this.listener?.OnOtherCollisionEnter(other);
            }
        }
        public virtual void OnCollisionExit(UnityEngine.Collision other) {
            if (other.gameObject.CompareTag(GameTags.Creature)) {
                var creatureCollider = other.gameObject.GetComponent<CreatureCollider>();
                this.listener?.OnCreatureCollisionExit(creatureCollider.creature, other);
            }
            
        }
        public virtual void OnTriggerEnter(Collider other) {
            if (other.gameObject.CompareTag(GameTags.Creature)) {
                var creatureCollider = other.gameObject.GetComponent<CreatureCollider>();
                this.listener?.OnCreatureTriggerEnter(creatureCollider.creature);
            } else {
                this.listener?.OnOtherTriggerEnter(other);
            }
            
        }
        public virtual void OnTriggerExit(Collider other) {
            if (other.gameObject.CompareTag(GameTags.Creature)) {
                var creatureCollider = other.gameObject.GetComponent<CreatureCollider>();
                this.listener?.OnCreatureTriggerExit(creatureCollider.creature);
            }
        }
    }
}