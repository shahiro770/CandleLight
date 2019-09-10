/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 11, 2019
* 
* The PartyMember class is used to store information about a member of the party. 
* It is always attached to a PartyMember GameObject.
*
*/

using Combat;
using Localization;
using Events;
using PanelConstants = Constants.PanelConstants;
using Party;
using PlayerUI;
using System.Collections;
using UnityEngine;

namespace Characters {

    public class PartyMember : Character {
        
        /* external component references */
        public PartyMemberVisualController pmvc;    /// <value> Handles all visual components related to partyMember </value>

        public string className { get; set; }       /// <value> Warrior, Mage, Archer, or Thief </value>
        public string subClassName { get; set; }    /// <value> Class specializations </value>
        public string pmName { get; set; }          /// <value> Name of the partyMember </value>
        public string race { get; set; }            /// <value> Human, Lizardman, Undead, etc. </value>
        public int EXP { get; set; }                /// <value> Current amount of experience points </value>
        public int EXPToNextLVL { get; set; }       /// <value> Total experience points to reach next level </value>
        public bool doneEXPGaining { get; private set; } = false;   /// <value> Total experience points to reach next level </value>

        /// <summary>
        /// When a PartyMember GO is instantiated, it needs to have its values initialized
        /// </summary> 
        /// <param name="personalInfo"> className, subClassName, memberName, and race in an array </param>
        /// <param name="LVL"> Power level </param>
        /// <param name="EXP"> Experience points </param>
        /// <param name="HP"> Health points </param>
        /// <param name="MP"> Mana Points </param>
        /// <param name="stats"> STR, INT, DEX, LUK </param>
        /// <param name="attacks"> List of known attacks (length 4)</param>
        public void Init(string[] personalInfo, int LVL, int EXP, int HP, int MP, int[] stats, Attack[] attacks) {
            base.Init(LVL, HP, MP, stats, attacks);
            this.EXP = EXP;
            this.EXPToNextLVL = CalcEXPToNextLVL(LVL);
            this.className = personalInfo[0];
            this.subClassName = personalInfo[1];
            this.pmName = personalInfo[2];
            this.race = personalInfo[3];

            pmvc.Init(this);
        }

        /// <summary>
        /// Sets EXPToNextLevel based off of a math
        /// </summary>
        /// <param name="level"> Level to calculate EXP to next level for </param>
        public int CalcEXPToNextLVL(int LVL) {
            return (10 * (LVL * 2 - 1)) / 2; 
        }

        /// <summary>
        /// Levels up a partyMember character; overriding base for different scaling for classes
        /// </summary>
        /// <param name="multiplier"> Multiplier because base needed it, won't be used here </param>
        public override void LVLUp(int multiplier = 1) {
            LVL += 1;

            if (className == "Warrior") {
                STR += LVL * 2;
                DEX += (int)(LVL * 1.5);
                INT += (int)(LVL * 1.25);
                LUK += LVL;
            }
            else if (className == "Mage") {
                STR += LVL;
                DEX += (int)(LVL * 1.25);
                INT += LVL * 2;
                LUK += (int)(LVL * 1.5);
            }
            else if (className == "Archer") {
                STR += (int)(LVL * 1.5);
                DEX += LVL * 2;
                INT += (int)(LVL * 1.25);
                LUK += (int)(LVL * 1.25);
            }
            else if (className == "Thief") {
                STR += (int)(LVL * 1.25);
                DEX += (int)(LVL * 1.5);
                INT += (int)(LVL * 1.25);
                LUK += LVL * 2;
            }
            
            HP += (int)((STR * 0.5) + (DEX * 0.5));
            MP += (int)((INT * 0.5) + (LUK * 0.5));

            pmvc.UpdateHPAndMPBars();
        }

