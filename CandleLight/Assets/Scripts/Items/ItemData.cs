/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: August 5, 2019
* 
* The ItemData class is used to store information about an item that is in the party's inventory
* for saving purposes.
*
*/

using Attack = Combat.Attack;
using ClassConstants = Constants.ClassConstants;
using UnityEngine;

namespace Items {

    [System.Serializable]
    public class ItemData {

        public string nameID;                           /// <value> Name of item </value>
        public string type { get; private set; }        /// <value> Type of item (consumable, weapon, secondary, armor) </value>
        public string subType { get; private set; }     /// <value> SubType of item (varies depending on type) </value>
        public string className = ClassConstants.ANY;    /// <value> Class that can use this item </value>

        public string[] effects = new string[3];            /// <value> List of effects </value>
        public int[] values = new int[3];                   /// <value> List of values associated with effects </value>
        public int effectsNum = 0;                          /// <value> Number of effects </value>
        
        public Attack a;
        public int uses;
        public int maxUses;

        /// <summary>
        /// Gear or special item data
        /// </summary>
        /// <param name="i"></param>
        public ItemData(Item i) {
            this.nameID = i.nameID;
            this.type = i.type;
            this.subType = i .subType;
            this.className = i.className;
            this.effects = i.effects;
            this.values = i.values;
            this.effectsNum = i.effectsNum;
        }

        /// <summary>
        /// Candle item data
        /// </summary>
        /// <param name="c"></param>
        public ItemData(Candle c) {
            this.nameID = c.nameID;
            this.type = c.type;
            this.subType = c .subType;
            this.className = c.className;
            this.effects = c.effects;
            this.values = c.values;
            this.effectsNum = c.effectsNum;
            this.a = c.a;
            this.uses = c.uses;
            this.maxUses = c.maxUses;
        }
    }
}