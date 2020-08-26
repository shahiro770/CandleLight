/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: October 17, 2019
* 
* The StatusEffectConstants class is used to store the types statusEffects as constants.
* IMPORTANT: Make sure to update statusEffect's buff and not buff switch statement when adding more constants 
*
*/

namespace Constants {

    public static class StatusEffectConstants {
        
        public const string BURN = "burn";     /// <value> Lose HP equal to % of attacker's MATK </value>
        public const string POISON = "poison"; /// <value> Lose % of max HP </value>
        public const string TAUNT = "taunt";   /// <value> Attacker gains PATK and is forced to attack the caster </value>
        public const string FREEZE = "freeze"; /// <value> Reduces PDEF, DOG, and ACC </value>
        public const string RAGE = "rage";     /// <value> Gain PATK </value>
        public const string BLEED = "bleed";   /// <value> Lose HP equal to % attacker's PATK, restore that much health to the attacker </value>
        public const string WEAKNESS = "weakness";    /// <value> Reduces PATK </value>
        public const string ADVANTAGE = "advantage";  /// <value> Gain increased crit chance </value>
        public const string ROOT = "root";     /// <value> DOG is reduced by 50% and takes double damage from burn </value>
        public const string TRAP = "trap";     /// <value> Attempting to escape is much harder </value>
        public const string REGENERATE = "regenerate";   /// <value> Restoring HP at the end of its turn </value>
        public const string FOCUS = "focus";   /// <value> Restoring MP at the end of its turn </value>
        public const string STUN = "stun";     /// <value> Cannot act on their turn </value>
        public const string SHOCK = "shock";   /// <value> Taking bonus damage from magical attacks</value>
        public const string GUARD = "guard";   /// <value> Gains PDEF </value>
        public const string CURE = "cure";     /// <value> Debuffs last half as long </value>
        public const string MIRACLE = "miracle";     /// <value>  User gains max PDEF and MDEF </value>
        public const string FATALWOUND = "fatalWound";  /// <value> Game ending debuff </value>
        public const string VAMPIRE = "vampire";      /// <value> Has bleedPlus (increased bleed damage) </value>
        public const string BOSS = "boss";     /// <value> Makes the enemy stronger against shenanigans </value>
        public const string RBW = "rbw";       /// <value> Randomly decide between root, bleed, and weakness </value>
        public const string FROSTBITE = "frostbite";          /// <value> Magical damage over time, doubled if inflicted with FREEZE </value>
        public const string BARRIER = "barrier";              /// <value> Gains MDEF </value>
        public const string MARIONETTE = "marionette";        /// <value> Reducees MDEF and is taunted </value>
        public const string FAMILIAR = "familiar";            /// <value> Indicates a temporary partyMember </value>
        public const string CHAMPIONHP = "championHP";        /// <value> Bonus HP and HP regen  </value>
        public const string CHAMPIONATK = "championATK";      /// <value> Bonus ATK (used to indicate CHAMPIONPATK or CHAMPIONMATK) </value>
        public const string CHAMPIONPATK = "championPATK";    /// <value> Bonus PATK and HP  </value>
        public const string CHAMPIONMATK = "championMATK";    /// <value> Bonus MATK and HP </value>
        public const string CHAMPIONDEF = "championDEF";      /// <value> Bonus DEF (used to indicate CHAMPIONPDEF or CHAMPIONMDEF) </value>
        public const string CHAMPIONPDEF = "championPDEF";    /// <value> Bonus PDEF and HP  </value>
        public const string CHAMPIONMDEF = "championMDEF";    /// <value> Bonus MDEF and HP </value>
    }
}