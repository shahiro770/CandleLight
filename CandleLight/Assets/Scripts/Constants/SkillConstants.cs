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
        public enum warriorSkills { SLASH = -2, BASH, FIRESLASH, TAUNT, STEADFAST, MANABLADE, SAFEGUARD, BLOODSWORN, VAMPIRICMAIL };
        public enum mageSkills { WHACK = -2, FIREBALL, THIRDEYE, FROST, PYROMANCY, CRITICALMAGIC, HEAL, CANDLEMANCY, MANASHIELD };   
        public enum archerSkills { ARROW = -2, ENROOT, VANTAGEPOINT, SCAVENGER, POISONARROW, SURVIVALIST, ELVENGIFT, VOLLEY, CURSEDROOTS };
        public enum rogueSkills { STAB = -2, LACERATE, FAEDUST, CLOAKED, WAXTHIEF, AMBUSHER, DEADLY, THUNDEREDGE, KILLERINSTINCT };
    }
}