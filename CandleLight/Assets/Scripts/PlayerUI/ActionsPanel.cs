/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 11, 2019
* 
* The ActionPanel class is used to organize and initialize Action buttons.
* An action can be an attack, dialog option, a direction to travel in, or more, depending on the event.
*
*/

using ActionConstants = Constants.ActionConstants;
using Actions;
using Characters;
using Combat;
using Events;
using PanelConstants = Constants.PanelConstants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PlayerUI {

    public class ActionsPanel : Panel {
        
        public CombatManager cm { get; set; }       /// <value> Combat manager to reference other scripts in the combat scene </value>
        public Action[] actions = new Action[5];    /// <value> List of actions, capped at 5 </value>
        
        private EventSystem es;                     /// <value> eventSystem reference </value>
        private Action selectedAction;              /// <value> Action that was selected </value>
        private bool isLeavePossible;               /// <value> Flag for if player can leave scenario </value>

        /// <summary>
        /// Awake to get initialize event system
        /// </summary>
        void Awake() {
            es = EventSystem.current;
        }
        
        /// <summary>
        /// Initializes the actionsPanel, setting fields reflective of the overall event (e.g. if it can be fled)
        /// </summary>
        /// <param name="isLeavePossible"> True if the player can leave the event, false otherwise </param>
        public void Init(bool isLeavePossible) {
            this.isLeavePossible = isLeavePossible;
        }

        /// <summary>
        /// Initialize all actions with interaction actions for exploration
        /// </summary>
        /// <param name="interactions"> List of interactions according to event </param>
        /// <remark> Has yet to be added </remark>
        public void SetInteractionActions(Interaction[] interactions) { 
            for (int i = 0; i < interactions.Length; i++) {
                actions[i].SetAction(ActionConstants.INTERACTION, interactions[i]);
            }
            
            if (isLeavePossible) {
                // last action will always be leave-related if allowed
                actions[actions.Length - 1].SetAction(ActionConstants.TRAVEL, interactions[actions.Length - 1].nameKey);
                actions[actions.Length - 1].SetInteractable(true); 
            }
            else {
                actions[actions.Length - 1].SetInteractable(false);
            }

            SetAllActionsInteractable();
            SetInitialNavigation();
        }

        /// <summary>
        /// Initialize all actions with the partyMembers' attacks for combat
        /// </summary>
        /// <param name="attacks"> List of all attacks according to the partyMember </param>
        public void SetAttackActions(Attack[] attacks) {
            for (int i = 0; i < attacks.Length; i++) {
                actions[i].SetAction(ActionConstants.ATTACK, attacks[i]);
            }
            if (isLeavePossible) {
                actions[actions.Length - 1].SetAction(ActionConstants.FLEE);  // last action will always be flee in combat if allowed
            }
            else {
                actions[actions.Length - 1].SetAction(ActionConstants.NONE);
            }

            SetInitialNavigation();
        }

        /// <summary>
        /// Display all the actions of a partyMember and their usability
        /// </summary>
        /// <param name="pm"></param>
        public void DisplayPartyMember(PartyMember pm) {
            SetAttackActions(pm.attacks);
            SetAllActionsInteractable();
            CheckAndSetActionsToUnusable(pm.CMP, pm.CHP);
        }

        /// <summary>
        /// Calls the releveant method depending on the type of action selected
        /// </summary>
        /// <param name="a"> Name of action to be taken </param>
        public void SelectAction(Action a) {
            if (a.actionType == ActionConstants.ATTACK && a.isUsable) {
                a.ShowActionSelected();  // attack actions will show which attack is selected while user decides what to do next
                selectedAction = a;
                AttackActionSelected(a.a);
            }
            else if (a.actionType == ActionConstants.UNDO) {
                UndoAttackActionSelected();
            }
            else if (a.actionType ==  ActionConstants.FLEE || (a.actionType == ActionConstants.TRAVEL)) {
                EventManager.instance.GetNextEvent(a.i);
            }
        }

        /// <summary>
        /// Notifies combat manager that player is going to attack, disabling all attack actions, 
        /// changing button navigation, and changing some button options.
        /// </summary>
        /// <param name="a"> Name of action to be taken </param>
        public void AttackActionSelected(Attack a) {
            for (int i = 0; i < actions.Length - 1;i++) {
                if (actions[i].actionType != ActionConstants.NONE) {
                    actions[i].SetInteractable(false);  
                } 
                if (actions[i] == selectedAction) {
                    actions[i].ShowActionSelected();
                }
            }
            actions[actions.Length - 1].SetAction(ActionConstants.UNDO);

            cm.PreparePMAttack(a);
        }

        /// <summary>
        /// Reverts UI back to before an attack was selected, enabling all options that were selectable
        /// </summary>
        public void UndoAttackActionSelected() {
            for (int i = 0; i < actions.Length ;i++) {
                if (actions[i].actionType != ActionConstants.NONE) {
                    actions[i].SetInteractable(true);  
                }
                if (actions[i] == selectedAction) {
                    actions[i].ShowActionUnselected();
                    selectedAction = null;
                }
            }

            if (isLeavePossible) {
                actions[actions.Length - 1].SetAction(ActionConstants.FLEE);
            } else {
                actions[actions.Length - 1].SetAction(ActionConstants.NONE);
            }

            ResetFifthButtonNavigation();

            cm.UndoPMAction();  // update combat manager to know party members can't attack yet
        }

        /// <summary>
        /// Enable all non-none actions to be interacted with using mouse or keys
        /// </summary>
        public void SetAllActionsInteractable() {
            int firstInteractableIndex = 0;

            for (int i = 0; i < actions.Length ;i++) {
                if (actions[i].actionType != ActionConstants.NONE) {
                    actions[i].SetInteractable(true);  
                    firstInteractableIndex = i;
                }
            }

            es.SetSelectedGameObject(actions[firstInteractableIndex].b.gameObject);  // make event system select first selectable action
        }

        /// <summary>
        /// Prevents all non-none actions from being interacted with
        /// </summary>
        public void SetAllActionsUninteractable() {
            for (int i = 0; i < actions.Length; i++) {
                if (actions[i].actionType != ActionConstants.NONE) {
                    actions[i].SetInteractable(false);  
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
        /// Sets an action to visually be unusable if the partyMember can't use it
        /// </summary>
        /// <param name="CMP"> Current HP of partyMember </param>
        /// <param name="CHP"> Current MP of partyMember </param>
        /// <remark> Only affects combat for now </remark>
        private void CheckAndSetActionsToUnusable(int CMP, int CHP) {
             for (int i = 0; i < actions.Length - 1; i++) {
                if (actions[i].actionType == "attack") {
                    Attack a = actions[i].a;

                    if (a.costType == "MP") {
                        if (a.cost > CMP) {
                            actions[i].SetUsable(false);
                        }
                        else {
                            actions[i].SetUsable(true);
                        }
                    }
                    else if (a.costType == "HP") {
                        if (a.cost > CHP) {
                            actions[i].SetUsable(false);
                        }
                        else {
                            actions[i].SetUsable(true);
                        }
                    }
                } 
            } 
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
        /// Sets the horizontal navigation to navigate to buttons that are not in the actionsPanel
        /// </summary>
        /// <param name="p"> Other panel </param>
        public void SetHorizontalNavigation(Panel p) {
            if (p.GetPanelName() == PanelConstants.PARTYPANEL) {
                if (actions[3].IsInteractable()) {
                    SetButtonNavigation(3, "right", p.GetNavigatableButton());
                }
                else if (actions[2].IsInteractable()) {
                    SetButtonNavigation(2, "right", p.GetNavigatableButton());
                }
                if (actions[1].IsInteractable()) {
                    SetButtonNavigation(1, "right", p.GetNavigatableButton());
                }
                else if (actions[0].IsInteractable()) {
                    SetButtonNavigation(0, "right", p.GetNavigatableButton());
                }
                if (actions[4].IsInteractable()) {
                    SetButtonNavigation(4, "right", p.GetNavigatableButton());
                }
            }
        }

        /// <summary>
        /// Resets the navigation of the fifth button (flee, undo)
        /// </summary>
        private void ResetFifthButtonNavigation() {
            Button b = actions[4].GetComponent<Button>();
            Navigation n = b.navigation;

            n.selectOnUp = actions[2].IsInteractable() ? n.selectOnUp : actions[0].GetComponent<Button>();
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
                if (actions[i].IsInteractable()) {
                    Button b = actions[i].GetComponent<Button>();
                    Navigation n = b.navigation;
                    if (i == 0) {
                        n.selectOnDown = actions[2].IsInteractable() ? n.selectOnDown : actions[4].GetComponent<Button>();
                        n.selectOnRight = actions[1].IsInteractable() ? n.selectOnRight : null;
                        b.navigation = n;
                    }    
                    else if (i == 1) {
                        n.selectOnDown = actions[3].IsInteractable() ? n.selectOnDown : actions[4].GetComponent<Button>();
                        b.navigation = n;
                    }
                    /* else if (i == 2) {
                        n.selectOnRight = actions[3].isEnabled ? n.selectOnRight : 
                    } */
                    else if (i == 4) {
                        n.selectOnUp = actions[2].IsInteractable() ? n.selectOnUp : actions[0].GetComponent<Button>();
                        b.navigation = n;
                    }
                }
            }             
        }

        /// <summary>
        /// Returns the name of this panel
        /// </summary>
        /// <returns> Name of panel </returns>
        public override string GetPanelName() {
            return PanelConstants.ACTIONSPANEL;
        }

        /// <summary>
        /// Returns the Button that adjacent panels will navigate to
        /// </summary>
        /// <returns> Button to be navigated to </returns>
        public override Button GetNavigatableButton() {
            return null;
        }
    }
}
