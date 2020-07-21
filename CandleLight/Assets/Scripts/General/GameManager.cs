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
using Localization;
using System.Collections; 
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
        public string areaName = "GreyWastes";          /// <value> Name of area being explored </value>
        public float canvasWidth = 960;                     /// <value> gameObject positions on the screen are scaled via the canvas, change this number if scaling changes </value>
        public float canvasHeight = 540;                    /// <value> gameObject positions on the screen are scaled via the canvas, change this number if scaling changes </value>
        public float canvasScaleFactor = 1 / 0.01851852f;   /// <value> Factor to scale up position values in code</value>
        public float animationSpeed = 1f;               /// <value> Value that alters the speed of animations</value>
        public bool isTutorial = true;                  /// <value> Flag for if the tutorial is enabled (has to be changed from editor) </value>
        public bool isTips = true;                      /// <value> Flag for if helpful tips should show up when possible </value>
        public bool firstConsumable = true;             /// <value> Flag for if the player hasn't encountered their first consumable </value>
        public bool firstCandle = true;                 /// <value> Flag for if the player hasn't encountered their first candle </value> 
        public bool firstShop = true;                   /// <value> Flag for if the player hasn't encountered their first shop </value> 
        public bool firstCandleCombat = true;           /// <value> Flag for if the player hasn't brought a candle into combat for the first time </value>
        public bool firstChampion = true;               /// <value> Flag for if the player hasn't encountered their first champion monster </value>
        public bool firstFailedSkillDisable = true;     /// <value> Flag for if the player fails to disable a skill (due to it being required to enable a column) </value>
        public bool firstFailedSkillEnable = true;      /// <value> Flag for if the player fails to enable a skill (due to have too many active skills) </value>

        private string activeScene = "Game";            /// <value> Current scene being displayed </value>
        private string areaScene = "Area";              /// <value> Name of area scene </value>
        private float lerpSpeed = 2f;                   /// <value> Speed at which loading screen fades </value>
        private int tipNum = 11;                        /// <value> (11 tips) </value>

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
