/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: January 28, 2019
* 
* ClassInfo holds the text descriptions of the various classes. It allows for the description
* to be set or cleared.
*
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassInfo : MonoBehaviour {
    
    private LocalizedText[] classInfo;      /// <value> Reference to localized text </value>
    
    /// <summary>
    /// Awake to get references to child components
    /// </summary>
    void Awake() {
        classInfo = GetComponentsInChildren<LocalizedText>();
    }

    /// <summary>
    /// Change the displayed class description
    /// </summary>
    /// <param name="classString"> Changes the class description depending on the class. Null to clear </param>
    public void SetClassInfo(string classString) {
        if (classInfo == null)  {                   // weird behaviour where awake is not called in time
            classInfo = GetComponentsInChildren<LocalizedText>();
        }
        if (classString == null) {
            classInfo[0].Clear();
            classInfo[1].Clear();
        }
        else {
            classInfo[0].SetKey(classString + "_title");
            classInfo[1].SetKey(classString + "_description");
        }
    }
}
