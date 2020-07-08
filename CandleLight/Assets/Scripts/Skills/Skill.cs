/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: Decenber 29, 2019
* 
* The Skill class holds information about a skill a partyMember can learn to 
* gain new passive effects or attacks.
*
*/

using Attack = Combat.Attack;
using SkillConstants = Constants.SkillConstants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skills {

    public class Skill {
        
        public string name;
        public int type;
        public int upgradeSkill;

        public Attack a;
        public Attack storedAttack;   /// <value> Store attack being replaced </value>
        public Color skillColour;

        public string titleKey;
        public string subKey; 
        public string desKey;
        public bool skillEnabled;
        
        public Skill(string name, int type, int upgradeSkill, Attack a, Color skillColour) {
            this.name = name;
            this.type = type;
            this.upgradeSkill = upgradeSkill;
            this.a = a;
            this.skillColour = skillColour;

            titleKey = name + "_skill_title";
            subKey = name + "_skill_sub";
            desKey = name + "_skill_des";
            skillEnabled = false;
        }
    }
}
