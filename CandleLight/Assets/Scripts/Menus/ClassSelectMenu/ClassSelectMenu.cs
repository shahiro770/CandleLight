/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: January 23, 2019
* 
* The ClassSelectMenu class is used to control all elements on the class select menu UI.
* It is the screen where the user chooses their class (Warrior, Mage, Archer, Thief) to start the
* game with.
* It stores information when the user clicks a ClassButton, and visually updates other UI
* gameObjects in it.
*
*/

using Party;
using General;
using System.Collections;
using System.Collections.Generic;
using UIEffects;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Menus.ClassSelectMenu {

    public class ClassSelectMenu : MonoBehaviour {
    
        public Button selectButton;                 /// <value> Confirmation button  </value>
        public ClassButton firstToSelect;           /// <value> First button to select on enabling </value>
        public ClassInfo classInfo;                 /// <value> Displays information of a class </value>
        
        private EventSystem es;                     /// <value> eventSystem reference </value>
        private ButtonTransitionState sbBts;        /// <value> Confirmation button's visual state controller </value>   
        private ClassButton[] classButtons;         /// <value> List of all class buttons </value>
        private string classString = null;          /// <value> name of currently selected class </value>
        private int classNum = 4;                   /// <value> Number of classes is 4 for now </value>
        private bool selectButtonEnabled = false;   /// <value> Select button will only move to next scene if a class is selected </value>

        /// <summary>
        /// Awake to intialize eventSystem and select button's alternate colour blocks
        /// </summary> 
        void Awake() {
            es = EventSystem.current;
            classButtons = GetComponentsInChildren<ClassButton>();

            ColorBlock sbEnabledBlock = selectButton.colors; 
            sbEnabledBlock.normalColor = new Color32(215, 215, 215, 255);
            sbEnabledBlock.highlightedColor = new Color32(255, 255, 255, 255);
            sbEnabledBlock.pressedColor = sbEnabledBlock.highlightedColor;

            sbBts = selectButton.GetComponent<ButtonTransitionState>();
            sbBts.SetColorBlock("normalAlternate", sbEnabledBlock);
        }

        /// <summary>
        /// OnEnable to select first class button and revert previous selections due to switching menus
        /// </summary> 
        void OnEnable() {
            es.SetSelectedGameObject(firstToSelect.b.gameObject);
            firstToSelect.OnSelect(null);   // hack to ensure first button to select is visibly selected
            
            StartCoroutine(SetupButtons());
            classString = null;
            classInfo.SetClassInfo(classString);
            if (selectButtonEnabled) {
                ToggleSelectButton();
            }
        }
        
        /// <summary>
        /// Reset class button sprites back to their default unselected sprites
        /// </summary> 
        private IEnumerator SetupButtons() {
            for (int i = 0; i < classButtons.Length; i++) {
                while (!classButtons[i].isReady) {  // hack to prevent unity from setting sprites on not-awake'd buttons
                    yield return null;
                }
                classButtons[i].SetSprite("normal");
            }
        }

        /// <summary>
        /// Visually show that a class has been selected.
        /// Changes a class button's sprite state to pressed and enables the select button.
        /// Also changes the displayed class info.
        /// </summary> 
        /// <param name="cb"> Class button to change appearance of </param>
        public void SelectClassButton(ClassButton cb) {
            SpriteState bSpriteState = cb.GetComponentInChildren<Button>().spriteState;

            for (int i = 0; i < classNum; i++) {
                if (cb != classButtons[i]) {
                    classButtons[i].SetSprite("normal");
                } else {
                    classButtons[i].SetSprite("pressed");
                    classString = classButtons[i].GetClassString();
                    classInfo.SetClassInfo(classString); 
                }
            }

            if (!selectButtonEnabled) {
                ToggleSelectButton();
            }
        }

        /// <summary>
        /// Toggle select button's colouring to show if its enabled or disabled
        /// </summary> 
        public void ToggleSelectButton() {
            if (!selectButtonEnabled) {
                selectButtonEnabled = true;
                sbBts.SetColor("normalAlternate");
            }
            else {
                selectButtonEnabled = false;
                sbBts.SetColor("normal");
            }
        }

        /// <summary>
        /// Starts the game by loading the next world map scene
        /// </summary> 
        /// <remark> TODO: World map scene still has to be made </remark>
        public void BeginGame() {
            if (selectButtonEnabled) {
                PartyManager.instance.ClearPartyMembers();
                PartyManager.instance.AddPartyMember("Mage");
                PartyManager.instance.AddPartyMember("Archer");
                GameManager.instance.LoadAreaScene("GreyWastes");
            }
        }
    }
}
