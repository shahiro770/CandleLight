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
using Audio;
using Characters;
using Combat;
using Events;
using GameManager = General.GameManager;
using PanelConstants = Constants.PanelConstants;
using tutorialTriggers = Constants.TutorialConstants.tutorialTriggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PlayerUI {

    public class ActionsPanel : Panel {
        
        /* external component references */
        public EventDescription actionDescription;
        public Action[] actions = new Action[5];    /// <value> List of actions, capped at 5 </value>
        public Button toggleButton;
        public Image toggleImage;
        public Sprite attacksSprite;
        public Sprite interactionsSprite;
        
        public Action selectedAction;              /// <value> Action that was selected </value>
        public bool isStoringInt = false;

        private EventSystem es;                     /// <value> eventSystem reference </value>
        private Interaction[] storedInts = new Interaction[5];          /// <value> List of interactions stored </value>
        private bool[] storedUsability = new bool[5];
        private Interaction travelInt;
        private Interaction fightInt;
        private Interaction tutorialInt;
        private bool isLeavePossible;               /// <value> Flag for if player can leave scenario </value>
        
       
        /// <summary>
        /// Awake to get initialize event system
        /// </summary>
        void Awake() {
            es = EventSystem.current;
        }

        public void SetGeneralInteractions(Interaction travelInt, Interaction fightInt, Interaction tutorialInt) {
            this.travelInt = travelInt;
            this.fightInt = fightInt;
            this.tutorialInt = tutorialInt;
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
            SetActionsUsable(true); // TODO: this kinda of interactions sets the colour like 3 times, but hopefully this isn't expensive (but redoing the logic here is risky)

            for (int i = 0; i < interactions.Length; i++) {
                actions[i].SetAction(ActionConstants.INTERACTION, interactions[i]);
                if (interactions[i].name == "takeAll") {
                    actions[i].SetAction(ActionConstants.TAKEALL);
                }
                if (interactions[i].checkIndicator != 0) {
                    if (interactions[i].checkIndicator >= 1 && interactions[i].checkIndicator <= 4) {
                        actions[i].SetCheckColor(interactions[i].checkIndicator);
                        if (GameManager.instance.tutorialTriggers[(int)tutorialTriggers.isTips] == true
                        && GameManager.instance.tutorialTriggers[(int)tutorialTriggers.firstStatInt] == true) {
                            EventManager.instance.SetTutorialNotification("stat");
                            GameManager.instance.tutorialTriggers[(int)tutorialTriggers.firstStatInt] = false;
                        }
                    }
                }
            }

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
        /// Initializes all actions with the partyMember's attacks for combat, but sets the fifth to none
        /// </summary>
        /// <param name="pm"></param>
        public void SetCombatActionsNoFifth(PartyMember pm) {
            for (int i = 0; i < pm.attacks.Length; i++) {
                actions[i].SetAction(ActionConstants.ATTACK, pm.attacks[i]);
            }
            actions[actions.Length -1].SetAction(ActionConstants.NONE);
            SetAllActionsInteractable();
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

        /// <summary>
        /// Tutorial only, after ending combat, the player is led to the next specific event
        /// (Hacky, will need to be replaced if other combat events lead to specific events)
        /// </summary>
        public void PostCombatActionsTutorial() {
            for (int i = 0; i < actions.Length - 1; i++) {
                actions[i].SetAction(ActionConstants.NONE);
            }
            actions[actions.Length - 1].SetAction(ActionConstants.INTERACTION, tutorialInt);
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
                    Interaction newInt =  GameManager.instance.DB.GetInteractionByName(intName);
                    actions[i].SetAction(ActionConstants.INTERACTION, newInt);
                    actions[i].SetInteractable(true);
                    if (newInt.checkIndicator != 0) {
                        if (newInt.checkIndicator >= 1 && newInt.checkIndicator <= 4) {
                            actions[i].SetCheckColor(newInt.checkIndicator);
                            if (GameManager.instance.tutorialTriggers[(int)tutorialTriggers.isTips] == true
                            && GameManager.instance.tutorialTriggers[(int)tutorialTriggers.firstStatInt] == true) {
                                EventManager.instance.SetTutorialNotification("stat");
                                GameManager.instance.tutorialTriggers[(int)tutorialTriggers.firstStatInt] = false;
                            }
                        }
                    }
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
            CheckAndSetActionsToUnusable(pm.CHP, pm.CMP);
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
        /// Swap between combat and interaction actions for the current event
        /// (Combat actions cannot be used)
        /// </summary>
        /// <param name="pm"></param>
        public void ToggleInteractionAndCombatActions(PartyMember pm) {
            if (isStoringInt == false) {
                isStoringInt = true;
                toggleImage.sprite = interactionsSprite;
                for (int i = 0; i < storedInts.Length; i++) {
                    storedInts[i] = actions[i].i;
                    storedUsability[i] = actions[i].isUsable;
                }
                SetCombatActionsNoFifth(pm);
                CheckAndSetActionsToUnusable(pm.CHP, pm.CMP);
            }
            else {
                isStoringInt = false;
                toggleImage.sprite = attacksSprite;
                for (int i = 0; i < storedInts.Length; i++) {
                    if (storedInts[i] != null) {
                        actions[i].SetAction(ActionConstants.INTERACTION, storedInts[i]);
                        actions[i].SetUsable(storedUsability[i]);
                        if (storedInts[i].name == "takeAll") {
                            actions[i].SetAction(ActionConstants.TAKEALL);
                            EventManager.instance.UpdateTakeAll();
                        }
                        if (storedInts[i].checkIndicator != 0) {
                            if (storedInts[i].checkIndicator >= 1 && storedInts[i].checkIndicator <= 4) {
                                actions[i].SetCheckColor(storedInts[i].checkIndicator); // no need to check for tutorials here, it would've happened pre toggle
                            }
                        }
                    }
                    else {
                        actions[i].SetAction(ActionConstants.NONE);
                    }
                    storedInts[i] = null;
                }
            }
            
            SetAllActionsInteractable();
        }

        /// <summary>
        /// Update displayed combat actions for when outside of combat and the player is making
        /// adjustments to their attacks and stats (via the actions panel toggle)
        /// </summary>
        /// <param name="pm"></param>
        public void UpdateCombatActions(PartyMember pm) {
            if (isStoringInt == true) {
                SetCombatActionsNoFifth(pm);
                CheckAndSetActionsToUnusable(pm.CHP, pm.CMP);
            }
        }

        public void SetToggleButtonInteractable(bool value) {
            toggleButton.interactable = value;
        }

        /// <summary>
        /// Updates the take all button's usability
        /// Fails if the first action is not of type TAKEALL 
        /// </summary>
        /// <param name="numItemsTakeable"></param>
        public void UpdateTakeAll(int numSpareFull) {
            if (actions[0].actionType == ActionConstants.TAKEALL) {
                if (numSpareFull > 0) {
                    actions[0].SetUsable(true);
                }
                else {
                    actions[0].SetUsable(false);
                }
            }
        }

        /// <summary>
        /// Calls the releveant method depending on the type of action selected
        /// </summary>
        /// <param name="a"> Name of action to be taken </param>
        public void SelectAction(Action a) {
            if (UIManager.instance.panelButtonsEnabled == true && a.isUsable == true) {
                if (a.actionType == ActionConstants.ATTACK && CombatManager.instance.inCombat == true) {
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

            AudioManager.instance.PlaySFX("click");
        }

        /// <summary>
        /// Enable all non-none actions to be interacted with using mouse or keys
        /// </summary>
        /// <param name="initialSelection"> Flag for if current selected action should remain selected </param>
        public void SetAllActionsInteractable(bool initialSelection = false) {
            for (int i = 0; i < actions.Length; i++) {
                if (actions[i].actionType != ActionConstants.NONE) {
                    actions[i].SetInteractable(true); 
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
            toggleButton.interactable = false;
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
            SetToggleButtonInteractable(false);
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
                if (actions[i].actionType == ActionConstants.ATTACK) {
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
                        if (a.costValue > CHP) {    // note: caster might die casting (but potential clutch god moments)
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
