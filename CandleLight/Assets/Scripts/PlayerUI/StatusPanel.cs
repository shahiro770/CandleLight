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
using PanelConstants = Constants.PanelConstants;
using UnityEngine.UI;

namespace PlayerUI {

    public class StatusPanel : Panel {

        public Bar HPBar;               /// <value> HPBar of active partyMember </value>
        public Bar MPBar;               /// <value> MPBar of active partyMember </value>
        public EffectsBar effectsBar;   /// <value> EffectBar of active partyMember </value>
        public PartyMemberVisualController pmvc = null;     /// <value> Controller for all visuals related to partyMember</value>

        /// <summary>
        /// Initialize all the bars of the active partyMember
        /// </summary>
        /// <param name="pm"></param>
        public void DisplayPartyMember(PartyMemberVisualController pmvc) {
            if (this.pmvc != null) {    // forget what this was for
                this.pmvc.UnsetHPAndMPBar(GetPanelName());
            }

            pmvc.SetHPAndMPBar(GetPanelName(), HPBar, MPBar);
            this.pmvc = pmvc;
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