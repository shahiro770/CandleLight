/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 11, 2019
* 
* The Bar class is used to display health, mana, experience, and level point values of a character through a rectangle.
*
*/

using Localization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerUI {

    [System.Serializable]
    public class Bar : MonoBehaviour {

        /* external component references */
        public Animator barAnimator;
        public Image frontFill;                     /// <value> Image that displays overtop of Bar's background </value>
        public LocalizedText text;                  /// <value> Text to be displayed (if HP or MP can be translated </value>
        
        [field: SerializeField] public float maxAmount { get; set; }       /// <value> Max points the character has </value>
        [field: SerializeField] public float currentAmount { get; set; }   /// <value> Current points the character has </value>
        [field: SerializeField] public float lerpSpeed;                /// <value> Speed at which bar visually changes to fillAmount </value>
        [field: SerializeField] public float fillAmount;               /// <value> Percentage that bar should be filled </value>
        
        /// <summary>
        /// Sets the lerpSpeed at the start
        /// </summary>
        void Start() {
            lerpSpeed = 1f;
        }

        /// <summary>
        /// Initializes the max and current amounts of the bar
        /// </summary>
        /// <param name="maxAmount"> Max amount, must be greater than 0 </param>
        /// <param name="currentAmount"> Current amount </param>
        public void SetMaxAndCurrent(int maxAmount, int currentAmount) {
            this.maxAmount = maxAmount;
            this.currentAmount = currentAmount;
            SetDisplay(true);
        }

        /// <summary>
        /// Initializes the max and current amounts of the bar, as well as the width of the bar
        /// </summary>
        /// <param name="maxAmount"> Max amount, must be greater than 0 </param>
        /// <param name="currentAmount"> Current amount </param>
        /// <param name="vectorSize"> Vector, only width component matters for now </param>
        public void SetMaxAndCurrent(int maxAmount, int currentAmount, Vector2 vectorSize) {
            this.maxAmount = maxAmount;
            this.currentAmount = currentAmount;
            SetWidth(vectorSize.x);
            SetDisplay();
        }

        /// <summary>
        /// Sets the max and current amounts of the EXP bar
        /// </summary>
        /// <param name="maxAmount"></param>
        /// <param name="currentAmount"></param>
        public void SetMaxAndCurrentEXP(int maxAmount, int currentAmount) {
            this.maxAmount = maxAmount;
            this.currentAmount = currentAmount;
            SetDisplayEXP(true);
        }

        /// <summary>
        /// Sets the width of the bar in pixels (may be scaled due to canvas)
        /// </summary>
        /// <param name="width"> Width of the bar, float due to Vector2 </param>
        private void SetWidth(float width) {
            RectTransform rt = GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(width, rt.sizeDelta.y);  // maintain height
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
        /// Sets the current amount of points, and calls to update visually
        /// </summary>
        /// <param name="currentAmount"> Current amount </param>
        public void SetCurrent(int currentAmount) {
            this.currentAmount = currentAmount;
            SetDisplay();
        }

        /// <summary>
        /// Sets the current amount for the EXP bar, and will either update display
        /// with the bar with the amount or wrapping back to 0 if the amount is greater
        /// than the max amount
        /// </summary>
        /// <param name="currentAmount"></param>
        public void SetCurrentEXP(int currentAmount) {
            this.currentAmount = currentAmount;
            if (currentAmount > maxAmount) {
                StartCoroutine(SetDisplayEXPWithOverflow((int)(currentAmount - maxAmount)));
            }
            else {
                SetDisplayEXP();
            }
        }

        /// <summary>
        /// Sets the display value and fill amount of the bar,
        /// For example, a Bar used for HP with 50% HP will have the bar filled halfway with the frontfill image)
        /// </summary>
        private void SetDisplay(bool immediate = false) {
            text.SetText(currentAmount.ToString());
            fillAmount = currentAmount / maxAmount;
            if (immediate) {
                frontFill.fillAmount = fillAmount;
            }
            else {
                StartCoroutine(Fill());
            }
        }

        /// <summary>
        /// Displays the EXP amount with a % sign
        /// </summary>
        /// <param name="immediate"> Flag for if display should fill slowly or immediately jump </param>
        private void SetDisplayEXP(bool immediate = false) {
            fillAmount = currentAmount / maxAmount;
            if (immediate) {
                text.SetText(currentAmount.ToString() + "%");
                frontFill.fillAmount = fillAmount;
            }
            else {
                StartCoroutine(FillEXP());
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
            yield return StartCoroutine(FillEXP());
            currentAmount = 0;
            SetDisplayEXP(true);
            SetCurrentEXP(overflowAmount);
        }

        /// <summary>
        /// Fills the bar up to the current fillAmount
        /// </summary>
        /// <returns></returns>
        private IEnumerator Fill() {
            float timeStartedLerping = Time.time;
            float timeSinceStarted = Time.time - timeStartedLerping;
            float percentageComplete = Time.deltaTime * lerpSpeed;
            float prevFill = frontFill.fillAmount;

            while (frontFill.fillAmount != fillAmount) {
                timeSinceStarted = Time.time - timeStartedLerping;
                percentageComplete = Time.deltaTime * lerpSpeed;

                frontFill.fillAmount = Mathf.Lerp(frontFill.fillAmount, fillAmount, percentageComplete);

                yield return new WaitForEndOfFrame();
            }
        }

        /// <summary>
        /// Fills the bar up to the current fillAmount, and displays the text as the percentage of the bar filled
        /// </summary>
        /// <returns></returns>
        private IEnumerator FillEXP() {
            float timeStartedLerping = Time.time;
            float timeSinceStarted = Time.time - timeStartedLerping;
            float percentageComplete = timeSinceStarted * lerpSpeed;
            float prevFill = frontFill.fillAmount;

            while (frontFill.fillAmount != fillAmount) {
                timeSinceStarted = Time.time - timeStartedLerping;
                percentageComplete = timeSinceStarted * lerpSpeed;
                
                frontFill.fillAmount = Mathf.Lerp(frontFill.fillAmount, fillAmount, percentageComplete);
                text.SetText(((int)(frontFill.fillAmount * 100)).ToString() + "%");

                yield return new WaitForEndOfFrame();
            }
        }
    }
}
