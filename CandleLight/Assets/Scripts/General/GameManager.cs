/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 23, 2019
* 
* The GameManager class is used as the overarching manager for scenes and other manager objects
* It is held within the Game scene where it can always be accessed globally.
*
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DataBank;

public class GameManager : MonoBehaviour {

    public static GameManager instance;         /// <value> global instance </value>

    public GameDB DB { get; set; }              /// <value> Access to database to fetch and store information </value>
    public string[] monsterNames { get; set; }  /// <value> list of monsters to be instantiated </value>

    private string activeScene = "";            /// <value> current scene being displayed </value>
    private string initialScene = "Loading";
    private string combatScene = "Combat";

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
        activeScene = sceneName;
    }

    /// <summary>
    /// Additively loads combat scene and unload previous scene
    /// </summary> 
    /// <param name="monsterNames"> Names of the monsters to be instantiated </param>
    /// <remark> In future, will need to call respective data saving functions after scene changes </remark>
    public void LoadCombatScene(string[] monsterNames) {
        SceneManager.UnloadSceneAsync(activeScene);
        SceneManager.LoadScene(combatScene, LoadSceneMode.Additive);
        activeScene = combatScene;
        this.monsterNames = monsterNames;
    }
}
