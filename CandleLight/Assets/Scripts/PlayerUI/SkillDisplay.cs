﻿/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: December 25, 2019
* 
* The SkillDisplay class is used to display skills. They can be toggled on and off.
*
*/

using Skills;
using UIEffects;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerUI {
    
    public class SkillDisplay : MonoBehaviour {

        /* external component references*/
        public Button b;
        public ButtonTransitionState bts;
        public SpriteRenderer skillSpriteRenderer;   /// <value> Sprite to be displayed </value>
        public Sprite lockedSkillSprite;
        public Tooltip t;
    
        public PartyMemberDisplay pmd;
        public Color skillColour;
        public int colIndex;
        public int skillIndex;
        public bool skillDisplayEnabled;

        private Color lockedSkillColour = new Color(255, 255, 255, 255);
        private Skill displayedSkill;
        private string lockedKeyTitle = "locked_skill_title";
        private string lockedKeySub = "locked_skill_sub";
        private string lockedKeyDes = "locked_skill_des";

        public void Init(int skillIndex, Skill displayedSkill, Sprite skillSprite, Color skillColour, PartyMemberDisplay pmd) {
            skillSpriteRenderer.sprite = skillSprite;
            this.skillColour = skillColour;
            this.skillIndex = skillIndex;
            this.skillDisplayEnabled = displayedSkill.skillEnabled;
            this.displayedSkill = displayedSkill;
            this.pmd = pmd;
            SetTooltip();

            ColorBlock normalBlock = b.colors; 

            if (skillDisplayEnabled == true) {
                normalBlock.normalColor = new Color(skillColour.r, skillColour.g, skillColour.b, 0.5f);
                normalBlock.highlightedColor = skillColour;
                normalBlock.pressedColor = skillColour;
                skillSpriteRenderer.color = skillColour;
                bts.SetColorBlock("normal", normalBlock);
                bts.SetColor("normal");   
            }
            else {
                normalBlock.normalColor = new Color32(141, 141, 141, 255);
                normalBlock.highlightedColor = new Color32(255, 255, 255, 200);
                normalBlock.pressedColor = new Color32(255, 255, 255, 255);
                skillSpriteRenderer.color = new Color32(133, 133, 133, 255);
                bts.SetColorBlock("normal", normalBlock);
                bts.SetColor("normal");
            }
        }

        public void Init() {
            skillSpriteRenderer.sprite = lockedSkillSprite;
            this.skillIndex = -1;           // no skill shown
            skillDisplayEnabled = false;
            SetTooltip();

            ColorBlock normalBlock = b.colors; 
            normalBlock.highlightedColor = lockedSkillColour;
            normalBlock.pressedColor = lockedSkillColour;
            bts.SetColorBlock("normal", normalBlock);
            bts.SetColor("normal");
        }

        public void SetColour(bool skillDisplayEnabled) {
            ColorBlock normalBlock = b.colors; 
            this.skillDisplayEnabled = skillDisplayEnabled;

            if (skillDisplayEnabled == true) {
                normalBlock.normalColor = new Color(skillColour.r, skillColour.g, skillColour.b, 0.75f);
                normalBlock.highlightedColor = skillColour;
                normalBlock.pressedColor = skillColour;
                skillSpriteRenderer.color = skillColour;
                bts.SetColorBlock("normal", normalBlock);
                bts.SetColor("normal");   
            }
            else {
                normalBlock.normalColor = new Color32(141, 141, 141, 150);
                normalBlock.highlightedColor = new Color32(255, 255, 255, 200);
                normalBlock.pressedColor = new Color32(255, 255, 255, 255);
                skillSpriteRenderer.color = new Color32(133, 133, 133, 255);
                bts.SetColorBlock("normal", normalBlock);
                bts.SetColor("normal");
            }
        }

        /// <summary>
        /// Sets the text displayed in the tooltip
        /// </summary>
        public void SetTooltip() {
            RectTransform buttonRect = b.GetComponent<RectTransform>();
            t.SetImageDisplayBackgroundWidth(buttonRect.sizeDelta.x);

            if (skillIndex != -1) {
                t.SetKey("title", displayedSkill.titleKey);
                t.SetKey("subtitle", displayedSkill.subKey);
                t.SetKey("description", displayedSkill.desKey);
            }
            else {
                t.SetKey("title", lockedKeyTitle);
                t.SetKey("subtitle", lockedKeySub);
                t.SetKey("description", lockedKeyDes);
            }
        }

        public void SetInteractable(bool value) {
            b.interactable = value;
        }
    }
}
