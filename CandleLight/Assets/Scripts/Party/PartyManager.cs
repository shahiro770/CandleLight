/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 23, 2019
* 
* The PartyManager class is used to manage the partyMembers and their information.
* It is held within the Game scene where it can always be accessed globally.
*
*/

using ClassConstants = Constants.ClassConstants;
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

        public int bonusChampionChance = 0;     /// <value> Chance of encountering champion monsters, summed from all partyMembers </value>
        public int WAX { get; private set; }    /// <value> Currency party has stored up </value>
        public float itemDropMultiplier = 1f;   /// <value> Current multiplier on item drop rates from enemies </value>
        public float WAXDropMultiplier = 1f;    /// <value> Current multiplier on WAX drop amounts from enemies </value>

        private List<PartyMember> partyMembersAll = new List <PartyMember>();
        private List<PartyMember> partyMembersAlive = new List<PartyMember>();  /// <value> List of partyMembers in party </value>
        private List<PartyMember> partyMembersDead = new List<PartyMember>();   /// <value> List of partyMembers in party </value>
        private PartyMember activePartyMember = null;
        private enum primaryStats { NONE, STR, DEX, INT, LUK };                 /// <value> Enumerated primary stats </value>
        private int maxPartyMembers = 4;                                        /// <value> Max number of partyMembers </value>
        private int ID = 0;                                                     /// <value> ID number to assign to each pm</value>

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
            if (GetNumPartyMembers() < maxPartyMembers) {
                GameObject newMember = Instantiate(partyMember, new Vector3(0f,0f,0f), Quaternion.identity);
                GameManager.instance.DB.GetPartyMemberByClass(className, newMember.GetComponent<PartyMember>());
                newMember.transform.SetParent(gameObject.transform, false);
                newMember.GetComponent<PartyMember>().ID = (ID++);
                partyMembersAlive.Add(newMember.GetComponent<PartyMember>());
                partyMembersAll.Add(newMember.GetComponent<PartyMember>());
            }

            activePartyMember = GetFirstPartyMemberAlive();
        }

        /// <summary>
        /// Removes all partyMembers from PartyMembers list
        /// </summary>
        public void ResetGame() {
            WAX = 0;
            ID = 0;
            partyMembersAll.Clear();
            partyMembersAlive.Clear();
            partyMembersDead.Clear();
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
        /// Returns the number of partyMembers dead
        /// </summary>
        /// <returns></returns>
        public int GetNumPartyMembersDead() {
            return partyMembersDead.Count;
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

            pm.RemoveAllStatusEffects();
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
                    if (pm.className == ClassConstants.WARRIOR) {
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
                    if (type == ResultConstants.STATALL || type == ResultConstants.STATALLANDLEAVE || type == ResultConstants.COMBATWITHSIDEEFFECTS || type == ResultConstants.STATALLANDITEMANDLEAVE
                        || type == ResultConstants.QUESTCOMPLETE || type == ResultConstants.QUESTCOMPLETEANDNEWINT) {
                        if (partyMembersAll[i].CheckDeath() == false) {
                            if (partyMembersAll[i].className == ClassConstants.WARRIOR) {
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
                    else if (type == "Combat") {    // TODO: Make AoE attacks against the party show damage taken by everyone, as well as combat constants for type
                        yield return null;
                    }
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
                    StartCoroutine(pm.LoseMP(amount * -1));
                }
            }
        } 

        /// <summary>
        /// Recovers a tiny bit of MP and HP for all partyMembers
        /// </summary>
        public void RegenParty() {
            foreach (PartyMember pm in partyMembersAlive) {
                pm.Regen();
            }
        }

        /// <summary>
        /// Brings all dead partyMembers back to life with a minimum of a third of their health and mana
        /// </summary>
        public void RevivePartyMembers() {
            foreach (PartyMember pm in partyMembersDead) {
                pm.AddHP((int)(pm.HP * 0.34));
                pm.AddMP((int)(pm.MP * 0.34));
            }
            foreach(PartyMember pm in partyMembersDead) {
                partyMembersAlive.Add(pm);
            }
            partyMembersDead.Clear();
        }

        /// <summary>
        /// Increases the amount of WAX the party has
        /// </summary>
        /// <param name="amount"> Positive int to increase by </param>
        public void AddWAX(int amount) {
            WAX += amount;
            EventManager.instance.UpdateWAXAmounts();
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
            EventManager.instance.UpdateWAXAmounts();
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

        public void GetChampionChanceAll() {
            bonusChampionChance = 0;
            foreach (PartyMember pm in partyMembersAlive) {
                bonusChampionChance += pm.championChance;
            }
        }

        /// <summary>
        /// Equips a gear item to the activePartyMember
        /// </summary>
        /// <param name="id"> ItemDisplay </param>
        /// <param name="subType"> Subtype of gear (weapon, secondary, armour) </param>
        public void EquipGear(ItemDisplay id, string subType) {
            activePartyMember.EquipGear(id.displayedGear, id.subType);  // need to figure out if itemDisplay's item should be private
        }

        /// <summary>
        /// Unequips a gear item from the activePartyMember
        /// </summary>
        /// <param name="subType"> Subtype of gear (weapon, secondary, armour) </param>
        public void UnequipGear(string subType) {
            activePartyMember.UnequipGear(subType);  // need to figure out if itemDisplay's item should be private
        }

        /// <summary>
        /// Equips the active partyMember with a candle, altering stats
        /// </summary>
        /// <param name="id"> itemDisplay </param>
        /// <param name="subType"> subType for candles is a number correlating to the equippable index (0, 1, or 2) </param>
        public void EquipCandle(ItemDisplay id, string subType) {
            int index = (subType[0] - '0');
            activePartyMember.EquipCandle(id.displayedCandle, index);
        }

        /// <summary>
        /// Unequips the active partyMember with a candle, altering stats
        /// </summary>
        /// <param name="subType"> subType for candles is a number correlating to the equippable index (0, 1, or 2) </param>
        public void UnequipCandle(string subType) {
            int index = (subType[0] - '0');
            activePartyMember.UnequipCandle(index);
        }

        /// <summary>
        /// Use one of the active partyMember's candles
        /// </summary>
        /// <param name="index"> Index (0, 1, or 2) correlating to the active candle index </param>
        public void UseCandle(int index) {
            activePartyMember.UseCandle(index);
        }

        /// <summary>
        /// Have each partyMember rekindle all of their active candles
        /// </summary>
        public void Rekindle() {
            foreach (PartyMember pm in partyMembersAlive) {
                pm.Rekindle();
            }
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
        /// Looks through the party to see if the matching skill is enabled, and trigger it
        /// if it is
        /// </summary>
        /// <param name="className"> PartyMember's the class </param>
        /// <param name="index"> Index of skill </param>
        /// <param name="cpm"> Character that just attacked if in combat </param>
        public void TriggerSkillEnabled(string className, int index, Character c = null) {
            foreach (PartyMember pm in partyMembersAlive) {
                if (pm.className == className) {
                    if (pm.skills[(int)index].skillEnabled == true) {
                        pm.TriggerSkill(className, index, c);
                    }
                }
            }
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
