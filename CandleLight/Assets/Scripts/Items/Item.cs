/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: August 5, 2019
* 
* The Item class is used to store information about an item
*
*/

using ClassConstants = Constants.ClassConstants;
using ItemDisplay = PlayerUI.ItemDisplay;
using UnityEngine;

namespace Items {

    public class Item {

        public ItemDisplay id;                          /// <value> Reference to the visual component of the item </value>
        
        public string nameID;                           /// <value> Name of item </value>
        public string type { get; private set; }        /// <value> Type of item (consumable, weapon, secondary, armor) </value>
        public string subType { get; private set; }     /// <value> SubType of item (varies depending on type) </value>
        public string className = ClassConstants.ANY;    /// <value> Class that can use this item </value>

        public Sprite itemSprite;  /// <value> Item's sprite </value>
        public string[] effects = new string[3];            /// <value> List of effects </value>
        public int[] values = new int[3];                   /// <value> List of values associated with effects </value>
        public int effectsNum = 0;                          /// <value> Number of effects </value>
        public int WAXvalue { get; protected set; } = 0;    /// <value> WAX value of item </value>       

        /// <summary>
        /// Sets image to display a given sprite
        /// </summary>
        /// <param name="spr"> Sprite to be displayed </param>
        public Item(string nameID, string type, string subType) {
            this.nameID = nameID;
            this.type = type;
            this.subType = subType;
        }

        /// <summary>
        /// Empty constructor
        /// </summary>
        public Item() {
            this.itemSprite = null;
        }

        /// <summary>
        /// Returns effects
        /// </summary>
        /// <returns></returns>
        public virtual string[] GetEffects() {
            return effects;
        }

        /// <summary>
        /// Returns amounts
        /// </summary>
        /// <returns></returns>
        public virtual int[] GetAmounts() {
            return null;
        }

        /// <summary>
        /// Returns how much an item is worth
        /// </summary>
        /// <returns></returns>
        public virtual void CalculateWAXValue() {
            return;
        }

        /// <summary>
        /// Returns amounts as strings
        /// </summary>
        /// <returns></returns>
        public virtual string[] GetAmountsAsStrings() {
            string[] amountStrings = new string[effects.Length];
            for (int i = 0; i < effects.Length; i++) {
                if (effects[i] == "%MAXHP" || effects[i] == "CRITCHANCE" || effects[i] == "CHAMPIONCHANCE") {     // TODO Make this account for percent effects
                    amountStrings[i] = values[i] + "%";
                }
                else if (effects[i] == "BURNPLUS" || effects[i] == "POISONPLUS" || effects[i] == "BLEEDPLUS" 
                || effects[i] == "MPREGENDOUBLE" || effects[i] == "HPREGENDOUBLE") {
                    amountStrings[i] = "";
                }
                else {
                    amountStrings[i] = values[i].ToString();
                }
            }

            return amountStrings;
        }

        /// <summary>
        /// Returns a string array containing texts that a tooltip uses to determine its keys
        /// </summary>
        /// <returns> String array length 3 </returns>
        public virtual string[] GetTooltipBasicKeys() {
            return new string[3]{ nameID, type, subType };
        }
        
        /// <summary>
        /// Returns effect's keys for tooltips
        /// </summary>
        /// <returns></returns>
        public virtual string[] GetTooltipEffectKeys() {
            return null;
        }

        /// <summary>
        /// Updates the item's itemDisplays' sprite to the desired sprite
        /// </summary>
        /// <param name="s"></param>
        public virtual void SetSprite(Sprite s) {}
        
        /// <summary>
        /// Returns item data for saving
        /// </summary>
        /// <returns></returns>
        public virtual ItemData GetItemData() {
            return new ItemData(this);
        }
    }
}