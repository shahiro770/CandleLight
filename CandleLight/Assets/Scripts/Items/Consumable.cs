/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: August 5, 2019
* 
* The Consumable class is used to store information about an item that is used for healing,
* buffing, and sometimes debuffing.
*
*/

using UnityEngine;

namespace Items {

    public class Consumable : Item {

        /// <summary>
        /// Consumable item constructor
        /// </summary>
        public Consumable(string nameID, string type, string subType, string[] effects, int[] values) : base(nameID, type, subType) {
            for (int i = 0; i < effects.Length; i++) {
                 this.effects[i] = effects[i];
                if (effects[i] != "none") {
                    effectsNum++;
                }
            }
            this.values = values;
            itemSprite = Resources.Load<Sprite>("Sprites/Items/Consumables/" + nameID);
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="spr"> Sprite to be displayed </param>
        public Consumable(Consumable c) : base(c.nameID, c.type, c.subType) {
            for (int i = 0; i < effects.Length; i++) {
                this.effects[i] = c.effects[i];
                if (effects[i] != "none") {
                    effectsNum++;
                }
            }
            for (int i = 0; i < values.Length; i++) {
                this.values[i] = c.values[i];
            }
            itemSprite = c.itemSprite;
        }

        /// <summary>
        /// Empty constructor
        /// </summary>
        public Consumable() : base() {}
        
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
            else if (quality == "high") {
                multiplier = 1.5f;
            }
            else {      // perfect quality, can only be achieved under rare circumstances
                multiplier = 2f;
            }

            for (int i = 0; i < effects.Length; i++) {
                if (i == effectIndex && effectsNum > 1 && quality == "med" ) {         // medium quality will have at most 2 random effects
                    effects[i] = "none";
                }   
                else if (i != effectIndex && quality == "low") {    // low quality will have 1 random effect
                    effects[i] = "none";
                }

                values[i] =  Random.Range((int)(Mathf.Max(1, values[i] * multiplier)), (int)(values[i] * (1 + multiplier)));
            }
        }

        /// <summary>
        /// Returns how much an item is worth
        /// </summary>
        /// <returns></returns>
        public override int GetWAXValue() {
            int WAX = 0;

            for (int i = 0; i < effects.Length; i++) {
                switch(effects[i]) {
                    case "HP":
                    case "MP":
                        WAX += (int)(values[i] * 0.5f);
                        break;
                    case "regenerate":
                    case "cure":
                        WAX += values[i] * 2;
                        break;
                    case "poison":
                        WAX -= values[i] * 5;
                        break;
                    default:
                        break;
                }
            }

            return WAX;
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
                if (effects[i] == "%MAXHP") {           // TODO Make this account for percent effects
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