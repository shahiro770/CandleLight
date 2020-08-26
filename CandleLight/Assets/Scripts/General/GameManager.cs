﻿/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 23, 2019
* 
* The GameManager class is used as the overarching manager for scenes and other manager objects
* It is held within the Game scene where it can always be accessed globally.
* IMPORTANT: Need to change tutorialTriggers in editor as well if they're modified (its reatrded cause they need to be serialized)
*
*/


using Audio;
using Constants;
using Database;
using Items;
using Localization;
using Party;
using System.Collections; 
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UIManager = PlayerUI.UIManager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace General {

    public class GameManager : MonoBehaviour {

        public static GameManager instance;             /// <value> Global instance </value>
        
        /* external comp refs */
        public CanvasGroup loadingCanvas;
        public GameObject loadingScreen;
        public Image loadingBarFill; 
        public LocalizedText tipText;
        public SpriteRenderer loadingFlameSpriteRenderer;
        
        public Camera mainCamera { get; private set; }  /// <value> Cached main camera reference for performance </value>
        public GameDB DB { get; set; }                  /// <value> Access to database to fetch and store information </value>
        public Item pastItem;                           /// <value> Item stored from previous run under special condition </value>
        public GeneralSaveData gsData;                  /// <value> Data that cannot be cleared after a run ends </value>
        public SaveData data;                           /// <value> Data that was loaded from a save file regarding a run </value>
        public Sprite[] achievementSprites;             /// <value> Sprite array for all achievement sprites (loaded here cause both area and mainMenu need this) </value>
        public string areaName = "GreyWastes";          /// <value> Name of area being explored, which is constant until dlc comes out </value>
        public float canvasWidth = 960;                     /// <value> gameObject positions on the screen are scaled via the canvas, change this number if scaling changes </value>
        public float canvasHeight = 540;                    /// <value> gameObject positions on the screen are scaled via the canvas, change this number if scaling changes </value>
        public float canvasScaleFactor = 1 / 0.01851852f;   /// <value> Factor to scale up position values in code </value>
        public float animationSpeed;                    /// <value> Value that alters the speed of animations </value>
        public float timeTaken = -1;                    /// <value> Time spent on the most recent run (-1 means run ended in a loss) </value>
        public int enemiesKilled = 0;                   /// <value> Number of monsters killed </value>
        public int WAXobtained = 0;                     /// <value> Amount of WAX obtained (doesn't matter if its spent) </value>
        public int totalEvents = 0;                     /// <value> Total number of events visited </value>
        public bool[] tutorialTriggers = Enumerable.Repeat<bool>(true, System.Enum.GetNames(typeof(TutorialConstants.tutorialTriggers)).Length).ToArray();
        public bool[] achievementsUnlocked = Enumerable.Repeat<bool>(false, System.Enum.GetNames(typeof(TutorialConstants.tutorialTriggers)).Length).ToArray();

        private string activeScene = "Game";            /// <value> Current scene being displayed </value>
        private string areaScene = "Area";              /// <value> Name of area scene </value>
        private float lerpSpeed = 2f;                   /// <value> Speed at which loading screen fades </value>
        private int tipNum = 11;                        /// <value> (11 loading screen tips) </value>

        /// <summary>
        /// Awake to instantiate singleton
        /// </summary> 
        void Awake() { 
            Application.targetFrameRate = 60;

            if (instance == null) {
                instance = this;
            }
            else if (instance != this) {
                DestroyImmediate (gameObject);
                instance = this;
            }
            
            mainCamera = Camera.main;                   // store reference to camera for other game objects to obtain
            DB = new GameDB();
            data = null;
            
        }

        /// <summary>
        /// Load in general data after all relevant gameobjects have woke up
        /// </summary>
        void Start() {
            LoadAchievementSprites();
            LoadGeneralData();
        }

        /// <summary>
        /// Start to load the next scene (start the coroutine here to not interrupt any loading)
        /// </summary>
        /// <param name="sceneName"></param>
        public void StartLoadNextScene(string sceneName) {
            StartCoroutine(LoadNextScene(sceneName));
        }

        /// <summary>
        /// Additively load next scene and unload previous scene
        /// </summary> 
        /// <remark> In future, will need to call respective data saving functions after scene changes </remark>
        public IEnumerator LoadNextScene(string sceneName) {   
            SetTipText();
            loadingBarFill.fillAmount = 0;
            yield return StartCoroutine(FadeLoadingScreen(1));
            if (activeScene != "Game") {
                SceneManager.UnloadSceneAsync(activeScene);
            }

            AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            while (loadOp.isDone == false) {
                loadingBarFill.fillAmount = Mathf.Clamp01(loadOp.progress / 0.9f);
                yield return null;
            } 

            yield return StartCoroutine(FadeLoadingScreen(0));
            
            Resources.UnloadUnusedAssets();
            activeScene = sceneName;
        } 

        private void SetTipText() {
            tipText.SetKey("tip" + Random.Range(0, tipNum) + "_des");
        }

        /// <summary>
        /// Saves the game's data
        /// </summary>
        /// <param name="data"></param>
        public void SaveGame(SaveData data) {
            BinaryFormatter formatter  = new BinaryFormatter();
            string path = Application.persistentDataPath + "/save.cndl";
            FileStream s = new FileStream(path, FileMode.Create);

            formatter.Serialize(s, data);
            s.Close();
        }

        /// <summary>
        /// Saves data that isn't meant to be cleared 
        /// </summary>
        /// <param name="data"></param>
        public void SaveGeneralData(GeneralSaveData gsData) {
            ItemData pastItemData = null;
            if (pastItem != null) {
                if (pastItem.type == ItemConstants.CANDLE) {
                    pastItemData = ((Candle)pastItem).GetItemData();
                }
                else {
                    pastItemData = pastItem.GetItemData();
                }
            }
            gsData.pastItemData = pastItemData;
            if (this.gsData.mostEnemies < enemiesKilled) {
                gsData.mostEnemies = enemiesKilled;
            }
            else {
                gsData.mostEnemies = this.gsData.mostEnemies;
            }
            if (this.gsData.mostEvents < totalEvents) {
                gsData.mostEvents = totalEvents;
            }
            else {
                gsData.mostEvents = this.gsData.mostEvents;
            }
            if (this.gsData.mostWAX < WAXobtained) {
                gsData.mostWAX = WAXobtained;
            }
            else {
                gsData.mostWAX = this.gsData.mostWAX;
            }
            if (timeTaken != -1 && (timeTaken < this.gsData.fastestTime || this.gsData.fastestTime == -1)) {    // only update time if it was faster and the player won
                gsData.fastestTime = timeTaken;
            }
            else {
                gsData.fastestTime = this.gsData.fastestTime;;
            }
            this.gsData = gsData;

            BinaryFormatter formatter  = new BinaryFormatter();
            string path = Application.persistentDataPath + "/generalSave.cndl";
            FileStream s = new FileStream(path, FileMode.Create);

            formatter.Serialize(s, gsData);
            s.Close();
        }

        /// <summary>
        /// Load game data, to continue where the player left off
        /// </summary>
        /// <remark>
        /// Cannot load game if in tutorial
        /// </remark>
        public void LoadGame() {
            string path = Application.persistentDataPath + "/save.cndl";
            if (File.Exists(path)) {
                BinaryFormatter formatter  = new BinaryFormatter();
                FileStream s = new FileStream(path, FileMode.Open);

                data = formatter.Deserialize(s) as SaveData;
                s.Close();

                tutorialTriggers = data.tutorialTriggers;
                PartyManager.instance.LoadData(data);
                
                StartLoadNextScene("area");
            }
            else {
                Debug.LogError("No save data found");
            }
        }

        /// <summary>
        /// Load general data (highscores, settings, etc.)
        /// </summary>
        public void LoadGeneralData() {
            string path = Application.persistentDataPath + "/generalSave.cndl";
            if (File.Exists(path)) {
                BinaryFormatter formatter  = new BinaryFormatter();
                FileStream s = new FileStream(path, FileMode.Open);

                gsData = formatter.Deserialize(s) as GeneralSaveData;
                s.Close();

                tutorialTriggers = gsData.tutorialTriggers;
                animationSpeed = gsData.animationSpeed;
                UIManager.instance.isTimer = gsData.isTimer;
                AudioManager.instance.bgmVolume = gsData.bgmVolume;
                AudioManager.instance.sfxVolume = gsData.sfxVolume;
                if (gsData.pastItemData != null) {
                    if (gsData.pastItemData.type == ItemConstants.GEAR) {
                        pastItem = new Gear(gsData.pastItemData);
                    }
                    else if (gsData.pastItemData.type == ItemConstants.CANDLE) {
                        pastItem = new Candle(gsData.pastItemData);
                    }
                    else {
                        pastItem = new Special(gsData.pastItemData);
                    }
                }
            }
            else {  // default settings on first load, or if generalSAveData non existance
                gsData = new GeneralSaveData(null, new HighScoreData[4], Enumerable.Repeat<bool>(true, System.Enum.GetNames(typeof(TutorialConstants.tutorialTriggers)).Length).ToArray(),
                Enumerable.Repeat<bool>(false, System.Enum.GetNames(typeof(TutorialConstants.tutorialTriggers)).Length).ToArray(), 
                false, 0.5f, 1f, 1, 0, 0, 0, -1);
                achievementsUnlocked = gsData.achievementsUnlocked;
                tutorialTriggers = gsData.tutorialTriggers;
                animationSpeed = gsData.animationSpeed;               
                UIManager.instance.isTimer = gsData.isTimer;
                AudioManager.instance.bgmVolume = gsData.bgmVolume;
                AudioManager.instance.sfxVolume = gsData.sfxVolume;
                pastItem = null;
            }
        }

        /// <summary>
        /// Load achievement sprites
        /// </summary>
        public void LoadAchievementSprites() {
            achievementSprites = new Sprite[5];
            for (int i = 0; i < achievementSprites.Length; i++) {
                achievementSprites[i] = Resources.Load<Sprite>("Sprites/Achievements/" + i);
            }
        }
        
        /// <summary>
        /// Delete run-specific save data
        /// </summary>
        public void DeleteSaveData() {
            string path = Application.persistentDataPath + "/save.cndl";
            if (File.Exists(path)) {
                File.Delete(path);
            }
            data = null;
        }

        /// <summary>
        /// Adds a highscore to the highscore data if it is high enough
        /// </summary>
        /// <param name="score"> Score amount </param>
        /// <param name="subAreaIndex"> SubArea the player got to before the run ended </param>
        public void AddHighScoreData(int score, int subAreaIndex) {
            string[] partyComposition = PartyManager.instance.GetPartyComposition();
            for (int i = 0; i < gsData.hsds.Length; i++) {
                if (gsData.hsds[i] == null) {
                    gsData.hsds[i] = new HighScoreData(areaName, partyComposition[0], partyComposition[1], score, subAreaIndex);
                    break;
                }
                else if (score > gsData.hsds[i].score) {
                    for (int j = gsData.hsds.Length - 1; j > i; j--) {
                        gsData.hsds[j] = gsData.hsds[j - 1];
                    }
                    gsData.hsds[i] = new HighScoreData(areaName, partyComposition[0], partyComposition[1], score, subAreaIndex);
                    break;
                }
            }
        }

        /// <summary>
        /// Changes the alpha of the loading screen to the target value, and sets a random tip
        /// </summary>
        /// <param name="targetAlpha"> Int 0 or 1 </param>
        /// <returns> IEnumerator for smooth animation </returns>
        private IEnumerator FadeLoadingScreen(int targetAlpha) {
            float timeStartedLerping = Time.time;
            float timeSinceStarted = Time.time - timeStartedLerping;
            float percentageComplete = timeSinceStarted * lerpSpeed;
            float prevAlpha = loadingCanvas.alpha;
            float newAlpha;

            if (targetAlpha == 1) {
                loadingScreen.SetActive(true);
            }
            
            while (loadingCanvas.alpha != targetAlpha) {
                timeSinceStarted = Time.time - timeStartedLerping;
                percentageComplete = timeSinceStarted * lerpSpeed;

                newAlpha = Mathf.Lerp(prevAlpha, targetAlpha, percentageComplete);

                loadingCanvas.alpha = newAlpha;
                loadingFlameSpriteRenderer.color = new Color(loadingFlameSpriteRenderer.color.r, loadingFlameSpriteRenderer.color.g, loadingFlameSpriteRenderer.color.b, newAlpha);

                yield return new WaitForEndOfFrame();
            }
            
            if (targetAlpha == 0) {       
                loadingScreen.SetActive(false);
            }
        }
    }
}
