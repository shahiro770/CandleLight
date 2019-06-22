/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 11, 2019
* 
* The Bar class is used to display health or mana point values of a character through a rectangle.
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

        public Animator barAnimator;
        public Image frontFill;                     /// <value> Image that displays overtop of Bar's background </value>
        public LocalizedText text;                  /// <value> Text to be displayed (if HP or MP can be translated </value>
        [field: SerializeField] public float maxAmount { get; set; }       /// <value> Max points the character has </value>
        [field: SerializeField] public float currentAmount { get; set; }   /// <value> Current points the character has </value>
        [field: SerializeField] public float lerpSpeed = 2;
        [field: SerializeField] public float fillAmount = 100;

        /// <summary>
        /// Update to slowly making bar's fill amount match its changed value
        /// </summary>
        public void Update() {
            if (frontFill.fillAmount != fillAmount) {
                frontFill.fillAmount = Mathf.Lerp(frontFill.fillAmount, fillAmount, Time.deltaTime * lerpSpeed);
            }
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
        /// <param name="currentAmount"> Current amount </param>
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
        /// Sets the display value and fill amount of the bar,
        /// For example, a Bar used for HP with 50% HP will have the bar filled halfway with the frontfill image)
        /// </summary>
        private void SetDisplay(bool immediate = false) {
            text.Append(currentAmount.ToString());
            fillAmount = currentAmount / maxAmount;
            if (immediate) {
                frontFill.fillAmount = currentAmount / maxAmount;
            }
        }


        void DoStuff(Object something) {
            // Do stuff
        }

        void DoSameStuffForADifferentReason(Object something) {
            DoStuff(something);
        }
    }
}
