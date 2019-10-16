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
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerUI {

    public class EventDisplay : MonoBehaviour {

        /* external component references */
        public ActionsPanel actionsPanel;       /// <value> actionsPanel reference</value>
        public SpriteRenderer eventSprite;      /// <value> Image to be displayed </value>
        public CanvasGroup imgCanvas;           /// <value> Background cavnas to control alpha</value> 
        public ItemSlot[] itemSlots = new ItemSlot[3];  /// <value> Item slots references</value> 
        
        private float lerpSpeed = 4;    /// <value> Speed at which eventDisplay fades in and out </value>
        private int itemNum = 0;        /// <value> Number of items shown </value>

        /// <summary>
        /// Sets image to display a given sprite
        /// </summary>
        /// <param name="spr"> Sprite to be displayed </param>
        public void SetSprite(Sprite spr) {
            eventSprite.sprite = spr;
        }

        public void SetItemDisplays(List<Item> items) {
            this.itemNum = items.Count > itemSlots.Length ? itemSlots.Length : items.Count;

            for (int i = 0; i < itemNum; i++) {
                itemSlots[i].PlaceItem(items[i]);
            }

            SetInitialNavigation();
        }

        public void TakeAllItems() {
            for (int i = 0; i < itemNum; i++) {
                itemSlots[i].TakeItem(true);
            }
        }

        /// <summary>
        /// Sets up the initial navigation of the action buttons.
        /// Player may have less than 4 action options available, but the fifth button will almost always
        /// have an option, so navigation between above buttons and the fifth button must be adjusted.
        /// </summary>
        /// <remark> In the future, will have to navigate to other UI panels such as items or information </remark>
        private void SetInitialNavigation() {
            for (int i = 0; i < itemNum; i++) {
                Button b = itemSlots[i].b;
                Navigation n = b.navigation;
                
                if (i > 0) {
                    n.selectOnUp = itemSlots[i - 1].b;
                }
                if (i != itemNum - 1) {
                    n.selectOnDown = itemSlots[i + 1].b;
                }
                else {
                    n.selectOnDown = actionsPanel.GetNavigatableButtonUp();   // actionsPanel's first button will always be active during item taking
                }

                b.navigation = n;
            }

            actionsPanel.SetButtonNavigation(0, "up", itemSlots[itemNum - 1].b);      
            actionsPanel.SetButtonNavigation(1, "up", itemSlots[itemNum - 1].b);             
        }

        /// <summary>
        /// Makes the eventDisplay visible
        /// </summary>
        /// <param name="value"></param>
        public void SetVisible(bool value) {
            if (value == true) {
                imgCanvas.blocksRaycasts = true;
                StartCoroutine(Fade(1));
            }
            else {
                imgCanvas.blocksRaycasts = false;
                for (int i = 0; i < itemNum; i++) {
                    if (itemSlots[i].gameObject.activeSelf){
                        itemSlots[i].SetVisible(false);
                    }
                }
                StartCoroutine(Fade(0));
            }
        }

        public void SetItemsVisible(bool value) {
            for (int i = 0; i < itemNum; i++) {
                if (itemSlots[i].gameObject.activeSelf){
                    itemSlots[i].SetVisible(value);
                }
            }
        }

        /// <summary>
        /// Sets the eventDisplay's position on screen
        /// </summary>
        /// <param name="pos"></param>
        public void SetPosition(Vector3 pos) {
            gameObject.transform.localPosition = pos;
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
            float prevAlpha = imgCanvas.alpha;
            float newAlpha;

            while (imgCanvas.alpha != targetAlpha) {
                timeSinceStarted = Time.time - timeStartedLerping;
                percentageComplete = timeSinceStarted * lerpSpeed;

                newAlpha = Mathf.Lerp(prevAlpha, targetAlpha, percentageComplete);

                imgCanvas.alpha = newAlpha;
                eventSprite.color = new Color(255, 255, 255, newAlpha);

                yield return new WaitForEndOfFrame();
            }
        }
    }
}