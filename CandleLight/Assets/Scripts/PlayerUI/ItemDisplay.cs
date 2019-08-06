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
using Party;
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
            img.color = new Color(img.color.r, img.color.g, img.color.b, 255);

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

        public void TakeItem() {
            if (displayedItem != null) {
                if (displayedItem.type == "EXP") {
                    PartyManager.instance.GainEXP(displayedItem.EXPAmount);
                }
                if (displayedItem.type == "HP") {
                    PartyManager.instance.AddHP(displayedItem.HPAmount);
                }
                if (displayedItem.type == "MP") {
                    //MPAmount = amount;
                }
                if (displayedItem.type == "WAX") {
                    PartyManager.instance.AddWax(displayedItem.WAXAmount);
                }

                displayedItem = null;
                img.sprite = null;
                img.color = new Color(img.color.r, img.color.g, img.color.b, 0);
            }
        }

        public void SetNavigation() {

        }

        /// <summary>
        /// Makes the eventDisplay visible and interactable
        /// </summary>
        /// <param name="value"></param>
        public void SetVisible(bool value) {
            if (value == true) {
                b.interactable = true;
                StartCoroutine(Fade(1));
                
            }
            else {
                b.interactable = false;
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