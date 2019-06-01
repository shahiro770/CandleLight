/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 11, 2019
* 
* The StatusManager class is used to manage all status bars of a partyMember.
* This includes the player's HPBar, MPBar, and EffectBar.
*
*/

using Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerUI {

    public class StatusManager : MonoBehaviour {

        public EffectBar effectBar; 
        public Bar HPBar;
        public Bar MPBar;

        /// <summary>
        /// Initialize all the bars of the active partyMember
        /// </summary>
        /// <param name="pm"></param>
        public void Init(PartyMember pm) {
            pm.HPBar = HPBar;
            pm.MPBar = MPBar;
            HPBar.Init(pm.HP, pm.CHP);
            MPBar.Init(pm.MP, pm.CMP);
        }
    }
}