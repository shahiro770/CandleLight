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

        public Color32 unusableColour = new Color32(196, 36, 48, 255);  /// <value> Red colour to indicate unusable, stored here to minimize space </value>
        public Color32 subtitleColour = new Color32(178, 178, 178, 255);  /// <value> Grey colour for subtitles, stored here to minimize space </value>
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
        
        /// <summary>
        /// Starts dragging the held item across the screen wherever the mouse position is.
        /// UIManager is never set unactive, so itemDisplays being dragged won't stop until placed somewhere.
        /// </summary>
        public void StartDragItem() {
            StartCoroutine(heldItemDisplay.StartDragItem());
        }
    }
}