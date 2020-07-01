/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: June 18, 2019
* 
* The TooltipControllerTextMesh class is used to give a component with a testMeshProUGUI
* the ability to control when its tooltip displays. This is useful for gameObjects
* where the button is not on the same component with the object's script (which makes 
* tooltips unresponsive).
*
*/

using UnityEngine;
using UnityEngine.EventSystems; 
using UnityEngine.UI;

namespace PlayerUI {

    public class TooltipControllerTextMesh : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

        /* external component references */
        public TooltipTextMesh t;

        public void OnPointerEnter(PointerEventData pointerEventData) {
            t.SetVisible(true);
        }

        public void OnPointerExit(PointerEventData pointerEventData) {
            t.SetVisible(false);
        }
    }
}

