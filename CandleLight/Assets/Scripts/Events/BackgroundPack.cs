
using UnityEngine;

namespace Events {

    public class BackgroundPack {

        private Sprite[] backgrounds = new Sprite[10];

        public string packName { get; private set; } 
        public int backgroundNum = 0; /// <value> Number of sprites in pack </value>
        
        public BackgroundPack(string packName, string areaName, int[] spriteIndices) {
            this.packName = packName;

            for (int i = 0; i < spriteIndices.Length; i++) {
                if (spriteIndices[i] != -1) {
                    this.backgrounds[i] = Resources.Load<Sprite>("Sprites/Backgrounds/" + areaName + "/" + areaName + spriteIndices[i]);
                    backgroundNum++;
                }
            }
        }

        public Sprite GetBackground() {
            return backgrounds[Random.Range(0, backgroundNum)];
        }
        
        public Sprite GetBackground(int index) {
            return backgrounds[index];
        }
    }
}