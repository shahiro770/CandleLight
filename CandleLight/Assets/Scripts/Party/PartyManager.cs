using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataBank;

public class PartyManager : MonoBehaviour {

    public static PartyManager instance;
    public GameObject partyMember;
    
    private GameDB DB;
    private List<PartyMember> partyMembers = new List<PartyMember>();

    void Awake() {
        if (instance == null) {
            instance = this;
        }
        else if (instance != this) {
            DestroyImmediate (gameObject);
            instance = this;
        }
    }

    void Start() {
        this.DB = GameManager.instance.GetDB();
    }

    public void AddPartyMember(string className) {
        GameObject newMember = Instantiate(partyMember, new Vector3(0f,0f,0f), Quaternion.identity);
        DB.GetPartyMemberByClass(className, newMember.GetComponent<PartyMember>());
        newMember.transform.SetParent(gameObject.transform, false);
        partyMembers.Add(newMember.GetComponent<PartyMember>());
    }
    
    public List<PartyMember> GetPartyMembers(ref int countID) {
        foreach (PartyMember pm in partyMembers) {
            pm.ID = countID++;
        }

        return partyMembers;
    }
}
