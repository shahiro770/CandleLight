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
using Party;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerUI {

    public class PartyPanel : Panel {

        public PartyMemberDisplay[] pmDisplays = new PartyMemberDisplay[4];     /// <value> List of partymembers to display </value>

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
        }

        public void AddPartyMember(PartyMember pm) {
            int newIndex = PartyManager.instance.GetNumPartyMembers();
            pmDisplays[newIndex - 1].gameObject.SetActive(true);
            pmDisplays[newIndex - 1].Init(pm);
        }

        public void SetHorizontalNavigation(ActionsPanel ap) {
            if (ap.actions[1].isEnabled) {
                foreach(PartyMemberDisplay pmd in pmDisplays) {
                    pmd.SetNavigation("left", ap.actions[1].b);
                }
            }
            else if (ap.actions[0].isEnabled) {
                foreach(PartyMemberDisplay pmd in pmDisplays) {
                    pmd.SetNavigation("left", ap.actions[0].b);
                }
            }
            else if (ap.actions[4].isEnabled) {
                foreach(PartyMemberDisplay pmd in pmDisplays) {
                    pmd.SetNavigation("left", ap.actions[4].b);
                }
            }
        }

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

        public void EnableButtons() {
            foreach(PartyMemberDisplay pmd in pmDisplays) {
                pmd.Enable();
            }
        }

        public void DisableButtons() {
            foreach(PartyMemberDisplay pmd in pmDisplays) {
                pmd.Disable();
            }
        }

        public override Button GetNavigatableButton() {
            return pmDisplays[0].b;
        }

        public override string GetPanelName() {
            return "party";
        }
    }
}
