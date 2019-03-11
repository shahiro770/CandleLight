/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 23, 2019
* 
* The PartyManager class is used to manage the party members and their information.
* It is held within the Game scene where it can always be accessed globally.
*
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataBank;

public class PartyManager : MonoBehaviour {

    public static PartyManager instance;    /// <value> global instance </value>
    public GameObject partyMember;          /// <value> partyMember game object to instantiate </value>
    
    private List<PartyMember> partyMembers = new List<PartyMember>();

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
    }

    /// <summary>
    /// Adds a party member to the list of party members
    /// </summary> 
    /// <param name="className"> Class of the party member to be added </param>
    public void AddPartyMember(string className) {
        GameObject newMember = Instantiate(partyMember, new Vector3(0f,0f,0f), Quaternion.identity);
        GameManager.instance.DB.GetPartyMemberByClass(className, newMember.GetComponent<PartyMember>());
        newMember.transform.SetParent(gameObject.transform, false);
        partyMembers.Add(newMember.GetComponent<PartyMember>());
    }
    
    /// <summary>
    /// Returns a list containing all party members
    /// </summary> 
    /// <returns>
    /// A list of party members. 
    /// Each party member is assigned a unique ID in preparation for queueing in combat. 
    /// ID is incremented for each party member.
    /// </returns>
    /// <param name="countID"> ID that is incremented and assigned to each party member for queuing </param>
    public List<PartyMember> GetPartyMembers(ref int countID) {
        foreach (PartyMember pm in partyMembers) {
            pm.ID = countID++;
        }

        return partyMembers;
    }
}
