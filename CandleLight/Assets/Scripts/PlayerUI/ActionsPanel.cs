﻿/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 11, 2019
* 
* The ActionPanel class is used to organize and initialize Action buttons.
* An action can be an attack, dialog option, a direction to travel in, or more, depending on the event.
*
*/

using ActionConstants = Constants.ActionConstants;
using Characters;
using Combat;
using Events;
using GameManager = General.GameManager;
using PanelConstants = Constants.PanelConstants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PlayerUI {

    public class ActionsPanel : Panel {
        
        /* external component references */
        public EventDescription actionDescription;
        public Action[] actions = new Action[5];    /// <value> List of actions, capped at 5 </value>
        
        public Action selectedAction;              /// <value> Action that was selected </value>

        private EventSystem es;                     /// <value> eventSystem reference </value>
        private Interaction travelInt;
        private Interaction fightInt;
        private bool isLeavePossible;               /// <value> Flag for if player can leave scenario </value>

        /// <summary>
        /// Awake to get initialize event system
        /// </summary>
        void Awake() {
            es = EventSystem.current;
        }

        public void SetGeneralInteractions(Interaction travelInt, Interaction fightInt) {
            this.travelInt = travelInt;
            this.fightInt = fightInt;
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
        /// <param name="interactions"> List of interactions according to event, length 5 </param>
        public void SetInteractionActions(Interaction[] interactions) { 
            for (int i = 0; i < interactions.Length; i++) {
                actions[i].SetAction(ActionConstants.INTERACTION, interactions[i]);
            }

            SetActionsUsable(true);
            SetAllActionsInteractable();
            FadeActions(1);
        }

        /// <summary>
        /// Displays actions for after combat
        /// </summary>
        public void TravelActions() { 
            for (int i = 0; i < actions.Length - 1; i++) {
                actions[i].SetAction(ActionConstants.NONE);
            }

            actions[actions.Length - 1].SetAction(ActionConstants.INTERACTION, travelInt);

            SetActionsUsable(true);
            SetAllActionsInteractable();
        }

        /// <summary>
        /// Displays actions for before combat when prompted
        /// </summary>
        public void PreCombatActions() { 
            actions[0].SetAction(ActionConstants.INTERACTION, fightInt);
            for (int i = 1; i < actions.Length; i++) {
                actions[i].SetAction(ActionConstants.NONE);
            }

            SetActionsUsable(true);
            SetAllActionsInteractable();
        }

        /// <summary>
        /// Initialize all actions with the partyMembers' attacks for combat
        /// </summary>
        /// <param name="attacks"> List of all attacks according to the partyMember </param>
        public void SetCombatActions(Attack[] attacks) {
            for (int i = 0; i < attacks.Length; i++) {
                actions[i].SetAction(ActionConstants.ATTACK, attacks[i]);
            }
            if (isLeavePossible) {
                actions[actions.Length - 1].SetAction(ActionConstants.FLEE);
            }
            else {
                actions[actions.Length - 1].SetAction(ActionConstants.NONE);
            }
        }

        /// <summary>
        /// Displays all actions as empty
        /// </summary>
        public void ClearAllActions() {
            for (int i = 0; i < actions.Length; i++) {
                actions[i].SetAction(ActionConstants.NONE);
                actions[i].SetUsable(true);
            }
        }

        /// <summary>
        /// Displays actions for after combat
        /// </summary>
        /// <param name="itemNum"> Number of items being displayed in the rewardsPanel </param>
        public void PostCombatActions(int itemNum) { 
            if (itemNum > 0) {
                actions[0].SetAction(ActionConstants.TAKEALL);
                for (int i = 1; i < actions.Length - 1; i++) {
                    actions[i].SetAction(ActionConstants.NONE);
                }
            }
            else {
                for (int i = 0; i < actions.Length - 1; i++) {
                    actions[i].SetAction(ActionConstants.NONE);
                }
            }

            actions[actions.Length - 1].SetAction(ActionConstants.INTERACTION, travelInt);

            SetActionsUsable(true);
            SetAllActionsUninteractable();
        }

        public void SetItemActions() {
            actions[0].SetAction(ActionConstants.TAKEALL);
            for (int i = 1; i < actions.Length - 1; i++) {
                actions[i].SetAction(ActionConstants.NONE);
            }

            actions[actions.Length - 1].SetAction(ActionConstants.INTERACTION, travelInt);
            
            SetActionsUsable(true);
            SetAllActionsInteractable(false);
        }

        /// <summary>
        /// Load a new interaction into the actionsPanel
        /// Will not do anything if maximum number of actions is reached
        /// </summary>
        /// <param name="intName"> Name of the interaction to load </param>
        public void AddInteraction(string intName) {
            for (int i = 0; i < actions.Length; i++) {
                if (actions[i].actionType == ActionConstants.NONE) {
                    actions[i].SetAction(ActionConstants.INTERACTION, GameManager.instance.DB.GetInteractionByName(intName));
                    actions[i].SetInteractable(true);
                    break;
                }
            }
        }

        /// <summary>
        /// Displays the first partyMember with special fade effects
        /// </summary>
        /// <param name="pm"></param>
        public void DisplayFirstPartyMember(PartyMember pm) {
            SetCombatActions(pm.attacks);
            FadeActions(1);
            CheckAndSetActionsToUnusable(pm.CMP, pm.CHP);
        }

        /// <summary>
        /// Display all the actions of a partyMember and their usability
        /// </summary>
        /// <param name="pm"></param>
        public void DisplayPartyMember(PartyMember pm) {
            SetCombatActions(pm.attacks);
            SetAllActionsInteractable();
            CheckAndSetActionsToUnusable(pm.CHP, pm.CMP);
        }

        /// <summary>
        /// Calls the releveant method depending on the type of action selected
        /// </summary>
        /// <param name="a"> Name of action to be taken </param>
        public void SelectAction(Action a) {
            if (UIManager.instance.panelButtonsEnabled == true && a.isUsable == true) {
                if (a.actionType == ActionConstants.ATTACK) {
                    selectedAction = a;
                    for (int i = 0; i < actions.Length - 1; i++) {
                        if (actions[i] == selectedAction) {
                            actions[i].ShowActionSelected();
                        }
                        else if (actions[i].actionType != ActionConstants.NONE) {
                            actions[i].ShowActionUnselected();  
                        }
                    }   
                    CombatManager.instance.PreparePMAttack(a.a);
                }
                else if (a.actionType == ActionConstants.INTERACTION) {
                    if (a.i.isSingleUse) {
                        a.SetUsable(false);
                    }
                    StartCoroutine(EventManager.instance.Interact(a.i));
                }
                else if (a.actionType == ActionConstants.TAKEALL) {
                    EventManager.instance.TakeAllItems();
                }
                else if (a.actionType ==  ActionConstants.FLEE) {
                    StartCoroutine(CombatManager.instance.AttemptFlee());
                }
            }
        }

        /// <summary>
        /// Enable all non-none actions to be interacted with using mouse or keys
        /// </summary>
        /// <param name="initialSelection"> Flag for if current selected action should remain selected </param>
        public void SetAllActionsInteractable(bool initialSelection = false) {
            int firstInteractableIndex = -1;

            for (int i = 0; i < actions.Length; i++) {
                if (actions[i].actionType != ActionConstants.NONE) {
                    actions[i].SetInteractable(true); 
                    if (firstInteractableIndex == -1) {
                        firstInteractableIndex = i;
                    } 
                } else {
                    actions[i].SetInteractable(false);  
                }
            }
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
            selectedAction = null;
        }

        /// <summary>
        /// Sets all actions uninteractable and fades each action's text out
        /// </summary>
        public void SetAllActionsUninteractableAndFadeOut() {
            for (int i = 0; i < actions.Length; i++) {
                if (actions[i].actionType != ActionConstants.NONE) {
                    actions[i].SetInteractable(false);  
                } 
            }
            selectedAction = null;
            FadeActions(0);
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
        /// Sets every attack action to visually be unusable if the partyMember can't use it
        /// </summary>
        /// <param name="CHP"> Current HP of partyMember </param>
        /// <param name="CMP"> Current MP of partyMember </param>
        private void CheckAndSetActionsToUnusable(int CHP, int CMP) {
             for (int i = 0; i < actions.Length; i++) {
                if (actions[i].actionType == "attack") {
                    Attack a = actions[i].a;
                
                    if (a.costType == "MP") {
                        if (a.costValue > CMP) {
                            actions[i].SetUsable(false);
                        }
                        else {
                            actions[i].SetUsable(true);
                        }
                    }
                    else if (a.costType == "HP") {
                        if (a.costValue > CHP) {
                            actions[i].SetUsable(false);
                        }
                        else {
                            actions[i].SetUsable(true);
                        }
                    }
                } 
                else {
                    actions[i].SetUsable(true);
                }
            } 
        }

        /// <summary>
        /// Sets all actions to visually be usable or unusable if the partyMember can't use it
        /// due to mana or health constraints.  
        /// </summary>
        /// <param name="value"> True to show useable, false to show unusable</param>
        public void SetActionsUsable(bool value) {
            for (int i = 0; i < actions.Length; i++) {
                actions[i].SetUsable(value);
            }
        }

        /// <summary>
        /// Fades actions to the target alpha value
        /// </summary>
        /// <param name="targetAlpha"> Int 0 or 1 </param>
        private void FadeActions(int targetAlpha) {
            foreach (Action a in actions) {
                StartCoroutine(a.Fade(targetAlpha));
            }
        }

        public void LogActions() {
            foreach (Action a in actions) {
                Debug.Log("action name: " + a.name + " interactable: " + a.IsInteractable());
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
        /// Returns the Button that a panel on the right would want to navigate to
        /// </summary>
        /// <returns> Button to be navigated to </returns>
        public  Button GetNavigatableButtonRight() {
            if (actions[1].IsInteractable()) {
                return actions[1].b;
            }
            else {  // actions[4]
                return actions[4].b;
            }
        }

        /// <summary>
        /// Returns the Button that a panel on the left would want to navigate to
        /// </summary>
        public Button GetNavigatableButtonLeft() {
            if (actions[0].IsInteractable()) {
                return actions[0].b;
            }
            else {  // actions[4]
                return actions[4].b;
            }
        }

        /// <summary>
        /// Returns the Button that a panel above would want to navigate to
        /// </summary>
        public Button GetNavigatableButtonUp() {
            if (actions[0].IsInteractable()) {
                return actions[0].b;
            }
            else {  // actions[4]
                return actions[4].b;
            }
        }
    }
}
