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
        /// Sets the text to the inputted value
        /// </summary>
        /// <param name="text"> String to display </param>
        public void SetText(string text) {
            meshText.text = text;
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
        /// Sets the text and display text to null
        /// </summary>
        public void Clear() {
            key = null;
            meshText.text = null;
        }

        /// <summary>
        /// Sets text for a partyMember taking damage
        /// </summary>
        /// <param name="text"> name of PartyMember </param>
        /// <param name="amount"> Amount of damage taken </param>
        public void SetDamageText(string text, int amount) {
            meshText.text = text +  " " + LocalizationManager.instance.GetLocalizedValue("lost_word_event") + " " + amount.ToString() + " HP";
        }
    }
}
