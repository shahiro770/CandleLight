/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 11, 2019
* 
* The CombatManager class is used to manage all classes during the combat scene.
* Party members, monsters, actions, and turn ordering are all managed in the combat manager.
*
*/

using Characters;
using Database;
using General;
using Party;
using PlayerUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Combat {

    public delegate void SelectMonsterDelegate(Monster m);

    public class CombatManager : MonoBehaviour {
        
        private readonly bool PMTURN = true;        /// <value> Flag for party member turn </value>
        private readonly bool MTURN = false;        /// <value> Flag for monster turn </value>

        public static CombatManager instance;       /// <value> Combat scene instance </value>

        public Canvas enemyCanvas;                  /// <value> Canvas for where monsters are displayed </value>
        public StatusPanel statusPanel;             /// <value> Display for active party member's status </value>
        public ActionsPanel actionsPanel;           /// <value> Display for active party member's actions </value>
        public PartyPanel partyPanel;               /// <value> Display for all party member's status </value>
        public GameObject monster;                  /// <value> Monster GO to instantiate </value>
        public bool isReady { get; private set; } = false;              /// <value> Localization happens at the start, program loads while waiting </value>
        
        private EventSystem es;                                             /// <value> EventSystem reference </value>
        private List<PartyMember> partyMembers = new List<PartyMember>();   /// <value> List of party members </value>
        private PartyMember activePartyMember;                              /// <value> Current party member preparing to act </value>
        private List<Monster> monsters =  new List<Monster>();              /// <value> List of monsters </value>
        private Monster activeMonster;                                      /// <value> Current monster preparing to act </value>
        private List<Monster> selectedMonsters = new List<Monster>();       /// <value> List of monsters selected </value>
        private CharacterQueue cq = new CharacterQueue();                   /// <value> Queue for attacking order in combat </value>
        private Attack selectedAttack = null;       /// <value> Attack selected by party member </value>
        private int countID = 0;                    /// <value> Unique ID for each character in combat </value>
        private int middleMonster = 0;              /// <value> Index of monster in the middle of the canvas, rounds down </value>
        private int maxEnemies = 5;                 /// <value> Max number of enemies that can appear on screen </value>
        private bool turn;                          /// <value> Current turn (PMTURN or MTURN) </value>
        private bool isFleePossible = true;         /// <value> Flag for if player can flee battle, will need to make this changeable in the future </value>

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
        IEnumerator Start() {
            actionsPanel.cm = this;
            actionsPanel.Init(isFleePossible);

            foreach (string monsterName in GameManager.instance.monsterNames) {
                yield return StartCoroutine(AddMonster(monsterName));
            }
            ArrangeMonsters();
            SetMonsterNavigation();
            
            partyMembers = PartyManager.instance.GetPartyMembers(ref countID);
            partyPanel.Init(partyMembers);

            foreach (Monster m in monsters) {
                cq.AddCharacter(m);
            }
            foreach (PartyMember pm in partyMembers) {
                cq.AddCharacter(pm);
            }

            cq.FinalizeQueue();
            StartCombat();
        }

        /// <summary>
        /// Starts combat by displaying the next party member's active turn
        /// and determining which character moves next
        /// </summary>
        private void StartCombat() {
            DisplayFirstPartyMember();          // active party member is not set
            actionsPanel.DisableAllActions();   // actions are all disabled until game determines who moves first
            GetNextTurn();
        }

        /// <summary>
        /// Determine's who's turn it is in the queue and then prepares the attacking character
        /// to either see their options if its a party member, or attack if its a monster.
        /// </summary>
        public void GetNextTurn() {
            Character c = cq.GetNextCharacter();
            bool prevTurn = turn;  // prevent redundant things that don't change between back to back turns of monsters/partymembers

            if (c is PartyMember) {
                activePartyMember = (PartyMember)c;
                turn = PMTURN;
            } else {
                activeMonster = (Monster)c;
                turn = MTURN; 
            }  
            
            if (turn) {
                if (prevTurn == MTURN) {
                    // stuff
                }
                PlayerTurn();
            } else {
                if (prevTurn == PMTURN) {
                    actionsPanel.DisableAllActions();
                }
                StartCoroutine(MonsterTurn());
            }
        }

        /// <summary>
        /// Prepare all monsters and relevant panels (Actions, Status) for the player's turn
        /// </summary>
        private void PlayerTurn() {
            DisplayActivePartyMember();
            EnableAllMonsterSelection();
            actionsPanel.EnableAllActions();
            actionsPanel.CheckAndSetActionsToUnusable(activePartyMember.CMP, activePartyMember.CHP);
            actionsPanel.SetHorizontalNavigation(partyPanel);
            partyPanel.EnableButtons();
            partyPanel.SetHorizontalNavigation(actionsPanel);
        }

        /// <summary>
        /// Ends combat by returning to the main menu scene.
        /// </summary>
        /// <remark> 
        /// For now, this leads back to the main menu. Later, this will need to lead to 
        /// either a defeat screen or a rewards screen
        /// </remark>
        private void EndCombat() {
            GameManager.instance.LoadNextScene("MainMenu");
        }

        /// <summary>
        /// Selects a monster to be attacked
        /// </summary>
        /// <param name="monsterToSelect"> Monster to select </param>
        /// <remark> Will probably make a UI info popup when clicking on monsters with no attack in the future </remark>
        public void SelectMonster(Monster monsterToSelect) {
            DeselectMonsters();
            
            selectedMonsters.Add(monsterToSelect);

            // Player can freely click on monsters without having an attack selected
            // Will probably make a UI popup when clicking on monsters with no attack in the future
            if (selectedAttack != null) {
                monsterToSelect.SelectMonsterButton();
                StartCoroutine(ExecutePMAttack(selectedAttack, monsterToSelect));     
            }
        }

        /// <summary>
        /// Selects multiple monster to be attacked
        /// </summary>
        /// <param name="monstersToSelect"> Monsters to select </param>
        /// <remark> Needs to be worked on for the future </remark>
        /* public void SelectMonster(List<Monster> monstersToSelect) {
            DeselectMonsters();

            foreach (Monster m in monstersToSelect) {
                m.SelectMonsterButton();
                selectedMonsters.Add(m);
            }
        }*/

        public void DeselectMonsters() {
            foreach (Monster m in selectedMonsters) {
                m.DeselectMonsterButton();
            }
            selectedMonsters.Clear();
        }

        /// <summary>
        /// Adds a monster GO to the enemy canvas, initializing its values and setting navigation
        /// </summary>
        /// <param name="monsterName"> Name of the monster to be fetched from the DB </param>
        private IEnumerator AddMonster(string monsterName) {
            GameObject newMonster = Instantiate(DataManager.instance.GetLoadedMonster(monsterName));
            newMonster.SetActive(true);
            Monster monsterComponent = newMonster.GetComponent<Monster>();
            
            monsterComponent.ID = countID++;
             monsterComponent.SetHealthBar();
            
            SelectMonsterDelegate smd = new SelectMonsterDelegate(SelectMonster);
            monsterComponent.AddSMDListener(smd);

            // assumes there will always be an action at button 0
            monsterComponent.SetNavigation("down", actionsPanel.GetActionButton(0));
            newMonster.transform.SetParent(enemyCanvas.transform, false);
            
            monsters.Add(monsterComponent);

            yield break;
        }

        /// <summary>
        /// Arranges monsters on screen to look nice depending on the number.
        /// Only a max of 5 monsters for now, although 3 large monsters, 4 normal 
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
                    float spacing0 = ((monsters[0].spriteWidth / 2) + 10) * -1;
                    float spacing1 = ((monsters[0].spriteWidth / 2) + 10);  

                    monsters[0].gameObject.transform.localPosition = new Vector3(spacing0, monsters[0].gameObject.transform.position.y, 0.0f);
                    monsters[1].gameObject.transform.localPosition = new Vector3(spacing1, monsters[1].gameObject.transform.position.y, 0.0f);  
                }
                else if (monsters.Count == 3) {
                    float spacing0 = ((monsters[1].spriteWidth / 2 + monsters[0].spriteWidth / 2) + 10) * -1;  
                    float spacing1 = 0;
                    float spacing2 = ((monsters[1].spriteWidth / 2 + monsters[2].spriteWidth / 2) + 10);
                    
                    monsters[0].gameObject.transform.localPosition = new Vector3(spacing0, monsters[0].gameObject.transform.position.y, 0.0f);
                    monsters[1].gameObject.transform.localPosition = new Vector3(spacing1, monsters[1].gameObject.transform.position.y, 0.0f);  
                    monsters[2].gameObject.transform.localPosition = new Vector3(spacing2, monsters[2].gameObject.transform.position.y, 0.0f);  
                }
                else if (monsters.Count == 4) {
                    float spacing0 = ((monsters[1].spriteWidth / 2) + monsters[0].spriteWidth + 30) * -1;  
                    float spacing1 = ((monsters[1].spriteWidth / 2) + 10) * -1; 
                    float spacing2 = (monsters[2].spriteWidth / 2) + 10;
                    float spacing3 = (monsters[2].spriteWidth / 2) + monsters[3].spriteWidth + 30;  

                    monsters[0].gameObject.transform.localPosition = new Vector3(spacing0, monsters[0].gameObject.transform.position.y, 0.0f);
                    monsters[1].gameObject.transform.localPosition = new Vector3(spacing1, monsters[1].gameObject.transform.position.y, 0.0f);  
                    monsters[2].gameObject.transform.localPosition = new Vector3(spacing2, monsters[2].gameObject.transform.position.y, 0.0f); 
                    monsters[3].gameObject.transform.localPosition = new Vector3(spacing3, monsters[3].gameObject.transform.position.y, 0.0f);   
                }
                else if (monsters.Count == 5) {
                    float spacing0 = ((monsters[2].spriteWidth / 2) + monsters[1].spriteWidth + monsters[0].spriteWidth / 2 + 30) * -1;  
                    float spacing1 = ((monsters[2].spriteWidth / 2) + monsters[1].spriteWidth / 2 + 10) * -1; 
                    float spacing2 = 0;
                    float spacing3 = (monsters[2].spriteWidth / 2) + monsters[3].spriteWidth / 2 + 10; 
                    float spacing4 = ((monsters[2].spriteWidth / 2) + monsters[3].spriteWidth + monsters[4].spriteWidth / 2 + 30);  

                    monsters[0].gameObject.transform.localPosition = new Vector3(spacing0, monsters[0].gameObject.transform.position.y, 0.0f);
                    monsters[1].gameObject.transform.localPosition = new Vector3(spacing1, monsters[1].gameObject.transform.position.y, 0.0f);  
                    monsters[2].gameObject.transform.localPosition = new Vector3(spacing2, monsters[2].gameObject.transform.position.y, 0.0f); 
                    monsters[3].gameObject.transform.localPosition = new Vector3(spacing3, monsters[3].gameObject.transform.position.y, 0.0f);
                    monsters[4].gameObject.transform.localPosition = new Vector3(spacing4, monsters[4].gameObject.transform.position.y, 0.0f);      
                } 
            }
        }

        private void SetMonsterNavigation() {
            foreach (Monster m in monsters) {
                m.ResetNavigation();
            }
            foreach(Monster m in monsters) {
                m.SetNavigation("down", actionsPanel.GetActionButton(0));
            }

            if (monsters.Count > 1) {
                for (int i = 0; i < monsters.Count; i++) {
                    if (i == 0) {
                        monsters[i].SetNavigation("right", monsters[i + 1].b);
                    }
                    else if (i == monsters.Count - 1) {
                        monsters[i].SetNavigation("left", monsters[i - 1].b);
                    }
                    else {
                        monsters[i].SetNavigation("left", monsters[i - 1].b);
                        monsters[i].SetNavigation("right", monsters[i + 1].b);
                    }
                }
            }
            
            middleMonster = (int)(Mathf.Floor(monsters.Count / 2f));
            actionsPanel.SetButtonNavigation(0, "up", monsters[middleMonster].b);
            actionsPanel.SetButtonNavigation(1, "up", monsters[middleMonster].b);
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
                m.SetNavigation("down", actionsPanel.GetActionButton(4));
            }
            actionsPanel.SetButtonNavigation(4, "up", monsters[middleMonster].b);
            partyPanel.SetHorizontalNavigation(actionsPanel);

            es.SetSelectedGameObject(monsters[middleMonster].b.gameObject);
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
        /// Need to split this up into a cleanup phase function
        /// </returns>
        public IEnumerator ExecutePMAttack(Attack a, Monster m) {
            actionsPanel.DisableAllActions();
            partyPanel.DisableButtons();
            DisableAllMonsterSelection();

            yield return StartCoroutine(activePartyMember.PayAttackCost(a.costType, a.cost));
            yield return StartCoroutine(m.LoseHP(a.damage, a.animationClipName));
            
            if (m.CheckDeath()) {           // need to clean this up
                cq.RemoveCharacter(m.ID);
                monsters.Remove(m);
                DeselectMonsters();
                yield return StartCoroutine(m.Die());
            }

            EndTurn();
        }

        public void EndTurn() {
            if (CheckBattleOver()) {
                EndCombat();
            }
            else {
                selectedAttack = null;
                DeselectMonsters();
                actionsPanel.ResetFifthButtonNavigation();
                SetMonsterNavigation();

                GetNextTurn();
            } 
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
        /// Start the active monster's turn
        /// </summary>
        /// <returns> Yields to allow monster attack animation to play </returns>
        private IEnumerator MonsterTurn() {
            List<PartyMember> partyMembersToRemove = new List<PartyMember>();
            yield return StartCoroutine(ExecuteMonsterAttack());
            foreach (PartyMember pm in partyMembers) {
                if (pm.CheckDeath()) {
                    cq.RemoveCharacter(pm.ID);
                    partyMembersToRemove.Add(pm);
                }
            }
            foreach (PartyMember pm in partyMembersToRemove) {
                partyMembers.Remove(pm);
            }
            if (CheckBattleOver()) {
                EndCombat();
            } 
            else {
                GetNextTurn();
            }
        }
        
        /// <summary>
        /// Executes a monster's attack, by first getting the attack it wants to use, and then selecting
        /// its target based on its AI
        /// </summary>
        /// <returns> IEnumerator to play animations after each action </returns>
        private IEnumerator ExecuteMonsterAttack() {
            int targetChoice = 0;
            Attack attackChoice = activeMonster.SelectAttack();
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

            yield return (StartCoroutine(activeMonster.PlayAttackAnimation()));
            if (attackChoice.damage > 0) {
                yield return (StartCoroutine(partyMembers[targetChoice].LoseHP(attackChoice.damage, partyMembers[targetChoice] == activePartyMember)));
            }
        }

        /// <summary>
        /// Changes the UI to reflect the first party member in the queue's information
        /// </summary>s
        public void DisplayFirstPartyMember() {
            activePartyMember = cq.GetFirstPM();        // ActivePartyMember will be redundantly set a second time
            actionsPanel.SetAttackActions(activePartyMember.attacks);
            statusPanel.Init(activePartyMember);
        }

        /// <summary>
        /// Changes the UI to reflect the active party member's information
        /// </summary>s
        public void DisplayActivePartyMember() {
            if (activePartyMember != null) {
                actionsPanel.SetAttackActions(activePartyMember.attacks);
                statusPanel.Init(activePartyMember);
            }
        }

        /// <summary>
        /// Revert target selection UI to action selection phase
        /// </summary>
        public void UndoPMAction() {
            selectedAttack = null;
            foreach(Monster m in monsters) {
                m.SetNavigation("down", actionsPanel.GetActionButton(4));
            }
            actionsPanel.CheckAndSetActionsToUnusable(activePartyMember.CMP, activePartyMember.CHP);
            actionsPanel.ResetFifthButtonNavigation();
            partyPanel.SetHorizontalNavigation(actionsPanel);
        }

        /// <summary>
        /// Disables all interaction of monsters
        /// </summary>
        public void DisableAllMonsterSelection() {
            foreach (Monster m in monsters) {
                m.DisableInteraction();
            }
        }  

        /// <summary>
        /// Enables selection of all monsters
        /// </summary>
        public void EnableAllMonsterSelection() {
            foreach (Monster m in monsters) {
                m.EnableInteraction();
            }
        }

        /* private void SetStatus() {
            statusManager.Init(activePartyMember);
        } */
    }
}
