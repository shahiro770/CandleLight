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
        /// Randomizes the amounts for all effects depending on the items' qualities
        /// </summary>
        /// <param name="quality"> String quality of the gear, will make this into a constant soon </param>
        /// <remark>
        /// This function is kept seperate from the consumable's version for future changes
        /// (such as handling custom gear modifiers, like "10% chance to poison enemies on attacks")
        /// </remark>
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
        /// Returns how much an item is worth
        /// </summary>
        /// <returns></returns>
        public override int GetWAXValue() {
            int WAX = 0;

            for (int i = 0; i < effects.Length; i++) {
                switch(effects[i]) {
                    case "STR":
                    case "DEX":   
                    case "INT":  
                    case "LUK":   
                    case "HP":
                    case "MP":
                    case "DOG":
                    case "ACC":
                    case "CRITCHANCE":
                        WAX += values[i];
                        break;
                    case "PATK":
                    case "MATK":
                        WAX += values[i] * 3;
                        break;
                    case "PDEF":
                    case "MDEF":
                        WAX += values[i] * 4;
                        break;
                    case "CRITMULT":
                        WAX += values[i] * 100;
                        break;
                    case "BLEEDPLUS":
                    case "MPREGENDOUBLE":
                    case "HPREGENDOUBLE":
                        WAX += 10;
                        break;
                    default:
                        break;
                }
            }

            return WAX;
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