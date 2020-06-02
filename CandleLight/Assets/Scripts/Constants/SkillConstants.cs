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

        public static readonly string ACTIVE = "active";
        public static readonly string PASSIVE = "passive";

        // if skill indexing changes, the order of these values might change
        public enum warriorSkills { FIRESLASH, TAUNT, STEADFAST, MANABLADE, SAFEGUARD };
        public enum mageSkills { THIRDEYE, FROST, PYROMANCY, CRITICALMAGIC, HEAL };   
        public enum archerSkills { VANTAGEPOINT, SCAVENGER, POISONARROW, SURVIVALIST, ELVENGIFT };
        public enum rogueSkills { FAEDUST, CLOAKED, WAXTHIEF, AMBUSHER, DEADLY };
    }
}