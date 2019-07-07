/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: July 4, 2019
* 
* The EventDisplay class is used to display an image in the UI for the player to see
* in a specific event.
*
*/

using UnityEngine;
using UnityEngine.UI;

namespace Events {

    public class EventDisplay : MonoBehaviour {

        public Image img;   /// <value> Image to be displayed </value>

        /// <summary>
        /// Sets image to display a given sprite
        /// </summary>
        /// <param name="spr"> Sprite to be displayed </param>
        public void SetImage(Sprite spr) {
            img.sprite = spr;
        }

        /// <summary>
        /// Makes the eventDisplay visible
        /// </summary>
        /// <param name="value"></param>
        public void SetVisible(bool value) {
            if (value == true) {
                gameObject.SetActive(true);
            }
            else {
                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Sets the eventDisplay's position on screen
        /// </summary>
        /// <param name="pos"></param>
        public void SetPosition(Vector3 pos) {
            gameObject.transform.localPosition = pos;
        }
    }
}