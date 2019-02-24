/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: January 28, 2019
* 
* LocalizationManager handles loading the appropriate language file and sending requested texts 
* to LocalizationTexts. It is a singleton and is present in all scenes.
*
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LocalizationManager : MonoBehaviour {
    
    public static LocalizationManager instance;
    
    private Dictionary<string, string> localizedText;
    private bool isReady = false;
    private string missingTextString = "Localized text not found";

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null) {
            instance = this;
        }
        else if (instance != this) {
            DestroyImmediate (gameObject);
            instance = this;
        }
    }

    public void LoadLocalizedText(string fileName) {
        localizedText = new Dictionary<string, string>();
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
        if (File.Exists(filePath)) {
            string dataAsJson = File.ReadAllText(filePath);

            // deserialize text from text to a LocalizationData object
            LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(dataAsJson);

            for(int i = 0; i < loadedData.texts.Length; i++) {
                localizedText.Add(loadedData.texts[i].key, loadedData.texts[i].value);
            }
        }
        else {
            // ideally would handle this more gracefully, than just throwing an error (e.g. a pop up)
            Debug.LogError("Cannot find file");
        }
        
        isReady = true;
    }

    public string GetLocalizedValue(string key) {
        string result = missingTextString;
        if (localizedText.ContainsKey(key)) {
            result = localizedText[key];
        }

        return result;
    }

    public bool GetIsReady() {
        return isReady;
    }
}
