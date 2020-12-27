/*
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
        public GeneralSaveData gsData;                  /// <value> Data that can be saved to a file at any time, unaffecting the current run (for the most part) </value>
        public GeneralSaveData gsDataCurrent;           /// <value> Mutable copy of gsData, effectively the "state" for several game variables </value>
        public RunData rData;                           /// <value> Data that was loaded from a save file regarding a run </value>
        public Sprite[] achievementSprites;             /// <value> Sprite array for all achievement sprites (loaded here cause both area and mainMenu need this) </value>
        public string areaName = "GreyWastes";          /// <value> Name of area being explored, which is constant until dlc comes out </value>
        public float canvasWidth = 960;                     /// <value> gameObject positions on the screen are scaled via the canvas, change this number if scaling changes </value>
        public float canvasHeight = 540;                    /// <value> gameObject positions on the screen are scaled via the canvas, change this number if scaling changes </value>
        public float canvasScaleFactor = 1 / 0.01851852f;   /// <value> Factor to scale up position values in code </value>
        public float timeTaken = -1;                    /// <value> Time spent on the most recent run (-1 means run ended in a loss) </value>
        public int monstersKilled = 0;                  /// <value> Number of monsters killed </value>
        public int WAXobtained = 0;                     /// <value> Amount of WAX obtained (doesn't matter if its spent) </value>
        public int totalEvents = 0;                     /// <value> Total number of events visited </value>

        private string activeScene = "Game";            /// <value> Current scene being displayed </value>
        private string areaScene = "Area";              /// <value> Name of area scene </value>
        private float lerpSpeed = 2f;                   /// <value> Speed at which loading screen fades </value>
        private int tipNum = 11;                        /// <value> (11 loading screen tips) </value>

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
            DB = new GameDB();
            rData = null;
        }

        /// <summary>
        /// Load in general data after all relevant gameobjects have woke up
        /// </summary>
        void Start() {
            Application.targetFrameRate = 60;
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
        public void SaveRunData(RunData rDataToSave) {
            rData = rDataToSave;
            BinaryFormatter formatter  = new BinaryFormatter();
            string path = Application.persistentDataPath + "/save.cndl";
            FileStream s = new FileStream(path, FileMode.Create);

            formatter.Serialize(s, rDataToSave);
            s.Close();
        }

        /// <summary>
        /// Saves data that isn't meant to be cleared 
        /// </summary>
        /// <param name="data"></param>
        public void SaveGeneralData(GeneralSaveData gsDataToSave) {
            ItemData pastItemData = null;
            if (pastItem != null) {
                if (pastItem.type == ItemConstants.CANDLE) {
                    pastItemData = ((Candle)pastItem).GetItemData();
                }
                else {
                    pastItemData = pastItem.GetItemData();
                }
            }
            gsDataToSave.pastItemData = pastItemData;

            if (gsDataToSave.mostMonsters < monstersKilled) {
                gsDataToSave.mostMonsters = monstersKilled;
            }
            else {
                gsDataToSave.mostMonsters = gsData.mostMonsters;
            }
            if (gsDataToSave.mostEvents < totalEvents) {
                gsDataToSave.mostEvents = totalEvents;
            }
            else {
                gsDataToSave.mostEvents = gsData.mostEvents;
            }
            if (gsDataToSave.mostWAX < WAXobtained) {
                gsDataToSave.mostWAX = WAXobtained;
            }
            else {
                gsDataToSave.mostWAX = gsData.mostWAX;
            }
            if (timeTaken != -1 && (timeTaken < gsData.fastestTime || gsData.fastestTime == -1)) {    // only update time if it was faster and the player won
                gsDataToSave.fastestTime = timeTaken;
            }
            else {
                gsDataToSave.fastestTime = gsData.fastestTime;;
            }
            gsData = gsDataToSave;

            BinaryFormatter formatter  = new BinaryFormatter();
            string path = Application.persistentDataPath + "/generalSave.cndl";
            FileStream s = new FileStream(path, FileMode.Create);

            formatter.Serialize(s, gsData);
            s.Close();
        }

        public void SaveGeneralData() {
            if (rData != null) {    // do not update these properties if a run is currently in progress
                gsDataCurrent.aromas = gsData.aromas;
                gsDataCurrent.difficultyModifier = gsData.difficultyModifier;
                gsDataCurrent.mostMonsters = gsData.mostMonsters;
                gsDataCurrent.mostEvents = gsData.mostEvents;
                gsDataCurrent.mostWAX = gsData.mostWAX;
                gsDataCurrent.fastestTime = gsData.fastestTime;
            }

            gsData = new GeneralSaveData(gsDataCurrent);
            GameManager.instance.SaveGeneralData(gsData);
        }

        /// <summary>
        /// Load game data, to continue where the player left off
        /// </summary>
        /// <remark>
        /// Cannot load game if in tutorial
        /// </remark>
        public void LoadRunData() {
            string path = Application.persistentDataPath + "/save.cndl";
            if (File.Exists(path)) {
                BinaryFormatter formatter  = new BinaryFormatter();
                FileStream s = new FileStream(path, FileMode.Open);

                rData = formatter.Deserialize(s) as RunData;
                s.Close();

                gsData.tutorialTriggers = rData.tutorialTriggers;
                gsData.difficultyModifier = rData.difficultyModifier;
                gsData.aromas = rData.aromas;
                PartyManager.instance.LoadData(rData);
                
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

                if (gsData.version != 0.2f) {  // TEMPORARY 
                    SetInitialGeneralData();
                }
                else {
                    UIManager.instance.isTimer = gsData.isTimer;
                    AudioManager.instance.bgmVolume = gsData.bgmVolume;
                    AudioManager.instance.sfxVolume = gsData.sfxVolume;
                    Screen.fullScreen = gsData.isFullscreen;
                    Screen.SetResolution(gsData.resolutionWidth, gsData.resolutionHeight, Screen.fullScreen);

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
                    gsDataCurrent = new GeneralSaveData(gsData);
                }
            }
            else {  // default settings on first load, or if generalSaveData non existance
                SetInitialGeneralData();
            }
        }

        /// <summary>
        /// Set the initial general save data for the run
        /// </summary>
        public void SetInitialGeneralData() {
            gsData = new GeneralSaveData(
                0.2f,   // version
                null,   // past item data
                new HighScoreData[4],
                Enumerable.Repeat<bool>(true, System.Enum.GetNames(typeof(TutorialConstants.tutorialTriggers)).Length).ToArray(),
                Enumerable.Repeat<bool>(false, System.Enum.GetNames(typeof(AchievementConstants.achievementConstants)).Length).ToArray(), 
                Enumerable.Repeat<bool>(false, System.Enum.GetNames(typeof(AromaConstants.aromaConstants)).Length).ToArray(), 
                null,   // partycombos
                false,  // isTimer
                1f,     // scoremodifier
                1f,     // animationspeed
                0.35f,  // bgmvolume
                1f,     // sfxvolume
                false,  // isfullscreen
                1,      // resolution width
                1,      // resolution height
                0.75f,  // difficulty modifier
                0,      // most monsters
                0,      // most wax
                0,      // most events
                -1      // fastest clear time
            );    

            gsData.partyCombos = new string[,] { 
                { ClassConstants.ARCHER, ClassConstants.ARCHER }, 
                { ClassConstants.ARCHER, ClassConstants.MAGE }, 
                { ClassConstants.ARCHER, ClassConstants.ROGUE },
                { ClassConstants.ARCHER, ClassConstants.WARRIOR }, 
                { ClassConstants.MAGE, ClassConstants.MAGE },    
                { ClassConstants.MAGE, ClassConstants.ROGUE },  
                { ClassConstants.MAGE, ClassConstants.WARRIOR },    
                { ClassConstants.ROGUE, ClassConstants.ROGUE }, 
                { ClassConstants.ROGUE, ClassConstants.WARRIOR }, 
                { ClassConstants.WARRIOR, ClassConstants.WARRIOR }, 
            };
            UIManager.instance.isTimer = gsData.isTimer;
            AudioManager.instance.bgmVolume = gsData.bgmVolume;
            AudioManager.instance.sfxVolume = gsData.sfxVolume;
            
            // get and save the initial resolution the app will play at, being full screen by defaultt
            int[ , ] resolutions = new int[,] {
                { 960, 540 }, 
                { 1024, 576 }, 
                { 1152, 648 }, 
                { 1280, 720 }, 
                { 1366, 768 }, 
                { 1600, 900 }, 
                { 1920, 1080 }, 
                { 2560, 1440 },
                { 3840, 2160 }, 
            };
            for (int i = 1; i < resolutions.GetLength(0); i++) {
                if (Screen.resolutions[Screen.resolutions.Length - 1].width <= resolutions[i, 0] && Screen.resolutions[Screen.resolutions.Length - 1].height <= resolutions[i, 1]) {
                    gsData.resolutionWidth = resolutions[i - 1, 0]; 
                    gsData.resolutionHeight = resolutions[i - 1, 1]; 
                    break;
                }  
                else if (i == resolutions.GetLength(0) - 1) {
                    gsData.resolutionWidth = resolutions[resolutions.Length - 1, 0];
                    gsData.resolutionHeight = resolutions[resolutions.Length - 1, 1];
                }
            }
            Screen.fullScreen = false;
            Screen.SetResolution(gsData.resolutionWidth, gsData.resolutionHeight, Screen.fullScreen);

            SaveGeneralData(gsData);
            gsDataCurrent = new GeneralSaveData(gsData);
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
        public void DeleteRunData() {
            string path = Application.persistentDataPath + "/save.cndl";
            if (File.Exists(path)) {
                File.Delete(path);
            }
            rData = null;
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
