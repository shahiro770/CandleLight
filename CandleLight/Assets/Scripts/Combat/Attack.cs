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

    public string nameKey { get; set; } // used to access localized text

    public string name { get; }
    public string effect { get; }
    public int damage { get; }

    public Attack(string name, int damage) {
        this.name = name;
        this.damage = damage;
        nameKey = name.ToLower() + "_attack";
    }
}
