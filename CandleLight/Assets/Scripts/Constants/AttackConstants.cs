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
        public static readonly string DEBUFF = "debuff";      /// <value> Attack that gives a harmful status effect </value>
        public static readonly string BUFF = "buff";          /// <value> Attack that gives a helpful status effect</value>
        public static readonly string BUFFSELF = "buffSelf";  /// <value> Attack that gives a helpful status effect, but can only target the user </value>
        public static readonly string HEALHP = "healHP";      /// <value> Attack that heals someone on the same side as caster </value>
        public static readonly string HEALHPSELF = "healHPSelf";    /// <value> Attack that restores HP, but can only target the user </value> 
        public static readonly string HEALMP = "healMP";      /// <value> Attack that restore MP to someone on the same side as caster </value>
        public static readonly string HEALMPSELF = "healMPSelf";    /// <value> Attack that restores MP, but can only target the user </value> 
    }
}