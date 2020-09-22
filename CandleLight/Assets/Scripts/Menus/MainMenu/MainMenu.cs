/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: January 23, 2019
* 
* The MainMenu class is the main menu for the game. It allows the user to select
* basic game options, such as starting the game or quitting the game.
*
*/

using Audio;
using GameManager = General.GameManager;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.UI;
using System.IO;

namespace Menus.MainMenu {

    public class MainMenu : MonoBehaviour {
        
        public ParticleSystem ps;
        public Button continueButton;
        public ClassSelectMenu.ClassSelectMenu csm;
        public Canvas sceneCanvas;
        public Light2D freeForm;
        public Light2D particleLight;

        private Color defaultColour = new Color32(230, 126, 34, 255);

        void Start() {
            Camera mainCamera = Camera.main;
            sceneCanvas.worldCamera = mainCamera;
            AudioManager.instance.PlayBGM("mainMenu");
        }

        void OnEnable() {
            var main = ps.main;
            var colorOverLifetime = ps.colorOverLifetime;
            colorOverLifetime.color = defaultColour;
            freeForm.color = defaultColour;
            particleLight.color = defaultColour;

            // continue button is only enabled if save data exists
            string path = Application.persistentDataPath + "/save.cndl";
            if (File.Exists(path)) {
                continueButton.interactable = true;
            }
            else {
                continueButton.interactable = false;
            }
        }

        /// <summary>
        /// Continue the saved run
        /// </summary>
        public void ContinueGame() {
            string path = Application.persistentDataPath + "/save.cndl";
            if (File.Exists(path)) {
                GameManager.instance.LoadRunData();
            }
        }

        /// <summary>
        /// Quits the application
        /// </summary> 
        public void QuitGame() {
            Application.Quit();
        }
    }
}