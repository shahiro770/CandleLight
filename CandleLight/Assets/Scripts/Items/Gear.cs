/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: August 5, 2019
* 
* The Gear class is used to store information about an item that is equipped by a partyMember
* to increase their stats.
*
*/

using UnityEngine;

namespace Items {

    public class Gear : Item {

        /// <summary>
        /// Gear item constructor
        /// </summary>
        public Gear(string nameID, string type, string subType, string className, string[] effects, int[] values) : base(nameID, type, subType) {
            this.className = className;
            this.effects = effects;
            for (int i = 0; i < effects.Length; i++) {
                if (effects[i] != "none") {
                    effectsNum++;
                }
            }
            this.values = values;
            itemSprite = Resources.Load<Sprite>("Sprites/Items/Gear/" + nameID);
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        public Gear(Gear g) : base(g.nameID, g.type, g.subType) {
            this.className = g.className;
            for (int i = 0; i < effects.Length; i++) {
                this.effects[i] = g.effects[i];
                if (effects[i] != "none") {
                    effectsNum++;
                }
            }
            for (int i = 0; i < values.Length; i++) {
                this.values[i] = g.values[i];
            }
            itemSprite = g.itemSprite;
        }

        /// <summary>
        /// Empty constructor
        /// </summary>
        public Gear() : base() {}

        /// <summary>
        /// Randomizes the amounts for all effects depending on the items' qualities
        /// </summary>
        /// <param name="quality"></param>
        public void RandomizeAmounts(string quality) {
            float multiplier;
            int effectIndex = Random.Range(0, effectsNum);

            if (quality == "low") {
                multiplier = 0.5f;
            }
            else if (quality == "med") {
                multiplier = 1f;
            }
            else {
                multiplier = 1.5f;
            }
            
            for (int i = 0; i < effects.Length; i++) {
                if (i == effectIndex && effectsNum > 1 && quality == "med" ) {         // medium quality will have at most 2 random effects
                    effects[i] = "none";
                }   
                else if (i != effectIndex && quality == "low") {    // low quality will have 1 random effect
                    effects[i] = "none";
                }

                values[i] =  Random.Range((int)(values[i] * multiplier), (int)(values[i] * (1 + multiplier)));
            }
        }
 
        /// <summary>
        /// Returns effects
        /// </summary>
        /// <returns></returns>
        public override string[] GetEffects() {
            return effects;
        }

        /// <summary>
        /// Returns amounts
        /// </summary>
        /// <returns></returns>
        public override int[] GetAmounts() {
            return values;
        }

        /// <summary>
        /// Returns the amounts as strings
        /// </summary>
        /// <returns></returns>
        public override string[] GetAmountsAsStrings() {
            string[] amountStrings = new string[effects.Length];
            for (int i = 0; i < effects.Length; i++) {
                if (effects[i] == "%MAXHP" || effects[i] == "CRITCHANCE") {     // TODO Make this account for percent effects
                    amountStrings[i] = values[i] + "%";
                }
                else {
                    amountStrings[i] = values[i].ToString();
                }
            }

            return amountStrings;
        }


        /// <summary>
        /// Returns the effect keys in a string array
        /// </summary>
        /// <returns></returns>
        public override string[] GetTooltipEffectKeys() {
            string[] effectKeys = new string[effects.Length];
            
            for (int i = 0; i < effects.Length; i++) {
                effectKeys[i] = effects[i] + "_label";
            }

            return effectKeys;
        }
    }
}