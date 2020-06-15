/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: November 10, 2019
* 
* The StatusEffectDisplay class displays statusEffects in the UI in a nice square.
*/

using LocalizedText = Localization.LocalizedText;
using StatusEffect = Characters.StatusEffect;
using StatusEffectConstants = Constants.StatusEffectConstants;
using UIEffects;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerUI {

    [System.Serializable]
    public class StatusEffectDisplay : MonoBehaviour {

        /* external component references */
        public LocalizedText durationText;   /// <value> Text displaying the number of turns this SE has left </value>
        public Button b;                    /// <value> Button to hover over to trigger tooltip </value>
        public ButtonTransitionState bts;   /// <value> Button's visual state controller </value>
        public Tooltip t;

        private StatusEffect se;     /// <value>  statusEffect being displayed  </value>
        private string[] textKeys = new string[2];  /// <value> Text keys for status effects </value>    
        private string[] amounts = new string[2];   /// <value> Amounts to the displayed in tooltips </value>    

        /// <summary>
        /// Constructor to initialize statusEffect display's properties
        /// </summary>
        public void Init(StatusEffect se) {
            this.se = se;
            gameObject.SetActive(false);
            se.SetDisplay(this);
            ColorBlock normalBlock = b.colors; 

            UpdateText();

            switch(se.name) {
                case StatusEffectConstants.BURN:
                    normalBlock.normalColor = new Color32(185, 29, 0, 200);
                    normalBlock.highlightedColor = new Color32(185, 29, 0, 255);
                    normalBlock.pressedColor = new Color32(185, 29, 0, 255);
                    normalBlock.disabledColor = new Color32(185, 29, 0, 150);
                    durationText.SetColour(new Color32(185, 29, 0, 255));
                    break;
            
                case StatusEffectConstants.POISON:
                    normalBlock.normalColor = new Color32(92, 138, 57, 200);
                    normalBlock.highlightedColor = new Color32(92, 138, 57, 255);
                    normalBlock.pressedColor = new Color32(92, 138, 57, 255);
                    normalBlock.disabledColor = new Color32(92, 138, 57, 150);
                    durationText.SetColour(new Color32(92, 138, 57, 255));
                    break;

                case StatusEffectConstants.TAUNT:
                    normalBlock.normalColor = new Color32(133, 133, 133, 200);
                    normalBlock.highlightedColor = new Color32(133, 133, 133, 255);
                    normalBlock.pressedColor = new Color32(133, 133, 133, 255);
                    normalBlock.disabledColor = new Color32(133, 133, 133, 150);
                    durationText.SetColour(new Color32(133, 133, 133, 255));
                    break;

                case StatusEffectConstants.FREEZE:
                    normalBlock.normalColor = new Color32(0, 152, 220, 200);
                    normalBlock.highlightedColor = new Color32(0, 152, 220, 255);
                    normalBlock.pressedColor = new Color32(0, 152, 220, 255);
                    normalBlock.disabledColor = new Color32(0, 152, 220, 150);
                    durationText.SetColour(new Color32(0, 152, 220, 255));
                    break;
            
                case StatusEffectConstants.RAGE:
                    normalBlock.normalColor = new Color32(234, 50, 60, 200);
                    normalBlock.highlightedColor = new Color32(234, 50, 60, 255);
                    normalBlock.pressedColor = new Color32(234, 50, 60, 255);
                    normalBlock.disabledColor = new Color32(234, 50, 60, 150);
                    durationText.SetColour(new Color32(234, 50, 60, 255));
                    break;
            
                case StatusEffectConstants.BLEED:
                    normalBlock.normalColor = new Color32(138, 7, 7, 200);
                    normalBlock.highlightedColor = new Color32(138, 7, 7, 255);
                    normalBlock.pressedColor = new Color32(138, 7, 7, 255);
                    normalBlock.disabledColor = new Color32(138, 7, 7, 150);
                    durationText.SetColour(new Color32(138, 7, 7, 255));
                    break;

                case StatusEffectConstants.WEAKNESS:
                    normalBlock.normalColor = new Color32(98, 36, 97, 200);
                    normalBlock.highlightedColor = new Color32(98, 36, 97, 255);
                    normalBlock.pressedColor = new Color32(98, 36, 97, 255);
                    normalBlock.disabledColor = new Color32(98, 36, 97, 150);
                    durationText.SetColour(new Color32(98, 36, 97, 255));
                    break;
            
                case StatusEffectConstants.ADVANTAGE:
                    normalBlock.normalColor = new Color32(230, 126, 34, 200);
                    normalBlock.highlightedColor = new Color32(230, 126, 34, 255);
                    normalBlock.pressedColor = new Color32(230, 126, 34, 255);
                    normalBlock.disabledColor = new Color32(230, 126, 34, 150);
                    durationText.SetColour(new Color32(230, 126, 34, 255));
                    break;
                
                case StatusEffectConstants.ROOT:
                    normalBlock.normalColor = new Color32(93, 44, 40, 200);
                    normalBlock.highlightedColor = new Color32(93, 44, 40, 255);
                    normalBlock.pressedColor = new Color32(93, 44, 40, 255);
                    normalBlock.disabledColor = new Color32(93, 44, 40, 150);
                    durationText.SetColour(new Color32(93, 44, 40, 255));
                    break;
                case StatusEffectConstants.STUN:
                    normalBlock.normalColor = new Color32(255, 205, 2, 200);
                    normalBlock.highlightedColor = new Color32(255, 205, 2, 255);
                    normalBlock.pressedColor = new Color32(255, 205, 2, 255);
                    normalBlock.disabledColor = new Color32(255, 205, 2, 150);
                    durationText.SetColour(new Color32(255, 205, 2, 255));
                    break;
                case StatusEffectConstants.SHOCK:
                    normalBlock.normalColor = new Color32(255, 235, 87, 200);
                    normalBlock.highlightedColor = new Color32(255, 235, 87, 255);
                    normalBlock.pressedColor = new Color32(255, 235, 87, 255);
                    normalBlock.disabledColor = new Color32(255, 235, 87, 150);
                    durationText.SetColour(new Color32(255, 235, 87, 255));
                    break;
                case StatusEffectConstants.REGENERATE:
                    normalBlock.normalColor = new Color32(90, 197, 79, 200);
                    normalBlock.highlightedColor = new Color32(90, 197, 79, 255);
                    normalBlock.pressedColor = new Color32(90, 197, 79, 255);
                    normalBlock.disabledColor = new Color32(90, 197, 79, 255);
                    durationText.SetColour(new Color32(90, 197, 79, 255));
                    break;
                case StatusEffectConstants.GUARD:
                    normalBlock.normalColor = new Color32(155, 66, 28, 200);
                    normalBlock.highlightedColor = new Color32(155, 66, 28, 255);
                    normalBlock.pressedColor = new Color32(155, 66, 28, 255);
                    normalBlock.disabledColor = new Color32(155, 66, 28, 150);
                    durationText.SetColour(new Color32(155, 66, 28, 255));
                    break;
                case StatusEffectConstants.CURE:
                    normalBlock.normalColor = new Color32(90, 197, 79, 200);
                    normalBlock.highlightedColor = new Color32(90, 197, 79, 255);
                    normalBlock.pressedColor = new Color32(90, 197, 79, 255);
                    normalBlock.disabledColor = new Color32(90, 197, 79, 150);
                    durationText.SetColour(new Color32(90, 197, 79, 255));
                    break;
                case StatusEffectConstants.TRAP:
                    normalBlock.normalColor = new Color32(133, 133, 133, 200);
                    normalBlock.highlightedColor = new Color32(133, 133, 133, 255);
                    normalBlock.pressedColor = new Color32(133, 133, 133, 255);
                    normalBlock.disabledColor = new Color32(133, 133, 133, 150);
                    durationText.SetColour(new Color32(133, 133, 133, 255));
                    break;
                case StatusEffectConstants.BOSS:
                case StatusEffectConstants.CHAMPIONHP:
                case StatusEffectConstants.CHAMPIONPATK:
                case StatusEffectConstants.CHAMPIONMATK:
                case StatusEffectConstants.CHAMPIONPDEF:
                case StatusEffectConstants.CHAMPIONMDEF:
                    normalBlock.normalColor = new Color32(255, 255, 255, 200);
                    normalBlock.highlightedColor = new Color32(255, 255, 255, 255);
                    normalBlock.pressedColor = new Color32(255, 255, 255, 255);
                    normalBlock.disabledColor = new Color32(255, 255, 255, 150);
                    durationText.SetColour(new Color32(255, 255, 255, 255));
                    break;
            }

            bts.SetColorBlock("normal", normalBlock);
            bts.SetColor("normal");
            gameObject.SetActive(true);
            SetTooltip();
        }

        public void SetTooltip() {
            RectTransform buttonRect = b.GetComponent<RectTransform>();

            t.SetImageDisplayBackgroundWidth(buttonRect.sizeDelta.x);
            if (se.plus == false) {
                textKeys[0] = se.name + "_description";
            }
            else {
                textKeys[0] = se.name + "_plus_description";
            }
            
            amounts[0] = "";
            if (se.name == StatusEffectConstants.BURN || se.name == StatusEffectConstants.POISON || se.name == StatusEffectConstants.BLEED) {
                textKeys[1] = "damage_description";
                amounts[1] = se.value.ToString();
            }
            else if (se.name == StatusEffectConstants.CHAMPIONHP || se.name == StatusEffectConstants.REGENERATE) {
                textKeys[1] = "HP_regen_description";
                amounts[1] = se.value.ToString();
            }
            else {
                textKeys[1] = "none_label";
                amounts[1] = "";
            }

            t.SetKey("title", se.name + "_title");
            t.SetKey( "subtitle", se.name + "_sub");
            t.SetAmountTextMultiple( "description", textKeys, amounts);
        }

        /// <summary>
        /// Update the duration text
        /// </summary>
        public void UpdateText() {
            if (se.duration > 9) {
                durationText.SetText("?");
            }
            else {
                durationText.SetText(se.duration.ToString());
            }
        }

        /// <summary>
        /// Update the values in the status effect tooltip
        /// </summary>
        public void UpdateValue() {
            if (se.name == StatusEffectConstants.BURN) {
                textKeys[1] = "damage_description";
                amounts[1] = se.value.ToString();
            }
            else if (se.name == StatusEffectConstants.POISON) {
                textKeys[1] = "damage_description";
                amounts[1] = se.value.ToString();
            }
            else if (se.name == StatusEffectConstants.BLEED) {
                textKeys[1] = "damage_description";
                amounts[1] = se.value.ToString();
            }

            t.SetAmountTextMultiple( "description", textKeys, amounts);
        }
    }
}
 