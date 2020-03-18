/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: October 17, 2019
* 
* The StatusEffectConstants class is used to store the types statusEffects as constants.
*
*/

namespace Constants {

    public static class StatusEffectConstants {
        
        public static readonly string BURN = "burn";     /// <value> Lose HP equal to % of attacker's MATK </value>
        public static readonly string POISON = "poison"; /// <value> Lose % of max HP </value>
        public static readonly string PDEFUP = "PDEFup"; /// <value> Increases physical defense </value>
        public static readonly string TAUNT = "taunt";   /// <value> Attacker gains PATK and is forced to attack the caster </value>
        public static readonly string FREEZE = "freeze"; /// <value> Reduces PDEF, DOG, and ACC </value>
        public static readonly string RAGE = "rage";     /// <value> Gain PATK </value>
        public static readonly string BLEED = "bleed";   /// <value> Lose HP equal to % attacker's PATK, restore that much health to the attacker </value>
        public static readonly string WEAKNESS = "weakness";    /// <value> Reduces PATK </value>
        public static readonly string ADVANTAGE = "advantage";  /// <value> Gain increased crit chance </value>
        public static readonly string ROOT = "root"; /// <value> DOG is reduced by 100% </value>
    }
}