/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 23, 2019
* 
* The PartyManager class is used to manage the partyMembers and their information.
* It is held within the Game scene where it can always be accessed globally.
*
*/

using Candle = Items.Candle;
using ClassConstants = Constants.ClassConstants;
using Characters;
using Events;
using General;
using PlayerUI;
using ItemConstants = Constants.ItemConstants;
using SkillConstants = Constants.SkillConstants;
using ResultConstants = Constants.ResultConstants;
using System.Collections;
using System.Collections.Generic;
using TutorialConstants = Constants.TutorialConstants;
using UnityEngine;

namespace Party {

    public class PartyManager : MonoBehaviour {
        
        public static PartyManager instance;    /// <value> global instance </value>
        
        /* external component references */
        public GameObject partyMember;          /// <value> partyMember game object to instantiate </value>

        public string storedPartyMember;        /// <value> Classname of the partyMember to add </value>
        public int bonusChampionChance = 0;     /// <value> Chance of encountering champion monsters, summed from all partyMembers </value>
        public int WAX { get; private set; }    /// <value> Currency party has stored up </value>
        public float itemDropMultiplier = 1f;   /// <value> Current multiplier on item drop rates from enemies </value>
        public float WAXDropMultiplier = 1f;    /// <value> Current multiplier on WAX drop amounts from enemies </value>

        private List<PartyMember> partyMembersAll = new List <PartyMember>();   /// <value> List containing all partyMembers (alive and dead) </value>
        private List<PartyMember> partyMembersAlive = new List<PartyMember>();  /// <value> List of partyMembers in party </value>
        private List<PartyMember> partyMembersDead = new List<PartyMember>();   /// <value> List of partyMembers in party </value>
        private List<PartyMember> summons = new List<PartyMember>();            /// <value> List of summons loaded </value>
        private PartyMember activePartyMember = null;
        private enum primaryStats { NONE, STR, DEX, INT, LUK };                 /// <value> Enumerated primary stats </value>
        private string[] summonNames = new string[] { ClassConstants.FROSTGOLEM };  /// <value> Name of all possible summons </value>
        private int maxPartyMembers = 4;                                        /// <value> Max number of partyMembers </value>
        private int ID = 0;                                                     /// <value> ID number to assign to each pm</value>
        private bool shouldStore = true;                                        /// <value> Flag for if need to store the next partyMember (tutorial only) </value>

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
            if (GameManager.instance.tutorialTriggers[(int)TutorialConstants.tutorialTriggers.isTutorial] == true && GetNumPartyMembers() == 1 && shouldStore == true) { // if in tutorial, the second partyMember joins later
                storedPartyMember = className;
                shouldStore = false;
            }
            else if (GetNumPartyMembers() < maxPartyMembers) {
                GameObject newMember = Instantiate(partyMember, new Vector3(0f,0f,0f), Quaternion.identity);
                PartyMember pmComponent =  newMember.GetComponent<PartyMember>();
                GameManager.instance.DB.GetPartyMemberByClass(className, newMember.GetComponent<PartyMember>());
                newMember.transform.SetParent(gameObject.transform, false);
                pmComponent.ID = (ID++);
                pmComponent.GenerateName(GetNumPartyMembers());
                if (GameManager.instance.tutorialTriggers[(int)TutorialConstants.tutorialTriggers.isTutorial] == true) {
                    pmComponent.LVLDown();
                }
                partyMembersAlive.Add(pmComponent);
                partyMembersAll.Add(pmComponent);
            }

            activePartyMember = GetFirstPartyMemberAlive();
        }

        /// <summary>
        /// Adds a partyMember to the list of partyMembers using saved data
        /// </summary>   
        /// <param name="className"> Class of the partyMember to be added </param>
        public void AddPartyMember(PartyMemberData pmData) {
            if (GetNumPartyMembers() < maxPartyMembers) {
                GameObject newMember = Instantiate(partyMember, new Vector3(0f,0f,0f), Quaternion.identity);
                PartyMember pmComponent =  newMember.GetComponent<PartyMember>();
                pmComponent.Init(pmData);
                newMember.transform.SetParent(gameObject.transform, false);
                pmComponent.ID = (ID++);
                pmComponent.GenerateName(GetNumPartyMembers());
                if (pmComponent.CHP == 0) {
                    partyMembersDead.Add(pmComponent);
                }
                else {
                    partyMembersAlive.Add(pmComponent);
                    
                }
                partyMembersAll.Add(pmComponent);
            }

            if (partyMembersAlive.Count == 0) {
                activePartyMember = partyMembersAll[0];
            }
            else {
                activePartyMember = GetFirstPartyMemberAlive();
            }
        }

        public void AddStoredPartyMember() {
            AddPartyMember(storedPartyMember);
            EventManager.instance.SetPartyMembertNotification(partyMembersAll[1].pmName);
            EventManager.instance.UpdatePartyMembers();
            storedPartyMember = null;
        }

