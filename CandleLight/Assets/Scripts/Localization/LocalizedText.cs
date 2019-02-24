/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: January 28, 2019
* 
* LocalizedText is used to display text after getting it from the appropriate language file.
*
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LocalizedText : MonoBehaviour {
    
    public string key;
    public TextMeshProUGUI meshText; // text mesh pros are not always loaded in time, and this causes problems

    private string keyText;
    
    void Awake() {
        if (key != null) {
            meshText.text = LocalizationManager.instance.GetLocalizedValue(key);
            keyText = meshText.text;   
        }
    }

    public void SetKey(string newKey) {
        key = newKey;
        meshText.text = LocalizationManager.instance.GetLocalizedValue(key);
    }

    public void Clear() {
        key = null;
        meshText.text = null;
    }

    public void Append(string text) {
        meshText.text = keyText + " " + text;
    }
}
