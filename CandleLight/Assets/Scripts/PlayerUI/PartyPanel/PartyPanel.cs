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
using System.Collections.Generic;
using UnityEngine.UI;

namespace PlayerUI {

    public class PartyPanel : Panel {
        
        public ActionsPanel actionsPanel;       /// <value> ActionsPanel reference </value>
        public PartyMemberDisplay[] pmDisplays = new PartyMemberDisplay[4];     /// <value> List of partymembers to display </value>
        public StatsPanel statsPanel;           /// <value> Panel for stats </value>
        public bool isOpen = false;             /// <value> Flag for if panel is being displayed </value>

        /// <summary>
        /// Update the partyPanel with relevant information and visuals when opened
        /// </summary>
        void OnEnable() {
            isOpen = true;
            Init(PartyManager.instance.GetPartyMembers());
        }

        /// <summary>
        /// Set isOpen to false on disabling so relevant interactions don't happen
        /// </summary>
        void OnDisable() {
            isOpen = false;
        }

        /// <summary>
        /// Displays each partymember's information
        /// </summary>
        /// <param name="pms"></param>
        public void Init(List<PartyMember> pms) {
            for (int i = 0; i < pmDisplays.Length;i++) {
                pmDisplays[i].gameObject.SetActive(false);
            }
            for (int i = 0; i < pms.Count; i++) {
                int x = i;          // unity loses track of loop variable, so copying somehow fixes this
                pmDisplays[i].gameObject.SetActive(true);
                pmDisplays[i].Init(pms[i].pmvc);
            }

            SetVerticalNavigation();
            Invoke("SetHorizontalNavigation", 0.01f);   // hack because unity actions don't always OnEnable after everything Awakes
        }

        /// <summary>
        /// Adds a new partyMemberDisplay to the partyPanel
        /// </summary>
        /// <param name="pm"> PartyMember to base display off of </param>
        public void AddPartyMemberDisplay(PartyMember pm) {
            int newIndex = PartyManager.instance.GetNumPartyMembers();
            pmDisplays[newIndex - 1].gameObject.SetActive(true);
            pmDisplays[newIndex - 1].Init(pm.pmvc);   
        }

        /// <summary>
        /// Displays the active partyMember with the proper visual colouring
        /// </summary>
        /// <param name="pmd"></param>
        public void DisplayActivePartyMember(PartyMemberDisplay pmd) {
            for (int i = 0; i < PartyManager.instance.GetNumPartyMembers(); i++) {
                pmDisplays[i].ShowNormal();
            }

            pmd.ShowActive();
        }

        /// <summary>
        /// Sets the horizontal navigation for PartyMemberDisplay to other panels
        /// </summary>
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

            actionsPanel.SetHorizontalNavigation(this);
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
            for (int i = 0; i < PartyManager.instance.GetNumPartyMembers(); i++) {
                pmDisplays[i].SetInteractable(true);
            }
        }

        /// <summary>
        /// Disables all of the PartyMemberDisplays to not be clickable or selectable.
        /// Also closes the statsPanel
        /// </summary>
        public void DisableButtons() {
            for (int i = 0; i < PartyManager.instance.GetNumPartyMembers(); i++) {
                pmDisplays[i].SetInteractable(false);
                pmDisplays[i].ShowNormal();
            }
            statsPanel.gameObject.SetActive(false);
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
