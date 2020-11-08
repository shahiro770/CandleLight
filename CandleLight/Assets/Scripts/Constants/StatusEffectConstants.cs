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

    public enum StatusEffectConstant {
        NONE,                   /// <value> No status effect </value>  
        BURN,                   /// <value> Lose HP equal to % of attacker's MATK </value>
        POISON,                 /// <value> Lose % of max HP </value>
        TAUNT,                  /// <value> Attacker gains PATK and is forced to attack the caster </value>
        FREEZE,                 /// <value> Reduces PDEF and ACC </value>
        RAGE,                   /// <value> Gain PATK </value>
        BLEED,                  /// <value> Lose HP equal to % attacker's PATK, restore that much health to the attacker </value>
        WEAKNESS,               /// <value> Reduces PATK </value>
        ADVANTAGE,              /// <value> Gain increased crit chance </value>
        ROOT,                   /// <value> DOG is reduced by 50% and takes double damage from burn </value>
        TRAP,                   /// <value> Attempting to escape is much harder </value>
        REGENERATE,             /// <value> Restoring HP at the end of its turn </value>
        FOCUS,                  /// <value> Restoring MP at the end of its turn </value>
        STUN,                   /// <value> Cannot act on their turn </value>
        SHOCK,                  /// <value> Taking bonus damage from magical attacks </value>
        GUARD,                  /// <value> Gains PDEF </value>
        CURE,                   /// <value> Debuffs last half as long </value>
        MIRACLE,                /// <value> User takes no damage from direct attacks </value>
        ETHEREAL,               /// <value> Gain MATK </value>
        FATALWOUND,             /// <value> Game ending debuff </value>
        VAMPIRE,                /// <value> Has bleedPlus (increased bleed damage) </value>
        RBW,                    /// <value> Randomly decide between root, bleed, and weakness </value>
        FROSTBITE,              /// <value> Magical damage over time scaling off of PATK, doubled if inflicted with FREEZE </value>
        BARRIER,                /// <value> Increases MDEF </value>
        MARIONETTE,             /// <value> Decreases DOG and is taunted </value>
        NIMBLE,                 /// <value> Increases DOG </value>
        SILENCE,                /// <value> Decreases MATK </value>
        FAMILIAR,               /// <value> Indicates a temporary partyMember </value>
        BOSS,                   /// <value> Makes the monster stronger against shenanigans </value>
        CHAMPIONHP,             /// <value> Bonus HP and HP regen  </value>
        CHAMPIONATK,            /// <value> Bonus ATK (used to indicate CHAMPIONPATK or CHAMPIONMATK) </value>
        CHAMPIONPATK,           /// <value> Bonus PATK and HP  </value>
        CHAMPIONMATK,           /// <value> Bonus MATK and HP </value>       
        CHAMPIONDEF,            /// <value> Bonus DEF (used to indicate CHAMPIONPDEF or CHAMPIONMDEF) </value>
        CHAMPIONPDEF,           /// <value> Bonus PDEF and HP  </value>
        CHAMPIONMDEF,           /// <value> Bonus MDEF and HP </value>
        SCUM                    /// <value> HP and MP regen are reduced </value>  
    }
}