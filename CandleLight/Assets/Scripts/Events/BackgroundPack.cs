
/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: July 17, 2019
* 
* The BackgroundPack class holds sprites for backgrounds in a particular event.
* Even events in the same subArea may have different backgroundPacks.
*
*/


using UnityEngine;

namespace Events {

    public class BackgroundPack {

        private Sprite[] backgrounds = new Sprite[10];  /// <value> 10 sprites max </value>

        public string name { get; private set; }        /// <value> Name of pack </value>
        public int backgroundNum = 0;                   /// <value> Number of sprites in pack </value>
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"> Name of pack </param>
        /// <param name="areaName"> Area where backgroundPack will be used in </param>
        /// <param name="spriteIndices">
        /// Indice array of length 10 max for each sprite to be loaded
        /// from resources 
        /// </param>
        public BackgroundPack(string name, string areaName, int[] spriteIndices) {
            this.name = name;

            for (int i = 0; i < spriteIndices.Length; i++) {
                if (spriteIndices[i] != -1) {
                    this.backgrounds[i] = Resources.Load<Sprite>("Sprites/Backgrounds/" + areaName + "/" + areaName + spriteIndices[i]);
                    backgroundNum++;
                }
            }
        }

        /// <summary>
        /// Get a random background sprite
        /// </summary>
        /// <returns> Random background  sprite </returns>
        public Sprite GetBackground() {
            return backgrounds[Random.Range(0, backgroundNum)];
        }
        
        /// <summary>
        /// Get a background sprite by specific index
        /// </summary>
        /// <param name="index"> Integer index (must be less than 10) </param>
        /// <returns> Background sprite </returns>
        public Sprite GetBackground(int index) {
            return backgrounds[index];
        }
    }
}