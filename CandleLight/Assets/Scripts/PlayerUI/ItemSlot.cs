/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: July 4, 2019
* 
* The ItemDisplay class is used to display items. They can be interacted with just like buttons,
* allowing for dragging and dropping of the item inside, or clicking instantly to take the item
*
*/

using ClassConstants = Constants.ClassConstants;
using EventManager = Events.EventManager;
using Items;
using ItemConstants = Constants.ItemConstants;
using PanelConstants = Constants.PanelConstants;
using Party;
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
        public Image imgBackground;         /// <value> Background for image tooltip will be on </value> 
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
        /// Only used for item creation on eventDisplays, hence the itemSlot type can be defaulted to "any",
        /// reverting "shop" type itemSlots 
        /// </summary>
        /// <param name="newItem"> Item object </param>
        public void PlaceItem(Item newItem) {
            itemSlotType = ItemConstants.ANY;

            if (newItem != null) {  
                SetVisible(true);
            
                GameObject newItemDisplay = Instantiate(itemDisplayPrefab);
                currentItemDisplay = newItemDisplay.GetComponent<ItemDisplay>();
                currentItemDisplay.Init(newItem);
                currentItemDisplay.parentSlot = this;

                newItemDisplay.transform.SetParent(this.transform, false);
                // hide the defaultSprite if it shows
                defaultSpriteRenderer.color = new Color(defaultSpriteRenderer.color.r, defaultSpriteRenderer.color.g, defaultSpriteRenderer.color.b, 0);
                    

                t.SetImageDisplayBackgroundWidth(imgBackground.rectTransform.sizeDelta.x);
                SetTooltipText();     
            }
            else {
                // show the default sprite if nothing is in the slot (assuming there is a default sprite)
                defaultSpriteRenderer.color = new Color(defaultSpriteRenderer.color.r, defaultSpriteRenderer.color.g, defaultSpriteRenderer.color.b, 255); 
                SetVisible(true);
            }
        }

        /// <summary>
        /// Initializes the item slot with a new item, creating an item display
        /// Used for the gearPanel to instantly show partyMember's equipped items when switching between partyMembers
        /// TODO: When loading is enabled, placeItemInstant will have to update numSparefull, or there must be a method that does so
        /// </summary>
        /// <param name="newItem"> Item data </param>
        public void PlaceItemInstant(Item newItem) {
            if (newItem != null) {
                GameObject newItemDisplay = Instantiate(itemDisplayPrefab);
                currentItemDisplay = newItemDisplay.GetComponent<ItemDisplay>();
                currentItemDisplay.Init(newItem);
                currentItemDisplay.parentSlot = this;

                newItemDisplay.transform.SetParent(this.transform, false);
                // hide the defaultSprite if it shows
                defaultSpriteRenderer.color = new Color(defaultSpriteRenderer.color.r, defaultSpriteRenderer.color.g, defaultSpriteRenderer.color.b, 0);    

                t.SetImageDisplayBackgroundWidth(imgBackground.rectTransform.sizeDelta.x);
                SetTooltipText();

                if (itemSlotSubType != ItemConstants.ANY) {    // bad way to determine if its a partyMember's equippable slot
                    defaultSpriteRenderer.color = new Color(defaultSpriteRenderer.color.r, defaultSpriteRenderer.color.g, defaultSpriteRenderer.color.b, 0);
                    if (itemSlotType == ItemConstants.GEAR) {
                        PartyManager.instance.EquipGear(newItemDisplay.GetComponent<ItemDisplay>(), itemSlotSubType);
                    }
                    if (itemSlotType == ItemConstants.CANDLE) { 
                        PartyManager.instance.EquipCandle(newItemDisplay.GetComponent<ItemDisplay>(), itemSlotSubType);
                        CandlesPanel candlesPanel = (CandlesPanel)parentPanel;
                        candlesPanel.SetUsable(itemSlotSubType[0] - '0');
                    }
                }
            }
            else {
                defaultSpriteRenderer.color = new Color(defaultSpriteRenderer.color.r, defaultSpriteRenderer.color.g, defaultSpriteRenderer.color.b, 255);
                t.SetImageDisplayBackgroundWidth(imgBackground.rectTransform.sizeDelta.x);
                SetTooltipText();
            }
        }

        /// <summary>
        /// Initializes the itemSlot with an itemDisplay
        /// </summary>
        /// <param name="newItemDisplay"></param>
        public void PlaceItem(ItemDisplay newItemDisplay, bool direct = false) {
            currentItemDisplay = newItemDisplay;
            currentItemDisplay.parentSlot = this;
            currentItemDisplay.transform.SetParent(this.transform, false);          // position item placed in the middle of the slot
            currentItemDisplay.transform.localPosition = new Vector3(0f, 0f, 0f);
            
            t.SetImageDisplayBackgroundWidth(imgBackground.rectTransform.sizeDelta.x);
            SetTooltipText();
            
            if (itemSlotSubType != ItemConstants.ANY) {    // bad way to determine if its a partyMember's equippable slot
                defaultSpriteRenderer.color = new Color(defaultSpriteRenderer.color.r, defaultSpriteRenderer.color.g, defaultSpriteRenderer.color.b, 0);
                if (itemSlotType == ItemConstants.GEAR) {
                    PartyManager.instance.EquipGear(newItemDisplay, itemSlotSubType);
                }
                if (itemSlotType == ItemConstants.CANDLE) { 
                    PartyManager.instance.EquipCandle(newItemDisplay, itemSlotSubType);
                    CandlesPanel candlesPanel = (CandlesPanel)parentPanel;
                    candlesPanel.SetUsable(itemSlotSubType[0] - '0');
                }
            }
            else {
                if (newItemDisplay.type == ItemConstants.CANDLE) {  
                    newItemDisplay.displayedCandle.SetActive(false);    // for candles, if placed in a non-active slot, update the sprite
                }
                parentPanel.PlaceItem();
            }
   
            if (parentPanel.GetPanelName() == PanelConstants.EVENTDISPLAY || parentPanel.GetPanelName() == PanelConstants.REWARDSPANEL) {  // item is being placed in an eventDisplay or rewardsPanel
                EventManager.instance.UpdateTakeAll();
            }

            
            if (direct == false) {
                t.SetVisible(true);
            }
        }

        /// <summary>
        /// Initializes the itemSlot with an item, creating an itemDisplay.
        /// Used for shops by identifying the itemSlot as a "shop" type, forcing the checking
        /// of transactions to take the item, or place an item into the slot.
        /// </summary>
        /// <param name="newItem"></param>
        public void PlaceItemShop(Item newItem) {
            PlaceItem(newItem);

            itemSlotType = "shop";
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
            if (itemSlotType == "shop") {
                if (UIManager.instance.heldItemDisplay != null) {
                    if (TryShopTransaction(UIManager.instance.heldItemDisplay) == true) {
                        if (currentItemDisplay != null) {
                            ItemDisplay temp = UIManager.instance.heldItemDisplay;
                            UIManager.instance.heldItemDisplay = null;

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
                else {
                    if (TryShopTransaction() == true) {
                        TakeItem();
                    }
                }
            }
            else {
                if (UIManager.instance.heldItemDisplay != null) {
                    if (CheckCorrectItemType(UIManager.instance.heldItemDisplay) == true) {
                        if (currentItemDisplay != null) {
                            ItemDisplay temp = UIManager.instance.heldItemDisplay;
                            UIManager.instance.heldItemDisplay = null;

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
       }

        /// <summary>
        /// Takes the item from the item slot
        /// </summary>
        /// <param name="direct"> Flag for if the item will be taken directly from the itemslot with no dragging </param>
        public void TakeItem(bool direct = false) {
            if (currentItemDisplay != null && isTakeable == true) {
                bool itemTaken = false;

                if (currentItemDisplay.type == ItemConstants.CONSUMABLE) {  // consumable items are used on click
                    string[] effects = currentItemDisplay.GetEffects();
                    int[] amounts = currentItemDisplay.GetValues();
                    for (int i = 0; i < effects.Length; i++) {
                        if (effects[i] == "EXP") {
                            PartyManager.instance.AddEXP(amounts[i]);
                        }
                        else if (effects[i] == "HP") {
                            StartCoroutine(PartyManager.instance.ChangeHPAll(amounts[i]));
                        }
                        else if (effects[i] == "MP") {
                            PartyManager.instance.ChangeMPAll(amounts[i]);
                        }
                        else if (effects[i] == "WAX") {
                            PartyManager.instance.AddWAX(amounts[i]);
                        }
                        else if (effects[i] != "none") { // status effects
                            PartyManager.instance.AddSE(effects[i], amounts[i]);
                        }
                    }
                    itemTaken = true;
                    Destroy(currentItemDisplay.gameObject);
                }
                else {  // non-consumable items must be dragged into the user's inventory, or placed directly via button
                    /*  
                        Item taken directly from an eventsDisplay/rewardsPanel via the "take all" button
                        resulting in the item being placed in the corresponding panel if there's room 
                    */
                    if (direct == true) {   
                        Panel targetPanel = EventManager.instance.GetTargetPanel(currentItemDisplay.type);

                        if (currentItemDisplay.type == ItemConstants.GEAR) {
                            GearPanel gearPanel = (GearPanel)targetPanel;
                            if (gearPanel.PlaceItem(currentItemDisplay, direct)) {
                                itemTaken = true;
                            }
                        }
                        else if (currentItemDisplay.type == ItemConstants.CANDLE) {
                            CandlesPanel candlesPanel = (CandlesPanel)targetPanel;
                            if (candlesPanel.PlaceItem(currentItemDisplay, direct)) {
                                itemTaken = true;
                            }
                        }
                        else if (currentItemDisplay.type == ItemConstants.SPECIAL) {
                            SpecialPanel specialPanel = (SpecialPanel)targetPanel;
                            if (specialPanel.PlaceItem(currentItemDisplay, direct)) {
                                itemTaken = true;
                            }
                        }
                    }
                    else {
                        UIManager.instance.heldItemDisplay = currentItemDisplay;
                        UIManager.instance.StartDragItem();
                        UIManager.instance.panelButtonsEnabled = false;
                        itemTaken = true;
                    }
                }
                
                if (itemTaken) {    // if item is taken, update the itemSlot
                    currentItemDisplay = null;
                    defaultSpriteRenderer.color = new Color(defaultSpriteRenderer.color.r, defaultSpriteRenderer.color.g, defaultSpriteRenderer.color.b, 255);
                    t.SetVisible(false);
                    SetTooltipText();

                    if (parentPanel.GetPanelName() == PanelConstants.GEARPANEL) {
                        if (itemSlotSubType == ItemConstants.ANY) {     // gearPanel updates to have more free spare itemSlots
                            GearPanel gearPanel = (GearPanel)parentPanel;
                            gearPanel.TakeItem();
                        }
                        else  { // item was unequipped from partyMember
                            PartyManager.instance.UnequipGear(itemSlotSubType);
                        }
                    }
                    else if (parentPanel.GetPanelName() == PanelConstants.CANDLESPANEL) {
                        CandlesPanel candlesPanel = (CandlesPanel)parentPanel;
                        
                        if (itemSlotSubType == ItemConstants.ANY) {     // candlesPanel updates to have more free spare itemSlots
                            candlesPanel.TakeItem();
                        }
                        else  { // item was unequipped from partyMember
                            PartyManager.instance.UnequipCandle(itemSlotSubType);
                            candlesPanel.SetUsable(itemSlotSubType[0] - '0');
                        }
                    }
                    else if (parentPanel.GetPanelName() == PanelConstants.SPECIALPANEL) {
                        SpecialPanel specialPanel = (SpecialPanel)parentPanel;
                        if (itemSlotSubType == ItemConstants.ANY) {     // specialPanel updates to have more free spare itemSlots
                            specialPanel.TakeItem();
                        }
                    }
                    else if (UIManager.instance.heldItemDisplay != null) {
                        EventManager.instance.OpenItemPanel(UIManager.instance.heldItemDisplay);
                        parentPanel.TakeItem();
                        EventManager.instance.UpdateTakeAll();
                    }
                    else {
                        parentPanel.TakeItem();
                        EventManager.instance.UpdateTakeAll();
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
        /// If this itemSlot is in the candlesPanel, update the usability of the candle held.
        /// This is used to communicate upward with the panel for the usability buttons.
        /// </summary>
        /// <param name="value"></param>
        public void SetUsable(bool value) {
            if (parentPanel.GetPanelName() == PanelConstants.CANDLESPANEL) {
                 CandlesPanel candlesPanel = (CandlesPanel)parentPanel;
                 candlesPanel.SetUsable(itemSlotSubType[0] - '0');
            }
        }

        /// <summary>
        /// Checks if an itemDisplay of a type can be placed inside this slot
        /// </summary>
        /// <param name="i"> ItemDisplay to check </param>
        /// <returns> True if accepted, false otherwise </returns>
        public bool CheckCorrectItemType(ItemDisplay id) {
            if (itemSlotType == ItemConstants.ANY) {
                return true;
            }
            else if (id.type == itemSlotType && (id.subType == itemSlotSubType || itemSlotSubType == "0" || itemSlotSubType == "1" || itemSlotSubType == "2" || itemSlotSubType == ItemConstants.ANY)) {
                if (id.className != ClassConstants.ANY && itemSlotSubType != ItemConstants.ANY) {
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
        /// Attempts a shop transaction, completing the transaction if the player can afford it
        /// </summary>
        /// <param name="newItemDisplay"> Item being placed into the slot (i.e. selling or trading), can be null if only buying</param>
        /// <returns> true if transaction is succesful, false otherwise</returns>
        public bool TryShopTransaction(ItemDisplay newItemDisplay = null) {
            if (currentItemDisplay == null) {
                if (newItemDisplay == null) {   // clicking on an empty slot with nothing in hand
                    return false;
                }

                // selling
                PartyManager.instance.AddWAX((int)(newItemDisplay.GetWAXValue() * 0.5f));

                return true;
            }
            else {
                if (newItemDisplay == null) {   // buying
                    if (PartyManager.instance.WAX - currentItemDisplay.GetWAXValue() >= 0) {
                        PartyManager.instance.LoseWAX(currentItemDisplay.GetWAXValue());
                        return true;
                    }
                    
                    return false;
                }
                else {  // trading
                    if ((int)(PartyManager.instance.WAX + newItemDisplay.GetWAXValue() * 0.5f) - currentItemDisplay.GetWAXValue() >= 0) {
                        PartyManager.instance.AddWAX((int)(newItemDisplay.GetWAXValue() * 0.5f));
                        PartyManager.instance.LoseWAX(currentItemDisplay.GetWAXValue());
                        return true;
                    }

                    return false;
                }
            } 
        }

        /// <summary>
        /// Sets the text displayed in the tooltip
        /// </summary>
        public void SetTooltipText() {
            if (currentItemDisplay != null) {
                string[] basicKeys = currentItemDisplay.GetTooltipBasicKeys();

                if (basicKeys[1] == ItemConstants.CONSUMABLE) {
                    t.SetKey("title", basicKeys[0] + "_item");
                    t.SetKey("subtitle", basicKeys[1] + "_item_sub");
                    t.SetTextColour("subtitle", UIManager.instance.subtitleColour);
                    t.SetAmountTextMultiple("description", currentItemDisplay.GetTooltipEffectKeys(), currentItemDisplay.GetValuesAsStrings());
                    if (parentPanel.GetPanelName() == PanelConstants.EVENTDISPLAY && UIManager.instance.inShop == true) {
                        t.SetAmountText("value", "WAX_label", currentItemDisplay.GetWAXValue());
                    }
                    else {
                        t.SetTextActive("value", false);    // don't display the worth text if there is no item
                    }
                }
                else if (basicKeys[1] == ItemConstants.GEAR) {
                    t.SetKey("title", basicKeys[0] + "_item");
                    if (currentItemDisplay.className == ClassConstants.ANY) {
                        t.SetKey("subtitle", basicKeys[2] + "_item_sub");
                    }
                    else {
                        t.SetKeyMultiple("subtitle", new string[2] {currentItemDisplay.className + "_label", basicKeys[2] + "_item_sub"});
                    }
                    if (currentItemDisplay.className != ClassConstants.ANY && PartyManager.instance.IsClassInParty(currentItemDisplay.className) == false) {
                        t.SetTextColour("subtitle", UIManager.instance.unusableColour);
                    }
                    else {
                        t.SetTextColour("subtitle", UIManager.instance.subtitleColour);
                    }
                    
                    t.SetAmountTextMultiple("description", currentItemDisplay.GetTooltipEffectKeys(), currentItemDisplay.GetValuesAsStrings());
                    if (parentPanel.GetPanelName() == PanelConstants.EVENTDISPLAY && UIManager.instance.inShop == true) {
                        t.SetAmountText("value", "WAX_label", currentItemDisplay.GetWAXValue());
                    }
                    else {
                        t.SetAmountText("value", "WAX_label", (int)(currentItemDisplay.GetWAXValue() * 0.5f));
                    }
                }
                else if (basicKeys[1] == ItemConstants.CANDLE) {
                    t.SetKey("title", basicKeys[0] + "_item");
                    t.SetKey("subtitle", basicKeys[1] + "_item_sub");
                    t.SetTextColour("subtitle", UIManager.instance.subtitleColour);
                    t.SetAmountTextMultiple("description", currentItemDisplay.GetTooltipEffectKeys(), currentItemDisplay.GetValuesAsStrings());
                    if (parentPanel.GetPanelName() == PanelConstants.EVENTDISPLAY && UIManager.instance.inShop == true) {
                        t.SetAmountText("value", "WAX_label", currentItemDisplay.GetWAXValue());
                    }
                    else {
                        t.SetAmountText("value", "WAX_label", (int)(currentItemDisplay.GetWAXValue() * 0.5f));
                    }
                }
                else if (basicKeys[1] == ItemConstants.SPECIAL) {
                    t.SetKey("title", basicKeys[0] + "_item");
                    t.SetKey("subtitle", basicKeys[2] + "_item_sub");
                    t.SetTextColour("subtitle", UIManager.instance.subtitleColour);
                    t.SetKey("description", basicKeys[0] + "_item_des");
                    if (currentItemDisplay.subType == ItemConstants.SPECIAL) {  // very special items can't be sold, hence no value
                        t.SetAmountText("value", "WAX_label", (int)(currentItemDisplay.GetWAXValue()));
                    }
                }
            }
            else {  // if there is no item held
                t.SetKey("title", itemSlotSubType + "_item");
                t.SetKey("subtitle", itemSlotSubType + "_item_sub_default");
                t.SetKey("description", "none_label");
                t.SetTextActive("value", false);    // don't display the worth text if there is no item
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
        /// Sets the itemSlot visible and sets its type to "shop"
        /// </summary>
        /// <param name="value"></param>
        public void SetVisibleShop(bool value) {
            itemSlotType = "shop";

            SetVisible(value);
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