        /// <summary>
        /// Load the summoned partyMembers (so they don't need to be fetched from the database at runtime)
        /// </summary>
        public void LoadSummons() {
            foreach (string summonName in summonNames) {
                GameObject summon = Instantiate(partyMember, new Vector3(0f,0f,0f), Quaternion.identity);
                PartyMember pmComponent =  summon.GetComponent<PartyMember>();
                GameManager.instance.DB.GetPartyMemberByClass(summonName, summon.GetComponent<PartyMember>());
                summon.gameObject.transform.SetParent(gameObject.transform, false);
                summons.Add(pmComponent);
            }
        }

        /// <summary>
        /// Adds a loaded summon to the party
        /// </summary>
        /// <param name="summoner"> PartyMember summoning this </param>
        /// <param name="className"> Class of the summon (which is the unique ID) </param>
        /// <param name="ID"> Unique ID to be assigned to this summon </param>
        public void AddPartyMemberSummon(PartyMember summoner, string className, int ID) {
            GameObject clone = Instantiate(partyMember, new Vector3(0f,0f,0f), Quaternion.identity);
            PartyMember summon = clone.GetComponent<PartyMember>();
            summon.gameObject.transform.SetParent(gameObject.transform, false);
            summon.ID = ID;
            summon.InitSummon(summons.Find(s => s.className == className), summoner);
            
            partyMembersAlive.Add(summon);
            partyMembersAll.Add(summon);
            EventManager.instance.UpdatePartyMembers();
            summon.GetSummonBuff();     // have to add the buff AFTER the summon's pmvc is initted (so all visual components can be used)
        }
        
        /// <summary>
        /// Remove a partyMember from the party
        /// </summary>
        /// <param name="pm"> partyMember to remove </param>
        public void RemovePartyMember(PartyMember pm) {
            pm.RemoveAllStatusEffects();
            partyMembersAll.Remove(pm);
            partyMembersAlive.Remove(pm);
            partyMembersDead.Remove(pm);
            EventManager.instance.UpdatePartyMembers();
            Destroy(pm.gameObject);
        }

        /// <summary>
        /// Removes all partyMembers from PartyMembers list
        /// </summary>
        public void ResetGame() {
            shouldStore = true;
            WAX = 0;
            ID = 0;
            for (int i = partyMembersAll.Count - 1; i >= 0; i--)  {    // not sure if this is redundant
                Destroy(partyMembersAll[i].gameObject); 
            }
            partyMembersAll.Clear();
            partyMembersAlive.Clear();
            partyMembersDead.Clear();
        }

        /// <summary>
        /// Load partymanager specific values from saveData
        /// </summary>
        /// <param name="data"></param>
        public void LoadData(SaveData data) {
            WAX = data.WAX;
            ID = 0;
            for (int i = partyMembersAll.Count - 1; i >= 0; i--)  {    // not sure if this is redundant
                Destroy(partyMembersAll[i].gameObject); 
            }
            partyMembersAll.Clear();
            partyMembersAlive.Clear();
            partyMembersDead.Clear();

            foreach (PartyMemberData pmData in data.partyMemberDatas) {
                PartyManager.instance.AddPartyMember(pmData);
            }
        }
        
