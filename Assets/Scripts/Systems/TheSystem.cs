using System;
using RamenSea.Foundation3D.Services.KeyStore;

namespace Systems {
    [Serializable]
    public class GameSave {
        public bool hasCollectedPepeHead;
        public bool hasCollectedPepeBody;
        public bool hasCollectedPepeWings;
        public bool hasCollectedPepeLegs;
        public bool hasUnDammedRiver;
        public bool hasSeenIntro;
        public bool hasSeenLocationHint1;
    }
    public class TheSystem {
        private static TheSystem instance;


        public KeyStoreService keyStore;
        public GameSave save;
        
        
        public TheSystem() {
            this.keyStore = new KeyStoreService();
            this.keyStore.jsonSerializer = new UnityJsonSerializer();
            
            this.save = this.keyStore.GetDeserializedObject<GameSave>("game_save");
            if (this.save == null) {
                this.save = new GameSave();
            }
        }

        public void UpdateSave() {
            this.keyStore.Set("game_save", this.save);
        }
        public void ClearSave() {
            this.keyStore.Remove("game_save");
            this.save = new GameSave();
        }
        public static TheSystem Get() {
            if (instance == null) {
                instance = new TheSystem();
            }
            return instance;
        }
    }
}