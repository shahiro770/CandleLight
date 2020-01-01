/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: December 25, 2019
* 
* The SkillDisplay class is used to display skills. They can be toggled on and off.
*
*/

using UIEffects;
using Skills;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerUI {
    
    public class SkillDisplay : MonoBehaviour {

        /* external component references*/
        public Button b;
        public ButtonTransitionState bts;
        public SpriteRenderer skillSpriteRenderer;   /// <value> Sprite to be displayed </value>
        public Sprite lockedSkillSprite;

        public Color skillColour;
        public int colIndex;
        public int skillIndex;
        public bool skillDisplayEnabled;

        private Color lockedSkillColour = new Color(255, 255, 255, 255);

        public void Init(int skillIndex, bool skillDisplayEnabled, Sprite skillSprite, Color skillColour) {
            skillSpriteRenderer.sprite = skillSprite;
            this.skillColour = skillColour;
            this.skillIndex = skillIndex;
            this.skillDisplayEnabled = skillDisplayEnabled;

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
            skillDisplayEnabled = false;

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

        public void SetInteractable(bool value) {
            b.interactable = value;
        }
    }
}
