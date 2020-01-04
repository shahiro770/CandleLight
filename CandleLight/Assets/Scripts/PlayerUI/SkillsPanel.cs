/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: August 1, 2019
* 
* The skillsPanel class is used to show a partyMember's skill tree, and handle
* all interactions that would happen on it.
*
*/

using Characters;
using Localization;
using Party;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerUI {

    public class SkillsPanel : Panel {

        public SkillDisplay[] skillDisplays = new SkillDisplay[12];
        public PartyMemberDisplay[] pmDisplays = new PartyMemberDisplay[4];

        public int[] colPoints = new int[] { 0, 0, 0, 0 };
        public bool isOpen;             /// <value> Flag for if this panel is open (true if open, false otherwise) </value>

        private bool initializing = true;
        private bool isTogglable = true;

        public void OnEnable() {
            isOpen = true;
            Init();

            if (initializing == true) {     // initialize to set pmd references for pmvc, implies skillsPanel is initially open 
                initializing = false;
                gameObject.SetActive(false);
            }
        }

        public void OnDisable() {
            isOpen = false;
        }

        public void Init() {
            List<PartyMember> pms = PartyManager.instance.GetPartyMembers();
            PartyMember pm = PartyManager.instance.GetActivePartyMember();

            for (int i = 0; i < pmDisplays.Length;i++) {
                pmDisplays[i].gameObject.SetActive(false);
            }
    
            for (int i = 0; i < pms.Count; i++) {
                int x = i;          // unity loses track of loop variable, so copying somehow fixes this
                pmDisplays[i].gameObject.SetActive(true);
                pmDisplays[i].InitSkillsDisplay(pms[i].pmvc, pms[i].skillPoints);
            }     

            for (int i = 0; i < skillDisplays.Length; i++) {
                if (pm.skills[i] != null) { // temporary
                    int x = i;
                    skillDisplays[i].Init(i, pm.skills[i], pm.pmvc.skillSprites[i], pm.skills[i].skillColour,  pm.pmvc.pmdSkillsPanel);
                } 
                else {
                    skillDisplays[i].Init();
                }
            }
        }

        public void ToggleSkill(SkillDisplay sd) {
            if (isTogglable == true) {
                if (sd.skillDisplayEnabled == true) {
                    if ((sd.colIndex != 3 && colPoints[sd.colIndex + 1] == 0) || sd.colIndex == 3) {
                        if (PartyManager.instance.DisableSkill(sd.skillIndex)) {
                            sd.skillDisplayEnabled = false;
                            colPoints[sd.colIndex]--;
                            
                            sd.SetColour(sd.skillDisplayEnabled);
                            UpdateSkillsVisible();
                            sd.pmd.UpdateSkillPointsText(PartyManager.instance.GetSkillPoints());
                        }            
                    }
                }
                else if (sd.skillDisplayEnabled == false && PartyManager.instance.GetActivePartyMember().skillPoints > 0) {
                    if ((sd.colIndex != 0 && colPoints[sd.colIndex - 1] != 0) || sd.colIndex == 0) {
                        if (PartyManager.instance.EnableSkill(sd.skillIndex)) {
                            sd.skillDisplayEnabled = true;
                            colPoints[sd.colIndex]++;
                            
                            sd.SetColour(sd.skillDisplayEnabled);
                            UpdateSkillsVisible();
                            sd.pmd.UpdateSkillPointsText(PartyManager.instance.GetSkillPoints());
                        }
                    }     
                }
            }
        }

        public void UpdateSkillsVisible() {
            if (colPoints[0] > 0 && colPoints[1] == 0) {
                skillDisplays[3].Init();
                skillDisplays[4].Init();
                skillDisplays[5].Init();
            }
            if (colPoints[1] > 0 && colPoints[2] == 0) {
                skillDisplays[6].Init();
                skillDisplays[7].Init();
                skillDisplays[8].Init();
            }
            if (colPoints[2] > 0 && colPoints[3] == 0) {
                skillDisplays[9].Init();
                skillDisplays[10].Init();
                skillDisplays[11].Init();
            } 
        }

        public void SetTogglable(bool value) {
            isTogglable = value;
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
    }
}