        /// <summary>
        /// Returns the list of partyMembers
        /// </summary>
        /// <returns> List of partyMembers </returns>
        public List<PartyMember> GetPartyMembers() {
            return partyMembersAll;
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

        public string GetPartyMemberName(int index) {
            return partyMembersAll[index].pmName;
        }

        public void RotatePartyMember(int amount) {
            int index = partyMembersAll.FindIndex(p => activePartyMember.ID == p.ID);
            index += amount;
             if (index == partyMembersAll.Count) {
                index = 0;
            }
            else if (index < 0) {
                index = partyMembersAll.Count - 1;
            }
            SetActivePartyMember(partyMembersAll[index]);
        }

        /// <summary>
        /// Sets the active partyMember
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
        /// Returns the first partyMember with an item item slot that can equip the gear
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public PartyMember GetAvailablePartyMember(ItemDisplay id) {
            foreach (PartyMember pm in partyMembersAll) {
                if (pm.className == id.className || id.className == ItemConstants.ANY) {    // find the first party member that can equip the item with nothing equipped
                    if (id.subType == ItemConstants.WEAPON && pm.weapon == null) {
                        return pm;
                    }
                    else if (id.subType == ItemConstants.SECONDARY && pm.secondary == null) {
                        return pm;
                    }
                    else if (id.subType == ItemConstants.ARMOUR && pm.armour == null) {
                        return pm;
                    }
                }
            }
            foreach (PartyMember pm in partyMembersAll) {   // find the first party member that can equip the item otherwise
                if (pm.className == id.className) {
                    return pm;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the highest LVL a partyMember has obtained
        /// </summary>
        /// <returns></returns>
        public int GetHighestPartyMemberLVL() {
            int max = 0;
            foreach (PartyMember pm in partyMembersAll) {
                if (pm.LVL > max) {
                    max = pm.LVL;
                }
            }

            return max;
        }

        /// <summary>
        /// Returns false if the partyMember at the index is alive
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool CheckDeath(int index) {
            return partyMembersAll[index].CheckDeath() == true;
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
        /// Change CHP of all partyMembers, only used outside of combat
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="type"> for certain result types, hp may be a loss, so need to pass it in and check </param>
        public IEnumerator ChangeHPAll(int amount, string type = "none") {
            if (amount >= 0) {
                foreach (PartyMember pm in partyMembersAlive) {
                    pm.AddHP(amount);
                }
            }
            else {
                for (int i = 0; i < partyMembersAll.Count; i++) {
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
                    else {
                        yield return null; // I GUESS?????
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
                pm.AddHP((int)(pm.HP * 0.34f));
                pm.AddMP((int)(pm.MP * 0.34f));
            }
            foreach(PartyMember pm in partyMembersDead) {
                partyMembersAlive.Add(pm);
            }
            partyMembersDead.Clear();
        }

        /// <summary>
        /// Similar to a revive, except for summons, it always restore the summon
        /// to full health and mana, purging status effects, and may be used while the summon
        /// is alive (hence not always needing to register the partyMember alive again)
        /// </summary>
        /// <param name="summon"></param>
        public void RestoreSummon(PartyMember summon) {
            if (summon.CheckDeath() == true) {
                RegisterPartyMemberAlive(summon);
            }
            summon.AddHP((int)(summon.HP));
            summon.AddMP((int)(summon.MP));  
            summon.RemoveAllStatusEffects();   
            summon.GetSummonBuff();
        }

        /// <summary>
        /// Increases the amount of WAX the party has
        /// </summary>
        /// <param name="amount"> Positive int to increase by </param>
        public void AddWAX(int amount) {
            WAX += amount;
            GameManager.instance.WAXobtained += amount;
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

        public void SetBonusChampionChance() {
            bonusChampionChance = 0;
            foreach (PartyMember pm in partyMembersAlive) {
                bonusChampionChance += pm.championChance;
            }
        }

        /// <summary>
        /// Returns true if a given class is in the party
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public bool IsClassInParty(string className) {
            foreach (PartyMember pm in partyMembersAll) {
                if (pm.className == className) {
                    return true;
                }
            }
            return false;
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

        /// <summary>
        /// Returns true if the party has someone with a candle equipped, false otherwise
        /// </summary>
        /// <returns></returns>
        public bool IsCandlesEquipped() {
            foreach(PartyMember pm in partyMembersAlive) {
                foreach (Candle c in pm.activeCandles) {
                    if (c != null) {
                        return true;
                    }
                }
            }

            return false;
        }

        public int GetSkillPoints() {
            return activePartyMember.skillPoints;
        }

        /// <summary>
        /// Gets the number of unused skill points of all partyMember
        /// </summary>
        /// <returns></returns>
        public int GetSkillPointsAll() {
            int skillPointsTot = 0;
            foreach(PartyMember pm in partyMembersAll) {
                skillPointsTot += pm.skillPoints;
            }

            return skillPointsTot;
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

        /// <summary>
        /// Returns partyMember data for saving
        /// </summary>
        /// <returns></returns>
        public PartyMemberData[] GetPartyMemberDatas() {
            PartyMemberData[] partyMemberDatas;
            partyMemberDatas = new PartyMemberData[partyMembersAll.Count];

            for (int i = 0; i < partyMemberDatas.Length; i++) {
                partyMemberDatas[i] = new PartyMemberData(partyMembersAll[i]);
            }

            return partyMemberDatas;
        }

        /// <summary>
        /// Get all the class names of partyMembers
        /// This is only used if the player tries to restart the tutorial segment
        /// </summary>
        /// <returns></returns>
        public string[] GetPartyComposition() {
            string[] partyComposition;

            // if in tutorial, the second partyMember joins later, so need to use storedPartyMember if that partyMember hasn't joined yet
            if (GameManager.instance.tutorialTriggers[(int)TutorialConstants.tutorialTriggers.isTutorial] == true && storedPartyMember != null) { 
                partyComposition = new string[partyMembersAll.Count + 1];
                partyComposition[0] = partyMembersAll[0].className;
                partyComposition[1] = storedPartyMember;
            }
            else {
                partyComposition = new string[partyMembersAll.Count];

                for (int i = 0; i < PartyManager.instance.GetPartyMembers().Count; i++) {
                   partyComposition[i] = partyMembersAll[i].className;
                }
            }

            return partyComposition;
        }
    }
}
