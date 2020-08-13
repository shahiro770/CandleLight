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


using Constants;
using Database;
using Items;
using Localization;
using Party;
using System.Collections; 
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
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
        public SaveData data;                           /// <value> Data that was loaded from a save file</value>
        public string areaName = "GreyWastes";          /// <value> Name of area being explored, which is constant until dlc comes out </value>
        public float canvasWidth = 960;                     /// <value> gameObject positions on the screen are scaled via the canvas, change this number if scaling changes </value>
        public float canvasHeight = 540;                    /// <value> gameObject positions on the screen are scaled via the canvas, change this number if scaling changes </value>
        public float canvasScaleFactor = 1 / 0.01851852f;   /// <value> Factor to scale up position values in code </value>
        public float animationSpeed = 1f;               /// <value> Value that alters the speed of animations </value>
        public int monstersKilled = 0;                  /// <value> Number of monsters killed </value>
        public int WAXobtained = 0;                     /// <value> Amount of WAX obtained (doesn't matter if its spent) </value>
        public int totalEvents = 0;                     /// <value> Total number of events visited </value>
        public bool[] tutorialTriggers = Enumerable.Repeat<bool>(true, System.Enum.GetNames(typeof(TutorialConstants.tutorialTriggers)).Length).ToArray();

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
        /// Load game data, to continue where the player left off
        /// </summary>
        public void LoadGame() {
            string path = Application.persistentDataPath + "/save.cndl";
            if (File.Exists(path)) {
                BinaryFormatter formatter  = new BinaryFormatter();
                FileStream s = new FileStream(path, FileMode.Open);

                data = formatter.Deserialize(s) as SaveData;
                s.Close();

                tutorialTriggers = data.tutorialTriggers;
                
                if (data.tutorialTriggers[(int)TutorialConstants.tutorialTriggers.isTutorial] == false) {
                    PartyManager.instance.LoadData(data);
                }
                // If in the tutorial, game is effecitvely a restart with tutorial on
                else if (areaName == "GreyWastes") {
                    PartyManager.instance.ResetGame();
                    foreach (PartyMemberData pmData in data.partyMemberDatas) {
                        PartyManager.instance.AddPartyMember(pmData.className);
                    }
                }
                StartLoadNextScene("area");
            }
            else {
                Debug.LogError("No save data found");
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
