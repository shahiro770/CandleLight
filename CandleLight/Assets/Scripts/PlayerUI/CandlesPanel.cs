/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: August 1, 2019
* 
* The CandlesPanel is the display showing the player's inventory for candles,
* as well as the ability to equip and use candles.
*
*/

using Characters;
using CombatManager = Combat.CombatManager;
using PanelConstants = Constants.PanelConstants;
using Party;
using UIEffects;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerUI {

    public class CandlesPanel : Panel {
        
        /* external component references */
        public ItemSlot[] activeCandles;                    /// <value> active candle item slots </value>
        public ItemSlot[] spare;                            /// <value> spare candle item slots </value>
        public Button[] useButtons;                         /// <value> Buttons clicked to use candles </value>
        public ButtonTransitionState[] useButtonbtss;       /// <value> Use button btss </value>
        public Tooltip[] useTooltips;                       /// <value> Use button tooltips </value>

        public PartyMemberVisualController pmvc = null;     /// <value> Controller for all visuals related to partyMember </value>
    
        public int numSpareFull = 0;    /// <value> Number of spare itemSlots with items in them (max 9) </value>
        public bool isOpen;             /// <value> Flag for if this panel is open (true if open, false otherwise) </value>

        private int maxSpare = 9;       /// <value> Max number of spare itemSlots </value>

        /// <summary>
        /// Update the panel with relevant information and visuals when opened
        /// </summary>
        void OnEnable() {
            isOpen = true;
            Init(PartyManager.instance.GetActivePartyMember());
        }

        /// <summary>
        /// Set isOpen to false on disabling so relevant interactions don't happen
        /// </summary>
        void OnDisable() {
            isOpen = false;
        }

        /// <summary>
        /// Sets the displayed candles of a partyMember
        /// </summary>
        /// <param name="pm"> PartyMember who's equipped gear should be displayed </param>
        public void Init(PartyMember pm) {
            if (this.pmvc != null) {                // will be null on first open   
                if (this.pmvc.activeCandles[0] != null) {
                    activeCandles[0].HideItem();
                }
                if (this.pmvc.activeCandles[1] != null) {
                    activeCandles[1].HideItem();
                }
                if (this.pmvc.activeCandles[2] != null) {
                    activeCandles[2].HideItem();
                }
            }

            pmvc = pm.pmvc;
            if (pmvc.activeCandles[0] != null) {
                activeCandles[0].ShowItem(pmvc.activeCandles[0]);
            }
            else {
                activeCandles[0].PlaceItemInstant(pm.activeCandles[0]);
            }

            if (pmvc.activeCandles[1] != null) {
                activeCandles[1].ShowItem(pmvc.activeCandles[1]);
            }
            else {
                activeCandles[1].PlaceItemInstant(pm.activeCandles[1]);
            }

             if (pmvc.activeCandles[2] != null) {
                activeCandles[2].ShowItem(pmvc.activeCandles[2]);
            }
            else {
                activeCandles[2].PlaceItemInstant(pm.activeCandles[2]);
            }

            if (activeCandles[0].b.interactable == true) {  // if the panel is not interactable, don't set the use buttons (which will be set when interactability is true)
                SetUsables();
            }
            
            activeCandles[0].SetColour(pmvc.partyMemberColour);
            activeCandles[1].SetColour(pmvc.partyMemberColour);
            activeCandles[2].SetColour(pmvc.partyMemberColour);
        }

        /// <summary>
        /// Sets the displayed candles of a partyMember, using a pmvc instead of a pm
        /// </summary>
        /// <param name="pmvc"></param>
        public void Init(PartyMemberVisualController pmvc) {
            if (this.pmvc.activeCandles[0] != null) {
                activeCandles[0].HideItem();
            }
            if (this.pmvc.activeCandles[1] != null) {
                activeCandles[1].HideItem();
            }
            if (this.pmvc.activeCandles[2] != null) {
                activeCandles[2].HideItem();
            }
            
            this.pmvc = pmvc;

            if (pmvc.activeCandles[0] != null) {
                activeCandles[0].ShowItem(pmvc.activeCandles[0]);
            }
            if (pmvc.activeCandles[1] != null) {
                activeCandles[1].ShowItem(pmvc.activeCandles[1]);
            }
            if (pmvc.activeCandles[2] != null) {
                activeCandles[2].ShowItem(pmvc.activeCandles[2]);
            }

            SetUsables();

            activeCandles[0].SetColour(pmvc.partyMemberColour);
            activeCandles[1].SetColour(pmvc.partyMemberColour);
            activeCandles[2].SetColour(pmvc.partyMemberColour);
        }

        /// <summary>
        /// Places an item in a spare itemSlot
        /// </summary>
        /// <param name="id"> ItemDisplay </param>
        /// <returns> True if possible, false otherwise </returns>
        public bool PlaceItem(ItemDisplay id, bool direct = false) {
            if (maxSpare > numSpareFull) {
                numSpareFull++;

                for (int i = 0;i < spare.Length; i++) {
                    if (spare[i].currentItemDisplay == null) {
                        spare[i].PlaceItem(id, direct);
                        break;
                    }
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Decreases the number of spare item slots full upon an item being taken
        /// </summary>
        public void TakeItem() {
            numSpareFull--;
        }

        /// <summary>
        /// Sets if all of the item slots can have their items taken
        /// </summary>
        /// <param name="value"></param>
        public void SetTakeable(bool value) {
            for (int i = 0;i < spare.Length; i++) {
                spare[i].SetTakeable(value);
            }

            activeCandles[0].SetTakeable(value);
            activeCandles[1].SetTakeable(value);
            activeCandles[2].SetTakeable(value);
        }

        /// <summary>
        /// Set the usable buttons if they're usable
        /// </summary>
        public void SetUsables() {
            SetUsable(0);
            SetUsable(1);
            SetUsable(2);
        }

        /// <summary>
        /// Checks if a use button is usable, updating its tooltip for the attack
        /// </summary>
        /// <param name="index"> Active candle slot to check (0, 1, or 2) </param>
        public void SetUsable(int index) {
            if (activeCandles[index].currentItemDisplay != null) {
                if (activeCandles[index].currentItemDisplay.displayedCandle.isUsable == true) {
                    useButtons[index].interactable = true;
                    activeCandles[index].currentItemDisplay.displayedCandle.SetActive(true);
                    SetTooltipText(index);
                }
                else {
                    useButtons[index].interactable = false;
                    activeCandles[index].currentItemDisplay.displayedCandle.SetActive(false);
                    useTooltips[index].SetVisible(false);
                }
            }
            else { 
                useButtons[index].interactable = false;
                useTooltips[index].SetVisible(false);
            }
        }

        /// <summary>
        /// Use a candle's attack.
        /// OnClick for the use candle button.
        /// </summary>
        /// <param name="index"></param>
        public void UseCandle(int index) {
            if (CombatManager.instance.inCombat == true) {
                CombatManager.instance.UseCandle(index);
            }
            else {
                PartyManager.instance.UseCandle(index);
            }
            
            SetUsable(index);
        }

        /// <summary>
        /// Sets if the item slots can be interacted with (includes hovering to see tooltips)
        /// </summary>
        /// <param name="value"></param>
        public void SetInteractable(bool value) {
            for (int i = 0;i < spare.Length; i++) {
                spare[i].SetInteractable(value);
            }

            activeCandles[0].SetInteractable(value);
            activeCandles[1].SetInteractable(value);
            activeCandles[2].SetInteractable(value);  

            if (value == false) {
                useButtons[0].interactable = value;
                useTooltips[0].SetVisible(value);
                useButtons[1].interactable = value;
                useTooltips[1].SetVisible(value);
                useButtons[2].interactable = value;
                useTooltips[2].SetVisible(value);
            }
            else {
                SetUsables();
            }
        }

        /// <summary>
        /// Sets the tooltip text for the use buttons
        /// </summary>
        /// <param name="index"> Index of the use button (0, 1, or 2) </param>
        private void SetTooltipText(int index) {
            string candleNameID = activeCandles[index].currentItemDisplay.GetTooltipBasicKeys()[0];
            useTooltips[index].SetImageDisplayBackgroundWidth(useButtons[index].GetComponent<RectTransform>().sizeDelta.x);

            useTooltips[index].SetKey("title", candleNameID + "_use_title");
            useTooltips[index].SetKey("subtitle", activeCandles[index].currentItemDisplay.displayedCandle.uses + "_use_sub");
            useTooltips[index].SetKey("description", candleNameID + "_use_des");
        }

        /// <summary>
        /// Returns the name of this panel
        /// </summary>
        /// <returns> Name of panel </returns>
        public override string GetPanelName() {
            return PanelConstants.CANDLESPANEL;
        }

        /// <summary>
        /// Returns the Button that adjacent panels will navigate to
        /// </summary>
        /// <returns> Button to be navigated to </returns>
        public override Button GetNavigatableButton() {
            return spare[0].b;
        }
    }
}