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
using Items;
using Party;
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

        public Item weapon = new Item();        /// <value> Weapon </value>
        public Item secondary = new Item();     /// <value> Secondary </value>
        public Item armour = new Item();        /// <value> Armour </value>

        /// <summary>
        /// When a PartyMember GO is instantiated, it needs to have its values initialized
        /// </summary> 
        /// <param name="personalInfo"> className, subClassName, memberName, and race in an array </param>
        /// <param name="LVL"> Power level </param>
        /// <param name="EXP"> Experience points </param>
        /// <param name="CHP"> Current health points, for now irrelevant, but will be used for saving </param>
        /// <param name="CMP"> Current mana Points, for now irrelevant, but will be used for saving </param>
        /// <param name="stats"> STR, INT, DEX, LUK </param>
        /// <param name="attacks"> List of known attacks (length 4)</param>
        public void Init(string[] personalInfo, int LVL, int EXP, int CHP, int CMP, int[] stats, Attack[] attacks) {
            base.Init(LVL, CHP, CMP, stats, attacks);
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
            // it takes 5 LVL 1 enemies for a LVL 1 player to reach LVL 2
            // it takes 47 LVL 98 enemies for LVL 98 player to reach LVL 99
            return (int)(5 * Mathf.Pow(LVL, 2.15f) + 3); 
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
            
            CalculateSecondaryStats();

            pmvc.UpdateHPAndMPBars();
            pmvc.UpdateStats();
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
                    overflow -= EXPToNextLVL;
                    prevEXPToNextLVL = EXPToNextLVL;
                    EXPToNextLVL = CalcEXPToNextLVL(LVL + 1);   // need this value to change the EXP display, but can't LVL up until after bar fills
                    yield return(StartCoroutine(pmvc.DisplayEXPChange(prevEXPToNextLVL, prevEXPToNextLVL)));
                    LVLUp();
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

            if (CHP < 0) {
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
        /// Handles all logic and visuals for when this partyMember is attacked
        /// </summary>
        /// <param name="a"> Attack used </param>
        /// <returns></returns>
        public IEnumerator GetAttacked(Attack a, Character c) {
            bool attackHit = CalculateAttackHit(c);

            if (attackHit) {
                int damage = CalculateAttackDamage(a);;
                bool isCrit = CalculateAttackCrit(c);
                if (isCrit) {
                    damage = CalculateAttackDamageCrit(damage, c);
                    damage = CalculateAttackReductions(damage, a);
                }
                else {
                    damage = CalculateAttackReductions(damage, a);
                }
                
                pmvc.SetDamageTaken(damage, isCrit);

                yield return StartCoroutine(LoseHP(damage));
            }
            else {
                yield return StartCoroutine(DodgeAttack());
            }
        }

        /// <summary>
        /// Handles all logic and tells the visual component to update after dodging an attack
        /// </summary>
        /// <returns></returns>
        public IEnumerator DodgeAttack() {
            yield return StartCoroutine(pmvc.DisplayAttackDodged());
        }

        /// <summary>
        /// Log stats informaton about the PartyMember for debugging
        /// </summary> 
        public override void LogPrimaryStats() {
            base.LogPrimaryStats();
        }

        /// <summary>
        /// Log partyMember's class name
        /// </summary> 
        public override void LogName() {
            Debug.Log("PartyMemberName " + className);
        }
    }
}
