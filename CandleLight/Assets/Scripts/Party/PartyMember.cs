/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 11, 2019
* 
* The PartyMember class is used to store information about the PartyMember. 
*
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyMember : Character {
    
    public string className { get; set; }
    public string subClassName { get; set; }
    public string memberName { get; set; }
    public string race { get; set; }


    public void Init(string[] personalInfo, int LVL, int HP, int MP, int[] stats, Attack[] attacks) {
        base.Init(LVL, HP, MP, stats, attacks);
        this.className = personalInfo[0];
        this.subClassName = personalInfo[1];
        this.memberName = personalInfo[2];
        this.race = personalInfo[3];
    }

    public override void LogStats() {
        base.LogStats();
    }

    public override void LogName() {
        Debug.Log("PartyMemberName " + className);
    }
}
