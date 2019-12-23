/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: July 4, 2019
* 
* The ItemDisplay class is used to display items. They can be interacted with just like buttons,
* allowing for dragging and dropping of the item inside, or clicking instantly to take the item
*
*/

using EventManager = Events.EventManager;
using Items;
using Party;
using StatusEffectConstants = Constants.StatusEffectConstants;
using System.Collections;
using UIEffects;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PlayerUI {

    public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler {

        /* external component references */
        public Panel parentPanel;           /// <value> Reference to the panel that contains this itemSlot </value>
        public CanvasGroup imgCanvas;       /// <value> Image alpha </value>
        public Button b;                    /// <value> Button component </value>
        public ButtonTransitionState bts;   /// <value> Button transisition state </value>
        public GameObject itemDisplayPrefab;    /// <value> ItemDisplay prefab for instantiating itemDisplays </value>
        public ItemDisplay currentItemDisplay;  /// <value> Current itemDisplay held, null if empty </value>
        public Sprite defaultSprite = null; /// <value> Item sprite to display when no item is held </value>
        public SpriteRenderer defaultSpriteRenderer;   /// <value> Sprite to be displayed in the event no item is held </value>
        public Image imgBackground;         /// <value> Background for image this tooltip is for </value> 
        public Tooltip t;                   /// <value> Tooltip component to display item info </value>
        public string itemSlotType;         /// <value> Item type this slot accepts </value>
        public string itemSlotSubType;      /// <value> Item subType this slot accepts </value>
        public bool isTakeable = true;      /// <value> Flag for if item is draggable </value>
        
        private EventSystem es;             /// <value> EventSystem reference </value>
        private float lerpSpeed = 4;        /// <value> Speed at which item display fades in and out </value>

        /// <summary>
        /// Awake to get eventSystem reference
        /// </summary>
        public void Awake() {
            es = EventSystem.current;
            defaultSpriteRenderer.sprite = defaultSprite;
        }

        /// <summary>
        /// Initializes the itemSlot with an item, creating an itemDisplay
        /// </summary>
        /// <param name="newItem"> Item object </param>
        public void PlaceItem(Item newItem) {
            if (newItem != null) {  
                SetVisible(true);
            
                if (newItem.type == null) { // show the default sprite if nothing is in the slot (assuming there is a default sprite)
                    defaultSpriteRenderer.color = new Color(defaultSpriteRenderer.color.r, defaultSpriteRenderer.color.g, defaultSpriteRenderer.color.b, 255); //not this one
                } 
                else {
                    GameObject newItemDisplay = Instantiate(itemDisplayPrefab);
                    currentItemDisplay = newItemDisplay.GetComponent<ItemDisplay>();
                    currentItemDisplay.Init(newItem);

                    newItemDisplay.transform.SetParent(this.transform, false);
                    // hide the defaultSprite if it shows
                    defaultSpriteRenderer.color = new Color(defaultSpriteRenderer.color.r, defaultSpriteRenderer.color.g, defaultSpriteRenderer.color.b, 0);
                }       

                t.SetImageDisplayBackgroundWidth(imgBackground.rectTransform.sizeDelta.x);
                SetTooltipText();     
            }
            else {
                SetVisible(true);
            }
        }

        /// <summary>
        /// Initializes the i
        /// </summary>
        /// <param name="newItem"></param>
        public void PlaceItemInstant(Item newItem) {
            if (newItem != null) {  
            
                if (newItem.type == null) {
                    print(gameObject.name);
                    defaultSpriteRenderer.color = new Color(defaultSpriteRenderer.color.r, defaultSpriteRenderer.color.g, defaultSpriteRenderer.color.b, 255);
                } 
                else {
                    GameObject newItemDisplay = Instantiate(itemDisplayPrefab);
                    currentItemDisplay = newItemDisplay.GetComponent<ItemDisplay>();
                    currentItemDisplay.Init(newItem);

                    newItemDisplay.transform.SetParent(this.transform, false);
                    // hide the defaultSprite if it shows
                    defaultSpriteRenderer.color = new Color(defaultSpriteRenderer.color.r, defaultSpriteRenderer.color.g, defaultSpriteRenderer.color.b, 0);
                }       

                t.SetImageDisplayBackgroundWidth(imgBackground.rectTransform.sizeDelta.x);
                SetTooltipText();
            }
            else {
                defaultSpriteRenderer.color = new Color(defaultSpriteRenderer.color.r, defaultSpriteRenderer.color.g, defaultSpriteRenderer.color.b, 255);
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
            
            if (itemSlotSubType != "any") {    // bad way to determine if its a partyMember's equipment slot
                defaultSpriteRenderer.color = new Color(defaultSpriteRenderer.color.r, defaultSpriteRenderer.color.g, defaultSpriteRenderer.color.b, 0);
                if (itemSlotType == "gear") {
                    PartyManager.instance.EquipGear(newItemDisplay, itemSlotSubType);
                }    
            }

            if (es.currentSelectedGameObject == this.gameObject) {
                es.SetSelectedGameObject(this.gameObject);
                t.SetVisible(true);
            }
        }

        /// <summary>
        /// Destroys the currently displayed item
        /// </summary>
        public void ClearItem() {
            if (currentItemDisplay != null) {
                Destroy(currentItemDisplay.gameObject);
                currentItemDisplay = null;
            }
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
            if (currentItemDisplay != null && isTakeable == true) {
                bool itemTaken = false;

                if (currentItemDisplay.type == "consumable") {  // consumable items are used on click
                    string[] effects = currentItemDisplay.GetEffects();
                    int[] amounts = currentItemDisplay.GetValues();
                    for (int i = 0; i < effects.Length; i++) {
                        if (effects[i] == "EXP") {
                            PartyManager.instance.AddEXP(amounts[i]);
                        }
                        if (effects[i] == "HP") {
                            PartyManager.instance.AddHPAll(amounts[i]);
                        }
                        if (effects[i] == "MP") {
                            PartyManager.instance.AddMPAll(amounts[i]);
                        }
                        if (effects[i] == "WAX") {
                            PartyManager.instance.AddWAX(amounts[i]);
                        }
                        if (effects[i] == StatusEffectConstants.POISON) {
                            PartyManager.instance.AddSE(effects[i], amounts[i]);
                        }
                    }
                    itemTaken = true;
                    Destroy(currentItemDisplay.gameObject);
                }
                else {  // non-consumable items must be dragged into UI
                    if (direct == true || Input.GetKeyDown(KeyCode.Return)) {
                        Panel targetPanel = EventManager.instance.GetTargetPanel(currentItemDisplay.type);

                        if (currentItemDisplay.type == "gear") {
                            GearPanel gearPanel = (GearPanel)targetPanel;
                            if (gearPanel.PlaceItem(currentItemDisplay)) {
                                itemTaken = true;
                            }
                        }
                    }
                    else {
                        UIManager.instance.heldItemDisplay = currentItemDisplay;
                        StartCoroutine(currentItemDisplay.StartDragItem());
                        UIManager.instance.panelButtonsEnabled = false;
                        itemTaken = true;
                    }
                }
                
                if (itemTaken) {    // if item is taken, update the itemSlot
                    currentItemDisplay = null;
                    defaultSpriteRenderer.color = new Color(defaultSpriteRenderer.color.r, defaultSpriteRenderer.color.g, defaultSpriteRenderer.color.b, 255);
                    t.SetVisible(false);
                    SetTooltipText();

                    if (parentPanel != null) {
                        if (itemSlotType == "gear" && itemSlotSubType == "any") {   // gearPanel updates to have more free spare itemSlots
                            GearPanel gearPanel = (GearPanel)parentPanel;
                            gearPanel.TakeItem();
                        }
                        else if (itemSlotSubType != "any") { // item was unequipped from partyMember
                            PartyManager.instance.UnequipGear(itemSlotSubType);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sets the itemSlot to allow its item to be taken
        /// </summary>
        /// <param name="value"> True if item can be taken, false otherwise </param>
        public void SetTakeable(bool value) {
            isTakeable = value;
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
            else if (id.type == itemSlotType && id.subType == itemSlotSubType) {
                if (id.className != "any") {
                    if (id.className == PartyManager.instance.GetActivePartyMember().className) {
                        return true;
                    }
                }
                else {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Hides the item being displayed, so that if partyMembers switch,
        /// instead of itemDisplays needing to be re-instantiated, the itemDisplayed can be hidden and displayed
        /// </summary>
        public void HideItem() {
            currentItemDisplay.gameObject.SetActive(false);
            currentItemDisplay = null;
            SetTooltipText();

            defaultSpriteRenderer.color = new Color(defaultSpriteRenderer.color.r, defaultSpriteRenderer.color.g, defaultSpriteRenderer.color.b, 255);
        }

        /// <summary>
        /// Shows the item being displayed, so that if partyMembers switch,
        /// instead of itemDisplays needing to be re-instantiated, the itemDisplayed can be hidden and displayed
        /// </summary>
        /// <remark>
        /// Assumes that there is already an item in the slot
        /// </remark>
        /// <param name="id"> ItemDisplay to show </param>
        public void ShowItem(ItemDisplay id) {
            currentItemDisplay = id;
            currentItemDisplay.gameObject.SetActive(true);
            SetTooltipText();

            defaultSpriteRenderer.color = new Color(defaultSpriteRenderer.color.r, defaultSpriteRenderer.color.g, defaultSpriteRenderer.color.b, 0);
        }

        /// <summary>
        /// Sets the text displayed in the tooltip
        /// </summary>
        public void SetTooltipText() {
            if (currentItemDisplay != null) {
                string[] basicKeys = currentItemDisplay.GetTooltipBasicKeys();

                if (basicKeys[1] == "consumable") {
                    t.SetKey("title", basicKeys[0] + "_item");
                    t.SetKey( "subtitle", basicKeys[1] + "_item_sub");
                    t.SetAmountTextMultiple( "description", currentItemDisplay.GetTooltipEffectKeys(), currentItemDisplay.GetValuesAsStrings());
                }
                else if (basicKeys[1] == "gear") {
                    t.SetKey("title", basicKeys[0] + "_item");
                    if (currentItemDisplay.className == "any") {
                        t.SetKey( "subtitle", basicKeys[2] + "_item_sub");
                    }
                    else {
                        t.SetKeyMultiple( "subtitle", new string[2] {currentItemDisplay.className + "_label", basicKeys[2] + "_item_sub"});
                    }
                    
                    t.SetAmountTextMultiple( "description", currentItemDisplay.GetTooltipEffectKeys(), currentItemDisplay.GetValuesAsStrings());
                }
            }
            else {  // if there is no item held
                t.SetKey("title", itemSlotSubType + "_item");
                t.SetKey( "subtitle", itemSlotSubType + "_item_sub_default");
                t.SetKey( "description", "none_label");
            }
        }

        /// <summary>
        /// Colours the default sprite and button outline
        /// </summary>
        /// <param name="newColour"> Color object </param>
        /// <remark>
        /// Used to colour equipment slots to make each character's gear visibly distinct
        /// </remark>
        public void SetColour(Color newColour) {
            imgBackground.color = newColour;
            defaultSpriteRenderer.color = new Color(newColour.r, newColour.g, newColour.b, defaultSpriteRenderer.color.a);
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
        /// Sets if the itemSlot can be interacted with (tooltip can only show if itemSlot can be interacted with)
        /// </summary>
        /// <param name="value"></param>
        public void SetInteractable(bool value) {
            b.interactable = value;

            if (value == false) {
                t.SetVisible(false);
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
            
            if (currentItemDisplay != null) {
                currentItemDisplay.SetVisible(false);
            }
            while (imgCanvas.alpha != targetAlpha) {
                timeSinceStarted = Time.time - timeStartedLerping;
                percentageComplete = timeSinceStarted * lerpSpeed;

                newAlpha = Mathf.Lerp(prevAlpha, targetAlpha, percentageComplete);

                imgCanvas.alpha = newAlpha;
                defaultSpriteRenderer.color = new Color(defaultSpriteRenderer.color.r, defaultSpriteRenderer.color.g, defaultSpriteRenderer.color.b, newAlpha);

                yield return new WaitForEndOfFrame();
            }
            
            if (targetAlpha == 0) {       
                ClearItem();
                gameObject.SetActive(false);
            }
        }

        public void OnPointerEnter(PointerEventData pointerEventData) {
            if ((defaultSprite != null || currentItemDisplay != null) && b.interactable == true) {
                t.SetVisible(true);
            }
        }

        public void OnPointerExit(PointerEventData pointerEventData) {
            if ((defaultSprite != null || currentItemDisplay != null) && b.interactable == true) {
                t.SetVisible(false);
            }
        }

        public void OnSelect(BaseEventData baseEventData) {
            if ((defaultSprite != null || currentItemDisplay != null) && b.interactable == true) {
                t.SetVisible(true);
            }
        }

        public void OnDeselect(BaseEventData baseEventData) {
            if ((defaultSprite != null || currentItemDisplay != null) && b.interactable == true) {
                this.t.SetVisible(false);
            }
        }
    }
}