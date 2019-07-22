/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: June 9, 2019
* 
* The PartyPanel class manages various visuals that show the player the status of all members
* of their party. 
*
*/

using Characters;
using PanelConstants = Constants.PanelConstants;
using Party;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerUI {

    public class PartyPanel : Panel {

        public ActionsPanel actionsPanel;
        public PartyMemberDisplay[] pmDisplays = new PartyMemberDisplay[4];     /// <value> List of partymembers to display </value>

        void Start() {
            Init(PartyManager.instance.GetPartyMembers());
        }

        /// <summary>
        /// Displays each partymember's information
        /// </summary>
        /// <param name="pms"></param>
        public void Init(List<PartyMember> pms) {
            for (int i = 0; i < pms.Count; i++) {
                pmDisplays[i].gameObject.SetActive(true);
                pmDisplays[i].Init(pms[i]);
            }

            SetVerticalNavigation();
            SetHorizontalNavigation();
            actionsPanel.SetHorizontalNavigation(this);
        }

        /// <summary>
        /// Adds a new partyMemberDisplay to the partyPanel
        /// </summary>
        /// <param name="pm"> PartyMember to base display off of </param>
        public void AddPartyMemberDisplay(PartyMember pm) {
            int newIndex = PartyManager.instance.GetNumPartyMembers();
            pmDisplays[newIndex - 1].gameObject.SetActive(true);
            pmDisplays[newIndex - 1].Init(pm);
        }

        /// <summary>
        /// Sets the horizontal navigation for PartyMemberDisplay to other panels
        /// </summary>
        /// <param name="actionsPanel"> ActionsPanel reference </param>
        public void SetHorizontalNavigation() {
            if (actionsPanel.actions[1].IsInteractable()) {
                foreach(PartyMemberDisplay pmd in pmDisplays) {
                    pmd.SetNavigation("left", actionsPanel.actions[1].b);
                }
            }
            else if (actionsPanel.actions[0].IsInteractable()) {
                foreach(PartyMemberDisplay pmd in pmDisplays) {
                    pmd.SetNavigation("left", actionsPanel.actions[0].b);
                }
            }
            else if (actionsPanel.actions[4].IsInteractable()) {
                foreach(PartyMemberDisplay pmd in pmDisplays) {
                    pmd.SetNavigation("left", actionsPanel.actions[4].b);
                }
            }
        }

        /// <summary>
        /// Sets the vertical navigation between PartyMemberDisplays
        /// </summary>
        public void SetVerticalNavigation() {
            if (pmDisplays.Length > 1) {
                for (int i = 0; i < pmDisplays.Length; i++) {
                    if (i == 0) {
                        pmDisplays[i].SetNavigation("down", pmDisplays[i + 1].b);
                    }
                    else if (i == pmDisplays.Length - 1) {
                        pmDisplays[i].SetNavigation("up", pmDisplays[i - 1].b);
                    }
                    else {
                        pmDisplays[i].SetNavigation("up", pmDisplays[i - 1].b);
                        pmDisplays[i].SetNavigation("down", pmDisplays[i + 1].b);
                    }
                } 
            }
        }

        /// <summary>
        /// Enables all of the PartyMemberDisplays to be clickable and selectable
        /// </summary>
        public void EnableButtons() {
            foreach(PartyMemberDisplay pmd in pmDisplays) {
                pmd.Enable();
            }
        }

        /// <summary>
        /// Disables all of the PartyMemberDisplays to not be clickable or selectable
        /// </summary>
        public void DisableButtons() {
            foreach(PartyMemberDisplay pmd in pmDisplays) {
                pmd.Disable();
            }
        }

        /// <summary>
        /// Returns the name of this panel
        /// </summary>
        /// <returns> Name of panel </returns>
        public override string GetPanelName() {
            return PanelConstants.PARTYPANEL;
        }

        /// <summary>
        /// Returns the Button that adjacent panels will navigate to
        /// </summary>
        /// <returns> Button to be navigated to </returns>
        public override Button GetNavigatableButton() {
            return pmDisplays[0].b;
        }
    }
}
