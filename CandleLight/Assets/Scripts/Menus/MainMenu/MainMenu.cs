/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: January 23, 2019
* 
* The MainMenu class is the main menu for the game. It allows the user to select
* basic game options, such as starting the game or quitting the game.
*
*/

using UnityEngine;

namespace Menus.MainMenu {

    public class MainMenu : MonoBehaviour {
        
        public ParticleSystem ps;

        private Color defaultColour = new Color32(230, 126, 34, 255);

        void OnEnable() {
            var main = ps.main;
            var colorOverLifetime = ps.colorOverLifetime;
            colorOverLifetime.color = defaultColour;
        }

        /// <summary>
        /// Quits the application
        /// </summary> 
        public void QuitGame() {
            Application.Quit();
        }
    }
}