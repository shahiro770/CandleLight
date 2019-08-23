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
        
        private List<PartyMember> partyMembersAlive = new List<PartyMember>();   /// <value> List of partyMembers in party </value>
        private List<PartyMember> partyMembersDead = new List<PartyMember>();   /// <value> List of partyMembers in party </value>
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
            if (partyMembersAlive.Count + partyMembersDead.Count < maxPartyMembers) {
                GameObject newMember = Instantiate(partyMember, new Vector3(0f,0f,0f), Quaternion.identity);
                GameManager.instance.DB.GetPartyMemberByClass(className, newMember.GetComponent<PartyMember>());
                newMember.transform.SetParent(gameObject.transform, false);
                partyMembersAlive.Add(newMember.GetComponent<PartyMember>());
            }
        }

        /// <summary>
        /// Removes all partyMembers from PartyMembers list
        /// </summary>
        public void ClearPartyMembers() {
            partyMembersAlive.Clear();
            partyMembersDead.Clear();
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
            List<PartyMember> partyMembers = new List<PartyMember>();
            foreach (PartyMember pm in partyMembersAlive) {
                pm.ID = countID++;
                partyMembers.Add(pm);
            }
            foreach (PartyMember pm in partyMembersDead) {
                pm.ID = countID++;
                partyMembers.Add(pm);
            }

            return partyMembers;
        }

        /// <summary>
        /// Returns the list of partyMembers
        /// </summary>
        /// <returns> List of partyMembers </returns>
        public List<PartyMember> GetPartyMembers() {
            List<PartyMember> partyMembers = new List<PartyMember>();
            foreach (PartyMember pm in partyMembersAlive) {
                partyMembers.Add(pm);
            }
            foreach (PartyMember pm in partyMembersDead) {
                partyMembers.Add(pm);
            }

            return partyMembers;
        }

        /// <summary>
        /// Returns the number of partyMembers in the partyMembers list
        /// </summary>
        /// <returns> int amount of partyMember </returns>
        public int GetNumPartyMembers() {
            return partyMembersAlive.Count + partyMembersDead.Count;
        }

        public void RegisterPartyMemberAlive(PartyMember pm) {
            partyMembersAlive.Add(pm);
            partyMembersDead.Remove(pm);
        }

        public void RegisterPartyMemberDead(PartyMember pm) {
            partyMembersAlive.Remove(pm);
            partyMembersDead.Add(pm);
        }

        public void GainEXP(int amount) {
            foreach (PartyMember pm in partyMembersAlive) {
                StartCoroutine(pm.GainEXP(amount));
            }
        }

        public void AddHPSingle(int amount) {
            partyMembersAlive[Random.Range(0, partyMembersAlive.Count)].AddHP(amount);
        }

        public void AddHPSingle(PartyMember pm, int amount) {
            pm.AddHP(amount);
        }

        public void AddHPMultiple(int amount) {
            foreach (PartyMember pm in partyMembersAlive) {
                pm.AddHP(amount);
            }
        }

        public void AddMPSingle(int amount) {
            partyMembersAlive[Random.Range(0, partyMembersAlive.Count)].AddMP(amount);
        }

        public void AddMPSingle(PartyMember pm, int amount) {
            pm.AddMP(amount);
        }

        public void AddMPMultiple(int amount) {
            foreach (PartyMember pm in partyMembersAlive) {
                pm.AddMP(amount);
            }
        }

        public void RegenParty() {
            foreach (PartyMember pm in partyMembersAlive) {
                pm.Regen();
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
