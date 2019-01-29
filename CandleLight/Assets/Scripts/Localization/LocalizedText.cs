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

public class LocalizedText : MonoBehaviour
{
    public string key;
    public TextMeshProUGUI meshText; // text mesh pros are not always loaded in time, and this causes problems

    void Awake() {
        if (key != null) {
            meshText.text = LocalizationManager.instance.GetLocalizedValue(key);   
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
}
