/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: June 18, 2019
* 
* The EventDescription gives a description of the current event.
* This can be a prompt for the event, a monster's attack name, and etc.
*
*/

using Attack = Combat.Attack;
using Characters;
using Localization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerUI {

    public class EventDescription : MonoBehaviour {
        
        /* external component references */
        public LocalizedText eventText;     /// <value> Text for event </value>
        public Image textBackground;
        public CanvasGroup textBackgroundCanvas; /// <value> Canvas group for controlling alpha </value>
        
        private Color32 normalColour = new Color32(255, 255, 255, 255);
        private Color32 normalColourHalfAlpha = new Color32(255, 255, 255, 128);
        private Color32 unusableColour = new Color32(196, 36, 48, 255);
        private Color32 unusableColourHalfAlpha = new Color32(196, 36, 48, 128);
        private float lerpSpeed = 4;        /// Speed at which canvas fades in and out
        private string costText = LocalizationManager.instance.GetLocalizedValue("cost_text");
        private string damageText = LocalizationManager.instance.GetLocalizedValue("damage_text");
        private string colour = "normal";

        /// <summary>
        /// Changes the displayed text
        /// </summary>
        /// <param name="textKey"> Localized key for text to display </param>
        public void SetKey(string textKey) {
            if (this.colour != "normal") {
                SetColour("normal");
            }
            textBackgroundCanvas.alpha = 1;
            eventText.SetKey(textKey);
        }

        /// <summary>
        /// Changes the displayed text and fades in
        /// </summary>
        /// <param name="textKey"> Text to display </param>
        public void SetKeyAndFadeIn(string textKey) {
            if (this.colour != "normal") {
                SetColour("normal");
            }
            eventText.SetKey(textKey);
            StartCoroutine(Fade(1));
        }
        
        /// <summary>
        /// Fades the display out
        /// </summary>
        public void FadeOut() {
            StartCoroutine(Fade(0));
        }

        /// <summary>
        /// Changes the displayed text to show how much damage a partyMember took
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="amount"></param>
        public void SetPMDamageText(PartyMember pm, int amount) {
            if (this.colour != "normal") {
                SetColour("normal");
            }
            eventText.SetDamageText(pm.memberName, amount);
        }

        /// <summary>
        /// Changes the displayed text to show the cost and effects of an attack action
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="amount"></param>
        public void SetAttackText(Attack a, bool isUsable) {
            string attackString = a.cost + " " + a.costType + " " + a.damage + " " + damageText;
            eventText.SetText(attackString);

            if (!isUsable) {
                SetColour("unusable");
            }
            else {
                SetColour("normal");
            }
            textBackgroundCanvas.alpha = 1;
        }

        /// <summary>
        /// Stop displaying and remove all text
        /// </summary>
        public void ClearText() {
            textBackgroundCanvas.alpha = 0;
            eventText.Clear();
        }

        public bool HasText() {
            return eventText.HasText();
        }

        public void SetColour(string colour) {
            this.colour = colour;

            if (colour == "unusable") {
                textBackground.color = unusableColourHalfAlpha;
                eventText.SetColour(unusableColour);
            }
            else if (colour == "normal") {
                textBackground.color = normalColourHalfAlpha;
                eventText.SetColour(normalColour);
            }
        }

        /// <summary>
        /// Changes the alpha of the display to the target value
        /// </summary>
        /// <param name="targetAlpha"> Int 0 or 1 </param>
        /// <returns> IEnumerator for smooth animation </returns>
        private IEnumerator Fade(int targetAlpha) {
            float timeStartedLerping = Time.time;
            float timeSinceStarted = Time.time - timeStartedLerping;
            float percentageComplete = timeSinceStarted * lerpSpeed;
            float prevAlpha = textBackgroundCanvas.alpha;

            while (textBackgroundCanvas.alpha != targetAlpha) {
                timeSinceStarted = Time.time - timeStartedLerping;
                percentageComplete = timeSinceStarted * lerpSpeed;

                textBackgroundCanvas.alpha = Mathf.Lerp(prevAlpha, targetAlpha, percentageComplete);

                yield return new WaitForEndOfFrame();
            }
        }
    }
}
