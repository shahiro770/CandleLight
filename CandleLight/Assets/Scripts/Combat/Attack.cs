/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 11, 2019
* 
* The Attack class is used to store enemy attack information.
*
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack {

    public string nameKey { get; set; }             /// <value> Key used to access localized text  </value>
    public string name { get; private set; }        /// <value> Name of attack </value>
    public string effect { get; private set; }      /// <value> Side effect of attack </value>
    public int damage { get; private set; }         /// <value> Damage dealt by attack </value>
    public int cost { get; private set; }           /// <value> Cost to use attack </value>
    public string costType { get; private set; }    /// <value> Cost type (HP, MP) </value>

    /// <summary>
    /// Constructor to initialize name and dammage
    /// </summary>
    /// <param name="name"> Name of attack </param>
    /// <param name="damage"> Amount of damage attack deals </param>
    public Attack(string name, int damage) {
        this.name = name;
        this.damage = damage;
        nameKey = name.ToLower() + "_attack";
    }
}
