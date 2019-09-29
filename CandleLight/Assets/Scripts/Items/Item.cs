/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: August 5, 2019
* 
* The Item class is used to store information about an item
*
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Items {

    public class Item {

        public Sprite itemSprite { get; private set; }  /// <value> Item's sprite </value>
        public string type { get; private set; }        /// <value> Type of item (consumable, weapon, secondary, armor) </value>
        public string subType { get; private set; }     /// <value> SubType of item (varies depending on type) </value>
        public int EXPAmount { get; private set; }      /// <value> Amount of EXP item gives </value>
        public int HPAmount { get; private set; }       /// <value> Amount of HP item gives </value>
        public int MPAmount { get; private set; }       /// <value> Amount of MP item gives </value>
        public int WAXAmount { get; private set; }      /// <value> Amount of WAX item gives </value>

        /// <summary>
        /// Sets image to display a given sprite
        /// </summary>
        /// <param name="spr"> Sprite to be displayed </param>
        public Item(string type, string subType, int amount, Sprite itemSprite) {
            this.type = type;
            this.subType = subType;
            this.itemSprite = itemSprite;

            if (type == "consumable") {
                if (subType == "EXP") {
                    EXPAmount = amount;
                }
                else if (subType == "HP") {
                    HPAmount = amount;
                }
                else if (subType == "MP") {
                    MPAmount = amount;
                }
                else if (subType == "WAX") {
                    WAXAmount = amount;
                }
            }
            else if (type == "gear") {
                if (subType == "secondary") {
                    
                }
            }
        }

        /// <summary>
        /// Empty constructor
        /// </summary>
        public Item() {
            this.type = null;
            this.itemSprite = null;
        }

        /// <summary>
        /// Return's an item's amount based on a subType
        /// </summary>
        /// <param name="subType"> Subtype string </param>
        /// <returns> Positive int amount </returns>
        public int GetAmount(string subType) {
            if (subType == "EXP") {
                return EXPAmount;
            }
            else if (subType == "HP") {
                return HPAmount;
            }
            else if (subType == "MP") {
                return MPAmount;
            }
            else if (subType == "WAX") {
                return WAXAmount;
            }
            else {
                return -1;
            }
        }

        /// <summary>
        /// Returns a string array containing texts that a tooltip uses to determine its keys
        /// TODO: Make this have cases depending on the item sub type
        /// </summary>
        /// <returns> String array length 3 </returns>
        public string[] GetTooltipKeys() {
            return new string[3]{ type, subType, GetAmount(subType).ToString() };
        }
    }
}