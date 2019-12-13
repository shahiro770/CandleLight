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
        
        public static readonly string BURN = "burn";     /// <value> Lose HP equal to 1 + 20% of attacker's MATK </value>
        public static readonly string POISON = "poison"; /// <value> Lose 5% of max HP </value>
        public static readonly string PDEFUP = "PDEFup"; /// <value> Increases physical defense </value>
        public static readonly string TAUNT = "taunt";   /// <value> Gain 30% PATK and forces target to attack the caster </value>
        public static readonly string FREEZE = "freeze"; /// <value> Reduces PDEF, DOG, and ACC by 30% </value>
        public static readonly string RAGE = "rage";     /// <value> Gain 50% PATK </value>
        public static readonly string BLEED = "bleed";   /// <value> Lose HP equal to 1 + 34% of attacker's PATK, restore half that much health to the attacker</value>
        public static readonly string WEAKNESS = "weakness";    /// <value> Reduces PATK by 30% </value>
    }
}