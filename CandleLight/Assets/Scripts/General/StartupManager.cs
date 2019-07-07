/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: January 28, 2019
* 
* StartupManager initiates the loading of all text when the game starts.
* In the future, it will be used to load more.
*
*/

using AssetManagers;
using General;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Localization {

    public class StartupManager : MonoBehaviour {

        /// <summary>
        /// Start to wait for the loading of all strings from the JSON file into the LocalizationManager,
        /// AssetBundles from online if being used, and data from the dataManager
        /// </summary>
        /// <returns>
        /// Yields until loading is finished, then has GameManager load the main menu
        /// </returns>
        private IEnumerator Start () {
            LocalizationManager.instance.LoadLocalizedText("en.json");
            while (!LocalizationManager.instance.isReady || !AssetBundleManager.instance.isReady) {
                yield return null;
            }
            DataManager.instance.Init();
            while (!DataManager.instance.isReady) {
                yield return null;
            }
            
            GameManager.instance.LoadNextScene("MainMenu");
        }
    }
}