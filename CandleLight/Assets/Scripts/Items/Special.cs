/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: August 5, 2019
* 
* The Special class is used to store information about an item that is used for quests
*
*/

using UnityEngine;

namespace Items {

    public class Special : Item {

        /// <summary>
        /// Consumable item constructor
        /// </summary>
        public Special(string nameID, string type, string subType) : base(nameID, type, subType) {
            itemSprite = Resources.Load<Sprite>("Sprites/Items/Special/" + nameID);
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="spr"> Sprite to be displayed </param>
        public Special(Special s) : base(s.nameID, s.type, s.subType) {   
            itemSprite = s.itemSprite;
        }

        /// <summary>
        /// Returns how much an item is worth
        /// </summary>
        /// <returns></returns>
        public override int GetWAXValue() {
            return 1;
        }
    }
}