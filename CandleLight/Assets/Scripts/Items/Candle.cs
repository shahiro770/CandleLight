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
        public int uses = 0;
        public int maxUses = 0;

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
            a.nameKey = nameID + "_use_title";
            itemSprite = Resources.Load<Sprite>("Sprites/Items/Candles/" + nameID);
            activeSprite = Resources.Load<Sprite>("Sprites/Items/Candles/" + nameID + "Active");
            usedSprite = Resources.Load<Sprite>("Sprites/Items/Candles/" + nameID + "Used");

            isUsable = true;
            uses = values[1];
            maxUses = uses;
            CalculateWAXValue();
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
            CalculateWAXValue();
        }

        /// <summary>
        /// Data constructor
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public Candle(ItemData i) : base (i.nameID, i.type, i.subType) {
            className = i.className;
            effects = i.effects;
            values = i.values;
            a = i.a;

            itemSprite = Resources.Load<Sprite>("Sprites/Items/Candles/" + nameID);
            activeSprite = Resources.Load<Sprite>("Sprites/Items/Candles/" + nameID + "Active");
            usedSprite = Resources.Load<Sprite>("Sprites/Items/Candles/" + nameID + "Used");

            uses = i.uses;
            maxUses = i.maxUses;
            if (i.uses > 0) {
                isUsable = true;
            }
            else {
                isUsable = false;
            }
        }

        /// <summary>
        /// Returns how much an item is worth
        /// </summary>
        /// <returns></returns>
        public override void CalculateWAXValue() {
            int WAX = 0;
    
            switch(effects[0]) {
                case "MPREGENDOUBLE":
                case "HPREGENDOUBLE":
                    WAX += 22;
                    break;
                case "PATK":
                    WAX += values[0] * 8;
                    break;
                case "PDEF":
                case "MDEF":
                    WAX += values[0] * 7;
                    break;
                case "CRITCHANCE":
                    WAX += values[0] * 2;
                    break;
                case "HP":
                    WAX += values[0] * 2;
                    break;
                default:
                    break;
            }

            WAX *= maxUses;

            switch(effects[2]) {       
                case "CHAMPIONCHANCE":
                    WAX -= values[2];
                    break;
                default:
                    break;
            }

            WAXvalue = WAX;
        }

        /// <summary>
        /// Restore a candle's uses
        /// </summary>
        public void Rekindle() {
            uses = maxUses;
            SetUsable(true);
        }

        /// <summary>
        /// Restore a candle's uses to double its maximum (due to candlemancy)
        /// </summary>
        public void CandlemancyRekindle() {
            uses = maxUses * 2;
            SetUsable(true);
        }

        public void SetUses(int value) {
            uses = value;
            if (value > 0) {
                SetUsable(true);
            }
            else {
                SetUsable(false);
            }
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
            isUsable = value;
            if (isUsable == true) {
                SetActive(true);
            }
            else if (isUsable == false) {
                SetActive(false);
            }
            id.SetUsable(isUsable); 
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

        /// <summary>
        /// Returns itemData that is specific for candles
        /// </summary>
        /// <returns></returns>
        public override ItemData GetItemData() {
            return new ItemData(this);
        }
    }
}