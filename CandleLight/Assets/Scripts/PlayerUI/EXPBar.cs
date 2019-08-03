/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 11, 2019
* 
* The EXPBar class is used to display experience point values of a character through a rectangle.
*
*/

using Characters;
using Localization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerUI {

    public class EXPBar : MonoBehaviour {

        /* external component references */
        public Image frontFill;                     /// <value> Image that displays overtop of Bar's background </value>
        public LocalizedText EXPText;               /// <value> Text to be displayed (if HP or MP can be translated </value>
        public LocalizedText LVLText;

        private PartyMember pm;
        private float maxAmount;        /// <value> Max points the character has </value>
        private float currentAmount;    /// <value> Current points the character has </value>
        private float lerpSpeed = 1f;   /// <value> Speed at which bar visually changes to fillAmount </value>
        private float fillAmount;       /// <value> Percentage that bar should be filled </value>
        
        /// <summary>
        /// Initializes the max and current amounts of the bar
        /// </summary>
        /// <param name="maxAmount"> Max amount, must be greater than 0 </param>
        /// <param name="currentAmount"> Current amount </param>
        public void SetEXPBar(PartyMember pm, int maxAmount, int currentAmount) {
            this.pm = pm;
            SetMaxAndCurrentImmediate(maxAmount, currentAmount);
        }

        public void SetMaxAndCurrent(int maxAmount, int currentAmount) {
            this.maxAmount = maxAmount;
            this.currentAmount = currentAmount;

            SetDisplay();
        }

        public void SetMaxAndCurrentImmediate(int maxAmount, int currentAmount) {
            this.maxAmount = maxAmount;
            this.currentAmount = currentAmount;

            SetDisplay(true);
        }

        /// <summary>
        /// Sets the current amount for the EXP bar, and will either update display
        /// with the bar with the amount or wrapping back to 0 if the amount is greater
        /// than the max amount
        /// </summary>
        /// <param name="currentAmount"></param>
        public IEnumerator SetCurrent(int currentAmount) {
            this.currentAmount = currentAmount;
            SetDisplay();
            while (!DoneFilling()) {
                yield return new WaitForEndOfFrame();
            }
        }

        public void SetCurrentImmediate(int currentAmount) {
            this.currentAmount = currentAmount;
            SetDisplay(true);
        }

        /// <summary>
        /// Returns if the EXPBar's display is filled to the correct amount
        /// </summary>
        /// <returns> True if done, false otherwise </returns>
        public bool DoneFilling() {
            return fillAmount == frontFill.fillAmount;
        }

        /// <summary>
        /// Displays the EXP amount with a % sign
        /// </summary>
        /// <param name="immediate"> Flag for if display should fill slowly or immediately jump </param>
        private void SetDisplay(bool immediate = false) {
            fillAmount = currentAmount / maxAmount;
            if (immediate) {
                frontFill.fillAmount = fillAmount;
                EXPText.SetText(((int)(frontFill.fillAmount * 100)).ToString() + "%");
            }
            else {
                StartCoroutine(Fill());
            }
        }

        /// <summary>
        /// Fills the bar up to the current fillAmount, and displays the text as the percentage of the bar filled
        /// </summary>
        /// <returns></returns>
        private IEnumerator Fill() {
            float timeStartedLerping = Time.time;
            float timeSinceStarted = Time.time - timeStartedLerping;
            float percentageComplete = timeSinceStarted * lerpSpeed;
            float prevFill = frontFill.fillAmount;

            while (frontFill.fillAmount != fillAmount) {
                timeSinceStarted = Time.time - timeStartedLerping;
                percentageComplete = timeSinceStarted * lerpSpeed;
                
                frontFill.fillAmount = Mathf.Lerp(prevFill, fillAmount, percentageComplete);
                EXPText.SetText(((int)(frontFill.fillAmount * 100)).ToString() + "%");
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
