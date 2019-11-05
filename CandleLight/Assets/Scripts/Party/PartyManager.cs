/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 23, 2019
* 
* The PartyManager class is used to manage the partyMembers and their information.
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

        private List<PartyMember> partyMembersAll = new List <PartyMember>();
        private List<PartyMember> partyMembersAlive = new List<PartyMember>();  /// <value> List of partyMembers in party </value>
        private List<PartyMember> partyMembersDead = new List<PartyMember>();   /// <value> List of partyMembers in party </value>
        private PartyMember activePartyMember = null;
        private int maxPartyMembers = 4;                                        /// <value> Max number of partyMembers </value>
        
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
        /// Adds a partyMember to the list of partyMembers
        /// </summary>   
        /// <param name="className"> Class of the partyMember to be added </param>
        public void AddPartyMember(string className) {
            if (partyMembersAlive.Count + partyMembersDead.Count < maxPartyMembers) {
                GameObject newMember = Instantiate(partyMember, new Vector3(0f,0f,0f), Quaternion.identity);
                GameManager.instance.DB.GetPartyMemberByClass(className, newMember.GetComponent<PartyMember>());
                newMember.transform.SetParent(gameObject.transform, false);
                partyMembersAlive.Add(newMember.GetComponent<PartyMember>());
                partyMembersAll.Add(newMember.GetComponent<PartyMember>());
            }

            activePartyMember = GetFirstPartyMemberAlive();
        }

        /// <summary>
        /// Removes all partyMembers from PartyMembers list
        /// </summary>
        public void ClearPartyMembers() {
            partyMembersAll.Clear();
            partyMembersAlive.Clear();
            partyMembersDead.Clear();
        }
        
        /// <summary>
        /// Returns a list containing all partyMembers
        /// </summary> 
        /// <returns>
        /// A list of partyMembers. 
        /// Each partyMember is assigned a unique ID in preparation for queueing in combat. 
        /// ID is incremented for each partyMember.
        /// </returns>
        /// <param name="countID"> ID that is incremented and assigned to each partyMember for queuing </param>
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
        /// Returns the number of partyMembers 
        /// </summary>
        /// <returns> int amount of partyMember </returns>
        public int GetNumPartyMembers() {
            return partyMembersAlive.Count + partyMembersDead.Count;
        }

        /// <summary>
        /// Returns the number of partyMembers alive
        /// </summary>
        /// <returns></returns>
        public int GetNumPartyMembersAlive() {
            return partyMembersAlive.Count;
        }

        /// <summary>
        /// Returns the first partyMember in the list of partyMembers alive
        /// </summary>
        /// <returns> PartyMember that is alive</returns>
        public PartyMember GetFirstPartyMemberAlive() {
            return partyMembersAlive[0];
        }

        /// <summary>
        /// Sets the active partyMember, and displays it
        /// </summary>
        /// <param name="pm"></param>
        public void SetActivePartyMember(PartyMember pm) {
            activePartyMember = pm;
            pm.pmvc.DisplayActivePartyMember();
        }

        /// <summary>
        /// Returns the active partyMember
        /// </summary>
        /// <returns> PartyMember </returns>
        public PartyMember GetActivePartyMember() {
            return activePartyMember;
        }

        /// <summary>
        /// Adds a partyMember to the alive list and removes them from the dead list
        /// </summary>
        /// <param name="pm"> partyMember object </param>
        public void RegisterPartyMemberAlive(PartyMember pm) {
            partyMembersAlive.Add(pm);
            partyMembersDead.Remove(pm);
        }

        /// <summary>
        /// Adds a partyMember to the dead list and removes them from the alive list
        /// </summary>
        /// <param name="pm"> partyMember object </param>
        public void RegisterPartyMemberDead(PartyMember pm) {
            partyMembersAlive.Remove(pm);
            partyMembersDead.Add(pm);
        }

        /// <summary>
        /// Adds EXP to each partyMember
        /// </summary>
        /// <param name="amount"> Positive int amount to be added </param>
        public void AddEXP(int amount) {
            foreach (PartyMember pm in partyMembersAlive) {
                StartCoroutine(pm.AddEXP(amount));
            }
        }

        /// <summary>
        /// Adds HP to a random partyMember
        /// </summary>
        /// <param name="amount"> Positive int amount to be added </param>
        public void AddHPSingle(int amount) {
            partyMembersAlive[Random.Range(0, partyMembersAlive.Count)].AddHP(amount);
        }

        /// <summary>
        /// Adds HP to a specific partyMember
        /// </summary>
        /// <param name="pm"> partyMember object </param>
        /// <param name="amount"> Positive int amount to be added </param>
        public void AddHPSingle(PartyMember pm, int amount) {
            pm.AddHP(amount);
        }

        /// <summary>
        /// Adds HP to all partyMembers
        /// </summary>
        /// <param name="amount"> Positive int amount to be added </param>
        public void AddHPAll(int amount) {
            foreach (PartyMember pm in partyMembersAlive) {
                pm.AddHP(amount);
            }
        }

        /// <summary>
        /// Lose HP from a random partyMember
        /// </summary>
        /// <param name="amount"> Positive int amount to be lost </param>
        public void LoseHPSingle(int amount) {
            StartCoroutine(partyMembersAlive[Random.Range(0, partyMembersAlive.Count)].LoseHP(amount));
        }

        /// <summary>
        /// Lose HP from a specific partyMember
        /// </summary>
        /// <param name="pm"> partyMember object </param>
        /// <param name="amount"> Positive int amount to be lost </param>
        public void LoseHPSingle(PartyMember pm, int amount) {
            StartCoroutine(pm.LoseHP(amount));
        }

        /// <summary>
        /// Lose HP from all partyMembers
        /// </summary>
        /// <param name="amount"> Positive int to be lost </param>
        public void LoseHPAll(int amount) {
            foreach (PartyMember pm in partyMembersAll) {
                StartCoroutine(pm.LoseHP(amount));
            }
        }

        /// <summary>
        /// Add MP to a random partyMember
        /// </summary>
        /// <param name="amount"> Positive int to be added </param>
        public void AddMPSingle(int amount) {
            partyMembersAlive[Random.Range(0, partyMembersAlive.Count)].AddMP(amount);
        }

        /// <summary>
        /// Add MP to a specific partyMember
        /// </summary>
        /// <param name="pm"> partyMember object </param>
        /// <param name="amount"> Positive int to be added </param>
        public void AddMPSingle(PartyMember pm, int amount) {
            pm.AddMP(amount);
        }

        /// <summary>
        /// Add MP to all partyMembers
        /// </summary>
        /// <param name="amount"> Positive int to be added </param>
        public void AddMPAll(int amount) {
            foreach (PartyMember pm in partyMembersAlive) {
                pm.AddMP(amount);
            }
        }

        /// <summary>
        /// Lose MP from a random partyMember
        /// </summary>
        /// <param name="amount"> Positive int to be lost </param>
        public void LoseMPSingle(int amount) {
            StartCoroutine(partyMembersAlive[Random.Range(0, partyMembersAlive.Count)].LoseMP(amount));
        }

        /// <summary>
        /// Lose MP from a specific partyMember
        /// </summary>
        /// <param name="pm"> partyMember object </param>
        /// <param name="amount"> Positive int to be lost </param>
        public void LoseMPSingle(PartyMember pm, int amount) {
            StartCoroutine(pm.LoseMP(amount));
        }

        /// <summary>
        /// Lose MP from all partyMembers
        /// </summary>
        /// <param name="amount"> Positive int to be lost </param>
        public void LoseMPAll(int amount) {
            foreach (PartyMember pm in partyMembersAlive) {
                StartCoroutine(pm.LoseMP(amount));
            }
        }

        /// <summary>
        /// Regenerates MP and HP for all partyMembers
        /// </summary>
        public void RegenParty() {
            foreach (PartyMember pm in partyMembersAlive) {
                pm.Regen();
            }
        }

        /// <summary>
        /// Increases the amount of WAX the party has
        /// </summary>
        /// <param name="amount"> Positive int to increase by </param>
        public void AddWAX(int amount) {
            WAX += amount;
            EventManager.instance.infoPanel.UpdateAmounts(); //with singletons you can reference across scenes!
        }

        /// <summary>
        /// Decreases the amount of WAX the party has
        /// </summary>
        /// <param name="amount"> Positive int to decrease by </param>
        public void LoseWAX(int amount) {
            if (amount > WAX) {
                WAX = 0;
            }
            else {
                WAX -= amount;
            }
        }

        /// <summary>
        /// Equips a gear item to the activePartyMember
        /// </summary>
        /// <param name="id"> ItemDisplay </param>
        /// <param name="subType"> Subtype of gear (weapon, secondary, armour) </param>
        public void EquipGear(ItemDisplay id, string subType) {
            activePartyMember.EquipGear(id.GetGear(), id.subType);  // need to figure out if itemDisplay's item should be private
        }

        /// <summary>
        /// Unequips a gear item from the activePartyMember
        /// </summary>
        /// <param name="subType"> Subtype of gear (weapon, secondary, armour) </param>
        public void UnequipGear(string subType) {
            activePartyMember.UnequipGear(subType);  // need to figure out if itemDisplay's item should be private
        }

        public IEnumerator TriggerStatuses() { 
            PartyMember yielding = null;
            for (int i = 0; i < partyMembersAlive.Count; i++) {
                if (yielding == null && partyMembersAlive[i].statusEffects.Count > 0) {
                    yielding = partyMembersAlive[i];
                }
            }
            for (int i = 0; i < partyMembersAlive.Count; i++) {
                if (partyMembersAlive[i] != yielding) {
                    StartCoroutine(partyMembersAlive[i].TriggerStatuses(false));
                }
            }
            if (yielding != null) {
                yield return (StartCoroutine(yielding.TriggerStatuses(false)));
            }
        }
    }
}
