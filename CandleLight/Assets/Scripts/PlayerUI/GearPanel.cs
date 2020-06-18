/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: June 9, 2019
* 
* The GearPanel class manages various visuals that show the equipment of a given partyMember.
*
*/

using Characters;
using PanelConstants = Constants.PanelConstants;
using Party;
using UnityEngine.UI;

namespace PlayerUI {

    public class GearPanel : Panel {
        
        /* external component references */
        public ActionsPanel actionsPanel;
        public ItemSlot weaponSlot;     /// <value> Weapon item slot </value>
        public ItemSlot secondarySlot;  /// <value> Secondary item slot </value>
        public ItemSlot armourSlot;     /// <value> Armour item slot </value>
        public ItemSlot[] spare = new ItemSlot[9];  /// <value> Item slots as equipment inventory </value>
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
        /// Sets the displayed weapon, secondary, and armour of a partyMember
        /// </summary>
        /// <param name="pm"> PartyMember who's equipped gear should be displayed </param>
        public void Init(PartyMember pm) {
            if (this.pmvc != null) {                // will be null on first open   
                if (this.pmvc.weapon != null) {
                    weaponSlot.HideItem();
                }
                if (this.pmvc.secondary != null) {
                    secondarySlot.HideItem();
                }
                if (this.pmvc.armour != null) {
                    armourSlot.HideItem();
                }
            }

            pmvc = pm.pmvc;
            if (pmvc.weapon != null) {
                weaponSlot.ShowItem(pmvc.weapon);
            }
            else {
                weaponSlot.PlaceItemInstant(pm.weapon);
            }

            if (pmvc.secondary != null) {
                secondarySlot.ShowItem(pmvc.secondary);
            }
            else {
                secondarySlot.PlaceItemInstant(pm.secondary);
            }

            if (pmvc.armour != null) {
                armourSlot.ShowItem(pmvc.armour);
            }
            else {
                armourSlot.PlaceItemInstant(pm.armour);
            }

            weaponSlot.SetColour(pmvc.partyMemberColour);
            secondarySlot.SetColour(pmvc.partyMemberColour);
            armourSlot.SetColour(pmvc.partyMemberColour);
        }

        /// <summary>
        /// Sets the displayed weapon, secondary, and armour of a partyMember
        /// </summary>
        /// <param name="pmvc"> PartyMemberVisualController of partyMember to display </param>
        public void Init(PartyMemberVisualController pmvc) {
            if (this.pmvc.weapon != null) {
                weaponSlot.HideItem();
            }
            if (this.pmvc.secondary != null) {
                secondarySlot.HideItem();
            }
            if (this.pmvc.armour != null) {
                armourSlot.HideItem();
            }
            
            this.pmvc = pmvc;

            if (pmvc.weapon != null) {
                weaponSlot.ShowItem(pmvc.weapon);
            }
            if (pmvc.secondary != null) {
                secondarySlot.ShowItem(pmvc.secondary);
            }
            if (pmvc.armour != null) {
                armourSlot.ShowItem(pmvc.armour);
            }

            weaponSlot.SetColour(pmvc.partyMemberColour);
            secondarySlot.SetColour(pmvc.partyMemberColour);
            armourSlot.SetColour(pmvc.partyMemberColour);
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

            weaponSlot.SetTakeable(value);
            secondarySlot.SetTakeable(value);
            armourSlot.SetTakeable(value);
        }

        /// <summary>
        /// Sets if the item slots can be interacted with (includes hovering to see tooltips)
        /// </summary>
        /// <param name="value"></param>
        public void SetInteractable(bool value) {
            for (int i = 0;i < spare.Length; i++) {
                spare[i].SetInteractable(value);
            }

            weaponSlot.SetInteractable(value);
            secondarySlot.SetInteractable(value);
            armourSlot.SetInteractable(value);    
        }

        /// <summary>
        /// Returns the name of this panel
        /// </summary>
        /// <returns> Name of panel </returns>
        public override string GetPanelName() {
            return PanelConstants.GEARPANEL;
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
