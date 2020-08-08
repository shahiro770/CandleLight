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
using ItemConstants = Constants.ItemConstants;
using PanelConstants = Constants.PanelConstants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerUI {

    public class EventDisplay : Panel {

        /* external component references */
        public ActionsPanel actionsPanel;       /// <value> actionsPanel reference </value>
        public SpriteRenderer eventSprite;      /// <value> Image to be displayed </value>
        public CanvasGroup imgCanvas;           /// <value> Background cavnas to control alpha </value> 
        public ItemSlot[] itemSlots = new ItemSlot[6];  /// <value> Item slots references </value> 
        
        public int itemNum = 0;         /// <value> Number of items shown </value>
        
        private float lerpSpeed = 4;    /// <value> Speed at which eventDisplay fades in and out </value>
        
        /// <summary>
        /// Sets image to display a given sprite
        /// </summary>
        /// <param name="spr"> Sprite to be displayed </param>
        public void SetSprite(Sprite spr) {
            eventSprite.sprite = spr;
        }

        /// <summary>
        /// Initializes the item displays, showing items that can be taken
        /// </summary>
        /// <param name="items"></param>
        public void SetItemDisplays(List<Item> items) {
            this.itemNum = items.Count > itemSlots.Length ? itemSlots.Length : items.Count;
            this.numSpareFull = this.itemNum;

            for (int i = 0; i < itemNum; i++) {
                itemSlots[i].PlaceItem(items[i]);
            }
        }

        /// <summary>
        /// Called when player tries to sell an item via shift click
        /// </summary>
        /// <param name="id"></param>
        public void SellItem(ItemDisplay id) {
            for (int i = 0; i < itemSlots.Length ; i++) {
                if (itemSlots[i].currentItemDisplay == null) {
                    if (itemSlots[i].TryShopTransaction(id) == true) {
                        itemSlots[i].PlaceItem(id, true);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Updates the WAX effect value of all items (checks if this is possible internally)
        /// </summary>
        public void UpdateWAXValues() {
            for (int i = 0; i < itemNum; i++) {
                itemSlots[i].UpdateWAXValue();
            }
        }

        /// <summary>
        /// Initializes the item displays, showing items that can be purchased, with extra ones for selling
        /// </summary>
        /// <param name="items"></param>
        public void SetItemDisplaysShop(List<Item> items) {
            this.itemNum = items.Count > itemSlots.Length ? itemSlots.Length : items.Count;
            this.numSpareFull = this.itemNum;

            for (int i = 0; i < itemNum; i++) {
                itemSlots[i].PlaceItemShop(items[i]);
            }
            for (int i = itemNum; i < itemSlots.Length; i++) {
                itemSlots[i].SetVisibleShop(true);
            }
        }

        public void TakeAllItems() {
            for (int i = 0; i < itemNum; i++) {
                itemSlots[i].TakeItem(true);
            }
        }

        /// <summary>
        /// Returns a random item currently displayed by the eventDisplay
        /// </summary>
        /// <returns> An item if possible, null otherwise </returns>
        public Item GetRandomItem() {
            List<ItemDisplay> ids = new List<ItemDisplay>();
            foreach (ItemSlot i in itemSlots) {
                if (i.currentItemDisplay != null) {
                    ids.Add(i.currentItemDisplay);
                }
            }

            if (ids.Count != 0) {
                ItemDisplay id = ids[Random.Range(0, ids.Count)];
                if (id.type == ItemConstants.CONSUMABLE) {
                    return id.displayedConsumable;
                }
                if (id.type == ItemConstants.GEAR) {
                    return id.displayedGear;
                }
                else if (id.type == ItemConstants.CANDLE) {
                    return id.displayedCandle;
                }
                else {  // Special
                    return id.displayedSpecial;
                }
            }
            else {
                return null;
            }
        }

        /// <summary>
        /// Returns true if this eventDisplay is visible
        /// </summary>
        /// <returns></returns>
        public bool IsVisible() {
            return imgCanvas.alpha == 1;
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
                for (int i = 0; i < itemSlots.Length; i++) {
                    if (itemSlots[i].gameObject.activeSelf){
                        itemSlots[i].SetVisible(false);
                    }
                }
                itemNum = 0;
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

        public override string GetPanelName() {
            return PanelConstants.EVENTDISPLAY;    
        }
    }
}