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

    public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler {

        /* external component references */
        private EventSystem es;             /// <value> EventSystem reference </value>
        public CanvasGroup imgCanvas;       /// <value> Image alpha </value>
        public Button b;                    /// <value> Button component </value>
        public ButtonTransitionState bts;   /// <value> Button transisition state </value>
        public GameObject itemDisplayPrefab;    /// <value> ItemDisplay prefab for instantiating itemDisplays </value>
        public ItemDisplay currentItemDisplay;  /// <value> Current itemDisplay held, null if empty </value>
        public Sprite defaultSprite = null; /// <value> Item sprite to display when no item is held </value>
        public SpriteRenderer defaultSpriteRenderer;   /// <value> Sprite to be displayed in the event no item is held </value>
        public Image imgBackground;         /// <value> Background for image </value> 
        public Tooltip t;                   /// <value> Tooltip component to display item info </value>
        public string itemSlotType;         /// <value> Item type this slot accepts </value>
        
        private float lerpSpeed = 4;        /// <value> Speed at which item display fades in and out </value>

        /// <summary>
        /// Awake to get eventSystem reference
        /// </summary>
        void Awake() {
            es = EventSystem.current;
        }

        /// <summary>
        /// Initializes the itemSlot with an item, creating an itemDisplay
        /// </summary>
        /// <param name="newItem"> Item object </param>
        public void PlaceItem(Item newItem) {
            if (newItem != null) {  

                SetVisible(true);

                if (newItem.type == null) {
                    defaultSpriteRenderer.sprite = defaultSprite;
                } 
                else {
                    GameObject newItemDisplay = Instantiate(itemDisplayPrefab);
                    currentItemDisplay = newItemDisplay.GetComponent<ItemDisplay>();
                    currentItemDisplay.Init(newItem);

                    newItemDisplay.transform.SetParent(this.transform, false);
                }       

                t.SetImageDisplayBackgroundWidth(imgBackground.rectTransform.sizeDelta.x);
                SetTooltipText();
                
            }
            else {
                SetVisible(true);
            }
        }

        /// <summary>
        /// Initializes the itemSlot with an itemDisplay
        /// </summary>
        /// <param name="newItemDisplay"></param>
        public void PlaceItem(ItemDisplay newItemDisplay) {
            currentItemDisplay = newItemDisplay;
            currentItemDisplay.transform.SetParent(this.transform, false);          // position item placed in the middle of the slot
            currentItemDisplay.transform.localPosition = new Vector3(0f, 0f, 0f);
            
            t.SetImageDisplayBackgroundWidth(imgBackground.rectTransform.sizeDelta.x);
            SetTooltipText();
            t.SetVisible(true);
            
            es.SetSelectedGameObject(this.gameObject);
        }

        /// <summary>
        /// On click function
        /// </summary>
        public void OnClick() {
            if (UIManager.instance.heldItemDisplay != null) {
                if (CheckCorrectItemType(UIManager.instance.heldItemDisplay) == true) {
                    if (currentItemDisplay != null) {
                        ItemDisplay temp = UIManager.instance.heldItemDisplay;

                        TakeItem();

                        temp.EndDragItem();
                        PlaceItem(temp);
                    }
                    else {
                        UIManager.instance.heldItemDisplay.EndDragItem();
                        PlaceItem(UIManager.instance.heldItemDisplay);
                        UIManager.instance.heldItemDisplay = null;
                        UIManager.instance.panelButtonsEnabled = true;
                    }
                }
            }
            else if (currentItemDisplay != null) {
                TakeItem();
            }
       }

        /// <summary>
        /// Takes the item from the item display
        /// </summary>
        /// 
        public void TakeItem(bool direct = false) {
            if (currentItemDisplay != null) {
                if (currentItemDisplay.type == "consumable") {  // consumable items are used on click
                    if (currentItemDisplay.subType == "EXP") {
                        PartyManager.instance.AddEXP(currentItemDisplay.GetAmount());
                    }
                    if (currentItemDisplay.subType == "HP") {
                        PartyManager.instance.AddHPAll(currentItemDisplay.GetAmount());
                    }
                    if (currentItemDisplay.subType == "MP") {
                        PartyManager.instance.AddMPAll(currentItemDisplay.GetAmount());
                    }
                    if (currentItemDisplay.subType == "WAX") {
                        PartyManager.instance.AddWAX(currentItemDisplay.GetAmount());
                    }
                    Destroy(currentItemDisplay.gameObject);
                }
                else {  // non-consumable items must be dragged into UI
                    if (direct == true) {
                        
                    }
                    UIManager.instance.heldItemDisplay = currentItemDisplay;
                    StartCoroutine(currentItemDisplay.StartDragItem());
                    UIManager.instance.panelButtonsEnabled = false;
                }
                
                currentItemDisplay = null;
                defaultSpriteRenderer.sprite = defaultSprite;
                defaultSpriteRenderer.color = new Color(defaultSpriteRenderer.color.r, defaultSpriteRenderer.color.g, defaultSpriteRenderer.color.b, 255);
                t.SetVisible(false);
            }
        }

        /// <summary>
        /// Checks if an itemDisplay of a type can be placed inside this slot
        /// </summary>
        /// <param name="i"> ItemDisplay to check </param>
        /// <returns> True if accepted, false otherwise </returns>
        public bool CheckCorrectItemType(ItemDisplay id) {
            if (itemSlotType == "any") {
                return true;
            }
            else if (id.type == itemSlotType) {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Sets the text displayed in the tooltip
        /// </summary>
        public void SetTooltipText() {
            if (currentItemDisplay != null) {
                string[] itemKeys = currentItemDisplay.GetTooltipKeys();

                if (itemKeys[0] == "consumable") {
                    t.SetKey("title", itemKeys[1] + "_item");
                    t.SetKey( "subtitle", itemKeys[0] + "_item_sub");
                    t.SetAmountText( "description", itemKeys[1] + "_label", currentItemDisplay.GetAmount());
                }
            }
            else {
                t.SetKey("title", itemSlotType + "_item");
                t.SetKey( "subtitle", itemSlotType + "_item_sub");
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
                if (currentItemDisplay != null) {
                    currentItemDisplay.SetVisible(false);
                }
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
                defaultSpriteRenderer.color = new Color(255, 255, 255, newAlpha);

                yield return new WaitForEndOfFrame();
            }
            
            if (targetAlpha == 0) {
                if (currentItemDisplay != null) {
                    Destroy(currentItemDisplay.gameObject);
                    currentItemDisplay = null;
                    gameObject.SetActive(false);
                }
            }
        }

        public void OnPointerEnter(PointerEventData pointerEventData) {
            if (defaultSprite != null || currentItemDisplay != null)  {
                t.SetVisible(true);
            }
        }

        public void OnPointerExit(PointerEventData pointerEventData) {
            if (defaultSprite != null || currentItemDisplay != null) {
                t.SetVisible(false);
            }
        }

        public void OnSelect(BaseEventData baseEventData) {
            if (defaultSprite != null || currentItemDisplay != null) {
                t.SetVisible(true);
            }
        }

        public void OnDeselect(BaseEventData baseEventData) {
            if (defaultSprite != null || currentItemDisplay != null) {
                this.t.SetVisible(false);
            }
        }
    }
}