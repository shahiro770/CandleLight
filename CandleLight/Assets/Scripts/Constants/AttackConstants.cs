/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: March 17, 2019
* 
* The AttackConstants class is used to store the types attacks as constants.
* These are not the elemental types, only general categories
*
*/

namespace Constants {

    public static class AttackConstants {

        public static readonly string PHYSICAL = "physical";  /// <value> Attack damage is reduced by PDEF </value>
        public static readonly string MAGICAL = "magical";    /// <value> Attack damage damage is reduced by MDEF</value>
        public static readonly string PHYSICALDEBUFF = "physicalDebuff";  /// <value> Debuff damage is reduced by PDEF </value>
        public static readonly string MAGICALDEBUFF = "magicalDebuff";    /// <value> Debuff damage is reduced by MDEF </value>
        public static readonly string BUFF = "buff";          /// <value> Buffs cannot miss</value>
    }
}