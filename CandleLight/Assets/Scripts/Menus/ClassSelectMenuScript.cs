/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: January 23, 2019
* 
* The ClassSelectMenuScript is used to control all elements on the class select menu UI.
* It stores information when the user clicks a ClassButton, and visually updates other UI
* gameObjects in it.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClassSelectMenuScript : MonoBehaviour {
   
    public ClassButton firstToSelect; 
    public Button selectButton;         // current selected button
    
    private ClassButton[] classButtons;
    private ColorBlock enabledBlock;        // colours for the select button when enabled
    private ColorBlock disabledBlock;       // // colours for the select button when disabled
    private EventSystem es;
    private string classString = null;      // string of current selected class
    private int classNum = 4;               // number of classes is 4 for now
    private bool selectButtonEnabled = false;

    void Awake() {
        enabledBlock = selectButton.colors; 
        disabledBlock = selectButton.colors;
        
        enabledBlock.normalColor = new Color32(215, 215, 215, 255);
        enabledBlock.highlightedColor = new Color32(255, 255, 255, 255);
        enabledBlock.pressedColor = enabledBlock.highlightedColor;
           
        disabledBlock.normalColor = new Color32(196, 36, 48, 255);
        disabledBlock.highlightedColor = new Color32(255, 0, 64, 255);
        disabledBlock.pressedColor = disabledBlock.highlightedColor;
    }

    void OnEnable() {
        // select the first UI button as specified
        es = EventSystem.current;
        es.SetSelectedGameObject(firstToSelect.b.gameObject);
        firstToSelect.OnSelect(null);
        
        classButtons = GetComponentsInChildren<ClassButton>();
        for (int i = 0; i < classButtons.Length; i++) {
            classButtons[i].SetSprite(0);
        }

        if (selectButtonEnabled) {
            ToggleSelectButton();
        }
    }

    // change class button's sprite state and enable the select button
    public void SelectClassButton(ClassButton cb) {
        SpriteState bSpriteState = cb.GetComponentInChildren<Button>().spriteState;

        for (int i = 0; i < classNum;i++) {
            if (cb != classButtons[i]) {
                classButtons[i].SetSprite(0);
                classString = classButtons[i].GetClassString();
            } else {
                classButtons[i].SetSprite(1);
            }
        }

        if (!selectButtonEnabled) {
            ToggleSelectButton();
        }
    }

    // toggle select button's sprite coloring
    public void ToggleSelectButton() {
        if (!selectButtonEnabled) {
            selectButtonEnabled = true;
            selectButton.colors = enabledBlock;
        }
        else {
            selectButtonEnabled = false;
            selectButton.colors = disabledBlock;
        }
    }

    public void startGame() {
        if (selectButton.enabled) {
            // TO DO: Start the game
        }
    }
}
