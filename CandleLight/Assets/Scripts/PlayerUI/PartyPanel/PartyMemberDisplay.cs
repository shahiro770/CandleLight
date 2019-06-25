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
using PanelConstants = Constants.PanelConstants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerUI {

    public class PartyMemberDisplay : MonoBehaviour {

        public Animator pmDisplayAnimator;  /// <value> Animator for entire display </value>
        public Image classIcon;     /// <value> Icon of class </value>
        public Bar HPBar;           /// <value> Visual for health points </value>
        public Bar MPBar;           /// <value> Visual for mana points </value>
        public Button b;            /// <value> Button to make display clickable for more info </value>

        private PartyMember pm;     /// <value> PartyMember the display is referring to <value>

        /// <summary>
        /// Displays the class, health, and mana of a party member
        /// </summary>
        /// <param name="pm"></param>
        public void Init(PartyMember pm) {
            this.pm = pm;
            classIcon.sprite = Resources.Load<Sprite>("Sprites/Combat/PartyMemberIcons/" + pm.className + "PMIcon");
            pm.PMDisplay = this;
            pm.SetHPAndMPBar(PanelConstants.PARTYPANEL, HPBar, MPBar);
        }
        
        /// <summary>
        /// Sets navigation from PartyMemberDisplay's button to another button
        /// </summary>
        /// <param name="direction"> Direction button pressed to navigate </param>
        /// <param name="b2"> Other button </param>
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

        /* public void SetHPAndMPBar(int HP, int CHP, int MP, int CMP) {
            HPBar.SetMaxAndCurrent(HP, CHP);
            MPBar.SetMaxAndCurrent(MP, CMP);
        }

        public void SetHP(int amount) {
            HPBar.SetCurrent(amount);
        }

        public void SetMP(int amount) {
            MPBar.SetCurrent(amount);
        }*/

        /// <summary>
        /// Plays the animation for when the partyMember who's information is being displayed is damaged
        /// </summary>
        /// <returns> Waits for animation to finish playing </returns>
        public IEnumerator PlayDamagedAnimation() {
            pmDisplayAnimator.SetTrigger("damaged");
            do {
                yield return null;    
            } while (pmDisplayAnimator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Idle") == false);
            pmDisplayAnimator.ResetTrigger("damaged");
        }

        /// <summary>
        /// Enables the button, making it clickable
        /// </summary>
        public void Enable() {
            b.interactable = true;
            classIcon.raycastTarget = true;
        }

        /// <summary>
        /// Enables the button, making it clickable
        /// </summary>
        public void Disable() {
            b.interactable = false;
            classIcon.raycastTarget = false;
        }
    }
}
