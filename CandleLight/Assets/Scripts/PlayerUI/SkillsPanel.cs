/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: August 1, 2019
* 
* The 
*
*/

using Characters;
using Localization;
using Party;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerUI {

    public class SkillsPanel : Panel {

        public SkillDisplay[] skillDisplays = new SkillDisplay[12];
        public Image skillPointsSquare;
        public LocalizedText skillPointsAmount;

        public int[] colPoints = new int[] { 0, 0, 0, 0 };
        public bool isOpen;             /// <value> Flag for if this panel is open (true if open, false otherwise) </value>

        private bool isTogglable = true;

        public void OnEnable() {
            isOpen = true;
            Init(PartyManager.instance.GetActivePartyMember());
        }

        public void OnDisable() {
            isOpen = false;
        }

        public void Init(PartyMember pm) {
            for (int i = 0; i < skillDisplays.Length; i++) {
                if (pm.skills[i] != null) { // temporary
                    skillDisplays[i].Init(i, pm.skills[i].skillEnabled, pm.pmvc.skillSprites[i], pm.skills[i].skillColour);;
                } 
                else {
                    skillDisplays[i].Init();
                }
            }

            UpdateSkillPointsSquare();
        }

        public void ToggleSkill(SkillDisplay sd) {
            int currentPoints = PartyManager.instance.GetSkillPoints();
            if (isTogglable == true) {
                if (sd.skillDisplayEnabled == true) {
                    if ((sd.colIndex != 3 && colPoints[sd.colIndex + 1] == 0) || sd.colIndex == 3) {
                        if (PartyManager.instance.DisableSkill(sd.skillIndex)) {
                            sd.skillDisplayEnabled = false;
                            colPoints[sd.colIndex]--;
                            
                            sd.SetColour(sd.skillDisplayEnabled);
                            UpdateSkillPointsSquare();
                            UpdateSkillsVisible();
                        }            
                    }
                }
                else if (sd.skillDisplayEnabled == false && PartyManager.instance.GetActivePartyMember().skillPoints > 0) {
                    if ((sd.colIndex != 0 && colPoints[sd.colIndex - 1] != 0) || sd.colIndex == 0) {
                        if (PartyManager.instance.EnableSkill(sd.skillIndex)) {
                            sd.skillDisplayEnabled = true;
                            colPoints[sd.colIndex]++;
                            
                            sd.SetColour(sd.skillDisplayEnabled);
                            UpdateSkillPointsSquare();
                            UpdateSkillsVisible();
                        }
                    }     
                }
            }
        }

        public void UpdateSkillPointsSquare() {
            int currentPoints = PartyManager.instance.GetSkillPoints();
            print(currentPoints);
            skillPointsAmount.SetText(currentPoints.ToString());

            if (currentPoints > 0) {
                skillPointsAmount.SetColour(new Color(255, 255, 255, 255));
                skillPointsSquare.color = new Color(255, 255, 255, 255);
            }
            else {
                skillPointsAmount.SetColour(new Color(255, 255, 255, 128));
                skillPointsSquare.color = new Color(255, 255, 255, 128);
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
    }
}