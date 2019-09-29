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

        public int numSpareFull = 0;
        public bool isOpen;

        /// <summary>
        /// Update the partyPanel with relevant information and visuals when opened
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
        /// <param name="pm"></param>
        public void Init(PartyMember pm) {
            weapon.PlaceItem(pm.weapon);
            secondary.PlaceItem(pm.secondary);
            armour.PlaceItem(pm.armour);
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
