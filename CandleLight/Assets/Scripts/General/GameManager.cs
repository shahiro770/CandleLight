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
using UnityEngine;
using UnityEngine.SceneManagement;


namespace General {

    public class GameManager : MonoBehaviour {

        public static GameManager instance;             /// <value> Global instance </value>

        public Camera mainCamera { get; private set; }  /// <value> Cached main camera reference for performance </value>
        public GameDB DB { get; set; }                  /// <value> Access to database to fetch and store information </value>
        public string areaName = "GreyWastes";          /// <value> Name of area being explored </value>
        public float canvasWidth = 960;                     /// <value> gameObject positions on the screen are scaled via the canvas, change this number if scaling changes </value>
        public float canvasHeight = 540;                    /// <value> gameObject positions on the screen are scaled via the canvas, change this number if scaling changes </value>
        public float canvasScaleFactor = 1 / 0.01851852f;   /// <value> Factor to scale up position values in code</value>
        public bool isTutorial = true;                  /// <value> Flag for if the tutorial is enabled (has to be changed from editor) </value>
        public bool isTips = true;                      /// <value> Flag for if helpful tips should show up when possible </value>
        public bool firstConsumable = true;             /// <value> Flag for if the player hasn't encountered their first consumable </value>
        public bool firstCandle = true;                 /// <value> Flag for if the player hasn't encountered their first candle </value> 
        public bool firstShop = true;                   /// <value> Flag for if the player hasn't encountered their first shop </value> 
        public bool firstCandleCombat = true;           /// <value> Flag for if the player hasn't brought a candle into combat for the first time </value>
        public bool firstChampion = true;               /// <value> Flag for if the player hasn't encountered their first champion monster </value>
        public bool firstFailedSkillDisable = true;     /// <value> Flag for if the player fails to disable a skill (due to it being required to enable a column) </value>
        public bool firstFailedSkillEnable = true;      /// <value> Flag for if the player fails to enable a skill (due to have too many active skills) </value>

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
        public void LoadAreaScene() {
            SceneManager.UnloadSceneAsync(activeScene);
            SceneManager.LoadScene(areaScene, LoadSceneMode.Additive);
            activeScene = areaScene;
        }
    }
}
