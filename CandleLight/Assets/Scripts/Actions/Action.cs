/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 11, 2019
* 
* The Action class is used to take decisions on the UI.
* The action can be an attack, dialog option, a direction to travel in, or more, depending on the event.
*
*/

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
        public bool isEnabled { get; private set; } = true;     /// <value> Flag for if action button is disabled </value>

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
                this.actionType = "none";
                Disable();
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
                    Disable();
                } else {
                    this.a = a;
                    this.actionType = actionType;
                }
                SetKey(a.nameKey);
            }
        }

        /// <summary>
        /// Change button display to show it is selected after user navigates away from it after selecting
        /// </summary>
        public void SelectAction() {
            bts.SetColor("pressed");
        }

        /// <summary>
        /// Change button display back to default
        /// </summary>
        public void UnselectAction() {
            bts.SetColor("normal");
        }
        
        /// <summary>
        /// Sets the text to be displayed
        /// </summary>
        /// <param name="key"> String key that corresponds to dictionary </param>
        public void SetKey(string key) {
            actionText.SetKey(key);
        }

        /// <summary>
        /// Only disable button interaction, regardless of appearance, prevents the button from being spammed
        /// </summary>
        public void DisableInteraction() {
            b.interactable = false;
        }

        /// <summary>
        /// Disables the button, both visually and functionally
        /// </summary>
        public void Disable() {
            UnselectAction();
            isEnabled = false;          
            b.interactable = false;     // will make normal spriteState show disabled sprite
            i.raycastTarget = false;
        }

        /// <summary>
        /// Disables the button, only functionally
        /// </summary>
        public void FunctionallyDisable() {
            isEnabled = false;          
            b.interactable = false;
            i.raycastTarget = false;
        }

        /// <summary>
        /// Enables the button, both visually and functionally
        /// </summary>
        public void Enable() {
            isEnabled = true;
            b.interactable = true;
            i.raycastTarget = true;
        }
    }
}
