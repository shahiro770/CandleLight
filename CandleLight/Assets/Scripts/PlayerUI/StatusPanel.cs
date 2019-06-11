/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 11, 2019
* 
* The StatusPanel class is used to display all status bars of a partyMember.
* This includes the player's HPBar, MPBar, and EffectBar.
*
*/

using Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerUI {

    public class StatusPanel : MonoBehaviour {

        public Bar HPBar;
        public Bar MPBar;
        public EffectBar effectBar; 

        /// <summary>
        /// Initialize all the bars of the active partyMember
        /// </summary>
        /// <param name="pm"></param>
        public void Init(PartyMember pm) {
            pm.HPBar = HPBar;
            pm.MPBar = MPBar;
            HPBar.SetMaxAndCurrent(pm.HP, pm.CHP);
            MPBar.SetMaxAndCurrent(pm.MP, pm.CMP);
        }
    }
}