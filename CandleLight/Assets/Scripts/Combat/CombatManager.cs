/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 11, 2019
* 
* The CombatManager class is used to manage all classes during the combat scene.
* PartyMembers, monsters, actions, and turn ordering are all managed in the combat manager.
*
*/

using Constants;
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
        private readonly bool PMTURN = true;        /// <value> Flag for partyMember turn </value>
        private readonly bool MTURN = false;        /// <value> Flag for monster turn </value>

        public static CombatManager instance;       /// <value> Combat scene instance </value>

        /* external component references */
        public Canvas enemyCanvas;                  /// <value> Canvas for where monsters are displayed </value>
        public EventDescription eventDescription;   /// <value> eventDescription reference </value>
        public StatusPanel statusPanel;             /// <value> statusPanel reference </value>
        public ActionsPanel actionsPanel;           /// <value> actionsPanel reference </value>
        public GearPanel gearPanel;                 /// <value> gearPanel reference </value>
        public CandlesPanel candlesPanel;           /// <value> candlesPanel reference </value>
        public SpecialPanel specialPanel;           /// <value> SpecialPanel reference </value>
        public PartyPanel partyPanel;               /// <value> partyPanel reference </value>
        public SkillsPanel skillsPanel;             /// <value> skillsPanel reference </value>
        public TabManager itemsTabManager;          /// <value> Click on to display other panels </value>
        public TabManager utilityTabManager;        /// <value> Click on to display other panels </value>
        public GameObject monster;                  /// <value> Monster GO to instantiate </value>

        public bool inCombat { get; private set; } = false;       
        public bool isReady { get; private set; } = false;                  /// <value> Localization happens at the start, program loads while waiting </value>
        public List<Monster> enemiesKilled { get; private set; }           /// <value> List of monsters killed in combat instance </value>
        
        private EventSystem es;                                             /// <value> EventSystem reference </value>
        private List<PartyMember> partyMembersAll = new List<PartyMember>();    /// <value> List of partyMembers </value>
        private List<PartyMember> partyMembersAlive = new List<PartyMember>();  /// <value> List of partyMembers that are alive </value>
        private PartyMember activePartyMember;                              /// <value> Current partyMember preparing to act </value>
        private PartyMember selectedPartyMember;                            /// <value> Selected partyMember for heal/buff from party </value>
        private List<Monster> monsters =  new List<Monster>();              /// <value> List of monsters </value>
        private List<Monster> selectedMonsterAdjacents = new List<Monster>();   /// <value> List of monsters adjacent to the monster selected that will be affected </value>
        private Monster selectedMonster = null;                             /// <value> Selected monster </value>
        private Monster activeMonster;                                      /// <value> Current monster preparing to act </value>
        private CharacterQueue cq = new CharacterQueue();                   /// <value> Queue for attacking order in combat </value>
        private Attack selectedAttackMonster = null;/// <value> Attack selected by monster </value>
        private Attack selectedAttackPM = null;     /// <value> Attack selected by partyMember </value>
        private string[] championBuffs;             /// <value> List of buffs monsters can be spawn with </value>
        private int countID = 0;                    /// <value> Unique ID for each character in combat </value>
        private int middleMonster = 0;              /// <value> Index of monster in the middle of the canvas, rounds down </value>
        private int maxMonsters = 5;                /// <value> Max number of enemies that can appear on screen </value>
        private int selectedMonsterAttackIndex;     /// <value> Index of selectedMonster attack in its attack array </value>
        private int fleeBonus;                      /// <value> Repeated flee fails increase future flee chances </value>
        private bool turn;                          /// <value> Current turn (PMTURN or MTURN) </value>
        private bool pmSelectionFinalized = false;  /// <value> Flag for if player has selected an action and a monster if its an attack </value>
        private bool pmNoAction = false;            /// <value> Flag for if active partyMember can't do anything </value>
        private bool mNoAction = false;             /// <value> Flag for if active monster can't do anything </value>
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
        /// <param name="monsterNames"> Names of monsters to spawn in for combat </param>
        /// <param name="isFleePossible"> Flag for if the player can flee the combat event </param>
        public IEnumerator InitializeCombat(string[] monsterNames, string[] championBuffs, bool isFleePossible) {
            actionsPanel.Init(isFleePossible);
            isFleeSuccessful = false;
            fleeBonus = 0;
            inCombat = true;
            cq.Reset();
            monsters = new List<Monster>();
            enemiesKilled = new List<Monster>();
            partyMembersAlive = new List<PartyMember>();
            partyMembersAll = PartyManager.instance.GetPartyMembers();
            this.championBuffs = championBuffs;
            PartyManager.instance.SetBonusChampionChance();
            countID = partyMembersAll.Count + 1;  // monsters will be assigned unique ID numbers, incrementing off of the last partymember's ID
            foreach (PartyMember pm in partyMembersAll) {
                if (pm.CheckDeath() == false) {
                    partyMembersAlive.Add(pm);
                    pm.turnCounter = 0;
                    if (pm.className == ClassConstants.ARCHER) {
                        if (pm.skills[(int)SkillConstants.archerSkills.VANTAGEPOINT].skillEnabled == true) {
                            pm.AddStatusEffect(StatusEffectConstants.ADVANTAGE, 1, null);
                        }
                    }
                }
            }

            foreach (string monsterName in monsterNames) {
                yield return StartCoroutine(AddMonster(monsterName));
            }
            ArrangeMonsters();
            
            foreach (Monster m in monsters) {
                cq.AddCharacter(m);
            }
            foreach (PartyMember pm in partyMembersAlive) {
                cq.AddCharacter(pm);
            }
            cq.FinalizeQueue();

            yield return (StartCoroutine(DisplayCombatIntro()));
            
            StartCombat();
        }

        /// <summary>
        /// Adds a monster GO to the enemy canvas, initializing its values
        /// </summary>
        /// <param name="monsterName"> Name of the monster to be fetched from the DB </param>
        /// <returns> IEnumerator cause animations </returns>
        /// <remark> Assumes there will always be an action at button 0 </remark>
        private IEnumerator AddMonster(string monsterName) {
            GameObject newMonster = Instantiate(DataManager.instance.GetLoadedMonsterDisplay(monsterName));
            newMonster.SetActive(true); // monster game object must be active to manipulate its components
            
            Monster monsterComponent = newMonster.GetComponent<Monster>();
            SelectMonsterDelegate smd = new SelectMonsterDelegate(SelectMonster);
            
            monsterComponent.ID = countID++;
            monsterComponent.turnCounter = 0;
            monsterComponent.MultipleLVLUp(EventManager.instance.subAreaProgress);
            monsterComponent.ApplyDifficultyChanges(); 
            monsterComponent.GetBuffs(championBuffs);
            monsterComponent.md.AddSMDListener(smd);
            monsterComponent.md.SetAlternateColourBlock();
            monsterComponent.md.SetInteractable(false, false);

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
            actionsPanel.SetActionsUsable(true);
            actionsPanel.SetAllActionsUninteractableAndFadeOut();
            if (partyPanel.isOpen == false) {
                utilityTabManager.OpenPanel(0);
            }
            if (candlesPanel.isOpen == false) {
                itemsTabManager.OpenPanel(1);
            }
            if (GameManager.instance.tutorialTriggers[(int)TutorialConstants.tutorialTriggers.isTutorial] == true) {
                EventManager.instance.SetToastPanelsVisible(false);
            }
            DisableAllButtons();
            foreach (Monster m in monsters) {
                StartCoroutine(m.md.PlaySpawnAnimation());
            }
                  
            if (eventDescription.HasText()) {
                yield return new WaitForSeconds(1.5f / GameManager.instance.animationSpeed);   
                eventDescription.ClearText();    
            }
            else {
                yield return new WaitForSeconds(0.3f / GameManager.instance.animationSpeed);   
            }   
        }

        /// <summary>
        /// Starts combat by displaying the next partyMember's active turn
        /// and determining which character moves next
        /// </summary>
        private void StartCombat() {
            DisplayFirstPartyMember();          // active partyMember is not set
            DisableAllMonsterSelection();

            GetNextTurn();
        }

        #endregion

        /// <summary>
        /// Determine's who's turn it is in the queue and then prepares the attacking character
        /// to either see their options if its a partyMember, or attack if its a monster.
        /// </summary>
        public void GetNextTurn() {
            Character c = cq.GetNextCharacter();

            if (c is PartyMember) {
                activePartyMember = (PartyMember)c;
                PartyManager.instance.SetActivePartyMember(activePartyMember);
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

        #region [ Section 1 ] PartyMember Turn

        /// <summary>
        /// Perform all of the phases in the partyMember's turn
        /// </summary>
        /// <returns> IEnumerator so actions are all taken in order </returns>
        private IEnumerator PMTurn() {
            StartPMTurn();
            while (!pmSelectionFinalized) {    // (PreparePMAttack and SelectMonster or SelectPartyMember) or AttemptFlee
                yield return null;
            }
            DisableAllButtons();
            if (selectedAttackPM != null || pmNoAction == true) {
                yield return StartCoroutine(ExecutePMAttack());  
            }
            yield return StartCoroutine(CleanUpPMTurn());
            if (CheckBattleOver() || isFleeSuccessful) {
                EndCombat();
            }
            else {
                GetNextTurn();
            }
        }

        /// <summary>
        /// Prepare all monsters and relevant panels (Actions, Status) for the player's turn
        /// </summary>
        private void StartPMTurn() {
            activePartyMember.SetAttackValues();
            activePartyMember.turnCounter++;
            DisplayActivePartyMember();

            pmNoAction = activePartyMember.GetStatusEffect(StatusEffectConstants.STUN) != -1;
            if (activePartyMember.turnCounter % 3 == 0 && activePartyMember.className == ClassConstants.WARRIOR) {
                if (activePartyMember.skills[(int)SkillConstants.warriorSkills.THUNDEROUSANGER].skillEnabled == true) {
                    foreach (Monster m in monsters) {
                        m.AddStatusEffect(StatusEffectConstants.SHOCK, 1, null);
                    }
                }
            }

            if (pmNoAction == true) {
                pmSelectionFinalized = true;
            }
            else {
                EnableAllButtonsInSidePanels();
                eventDescription.ClearText();
                if (GameManager.instance.tutorialTriggers[(int)TutorialConstants.tutorialTriggers.isTutorial] == true) {
                    EventManager.instance.ProgressTutorial();
                }
                if (GameManager.instance.tutorialTriggers[(int)TutorialConstants.tutorialTriggers.isTips] == true && GameManager.instance.tutorialTriggers[(int)TutorialConstants.tutorialTriggers.firstCandleCombat]  == true 
                && PartyManager.instance.IsCandlesEquipped() == true) {
                    EventManager.instance.SetTutorialNotification("candles1");
                    GameManager.instance.tutorialTriggers[(int)TutorialConstants.tutorialTriggers.firstCandleCombat] = false;
                }
                if (GameManager.instance.tutorialTriggers[(int)TutorialConstants.tutorialTriggers.isTips]  == true && GameManager.instance.tutorialTriggers[(int)TutorialConstants.tutorialTriggers.firstChampion] == true) {
                    foreach(Monster m in monsters) {
                        if (m.isChampion == true) {
                            EventManager.instance.SetTutorialNotification("champion");
                            GameManager.instance.tutorialTriggers[(int)TutorialConstants.tutorialTriggers.firstChampion] = false;
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// On clicking an attack, handles any additional logic that is required
        /// </summary>
        /// <param name="a"> Attack to be executed </param>
        public void PreparePMAttack(Attack a) {
            selectedAttackPM = a;

            // highlight the party if the attack targets the party
            if (selectedAttackPM.type == AttackConstants.HEALHP || selectedAttackPM.type == AttackConstants.BUFF) {          
                partyPanel.SetBlinkSelectables(selectedAttackPM, true);
            }
            else if (selectedAttackPM.type == AttackConstants.SUMMON) {
                pmSelectionFinalized = true;
            }
            else {
                partyPanel.SetBlinkSelectables(null, false);
            }

            if (GameManager.instance.tutorialTriggers[(int)TutorialConstants.tutorialTriggers.isTutorial] == true) {
                EventManager.instance.ProgressTutorial();
            }  
        }

        /// <summary>
        /// Randomly determine if the player is able to end combat before killing all monsters
        /// </summary>
        /// <returns> IEnumerator for dramatic timing and animations </returns>
        /// <remark> Death animation for each monster is played as a de-spawning animation</remark>
        public IEnumerator AttemptFlee() {
            // partyMembers with higher luck will have a better chance at escaping
            int fleeChanceModifier = (activePartyMember.LVL - monsters[0].LVL) * 10;
            int chance = Random.Range(activePartyMember.LVL + fleeChanceModifier, 100 + fleeChanceModifier) ;
            List<Monster> monstersToRemove = new List<Monster>();

            // flee chance modifiers
            chance -= fleeBonus;
            if (activePartyMember.GetStatusEffect(StatusEffectConstants.TRAP) != -1) {
                chance *= 2;
            }
            if (activePartyMember.className == ClassConstants.ARCHER) {
                if (activePartyMember.skills[(int)SkillConstants.archerSkills.FLEETFOOTED].skillEnabled == true) {
                    chance = (int)(chance * 0.5f);
                }
            }

            // wait for all monsters to "despawn"
            DisableAllButtonsFlee();
            eventDescription.ClearText();
            for (int i = 0; i < monsters.Count - 1; i++) {  
                StartCoroutine(monsters[i].md.PlayDeathAnimation());
                monstersToRemove.Add(monsters[i]);
            }
            yield return (StartCoroutine(monsters[monsters.Count - 1].md.PlayDeathAnimation())); // yield to last monster so we wait for all monsters to die
            monstersToRemove.Add(monsters[monsters.Count - 1]);
            yield return new WaitForSeconds(Random.Range(0.5f, 1f));

            if (chance < 50) {
                DestroyMonsters(monstersToRemove);
                isFleeSuccessful = true;
            }
            else {
                fleeBonus = (fleeBonus + 2) * 2;
                eventDescription.SetKey(GetFleeFailedKey());
                for (int i = 0; i < monsters.Count - 1; i++) {  // wait for all monsters to respawn
                    StartCoroutine(monsters[i].md.PlaySpawnAnimation());
                    monstersToRemove.Add(monsters[i]);
                }
                yield return (StartCoroutine(monsters[monsters.Count - 1].md.PlaySpawnAnimation())); 
                monstersToRemove.Add(monsters[monsters.Count - 1]);
                yield return new WaitForSeconds(0.25f);
            }
            
            selectedAttackPM = null;
            pmSelectionFinalized = true;
        }

        /// <summary>
        /// Select a partyMember from the partyPanel as the target of an attack
        /// </summary>
        /// <param name="pmToSelect"> PartyMember to select </param>
        public void SelectPartyMember(PartyMember pmToSelect) {
            if (pmToSelect.CheckDeath() == false) {
                selectedPartyMember = pmToSelect;
                partyPanel.SetBlinkSelectables(null, false);

                pmSelectionFinalized = true;
            }
        }

        /// <summary>
        /// Use a candle's attack instead of a normal attack as an action
        /// </summary>
        /// <param name="index"> Equips a candle to one of the active candle slots (0, 1, or 2) </param>
        public void UseCandle(int index) {
            if (PartyManager.instance.GetActivePartyMember().ID == activePartyMember.ID) {
                selectedAttackPM = activePartyMember.activeCandles[index].a;
                activePartyMember.activeCandles[index].Use();
                pmSelectionFinalized = true;
            }
        }

        /// <summary>
        /// Deals damage to a monster.
        /// If the monster were to die, see if combat is finished, otherwise reset most of the UI
        /// to as if it were the start of the player's turn, and then determine the next turn.
        /// </summary>
        /// <returns> 
        /// Yields to allow animations to play out when a monster is being attacked or taking damage
        /// </returns>
        public IEnumerator ExecutePMAttack() { 
            if (GameManager.instance.tutorialTriggers[(int)TutorialConstants.tutorialTriggers.isTutorial] == true) {
                EventManager.instance.ProgressTutorial();
            } 
            if (pmNoAction == true) {
                eventDescription.SetNoMoveTextPM(activePartyMember.pmName);
                yield return new WaitForSeconds(1f);
            }
            else {
                eventDescription.SetKey(selectedAttackPM.nameKey); 
                yield return new WaitForSeconds(0.25f);

                yield return (StartCoroutine(activePartyMember.PayAttackCost(selectedAttackPM.costType, selectedAttackPM.costValue)));
                if (selectedAttackPM.scope == "single") {
                    if (selectedAttackPM.type == AttackConstants.PHYSICAL || selectedAttackPM.type == AttackConstants.MAGICAL) {
                        yield return (StartCoroutine(selectedMonster.GetAttacked(selectedAttackPM, activePartyMember)));    
                    }
                    else if (selectedAttackPM.type == AttackConstants.DEBUFF) {
                        yield return (StartCoroutine(selectedMonster.GetStatusEffected(selectedAttackPM, activePartyMember)));
                    }
                    else if (selectedAttackPM.type == AttackConstants.HEALHP || selectedAttackPM.type == AttackConstants.BUFF) {
                        yield return (StartCoroutine(selectedPartyMember.GetHelped(selectedAttackPM, activePartyMember)));
                    }
                    else if (selectedAttackPM.type == AttackConstants.HEALMPSELF || selectedAttackPM.type == AttackConstants.HEALHPSELF || selectedAttackPM.type == AttackConstants.BUFFSELF) {
                        yield return (StartCoroutine(activePartyMember.GetHelped(selectedAttackPM, activePartyMember)));
                    }
                    else if (selectedAttackPM.type == AttackConstants.SUMMON) {
                        if (activePartyMember.summon == null) {     // initial summon
                            PartyManager.instance.AddPartyMemberSummon(activePartyMember, selectedAttackPM.name, countID++);
                            cq.AddCharacterAndResort(activePartyMember.summon);
                            partyMembersAlive.Add(activePartyMember.summon);
                            yield return (StartCoroutine(activePartyMember.summon.GetSummoned(selectedAttackPM, false)));
                        }
                        else {
                            if (activePartyMember.summon.CheckDeath() == true) {    // revive
                                PartyManager.instance.RestoreSummon(activePartyMember.summon);
                                cq.AddCharacterAndResort(activePartyMember.summon);
                                partyMembersAlive.Add(activePartyMember.summon);
                            }
                            else {  // full heal and purge
                                PartyManager.instance.RestoreSummon(activePartyMember.summon);
                            }
                            yield return (StartCoroutine(activePartyMember.summon.GetSummoned(selectedAttackPM, true)));
                        }
                        
                    }
                }
                else if (selectedAttackPM.scope == "adjacent") {
                    if (selectedAttackPM.type == AttackConstants.PHYSICAL || selectedAttackPM.type == AttackConstants.MAGICAL) {
                        foreach (Monster m in selectedMonsterAdjacents) {
                            StartCoroutine(m.GetAttacked(selectedAttackPM, activePartyMember));    
                        }
                        StartCoroutine(selectedMonster.GetAttacked(selectedAttackPM, activePartyMember));    
                    
                        foreach (Monster m in monsters) {           // this will include monsters not being attacked, but the cost is negligible
                            while (m.IsDoneAnimating() == false) {
                                yield return null;
                            }
                        }
                    }
                }
                else if (selectedAttackPM.scope == "allEnemies") {
                    if (selectedAttackPM.type == AttackConstants.DEBUFF) {
                        for (int i = 0; i < monsters.Count; i++) {
                            StartCoroutine(monsters[i].GetStatusEffected(selectedAttackPM, activePartyMember)); 
                        }
                    }

                    foreach (Monster m in monsters) {
                        while (m.IsDoneAnimating() == false) {
                            yield return null;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Resolve end of turn effects (monster's being dead, end of turn debuffs, etc.)
        /// </summary>
        /// <returns> IEnumerator for animations </returns>
        public IEnumerator CleanUpPMTurn() {
            List<Monster> monstersToRemove = new List<Monster>();
            List<PartyMember> partyMembersToRemove = new List<PartyMember>();
               
            yield return StartCoroutine(activePartyMember.TriggerStatuses(true));  
            foreach (Monster m in monsters) {
                if (m.CheckDeath()) {
                    cq.RemoveCharacter(m.ID);
                    monstersToRemove.Add(m);
                    enemiesKilled.Add(m);

                    yield return StartCoroutine(m.md.PlayDeathAnimation());
                }
            }
            DestroyMonsters(monstersToRemove);

            foreach (PartyMember pm in partyMembersAlive) {
                if (pm.CheckDeath()) {
                    cq.RemoveCharacter(pm.ID);
                    partyMembersToRemove.Add(pm);
                }
            }
            foreach (PartyMember pm in partyMembersToRemove) {
                partyMembersAlive.Remove(pm);
            }

            DeselectMonsters();
            selectedPartyMember = null;
            selectedAttackPM = null;
            pmSelectionFinalized = false;
            pmNoAction = false;
        }

        #endregion

        #region [ Section 2 ] Monster Turn

        /// <summary>
        /// Perform all of the phases in the monster's turn
        /// </summary>
        /// <returns></returns>
        private IEnumerator MonsterTurn() {
            StartMonsterTurn();
            yield return StartCoroutine(ExecuteMonsterAttack());
            yield return StartCoroutine(CleanUpMonsterTurn());
            if (CheckBattleOver()) {
                EndCombat();
            }
            else {
                GetNextTurn();
            }
        }

        /// <summary>
        /// Start the active monster's turn
        /// </summary>
        /// <returns> Yields to allow monster attack animation to play </returns>
        private void StartMonsterTurn() {
            DisableAllButtons();
            activeMonster.turnCounter++;
            activeMonster.SetAttackValues();
        }

         /// <summary>
        /// Returns the active monster's selected attack based on its AI
        /// </summary>
        /// <returns> An Attack object to be used </returns>
        public Attack SelectMonsterAttack(PartyMember pm) {
            Attack[] attacks = activeMonster.attacks;
            string monsterAI = activeMonster.monsterAI;
            int attackNum = activeMonster.attackNum;
            selectedMonsterAttackIndex = -1;

            if (monsterAI == "random" || monsterAI == "weakHunter") {
                selectedMonsterAttackIndex = Random.Range(0, attackNum);
            }
            else if (monsterAI == "bleedHunter") {  // if a partyMember is bleeding, use last attack
                bool isBleeding = false;

                for (int i = 0; i < partyMembersAlive.Count; i++) {
                    if (partyMembersAlive[i].GetStatusEffect("bleed") != -1) {
                        isBleeding = true;
                        break;
                    }
                }

                if (isBleeding) {
                    selectedMonsterAttackIndex = attackNum - 1;
                }
                else {
                    selectedMonsterAttackIndex = Random.Range(0, attackNum - 1);
                }
            }
            else if (monsterAI == "lastAt60" || monsterAI == "weakHunterLastAt60") {    // only uses last attack after CHP falls below 60%, using it whenever possible if its a selfBuff
                if (activeMonster.CHP <= (int)(activeMonster.HP * 0.6f)) {

                    if ((attacks[attackNum - 1].type == AttackConstants.BUFFSELF || attacks[attackNum - 1].type == AttackConstants.HEALHPSELF
                    || attacks[attackNum - 1].type == AttackConstants.BUFF) && activeMonster.GetStatusEffect(attacks[attackNum - 1].seName) == -1) {
                        selectedMonsterAttackIndex = attackNum - 1;
                    }
                    else if ((attacks[attackNum - 1].type == AttackConstants.PHYSICAL || attacks[attackNum - 1].type == AttackConstants.MAGICAL)) {
                        selectedMonsterAttackIndex = Random.Range(0, attackNum);
                    }
                    else {
                        selectedMonsterAttackIndex = Random.Range(0, attackNum - 1);
                    }
                }
                else {
                    selectedMonsterAttackIndex = Random.Range(0, attackNum - 1);
                }
            } 
            else if (monsterAI == "debuffer") {
                int randomStart = Random.Range(0, attackNum);
                int circularIndex;
                for (int i = 0; i < attackNum; i++) {
                    circularIndex = (randomStart + i) % attackNum;
                    if (attacks[circularIndex].type == AttackConstants.DEBUFF && pm.GetStatusEffect(attacks[circularIndex].seName) == -1) {
                        selectedMonsterAttackIndex = circularIndex;
                        break;
                    }
                }
                if (selectedMonsterAttackIndex == -1) {
                    for (int i = 0; i < attackNum; i++) {
                        circularIndex = (randomStart + i) % attackNum;
                        if (attacks[circularIndex].type != AttackConstants.DEBUFF) {
                            selectedMonsterAttackIndex = circularIndex;
                            break;
                        }
                    }
                }
            }
            else if (monsterAI == "cycler") {
                activeMonster.lastAttackIndex = (activeMonster.lastAttackIndex + 1) % attackNum;
                selectedMonsterAttackIndex =  activeMonster.lastAttackIndex;
            }

            if (activeMonster.attacks[selectedMonsterAttackIndex].type == AttackConstants.DEBUFF) {
                if (pm.GetStatusEffect(activeMonster.attacks[selectedMonsterAttackIndex].seName) != -1) {
                    selectedMonsterAttackIndex = (selectedMonsterAttackIndex - 1) % activeMonster.attackNum;
                }
            }
            if ((attacks[selectedMonsterAttackIndex].type == AttackConstants.BUFFSELF || attacks[selectedMonsterAttackIndex].type == AttackConstants.HEALHPSELF
            || attacks[selectedMonsterAttackIndex].type == AttackConstants.BUFF) && activeMonster.GetStatusEffect(attacks[selectedMonsterAttackIndex].seName) != -1) {
                selectedMonsterAttackIndex = (selectedMonsterAttackIndex - 1) % activeMonster.attackNum;
            }

            return attacks[selectedMonsterAttackIndex];  
        }

        /// <summary>
        /// Executes a monster's attack, by first getting the attack it wants to use, and then selecting
        /// its target based on its AI
        /// </summary>
        /// <returns> IEnumerator to play animations after each action </returns>
        private IEnumerator ExecuteMonsterAttack() {
            PartyMember targetChoice = null;

            PartyMember taunter = (PartyMember)CheckTauntIndex(activeMonster);
            mNoAction = activeMonster.GetStatusEffect(StatusEffectConstants.STUN) != -1;

            if (mNoAction == true) {
                eventDescription.SetNoMoveTextM(activeMonster.monsterSpriteName);
                yield return StartCoroutine(activeMonster.md.PlayStartTurnAnimation());
                yield return new WaitForSeconds(0.3f);   
            }
            else {
                if (taunter != null) {
                    targetChoice = taunter;
                }
                else if (activeMonster.monsterAI == "random" || activeMonster.monsterAI == "lastAt60"
                || activeMonster.monsterAI == "debuffer" || activeMonster.monsterAI == "cycler") {
                    targetChoice = partyMembersAlive[Random.Range(0, partyMembersAlive.Count)];
                }
                else if (activeMonster.monsterAI == "weakHunter" || activeMonster.monsterAI == "weakHunterLastAt60") {
                    int weakest = 0;
                    int weakestHitChance = Random.Range(0, 100);

                    if (weakestHitChance < 25) {    // 25% chance of attacking weakest partyMember
                        for (int i = 1; i < partyMembersAlive.Count; i++) {          
                            if (partyMembersAlive[i].CHP < partyMembersAlive[weakest].CHP && !partyMembersAlive[i].CheckDeath()) {
                                weakest = i;
                            }
                        }
                        targetChoice = partyMembersAlive[weakest];
                    }
                    else {
                        targetChoice = partyMembersAlive[Random.Range(0, partyMembersAlive.Count)];
                    }
                }
                else if (activeMonster.monsterAI == "bleedHunter") {
                    for (int i = 0; i < partyMembersAlive.Count; i++) {
                        if (partyMembersAlive[i].GetStatusEffect(StatusEffectConstants.BLEED) != -1) {
                            targetChoice = partyMembersAlive[i];
                            break;
                        }
                    }
                    if (targetChoice == null) {
                        targetChoice = partyMembersAlive[Random.Range(0, partyMembersAlive.Count)];
                    }
                }
                selectedAttackMonster = SelectMonsterAttack(targetChoice);
                eventDescription.SetKey(selectedAttackMonster.nameKey);
                yield return StartCoroutine(activeMonster.md.PlayStartTurnAnimation());
                
                yield return (StartCoroutine(activeMonster.md.PlayAttackAnimation(selectedMonsterAttackIndex)));

                if (selectedAttackMonster.scope == "single") {
                    if (selectedAttackMonster.type == AttackConstants.PHYSICAL || selectedAttackMonster.type == AttackConstants.MAGICAL) {
                        yield return (StartCoroutine(targetChoice.GetAttacked(selectedAttackMonster, activeMonster)));
                    } 
                    else if (selectedAttackMonster.type == AttackConstants.DEBUFF) {
                        yield return (StartCoroutine(targetChoice.GetStatusEffected(selectedAttackMonster, activeMonster)));
                    }  
                    else if (selectedAttackMonster.type == AttackConstants.HEALHPSELF || selectedAttackMonster.type == AttackConstants.BUFFSELF) {
                        yield return (StartCoroutine(activeMonster.GetHelped(selectedAttackMonster, activeMonster)));
                    }
                }
                else if (selectedAttackMonster.scope == "allAllies") {
                    if (selectedAttackMonster.type == AttackConstants.BUFF) {
                        for (int i = 1; i < monsters.Count; i++) {
                            StartCoroutine(monsters[i].GetHelped(selectedAttackMonster, activeMonster)); 
                        }
                        yield return (StartCoroutine(monsters[0].GetHelped(selectedAttackMonster, activeMonster)));
                    }
                }
                else if (selectedAttackMonster.scope == "selfAndRandomAlly") {
                    if (monsters.Count > 1) {   // if there is another monster, target them as well
                        int target = Random.Range(0, monsters.Count);
                        if (monsters[target].ID == activeMonster.ID) {  // if target self, pick right, or left if no monster right
                            if (target < monsters.Count - 1) { 
                                target++;
                            }
                            else {
                                target--;
                            }
                        }
                        StartCoroutine(monsters[target].GetHelped(selectedAttackMonster, activeMonster));       
                    }
                    yield return (StartCoroutine(activeMonster.GetHelped(selectedAttackMonster, activeMonster)));
                }
                else if (selectedAttackMonster.scope == "allEnemies") {
                    for (int i = 0; i < partyMembersAlive.Count; i++) {
                        if (i == 1) {
                            eventDescription.SetAppendMode(true);
                        }
                        StartCoroutine(partyMembersAlive[i].GetAttacked(selectedAttackMonster, activeMonster)); 
                    }
                    
                    foreach (PartyMember pm in partyMembersAlive) {
                        while (pm.IsDoneAnimating() == false) {
                            yield return null;
                        }
                    }
                    eventDescription.SetAppendMode(false);
                } 
            }
        }

        /// <summary>
        /// Resolve end of turn effects (partyMembers's being dead, end of turn debuffs, etc.)
        /// </summary>
        /// <returns> IEnumerator for animations </returns>
        public IEnumerator CleanUpMonsterTurn() {
            List<Monster> monstersToRemove = new List<Monster>();
            List<PartyMember> partyMembersToRemove = new List<PartyMember>();
            
            yield return StartCoroutine(activeMonster.TriggerStatuses());  
            foreach (Monster m in monsters) {
                if (m.CheckDeath()) {
                    cq.RemoveCharacter(m.ID);
                    monstersToRemove.Add(m);
                    enemiesKilled.Add(m);

                    yield return StartCoroutine(m.md.PlayDeathAnimation());
                }
            }

            DestroyMonsters(monstersToRemove);

            foreach (PartyMember pm in partyMembersAlive) {
                if (pm.CheckDeath()) {
                    cq.RemoveCharacter(pm.ID);
                    partyMembersToRemove.Add(pm);
                }
            }
            foreach (PartyMember pm in partyMembersToRemove) {
                partyMembersAlive.Remove(pm);
            }

            selectedAttackMonster = null;
            mNoAction = false;
        }

        /// <summary>
        /// Checks if all partyMembers or all monsters are defeated
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
            inCombat = false;

            if (isFleeSuccessful) {
                endString = "FLEE";
            }
            else if (cq.CheckPartyDefeated()) {
                endString = "DEFEAT";
            }
            else if (cq.CheckMonstersDefeated()) {
                endString = "VICTORY";
            }

            List<PartyMember> summonsToRemove = new List<PartyMember>();
            foreach (PartyMember pm in partyMembersAll) {
                if (pm.summoner != null) {
                    summonsToRemove.Add(pm);
                    pm.summoner.summon = null;
                }
            }
            foreach (PartyMember pm in summonsToRemove) {
                PartyManager.instance.RemovePartyMember(pm);
            }

            StartCoroutine(EventManager.instance.DisplayPostCombat(endString));
        }

        #endregion

        #region [ Section 3 ] Monster Selection
       
        /// <summary>
        /// Selects a monster to be attacked
        /// </summary>
        /// <param name="monsterToSelect"> Monster to select </param>
        /// <remark> Will probably make a UI info popup when clicking on monsters with no attack in the future </remark>
        public void SelectMonster(Monster monsterToSelect) {
            DeselectMonsters();
            
            selectedMonster = monsterToSelect;
            Monster taunter = (Monster)CheckTauntIndex(activePartyMember);

            // Player can freely click on monsters without having an attack selected
            if (selectedAttackPM != null && (taunter == null || (taunter != null && monsterToSelect.ID == taunter.ID))) {
                if (selectedAttackPM.type != AttackConstants.HEALHP && selectedAttackPM.type != AttackConstants.BUFF) {
                    
                    if (selectedAttackPM.scope != "single") {
                        int monsterIndex = 0;
                        for (int i = 0; i < monsters.Count; i++) {
                            if (monsters[i].ID == monsterToSelect.ID) {
                                monsterIndex = i;
                                break;
                            }
                        }

                        if (selectedAttackPM.scope == "adjacent") {
                            if (monsterIndex - 1 >= 0) {
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
        /// Displays a dimmed "target UI" over monsters beside the main target for
        /// attacks that hit more than one monster
        /// </summary>
        /// <param name="monsterToSelect"> Monster as the main target </param>
        public void ShowAttackTargets(Monster monsterToSelect) {
            if (selectedAttackPM != null) {
                if (selectedAttackPM.scope == "adjacent") {
                    int monsterIndex = 0;
                    for (int i = 0; i < monsters.Count; i++) {
                        if (monsters[i].ID == monsterToSelect.ID) {
                            monsterIndex = i;
                            break;
                        }
                    }    
                    if (monsterIndex - 1 >= 0) {
                        monsters[monsterIndex - 1].md.SelectMonsterButtonAdjacent();
                    }
                    if (monsterIndex + 1 < monsters.Count) {
                        monsters[monsterIndex + 1].md.SelectMonsterButtonAdjacent();
                    }    
                }
                else if (selectedAttackPM.scope == "allEnemies") {
                    for (int i = 0; i < monsters.Count; i++) {
                        if (monsters[i].ID != monsterToSelect.ID) {
                            monsters[i].md.SelectMonsterButtonAdjacent();
                        }
                    }  
                }  
            }
        }

        /// <summary>
        /// Reverts the target UI to normal over all monsters besides the main target
        /// for attacks that hit more than one monster
        /// </summary>
        /// <param name="monsterToSelect"> Monster as the main target</param>
        public void HideAttackTargets(Monster monsterToSelect) {
            if (selectedAttackPM != null) {
                if (selectedAttackPM.scope == "adjacent") {
                    int monsterIndex = 0;
                    for (int i = 0; i < monsters.Count; i++) {
                        if (monsters[i].ID == monsterToSelect.ID) {
                            monsterIndex = i;
                            break;
                        }
                    }    
                    if (monsterIndex - 1 >= 0) {
                        monsters[monsterIndex - 1].md.DeselectMonsterButton();
                    }
                    if (monsterIndex + 1 < monsters.Count) {
                        monsters[monsterIndex + 1].md.DeselectMonsterButton();
                    }    
                }
                if (selectedAttackPM.scope == "allEnemies") {
                    for (int i = 0; i < monsters.Count; i++) {
                        if (monsters[i].ID != monsterToSelect.ID) {
                            monsters[i].md.DeselectMonsterButton();
                        }
                    }  
                }  
            }
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
                m.md.SetInteractable(true, true);
            }
        }

        /// <summary>
        /// Disables all interaction of monsters, except for their status effects
        /// </summary>
        public void DisableAllMonsterSelection() {
            foreach (Monster m in monsters) {
                m.md.SetInteractable(false, true);
            }
        }  

        /// <summary>
        /// Disables all interaction of monsters, including their status effects (only used when trying to flee)
        /// </summary>
        public void DisableAllMonsterSelectionFlee() {
            foreach (Monster m in monsters) {
                m.md.SetInteractable(false, false);
            }
        }  

        /// <summary>
        /// Arranges monsters on screen to look nice depending on the number.
        /// Only a max of 5 monsters for now, although 3 large monsters, 4 normal, and 5 small fit at a time
        /// </summary>
        /// <remark> Will add custom arrangements for wackier monster layouts in the future </remark>
        private void ArrangeMonsters(string arrangementType = "normal") {
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

        #endregion

        #region [ Section 4 ] PartyMember UI Management

        /// <summary>
        /// Disables interactions with all buttons player can select
        /// </summary>
        public void DisableAllButtons() {
            actionsPanel.SetAllActionsUninteractable();
            gearPanel.SetInteractable(false);
            candlesPanel.SetInteractable(false);
            specialPanel.SetInteractable(false);
            skillsPanel.SetInteractable(false);
            partyPanel.DisableButtons();
            itemsTabManager.SetAllButtonsUninteractable();
            utilityTabManager.SetAllButtonsUninteractable();
            DeselectMonstersVisually();
            DisableAllMonsterSelection();
        }

        public void DisableAllButtonsFlee() {
            actionsPanel.SetAllActionsUninteractable();
            gearPanel.SetInteractable(false);
            candlesPanel.SetInteractable(false);
            specialPanel.SetInteractable(false);
            skillsPanel.SetInteractable(false);
            partyPanel.DisableButtons();
            itemsTabManager.SetAllButtonsUninteractable();
            utilityTabManager.SetAllButtonsUninteractable();
            DeselectMonstersVisually();
            DisableAllMonsterSelectionFlee();
        }

        /// <summary>
        /// Enables all buttons to be selectable, except in the actionsPanel,
        /// as DisplayPartyMember() takes care of that, and does other checks
        /// </summary>
        public void EnableAllButtonsInSidePanels() {
            EnableAllMonsterSelection();
            itemsTabManager.SetAllButtonsInteractable();
            utilityTabManager.SetAllButtonsInteractable();
            partyPanel.EnableButtons();
            gearPanel.SetInteractable(true);
            candlesPanel.SetInteractable(true);
            specialPanel.SetInteractable(true);
            skillsPanel.SetInteractable(true);
        }
        
        /// <summary>
        /// Changes the UI to reflect the first partyMember in the queue's information
        /// </summary>s
        public void DisplayFirstPartyMember() {
            activePartyMember = cq.GetFirstPM();    // activePartyMember will be redundantly set a second time
            actionsPanel.DisplayFirstPartyMember(activePartyMember);
            PartyManager.instance.SetActivePartyMember(activePartyMember);
        }

        /// <summary>
        /// Changes the UI to reflect the active partyMember's information
        /// </summary>s
        public void DisplayActivePartyMember() {
            actionsPanel.DisplayPartyMember(activePartyMember);
            activePartyMember.pmvc.DisplayActivePartyMemberCombat();
        }

        #endregion

        /// <summary>
        /// Returns a key string for when the player fails to flee
        /// </summary>
        /// <returns> String </returns>
        public string GetFleeFailedKey() {
            return "flee_failed_" + Random.Range(0, 4);
        }

        /// <summary>
        /// Return the character another character is taunted by if they exist and are alive
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public Character CheckTauntIndex(Character c) {
            int tauntIndex = c.GetStatusEffect(StatusEffectConstants.TAUNT);
            int marionetteIndex = c.GetStatusEffect(StatusEffectConstants.MARIONETTE);

            if (tauntIndex != -1) {
                Character taunter = c.statusEffects[tauntIndex].afflicter;

                if (taunter != null && taunter.CheckDeath() == false) {
                    return taunter;
                }
            }
            if (marionetteIndex != -1) {
                Character taunter = c.statusEffects[marionetteIndex].afflicter;

                if (taunter != null && taunter.CheckDeath() == false) {
                    return taunter;
                }
            }

            return null;
        }
    }
}
