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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerUI {

    public class GearPanel : Panel {
        
        /* external component references */
        public ItemDisplay weapon;
        public ItemDisplay secondary;
        public ItemDisplay armor;
        public ItemDisplay[] spare = new ItemDisplay[9];

        public bool isOpen;

        /// <summary>
        /// Update the partyPanel with relevant information and visuals when opened
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

        public void Init(PartyMember pm) {
            weapon.Init(pm.weapon);
            secondary.Init(pm.secondary);
            armor.Init(pm.armor);
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
