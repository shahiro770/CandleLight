/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: June 18, 2019
* 
* The TooltipController class is used to give a component with a button
* the ability to control when its tooltip displays. This is useful for Monster gameObjects
* where the button is not on the same component with the Monster script (which makes 
* tooltips unresponsive).
*
*/

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PlayerUI {

    public class TooltipController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler {

        /* external component references */
        public Button b;
        public Tooltip t;

        public void OnPointerEnter(PointerEventData pointerEventData) {
            if (b.interactable) {
                t.SetVisible(true);
            }
        }

        //Detect when Cursor leaves the GameObject
        public void OnPointerExit(PointerEventData pointerEventData) {
            if (b.interactable) {
                t.SetVisible(false);
            }
        }

        public void OnSelect(BaseEventData baseEventData) {
            if (b.interactable) {
                t.SetVisible(true);
            }
        }

        //Detect when Cursor leaves the GameObject
        public void OnDeselect(BaseEventData baseEventData) {
            if (b.interactable) {
                t.SetVisible(false);
            }
        }
    }
}

