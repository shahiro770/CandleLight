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
using Localization;
using PanelConstants = Constants.PanelConstants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerUI {

    public class PartyMemberDisplay : MonoBehaviour {
        
        /* external component references */
        public Animator pmDisplayAnimator;  /// <value> Animator for entire display </value>
        public Image classIcon;         /// <value> Icon of class </value>
        public Image LVLBackground;     /// <value> Background to where level text is displayed </value>
        public Bar HPBar;               /// <value> Visual for health points </value>
        public Bar MPBar;               /// <value> Visual for mana points </value>
        public EXPBar EXPBar;           /// <value> Visual for experience points </value>
        public Button b;                /// <value> Button to make display clickable for more info </value>
        public LocalizedText LVLText;      /// <value> Text displaying current level </value>

        private PartyMember pm;     /// <value> PartyMember the display is referring to <value>

        /// <summary>
        /// Displays the class, health, and mana of a party member
        /// </summary>
        /// <param name="pm"></param>
        public void Init(PartyMember pm) {
            this.pm = pm;
            classIcon.sprite = Resources.Load<Sprite>("Sprites/Combat/PartyMemberIcons/" + pm.className + "PMIcon");
            pm.SetPartyMemberDisplay(this, PanelConstants.PARTYPANEL, HPBar, MPBar);
        }

        /// <summary>
        /// Displays the class, level, and EXP of a party member
        /// </summary>
        /// <param name="pm"></param>
        public void InitRewardsDisplay(PartyMember pm) {
            this.pm = pm;
            classIcon.sprite = Resources.Load<Sprite>("Sprites/Combat/PartyMemberIcons/" + pm.className + "PMIcon");
            pm.SetPartyMemberDisplayRewardsPanel(this, PanelConstants.REWARDSPANEL, EXPBar, LVLText);

            if (pm.className == "Warrior") {
                LVLBackground.color = new Color32(189, 29, 0, 255);
            }
            else if (pm.className == "Mage") {
                LVLBackground.color = new Color32(0, 152, 220, 255);
            }
            else if (pm.className == "Archer") {
                LVLBackground.color = new Color32(90, 197, 79, 255);
            }
            else if (pm.className == "Thief") {
                LVLBackground.color = new Color32(255, 235, 87, 255);
            }

            SetInteractable(false);
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
        /// Sets the interactivity of the action's button, and handles consequences
        /// </summary>
        /// <param name="value"> Enable interactivity on true and disable on false </param>
         public void SetInteractable(bool value) {
            b.interactable = value;
            classIcon.raycastTarget = value;
        }
    }
}