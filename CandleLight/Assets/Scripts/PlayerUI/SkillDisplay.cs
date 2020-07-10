/*
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
        public int colIndex;
    
        public PartyMemberDisplay pmd;
        public Color skillColour;    
        public int skillIndex;
        public bool skillDisplayEnabled;

        private Color lockedSkillColour = new Color(255, 255, 255, 255);
        private Skill displayedSkill;
        private string lockedKeyTitle = "locked_skill_title";
        private string lockedKeySub = "locked_skill_sub";
        private string lockedKeyDes = "locked_skill_des";

        /// <summary>
        /// Initialize a skillDisplay, displaying the skill's icon and preparing the possible colourings
        /// for if the skill is enabled or not.
        /// </summary>
        /// <param name="skillIndex"></param>
        /// <param name="displayedSkill"></param>
        /// <param name="skillSprite"></param>
        /// <param name="skillColour"></param>
        /// <param name="pmd"></param>
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
                normalBlock.normalColor = new Color(skillColour.r, skillColour.g, skillColour.b, 0.75f);
                normalBlock.highlightedColor = skillColour;
                normalBlock.pressedColor = skillColour;
                normalBlock.disabledColor = new Color(skillColour.r, skillColour.g, skillColour.b, 0.33f);
                skillSpriteRenderer.color = skillColour;
                bts.SetColorBlock("normal", normalBlock);
                bts.SetColor("normal");   
            }
            else {
                normalBlock.normalColor = new Color32(255, 255, 255, 100);
                normalBlock.highlightedColor = new Color32(141, 141, 141, 255);
                normalBlock.pressedColor = new Color32(255, 255, 255, 255);
                normalBlock.disabledColor = new Color32(61, 61, 61, 255);
                skillSpriteRenderer.color = new Color32(133, 133, 133, 255);
                bts.SetColorBlock("normal", normalBlock);
                bts.SetColor("normal");
            }
        }

        /// <summary>
        /// Initialize the skill display with a lock on it, showing the skill can not be
        /// enabled even if the player has skill points for it
        /// </summary>
        public void Init() {
            skillSpriteRenderer.sprite = lockedSkillSprite;
            this.skillIndex = -1;           // no skill shown (its locked)
            skillDisplayEnabled = false;
            SetTooltip();

            ColorBlock normalBlock = b.colors; 
            normalBlock.highlightedColor = lockedSkillColour;
            normalBlock.pressedColor = lockedSkillColour;
            normalBlock.disabledColor = new Color32(61, 61, 61, 255);
            bts.SetColorBlock("normal", normalBlock);
            bts.SetColor("normal");
            SetColour(skillDisplayEnabled);
        }

        public void SetColour(bool skillDisplayEnabled) {
            ColorBlock normalBlock = b.colors; 
            this.skillDisplayEnabled = skillDisplayEnabled;

            if (skillDisplayEnabled == true) {
                normalBlock.normalColor = new Color(skillColour.r, skillColour.g, skillColour.b, 0.75f);
                normalBlock.highlightedColor = skillColour;
                normalBlock.pressedColor = skillColour;
                normalBlock.disabledColor = new Color(skillColour.r, skillColour.g, skillColour.b, 0.33f);
                skillSpriteRenderer.color = skillColour;
                bts.SetColorBlock("normal", normalBlock);
                bts.SetColor("normal");   
            }
            else {
                normalBlock.normalColor = new Color32(255, 255, 255, 100);
                normalBlock.highlightedColor = new Color32(141, 141, 141, 255);
                normalBlock.pressedColor = new Color32(255, 255, 255, 255);
                normalBlock.disabledColor = new Color32(61, 61, 61, 255);
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
