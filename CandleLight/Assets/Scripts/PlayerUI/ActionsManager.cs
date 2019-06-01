/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 11, 2019
* 
* The ActionManager class is used to organize and initialize Action buttons.
* An action can be an attack, dialog option, a direction to travel in, or more, depending on the event.
*
*/

using Actions;
using Combat;
using Exploration;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PlayerUI {

    public class ActionsManager : MonoBehaviour {

        public CombatManager cm { get; set; }       /// <value> Combat manager to reference other scripts in the combat scene </value>
        public Action[] actions = new Action[5];    /// <value> List of actions, capped at 5 </value>
        
        private EventSystem es;                     /// <value> eventSystem reference </value>
        private Action selectedAction;              /// <value> Action that was selected </value>
        private bool isLeavePossible;                /// <value> Flag for if player can leave scenario </value>

        /// <summary>
        /// Awake to get initialize event system
        /// </summary>
        void Awake() {
            es = EventSystem.current;
        }

        public void Init(bool isLeavePossible) {
            this.isLeavePossible = isLeavePossible;
        }

        /// <summary>
        /// Initialize all actions with interaction actions for exploration
        /// </summary>
        /// <param name="interactions"> List of interactions according to event </param>
        /// <remark> Has yet to be added </remark>
        public void SetInteractionActions(Interaction[] interactions) {

        }

        /// <summary>
        /// Initialize all actions with attack actions for combat
        /// </summary>
        /// <param name="attacks"> List of all attacks according to the partyMember </param>
        /// /// <param name="isFleePossible"> Flag for if event can be fled </param>
        public void SetAttackActions(Attack[] attacks) {
            for (int i = 0; i < attacks.Length; i++) {
                actions[i].SetAction("attack", attacks[i]);
            }
            if (isLeavePossible) {
                actions[actions.Length - 1].SetAction("flee");  // last action will always be flee in combat if allowed
            }
            else {
                actions[actions.Length - 1].SetAction("none");
            }
            SetInitialNavigation();
        }

        /// <summary>
        /// Calls the releveant method depending on the type of action selected
        /// </summary>
        /// <param name="a"> Name of action to be taken </param>
        public void SelectAction(Action a) {
            if (a.actionType == "attack") {
                a.SelectAction();               // attack actions will show which attack is selected while user decides what to do next
                selectedAction = a;
                AttackActionSelected(a.a);
            }
            else if (a.actionType == "undo") {
                UndoAttackActionSelected();
            }
            else if (a.actionType == "flee") {

            }
        }

        /// <summary>
        /// Notifies combat manager that player is going to attack, disabling all attack actions, 
        /// changing button navigation, and changing some button options.
        /// </summary>
        /// <param name="a"> Name of action to be taken </param>
        public void AttackActionSelected(Attack a) {
            for (int i = 0; i < actions.Length - 1;i++) {
                if (actions[i].actionType != "none" && actions[i] != selectedAction) {
                    actions[i].Disable();  
                } 
                if (actions[i] == selectedAction) {
                    actions[i].FunctionallyDisable();
                }
            }
            actions[actions.Length - 1].SetAction("undo");

            cm.PreparePMAttack(a);
        }

        /// <summary>
        /// Reverts UI back to before an attack was selected, enabling all options that were selectable
        /// </summary>
        public void UndoAttackActionSelected() {
            for (int i = 0; i < actions.Length ;i++) {
                if (actions[i].actionType != "none") {
                    actions[i].Enable();  
                }
                if (actions[i] == selectedAction) {
                    actions[i].UnselectAction();
                    selectedAction = null;
                }
            }
            
            if (isLeavePossible) {
                actions[actions.Length - 1].SetAction("flee");
            } else {
                actions[actions.Length - 1].SetAction("none");
            }
            
            cm.UndoPMAction();  // update combat manager to know party members can't attack yet
        }

        /// <summary>
        /// Enable all useable actions
        /// </summary>
        public void EnableAllActions() {
            for (int i = 0; i < actions.Length ;i++) {
                if (actions[i].actionType != "none") {
                    actions[i].Enable();  
                }
                if (actions[i] == selectedAction) {
                    actions[i].UnselectAction();
                    selectedAction = null;
                }
            }

            es.SetSelectedGameObject(actions[0].b.gameObject);  // make event system select first action
        }

        /// <summary>
        /// Disables all useable actions
        /// </summary>
        public void DisableAllActions() {
            for (int i = 0; i < actions.Length;i++) {
                if (actions[i].actionType != "none") {
                    actions[i].Disable();  
                } 
            }
        }

        /// <summary>
        /// Returns the button component for a given action
        /// </summary>
        /// <param name="index"> Index of the button [0 - 4] </param>
        /// <returns> The button component of the action </returns>
        public Button GetActionButton(int index) {
            return actions[index].b;
        }

        /// <summary>
        /// Changes the navigation of an action button
        /// </summary>
        /// <param name="index"> Index of button [0 - 4] </param>
        /// <param name="direction"> Direction to be changed (up, down, left, right) </param>
        /// <param name="b2"> Other button to navigate to </param>
        public void SetButtonNavigation(int index, string direction, Button b2) {
            Button b = actions[index].GetComponent<Button>();
            Navigation n = b.navigation;

            if (direction == "up") {
                n.selectOnUp = b2;
                b.navigation = n;
            }
            else if (direction == "right") {
                n.selectOnRight = b2;
                b.navigation = n;
            }
            else if (direction == "down") {
                n.selectOnDown = b2;
                b.navigation = n;
            }
            else if (direction == "left") {
                n.selectOnLeft = b2;
                b.navigation = n;
            }
        }

        /// <summary>
        /// Resets the navigation of the fifth button (flee, undo)
        /// </summary>
        public void ResetFifthButtonNavigation() {
            Button b = actions[4].GetComponent<Button>();
            Navigation n = b.navigation;

            n.selectOnUp = actions[2].isEnabled ? n.selectOnUp : actions[0].GetComponent<Button>();
            b.navigation = n;
        }

        /// <summary>
        /// Sets up the initial navigation of the action buttons.
        /// Player may have less than 4 action options available, but the fifth button will almost always
        /// have an option, so navigation between above buttons and the fifth button must be adjusted.
        /// </summary>
        /// <remark> In the future, will have to navigate to other UI panels such as items or information </remark>
        private void SetInitialNavigation() {
            for (int i = 0; i < actions.Length; i++) {
                if (actions[i].isEnabled) {
                    Button b = actions[i].GetComponent<Button>();
                    Navigation n = b.navigation;
                    if (i == 0) {
                        n.selectOnDown = actions[2].isEnabled ? n.selectOnDown : actions[4].GetComponent<Button>();
                        n.selectOnRight = actions[1].isEnabled ? n.selectOnRight : null;
                        b.navigation = n;
                    }    
                    else if (i == 1) {
                        n.selectOnDown = actions[3].isEnabled ? n.selectOnDown : actions[4].GetComponent<Button>();
                        b.navigation = n;
                    }
                    /* else if (i == 2) {
                        n.selectOnRight = actions[3].isEnabled ? n.selectOnRight : 
                    } */
                    else if (i == 4) {
                        n.selectOnUp = actions[2].isEnabled ? n.selectOnUp : actions[0].GetComponent<Button>();
                        b.navigation = n;
                    }
                }
            }             
        }
    }
}