        /// <summary>
        /// Increases EXP and updates visuals that care
        /// </summary>
        /// <param name="amount"> Amount of EXP gained </param>
        public IEnumerator AddEXP(int amount) {
            doneEXPGaining = false;
            EXP += amount;

            if (EXP >= EXPToNextLVL) {
                int overflow = EXP;  
                int prevEXPToNextLVL = EXPToNextLVL;     

                while (overflow >= EXPToNextLVL) { // small chance player might level up more than once
                    LVLUp();
                    overflow -= EXPToNextLVL;
                    prevEXPToNextLVL = EXPToNextLVL;
                    EXPToNextLVL = CalcEXPToNextLVL(LVL);
                    yield return(StartCoroutine(pmvc.DisplayEXPChange(prevEXPToNextLVL, prevEXPToNextLVL)));
                }
                EXP = overflow;
            }
            yield return(StartCoroutine(pmvc.DisplayEXPChange(EXPToNextLVL, EXP)));

            doneEXPGaining = true;         
        }

        /// <summary>
        /// Increase the PartyMember's current health points by a specified amount.
        /// TODO: Cleanup logic so that AddHP manages whether or not the player recieve HP under
        /// various circumstances (e.g. only revival skills can bring a partyMember back from 0 CHP)
        /// </summary> 
        /// <param name="amount"> Amount of health points lost </param>
        public void AddHP(int amount) {    
            if (CHP == 0) { // Reviving a dead partyMember if CHP was originally 0 and addHp is allowed
                PartyManager.instance.RegisterPartyMemberAlive(this);
            }

            CHP += amount;

            if (CHP > HP) {
                CHP = HP;
            }

            StartCoroutine(pmvc.DisplayHPChange(false));
        }

        /// <summary>
        /// Increase the PartyMember's current mana points by a specified amount.
        /// TODO: Cleanup logic so that AddMP manages whether or not the player recieve MP under
        /// various circumstances (e.g. only revival skills can bring a partyMember back from 0 CHP)
        /// </summary> 
        /// <param name="amount"></param>
        public void AddMP(int amount) {
            CMP += amount;

            if (CMP > MP) {
                CMP = MP;
            }

            StartCoroutine(pmvc.DisplayMPChange(false));
        }

        /// <summary>
        /// Increase the partyMember's current mana and health points by 5% of their max amounts
        /// </summary>
        public void Regen() {
            AddHP((int)(Mathf.Ceil((float)HP * 0.05f)));
            AddMP((int)(Mathf.Ceil((float)MP * 0.05f)));
        }

        /// <summary>
        /// Reduce the PartyMember's current health points by a specified amount.false
        /// IEnumerator is used to make calling function wait for its completion
        /// </summary> 
        /// <param name="amount"> Amount of health points lost </param>
        public IEnumerator LoseHP(int amount) {
            // some sources such as results will use negative numbers to indicate loss
            amount = Mathf.Abs(amount);
            
            CHP -= amount;

            if (CHP <= 0) {
                CHP = 0;
                PartyManager.instance.RegisterPartyMemberDead(this);
            }

            yield return (StartCoroutine(pmvc.DisplayHPChange(true)));
        }

        /// <summary>
        /// Reduce the PartyMember's current health points by a specified amount.false
        /// IEnumerator is used to make calling function wait for its completion
        /// </summary> 
        /// <param name="amount"> Amount of health points lost </param>
        public IEnumerator LoseMP(int amount) {
            CMP -= amount;
            
            yield return (StartCoroutine(pmvc.DisplayMPChange(true)));
        }

        /// <summary>
        /// Check if the partyMember is dead
        /// </summary>
        /// <returns></returns>
        public bool CheckDeath() {
            return CHP == 0;
        }

        /// <summary>
        /// Reduce MP or HP after an attack is used depending on the attack's costs
        /// </summary>
        /// <param name="costType"> HP or MP </param>
        /// <param name="cost"> Amount to be lost </param>
        /// <returns> IEnumerator to pay cost before attack animations play </returns>
        public IEnumerator PayAttackCost(string costType, int cost) {
            if (costType == "MP") {
                yield return StartCoroutine(LoseMP(cost));
            } 
            else if (costType == "HP") {
                yield return StartCoroutine(LoseHP(cost));
            }
        }

        /// <summary>
        /// Log stats informaton about the PartyMember for debugging
        /// </summary> 
        public override void LogStats() {
            base.LogStats();
        }

        /// <summary>
        /// Log partyMember's class name
        /// </summary> 
        public override void LogName() {
            Debug.Log("PartyMemberName " + className);
        }
    }
}
