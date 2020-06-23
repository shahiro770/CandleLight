/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: March 17, 2019
* 
* The ItemConstants class is used to store the types items and itemSlots as constants
*
*/

namespace Constants {

    public static class ItemConstants {

        public static readonly string CONSUMABLE = "consumable";    /// <value> Consumable items are used instantly </value>
        public static readonly string CANDLE = "candle";            /// <value> Candle items provide buffs and debuffs while usable  </value>
        public static readonly string GEAR = "gear";                /// <value> Gear items provide constant buffs </value>
        public static readonly string SPECIAL = "special";          /// <value> </value>
        public static readonly string ANY = "any";                  /// <value> Item slot that can be placed in any slot </value>
        public static readonly string WEAPON = "weapon";            /// <value> Gear item that can only be placed in the weapon slot </value>
        public static readonly string SECONDARY = "secondary";      /// <value> Gear item that can only be placed in the secondary slot </value>
        public static readonly string ARMOUR = "armour";            /// <value> Gear item that can only be placed in the armour slot </value>
    }
}