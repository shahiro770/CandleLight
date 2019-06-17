/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 11, 2019
* 
* The Action class is used to take decisions on the UI.
* The action can be an attack, dialog option, a direction to travel in, or more, depending on the event.
* All actions are interactable (can be clicked and navigated to) by default.
*
*/

using ActionConstants = Constants.ActionConstants;
using Combat;
using Localization;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UIEffects;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Actions {
    public class Action : MonoBehaviour {

        public Attack a { get; private set; }       /// <value> Attack stored if attack </value>
        public Button b { get; private set; }       /// <value> Button component </value>
        public LocalizedText actionText;            /// <value> Text to be displayed </value>
        public string actionType { get; set; }      /// <value> Action type (Attack, flee, undo) </value>
        public bool isUsable { get; private set; } = true;   /// <value> Flag for if action button is usable </value>

        private ButtonTransitionState bts;          /// <value> Button's visual state controller </value>
        private Image i;                            /// <value> Button's sprite </value>
        private TextMeshProUGUI t;                  /// <value> Text display </value>

        /// <summary>
        /// Awake to get all components attached to the Action button
        /// </summary>
        void Awake() {
            b = GetComponent<Button>();
            bts = GetComponent<ButtonTransitionState>();
            i = GetComponent<Image>();
            t = GetComponentInChildren<TextMeshProUGUI>();

            ColorBlock normalBlock = b.colors; 
            ColorBlock unusableBlock = b.colors;
            
            unusableBlock.normalColor = new Color32(196, 36, 48, 255);
            unusableBlock.highlightedColor = new Color32(255, 0, 64, 255);
            unusableBlock.pressedColor = new Color32(120, 36, 48, 255);

            bts.SetColorBlock("normal", normalBlock);
            bts.SetColorBlock("normalAlternate", unusableBlock);
        }

        /// <summary>
        /// Sets the actionType
        /// </summary>
        /// <param name="actionType"> Type of action (flee, undo) </param>
        public void SetAction(string actionType) {
            if (actionType == ActionConstants.FLEE) {
                SetKey("flee_action");
            }
            else if (actionType == ActionConstants.UNDO) {
                SetKey("undo_action");
            } 
            else if (actionType == ActionConstants.NONE) {
                SetKey("none_action");
                SetInteractable(false);
            }

            this.actionType = actionType;
        }

        /// <summary>
        /// Sets the actionType. Overloaded to accept specifically an attack
        /// </summary>
        /// <param name="actionType"> Type of action (attack) </param>
        /// <param name="a"> Attack object to be stored </param>
        public void SetAction(string actionType, Attack a) {
            if (actionType == ActionConstants.ATTACK) {
                if (a.nameKey == "none_attack") {   // disable action if it does nothing
                    this.actionType = "none";
                    SetInteractable(false);
                } 
                else {
                    this.a = a;
                    this.actionType = actionType;
                }
                SetKey(a.nameKey);
            }
        }

        /// <summary>
        /// Change button colour to show it is selected after user navigates away from it after selecting
        /// </summary>
        public void ShowActionSelected() {
            bts.SetColor("pressed");
        }

        /// <summary>
        /// Change button colour back to default
        /// </summary>
        public void ShowActionUnselected() {
            bts.SetColor("normal");
        }

        /// <summary>
        /// Sets the interactivity of the action's button, and handles consequences
        /// </summary>
        /// <param name="value"> Enable interactivity on true and disable on false </param>
        public void SetInteractable(bool value) {
            b.interactable = value;
            i.raycastTarget = value;

            if (value == false && isUsable == true) {
                Debug.Log(actionText.keyText);
                ShowActionUnselected();   
            }            
        }
        
        public bool IsInteractable() {
            return b.interactable;
        }

        public void SetUsable(bool value) {
            isUsable = value;

            if (value == true) {
                bts.SetColor("normal");
            }
            else {
                bts.SetColor("normalAlternate");
            }
        }

        /// <summary>
        /// Sets the text to be displayed
        /// </summary>
        /// <param name="key"> String key that corresponds to dictionary </param>
        public void SetKey(string key) {
            actionText.SetKey(key);
        }
    }
}
