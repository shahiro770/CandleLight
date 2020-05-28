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
using Skills;
using SkillConstants = Constants.SkillConstants;
using System.Collections;
using System.Collections.Generic;
using StatusEffectConstants = Constants.StatusEffectConstants;
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
        public int skillPoints;
        public bool doneEXPGaining { get; private set; } = false;   /// <value> Total experience points to reach next level </value>

        public Attack noneAttack = new Attack("none", "physical", "0", "none", 0, 0, "MP", "0", "single", "none");
        public Skill[] skills = new Skill[12];
        public Gear weapon = new Gear();        /// <value> Weapon </value>
        public Gear secondary = new Gear();     /// <value> Secondary </value>
        public Gear armour = new Gear();        /// <value> Armour </value>
        
        private int numGear = 3;                /// <value> Max number of gear items a partyMember has </value>

        /// <summary>
        /// When a PartyMember GO is instantiated, it needs to have its values initialized
        /// </summary> 
        /// <param name="personalInfo"> className, subClassName, memberName, and race in an array </param>
        /// <param name="LVL"> Power level </param>
        /// <param name="EXP"> Experience points </param>
        /// <param name="CHP"> Current health points, for now irrelevant, but will be used for saving </param>
        /// <param name="CMP"> Current mana Points, for now irrelevant, but will be used for saving </param>
        /// <param name="stats"> STR, INT, DEX, LUK </param>
        /// <param name="attacks"> Array of known attacks (length 4)</param>
        public void Init(string[] personalInfo, int LVL, int EXP, int CHP, int CMP, int[] stats, Attack[] attacks, Skill[] skills) {
            base.Init(LVL, CHP, CMP, stats, attacks);
            CalculateStats(true);
            this.EXP = EXP;
            this.EXPToNextLVL = CalcEXPToNextLVL(LVL);
            this.className = personalInfo[0];
            this.subClassName = personalInfo[1];
            this.pmName = personalInfo[2];
            this.race = personalInfo[3];
            this.skills = skills;

            skillPoints = 1;

            pmvc.Init(this);
        }

        /// <summary>
        /// Sets EXPToNextLevel based off of a math
        /// </summary>
        /// <param name="level"> Level to calculate EXP to next level for </param>
        public int CalcEXPToNextLVL(int LVL) {
            // it takes 4 LVL 1 enemies for a LVL 1 player to reach LVL 2
            // it takes 47 LVL 98 enemies for LVL 98 player to reach LVL 99
            return (int)(5 * Mathf.Pow(LVL, 2.21f) + 3 * LVL); 
        }

        /// <summary>
        /// Levels up a partyMember character; overriding base for different scaling for classes
        /// </summary>
        /// <param name="multiplier"> Multiplier because base needed it, won't be used here </param>
        public override void LVLUp(int multiplier = 1) {
            LVL += 1;
            if (LVL % 2 == 1) {     // gain a skill point every other level
                skillPoints++;
                pmvc.ExciteSkillsTab();
            }

            if (className == "Warrior") {
                baseSTR += (int)(LVL * 1.5);
                baseDEX += (int)(LVL * 1.5);
                baseINT += (int)(LVL * 1.25);
                baseLUK += (int)(LVL * 1.25);
            }
            else if (className == "Mage") {
                baseSTR += LVL;
                baseDEX += (int)(LVL * 1.25);
                baseINT += (int)(LVL * 1.75);
                baseLUK += (int)(LVL * 1.5);
            }
            else if (className == "Archer") {
                baseSTR += (int)(LVL * 1.25);
                baseDEX += (int)(LVL * 1.75);
                baseINT += (int)(LVL * 1.5);
                baseLUK += LVL;
            }
            else if (className == "Thief") {
                baseSTR += LVL;
                baseDEX += (int)(LVL * 1.5);
                baseINT += (int)(LVL * 1.25);
                baseLUK += LVL * 2;
            }


            CalculateStats();

            pmvc.UpdateHPAndMPBars();
            pmvc.UpdateStats();
        }

        protected override void CalculateStats(bool setCurrent = false) {
            Gear gearToCalculate = null;

            STR = baseSTR;
            DEX = baseDEX;
            INT = baseINT;
            LUK = baseLUK;

            /* primary stats changes from gear */
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
                                STR += gearToCalculate.values[j];
                                break;
                            case "DEX":
                                DEX += gearToCalculate.values[j];
                                break;
                            case "INT":
                                INT += gearToCalculate.values[j];
                                break;
                            case "LUK":
                                LUK += gearToCalculate.values[j];
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            /* secondary stats */
            HP = (int)(STR * 2.25 + DEX * 1.5);
            MP = (int)(INT * 1.25 + LUK * 0.5);
            PATK = (int)(STR * 0.5 + DEX * 0.25);
            MATK = (int)(INT * 0.5 + LUK * 0.25); 
            PDEF = (int)(STR * 0.1 + DEX * 0.05);
            MDEF = (int)(INT * 0.15 + LUK * 0.05);
            DOG = (int)(DEX * 0.2 + LUK * 0.1);
            ACC = (int)(DEX * 0.2 + STR * 0.1 + INT * 0.1) + defaultACC;
            critChance = (int)(LUK * 0.1) + baseCritChance;
            critMult = baseCritMult;

            /* secondary stats changes from gear */
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
                            case "DOG":
                                DOG += gearToCalculate.values[j];
                                break;
                            case "ACC":
                                ACC += gearToCalculate.values[j];
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

            /* secondary stat changes from skills */
            if (className == "Mage") {
                if (skills[(int)SkillConstants.mageSkills.CRITICALMAGIC].skillEnabled == true) {
                     critChance = (int)(critChance * 1.25f);
                }
            }

            /* secondary stat changes from status effects */
            foreach (StatusEffect se in statusEffects) {
                if (se.name == StatusEffectConstants.TAUNT || se.name == StatusEffectConstants.RAGE) {
                    PATK += (int)(PATK * 0.5);
                }
                else if (se.name == StatusEffectConstants.FREEZE) {
                    DOG -= (int)(DOG * 0.3);
                    ACC -= (int)(ACC * 0.3);
                    PDEF -= (int)(PDEF * 0.3);
                }
                else if (se.name == StatusEffectConstants.WEAKNESS) {
                    PATK -= (int)(PATK * 0.3);
                }
                else if (se.name == StatusEffectConstants.ADVANTAGE) {
                    critChance += 50;
                }
                else if (se.name == StatusEffectConstants.ROOT) {
                    DOG -= (int)(DOG * 0.5);;
                }
            }
            
            if (setCurrent) {
                CHP = HP;
                CMP = MP;
            }

            if (CHP > HP) {
                CHP = HP;
            }
            if (CMP > MP) {
                CMP = MP;
            }
            if (critChance > 100) {
                critChance = 100;
            }
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
                    yield return(StartCoroutine(pmvc.DisplayEXPChange(prevEXPToNextLVL, prevEXPToNextLVL, LVL + 1)));
                    LVLUp();
                }
                EXP = overflow;
            }
            yield return(StartCoroutine(pmvc.DisplayEXPChange(EXPToNextLVL, EXP, LVL)));

            doneEXPGaining = true;         
        }

        /// <summary>
        /// Increase the PartyMember's current health points by a specified amount.
        /// TODO: Cleanup logic so that AddHP manages whether or not the player recieve HP under
        /// various circumstances (e.g. only revival skills can bring a partyMember back from 0 CHP)
        /// </summary> 
        /// <param name="amount"> Amount of health points lost </param>
        public void AddHP(int amount) {    
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
            if (className != "Mage") {
                AddMP((int)(Mathf.Ceil((float)MP * 0.12f)));
            }
            else {  
                if (skills[(int)SkillConstants.mageSkills.THIRDEYE].skillEnabled == true) {
                    AddMP((int)(Mathf.Ceil((float)MP * 0.24f)));
                }
                else {
                    AddMP((int)(Mathf.Ceil((float)MP * 0.12f)));
                }
            }
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
                RemoveStatusEffects();
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
                bool isStatus = CalculateAttackStatus(a, c);
                if (isCrit) {
                    damage = CalculateAttackDamageCrit(damage, c);
                    damage = CalculateAttackReductions(damage, a);
                }
                else {
                    damage = CalculateAttackReductions(damage, a);
                }
                
                pmvc.SetDamageTaken(damage, isCrit);

                yield return StartCoroutine(LoseHP(damage));
                
                if (isStatus && CheckDeath() == false) {
                    AddStatusEffect(a.seName, a.seDuration, c);
                }

                UpdateStatusEffectValues();
            }
            else {
                yield return StartCoroutine(DodgeAttack());
            }
        }

        /// <summary>
        /// Handles the calculations involved when attacked by an attack that only has a statusEffect
        /// associated with it
        /// </summary>
        /// <param name="a"></param>
        /// <param name="c"> Character statusing this </param>
        /// <returns></returns>
        public IEnumerator GetStatusEffected(Attack a, Character c) {
            bool attackHit = CalculateAttackHit(c);
           
            if (attackHit) {
                if (GetStatusEffect(a.seName) == -1) {  // no two tatusEffects of the same type can be on at once
                    StatusEffect newStatus = new StatusEffect(a.seName, a.seDuration);
                    newStatus.SetValue(c, this);
                    AddStatusEffect(newStatus);
                    pmvc.AddStatusEffectDisplay(newStatus);

                    UpdateStatusEffectValues();
                }
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
        /// Adds a status effect to the partyMember
        /// </summary>
        /// <param name="seName"> Name of the statusEffect </param>
        /// <param name="seDuration"> Duration of the statusEffect </param>
        /// <param name="c"> Character afflicting the statusEffect on this character, can be null for some effects </param>
        public void AddStatusEffect(string seName, int seDuration, Character c) {
            if (GetStatusEffect(seName) == -1) {  // no two statusEffects of the same type can be on at once
                StatusEffect newStatus = new StatusEffect(seName, seDuration);
                newStatus.SetValue(c, this);
                AddStatusEffect(newStatus);
                pmvc.AddStatusEffectDisplay(newStatus);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inCombat"> 
        /// Flag for if the statusEffect is triggering in combat.
        /// Status effects cannot kill the player outside of combat.
        /// </param>
        /// <returns></returns>
        public IEnumerator TriggerStatuses(bool inCombat) {
            int damageTaken = 0;
            int[] animationsToPlay = new int[] { 0 ,0, 0 }; 

            foreach (StatusEffect se in statusEffects) {
                if (se.name == StatusEffectConstants.BURN) {
                    damageTaken += se.value;
                    animationsToPlay[0] = 1;
                }
                else if (se.name == StatusEffectConstants.POISON) {
                    damageTaken += se.value;
                    animationsToPlay[1] = 1;
                }
                else if (se.name == StatusEffectConstants.BLEED) {
                    int bleedDamage = se.value;
                    damageTaken += bleedDamage;
                    if (se.afflicter != null) {
                        ((Monster)(se.afflicter)).AddHP(bleedDamage);
                    }
                    animationsToPlay[2] = 1;
                }
                se.UpdateDuration();
                
                if (se.duration == 0) {
                    seToRemove.Add(se);
                }
            }

            if (inCombat == true) { // if in combat, always yield to status effect animations
                pmvc.DisplayCleanUpStatusEffects(animationsToPlay);
                if (damageTaken > 0) {
                    pmvc.SetDamageTaken(damageTaken, false);
                    yield return StartCoroutine(LoseHP(damageTaken));
                }
            }
            else {
                if (CHP - damageTaken <= 0) {
                    damageTaken = CHP - 1;
                }
                pmvc.DisplayCleanUpStatusEffects(animationsToPlay);
                if (damageTaken > 0) {
                    StartCoroutine(LoseHP(damageTaken));
                }
            }
            
            RemoveStatusEffects();
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

            CalculateStats();
            UpdateStatusEffectValues();
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

            CalculateStats();
            UpdateStatusEffectValues();
            pmvc.UpdateStats();
            pmvc.SetEquippedGear();
        }


        public bool EnableSkill(int index) {
            bool statChange = false;

            if (skills[index].type == SkillConstants.ACTIVE) {
                if (attackNum < maxAttacks) {
                    skillPoints--;
                    attacks[attackNum] = skills[index].a;
                    attackNum++;
                    skills[index].skillEnabled = true;
                    
                    if (className == "Mage") {
                        if (index == (int)SkillConstants.mageSkills.PYROMANCY == true) {  
                            if (attacks[attackNum].seName == StatusEffectConstants.BURN) {
                                attacks[attackNum].seChance = attacks[attackNum].baseSeChance << 1;
                            }
                        }
                    }

                    return true;
                }
            }
            else if (skills[index].type == SkillConstants.PASSIVE) {
                skillPoints--;
                skills[index].skillEnabled = true;

                if (className == "Mage") {
                    if (index == (int)SkillConstants.mageSkills.PYROMANCY) {  
                        for (int i = 0; i < attackNum; i++) {
                            if (attacks[i].seName == StatusEffectConstants.BURN) {
                                attacks[i].seChance = attacks[i].baseSeChance << 1;
                            }
                        }
                    }
                    else if (index == (int)SkillConstants.mageSkills.CRITICALMAGIC) {
                        statChange = true; 
                    }
                }
                else if (className == "Archer") {
                    if (index == (int)(SkillConstants.archerSkills.SCAVENGER)) {
                        PartyManager.instance.itemDropMultiplier *= 1.25f;
                    }
                    if (index == (int)(SkillConstants.archerSkills.SURVIVALIST)) {
                        statChange = true;
                    }
                }

                if (statChange == true) {
                    CalculateStats();
                    UpdateStatusEffectValues();
                    pmvc.UpdateStats();
                }

                return true;
            }

            return false;
        }

        public bool DisableSkill(int index) {
            bool statChange = false;
           
            if (skills[index].type == SkillConstants.ACTIVE) {
                int attackIndex = -1;
                if (attackNum > minAttacks) {
                    skillPoints++;
                    attackNum--;
                    skills[index].skillEnabled = false;
                    for (int i = 0; i <= attackNum; i++) {   // shift attacks back                    
                        if (attacks[i].nameKey == skills[index].a.nameKey) {
                            attackIndex = i;
                            break;
                        }
                    }
                    for (int i = attackIndex; i < attackNum; i++) {
                        attacks[i] = attacks[i + 1];
                    }
                    attacks[attackNum] = noneAttack;

                    return true;
                }
            }
            else if (skills[index].type == SkillConstants.PASSIVE) {
                skillPoints++;
                skills[index].skillEnabled = false;

                if (className == "Mage") {
                    if (index == (int)SkillConstants.mageSkills.PYROMANCY) {  
                        for (int i = 0; i < attackNum; i++) {
                            if (attacks[i].seName == StatusEffectConstants.BURN) {
                                attacks[i].seChance = attacks[i].baseSeChance;
                            }
                        }
                    }
                    if (index == (int)SkillConstants.mageSkills.CRITICALMAGIC) {
                        statChange = true;     
                    }
                }
                 else if (className == "Archer") {
                    if (index == (int)(SkillConstants.archerSkills.SCAVENGER)) {
                        PartyManager.instance.itemDropMultiplier *= 0.8f;
                    }
                    if (index == (int)(SkillConstants.archerSkills.SURVIVALIST)) {
                        statChange = true;
                    }
                }

                if (statChange == true) {
                    CalculateStats();
                    UpdateStatusEffectValues();
                    pmvc.UpdateStats();
                }

                return true;
            }

            return false;
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
