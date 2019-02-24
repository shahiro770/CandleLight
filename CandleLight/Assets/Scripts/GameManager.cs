using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DataBank;

public class GameManager : MonoBehaviour {

    public static GameManager instance;

    private string[] monsterNames;
    private string activeScene = "";
    private GameDB DB;

    void Awake() {
        if (instance == null) {
            instance = this;
        }
        else if (instance != this) {
            DestroyImmediate (gameObject);
            instance = this;
        }
        
        activeScene = "Loading";
        SceneManager.LoadScene(activeScene, LoadSceneMode.Additive);
        DB = new GameDB();

    }

    public void LoadNextScene(string sceneName) {
        SceneManager.UnloadSceneAsync(activeScene);
        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        activeScene = sceneName;
    }

    public void LoadCombatScene(string[] monsterNames) {
        SceneManager.UnloadSceneAsync(activeScene);
        SceneManager.LoadScene("Combat", LoadSceneMode.Additive);
        this.monsterNames = monsterNames;
    }

    public GameDB GetDB() {
        return DB;
    }

    public string[] GetMonsterNames() {
        return monsterNames;
    }
}
