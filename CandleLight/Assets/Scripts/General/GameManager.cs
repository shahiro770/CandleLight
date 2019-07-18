/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 23, 2019
* 
* The GameManager class is used as the overarching manager for scenes and other manager objects
* It is held within the Game scene where it can always be accessed globally.
*
*/

using Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace General {

    public class GameManager : MonoBehaviour {

        public static GameManager instance;             /// <value> Global instance </value>

        public Camera mainCamera { get; private set; }  /// <value> Cached main camera reference for performance </value>
        public GameDB DB { get; set; }                  /// <value> Access to database to fetch and store information </value>
        public string areaName { get; set; }            /// <value> Name of area being explored </value>

        private string activeScene = "";                /// <value> Current scene being displayed </value>
        private string initialScene = "Loading";        /// <value> Scene to begin game with </value>
        private string areaScene = "Area";              /// <value> Name of area scene </value>

        /// <summary>
        /// Awake to instantiate singleton
        /// </summary> 
        void Awake() { 
            if (instance == null) {
                instance = this;
            }
            else if (instance != this) {
                DestroyImmediate (gameObject);
                instance = this;
            }
            
            Screen.SetResolution(1920, 1080, false);    // have to figure out resizing, for now game's resolution is fixed
            mainCamera = Camera.main;                   // store reference to camera for other game objects to obtain
            activeScene = initialScene;
            SceneManager.LoadScene(activeScene, LoadSceneMode.Additive);
            DB = new GameDB();
        }

        /// <summary>
        /// Additively load next scene and unload previous scene
        /// </summary> 
        /// <remark> In future, will need to call respective data saving functions after scene changes </remark>
        public void LoadNextScene(string sceneName) {
            SceneManager.UnloadSceneAsync(activeScene);
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
            Resources.UnloadUnusedAssets();
            activeScene = sceneName;
        }

        /// <summary>
        /// Additively loads combat scene and unload previous scene
        /// </summary> 
        /// <param name="monsterNames"> Names of the monsters to be instantiated </param>
        /// <remark> In future, will need to call respective data saving functions after scene changes </remark>
        public void LoadAreaScene(string areaName) {
            SceneManager.UnloadSceneAsync(activeScene);
            SceneManager.LoadScene(areaScene, LoadSceneMode.Additive);
            activeScene = areaScene;
            this.areaName = areaName;
        }
    }
}
