/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 11, 2019
* 
* The Attack class is used to store information about an action enemies and players can take
* to hurt another's feelings.
*
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combat {

    [System.Serializable]
    public class Attack {

        [field: SerializeField] public string nameKey { get; set; }             /// <value> Key used to access localized text  </value>
        [field: SerializeField] public string name { get; private set; }        /// <value> Name of attack </value>
        [field: SerializeField] public string effect { get; private set; }      /// <value> Side effect of attack </value>
        [field: SerializeField] public string animation { get; private set; }   /// <value> animation to be played when used </value>
        [field: SerializeField] public int damage { get; private set; }         /// <value> Damage dealt by attack </value>
        [field: SerializeField] public int cost { get; private set; }           /// <value> Cost to use attack </value>
        [field: SerializeField] public string costType { get; private set; }    /// <value> Cost type (HP, MP) </value>

        /// <summary>
        /// Constructor to initialize name and damage
        /// </summary>
        /// <param name="name"> Name of attack </param>
        /// <param name="damage"> Amount of damage attack deals </param>
        public Attack(string name, int damage, string animation) {
            this.name = name;
            this.damage = damage;
            this.animation = animation; 
            nameKey = name.ToLower() + "_attack";
        }
    }
}
