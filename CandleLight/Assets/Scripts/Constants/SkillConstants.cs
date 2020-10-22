/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: March 17, 2019
* 
* The SkillConstants class is used to store the types and names of skills as constants.
*
*/

namespace Constants {

    public static class SkillConstants {

        public static readonly int PASSIVE = 0;
        public static readonly int ACTIVE = 1;
        public static readonly int UPGRADE = 2;

        // if skill indexing changes, the order of these values might change
        // -2 and -1 are first and second starting skills respectively
        public enum warriorSkills { SLASH = -2, BASH, FIRESLASH, TAUNT, STEADFAST, MANABLADE, SAFEGUARD, BLOODSWORN, VAMPIRICMAIL, THUNDEROUSANGER, TIMELESSBLADE };
        public enum mageSkills { WHACK = -2, FIREBALL, THIRDEYE, FROST, PYROMANCY, CRITICALMAGIC, HEAL, CANDLEMANCY, MANASHIELD, FIERYVEIL, FROSTGOLEM };   
        public enum archerSkills { ARROW = -2, ENROOT, VANTAGEPOINT, SCAVENGER, POISONARROW, SURVIVALIST, ELVENGIFT, VOLLEY, CURSEDROOTS, BLOOMINGFORTUNE, FLEETFOOTED };
        public enum rogueSkills { STAB = -2, LACERATE, FAEDUST, CLOAKED, WAXTHIEF, AMBUSHER, DEADLY, THUNDEREDGE, KILLERINSTINCT, RITUALDAGGERS, ANGELSMERCY };
    }
}

```
Class   
Warrior   Col 0      Col 1        Col 2              Col 3
          Healthy    Manablade    Vampiric Mail      Tri Slash
          Whirlwind  Fire Slash   Thunderous Might   Draken's Rage
          Steadfast  Bloodsworn   Timeless Blade     Honour Guard

New Skills:
- Healthy doubles HP regen
- Whirlwind sidegrades Taunt, aoe physical attack that hits adjacents
- Tri Slash upgrades Slash, physical attack that scales off of MATK, has a 66% chance of inflict, freeze, burn, or shock
- Draken's Rage provides permanent Rage (PATK up) while a partyMember is dead
_ Honour Guard gives 33% the Warrior's PDEF to all partyMembers (including himself)

Changes:
- Timeless Blade now upgrades Safeguard

Class   
Mage    Col 0       Col 1               Col 2              Col 3
        Third Eye   Critical Magic      Mana Shield        Fire Storm
        Frost       Heal                Fiery Veil         Cripple
        Pyromancy   Candlemancy         Frost Golem        Twin Elements

New Skills:
- Healthy doubles HP regen
- Whirlwind sidegrades Taunt, aoe physical attack that hits adjacents
- Tri Slash upgrades Slash, physical attack that scales off of MATK, has a 66% chance of inflict, freeze, burn, or shock
- Draken's Rage provides permanent Rage (PATK up) while a partyMember is dead
_ Honour Guard gives 33% the Warrior's PDEF to all partyMembers (including himself)

Changes:
- Timeless Blade now upgrades Safeguard

Mage      Whack    Fireball   Channel      Seal
Archer    Arrow    Enroot     Elven Gift   Volley
Rogue     Stab     Lacerate   Fae Dust     Evade

New Skills:
- Channel buffs an ally +MATK for X turns
- Seal debuffs an enemy -MATK for X turns
- Evade buffs an ally +DOG for X turns