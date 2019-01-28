/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: January 23, 2019
* 
* ClassButtons are UI Button that the user can click to choose their starting class.
* This class passes up the necessary information to indicate such information and change
* visually to represent that they are selected.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ClassButton : MonoBehaviour {
    
    public Button b;
    public string classString;
    
    private SpriteState bSpriteState;
    private Sprite initial;
    private Sprite highlighted;
    private Sprite pressed;
    private string spriteMode = "NotSelected";
    private string classDescription;
    private Image i;

    void Awake() {
        i = b.GetComponent<Image>();
        bSpriteState = b.GetComponent<Button>().spriteState;

        initial = i.sprite;
        highlighted = bSpriteState.highlightedSprite;
        pressed = bSpriteState.pressedSprite;
        /* if (classString == "Warrior") {
            classDescription = "Warrior\nStudying the ways of the Draken, you've learned how to endure any amount of pain, and deal it all back. Your master";
        } 
        else if (classString== "Mage") {
            classDescription = "You are an appren Mage. The magical properties of candles are your toys. You wish ";
        }
        else if (classString== "Archer") {
            classDescription = "You are an aspiring Archer. Studying the ways of the Draken, you've learned how to use your strength.";
        }
        else if (classString== "Thief") {
           classDescription = "You are a Warrior. Studying the ways of the Draken, you've learned how to use your strength.";
        } */
    }

    public void OnSelect(BaseEventData eventData) {
        b.OnSelect(eventData);
    }

    public string GetClassString() {
        return classString;
    }
    
    // update sprite state when selected or not selected
    public void SetSprite(int mode) {
        if (mode == 0 && spriteMode == "Selected") {
            i.sprite = initial;
            ToggleSpriteState(); 
        }
        else if (mode == 1 && spriteMode == "NotSelected") {
            i.sprite = bSpriteState.pressedSprite;
            ToggleSpriteState();
        }
    }

    // prevent highlighted sprite from appearing on top of the sprite when selected
    public void ToggleSpriteState() {
        if (spriteMode == "NotSelected") {
            bSpriteState.highlightedSprite = pressed;
            b.GetComponent<Button>().spriteState = bSpriteState;
            spriteMode = "Selected";
        } else {
            bSpriteState.highlightedSprite = highlighted;
            b.GetComponent<Button>().spriteState = bSpriteState;
            spriteMode = "NotSelected";
        }
    }
}