using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour {
    
    public GameObject firstToSelect;
    EventSystem es;

    void OnEnable() {
        es = EventSystem.current;

        es.SetSelectedGameObject(firstToSelect);
        firstToSelect.GetComponent<Button>().OnSelect(null);
    }

     public void QuitGame() {
        Application.Quit();
    }
}


//TO DO:
//Make unity script that refocuses on the last ui element selected in the event user clicks on a non game object