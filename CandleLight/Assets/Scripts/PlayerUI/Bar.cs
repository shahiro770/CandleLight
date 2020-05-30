/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 11, 2019
* 
* The Bar class is used to display health and mana point values of a character through a rectangle.
*
*/

using Localization;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerUI {

    [System.Serializable]
    public class Bar : MonoBehaviour {

        /* external component references */
        public Animator barAnimator;
        public Image frontFill;                     /// <value> Image that displays overtop of Bar's background </value>
        public LocalizedText text;                  /// <value> Text to be displayed (if HP or MP can be translated </value>
        
        [field: SerializeField] private float maxAmount { get; set; }       /// <value> Max points the character has </value>
        [field: SerializeField] private float currentAmount { get; set; }   /// <value> Current points the character has </value>
        [field: SerializeField] private float lerpSpeed;                    /// <value> Speed at which bar visually changes to fillAmount </value>
        [field: SerializeField] private float lerpSpeedFast = 7f;           /// <value> Fast speed to lerp with </value>
        [field: SerializeField] private float lerpSpeedSlow = 3.5f;         /// <value> Slow speed to lerp with </value>
        [field: SerializeField] private float fillAmount;                   /// <value> Percentage that bar should be filled </value>

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
            SetDisplay(true);
        }

        /// <summary>
        /// Initializes the max and current amounts of the bar, and displays the current / max as the amount text
        /// </summary>
        /// <param name="maxAmount"> Max amount, must be greater than 0 </param>
        /// <param name="currentAmount"> Current amount </param>
        /// <param name="vectorSize"> Vector, only width component matters for now </param>
        public void SetMaxAndCurrentDisplayCurrentOverMax(int maxAmount, int currentAmount) {
            this.maxAmount = maxAmount;
            this.currentAmount = currentAmount;
            SetDisplayCurrentOverMax(true);
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
        /// Sets the current amount of points, and calls to update visually
        /// </summary>
        /// <param name="currentAmount"> Current amount </param>
        public void SetCurrent(int currentAmount) {
            this.currentAmount = currentAmount;
            SetDisplay();
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
        /// Displays the HP bar with the current/max as the amount text
        /// </summary>
        /// <param name="immediate"></param>
        private void SetDisplayCurrentOverMax(bool immediate = false) {
            text.SetText(currentAmount.ToString() + "/" + maxAmount.ToString());
            fillAmount = currentAmount / maxAmount;
            if (immediate) {
                frontFill.fillAmount = fillAmount;
            }
            else {
                StartCoroutine(Fill());
            }
        }

        /// <summary>
        /// Fills the bar up to the current fillAmount
        /// </summary>
        /// <returns></returns>
        private IEnumerator Fill() {
            if (Mathf.Abs(fillAmount - frontFill.fillAmount) * 100 > 75) {
                lerpSpeed = lerpSpeedFast;
            }
            else {
                lerpSpeed = lerpSpeedSlow;
            }

            float timeStartedLerping = Time.time;
            float timeSinceStarted = Time.time - timeStartedLerping;
            float percentageComplete = Time.deltaTime * lerpSpeed;
            
            bool lerpDirection = fillAmount >= frontFill.fillAmount;

            if (frontFill.fillAmount != fillAmount) {
                while (timeSinceStarted < 3) {  // lerping is approaching infinity so stop after 3 seconds
                    timeSinceStarted = Time.time - timeStartedLerping;
                    percentageComplete = Time.deltaTime * lerpSpeed;

                    frontFill.fillAmount = Mathf.Lerp(frontFill.fillAmount, fillAmount, percentageComplete);

                    yield return new WaitForEndOfFrame();
                }
            }
        }
    }
}
