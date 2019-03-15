/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 11, 2019
* 
* The PartyMember class is used to store information about a member of the party. 
* It is always attached to a PartyMember GameObject.
*
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyMember : Character {
    
    public string className { get; set; }       /// <value> Warrior, Mage, Archer, or Thief </value>
    public string subClassName { get; set; }    /// <value> Class specializations </value>
    public string memberName { get; set; }      /// <value> Name of the party member </value>
    public string race { get; set; }            /// <value> Human, Lizardman, Undead, etc. </value>
    public Bar HPBar { get; set; }              /// <value> Visual for health points </value>
    public Bar MPBar { get; set; }              /// <value> Visual for mana points </value>

    /// <summary>
    /// When a PartyMember GO is instantiated, it needs to have its values initialized
    /// </summary> 
    /// <param name="personalInfo"> className, subClassName, memberName, and race in an array </param>
    /// <param name="LVL"> Power level </param>
    /// <param name="HP"> Health points </param>
    /// <param name="MP"> Mana Points </param>
    /// <param name="attacks"> List of known attacks (length 4)</param>
    public void Init(string[] personalInfo, int LVL, int HP, int MP, int[] stats, Attack[] attacks) {
        base.Init(LVL, HP, MP, stats, attacks);
        this.className = personalInfo[0];
        this.subClassName = personalInfo[1];
        this.memberName = personalInfo[2];
        this.race = personalInfo[3];
    }

    /// <summary>
    /// Reduce the PartyMember's current health points by a specified amount.false
    /// IEnumerator is used to make calling function wait for its completion
    /// </summary> 
    /// <param name="amount"> Amount of health points lost </param>
    public override IEnumerator LoseHP(int amount) {
        CHP -= amount;
        if (CHP < 0) {
            CHP = 0;
        }
        
        HPBar.SetCurrent(CHP);  // update visually

        yield break;
    }

    /// <summary>
    /// Check if the party member is dead
    /// </summary>
    /// <returns></returns>
    public bool CheckDeath() {
        return CHP == 0;
    }

    /// <summary>
    /// Log stats informaton about the PartyMember for debugging
    /// </summary> 
    public override void LogStats() {
        base.LogStats();
    }

    /// <summary>
    /// Log party member's class name
    /// </summary> 
    public override void LogName() {
        Debug.Log("PartyMemberName " + className);
    }
}
