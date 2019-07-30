/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: January 23, 2019
* 
* The MainMenu class is the main menu for the game. It allows the user to select
* basic game options, such as starting the game or quitting the game.
*
*/

using General;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Menus.MainMenu {

    public class MainMenu : MonoBehaviour {
        
        public Button firstToSelect;    /// <value> First button to select on enabling </value>
        private EventSystem es;         /// <value> eventSystem reference </value>

        /// <summary>
        /// Enable the main menu to be viewed and used by the player
        /// </summary> 
        void OnEnable() {
            StartCoroutine(InitEs());
        }

        /// <summary>
        /// EventSystem doesn't enable on time in OnEnable. Have to wait for it to be enabled
        /// before user can reliably navigate UI.
        /// </summary>
        /// <returns> Yields until event system is enabled </returns>
        private IEnumerator InitEs() {
            while (es == null) {
                es = EventSystem.current; 
                yield return null;
            }
            es.SetSelectedGameObject(firstToSelect.gameObject); 
            firstToSelect.OnSelect(null);   // hack to ensure first button to select is visibly selected
        }

        /// <summary>
        /// Quits the application
        /// </summary> 
        public void QuitGame() {
            Application.Quit();
        }
    }
}