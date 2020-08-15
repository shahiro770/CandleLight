/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: January 28, 2019
* 
* LocalizedText is used to display text after getting it from the appropriate language file.
* Must be attached to a TextMeshPro GO.
*
*/

using TMPro;
using UnityEngine;

namespace Localization {

    public class LocalizedText : MonoBehaviour {
        
        public string key;                  /// <value> Global instance </value>
        public TextMeshProUGUI meshText;    /// <value> Text mesh pros are not always loaded in time, so store reference to guarantee access </value>
 
        [field: SerializeField] private bool textPreset = false;    /// <value> Flag for if text was set before the localizedText awoke </value>

        /// <summary>
        /// Start to intialize display text
        /// </summary>
        void Awake() {
            // do not set key if there is no key or if text was already set
            if (key != null && !textPreset) { 
                meshText.text = LocalizationManager.instance.GetLocalizedValue(key);
            }
        }

        /// <summary>
        /// Sets the display text based on the key
        /// </summary>
        /// <param name="newKey"> String key corresponding to a stored key value pairing in LocalizationManager </param>
        public void SetKey(string newKey) {
            key = newKey;
            meshText.text = LocalizationManager.instance.GetLocalizedValue(key);
        }
        
        /// <summary>
        /// Sets the display text by appending multiple key'd values
        /// </summary>
        /// <param name="newKeys"> List of keys </param>
        public void SetMultipleKeysAndJoin(string[] newKeys) {
            string finalMeshText = "";

            for (int i = 0; i < newKeys.Length; i++) {
                finalMeshText += LocalizationManager.instance.GetLocalizedValue(newKeys[i]) + " ";
            }

            meshText.text = finalMeshText;
            textPreset = true;
        }

        /// <summary>
        /// Sets the display text by appending multiple key'd values with additional text, and appending those strings
        /// </summary>
        /// <param name="newKeys"> List of keys </param>
        /// <param name="texts"> List of texts to append to keys </param>
        public void SetMultipleKeysAndAppend(string[] newKeys, string[] texts) {
            string finalMeshText = "";

            for (int i = 0; i < newKeys.Length; i++) {
                if (newKeys[i] != "none_label") {
                    finalMeshText += LocalizationManager.instance.GetLocalizedValue(newKeys[i]) + " " + texts[i] + "\n";    
                }
            }

            meshText.text = finalMeshText;
            textPreset = true;
        }

        /// <summary>
        /// Sets the text to the inputted value
        /// </summary>
        /// <param name="text"> String to display </param>
        public void SetText(string text) {
            meshText.text = text;
            textPreset = true;
        }

        /// <summary>
        /// Appends the inputted value
        /// </summary>
        /// <param name="text"> String to display </param>
        public void AppendText(string text) {
            meshText.text += text;
            textPreset = true;
        }
        
        /// <summary>
        /// Appends a key'd value to some text
        /// </summary>
        /// <param name="text"></param>
        /// <param name="newKey"></param>
        public void TextAndAppendKey(string text, string newKey) {
            key = newKey;
            meshText.text = text + " " + LocalizationManager.instance.GetLocalizedValue(key);
            textPreset = true;
        }

        /// <summary>
        /// Appends text to the end of the display text
        /// </summary>
        /// <param name="text"> String to append </param>
        public void SetKeyAndAppend(string newKey, string text) {
            key = newKey;
            meshText.text = LocalizationManager.instance.GetLocalizedValue(key) + " " + text;
            textPreset = true;
        }

        /// <summary>
        /// Appends text to the end of the display text
        /// </summary>
        /// <param name="text"> String to append </param>
        public void SetKeyAndAppendNoSpace(string newKey, string text) {
            key = newKey;
            meshText.text = LocalizationManager.instance.GetLocalizedValue(key) + text;
            textPreset = true;
        }

        /// <summary>
        /// Sets the text and display text to null
        /// </summary>
        public void Clear() {
            key = null;
            meshText.text = null;
        }

        /// <summary>
        /// Checks if the text has a key 
        /// </summary>
        /// <returns> True if key, false if keyless </returns>
        public bool HasText() {
            return key != null;
        }

        /// <summary>
        /// Make the font smaller if text is a little long 
        /// Only used in bar.cs to deal with long hp and mp bar numbers
        /// </summary>
        public void ScaleFont() {
            if (meshText.text.Length > 6) {
                meshText.fontSize = 13;
            }
            else {
                meshText.fontSize = 14;
            }
        }

        /// <summary>
        /// Sets the colour of the meshText.
        /// Only use this when alpha needs to be changed.
        /// </summary>
        /// <param name="newColor"> Color32 object (0 to 255 for each param in rgb) </param>
        public void SetColour(Color32 newColour) {
            meshText.color = newColour;
        }

        /// <summary>
        /// Sets the colour of the meshText, with just the hex value.
        /// </summary>
        /// <param name="newColor"> Hex string froc olour </param>
        public void SetColour(string newColour) {
            meshText.text = "<color=" + newColour + ">" + meshText.text + "</color>";
        }
    }
}
