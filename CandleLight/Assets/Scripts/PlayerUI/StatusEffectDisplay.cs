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

        /// <summary>
        /// Constructor to initialize statusEffect display's properties
        /// </summary>
        public void Init(StatusEffect se) {
            this.se = se;
            gameObject.SetActive(false);
            se.SetDisplay(this);
            ColorBlock normalBlock = b.colors; 

            durationText.SetText(se.duration.ToString());

            if (se.name == StatusEffectConstants.BURN) {
                normalBlock.normalColor = new Color32(185, 29, 0, 200);
                normalBlock.highlightedColor = new Color32(185, 29, 0, 255);
                normalBlock.pressedColor = new Color32(185, 29, 0, 255);
                normalBlock.disabledColor = new Color32(185, 29, 0, 150);
                durationText.SetColour(new Color32(185, 29, 0, 255));
            }
            else if (se.name == StatusEffectConstants.POISON) {
                normalBlock.normalColor = new Color32(92, 138, 57, 200);
                normalBlock.highlightedColor = new Color32(92, 138, 57, 255);
                normalBlock.pressedColor = new Color32(92, 138, 57, 255);
                normalBlock.disabledColor = new Color32(92, 138, 57, 150);
                durationText.SetColour(new Color32(92, 138, 57, 255));
            }
            else if (se.name == StatusEffectConstants.TAUNT) {
                normalBlock.normalColor = new Color32(133, 133, 133, 200);
                normalBlock.highlightedColor = new Color32(133, 133, 133, 255);
                normalBlock.pressedColor = new Color32(133, 133, 133, 255);
                normalBlock.disabledColor = new Color32(133, 133, 133, 150);
                durationText.SetColour(new Color32(133, 133, 133, 255));
            }
            else if (se.name == StatusEffectConstants.FREEZE) {
                normalBlock.normalColor = new Color32(0, 152, 220, 200);
                normalBlock.highlightedColor = new Color32(0, 152, 220, 255);
                normalBlock.pressedColor = new Color32(0, 152, 220, 255);
                normalBlock.disabledColor = new Color32(0, 152, 220, 150);
                durationText.SetColour(new Color32(0, 152, 220, 255));
            }
            else if (se.name == StatusEffectConstants.RAGE) {
                normalBlock.normalColor = new Color32(234, 50, 60, 200);
                normalBlock.highlightedColor = new Color32(234, 50, 60, 255);
                normalBlock.pressedColor = new Color32(234, 50, 60, 255);
                normalBlock.disabledColor = new Color32(234, 50, 60, 150);
                durationText.SetColour(new Color32(234, 50, 60, 255));
            }
            else if (se.name == StatusEffectConstants.BLEED) {
                normalBlock.normalColor = new Color32(138, 7, 7, 200);
                normalBlock.highlightedColor = new Color32(138, 7, 7, 255);
                normalBlock.pressedColor = new Color32(138, 7, 7, 255);
                normalBlock.disabledColor = new Color32(138, 7, 7, 150);
                durationText.SetColour(new Color32(138, 7, 7, 255));
            }
            else if (se.name == StatusEffectConstants.WEAKNESS) {
                normalBlock.normalColor = new Color32(98, 36, 97, 200);
                normalBlock.highlightedColor = new Color32(98, 36, 97, 255);
                normalBlock.pressedColor = new Color32(98, 36, 97, 255);
                normalBlock.disabledColor = new Color32(98, 36, 97, 150);
                durationText.SetColour(new Color32(98, 36, 97, 255));
            }

            bts.SetColorBlock("normal", normalBlock);
            bts.SetColor("normal");
            gameObject.SetActive(true);
            SetTooltip();
        }

        public void SetTooltip() {
            RectTransform buttonRect = b.GetComponent<RectTransform>();
            string[] textKeys = new string[2];    
            string[] amounts = new string[2];

            t.SetImageDisplayBackgroundWidth(buttonRect.sizeDelta.x);

            textKeys[0] = se.name + "_description";
            amounts[0] = "";
            if (se.name == StatusEffectConstants.BURN) {
                textKeys[1] = "damage_description";
                amounts[1] = se.value.ToString();
            }
            else if (se.name == StatusEffectConstants.POISON) {
                textKeys[1] = "damage_description";
                amounts[1] = se.value.ToString();
            }
            else if (se.name == StatusEffectConstants.TAUNT) {
                textKeys[1] = "none_label";
                amounts[1] = "";
            }
            else if (se.name == StatusEffectConstants.FREEZE) {
                textKeys[1] = "none_label";
                amounts[1] = "";
            }
            else if (se.name == StatusEffectConstants.RAGE) {
                textKeys[1] = "none_label";
                amounts[1] = "";
            }
            else if (se.name == StatusEffectConstants.BLEED) {
                textKeys[1] = "damage_description";
                amounts[1] = se.value.ToString();
            }
            else if (se.name == StatusEffectConstants.WEAKNESS) {
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
            durationText.SetText(se.duration.ToString());
        }
    }
}
 