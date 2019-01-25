using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClassSelectMenuScript : MonoBehaviour {
   
    public GameObject firstToSelect;
    //GameObject lastSelected = firstSelected;
    EventSystem es;

    void OnEnable() {
        es = EventSystem.current;
        es.SetSelectedGameObject(firstToSelect);
        firstToSelect.GetComponent<Button>().OnSelect(null);
    }
}
