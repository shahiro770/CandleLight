/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 23, 2019
* 
* The PartyManager class is used to manage the party members and their information.
* It is held within the Game scene where it can always be accessed globally.
*
*/

using Characters;
using Events;
using General;
using PlayerUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Party {

    public class PartyManager : MonoBehaviour {
        
        public static PartyManager instance;    /// <value> global instance </value>
        
        /* external component references */
        public GameObject partyMember;          /// <value> partyMember game object to instantiate </value>

        public int WAX { get; private set; }    /// <value> Currency party has stored up </value>
        
        private List<PartyMember> partyMembers = new List<PartyMember>();   /// <value> List of partyMembers in party </value>
        private int maxPartyMembers = 4;                                    /// <value> Max number of partyMembers </value>
        
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
            if (partyMembers.Count < maxPartyMembers) {
                GameObject newMember = Instantiate(partyMember, new Vector3(0f,0f,0f), Quaternion.identity);
                GameManager.instance.DB.GetPartyMemberByClass(className, newMember.GetComponent<PartyMember>());
                newMember.transform.SetParent(gameObject.transform, false);
                partyMembers.Add(newMember.GetComponent<PartyMember>());
            }
        }

        /// <summary>
        /// Removes all partyMembers from PartyMembers list
        /// </summary>
        public void ClearPartyMembers() {
            partyMembers.Clear();
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

        /// <summary>
        /// Returns the list of partyMembers
        /// </summary>
        /// <returns> List of partyMembers </returns>
        public List<PartyMember> GetPartyMembers() {
            return partyMembers;
        }

        /// <summary>
        /// Returns the number of partyMembers in the partyMembers list
        /// </summary>
        /// <returns> int amount of partyMember </returns>
        public int GetNumPartyMembers() {
            return partyMembers.Count;
        }

        public void GainEXP(int amount) {
            foreach (PartyMember pm in partyMembers) {
                StartCoroutine(pm.GainEXP(amount));
            }
        }

        public void AddHP(int amount) {
            foreach (PartyMember pm in partyMembers) {
                pm.AddHP(amount);
            }
        }

        /// <summary>
        /// Increases the amount of WAX the party has
        /// </summary>
        /// <param name="amount"> Positive int to increase by </param>
        public void AddWax(int amount) {
            WAX += amount;
            EventManager.instance.infoPanel.UpdateAmounts(); //with singletons you can reference across scenes!
        }

        /// <summary>
        /// Decreases the amount of WAX the party has
        /// </summary>
        /// <param name="amount"> Positive int to decrease by </param>
        public void LoseWax(int amount) {
            if (amount > WAX) {
                WAX = 0;
            }
            else {
                WAX -= amount;
            }
        }
    }
}
