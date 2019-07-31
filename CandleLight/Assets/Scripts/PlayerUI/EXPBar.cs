/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 11, 2019
* 
* The EXPBar class is used to display experience point values of a character through a rectangle.
*
*/

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

        private int LVL;
        private float maxAmount { get; set; }       /// <value> Max points the character has </value>
        private float currentAmount { get; set; }   /// <value> Current points the character has </value>
        private float lerpSpeed = 0.5f;               /// <value> Speed at which bar visually changes to fillAmount </value>
        private float fillAmount;                   /// <value> Percentage that bar should be filled </value>

        /// <summary>
        /// Initializes the max and current amounts of the bar
        /// </summary>
        /// <param name="maxAmount"> Max amount, must be greater than 0 </param>
        /// <param name="currentAmount"> Current amount </param>
        public void SetEXPBar(int LVL, int maxAmount, int currentAmount) {
            this.maxAmount = maxAmount;
            this.currentAmount = currentAmount;
            this.LVL = LVL;
            SetDisplay(true);
        }

        /// <summary>
        /// Sets the max amount of points, and calls to update visually
        /// </summary>
        /// <param name="maxAmount"> Max amount </param>
        public void SetMax(int maxAmount) {
            this.maxAmount = maxAmount;
            SetDisplay();
        }

        /// <summary>
        /// Sets the current amount for the EXP bar, and will either update display
        /// with the bar with the amount or wrapping back to 0 if the amount is greater
        /// than the max amount
        /// </summary>
        /// <param name="currentAmount"></param>
        public void SetCurrent(int currentAmount) {
            this.currentAmount = currentAmount;
            if (currentAmount > maxAmount) {
                StartCoroutine(SetDisplayEXPWithOverflow((int)(currentAmount - maxAmount)));
            }
            else {
                SetDisplay();
            }
        }

        /// <summary>
        /// Displays the EXP amount with a % sign
        /// </summary>
        /// <param name="immediate"> Flag for if display should fill slowly or immediately jump </param>
        private void SetDisplay(bool immediate = false) {
            fillAmount = currentAmount / maxAmount;
            if (immediate) {
                EXPText.SetText(currentAmount.ToString() + "%");
                frontFill.fillAmount = fillAmount;
            }
            else {
                StartCoroutine(Fill());
            }
        }

        /// <summary>
        /// Displays the EXP bar as being full, then empty, and then reevaluating
        /// with the excess EXP amount
        /// </summary>
        /// <param name="overflowAmount"> Amount left over after reaching max amount </param>
        /// <returns></returns>
        private IEnumerator SetDisplayEXPWithOverflow(int overflowAmount) {
            fillAmount = 1;
            yield return StartCoroutine(Fill());
            currentAmount = 0;
            LVL += 1;
            LVLText.SetText("LVL " + LVL.ToString());
            SetDisplay(true);
            SetCurrent(overflowAmount);
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
                
                frontFill.fillAmount = Mathf.Lerp(frontFill.fillAmount, fillAmount, percentageComplete);
                EXPText.SetText(((int)(frontFill.fillAmount * 100)).ToString() + "%");

                yield return new WaitForEndOfFrame();
            }
        }
    }
}
