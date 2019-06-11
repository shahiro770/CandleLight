/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: June 9, 2019
* 
* The PartyMemberdisplay class is used to display some relevant information about an individual partyMember.
* A PartyMemberDisplay is shown in the PartyPanel for each PartyMember.
*
*/

using Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerUI {

    public class PartyMemberDisplay : MonoBehaviour {

        public Image classIcon;     /// <value> Icon of class </value>
        public Bar HPBar;           /// <value> Visual for health points </value>
        public Bar MPBar;           /// <value> Visual for mana points </value>

        private PartyMember pm;     /// <value> PartyMember the display is referring to <value>

        /// <summary>
        /// Displays the class, health, and mana of a party member
        /// </summary>
        /// <param name="pm"></param>
        public void Init(PartyMember pm) {
            this.pm = pm;
            classIcon.sprite = Resources.Load<Sprite>("Sprites/Combat/PartyMemberIcons/" + pm.className + "PMIcon");
            pm.PartyPanelHPBar = HPBar;
            pm.PartyPanelMPBar = MPBar;
            HPBar.SetMaxAndCurrent(pm.HP, pm.CHP);
            MPBar.SetMaxAndCurrent(pm.MP, pm.CMP);
        }

        /// <summary>
        /// Updates the display to show the health of a partyMember
        /// </summary>
        public void UpdateDisplay() {
            HPBar.SetMaxAndCurrent(pm.HP, pm.CHP);
        }
    }
}
