/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: July 4, 2019
* 
* The ItemDisplay class is used to display items. They can be interacted with just like buttons,
* allowing for dragging and dropping of the item inside, or clicking instantly to take the item
*
*/

using Items;
using Party;
using System.Collections;
using UIEffects;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PlayerUI {

    public class ItemDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler {

        /* external component references */
        public CanvasGroup imgCanvas;       /// <value> Image alpha </value>
        public Button b;                    /// <value> Button component </value>
        public ButtonTransitionState bts;   /// <value> Button transisition state </value>
        public Image imgBackground;         /// <value> Background for image </value> 
        public SpriteRenderer itemSprite;   /// <value> Sprite to be displayed </value>
        public Sprite defaultSprite = null; /// <value> Item sprite to display when no item is held </value>
        public Tooltip t;                   /// <value> Tooltip component to display item info </value>
        public string itemDisplayType;
        
        private Item displayedItem;         /// <value> Item display </item>
        private float lerpSpeed = 4;        /// <value> Speed at which item display fades in and out </value>
       
        /// <summary>
        /// Initializes the itemDisplay with a given item
        /// </summary>
        /// <param name="displayedItem"> Item object </param>
        public void Init(Item displayedItem) {
            if (displayedItem != null) {
                this.displayedItem = displayedItem;
                if (displayedItem.type == null) {
                     itemSprite.sprite = defaultSprite;
                }
                else {
                    itemSprite.sprite = displayedItem.itemSprite;
                }

                itemSprite.color = new Color(itemSprite.color.r, itemSprite.color.g, itemSprite.color.b, 255);

                t.SetImageDisplayBackgroundWidth(imgBackground.rectTransform.sizeDelta.x);
                SetTooltipText();
                SetVisible(true);
            }
        }

        /// <summary>
        /// Takes the item from the item display
        /// </summary>
        public void TakeItem() {
            if (displayedItem != null && displayedItem.type != null) {
                if (displayedItem.type == "EXP") {
                    PartyManager.instance.AddEXP(displayedItem.EXPAmount);
                }
                if (displayedItem.type == "HP") {
                    PartyManager.instance.AddHPAll(displayedItem.HPAmount);
                }
                if (displayedItem.type == "MP") {
                    PartyManager.instance.AddMPAll(displayedItem.MPAmount);
                }
                if (displayedItem.type == "WAX") {
                    PartyManager.instance.AddWAX(displayedItem.WAXAmount);
                }

                displayedItem = null;
                itemSprite.sprite = defaultSprite;
                itemSprite.color = new Color(itemSprite.color.r, itemSprite.color.g, itemSprite.color.b, 0);
                t.SetVisible(false);
            }
        }

        /// <summary>
        /// Sets the text displayed in the tooltip
        /// </summary>
        public void SetTooltipText() {
            if (displayedItem != null && displayedItem.type != null) {
                if (displayedItem.isConsumable) {
                    t.SetKey("title", displayedItem.type + "_item");
                    t.SetKey( "subtitle", "consumable_item_sub");
                    t.SetAmountText( "description", displayedItem.type + "_label", displayedItem.GetAmount(displayedItem.type));
                }
            }
            else {
                t.SetKey("title", itemDisplayType + "_item");
                t.SetKey( "subtitle", itemDisplayType + "_item_sub");
                t.SetKey( "description", "none_label");
            }
        }

        /// <summary>
        /// Makes the itemDisplay visible and interactable
        /// </summary>
        /// <param name="value"> true to make visible, false to hide </param>
        public void SetVisible(bool value) {
            if (value == true) {
                b.interactable = true;
                gameObject.SetActive(true);
                StartCoroutine(Fade(1));
            }
            else {
                b.interactable = false;
                StartCoroutine(Fade(0));
            }
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
                itemSprite.color = new Color(255, 255, 255, newAlpha);

                yield return new WaitForEndOfFrame();
            }
            
            if (targetAlpha == 0) {
                gameObject.SetActive(false);
            }
        }

        public void OnPointerEnter(PointerEventData pointerEventData) {
            if (displayedItem != null) {
                t.SetVisible(true);
            }
        }

        //Detect when Cursor leaves the GameObject
        public void OnPointerExit(PointerEventData pointerEventData) {
            if (displayedItem != null) {
                t.SetVisible(false);
            }
        }

        public void OnSelect(BaseEventData baseEventData) {
            if (displayedItem != null) {
                t.SetVisible(true);
            }
        }

        //Detect when Cursor leaves the GameObject
        public void OnDeselect(BaseEventData baseEventData) {
            if (displayedItem != null) {
                t.SetVisible(false);
            }
        }
    }
}