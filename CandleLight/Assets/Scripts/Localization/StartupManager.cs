/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: January 28, 2019
* 
* StartupManager initiates the loading of all text when the game starts.
*
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartupManager : MonoBehaviour {

    // Use this for initialization
    private IEnumerator Start () 
    {
        LocalizationManager.instance.LoadLocalizedText("en.json");
        while (!LocalizationManager.instance.GetIsReady ()) 
        {
            yield return null;
        }

        GameManager.instance.LoadNextScene("MainMenu");
    }

}