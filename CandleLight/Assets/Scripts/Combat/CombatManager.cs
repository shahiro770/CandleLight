/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 11, 2019
* 
* The CombatManager class is used to manage all classes during the combat scene.
* Party members, monsters, actions, and turn ordering are all managed in the combat manager.
*
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using DataBank;

// STILL NEED A FUNCTION TO COLOUR THE UI THE AREA DEFAULT

namespace Combat {

    public delegate void SelectMonsterDelegate(Monster m);
    public class CombatManager : MonoBehaviour {

        private readonly bool PMTURN = true;        /// <value> Flag for party member turn </value>
        private readonly bool MTURN = false;        /// <value> Flag for monster turn </value>

        public static CombatManager instance;       /// <value> Combat scene instance </value>
        
        public Canvas enemyCanvas;                  /// <value> Canvas for where monsters are displayed </value>
        public StatusManager statusManager;         /// <value> Manager for active party member's status </value>
        public ActionsManager am;                   /// <value> Manager for active party member's actions </value>
        public GameObject monster;                  /// <value> Monster GO to instantiate </value>
        
        private EventSystem es;                     /// <value> EventSystem reference </value>
        private List<PartyMember> partyMembers = new List<PartyMember>();   /// <value> List of party members </value>
        private PartyMember activePartyMember;      /// <value> Current party member preparing to act </value>
        private List<Monster> monsters =  new List<Monster>();              /// <value> List of monsters </value>
        private Monster activeMonster;              /// <value> Current monster preparing to act </value>
        private List<Monster> selectedMonsters = new List<Monster>();       /// <value> List of monsters selected </value>
        private CharacterQueue cq = new CharacterQueue();                   /// <value> Queue for attacking order in combat </value>
        private Attack selectedAttack = null;       /// <value> Attack selected by party member </value>
        private int countID = 0;                    /// <value> Unique ID for each character in combat </value>
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
        void Start() {
            am.cm = this;
            am.Init(isFleePossible);

            foreach (string monsterName in GameManager.instance.monsterNames) {
                AddMonster(monsterName);
            }
            partyMembers = PartyManager.instance.GetPartyMembers(ref countID);

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
            DisplayFirstPartyMember(); // active party member is not set
            GetNextTurn();
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
        public void SelectMonster(Monster monsterToSelect) {
            foreach (Monster m in selectedMonsters) {
                m.DeselectMonsterButton();
            }
            selectedMonsters.Clear();

            selectedMonsters.Add(monsterToSelect);

            if (selectedAttack != null) {
                monsterToSelect.SelectMonsterButton();
                StartCoroutine(ExecuteAttack(selectedAttack, monsterToSelect));
            }
        }

        /// <summary>
        /// Selects multiple monster to be attacked
        /// </summary>
        /// <param name="monstersToSelect"> Monsters to select </param>
        /// <remark> Needs to be worked on for the future </remark>
        public void SelectMonster(List<Monster> monstersToSelect) {
            foreach (Monster m in selectedMonsters) {
                m.DeselectMonsterButton();
            }
            selectedMonsters.Clear();

            foreach (Monster m in monstersToSelect) {
                m.SelectMonsterButton();
                selectedMonsters.Add(m);
            }
        }

        /// <summary>
        /// Adds a monster GO to the enemy canvas, initializing its values and setting navigation
        /// </summary>
        /// <param name="monsterName"> Name of the monster to be fetched from the DB </param>
        private void AddMonster(string monsterName) {
            GameObject newMonster = Instantiate(monster);
            Monster monsterComponent = newMonster.GetComponent<Monster>() ;
            
            GameManager.instance.DB.GetMonsterByNameID(monsterName, monsterComponent);
            monsterComponent.ID = countID++;
            SelectMonsterDelegate smd = new SelectMonsterDelegate(SelectMonster);
            monsterComponent.AddSMDListener(smd);
            monsterComponent.SetNavigation(am.GetActionButton(0));
            newMonster.transform.SetParent(enemyCanvas.transform, false);
            monsters.Add(monsterComponent);

            if (monsters.Count == 1) {
                am.SetButtonNavigation(0, "up", monsters[0].b);
                am.SetButtonNavigation(1, "up", monsters[0].b);
            }
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
            monsters[0].SetNavigation(am.GetActionButton(4));
            am.SetButtonNavigation(4, "up", monsters[0].b);

            es.SetSelectedGameObject(monsters[0].b.gameObject);
        }

        /// <summary>
        /// Deals damage to a monster.
        /// If the monster were to die, see if combat is finished, otherwise reset most of the UI
        /// to as if it were the start of the player's turn, and then determine the next turn.
        /// </summary>
        /// <param name="a"> Attack to be executed </param>
        /// <param name="m"> Monster to be attacked </param>
        /// <returns> 
        /// Yields to allow animations to play out when a monster is being attacked or
        /// taking damage
        /// </returns>
        public IEnumerator ExecuteAttack(Attack a, Monster m) {
            am.DisableAllActions();
            DisableAllMonsterSelection();
            yield return StartCoroutine(m.LoseHP(a.damage));
            if (m.CheckDeath()) {
                cq.RemoveCharacter(m.ID);
                yield return StartCoroutine(m.Die());
                if (CheckBattleOver()) {
                    EndCombat();
                } 
            }

            m.DeselectMonsterButton();
            am.ResetFifthButtonNavigation();
            selectedAttack = null;
            monsters[0].SetNavigation(am.GetActionButton(0));

            GetNextTurn();
        }

        /// <summary>
        /// Revert target selection UI to action selection phase
        /// </summary>
        public void UndoPMAction() {
            selectedAttack = null;
            monsters[0].SetNavigation(am.GetActionButton(0));
            am.ResetFifthButtonNavigation();
        }

        /// <summary>
        /// Determine's who's turn it is in the queue and then prepares the attacking character
        /// to either see their options if its a party member, or attack if its a monster.
        /// </summary>
        public void GetNextTurn() {
            Character c = cq.GetNextCharacter();

            if (c is PartyMember) {
                activePartyMember = (PartyMember)c;
                turn = PMTURN;
            } else {
                activeMonster = (Monster)c;
                turn = MTURN;
            }  

            if (turn) {
                DisplayActivePartyMember();
                EnableAllMonsterSelection();
                am.EnableAllActions();   
            } else {
                am.DisableAllActions();
                StartCoroutine(MonsterTurn());
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
            yield return StartCoroutine(activeMonster.Attack(partyMembers));
            foreach (PartyMember pm in partyMembers) {
                if (pm.CheckDeath()) {
                    cq.RemoveCharacter(pm.ID);
                }
            }
            if (CheckBattleOver()) {
                EndCombat();
            } 
            else {
                GetNextTurn();
            }
        }

        /// <summary>
        /// Changes the UI to reflect the first party member in the queue's information
        /// </summary>s
        public void DisplayFirstPartyMember() {
            activePartyMember = cq.GetFirstPM();        // ActivePartyMember will be redundantly set a second
            am.SetAttackActions(activePartyMember.attacks);
            statusManager.Init(activePartyMember);
        }

        /// <summary>
        /// Changes the UI to reflect the active party member's information
        /// </summary>s
        public void DisplayActivePartyMember() {
            if (activePartyMember != null) {
                am.SetAttackActions(activePartyMember.attacks);
                statusManager.Init(activePartyMember);
            }
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
