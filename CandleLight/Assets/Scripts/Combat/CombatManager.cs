using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataBank;

public class CombatManager : MonoBehaviour {

    public static CombatManager instance;
    public Canvas enemyCanvas;
    public StatusManager statusManager;
    public ActionsManager actions;
    public GameObject monster;
    
    private List<Monster> monsters =  new List<Monster>();
    private List<PartyMember> partyMembers = new List<PartyMember>();

    private CharacterQueue cq = new CharacterQueue();
    private GameDB DB;
    private PartyMember activePartyMember;
    private bool playerTurn = false;
    private int countID = 0;

    void Awake() {
        if (instance == null) {
            instance = this;
        }
        else if (instance != this) {
            DestroyImmediate (gameObject);
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start() {
        DB = GameManager.instance.GetDB();

        foreach (string monsterName in GameManager.instance.GetMonsterNames()) {
            AddMonster(monsterName);
        }
        partyMembers = PartyManager.instance.GetPartyMembers(ref countID);

        foreach (Monster monster in monsters) {
            cq.AddCharacter(monster);
        }
        foreach (PartyMember pm in partyMembers) {
            cq.AddCharacter(pm);
        }

        cq.FinalizeQueue();
        SetActivePartyMember(); // active party member is not set
        //StartCombat();
    }

    // Update is called once per frame
   
		/* void Update()
		{
			//Check that playersTurn or enemiesMoving or doingSetup are not currently true.
			if (playersTurn) {
				
				//If any of these are true, return and do not start MoveEnemies.
				return;
            } 

        }*/
    private void SetActivePartyMember() {
        activePartyMember = cq.GetNextPM();
        //activePartyMember.LogAttacks();

        actions.SetActionTexts(activePartyMember.attacks);
        statusManager.Init(activePartyMember);
    }

    /* private void SetStatus() {
        statusManager.Init(activePartyMember);
    } */

    private void AddMonster(string monsterName) {
        GameObject newMonster = Instantiate(monster, new Vector3(0f,0f,0f), Quaternion.identity);
        Monster monsterComponent = newMonster.GetComponent<Monster>();

        DB.GetMonsterByNameID(monsterName, monsterComponent);
        monsterComponent.ID = countID++;
        newMonster.transform.SetParent(enemyCanvas.transform, false);
        monsters.Add(monsterComponent);
    }

    // void GetMonsters(string[] monsterNames) {
    //     foreach (string monsterName in monsterNames) {
    //         this.monsters.Add(DB.GetMonsterByNameID(monsterName));
    //     }
    // }
}
