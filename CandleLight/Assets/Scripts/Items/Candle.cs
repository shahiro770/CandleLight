/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: June 11, 2020
* 
* The Candle class is used to store information about an item that is used for
* passive buffs, debuffs, and a limited use attack.
*
*/

using Attack = Combat.Attack;
using UnityEngine;

namespace Items {

    public class Candle : Item {

        public Sprite activeSprite; /// <value> Candles are lit up when active </value>
        public Sprite usedSprite;   /// <value> </value>

        public Attack a;
        public bool isUsable;
        private int maxUses;
        public int uses;

        /// <summary>
        /// Candle item constructor
        /// </summary>
        /// <param name="nameID"> Name of the candle </param>
        /// <param name="type"> Type will always be "candle" for candles </param>
        /// <param name="subType"> subTypes don't matter for candles </param>
        /// <param name="className"> classes that can use this candle (normally any) </param>
        /// <param name="a"> Attack that can be used by equipping candle </param>
        /// <param name="effects"> Effects array </param>
        /// <param name="values"> Values array </param>
        /// <returns> Candle constructor </returns>
        /// <remarks>
        /// Candles will always have 3 effects, with little to no randomization.
        /// 1st effect is a positive effect, 2nd effect is empty, but the 2nd value represents
        /// the number of uses the candle gets, and final effect is the negative effect assocaited with keeping the candle active.
        /// </remarks>
        public Candle(string nameID, string type, string subType, string className, Attack a, string[] effects, int[] values) : base(nameID, type, subType) {
            this.className = className;
            this.effects = effects;
            this.values = values;
            this.a = a;
            itemSprite = Resources.Load<Sprite>("Sprites/Items/Candles/" + nameID);
            activeSprite = Resources.Load<Sprite>("Sprites/Items/Candles/" + nameID + "Active");
            usedSprite = Resources.Load<Sprite>("Sprites/Items/Candles/" + nameID + "Used");

            isUsable = true;
            uses = values[1];
            maxUses = uses;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="spr"> Sprite to be displayed </param>
        public Candle(Candle c) : base(c.nameID, c.type, c.subType) {
            className = c.className;
            effects = c.effects;
            values = c.values;
            this.a = c.a;

            itemSprite = c.itemSprite; 
            activeSprite = c.activeSprite; 
            usedSprite = c.usedSprite;
            
            isUsable = c.isUsable;
            uses = values[1];
            maxUses = uses;
        }

        /// <summary>
        /// Empty constructor
        /// </summary>
        public Candle() : base() {}
        
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
                    case "MPREGENDOUBLE":
                        WAX += 10;
                        break;
                    case "MP50%":
                        WAX += 20;
                        break;
                    default:
                        break;
                }
            }

            return WAX;
        }

        public void Rekindle() {
            uses = maxUses;
            SetUsable(true);
        }

        public void Use() {
            uses--;
            if (uses == 0) {
                SetUsable(false);
            }
        }

        /// <summary>
        /// Sets the candle's usability (if its attack and passive are online)
        /// </summary>
        /// <param name="value"> true if usable, false otherwise </param>
        public void SetUsable(bool value) {
            if (isUsable != value) {
                if (value == true) {
                    SetSprite(itemSprite);
                }
                else if (value == false) {
                    SetSprite(usedSprite);
                }
                id.SetUsable(value);
                isUsable = value;
            }           
        }

        /// <summary>
        /// Sets the sprite to show if its active and usable (activeSprite),
        /// not active but usable (itemSprite), or active/not active and not usable(usedSprite)
        /// </summary>
        /// <param name="value"></param>
        public void SetActive(bool value) {
            if (value == true && isUsable == true) {
                SetSprite(activeSprite);
            }
            else {
                if (isUsable == true) {
                    SetSprite(itemSprite);
                }
                else {
                    SetSprite(usedSprite);
                }
            }
        }

        /// <summary>
        /// Updates the candle's itemDisplays' sprite to the desired sprite
        /// </summary>
        /// <param name="s"></param>
        public override void SetSprite(Sprite s) {
            id.SetSprite(s);
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