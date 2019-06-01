/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: January 28, 2019
* 
* LocalizationManager handles loading the appropriate language file and sending requested texts 
* to LocalizationText components.
* It is held within the Game scene where it can always be accessed globally.
*
*/

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Localization {

    public class LocalizationManager : MonoBehaviour {
        
        public static LocalizationManager instance;                     /// <value> global instance </value>
        
        private Dictionary<string, string> localizedText;               /// <value> Dictionary storing key value pairings of all localized text </value>
        private string missingTextString = "Localized text not found";  /// <value> Error value if key does not exist </value>
        public bool isReady { get; private set; } = false;              /// <value> Localization happens at the start, program loads while waiting </value>

        // Start is called before the first frame update
        /// <summary>
        /// Awake to instantiate singleton
        /// </summary>
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

        /// <summary>
        /// Loads localized text from JSON file into the localizedText dictionary
        /// </summary>
        /// <param name="fileName"> Name of the file to be loaded.false Will vary depending on language </param>
        /// <remark> Only english for now </remark> 
        public void LoadLocalizedText(string fileName) {
            localizedText = new Dictionary<string, string>();
            // StreamingAssetsPath will always be known to unity, regardless of hardware
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

        /// <summary>
        /// Returns text based on a key value
        /// </summary>
        /// <param name="key"> Key to grab localized text from the localizedText dictionary. Keys are same across languages </param>
        /// <returns> 
        /// Result text string if key value is found, error text otherwise
        /// </returns>
        public string GetLocalizedValue(string key) {
            string result = missingTextString;
            if (localizedText.ContainsKey(key)) {
                result = localizedText[key];
            }

            return result;
        }
    }
}
