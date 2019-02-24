/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: January 23, 2019
* 
* The MainMenu for the game CandleLight. It allows the user to select
* basic game options, such as starting the game or quitting the game.
*
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {
    
    public Button firstToSelect;
    EventSystem es;

    void Start() {
        es = EventSystem.current;
        es.SetSelectedGameObject(firstToSelect.gameObject);
        firstToSelect.OnSelect(null);
    }

    void OnEnable() {
        es = EventSystem.current;
        // weird bug where first to select is not referenced in time
        if (es != null) {
            es.SetSelectedGameObject(firstToSelect.gameObject); 
        }
        firstToSelect.OnSelect(null);
    }

     public void QuitGame() {
        Application.Quit();
    }
}