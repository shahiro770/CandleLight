/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: June 9, 2019
* 
* The Panel class is an abstract class for the player's UI. Each segment of the player' UI
* (their inventory, actions, party, etc.) is put inside a panel.
*
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerUI {

    public class Panel : MonoBehaviour {

        public int numSpareFull = 0; /// <value> Number of spare itemSlots with items in them (panels that use this property have a max) </value>

        /// <summary>
        /// Increases number of numSpareFull
        /// </summary>
        public virtual void PlaceItem() {
            numSpareFull++;
        }

        /// <summary>
        /// Decreases number of numSpareFull
        /// </summary>
        public virtual void TakeItem() {
            numSpareFull--;
        }

        /// <summary>
        /// Returns the name of this panel
        /// </summary>
        /// <returns> Name of panel </returns>
        public virtual string GetPanelName() {
            return null;
        }
        
        /// <summary>
        /// Returns the Button that adjacent panels will navigate to
        /// </summary>
        /// <returns> Button to be navigated to </returns>
        public virtual Button GetNavigatableButton() {
            return null;
        }
    }
}