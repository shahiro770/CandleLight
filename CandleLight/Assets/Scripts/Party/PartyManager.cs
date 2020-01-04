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
using SkillConstants = Constants.SkillConstants;
using ResultConstants = Constants.ResultConstants;
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
        private enum primaryStats { NONE, STR, DEX, INT, LUK };                 /// <value> Enumerated primary stats </value>
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
            foreach (PartyMember pms in partyMembersAll) {
                pms.pmvc.ShowNormal();
            }

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
        /// Adds EXP to each partyMember who is alive
        /// </summary>
        /// <param name="amount"> Positive int amount to be added </param>
        public void AddEXP(int amount) {
            foreach (PartyMember pm in partyMembersAlive) {
                StartCoroutine(pm.AddEXP(amount));
            }
        }

        /// <summary>
        /// Change CHP of a random partyMember
        /// </summary>
        /// <param name="amount"> Positive int amount to be added </param>
        public void ChangeHPSingle(int amount, string type) {
            if (amount >= 0) {
                partyMembersAlive[Random.Range(0, partyMembersAlive.Count)].AddHP(amount);
            }
            else {
                PartyMember pm = partyMembersAlive[Random.Range(0, partyMembersAlive.Count)];
                if (type == ResultConstants.STATALL || type == ResultConstants.STATALLANDLEAVE || type == ResultConstants.COMBATWITHSIDEEFFECTS) {
                    if (pm.className == "Warrior") {
                        if (pm.skills[(int)SkillConstants.warriorSkills.STEADFAST].skillEnabled == true) {
                            pm.LoseHP(amount >> 1);
                        }
                    }
                    else {
                        pm.LoseHP(amount);
                    }
                }
            }
        }

        /// <summary>
        /// Change CHP of a specific partyMember
        /// </summary>
        /// <param name="amount"> Positive int amount to be added </param>
        public void ChangeHPSingle(PartyMember pm, int amount) {
            if (amount >= 0) {
                pm.AddHP(amount);
            }
            else {
                pm.LoseHP(amount);
            }
        }

        /// <summary>
        /// Change CHP of all partyMembers
        /// </summary>
        /// <param name="amount"></param>
        public IEnumerator ChangeHPAll(int amount, string type = "none") {
            if (amount >= 0) {
                foreach (PartyMember pm in partyMembersAlive) {
                    pm.AddHP(amount);
                }
            }
            else {
                for (int i = 0; i < partyMembersAll.Count; i++) {
                    if (type == ResultConstants.STATALL || type == ResultConstants.STATALLANDLEAVE || type == ResultConstants.COMBATWITHSIDEEFFECTS) {
                        if (partyMembersAll[i].CheckDeath() == false) {
                            if (partyMembersAll[i].className == "Warrior") {
                                if (partyMembersAll[i].skills[(int)SkillConstants.warriorSkills.STEADFAST].skillEnabled == true) {
                                    StartCoroutine(partyMembersAll[i].LoseHP(amount >> 1));
                                }
                                else {
                                    StartCoroutine(partyMembersAll[i].LoseHP(amount));
                                }
                            }
                            else {
                                StartCoroutine(partyMembersAll[i].LoseHP(amount));
                            }
                        }
                    }
                }

                if (partyMembersAlive.Count == 0) { // this will have to change if combat gets aoe attacks
                    yield return new WaitForSeconds(1.25f);
                    GameManager.instance.LoadNextScene("MainMenu");
                }
            }
        } 

         /// <summary>
        /// Change CMP of a random partyMember
        /// </summary>
        /// <param name="amount"> Positive int amount to be added </param>
        public void ChangeMPSingle(int amount, string type = "none") {
            if (amount >= 0) {
                partyMembersAlive[Random.Range(0, partyMembersAlive.Count)].AddMP(amount);
            }
            else {
                partyMembersAlive[Random.Range(0, partyMembersAlive.Count)].LoseMP(amount);
            }
        }

        /// <summary>
        /// Change CMP of a specific partyMember
        /// </summary>
        /// <param name="amount"> Positive int amount to be added </param>
        public void ChangeMPSingle(PartyMember pm, int amount) {
            if (amount >= 0) {
                pm.AddMP(amount);
            }
            else {
                pm.LoseMP(amount);
            }
        }

        /// <summary>
        /// Change CMP of all partyMembers
        /// </summary>
        /// <param name="amount"></param>
        public void ChangeMPAll(int amount, string type = "none") {
            if (amount >= 0) {
                foreach (PartyMember pm in partyMembersAlive) {
                    pm.AddMP(amount);
                }
            }
            else {
                foreach (PartyMember pm in partyMembersAlive) {
                    pm.LoseMP(amount);
                }
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

        public void AddSE(string seName, int seDuration) {
            foreach (PartyMember pm in partyMembersAlive) {
                pm.AddStatusEffect(seName, seDuration, null);   // will fail for effects that care about the afflicter
            }
        }

        /// <summary>
        /// Returns the sum of a primary stat amongst all alive partyMembers
        /// </summary>
        /// <param name="stat"> Enumerated int </param>
        /// <returns> Sum of stat </returns>
        public int GetPrimaryStatAll(int stat) {
            int sum = 0;
            foreach (PartyMember pm in partyMembersAlive) {
                if (stat == (int)primaryStats.STR) {
                    sum += pm.STR;          
                }
                else if (stat == (int)primaryStats.DEX) {
                    sum += pm.DEX;   
                }
                else if (stat == (int)primaryStats.INT) {
                    sum += pm.INT;   
                }
                else if (stat == (int)primaryStats.LUK) {
                    sum += pm.LUK;   
                }
            }

            return sum;
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

        public int GetSkillPoints() {
            return activePartyMember.skillPoints;
        }

        public bool EnableSkill(int index) {
            return activePartyMember.EnableSkill(index);
        }

        public bool DisableSkill(int index) {
            return activePartyMember.DisableSkill(index);
        }


        /// <summary>
        /// Triggers status effects on all partyMembers
        /// </summary>
        /// <param name="inCombat"> True if in combat, false otherwise </param>
        /// <returns></returns>
        public IEnumerator TriggerStatuses(bool inCombat) {
            if (inCombat) {
                PartyMember yielding = null;
                for (int i = 0; i < partyMembersAlive.Count; i++) {
                    if (yielding == null && partyMembersAlive[i].statusEffects.Count > 0) {
                        yielding = partyMembersAlive[i];
                    }
                }
                for (int i = 0; i < partyMembersAlive.Count; i++) {
                    if (partyMembersAlive[i] != yielding) {
                        StartCoroutine(partyMembersAlive[i].TriggerStatuses(inCombat));
                    }
                }
                if (yielding != null) {
                    yield return (StartCoroutine(yielding.TriggerStatuses(inCombat)));
                }
            }
            else {
                for (int i = 0; i < partyMembersAlive.Count; i++) {
                    StartCoroutine(partyMembersAlive[i].TriggerStatuses(inCombat));
                }
            }
        }
    }
}
