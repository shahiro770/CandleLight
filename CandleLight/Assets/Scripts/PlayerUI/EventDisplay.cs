/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: July 4, 2019
* 
* The EventDisplay class is used to display an image in the UI for the player to see
* in a specific event.
*
*/

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerUI {

    public class EventDisplay : MonoBehaviour {

        /* external component references */
        public Image img;   /// <value> Image to be displayed </value>
        public CanvasGroup imgCanvas;
        
        private float lerpSpeed = 4; 

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
                StartCoroutine(Fade(1));
            }
            else {
                StartCoroutine(Fade(0));
            }
        }

        /// <summary>
        /// Sets the eventDisplay's position on screen
        /// </summary>
        /// <param name="pos"></param>
        public void SetPosition(Vector3 pos) {
            gameObject.transform.localPosition = pos;
        }

        private IEnumerator Fade(int targetAlpha) {
            float timeStartedLerping = Time.time;
            float timeSinceStarted = Time.time - timeStartedLerping;
            float percentageComplete = timeSinceStarted * lerpSpeed;
            float prevAlpha = imgCanvas.alpha;

            while (imgCanvas.alpha != targetAlpha) {
                timeSinceStarted = Time.time - timeStartedLerping;
                percentageComplete = timeSinceStarted * lerpSpeed;

                imgCanvas.alpha = Mathf.Lerp(prevAlpha, targetAlpha, percentageComplete);

                yield return new WaitForEndOfFrame();
            }
        }
    }
}