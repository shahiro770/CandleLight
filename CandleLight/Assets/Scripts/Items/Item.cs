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

        public Sprite itemSprite { get; private set; }
        public string type { get; private set; }
        public int EXPAmount { get; private set; }      /// <value> Amount of EXP item gives </value>
        public int HPAmount { get; private set; }       /// <value> Amount of HP item gives </value>
        public int MPAmount { get; private set; }       /// <value> Amount of MP item gives </value>
        public int WAXAmount { get; private set; }      /// <value> Amount of WAX item gives </value>
        public bool isConsumable { get; private set; }

        /// <summary>
        /// Sets image to display a given sprite
        /// </summary>
        /// <param name="spr"> Sprite to be displayed </param>
        public Item(string type, int amount, Sprite itemSprite) {
            this.type = type;
            this.itemSprite = itemSprite;

            if (type == "EXP") {
                EXPAmount = amount;
                isConsumable = true;
            }
            else if (type == "HP") {
                HPAmount = amount;
                isConsumable = true;
            }
            else if (type == "MP") {
                MPAmount = amount;
                isConsumable = true;
            }
            else if (type == "WAX") {
                WAXAmount = amount;
                isConsumable = true;
            }
        }

        public int GetAmount(string type) {
            if (type == "EXP") {
                return EXPAmount;
            }
            else if (type == "HP") {
                return HPAmount;
            }
            else if (type == "MP") {
                return MPAmount;
            }
            else if (type == "WAX") {
                return WAXAmount;
            }
            else {
                return -1;
            }
        }
    }
}