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

    void OnEnable() {
        es = EventSystem.current;

        es.SetSelectedGameObject(firstToSelect.gameObject);
        firstToSelect.GetComponent<Button>().OnSelect(null);
    }

     public void QuitGame() {
        Application.Quit();
    }
}