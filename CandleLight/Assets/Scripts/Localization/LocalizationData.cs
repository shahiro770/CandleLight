/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: January 28, 2019
* 
* LocalizationData is used to store text data while parsing it from a JSON language file.
*
*/

[System.Serializable]
public class LocalizationData {
    public LocalizationItem[] texts;
}

[System.Serializable]
public class LocalizationItem {
    public string key;
    public string value;
}

