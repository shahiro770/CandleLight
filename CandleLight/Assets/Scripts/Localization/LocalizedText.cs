/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: January 28, 2019
* 
* LocalizedText is used to display text after getting it from the appropriate language file.
* Must be attached to a TextMeshPro GO.
*
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LocalizedText : MonoBehaviour {
    
    public string key;                 /// <value> global instance </value>
    public TextMeshProUGUI meshText;   /// <value> text mesh pros are not always loaded in time, so store reference to guarantee access </value>

    private string keyText;            /// <value> text retrieved from key </value>
    
    /// <summary>
    /// Start to intialize display text
    /// </summary>
    void Start() {
        if (key != null) {
            meshText.text = LocalizationManager.instance.GetLocalizedValue(key);
            keyText = meshText.text;   
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
    /// Sets the text and display text to null
    /// </summary>
    public void Clear() {
        key = null;
        meshText.text = null;
    }

    /// <summary>
    /// Appends text to the end of the display text
    /// </summary>
    /// <param name="text"> String to append </param>
    public void Append(string text) {
        meshText.text = keyText + " " + text;
    }
}
