/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: January 28, 2019
* 
* ClassInfo holds the text descriptions of the various classes. It allows for the information
* to be set or cleared.
*
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassInfo : MonoBehaviour
{
    private LocalizedText[] classInfo;
    
    void Awake() {
        classInfo = GetComponentsInChildren<LocalizedText>();
    }

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
