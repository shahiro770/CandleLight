/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: July 29, 2019
* 
* The MonsterDisplay class is used to display a killed monster's icon and the number of its type
* that were killed.
*
*/

using Characters;
using Localization;
using PanelConstants = Constants.PanelConstants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerUI {

    public class MonsterDisplay : MonoBehaviour {
        
        /* external component references */
        public Button b;                /// <value> Button to make display clickable for more info </value>
        public CanvasGroup imgCanvas;   /// <value> Alpha controller for image </value>
        public Image buttonBorder;      /// <value> Border image (UITile) </value>
        public Image monsterIcon;       /// <value> Icon of class </value>
        public LocalizedText multiplierText;   /// <value> String displaying number of times the monster was killed </value>
    
        private float lerpSpeed = 2f;   /// <value> Speed at which display fades in and out </value>

        /// <summary>
        /// Initializes display
        /// </summary>
        /// <param name="m"> Monster to display </param>
        /// <param name="quantity"> Number of monster killed </param>
        /// <returns> IEnumerator for smooth animation </returns>
        public IEnumerator Init(Monster m , int quantity) {
            monsterIcon.sprite = Resources.Load<Sprite>("Sprites/MonsterIcons/" + m.monsterArea + "/" + m.monsterSpriteName + "Icon");
            multiplierText.gameObject.SetActive(false); 
            SetInteractable(false);
           
            yield return StartCoroutine(ShrinkFade());
            yield return StartCoroutine(DisplayMultiplierText(quantity));
        }
        
        /// <summary>
        /// Sets navigation to other buttons
        /// </summary>
        /// <param name="direction"> Direction being pressed </param>
        /// <param name="b2"> Other button component </param>
        public void SetNavigation(string direction, Button b2) {
            Navigation n = b.navigation;

            if (direction == "up") {
                n.selectOnUp = b2;
                b.navigation = n;
            }
            else if (direction == "right") {
                n.selectOnRight = b2;
                b.navigation = n;
            }
            else if (direction == "down") {
                n.selectOnDown = b2;
                b.navigation = n;
            }
            else if (direction == "left") {
                n.selectOnLeft = b2;
                b.navigation = n;
            }

            b.navigation = n;
        }

        /// <summary>
        /// Shrinks the display from a set size and fades it in at the same time
        /// </summary>
        /// <returns> IEnumerator for smooth animation</returns>
        private IEnumerator ShrinkFade() {
            float timeStartedLerping = Time.time;
            float timeSinceStarted = Time.time - timeStartedLerping;
            float percentageComplete = timeSinceStarted * lerpSpeed;
            float prevAlpha = 0;
            float targetAlpha = 1;
            float prevBorderWidth = 90;
            float targetBorderWidth = 45;
            float prevIconWidth = 82;
            float targetIconWidth = 41;

            float newBorderWidth;
            float newIconWidth;

            imgCanvas.alpha = prevAlpha;
            buttonBorder.rectTransform.sizeDelta = new Vector2(prevBorderWidth, prevBorderWidth);
            monsterIcon.rectTransform.sizeDelta = new Vector2(prevIconWidth, prevIconWidth);


            while (imgCanvas.alpha != 1) {
                timeSinceStarted = Time.time - timeStartedLerping;
                percentageComplete = timeSinceStarted * lerpSpeed;

                imgCanvas.alpha = Mathf.Lerp(prevAlpha, targetAlpha, percentageComplete);
                newBorderWidth =  Mathf.Lerp(prevBorderWidth, targetBorderWidth, percentageComplete);
                newIconWidth = Mathf.Lerp(prevIconWidth, targetIconWidth, percentageComplete);
                
                buttonBorder.rectTransform.sizeDelta = new Vector2(newBorderWidth, newBorderWidth);
                monsterIcon.rectTransform.sizeDelta = new Vector2(newIconWidth, newIconWidth);

                yield return new WaitForEndOfFrame();
            }
        }

        /// <summary>
        /// Displays text for number of monster that was killed
        /// </summary>
        /// <param name="quantity"></param>
        /// <returns></returns>
        private IEnumerator DisplayMultiplierText(int quantity) {
            multiplierText.gameObject.SetActive(true);

            for (int i = 1; i <= quantity; i++) {
                multiplierText.SetText("x" + i.ToString());
                yield return new WaitForSeconds(0.20f);
            }
        }

        /// <summary>
        /// Sets the interactivity of the action's button, and handles consequences
        /// </summary>
        /// <param name="value"> Enable interactivity on true and disable on false </param>
         public void SetInteractable(bool value) {
            b.interactable = value;
            monsterIcon.raycastTarget = value;
        }
    }
}
