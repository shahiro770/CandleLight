/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: July 4, 2019
* 
* The EventDisplay class is used to display an image in the UI for the player to see
* in a specific event.
*
*/

using Items;
using System.Collections;
using UIEffects;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerUI {

    public class ItemDisplay : MonoBehaviour {

        /* external component references */
        public Image img;   /// <value> Image to be displayed </value>
        public CanvasGroup imgCanvas;
        public Button b;
        public ButtonTransitionState bts;
        
        private float lerpSpeed = 4; 
        private Item displayedItem;

        /// <summary>
        /// 
        /// </summary>
        public void Init(Item displayedItem) {
            this.displayedItem = displayedItem;

            if (displayedItem != null) {
                if (displayedItem.type == "EXP") {

                }
                if (displayedItem.type == "HP") {
                    //HPAmount = amount;
                }
                if (displayedItem.type == "MP") {
                    //MPAmount = amount;
                }
                if (displayedItem.type == "WAX") {

                }

                img.sprite = displayedItem.itemSprite;
            }
            SetVisible(true);
        }

        /// <summary>
        /// Sets image to display a given sprite
        /// </summary>
        /// <param name="spr"> Sprite to be displayed </param>
        public void SetImage(Sprite spr) {
            img.sprite = spr;
        }

        /// <summary>
        /// Makes the eventDisplay visible
        /// </summary>
        /// <param name="value"></param>
        public void SetVisible(bool value) {
            if (value == true) {
                StartCoroutine(Fade(1));
            }
            else {
                StartCoroutine(Fade(0));
            }
        }

        private IEnumerator Fade(int targetAlpha) {
            float timeStartedLerping = Time.time;
            float timeSinceStarted = Time.time - timeStartedLerping;
            float percentageComplete = timeSinceStarted * lerpSpeed;
            float prevAlpha = imgCanvas.alpha;

            while (imgCanvas.alpha != targetAlpha) {
                timeSinceStarted = Time.time - timeStartedLerping;
                percentageComplete = timeSinceStarted * lerpSpeed;

                imgCanvas.alpha = Mathf.Lerp(prevAlpha, targetAlpha, percentageComplete);

                yield return new WaitForEndOfFrame();
            }
        }
    }
}