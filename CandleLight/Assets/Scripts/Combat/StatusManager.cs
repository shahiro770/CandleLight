/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 11, 2019
* 
* The StatusManager class is used to manage all status bars of a partyMember.
* This includes the player's HPBar, MPBar, and EffectBar.
*
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusManager : MonoBehaviour
{
    public EffectBar effectBar;
    public Bar HPBar;
    public Bar MPBar;

    public void Init(PartyMember pm) {
        pm.HPBar = HPBar;
        pm.MPBar = MPBar;
        HPBar.Init(pm.HP, pm.CHP);
        MPBar.Init(pm.MP, pm.CMP);
    }
}
