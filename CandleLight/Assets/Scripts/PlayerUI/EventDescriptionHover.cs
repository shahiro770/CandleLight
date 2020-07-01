/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: January 28, 2019
* 
* EventDescriptionHover is a simple hover checker to make the eventDescription
* fade when hovered on (in the rare chance the player wants to see whats behind it)
*
*/

using UnityEngine;
using UnityEngine.EventSystems;

namespace PlayerUI {

    public class EventDescriptionHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
        
        public EventDescription eventDescription;
        public CanvasGroup textBackgroundCanvas; /// <value> Canvas group for controlling alpha </value>
    
        public void OnPointerEnter(PointerEventData pointerEventData) {
            if (textBackgroundCanvas.alpha == 1) {
                StartCoroutine(eventDescription.Fade(0));
            }
        }

        public void OnPointerExit(PointerEventData pointerEventData) {
            StartCoroutine(eventDescription.Fade(1));
        }
    }
}