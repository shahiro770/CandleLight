/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: June 18, 2019
* 
* The EventDescription gives a description of the current event.
* This can be a prompt for the event, a monster's attack name, and etc.
*
*/

using EventManager = Events.EventManager;
using Localization;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerUI {

    public class Tooltip : MonoBehaviour {
        
        public LocalizedText titleText;     /// <value> Text for event </value>
        public LocalizedText subtitleText;
        public LocalizedText descriptionText;
        public Image textBackground;        /// <value> Image behind text </value>
        public Image imageDisplayBackground; /// <value> Largest image around the image that when hovered, will display the tooltip </value>

        [field: SerializeField] private float rootCanvasWidthHalved;
        [field: SerializeField] private float rootCanvasHeightHalved;
        [field: SerializeField] private float canvasScaleFactor;    
        [field: SerializeField] private float imageDisplayBackgroundWidthHalved;    /// <value> Half the width of whatever image is being displayed </value>

        public void SetImageDisplayBackgroundWidth(float width) {
            rootCanvasWidthHalved = EventManager.instance.canvasWidth * 0.5f;
            rootCanvasHeightHalved = EventManager.instance.canvasHeight * 0.5f;
            canvasScaleFactor = EventManager.instance.canvasScaleFactor;
            imageDisplayBackgroundWidthHalved = width * 0.5f;
        }

        /// <summary>
        /// Changes the displayed text
        /// </summary>
        /// <param name="textKey"> Localized key for text to display </param>
        public void SetKey(string textName, string textKey) {
            if (textName == "title") {
                titleText.SetKey(textKey);
            }
            else if (textName == "subtitle") {
                subtitleText.SetKey(textKey);
            }
            else if (textName == "description") {
                descriptionText.SetKey(textKey);
            }
            else {
                Debug.LogError("TextName " + textName + " does not exist");
            }
        }

        /// <summary>
        /// Changes the displayed text of one of the text's using multiple keys
        /// </summary>

        public void SetKeyMultiple(string textName, string[] textKeys) {
            if (textName == "title") {
                titleText.SetMultipleKeysAndJoin(textKeys);
            }
            else if (textName == "subtitle") {
                subtitleText.SetMultipleKeysAndJoin(textKeys);
            }
            else if (textName == "description") {
                descriptionText.SetMultipleKeysAndJoin(textKeys);
            }
            else {
                Debug.LogError("TextName " + textName + " does not exist");
            }
        }

        /// <summary>
        /// Changes the displayed text to show text and append it with an integer amount
        /// </summary>

        public void SetAmountText(string textName, string textKey, int amount) {
            if (textName == "title") {
                titleText.SetKeyAndAppend(textKey, amount.ToString());
            }
            else if (textName == "subtitle") {
                subtitleText.SetKeyAndAppend(textKey, amount.ToString());
            }
            else if (textName == "description") {
                descriptionText.SetKeyAndAppend(textKey, amount.ToString());
            }
            else {
                Debug.LogError("TextName " + textName + " does not exist");
            }
        }

        /// <summary>
        /// Changes the displayed text 
        /// </summary>

        public void SetAmountTextMultiple(string textName, string[] textKeys, string[] amounts) {
            if (textName == "title") {
                titleText.SetMultipleKeysAndAppend(textKeys, amounts);
            }
            else if (textName == "subtitle") {
                subtitleText.SetMultipleKeysAndAppend(textKeys, amounts);
            }
            else if (textName == "description") {
                descriptionText.SetMultipleKeysAndAppend(textKeys, amounts);
            }
            else {
                Debug.LogError("TextName " + textName + " does not exist");
            }     

            if (this.gameObject.activeSelf == true) {
                StartCoroutine(SetPosition());
            }
        }

        /// <summary>
        /// Stop displaying and remove all text
        /// </summary>
        public void ClearText() {
             titleText.Clear();
             subtitleText.Clear();
             descriptionText.Clear();
        }

        /// <summary>
        /// Sets the position of the tooltip
        /// </summary>
        /// <returns> 
        /// IEnumerator so tooltip doesn't show until after its layout updates to dynamically
        /// resize based on its content.
        /// </returns>
        private IEnumerator SetPosition() {
            textBackground.color = new Color32(0, 0, 0, 0); // hide the tooltip while position adjustments are being made
            titleText.SetColour(new Color32(0, 0, 0 ,0));
            subtitleText.SetColour(new Color32(0, 0, 0 ,0));
            descriptionText.SetColour(new Color32(0, 0, 0 ,0));
            yield return new WaitForEndOfFrame();
            float textBackgroundWidth = textBackground.rectTransform.sizeDelta.x;
            float textBackgroundHeight = textBackground.rectTransform.sizeDelta.y;
            float newXPos = 0f;
            float newYPos = 0f;

            /* 
             * The way the textBackground (UITile behind the tooltip)'s position is calculated 
             * is from the center of the canvas. This means with a canvas that 960 pixels wide,
             * if the position of the textBackground + its width > 480 (960 / 2), it will be out of the screen.
             * +3 magic number is for spacing.
             */
            if ((imageDisplayBackground.transform.position.x * canvasScaleFactor + imageDisplayBackgroundWidthHalved + textBackgroundWidth + 3) >= rootCanvasWidthHalved) {
                newXPos = (textBackgroundWidth + imageDisplayBackgroundWidthHalved + 3) * -1;
            }
            else {
                newXPos = imageDisplayBackgroundWidthHalved + 3;
            }

            /* 
             * The way the textBackground (UITile behind the tooltip)'s position is calculated 
             * is from the center of the canvas. This means with a canvas that 540 pixels tall,
             * if the position of the textBackground + its height > 270 (540 / 2), it will be below the screen.
             * Tooltips can never go above the screen unless the image they are on is out of the screen, so that edge case
             * is no concern. Some numbers are treated as negatives due to being below the origin point (0, 0);
             * -7 magic number is for spacing.
             */
            if (imageDisplayBackground.transform.position.y * canvasScaleFactor - textBackgroundHeight <= rootCanvasHeightHalved * -1) {
                newYPos = (int)(((imageDisplayBackground.transform.position.y  * canvasScaleFactor) - textBackgroundHeight) + rootCanvasHeightHalved - 7) * -1;
            }
            else {
                newYPos = gameObject.transform.localPosition.y;
            }

            gameObject.transform.localPosition = new Vector3(newXPos, newYPos);

            textBackground.color = new Color32(255, 255, 255, 255);
            titleText.SetColour(new Color32(255, 255, 255 , 255));
            subtitleText.SetColour(new Color32(178, 178, 178 ,255));
            descriptionText.SetColour(new Color32(255, 255, 255 ,255));
        }

        public void SetVisible(bool value) {
            if (this.gameObject.activeSelf != value) {  // prevents itemSlot tooltips from blinking because the events seem to trigger statically (not sure why)
                this.gameObject.SetActive(value);

                if (value == true) {
                    StartCoroutine(SetPosition());
                }
            }
        }
    }
}
