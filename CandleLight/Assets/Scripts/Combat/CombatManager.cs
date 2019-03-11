using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using DataBank;

//STILL NEED A FUNCTION TO COLOUR THE UI THE AREA DEFAULT

namespace Combat {

    public delegate void SelectMonsterDelegate(Monster m);
    public class CombatManager : MonoBehaviour {

        private readonly bool PMTURN = true;
        private readonly bool MTURN = false;

        public static CombatManager instance;
        public Canvas enemyCanvas;
        public StatusManager statusManager;
        public ActionsManager am;
        public GameObject monster;
        public List<Monster> selectedMonsters = new List<Monster>();

        private List<Monster> monsters =  new List<Monster>();
        private List<PartyMember> partyMembers = new List<PartyMember>();
        private CharacterQueue cq = new CharacterQueue();
        private Action selectedAction = null;
        private PartyMember activePartyMember;
        private Monster activeMonster;
        private EventSystem es;
        private bool turn;     
        private int countID = 0;

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

        // Start is called before the first frame update
        void Start() {
            am.cm = this;

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

        public void SelectMonster(Monster monsterToSelect) {
            foreach (Monster m in selectedMonsters) {
                m.DeselectMonster();
            }
            selectedMonsters.Clear();

            monsterToSelect.SelectMonster();
            selectedMonsters.Add(monsterToSelect);

            if (selectedAction != null) {
                StartCoroutine(ExecuteAttack(selectedAction, monsterToSelect));
            }
        }

        public void SelectMonster(List<Monster> monstersToSelect) {
            foreach (Monster m in selectedMonsters) {
                m.DeselectMonster();
            }
            selectedMonsters.Clear();

            foreach (Monster m in monstersToSelect) {
                m.SelectMonster();
                selectedMonsters.Add(m);
            }
        }

        private void StartCombat() {
            SetActivePartyMember(); // active party member is not set
            GetNextTurn();
        }

        private void EndCombat() {
            GameManager.instance.LoadNextScene("MainMenu");
        }

        public void PreparePMAttack(Action a) {
            selectedAction = a;
            monsters[0].SetNavigation(am.GetActionButton(4));
            am.SetButtonNavigation(4, "up", monsters[0].b);

            es.SetSelectedGameObject(monsters[0].b.gameObject);
        }

        public IEnumerator ExecuteAttack(Action a, Monster m) {
            am.DisableAllActionInteractions();
            DisableAllMonsterSelection();
            yield return StartCoroutine(m.LoseHP(a.a.damage));
            if (m.CheckDeath()) {
                cq.RemoveCharacter(m.ID);
                yield return StartCoroutine(m.Die());
                if (CheckBattleOver()) {
                    EndCombat();
                } 
            }
            m.DeselectMonster();
            GetNextTurn();
        }

        public void UndoPMAction() {
            selectedAction = null;
            monsters[0].SetNavigation(am.GetActionButton(0));
            am.ResetFifthButtonNavigation();
        }

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
                EnableAllMonsterSelection();
                am.EnableAllActions();
                
            } else {
                am.DisableAllActions();
                StartCoroutine(MonsterTurn());
            }
        }

        public bool CheckBattleOver() {
            if (cq.CheckMonstersDefeated() || cq.CheckPartyDefeated()) {
                return true;
            }

            return false;
        }

        private IEnumerator MonsterTurn() {
            yield return StartCoroutine(activeMonster.Attack(partyMembers));
            GetNextTurn();
        }

        public void SetActivePartyMember() {
            activePartyMember = cq.GetNextPM();
            //activePartyMember.LogAttacks();

            am.SetAttackActions(activePartyMember.attacks);
            statusManager.Init(activePartyMember);
        }

        public void DisableAllMonsterSelection() {
            foreach (Monster m in monsters) {
                m.DisableInteraction();
            }
        }

        public void EnableAllMonsterSelection() {
            foreach (Monster m in monsters) {
                m.EnableInteraction();
            }
        }

        /* private void SetStatus() {
            statusManager.Init(activePartyMember);
        } */

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

        private void SetActionSelectedNavigation() {

        }

        // void GetMonsters(string[] monsterNames) {
        //     foreach (string monsterName in monsterNames) {
        //         this.monsters.Add(DB.GetMonsterByNameID(monsterName));
        //     }
        // }
    }
}
