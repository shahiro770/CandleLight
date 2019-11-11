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

        private StatusEffect se;     /// <value>  statusEffect being displayed  </value>

        /// <summary>
        /// Constructor to initialize statusEffect display's properties
        /// </summary>
        public void Init(StatusEffect se) {
            this.se = se;
            se.SetDisplay(this);
            ColorBlock normalBlock = b.colors; 

            durationText.SetText(se.duration.ToString());

            if (se.name == StatusEffectConstants.BURN) {
                Debug.Log("initing burn");
                normalBlock.normalColor = new Color32(185, 29, 0, 200);
                normalBlock.highlightedColor = new Color32(185, 29, 0, 255);
                normalBlock.pressedColor = new Color32(185, 29, 0, 255);
                durationText.SetColour(new Color32(185, 29, 0, 255));
            }
            else if (se.name == StatusEffectConstants.POISON) {
                Debug.Log("initing");
                normalBlock.normalColor = new Color32(92, 138, 57, 200);
                normalBlock.highlightedColor = new Color32(92, 138, 57, 255);
                normalBlock.pressedColor = new Color32(92, 138, 57, 255);
                durationText.SetColour(new Color32(92, 138, 57, 255));
            }

            bts.SetColorBlock("normal", normalBlock);
            bts.SetColor("normal");
        }

        /// <summary>
        /// Update the duration text
        /// </summary>
        public void UpdateText() {
            durationText.SetText(se.duration.ToString());
        }
    }
}
 