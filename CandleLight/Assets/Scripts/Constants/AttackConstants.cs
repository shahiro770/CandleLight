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

        public const string PHYSICAL = "physical";  /// <value> Attack damage is reduced by PDEF </value>
        public const string MAGICAL = "magical";    /// <value> Attack damage damage is reduced by MDEF</value>
        public const string DEBUFF = "debuff";      /// <value> Attack that gives a harmful status effect </value>
        public const string BUFF = "buff";          /// <value> Attack that gives a helpful status effect</value>
        public const string BUFFSELF = "buffSelf";  /// <value> Attack that gives a helpful status effect, but can only target the user </value>
        public const string HEALHP = "healHP";      /// <value> Attack that heals someone on the same side as caster </value>
        public const string HEALHPSELF = "healHPSelf";    /// <value> Attack that restores HP, but can only target the user </value> 
        public const string HEALMP = "healMP";      /// <value> Attack that restore MP to someone on the same side as caster </value>
        public const string HEALMPSELF = "healMPSelf";    /// <value> Attack that restores MP, but can only target the user </value> 
        public const string SUMMON = "summon";      /// <value> Adds a temporary party member to combat (limit one per partyMember) </value>

        public enum AttackScopes { 
            single,
            adjacent,
            allMonsters,
            allAllies,
            selfAndRandomAlly
        };
    }
}