/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: September 28, 2019
* 
* The UIManager class is used to store information that affects many different UI components.
*
*/

using UnityEngine;

namespace PlayerUI {

    public class UIManager : MonoBehaviour {

        public static UIManager instance;             /// <value> Global instance </value>

        public ItemDisplay heldItemDisplay;
        public bool panelButtonsEnabled = true;
        public bool inShop = false;                 /// <value> Flag for if the player is currently in a shop </value>

        /// <summary>
        /// Awake to instantiate singleton
        /// </summary> 
        void Awake() {
            if (instance == null) {
                instance = this;
            }
            else if (instance != this) {
                DestroyImmediate(gameObject);
                instance = this;
            }
        }
    }
}