/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: June 18, 2019
* 
* The EventDescription gives a description of the current event.
* This can be a prompt for the event, a monster's attack name, and etc.
*
*/

using GameManager = General.GameManager;
using Localization;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerUI {

    public class Tooltip : MonoBehaviour {
        
        public LocalizedText titleText;     /// <value> Text for title </value>
        public LocalizedText subtitleText;
        public LocalizedText descriptionText;
        public LocalizedText valueText;     /// <value> Text for the monetary value of an item (items only) </value>
        public Image textBackground;        /// <value> Image behind text </value>
        public Image imageDisplayBackground; /// <value> Largest image around the image that when hovered, will display the tooltip </value>

        [field: SerializeField] private float rootCanvasWidthHalved;
        [field: SerializeField] private float rootCanvasHeightHalved;
        [field: SerializeField] private float canvasScaleFactor;    
        [field: SerializeField] private float imageDisplayBackgroundWidthHalved;    /// <value> Half the width of whatever image is being displayed </value>
        Color32 titleColourCurrent = new Color32(255, 255, 255, 255);               /// <value> store the prevous colours to set them back when hiding during repositions </value>
        Color32 subtitleColourCurrent = new Color32(178, 178, 178, 255);
        Color32 descriptionColourCurrent = new Color32(255, 255, 255, 255);
        Color32 valueColourCurrent = new Color32(255, 205, 2, 255);

        /// <summary>
        /// Sets a bunch of parameters for properly positioning the tooltip
        /// </summary>
        /// <param name="width"> Width of the image this tooltip is for </param>
        public void SetImageDisplayBackgroundWidth(float width) {
            rootCanvasWidthHalved = GameManager.instance.canvasWidth * 0.5f;
            rootCanvasHeightHalved = GameManager.instance.canvasHeight * 0.5f;
            canvasScaleFactor = GameManager.instance.canvasScaleFactor;
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
            else if (textName == "value") {
                valueText.SetKey(textKey);
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
            else if (textName == "value") {
                valueText.SetMultipleKeysAndJoin(textKeys);
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
            else if (textName == "value") {
                if (valueText.gameObject.activeSelf == false) {
                    SetTextActive("value", true);
                }
                valueText.SetKeyAndAppend(textKey, amount.ToString());
            }
            else {
                Debug.LogError("TextName " + textName + " does not exist");
            }
        }

        /// <summary>
        /// Changes the displayed text to show text and append it with an integer amount
        /// with a string entered as the amount
        /// </summary>
        /// <param name="textName"></param>
        /// <param name="textKey"></param>
        /// <param name="amount"></param>
        public void SetAmountText(string textName, string textKey, string amount) {
            if (textName == "title") {
                titleText.SetKeyAndAppend(textKey, amount);
            }
            else if (textName == "subtitle") {
                subtitleText.SetKeyAndAppend(textKey, amount);
            }
            else if (textName == "description") {
                descriptionText.SetKeyAndAppend(textKey, amount);
            }
            else if (textName == "value") {
                if (valueText.gameObject.activeSelf == false) {
                    SetTextActive("value", true);
                }
                valueText.SetKeyAndAppend(textKey, amount);
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
            else if (textName == "value") {
                if (valueText.gameObject.activeSelf == false) {
                    SetTextActive("value", true);
                }
                valueText.SetMultipleKeysAndAppend(textKeys, amounts);
            }
            else {
                Debug.LogError("TextName " + textName + " does not exist");
            }     

            if (this.gameObject.activeSelf == true) {
                StartCoroutine(SetPosition());
            }
        }

        /// <summary>
        /// Sets the colour of the specified text
        /// </summary>
        /// <param name="textName"></param>
        /// <param name="newColor"></param>
        public void SetColour(string textName, Color32 newColor) {
            if (textName == "title") {
                titleText.SetColour(newColor);
                titleColourCurrent = newColor;
            }
            else if (textName == "subtitle") {
                subtitleText.SetColour(newColor);
                subtitleColourCurrent = newColor;
            }
            else if (textName == "description") {
                descriptionText.SetColour(newColor);
                descriptionColourCurrent = newColor;
            }
            else if (textName == "value") {
                valueText.SetColour(newColor);
                valueColourCurrent = newColor;
            }
        }

        /// <summary>
        /// Sets the colour of the specified text
        /// </summary>
        /// <param name="textName"></param>
        /// <param name="newColor"></param>
        public void SetColour(string textName, string newColor) {
            if (textName == "title") {
                titleText.SetColour(newColor);
            }
            else if (textName == "subtitle") {
                subtitleText.SetColour(newColor);
            }
            else if (textName == "description") {
                descriptionText.SetColour(newColor);
            }
            else if (textName == "value") {
                valueText.SetColour(newColor);
                
            }
        }

        /// <summary>
        /// Stop displaying and remove all text
        /// </summary>
        public void ClearText() {
             titleText.Clear();
             subtitleText.Clear();
             descriptionText.Clear();
             valueText.Clear();
        }

        /// <summary>
        /// Sets the gameObject of a given text active or inactive
        /// Used to negate unnecessary spacing added to a tooltip because 
        /// vertical layout groups will add spacing to gameObjects of height and width 0
        /// </summary>
        /// <param name="textName"></param>
        /// <param name="value"></param>
        public void SetTextActive(string textName, bool value) {
            if (textName == "title") {
                titleText.gameObject.SetActive(value);
            }
            else if (textName == "subtitle") {
                subtitleText.gameObject.SetActive(value);
            }
            else if (textName == "description") {
                descriptionText.gameObject.SetActive(value);
            }
            else if (textName == "value") {
                valueText.gameObject.SetActive(value);
            }
            else {
                Debug.LogError("TextName " + textName + " does not exist");
            }     
        }

        /// <summary>
        /// Sets the position of the tooltip
        /// </summary>
        /// <returns> 
        /// IEnumerator so tooltip doesn't show until after its layout updates to dynamically
        /// resize based on its content.
        /// </returns>
        private IEnumerator SetPosition() {
            textBackground.color = UIManager.instance.hideColour;   // hide the tooltip while position adjustments are being made
            titleText.SetColour(UIManager.instance.hideColour);
            subtitleText.SetColour(UIManager.instance.hideColour);
            descriptionText.SetColour(UIManager.instance.hideColour);
            valueText.SetColour(UIManager.instance.hideColour);
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
             * -7 magic number is for spacing and -9 is to deal with weirdo rounding.
             */
            if (imageDisplayBackground.transform.position.y * canvasScaleFactor - textBackgroundHeight <= (rootCanvasHeightHalved * -1) - 9) {
                newYPos = (int)(((imageDisplayBackground.transform.position.y  * canvasScaleFactor) - textBackgroundHeight) + rootCanvasHeightHalved - 7) * -1;
            }
            else {
                newYPos = gameObject.transform.localPosition.y;
            }
            gameObject.transform.localPosition = new Vector3(newXPos, newYPos);

            textBackground.color = new Color32(255, 255, 255, 255);
            titleText.SetColour(titleColourCurrent);
            subtitleText.SetColour(subtitleColourCurrent);
            descriptionText.SetColour(descriptionColourCurrent);
            valueText.SetColour(valueColourCurrent);
        }

        /// <summary>
        /// Makes the tooltip visible or invisible
        /// </summary>
        /// <param name="value"> true to make visible, false otherwise</param>
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
