/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: July 4, 2019
* 
* The ItemDisplay class is used to display items. They can be interacted with via ItemSlots that are holding them,
* allowing for them to be dragged and dropped.
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

    public class ItemDisplay : MonoBehaviour {//, IDragHandler, IEndDragHandler {

        /* external component references */
        public CanvasGroup imgCanvas;       /// <value> Image alpha </value>
        public SpriteRenderer itemSprite;   /// <value> Sprite to be displayed </value>
        
        public string type;                 /// <value> Type of item </value>
        public string subType;              /// <value> Subtype of item </value>
        public string className = "any";    /// <value> Name of class item can be used by </value>
        public bool dragging = false;       /// <value> Flag for if item is currently being dragged </value>
        
        private Item displayedItem;         /// <value> Item </value>
        private Gear displayedGear;         /// <value> Item as gear </value>
        private Consumable displayedConsumable; /// <value> Item as consumable </value>
        private float lerpSpeed = 4;        /// <value> Speed at which item display fades in and out </value>
        
        /// <summary>
        /// Initialize the itemDisplay to show a given item
        /// </summary>
        /// <param name="displayedItem"> Item to display </param>
        /// <param name="startSlot"></param>
        public void Init(Item displayedItem) {
            this.displayedItem = displayedItem;
            this.type = displayedItem.type;
            this.subType = displayedItem.subType;

            if (type == "consumable") {
                displayedConsumable = (Consumable)displayedItem;
            }
            else if (type == "gear") {
                displayedGear = (Gear)displayedItem;
                className = displayedItem.className;    // consumables will not be class restricted for now
            }
            itemSprite.sprite = displayedItem.itemSprite;
            itemSprite.color = new Color(itemSprite.color.r, itemSprite.color.g, itemSprite.color.b, 255);
            
            gameObject.SetActive(true);
            SetVisible(true);
        }

        /// <summary>
        /// Returns item as gear
        /// </summary>
        /// <returns></returns>
        public Gear GetGear() {
            return displayedGear;
        }

        /// <summary>
        /// Returns item as consumable
        /// </summary>
        /// <returns></returns>
        public Consumable GetConsumable() {
            return displayedConsumable;
        }

        /// <summary>
        /// Returns all of the item's effects
        /// </summary>
        /// <returns></returns>
        public string[] GetEffects() {
            if (displayedItem.type == "consumable") {
                return displayedConsumable.GetEffects();
            }
            else { //if (displayedItem.type == "gear") {
                return displayedGear.GetEffects();
            }    
        }

        /// <summary>
        /// Returns string array holding basic information about the item displayed
        /// </summary>
        /// <returns></returns>
        public string[] GetTooltipBasicKeys() {
            return displayedItem.GetTooltipBasicKeys();
        }  

        /// <summary>
        /// Returns the tooltip effect keys of the item
        /// </summary>
        /// <returns></returns>
        public string[] GetTooltipEffectKeys() {
            if (displayedItem.type == "consumable") {
                return displayedConsumable.GetTooltipEffectKeys();
            }
            else { //if (displayedItem.type == "gear") {
                return displayedGear.GetTooltipEffectKeys();
            }
        }

        /// <summary>
        /// Returns the values of the item
        /// </summary>
        /// <returns></returns>
        public int[] GetValues() {
            return displayedItem.values;
        }

        /// <summary>
        /// Returns the values of the item
        /// </summary>
        /// <returns></returns>
        public string[] GetValuesAsStrings() {
            return displayedItem.GetAmountsAsStrings();
        }

        /// <summary>
        /// Makes the itemDisplay visible and interactable
        /// </summary>
        /// <param name="value"> true to make visible, false to hide </param>
        public void SetVisible(bool value) {
            if (value == true) { 
                StartCoroutine(Fade(1));
            }
            else {
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
                Destroy(this.gameObject);   // destroy the item after it is no longer visible
            }
        }

        /// <summary>
        /// Start dragging the item, making it follow the cursor after its holding item slot is clicked
        /// </summary>
        /// <returns> IEnumerator to constantly update position </returns>
       public IEnumerator StartDragItem() {
            dragging = true;
        
            while (dragging == true) {
                Vector3 screenPoint = Input.mousePosition;
                screenPoint.z = 5.0f; // distance of the plane from the camera
                transform.position = Camera.main.ScreenToWorldPoint(screenPoint);
                yield return null;
            }
       }

        /// <summary>
        /// Stop dragging the item, end the loop in StartDragItem
        /// </summary>
       public void EndDragItem() {
           dragging = false;
       }
    }
}