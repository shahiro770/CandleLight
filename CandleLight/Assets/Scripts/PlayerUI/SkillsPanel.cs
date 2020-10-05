/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: August 1, 2019
* 
* The skillsPanel class is used to show a partyMember's skill tree, and handle
* all interactions that would happen on it.
* IMPORTANT: Whenever adding skills, you must update Init and UpdateSkillsVisible
*
*/

using Characters;
using EventManager = Events.EventManager;
using GameManager = General.GameManager;
using PanelConstants = Constants.PanelConstants;
using Party;
using System.Collections.Generic;
using TutorialConstants = Constants.TutorialConstants;

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

        /// <summary>
        /// Initializes the skillDisplays and pmds if its first initializing
        /// TODO: Reset initializing if a new, non-summon partymember is added (for now,
        /// a summoned partyMember should have 0 as skill points by default in the unity editor (or 
        /// possibly not show up))
        /// </summary>
        public void Init() {
            List<PartyMember> pms = PartyManager.instance.GetPartyMembers();
            PartyMember pm = PartyManager.instance.GetActivePartyMember();
            colPoints = new int[] { 0, 0, 0, 0 };

            // if (initializing == true) {
                for (int i = 0; i < pmDisplays.Length;i++) {
                    pmDisplays[i].gameObject.SetActive(false);
                }    
            // }
    
            for (int i = 0; i < pms.Count; i++) {
                int x = i;          // unity loses track of loop variable, so copying somehow fixes this
                // print(pm.pmvc.pmdSkillsPanel.name);
                // if (pm.pmvc.pmdSkillsPanel == null) {
                    pmDisplays[i].gameObject.SetActive(true);
                    pmDisplays[i].InitSkillsDisplay(pms[i].pmvc, pms[i].skillPoints);
                    
                //    print(pm.pmvc.pmdSkillsPanel.name);
                // }
                //print(pm.pmvc.pmdSkillsPanel);
                //pmDisplays[i].UpdateSkillPointsText(pms[i].skillPoints);
                // if (initializing == true) {
                //     pmDisplays[i].gameObject.SetActive(true);
                //     pmDisplays[i].InitSkillsDisplay(pms[i].pmvc, pms[i].skillPoints);
                // }
                // else {
                //     if (pm.pmvc.pmdSkillsPanel == null) {
                //         pmDisplays[i].gameObject.SetActive(true);
                //         pmDisplays[i].InitSkillsDisplay(pms[i].pmvc, pms[i].skillPoints);
                //     }
                //     pmDisplays[i].UpdateSkillPointsText(pms[i].skillPoints);
                // }
            }  

            for (int i = 0; i < skillDisplays.Length; i++) {
                int x = i;          // dumb hack cause unity forgets for loop ints
                if (pm.skills[i] != null) { // TEMPORARY
                    if (pm.skills[i].skillEnabled == true) {   
                        colPoints[i / 3]++;
                        skillDisplays[i].Init(i, pm.skills[i], pm.pmvc.skillSprites[i], pm.skills[i].skillColour,  pm.pmvc.pmdSkillsPanel);
                    }
                    else if (i < 3 || ( i >= 3 && colPoints[(i / 3) - 1] > 0)) {
                        skillDisplays[i].Init(i, pm.skills[i], pm.pmvc.skillSprites[i], pm.skills[i].skillColour,  pm.pmvc.pmdSkillsPanel);
                    }
                    else {
                        skillDisplays[i].Init();
                    }
                } 
                else {
                    skillDisplays[i].Init(-2);
                }
            }
        }

        public void ToggleSkill(SkillDisplay sd) {
            if (isTogglable == true) {
                if (sd.skillDisplayEnabled == true) {
                    if ((sd.colIndex != 3 && (colPoints[sd.colIndex + 1] == 0 || colPoints[sd.colIndex] > 1)) || sd.colIndex == 3) {
                        if (PartyManager.instance.DisableSkill(sd.skillIndex)) {
                            sd.skillDisplayEnabled = false;
                            colPoints[sd.colIndex]--;
                            
                            sd.SetColour(sd.skillDisplayEnabled);
                            UpdateSkillsVisible();
                            sd.pmd.UpdateSkillPointsText(PartyManager.instance.GetSkillPoints());
                        }            
                    }
                    else if (GameManager.instance.tutorialTriggers[(int)TutorialConstants.tutorialTriggers.isTips] == true 
                    && GameManager.instance.tutorialTriggers[(int)TutorialConstants.tutorialTriggers.firstFailedSkillDisable] == true) {
                        GameManager.instance.tutorialTriggers[(int)TutorialConstants.tutorialTriggers.firstFailedSkillDisable] = false;
                        EventManager.instance.SetTutorialNotification("skills2");     
                    }
                }
                else if (sd.skillDisplayEnabled == false && PartyManager.instance.GetActivePartyMember().skillPoints > 0 && sd.skillIndex != -1) {
                    if ((sd.colIndex != 0 && colPoints[sd.colIndex - 1] != 0) || sd.colIndex == 0) {
                        if (PartyManager.instance.EnableSkill(sd.skillIndex)) {
                            sd.skillDisplayEnabled = true;
                            colPoints[sd.colIndex]++;
                            
                            sd.SetColour(sd.skillDisplayEnabled);
                            UpdateSkillsVisible();
                            sd.pmd.UpdateSkillPointsText(PartyManager.instance.GetSkillPoints());
                        }
                        else if (GameManager.instance.tutorialTriggers[(int)TutorialConstants.tutorialTriggers.isTips] == true 
                        && GameManager.instance.tutorialTriggers[(int)TutorialConstants.tutorialTriggers.firstFailedSkillEnable] == true) {
                            GameManager.instance.tutorialTriggers[(int)TutorialConstants.tutorialTriggers.firstFailedSkillEnable] = false;
                            EventManager.instance.SetTutorialNotification("skills3");    
                        }
                    }
                }
            }
        }

        public void UpdateSkillsVisible() {
            PartyMember pm = PartyManager.instance.GetActivePartyMember();

            if (colPoints[0] > 0 && colPoints[1] == 0) {
                skillDisplays[3].Init(3, pm.skills[3], pm.pmvc.skillSprites[3], pm.skills[3].skillColour,  pm.pmvc.pmdSkillsPanel);
                skillDisplays[4].Init(4, pm.skills[4], pm.pmvc.skillSprites[4], pm.skills[4].skillColour,  pm.pmvc.pmdSkillsPanel);
                skillDisplays[5].Init(5, pm.skills[5], pm.pmvc.skillSprites[5], pm.skills[5].skillColour,  pm.pmvc.pmdSkillsPanel);
            }
            else if (colPoints[0] == 0) {
                skillDisplays[3].Init();
                skillDisplays[4].Init();
                skillDisplays[5].Init();
            }
            if (colPoints[1] > 0 && colPoints[2] == 0) {
                skillDisplays[6].Init(6, pm.skills[6], pm.pmvc.skillSprites[6], pm.skills[6].skillColour,  pm.pmvc.pmdSkillsPanel);
                skillDisplays[7].Init(7, pm.skills[7], pm.pmvc.skillSprites[7], pm.skills[7].skillColour,  pm.pmvc.pmdSkillsPanel);
                skillDisplays[8].Init(8, pm.skills[8], pm.pmvc.skillSprites[8], pm.skills[8].skillColour,  pm.pmvc.pmdSkillsPanel);
            }
            else if (colPoints[1] == 0) {
                skillDisplays[6].Init();
                skillDisplays[7].Init();
                skillDisplays[8].Init();
            }
            if (colPoints[2] > 0 && colPoints[3] == 0) {
                skillDisplays[9].Init(-2);
                skillDisplays[10].Init(-2);
                skillDisplays[11].Init(-2);
            } 
            else if (colPoints[2] == 0) {
                skillDisplays[9].Init(-2);
                skillDisplays[10].Init(-2);
                skillDisplays[11].Init(-2);
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
                if (pmd != pmDisplays[i]) {
                    pmDisplays[i].ShowNormal();
                }
            }

            pmd.ShowActive();
        }

        /// <summary>
        /// Displays the active partyMember with the proper visual colouring,
        /// colouring the background as well for combat clarity
        /// </summary>
        /// <param name="pmd"></param>
        public void DisplayActivePartyMemberCombat(PartyMemberDisplay pmd) {
            for (int i = 0; i < PartyManager.instance.GetNumPartyMembers(); i++) {
                if (pmd != pmDisplays[i]) {
                    pmDisplays[i].ShowNormal();
                }
            }

            pmd.ShowActiveCombat();
        }

        /// <summary>
        /// Sets the skillPanel's buttons interactability
        /// </summary>
        /// <param name="value"></param>
        public void SetInteractable(bool value) {
            for (int i = 0; i < skillDisplays.Length; i++) {
                skillDisplays[i].SetInteractable(value);
            }
            for (int i = 0; i < pmDisplays.Length; i++) {
                pmDisplays[i].SetInteractable(value, value);
            }
        }

        public override string GetPanelName() {
            return PanelConstants.SKILLSPANEL;
        }
    }
}