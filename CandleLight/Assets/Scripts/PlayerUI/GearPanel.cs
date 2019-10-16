/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: June 9, 2019
* 
* The GearPanel class manages various visuals that show the equipment of a given partyMember.
*
*/

using Characters;
using Items;
using PanelConstants = Constants.PanelConstants;
using Party;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerUI {

    public class GearPanel : Panel {
        
        /* external component references */
        public ItemSlot weapon;     /// <value> Weapon item slot </value>
        public ItemSlot secondary;  /// <value> Secondary item slot </value>
        public ItemSlot armour;     /// <value> Armour item slot </value>
        public ItemSlot[] spare = new ItemSlot[9];  /// <value> Item slots as equipment inventory </value>
        public PartyMemberVisualController pmvc = null;     /// <value> Controller for all visuals related to partyMember</value>

        public int numSpareFull = 0;    /// <value> Number of spare itemSlots with items in them (max 9) </value>
        public bool isOpen;             /// <value> Flag for if this panel is open (true if open, false otherwise) </value>

        private int maxSpare = 9;       /// <value> Max number of spare itemSlots </value>

        /// <summary>
        /// Update the partyPanel with relevant information and visuals when opened
        /// TODO: This renders twice for some reason at the start
        /// </summary>
        void OnEnable() {
            isOpen = true;
            Init(PartyManager.instance.GetActivePartyMember());

            foreach (ItemSlot iSpare in spare) {
                iSpare.PlaceItem(new Item());
            }
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
            this.pmvc = pm.pmvc;

            weapon.PlaceItem(pm.weapon);
            secondary.PlaceItem(pm.secondary);
            armour.PlaceItem(pm.armour);
        }

        /// <summary>
        /// Sets the displayed weapon, secondary, and armour of a partyMember
        /// </summary>
        /// <param name="pmvc"> PartyMemberVisualController of partyMember to display </param>
        public void Init(PartyMemberVisualController pmvc) {
            if (this.pmvc.weapon != null) {
                weapon.HideItem();
            }
            if (this.pmvc.secondary != null) {
                secondary.HideItem();
            }
            if (this.pmvc.armour != null) {
                armour.HideItem();
            }
            
            this.pmvc = pmvc;

            if (pmvc.weapon != null) {
                weapon.ShowItem(pmvc.weapon);
            }
            else {
                weapon.PlaceItem(new Gear());
            }
            
            if (pmvc.secondary != null) {
                secondary.ShowItem(pmvc.secondary);
            }
            else {
                secondary.PlaceItem(new Gear());
            }
            
            if (pmvc.armour != null) {
                armour.ShowItem(pmvc.armour);
            }
            else {
                armour.PlaceItem(new Gear());
            }
        }

        /// <summary>
        /// Places an item in a spare itemSlot
        /// </summary>
        /// <param name="id"> ItemDisplay </param>
        /// <returns> True if possible, false otherwise </returns>
        public bool PlaceItem(ItemDisplay id) {
            if (maxSpare > numSpareFull) {
                numSpareFull++;

                for (int i = 0;i < spare.Length; i++) {
                    if (spare[i].currentItemDisplay == null) {
                        spare[i].PlaceItem(id);
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

            weapon.SetTakeable(value);
            secondary.SetTakeable(value);
            armour.SetTakeable(value);
        }

        /// <summary>
        /// Sets if the item slots can be interacted with (includes hovering to see tooltips)
        /// </summary>
        /// <param name="value"></param>
        public void SetInteractable(bool value) {
            if (isOpen == true) {
                for (int i = 0;i < spare.Length; i++) {
                    spare[i].SetInteractable(value);
                }

                weapon.SetInteractable(value);
                secondary.SetInteractable(value);
                armour.SetInteractable(value);
            }
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
            return null;
        }
    }
}
