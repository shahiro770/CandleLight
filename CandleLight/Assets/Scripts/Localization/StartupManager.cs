/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: January 28, 2019
* 
* StartupManager initiates the loading of all text when the game starts.
* In the future, it will be used to load more.
*
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartupManager : MonoBehaviour {

    /// <summary>
    /// Star to wait for the loading of all strings from the JSON file into the LocalizationManager
    /// </summary>
    /// <returns>
    /// Yields until loading is finished, then has GameManager load the main menu
    /// </returns>
    private IEnumerator Start () 
    {
        LocalizationManager.instance.LoadLocalizedText("en.json");
        while (!LocalizationManager.instance.isReady) 
        {
            yield return null;
        }

        GameManager.instance.LoadNextScene("MainMenu");
    }

}