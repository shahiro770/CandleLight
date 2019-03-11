/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: January 23, 2019
* 
* The MainMenu class is the main menu for the game. It allows the user to select
* basic game options, such as starting the game or quitting the game.
*
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {
    
    public Button firstToSelect;    /// <value> First button to select on enabling </value>
    private EventSystem es;         /// <value> eventSystem reference </value>

    /// <summary>
    /// Start function to intialize eventSystem and select first button
    /// </summary> 
    void Awake() {
        es = EventSystem.current; 
    }

    /// <summary>
    /// Enable the main menu, making it visible and interactable
    /// </summary> 
    void OnEnable() {
        if (es != null) {               // weird bug where first to select is not referenced in time
            es.SetSelectedGameObject(firstToSelect.gameObject); 
        }
        firstToSelect.OnSelect(null);   // hack to ensure first button to select is visibly selected
    }

    /// <summary>
    /// Quits the application
    /// </summary> 
    public void QuitGame() {
        Application.Quit();
    }
}