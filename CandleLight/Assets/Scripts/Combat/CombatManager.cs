﻿/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 11, 2019
* 
* The CombatManager class is used to manage all classes during the combat scene.
* Party members, monsters, actions, and turn ordering are all managed in the combat manager.
*
* TODO:
* Need to work with attacks that target multiple monsters, and multiple partyMembers
* Unify naming (make interactable functions 1 function with true or false, make all button related stuff use
* the word interactable)
*/

using Characters;
using EventManager = Events.EventManager;
using General;
using Party;
using PlayerUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Combat {

    public delegate void SelectMonsterDelegate(Monster m);

    public class CombatManager : MonoBehaviour {
        
        /* constants */
        private readonly bool PMTURN = true;        /// <value> Flag for party member turn </value>
        private readonly bool MTURN = false;        /// <value> Flag for monster turn </value>

        public static CombatManager instance;       /// <value> Combat scene instance </value>

        /* external component references */
        public Canvas enemyCanvas;                  /// <value> Canvas for where monsters are displayed </value>
        public EventDescription eventDescription;   /// <value> Display for all text and prompts relevant to an action or event</value>
        public StatusPanel statusPanel;             /// <value> Display for active party member's status </value>
        public ActionsPanel actionsPanel;           /// <value> Display for active party member's actions </value>
        public PartyPanel partyPanel;               /// <value> Display for all party member's status </value>
        public TabManager utilityTabManager;           /// <value> Click on to display other panels </value>
        public GameObject monster;                  /// <value> Monster GO to instantiate </value>

        
        public bool isReady { get; private set; } = false;                  /// <value> Localization happens at the start, program loads while waiting </value>
        public List<Monster> monstersKilled { get; private set; }           /// <value> List of monsters killed in combat instance </value>
        
        private EventSystem es;                                             /// <value> EventSystem reference </value>
        private List<PartyMember> partyMembers = new List<PartyMember>();   /// <value> List of party members </value>
        private PartyMember activePartyMember;                              /// <value> Current party member preparing to act </value>
        private List<Monster> monsters =  new List<Monster>();              /// <value> List of monsters </value>
        private List<Monster> selectedMonsterAdjacents = new List<Monster>();   /// <value> List of monsters adjacent to the monster selected that will be affected </value>
        private Monster selectedMonster = null;                             /// <value> Selected monster </value>
        private Monster activeMonster;                                      /// <value> Current monster preparing to act </value>
        private CharacterQueue cq = new CharacterQueue();                   /// <value> Queue for attacking order in combat </value>
        private Attack selectedAttack = null;       /// <value> Attack selected by party member </value>
        private int countID = 0;                    /// <value> Unique ID for each character in combat </value>
        private int middleMonster = 0;              /// <value> Index of monster in the middle of the canvas, rounds down </value>
        private int maxMonsters = 5;                /// <value> Max number of enemies that can appear on screen </value>
        private bool turn;                          /// <value> Current turn (PMTURN or MTURN) </value>
        private bool pmSelectionFinalized = false;  /// <value> Flag for if player has selected an action and a monster if its an attack </value>
        private bool isFleePossible = true;         /// <value> Flag for if player can flee battle, will need to make this changeable in the future </value>
        private bool isFleeSuccessful = false;      /// <value> Flag for if player successfully fled from battle </value>

        #region [ Initialization ] Initialization

        /// <summary>
        /// Awake to instantiate singleton
        /// </summary>
        void Awake() {
            if (instance == null) {
                instance = this;
            }
            else if (instance != this) {
                DestroyImmediate (gameObject);
                instance = this;
            }
            
            es = EventSystem.current;
        }
        
        /// <summary>
        /// Start to initialize all monsters, characters, and combat queue before beginning combat
        /// </summary>
        public IEnumerator InitializeCombat(string[] monsterNames) {
            actionsPanel.Init(isFleePossible);
            isFleeSuccessful = false;
            cq.Reset();
            countID = 0;
            monsters = new List<Monster>();
            monstersKilled = new List<Monster>();
            partyMembers = PartyManager.instance.GetPartyMembers(ref countID);

            foreach (string monsterName in monsterNames) {
                yield return StartCoroutine(AddMonster(monsterName));
            }
            ArrangeMonsters();
            
            foreach (Monster m in monsters) {
                cq.AddCharacter(m);
            }
            foreach (PartyMember pm in partyMembers) {
                if (pm.CheckDeath() == false) {
                    cq.AddCharacter(pm);
                }
            }
            cq.FinalizeQueue();

            yield return (StartCoroutine(DisplayCombatIntro()));
            
            StartCombat();
        }

        /// <summary>
        /// Adds a monster GO to the enemy canvas, initializing its values and setting navigation
        /// </summary>
        /// <param name="monsterName"> Name of the monster to be fetched from the DB </param>
        /// <remark> Assumes there will always be an action at button 0 </remark>
        private IEnumerator AddMonster(string monsterName) {
            GameObject newMonster = Instantiate(DataManager.instance.GetLoadedMonsterDisplay(monsterName));
            newMonster.SetActive(true); // monster game object must be active to manipulate its components
            
            Monster monsterComponent = newMonster.GetComponent<Monster>();
            SelectMonsterDelegate smd = new SelectMonsterDelegate(SelectMonster);
            
            monsterComponent.ID = countID++;
            monsterComponent.md.SetHealthBar(); 
            monsterComponent.md.AddSMDListener(smd);
            monsterComponent.md.SetInteractable(false);

            newMonster.transform.SetParent(enemyCanvas.transform, false);
            
            monsters.Add(monsterComponent);
            
            newMonster.SetActive(false); // hide after manipulating components so player doesn't see monster until ArrangeMonsters()
            yield break;
        }

        /// <summary>
        /// Perform all animations at the start of combat (spawning, dramatic pausing, etc.)
        /// </summary>
        /// <returns> IEnumerator cause animations </returns>
        private IEnumerator DisplayCombatIntro() {
            DisableAllButtons();
            foreach (Monster m in monsters) {
                StartCoroutine(m.md.PlaySpawnAnimation());
            }
            yield return new WaitForSeconds(1.5f);   
            eventDescription.ClearText();
        }

        /// <summary>
        /// Starts combat by displaying the next party member's active turn
        /// and determining which character moves next
        /// </summary>
        private void StartCombat() {
            DisplayFirstPartyMember();          // active party member is not set
            DisableAllMonsterSelection();

            GetNextTurn();
        }

        #endregion

        /// <summary>
        /// Determine's who's turn it is in the queue and then prepares the attacking character
        /// to either see their options if its a party member, or attack if its a monster.
        /// </summary>
        public void GetNextTurn() {
            Character c = cq.GetNextCharacter();

            if (c is PartyMember) {
                activePartyMember = (PartyMember)c;
                turn = PMTURN;
            } 
            else {
                activeMonster = (Monster)c;
                turn = MTURN; 
            }  
            
            if (turn) {
                StartCoroutine(PMTurn());
            } 
            else {
                StartCoroutine(MonsterTurn());
            }
        }

        #region [ Section0 ] PartyMember Turn

        /// <summary>
        /// Perform all of the phases in the party member's turn
        /// </summary>
        /// <returns> IEnumerator so actions are all taken in order </returns>
        private IEnumerator PMTurn() {
            StartPMTurn();
            while (!pmSelectionFinalized) {    // (PreparePMAttack and SelectMonster) or AttemptFlee
                yield return null;
            }
            DisableAllButtons();
            if (selectedAttack != null) {
                yield return StartCoroutine(ExecutePMAttack());  
            }
            yield return StartCoroutine(CleanUpPMTurn());
            if (CheckBattleOver() || isFleeSuccessful) {
                EndCombat();
            }
            else {
                EndPMTurn();
                GetNextTurn();
            }
        }

        /// <summary>
        /// Prepare all monsters and relevant panels (Actions, Status) for the player's turn
        /// </summary>
        private void StartPMTurn() {
            DisplayActivePartyMember();
            EnableAllButtons();
            SetMonsterNavigation();

            actionsPanel.SetHorizontalNavigation(partyPanel);
            partyPanel.SetHorizontalNavigation();
        }

        /// <summary>
        /// Moves selection to the left most monster, and preventing all actions except for the
        /// bottom-most action (undo) to be selected
        /// </summary>
        /// <param name="a"> Attack to be executed </param>
        /// <remark> 
        /// Will have to disable other player panel's in the future and make monster selection
        /// more logical (e.g. selecting the last selected monster after the first attack, or
        /// selecting the middle most monster by default)
        /// </remark>
        public void PreparePMAttack(Attack a) {
            selectedAttack = a;
            foreach(Monster m in monsters) {
                m.md.SetNavigation("down", actionsPanel.GetActionButton(4));
            }
            actionsPanel.SetButtonNavigation(4, "up", monsters[middleMonster].md.b);
            partyPanel.SetHorizontalNavigation();

            es.SetSelectedGameObject(monsters[middleMonster].md.b.gameObject);
        }

        /// <summary>
        /// Randomly determine if the player is able to end combat before killing all monsters
        /// </summary>
        /// <returns> IEnumerator to play through death animations </returns>
        /// <remark> Death animation for each monster is played as a de-spawning animation</remark>
        public IEnumerator AttemptFlee() {
            int chance = Random.Range(activePartyMember.LVL, 100) - monsters[0].LVL;
            List<Monster> monstersToRemove = new List<Monster>();

            // wait for all monsters to despawn
            DisableAllButtons();
            for (int i = 0; i < monsters.Count - 1; i++) {  
                StartCoroutine(monsters[i].md.PlayDeathAnimation());
                monstersToRemove.Add(monsters[i]);
            }
            yield return (StartCoroutine(monsters[monsters.Count - 1].md.PlayDeathAnimation())); // yield to last monster so we wait for all monsters to die
            monstersToRemove.Add(monsters[monsters.Count - 1]);
            yield return new WaitForSeconds(0.75f);

            if (chance > 50) {
                DestroyMonsters(monstersToRemove);
                isFleeSuccessful = true;
            }
            else {
                eventDescription.SetKey(GetFleeFailedKey());
                for (int i = 0; i < monsters.Count - 1; i++) {  // wait for all monsters to respawn
                    StartCoroutine(monsters[i].md.PlaySpawnAnimation());
                    monstersToRemove.Add(monsters[i]);
                }
                yield return (StartCoroutine(monsters[monsters.Count - 1].md.PlaySpawnAnimation())); 
                monstersToRemove.Add(monsters[monsters.Count - 1]);
                yield return new WaitForSeconds(0.25f);
            }

            pmSelectionFinalized = true;
        }

        /// <summary>
        /// Revert target selection UI to action selection phase
        /// </summary>
        public void UndoPMAction() {
            selectedAttack = null;
            foreach(Monster m in monsters) {
                m.md.SetNavigation("down", actionsPanel.GetActionButton(0));
            }

            partyPanel.SetHorizontalNavigation();
        }

        /// <summary>
        /// Deals damage to a monster.
        /// If the monster were to die, see if combat is finished, otherwise reset most of the UI
        /// to as if it were the start of the player's turn, and then determine the next turn.
        /// </summary>
        /// <param name="a"> Attack to be executed </param>
        /// <param name="m"> Monster to be attacked </param>
        /// <returns> 
        /// Yields to allow animations to play out when a monster is being attacked or taking damage
        /// </returns>
        public IEnumerator ExecutePMAttack() {    
            eventDescription.SetKey(selectedAttack.nameKey); 
            yield return new WaitForSeconds(0.25f);

            yield return StartCoroutine(activePartyMember.PayAttackCost(selectedAttack.costType, selectedAttack.cost));
            if (selectedAttack.scope == "single") {
                yield return StartCoroutine(selectedMonster.LoseHP(selectedAttack.damage, selectedAttack.animationClipName));
            }
        }

        /// <summary>
        /// Resolve end of turn effects (monster's being dead, end of turn debuffs, etc.)
        /// </summary>
        /// <returns> IEnumerator for animations </returns>
        public IEnumerator CleanUpPMTurn() {
            List<Monster> monstersToRemove = new List<Monster>();
            
            foreach (Monster m in monsters) {
                if (m.CheckDeath()) {
                    cq.RemoveCharacter(m.ID);
                    monstersToRemove.Add(m);
                    monstersKilled.Add(m);

                    yield return StartCoroutine(m.md.PlayDeathAnimation());
                }
            }
            DestroyMonsters(monstersToRemove);

            DeselectMonsters();
            selectedAttack = null;
            pmSelectionFinalized = false;
            eventDescription.ClearText();
        }

        /// <summary>
        /// Ends the partyMember's turn, 
        /// </summary>
        public void EndPMTurn() {

        }

        #endregion

        #region [ Section1 ] Monster Turn

        /// <summary>
        /// Perform all of the phases in the monster's turn
        /// </summary>
        /// <returns></returns>
        private IEnumerator MonsterTurn() {
            StartMonsterTurn();
            yield return StartCoroutine(ExecuteMonsterAttack());
            CleanUpMonsterTurn();
            if (CheckBattleOver()) {
                EndCombat();
            }
            else {
                EndMonsterTurn();
                GetNextTurn();
            }
        }

        /// <summary>
        /// Start the active monster's turn
        /// </summary>
        /// <returns> Yields to allow monster attack animation to play </returns>
        private void StartMonsterTurn() {
            DisableAllButtons();
        }

        /// <summary>
        /// Executes a monster's attack, by first getting the attack it wants to use, and then selecting
        /// its target based on its AI
        /// </summary>
        /// <returns> IEnumerator to play animations after each action </returns>
        private IEnumerator ExecuteMonsterAttack() {
            int targetChoice = 0;
            Attack attackChoice = activeMonster.SelectAttack();
            eventDescription.SetKey(attackChoice.nameKey);
            yield return StartCoroutine(activeMonster.md.PlayStartTurnAnimation());

            if (activeMonster.monsterAI == "random") {
                targetChoice = Random.Range(0, partyMembers.Count);
            }
            else if (activeMonster.monsterAI == "weakHunter") {
                int weakest = 0;
                for (int i = 1; i < partyMembers.Count; i++) {
                    if (partyMembers[i].CHP < partyMembers[weakest].CHP && !partyMembers[i].CheckDeath()) {
                        weakest = i;
                    }
                }
                targetChoice = weakest;
            }
            
            yield return (StartCoroutine(activeMonster.md.PlayAttackAnimation()));
            eventDescription.SetPMDamageText(partyMembers[targetChoice], attackChoice.damage);
            
            yield return (StartCoroutine(partyMembers[targetChoice].LoseHP(attackChoice.damage)));
        }

        /// <summary>
        /// Resolve end of turn effects (partyMembers's being dead, end of turn debuffs, etc.)
        /// </summary>
        /// <returns> IEnumerator for animations </returns>
        public void CleanUpMonsterTurn() {
            List<PartyMember> partyMembersToRemove = new List<PartyMember>();
            
            foreach (PartyMember pm in partyMembers) {
                if (pm.CheckDeath()) {
                    cq.RemoveCharacter(pm.ID);
                    partyMembersToRemove.Add(pm);
                }
            }
            foreach (PartyMember pm in partyMembersToRemove) {
                partyMembers.Remove(pm);
            }

            eventDescription.ClearText();
        }

        /// <summary>
        /// Ends the monster's turn
        /// </summary>
        public void EndMonsterTurn() {
            // nothing for now
        }

        /// <summary>
        /// Checks if all party members or all monsters are defeated
        /// </summary>
        /// <returns></returns>
        public bool CheckBattleOver() {
            if (cq.CheckMonstersDefeated() || cq.CheckPartyDefeated()) {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Ends combat by returning to the main menu scene.
        /// </summary>
        /// <remark> 
        /// For now, this leads back to the main menu. Later, this will need to lead to 
        /// either a defeat screen or a rewards screen
        /// </remark>
        private void EndCombat() {
            string endString = "";

            if (isFleeSuccessful) {
                endString = "FLEE";
            }
            else if (cq.CheckMonstersDefeated()) {
                endString = "VICTORY";
            }
            else if (cq.CheckPartyDefeated()) {
                endString = "DEFEAT";
            }

            StartCoroutine(EventManager.instance.DisplayPostCombat(endString));
        }

        #endregion

        #region [ Section2 ] Monster Selection
       
        /// <summary>
        /// Selects a monster to be attacked
        /// </summary>
        /// <param name="monsterToSelect"> Monster to select </param>
        /// <remark> Will probably make a UI info popup when clicking on monsters with no attack in the future </remark>
        public void SelectMonster(Monster monsterToSelect) {
            DeselectMonsters();
            
            selectedMonster = monsterToSelect;

            // Player can freely click on monsters without having an attack selected
            // Will probably make a UI popup when clicking on monsters with no attack in the future
            if (selectedAttack != null) {
                if (selectedAttack.scope != "single") {
                    int monsterIndex = 0;
                    for (int i = 0; i < monsters.Count; i++) {
                        if (monsters[i].ID == monsterToSelect.ID) {
                            monsterIndex = i;
                            break;
                        }
                    }
                    if (selectedAttack.scope == "adjacent") {
                        if (monsterIndex - 1 > 0) {
                            selectedMonsterAdjacents.Add(monsters[monsterIndex - 1]);
                        }
                        if (monsterIndex + 1 < monsters.Count) {
                            selectedMonsterAdjacents.Add(monsters[monsterIndex + 1]);
                        }
                    }

                    monsterToSelect.md.SelectMonsterButton();
                    foreach (Monster m in selectedMonsterAdjacents) {
                        m.md.SelectMonsterButtonAdjacent();
                    }
                }
                
                pmSelectionFinalized = true;
            }
        }
        /// <summary>
        /// Deselect monster's visually in the UI but don't remove references to them
        /// </summary>
        /// <remark>
        /// This is to deselect monsters after an attack has been selected to remove
        /// selection UI during animations
        /// </remark>
        public void DeselectMonstersVisually() {
            if (selectedMonster != null) {
                selectedMonster.md.DeselectMonsterButton();
            }
            foreach (Monster m in selectedMonsterAdjacents) {
                m.md.DeselectMonsterButton();
            }
        }

        /// <summary>
        /// Deselects monsters, visually and while removing references to the selected ones and 
        /// </summary>
        public void DeselectMonsters() {
            if (selectedMonster != null) {
                selectedMonster.md.DeselectMonsterButton(); 
                selectedMonster = null;
            }
            
            foreach (Monster m in selectedMonsterAdjacents) {
                m.md.DeselectMonsterButton();
            }
            selectedMonsterAdjacents.Clear();
        }

        /// <summary>
        /// Destroy monster gameObjects
        /// </summary>
        /// <param name="monstersToRemove"> List of monsters to remove </param>
        public void DestroyMonsters(List<Monster> monstersToRemove) {
            foreach (Monster m in monstersToRemove) {
                monsters.Remove(m);
                Destroy(m.gameObject);
            }
        }

        /// <summary>
        /// Enables selection of all monsters
        /// </summary>
        public void EnableAllMonsterSelection() {
            foreach (Monster m in monsters) {
                m.md.SetInteractable(true);
            }
        }

        /// <summary>
        /// Disables all interaction of monsters
        /// </summary>
        public void DisableAllMonsterSelection() {
            foreach (Monster m in monsters) {
                m.md.SetInteractable(false);
            }
        }  

        /// <summary>
        /// Arranges monsters on screen to look nice depending on the number.
        /// Only a max of 5 monsters for now, although 3 large monsters, 4 normal, and 5 small fit at a time
        /// </summary>
        /// <remark> Will add custom arrangements for wackier monster layouts in the future </remark>
        private void ArrangeMonsters() {
            string arrangementType = "normal";

            if (arrangementType == "normal") {
                if (monsters.Count == 1) {
                    float spacing0 = 0;
                    monsters[0].gameObject.transform.localPosition = new Vector3(spacing0, monsters[0].gameObject.transform.position.y, 0.0f);
                }
                else if (monsters.Count == 2) {
                    float spacing0 = ((monsters[0].md.spriteWidth / 2) + 20) * -1;
                    float spacing1 = ((monsters[1].md.spriteWidth / 2) + 20);  
                    monsters[0].gameObject.transform.localPosition = new Vector3(spacing0, monsters[0].gameObject.transform.position.y, 0.0f);
                    monsters[1].gameObject.transform.localPosition = new Vector3(spacing1, monsters[1].gameObject.transform.position.y, 0.0f);  
                }
                else if (monsters.Count == 3) {
                    float spacing0 = ((monsters[1].md.spriteWidth / 2 + monsters[0].md.spriteWidth / 2) + 40) * -1;  
                    float spacing1 = 0;
                    float spacing2 = ((monsters[1].md.spriteWidth / 2 + monsters[2].md.spriteWidth / 2) + 40);
                    
                    monsters[0].gameObject.transform.localPosition = new Vector3(spacing0, monsters[0].gameObject.transform.position.y, 0.0f);
                    monsters[1].gameObject.transform.localPosition = new Vector3(spacing1, monsters[1].gameObject.transform.position.y, 0.0f);  
                    monsters[2].gameObject.transform.localPosition = new Vector3(spacing2, monsters[2].gameObject.transform.position.y, 0.0f);  
                }
                else if (monsters.Count == 4) {
                    float spacing0 = (monsters[1].md.spriteWidth + (monsters[0].md.spriteWidth / 2) + 60) * -1;  
                    float spacing1 = ((monsters[1].md.spriteWidth / 2) + 20) * -1; 
                    float spacing2 = (monsters[2].md.spriteWidth / 2) + 20;
                    float spacing3 = monsters[2].md.spriteWidth + (monsters[3].md.spriteWidth / 2) + 60;  

                    monsters[0].gameObject.transform.localPosition = new Vector3(spacing0, monsters[0].gameObject.transform.position.y, 0.0f);
                    monsters[1].gameObject.transform.localPosition = new Vector3(spacing1, monsters[1].gameObject.transform.position.y, 0.0f);  
                    monsters[2].gameObject.transform.localPosition = new Vector3(spacing2, monsters[2].gameObject.transform.position.y, 0.0f); 
                    monsters[3].gameObject.transform.localPosition = new Vector3(spacing3, monsters[3].gameObject.transform.position.y, 0.0f);   
                }
                else if (monsters.Count == 5) {
                    float spacing0 = ((monsters[2].md.spriteWidth / 2) + monsters[1].md.spriteWidth + monsters[0].md.spriteWidth / 2 + 80) * -1;  
                    float spacing1 = ((monsters[2].md.spriteWidth / 2) + monsters[1].md.spriteWidth / 2 + 40) * -1; 
                    float spacing2 = 0;
                    float spacing3 = (monsters[2].md.spriteWidth / 2) + monsters[3].md.spriteWidth / 2 + 40; 
                    float spacing4 = ((monsters[2].md.spriteWidth / 2) + monsters[3].md.spriteWidth + monsters[4].md.spriteWidth / 2 + 80);  

                    monsters[0].gameObject.transform.localPosition = new Vector3(spacing0, monsters[0].gameObject.transform.position.y, 0.0f);
                    monsters[1].gameObject.transform.localPosition = new Vector3(spacing1, monsters[1].gameObject.transform.position.y, 0.0f);  
                    monsters[2].gameObject.transform.localPosition = new Vector3(spacing2, monsters[2].gameObject.transform.position.y, 0.0f); 
                    monsters[3].gameObject.transform.localPosition = new Vector3(spacing3, monsters[3].gameObject.transform.position.y, 0.0f);
                    monsters[4].gameObject.transform.localPosition = new Vector3(spacing4, monsters[4].gameObject.transform.position.y, 0.0f);      
                } 
            }

            // show monster now that all of its properties are set up
            foreach (Monster m in monsters) {
                m.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// Sets the navigation between monster buttons
        /// </summary>
        private void SetMonsterNavigation() {
            foreach (Monster m in monsters) {
                m.md.ResetNavigation();
            }
            foreach(Monster m in monsters) {
                m.md.SetNavigation("down", actionsPanel.GetActionButton(0));
            }

            if (monsters.Count > 1) {
                for (int i = 0; i < monsters.Count; i++) {
                    if (i == 0) {
                        monsters[i].md.SetNavigation("right", monsters[i + 1].md.b);
                    }
                    else if (i == monsters.Count - 1) {
                        monsters[i].md.SetNavigation("left", monsters[i - 1].md.b);
                    }
                    else {
                        monsters[i].md.SetNavigation("left", monsters[i - 1].md.b);
                        monsters[i].md.SetNavigation("right", monsters[i + 1].md.b);
                    }
                }
            }
            
            middleMonster = (int)(Mathf.Floor(monsters.Count / 2f));
            actionsPanel.SetButtonNavigation(0, "up", monsters[middleMonster].md.b);
            actionsPanel.SetButtonNavigation(1, "up", monsters[middleMonster].md.b);
        }

        #endregion

        #region [ Section3 ] PartyMember UI Management

        /// <summary>
        /// Disables interactions with all buttons player can select
        /// </summary>
        public void DisableAllButtons() {
            actionsPanel.SetAllActionsUninteractable();
            partyPanel.DisableButtons();
            utilityTabManager.SetAllButtonsUninteractable();
            DeselectMonstersVisually();
            DisableAllMonsterSelection();
        }

        /// <summary>
        /// Enables all buttons to be selectable
        /// </summary>
        public void EnableAllButtons() {
            EnableAllMonsterSelection();
            actionsPanel.SetAllActionsInteractable();
            utilityTabManager.SetAllButtonsInteractable();
            partyPanel.EnableButtons();
        }
        
        /// <summary>
        /// Changes the UI to reflect the first party member in the queue's information
        /// </summary>s
        public void DisplayFirstPartyMember() {
            activePartyMember = cq.GetFirstPM();    // activePartyMember will be redundantly set a second time
            actionsPanel.DisplayFirstPartyMember(activePartyMember);
            statusPanel.DisplayPartyMember(activePartyMember);
        }

        /// <summary>
        /// Changes the UI to reflect the active party member's information
        /// </summary>s
        public void DisplayActivePartyMember() {
            actionsPanel.DisplayPartyMember(activePartyMember);
            statusPanel.DisplayPartyMember(activePartyMember);
        }

        #endregion

        /// <summary>
        /// Returns a key string for when the player fails to flee
        /// </summary>
        /// <returns> String </returns>
        public string GetFleeFailedKey() {
            return "flee_failed_" + Random.Range(0, 4);
        }
    }
}
