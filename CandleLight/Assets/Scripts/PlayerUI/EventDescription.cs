/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: June 18, 2019
* 
* The EventDescription gives a description of the current event.
* This can be a prompt for the event, a monster's attack name, and etc.
*
*/

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
        public CanvasGroup textBackgroundCanvas; /// <value> Canvas group for controlling alpha </value>
        
        private float lerpSpeed = 4;        /// Speed at which canvas fades in and out

        /// <summary>
        /// Changes the displayed text
        /// </summary>
        /// <param name="textKey"> Localized key for text to display </param>
        public void SetKey(string textKey) {
            textBackgroundCanvas.alpha = 1;
            eventText.SetKey(textKey);
        }

        /// <summary>
        /// Changes the displayed text and fades in
        /// </summary>
        /// <param name="textKey"> Text to display </param>
        public void SetKeyAndFadeIn(string textKey) {
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
            eventText.SetDamageText(pm.memberName, amount);
        }

        /// <summary>
        /// Stop displaying and remove all text
        /// </summary>
        public void ClearText() {
            textBackgroundCanvas.alpha = 0;
            eventText.Clear();
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
