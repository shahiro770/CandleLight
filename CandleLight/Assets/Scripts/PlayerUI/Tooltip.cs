/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: June 18, 2019
* 
* The EventDescription gives a description of the current event.
* This can be a prompt for the event, a monster's attack name, and etc.
*
*/

using Characters;
using Localization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerUI {

    public class Tooltip : MonoBehaviour {
        
        public LocalizedText titleText;     /// <value> Text for event </value>
        public LocalizedText subtitleText;
        public LocalizedText descriptionText;
        public Image textBackground;        /// <value> Image behind text </value>
        public Image imageDisplayBackground; /// <value> Largest image around the image that when hovered, will display the tooltip </value>
        public Canvas rootCanvas;

        /// <value> 
        /// gameObject positions on the screen are scaled via the canvas, change this number if scaling changes
        /// </value>
        private float canvasScaleConstant =  1 / 0.01851852f;    
        private float imageDisplayBackgroundWidthHalved;    /// <value> Half the width of whatever image is being displayed </value>
        private float rootCanvasWidthHalved;

        public void SetImageDisplayBackgroundWidth(float width) {
            imageDisplayBackgroundWidthHalved = width * 0.5f;
            rootCanvasWidthHalved = rootCanvas.pixelRect.size.x * 0.5f;
        }

        /// <summary>
        /// Changes the displayed text
        /// </summary>
        /// <param name="textKey"> Localized key for text to display </param>
        public void SetKey(string textKey, string textName) {
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
        /// Changes the displayed text
        /// </summary>
        /// <param name="textKey"> Localized key for text to display </param>
        public void SetAmountText(string textKey, string textName, int amount) {
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
        /// Stop displaying and remove all text
        /// </summary>
        public void ClearText() {
             titleText.Clear();
             subtitleText.Clear();
             descriptionText.Clear();
        }

        private void SetPosition() {
            float textBackgroundWidth = textBackground.rectTransform.sizeDelta.x;

            /* 
             * The way the textBackground (UITile behind the tooltip)'s position is calculated 
             * is from the center of the canvas. This means with a canvas that 960 pixels wide,
             * if the position of the textBackground + its width > 480 (960 / 2), it will be out of the screen.
             * +3 magic number is for spacing.
             */
            if ((textBackground.gameObject.transform.position.x * canvasScaleConstant) + textBackgroundWidth >= rootCanvasWidthHalved) {
                gameObject.transform.localPosition = new Vector3((textBackgroundWidth + imageDisplayBackgroundWidthHalved + 3) * -1, gameObject.transform.localPosition.y);
            }
            else {
                gameObject.transform.localPosition = new Vector3(imageDisplayBackgroundWidthHalved + 3, gameObject.transform.localPosition.y);
            }
        }

        public void SetTooltipVisible(bool value) {
            if (value == true) {
                SetPosition();
            }
             gameObject.SetActive(value);
        }
    }
}
