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

        public Gear weapon = new Gear();        /// <value> Weapon </value>
        public Gear secondary = new Gear();     /// <value> Secondary </value>
        public Gear armour = new Gear();        /// <value> Armour </value>

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
                baseSTR += (int)(LVL * 1.75);
                baseDEX += (int)(LVL * 1.5);
                baseINT += (int)(LVL * 1.25);
                baseLUK += LVL;
            }
            else if (className == "Mage") {
                baseSTR += LVL;
                baseDEX += (int)(LVL * 1.25);
                baseINT += (int)(LVL * 1.75);
                baseLUK += (int)(LVL * 1.5);
            }
            else if (className == "Archer") {
                baseSTR += (int)(LVL * 1.5);
                baseDEX += (int)(LVL * 1.75);
                baseINT += (int)(LVL * 1.25);
                baseLUK += LVL;
            }
            else if (className == "Thief") {
                baseSTR += LVL;
                baseDEX += (int)(LVL * 1.5);
                baseINT += (int)(LVL * 1.25);
                baseLUK += LVL * 2;
            }

            CalculateGearStatsPrimary(1);
            CalculateSecondaryStats();
            CalculateGearStatsSecondary();

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
            AddHP((int)(Mathf.Ceil((float)HP * 0.06f)));
            AddMP((int)(Mathf.Ceil((float)MP * 0.06f)));
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
        /// Updates all stats after a piece of equipment is equipped
        /// </summary>
        /// <param name="g"></param>
        /// <param name="subType"></param>
        public void EquipGear(Gear g, string subType) {
            if (subType == "weapon") {
                weapon = g;
            }
            else if (subType == "secondary") {
                secondary = g;
            }
            else if (subType == "armour") {
                armour = g;
            }

            CalculateGearStatsPrimary(1);
            CalculateSecondaryStats();
            CalculateGearStatsSecondary();
            pmvc.UpdateStats();
            pmvc.SetEquippedGear();
        }

        /// <summary>
        /// Updates all stats after a piece of gear is unequipped
        /// </summary>
        /// <param name="subType"></param>
        public void UnequipGear(string subType) {
            if (subType == "weapon") {
                weapon = null;
            }
            else if (subType == "secondary") {
                secondary = null;
            }
            else if (subType == "armour") {
                armour = null;
            }

            CalculateGearStatsPrimary(1);
            CalculateSecondaryStats();
            CalculateGearStatsSecondary();
            pmvc.UpdateStats();
            pmvc.SetEquippedGear();
        }

        /// <summary>
        /// Calculates stat changes to primary stats (STR, DEX, INT, LUK) after a gear is equipped
        /// </summary>
        /// <param name="addOrSubtract"></param>
        public void CalculateGearStatsPrimary(int addOrSubtract) {
            Gear gearToCalculate = null;
            int numGear = 3;
            STR = baseSTR;
            DEX = baseDEX;
            INT = baseINT;
            LUK = baseLUK;

            for (int i = 0; i < numGear; i++) {
                if (i == 0) {
                    gearToCalculate = weapon;
                }
                else if (i == 1) {
                    gearToCalculate = secondary;
                }
                else {
                    gearToCalculate = armour;
                }

                if (gearToCalculate != null) {
                    for (int j = 0; j < gearToCalculate.effects.Length; j++) {
                        switch(gearToCalculate.effects[j]) {
                            case "STR":
                                STR += gearToCalculate.values[j] * addOrSubtract;
                                break;
                            case "DEX":
                                DEX += gearToCalculate.values[j] * addOrSubtract;
                                break;
                            case "INT":
                                INT += gearToCalculate.values[j] * addOrSubtract;
                                break;
                            case "LUK":
                                LUK += gearToCalculate.values[j] * addOrSubtract;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Calculates all secondary stats (HP, MP, PATK, MATK, PDEF, MDEF, DODGE, ACC, CRITCHANCE, CRITMULT)
        /// </summary>
        public void CalculateGearStatsSecondary() {
            Gear gearToCalculate = null;
            int numGear = 3;

            for (int i = 0; i < numGear; i++) {
                if (i == 0) {
                    gearToCalculate = weapon;
                }
                else if (i == 1) {
                    gearToCalculate = secondary;
                }
                else {
                    gearToCalculate = armour;
                }

                if (gearToCalculate != null) {
                    for (int j = 0; j < gearToCalculate.effects.Length; j++) {
                        switch(gearToCalculate.effects[j]) {
                            case "HP":
                                HP += gearToCalculate.values[j];
                                break;
                            case "MP":
                                MP += gearToCalculate.values[j];
                                break;
                            case "PATK":
                                PATK += gearToCalculate.values[j];
                                break;
                            case "MATK":
                                MATK += gearToCalculate.values[j];
                                break;
                            case "PDEF":
                                PDEF += gearToCalculate.values[j];
                                break;
                            case "MDEF":
                                MDEF += gearToCalculate.values[j];
                                break;
                            case "DODGE":
                                dodge += gearToCalculate.values[j];
                                break;
                            case "ACC":
                                acc += gearToCalculate.values[j];
                                break;
                            case "CRITCHANCE":
                                critChance += gearToCalculate.values[j];
                                break;
                            case "CRITMULT":
                                critMult += gearToCalculate.values[j];
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
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
