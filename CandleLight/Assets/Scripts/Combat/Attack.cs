﻿/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 11, 2019
* 
* The Attack class is used to store information about an action monsters and players can take
* to hurt another's feelings.
*
*/

using UnityEngine;

namespace Combat {

    [System.Serializable]
    public class Attack {

        [field: SerializeField] public string animationClipName { get; private set; }   /// <value> Name of animation to be played when used </value>
        [field: SerializeField] public string costType { get; private set; }    /// <value> Cost type (HP, MP) </value>
        [field: SerializeField] public string nameKey { get; set; }             /// <value> Key used to access localized text  </value>
        [field: SerializeField] public string name { get; private set; }        /// <value> Name of attack </value>
        [field: SerializeField] public string type { get; private set; }        /// <value> Type of attack </value>
        [field: SerializeField] public string formula { get; private set; }     /// <value> Damage dealt by attack </value>
        [field: SerializeField] public string scope { get; private set; }       /// <value> String describing the number of targets the attack affects </value>
        [field: SerializeField] public string seName { get; private set; }      /// <value> Status effect of attack </value>
        [field: SerializeField] public int seDuration { get; private set; }     /// <value> Number of turns status effect lasts </value>       
        [field: SerializeField] public int seChance { get; private set; }       /// <value> Chance of status effect occuring </value>       
        [field: SerializeField] public int cost { get; private set; }           /// <value> Cost to use attack </value>      
        
        public int attackValue;  /// <value> Amount related to attack </value>

        /// <summary>
        /// Constructor to initialize attack's properties
        /// </summary>
        /// <param name="name"> Name of attack </param>
        /// <param name="type"> Type of attack </param>
        /// <param name="formula"> Amount of damage attack deals </param>
        /// <param name="costType"> Type of cost (MP or HP) </param>
        /// <param name="cost"> Cost of attack (in MP or HP) </param>
        /// <param name="scope"> String describing the targets this attack hits </param>
        /// <param name="animationClipName"> Animation clip to play when attack is used </param>
        public Attack(string name, string type, string formula, string seName, int seDuration, int seChance, string costType, int cost, string scope, string animationClipName) {
            this.name = name;
            this.type = type;
            this.formula = formula;
            this.seName = seName;
            this.seDuration = seDuration;
            this.seChance = seChance;
            this.costType = costType;
            this.cost = cost;            
            this.scope = scope;
            this.animationClipName = animationClipName; 
            nameKey = name + "_attack";
        }
    }
}
