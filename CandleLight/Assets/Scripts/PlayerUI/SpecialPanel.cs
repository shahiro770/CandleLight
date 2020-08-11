/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: August 1, 2019
* 
* The special panel holds special items that are needed for quests
* or symbolic for the main story.
*/

using EventManager = Events.EventManager;
using Items;
using PanelConstants = Constants.PanelConstants;

namespace PlayerUI {

    public class SpecialPanel : Panel {

        /* external component references */
        public ItemSlot[] verySpecials;                       /// <value> very special item slots </value>
        public ItemSlot[] spare;                             /// <value> special item slots </value>

        public bool isOpen;             /// <value> Flag for if this panel is open (true if open, false otherwise) </value>

        private int maxSpare = 9;       /// <value> Max number of spare itemSlots </value>

        /// <summary>
        /// Update the panel with relevant information and visuals when opened
        /// </summary>
        void OnEnable() {
            isOpen = true;
            Init();
        }

        /// <summary>
        /// Set isOpen to false on disabling so relevant interactions don't happen
        /// </summary>
        void OnDisable() {
            isOpen = false;
        }

        /// <summary>
        /// Initials the verySpecial items, putting stuff in them (which
        /// for this game's single level, is always the VerySpecial0 item)
        /// </summary>
        public void Init() {
            if (verySpecials[0].currentItemDisplay == null) {
                verySpecials[0].PlaceItemInstant(EventManager.instance.strangeBottle);
                verySpecials[1].PlaceItemInstant(null);
                verySpecials[2].PlaceItemInstant(null);
            }
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
        /// Checks if a specific item is held within the spare item slots
        /// </summary>
        /// <param name="itemName"></param>
        /// <param name="isClear"> True if the item is to be destroyed after the check </param>
        /// <returns></returns>
        public bool CheckItem(string itemName, bool isClear = false) {
            for (int i = 0; i < spare.Length; i++) {
                if (spare[i].currentItemDisplay != null) {
                    if (spare[i].currentItemDisplay.displayedSpecial.nameID == itemName) {
                        if (isClear == true) {
                            spare[i].ClearItem();
                        }
                        return true;
                    }
                }
            }
            
            return false;
        }

        /// <summary>
        /// Returns special data for saving
        /// </summary>
        /// <returns></returns>
        public ItemData[] GetSpareSpecialData() {
            ItemData[] spareSpecials = new ItemData[spare.Length];
            for (int i = 0; i < spareSpecials.Length; i++) {  
                if (spare[i].currentItemDisplay != null) {
                    spareSpecials[i] = spare[i].currentItemDisplay.displayedSpecial.GetItemData();
                }
            }

            return spareSpecials;
        }

        /// <summary>
        /// Load saved item data into spare slots
        /// </summary>
        /// <param name="specialData"></param>
        public void LoadData(ItemData[] specialData) {
            for (int i = 0; i < specialData.Length; i++) {  
                if (specialData[i] != null) {
                    Special s = new Special(specialData[i]);
                    spare[i].PlaceItemInstant(s);
                }
            }
        }

        /// <summary>
        /// Sets if all of the item slots can have their items taken
        /// </summary>
        /// <param name="value"></param>
        public void SetTakeable(bool value) {
            for (int i = 0; i < spare.Length; i++) {
                spare[i].SetTakeable(value);
            }

            // for (int i = 0; i < activeSpecials.Length; i++) {
            //     activeSpecials[i].SetTakeable()
            // }
        }

        /// <summary>
        /// Sets if the item slots can be interacted with (includes hovering to see tooltips)
        /// </summary>
        /// <param name="value"></param>
        public void SetInteractable(bool value) {
            for (int i = 0; i < spare.Length; i++) {
                spare[i].SetInteractable(value);
            }

            for (int i = 0; i < verySpecials.Length; i++) {
                verySpecials[i].SetInteractable(value);
            }
        }

        /// <summary>
        /// Returns the name of this panel
        /// </summary>
        /// <returns> Name of panel </returns>
        public override string GetPanelName() {
            return PanelConstants.SPECIALPANEL;
        }
    }
}