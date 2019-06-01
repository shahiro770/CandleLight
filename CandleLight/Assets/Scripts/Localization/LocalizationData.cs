/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: January 28, 2019
* 
* LocalizationData is used to store text data while parsing it from a JSON language file.
*
*/

namespace Localization { 
    
    /// <summary>
    /// Holds all of JSON parsed text strings
    /// </summary>
    /// <remark> Serializable to make it easy for unity to store, but this might be unreliable in the future </remark>
    [System.Serializable]
    public class LocalizationData {
        public LocalizationItem[] texts;
    }

    /// <summary>
    /// Key value pairing to store a localized string, using a key to store a string for the correct language
    /// </summary>
    /// <remark> Serializable to make it easy for unity to store, but this might be unreliable in the future </remark>
    [System.Serializable]
    public class LocalizationItem {
        public string key;
        public string value;
    }
}

