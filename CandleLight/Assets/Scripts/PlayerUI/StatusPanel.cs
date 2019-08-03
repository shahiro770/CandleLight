﻿/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 11, 2019
* 
* The StatusPanel class is used to display all status bars of a partyMember.
* This includes the player's HPBar, MPBar, and EffectBar.
*
*/

using Characters;
using PanelConstants = Constants.PanelConstants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerUI {

    public class StatusPanel : Panel {

        public Bar HPBar;               /// <value> HPBar of active partyMember </value>
        public Bar MPBar;               /// <value> MPBar of active partyMember </value>
        public EffectsBar effectsBar;   /// <value> EffectBar of active partyMember </value>
        public PartyMember pm = null;

        /// <summary>
        /// Initialize all the bars of the active partyMember
        /// </summary>
        /// <param name="pm"></param>
        public void DisplayPartyMember(PartyMember pm) {
            if (this.pm != null) {
                this.pm.UnsetHPAndMPBar(GetPanelName());
            }

            pm.SetHPAndMPBar(GetPanelName(), HPBar, MPBar);
            this.pm = pm;
        }

        /// <summary>
        /// Returns the name of this panel
        /// </summary>
        /// <returns> Name of panel </returns>
        public override string GetPanelName() {
            return PanelConstants.STATUSPANEL;
        }
        
        /// <summary>
        /// Returns the Button that adjacent panels will navigate to
        /// </summary>
        /// <returns> Button to be navigated to </returns>
        public override Button GetNavigatableButton() {
            return null;
        }
    }
}