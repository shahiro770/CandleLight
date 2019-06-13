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
<<<<<<< HEAD
        public Button b;            /// <value> Button to make display clickable for more info </value>
=======
>>>>>>> c9f502a07ca3538ce1c2a527cdb251c2bb101438

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
<<<<<<< HEAD
        
        public void SetNavigation(string direction, Button b2) {
            Navigation n = b.navigation;

            if (direction == "up") {
                n.selectOnUp = b2;
                b.navigation = n;
            }
            else if (direction == "right") {
                n.selectOnRight = b2;
                b.navigation = n;
            }
            else if (direction == "down") {
                n.selectOnDown = b2;
                b.navigation = n;
            }
            else if (direction == "left") {
                n.selectOnLeft = b2;
                b.navigation = n;
            }

            b.navigation = n;
        }

        public void Enable() {
            b.interactable = true;
            classIcon.raycastTarget = true;
        }

        public void Disable() {
            b.interactable = false;
            classIcon.raycastTarget = false;
        }
=======
>>>>>>> c9f502a07ca3538ce1c2a527cdb251c2bb101438
    }
}
