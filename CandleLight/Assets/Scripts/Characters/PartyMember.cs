/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 11, 2019
* 
* The PartyMember class is used to store information about a member of the party. 
* It is always attached to a PartyMember GameObject.
*
*/

using AttackConstants = Constants.AttackConstants;
using ClassConstants = Constants.ClassConstants;
using Combat;
using Items;
using Party;
using Skills;
using SkillConstants = Constants.SkillConstants;
using System.Collections;
using StatusEffectConstants = Constants.StatusEffectConstants;
using UnityEngine;

namespace Characters {

    public class PartyMember : Character {
        
        /* external component references */
        public PartyMemberVisualController pmvc;    /// <value> Handles all visual components related to partyMember </value>

        public string className { get; set; }       /// <value> Warrior, Mage, Archer, or Rogue </value>
        public string subClassName { get; set; }    /// <value> Class specializations </value>
        public string pmName { get; set; }          /// <value> Name of the partyMember </value>
        public string race { get; set; }            /// <value> Human, Lizardman, Undead, etc. </value>
        public int EXP { get; set; }                /// <value> Current amount of experience points </value>
        public int EXPToNextLVL { get; set; }       /// <value> Total experience points to reach next level </value>
        public int skillPoints;
        public bool doneEXPGaining { get; private set; } = false;   /// <value> Total experience points to reach next level </value>

        public Attack noneAttack = new Attack("none", "physical", "0", "none", 0, 0, "MP", "0", "single", "none");
        public Skill[] skills = new Skill[12];
        public Gear weapon = null;        /// <value> Weapon </value>
        public Gear secondary = null;     /// <value> Secondary </value>
        public Gear armour = null;        /// <value> Armour </value>
        public Candle[] activeCandles = new Candle[3];
        
        private int numGear = 3;                /// <value> Max number of gear items a partyMember can have </value>
        private int numCandles = 3;             /// <value> Max number of active candles a partyMember can have </value>

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
            this.race = personalInfo[3];
            this.skills = skills;
            skillPoints = 1;

            pmvc.Init(this);
        }

        /// <summary>
        /// Assigns a random name to this partyMember
        /// </summary>
        /// <param name="id"></param>
        public void GenerateName(int partyNum) {
            if (partyNum == 0) {
                this.pmName = "Holst";
            }
            else if (partyNum == 1) {
                this.pmName = "Flatyz";
            }
        }

        /// <summary>
        /// Sets EXPToNextLevel based off of a math
        /// </summary>
        /// <param name="level"> Level to calculate EXP to next level for </param>
        public int CalcEXPToNextLVL(int LVL) {
            // it takes 4 LVL 1 enemies for a LVL 1 player to reach LVL 2
            // it takes 47 LVL 98 enemies for LVL 98 player to reach LVL 99
            return 2 + (int)(5 * Mathf.Pow(LVL, 2.21f) + LVL); 
        }

        /// <summary>
        /// Levels up a partyMember character; overriding base for different scaling for classes
        /// </summary>
        /// <param name="multiplier"> Multiplier because base needed it, won't be used here </param>
        public override void LVLUp(int multiplier = 1) {
            LVL++; 
            skillPoints++;
            pmvc.ExciteSkillsTab();

            if (className == ClassConstants.WARRIOR) {
                baseSTR += (int)(LVL * 1.5);
                baseDEX += (int)(LVL * 1.5);
                baseINT += (int)(LVL * 1.25);
                baseLUK += (int)(LVL * 1.25);
            }
            else if (className == ClassConstants.MAGE) {
                baseSTR += (int)(LVL * 0.75);
                baseDEX += (int)(LVL * 1.25);
                baseINT += (int)(LVL * 1.75);
                baseLUK += (int)(LVL * 1.5);
            }
            else if (className == ClassConstants.ARCHER) {
                baseSTR += (int)(LVL * 1.25);
                baseDEX += (int)(LVL * 1.75);
                baseINT += (int)(LVL * 1.5);
                baseLUK += LVL;
            }
            else if (className == ClassConstants.ROGUE) {
                baseSTR += LVL;
                baseDEX += (int)(LVL * 1.5);
                baseINT += (int)(LVL * 1.25);
                baseLUK += (int)(LVL * 2.25);
            }


            CalculateStats();

            pmvc.UpdateHPAndMPBars();
            pmvc.UpdateStats();
        }

        public void LVLDown() {
            if (className == ClassConstants.WARRIOR) {
                baseSTR -= (int)(LVL * 1.5);
                baseDEX -= (int)(LVL * 1.5);
                baseINT -= (int)(LVL * 1.25);
                baseLUK -= (int)(LVL * 1.25);
            }
            else if (className == ClassConstants.MAGE) {
                baseSTR -= (int)(LVL * 0.75);
                baseDEX -= (int)(LVL * 1.25);
                baseINT -= (int)(LVL * 1.75);
                baseLUK -= (int)(LVL * 1.5);
            }
            else if (className == ClassConstants.ARCHER) {
                baseSTR -= (int)(LVL * 1.25);
                baseDEX -= (int)(LVL * 1.75);
                baseINT -= (int)(LVL * 1.5);
                baseLUK -= LVL;
            }
            else if (className == ClassConstants.ROGUE) {
                baseSTR -= LVL;
                baseDEX -= (int)(LVL * 1.5);
                baseINT -= (int)(LVL * 1.25);
                baseLUK -= (int)(LVL * 2.25);
            }

            LVL--;
            skillPoints--;

            EXPToNextLVL = CalcEXPToNextLVL(LVL);
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

            for (int i = 0; i < numCandles; i++) {
                if (activeCandles[i] != null && activeCandles[i].isUsable == true) {
                    switch(activeCandles[i].effects[0]) {
                        case "STR":
                            STR += activeCandles[i].values[0];
                            break;
                        case "DEX":
                            DEX += activeCandles[i].values[0];
                            break;
                        case "INT":
                            INT += activeCandles[i].values[0];
                            break;
                        case "LUK":
                            LUK += activeCandles[i].values[0];
                            break;
                        default:
                            break;
                    }
                }
            }

            /* secondary stats */
            HP = (int)(STR * 2.25 + DEX * 1.25);
            MP = (int)(INT * 1.25 + LUK * 0.5);
            PATK = (int)(STR * 0.5 + DEX * 0.25);
            MATK = (int)(INT * 0.5 + LUK * 0.25); 
            PDEF = (int)(STR * 0.1 + DEX * 0.05);
            MDEF = (int)(INT * 0.15 + LUK * 0.05);
            DOG = (int)(DEX * 0.2 + LUK * 0.1);
            ACC = (int)(DEX * 0.2 + STR * 0.1 + INT * 0.1) + defaultACC;
            if (LVL != 0) {
                critChance = (int)(LUK * 0.1) + baseCritChance;
            }
            else {
                critChance = 0; // tutorial prevents crits from the party so all tutorial blurbs can show up
            }
            critMult = baseCritMult;
            HPRegen = baseHPRegen;
            MPRegen = baseMPRegen;
            championChance = 0;
            bleedPlus = false;

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
                            case "MPREGENDOUBLE":
                                MPRegen *= 2f;
                                break;
                            case "HPREGENDOUBLE":
                                HPRegen *= 2f;
                                break;
                            case "BLEEDPLUS":
                                bleedPlus = true;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            /* secondary stat changes from candles */
            for (int i = 0; i < numCandles; i++) {
                if (activeCandles[i] != null && activeCandles[i].isUsable == true) {
                    switch(activeCandles[i].effects[0]) {       // positive effect
                        case "HP":
                            HP += activeCandles[i].values[0];
                            break;
                        case "MP":
                            MP += activeCandles[i].values[0];
                            break;
                        case "PATK":
                            PATK += activeCandles[i].values[0];
                            break;
                        case "MATK":
                            MATK += activeCandles[i].values[0];
                            break;
                        case "PDEF":
                            PDEF += activeCandles[i].values[0];
                            break;
                        case "MDEF":
                            MDEF += activeCandles[i].values[0];
                            break;
                        case "DOG":
                            DOG += activeCandles[i].values[0];
                            break;
                        case "ACC":
                            ACC += activeCandles[i].values[0];
                            break;
                        case "CRITCHANCE":
                            critChance += activeCandles[i].values[0];
                            break;
                        case "CRITMULT":
                            critMult += activeCandles[i].values[0];
                            break;
                        case "MPREGENDOUBLE":
                            MPRegen *= 2f;
                            break;
                        case "HPREGENDOUBLE":
                            HPRegen *= 2f;
                            break;
                        case "BLEEDPLUS":
                            bleedPlus = true;
                            break;
                        default:
                            break;
                    }
                    switch(activeCandles[i].effects[2]) {       // negative effects
                        case "CHAMPIONCHANCE":
                            championChance += activeCandles[i].values[2];
                            break;
                        default:
                            break;
                    }
                }
            }

            /* secondary stat changes from skills */
            if (className == ClassConstants.WARRIOR) {
                if (skills[(int)SkillConstants.warriorSkills.BLOODSWORN].skillEnabled == true) {
                    PATK += (int)(PATK * 0.3);
                }
            }
            if (className == ClassConstants.MAGE) {
                if (skills[(int)SkillConstants.mageSkills.CRITICALMAGIC].skillEnabled == true) {
                     critChance += 10;
                }
                if (skills[(int)SkillConstants.mageSkills.THIRDEYE].skillEnabled == true) {
                    MPRegen *= 2f;
                }
            }
            else if (className == ClassConstants.ROGUE) {
                if (skills[(int)SkillConstants.rogueSkills.CLOAKED].skillEnabled == true) {
                    int DOGBoost = (int)(DOG * 0.15f);
                    DOG += 15 >= DOGBoost ? 15 : DOGBoost;
                }
                if (skills[(int)SkillConstants.rogueSkills.DEADLY].skillEnabled == true) {
                    PATK += 5;
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
                else if (se.name == StatusEffectConstants.GUARD) {
                    PDEF += PDEF;
                }
                else if (se.name == StatusEffectConstants.MIRACLE) {
                    PDEF += 9999;
                    MDEF += 9999;
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
        /// Set each attack's attack value, including canddle attacks
        /// </summary>
        public override void SetAttackValues() {
            base.SetAttackValues();
            foreach (Candle c in activeCandles) {
                if (c != null && c.a != null) {
                    c.a.attackValue = GetAttackValue(c.a);
                    c.a.costValue = GetCostValue(c.a);
                }
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
        /// <param name="amount"> Amount of health points to add </param>
        public void AddHP(int amount) {    
            CHP += amount;

            if (CHP > HP) {
                CHP = HP;
            }

            StartCoroutine(pmvc.DisplayHPChange(false));
        }

        /// <summary>
        /// Increase the PartyMember's current health points by a specified amount,
        /// yielding to the DisplayHPChange animation
        /// </summary>
        /// <param name="amount"> Amount of health points to add</param>
        /// <returns> Yields for animations </returns>
        public IEnumerator AddHPYield(int amount) {    
            CHP += amount;

            if (CHP > HP) {
                CHP = HP;
            }

            yield return StartCoroutine(pmvc.DisplayHPChange(false, true));
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

            StartCoroutine(pmvc.DisplayMPChange(false, false));
        }

        /// <summary>
        /// Increase the PartyMember's current mana points by a specified amount.
        /// TODO: Cleanup logic so that AddMP manages whether or not the player recieve MP under
        /// various circumstances (e.g. only revival skills can bring a partyMember back from 0 CMP)
        /// </summary> 
        /// <param name="amount"></param>
        public IEnumerator AddMPYield(int amount) {
            CMP += amount;

            if (CMP > MP) {
                CMP = MP;
            }

            yield return StartCoroutine(pmvc.DisplayMPChange(false, true));
        }

        /// <summary>
        /// Increase the partyMember's current mana and health points by 5% of their max amounts
        /// </summary>
        public void Regen() {
            AddHP((int)(Mathf.Ceil((float)HP * HPRegen)));
            AddMP((int)(Mathf.Ceil((float)MP * MPRegen)));
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

        public IEnumerator LoseHPNoYield(int amount) {
            amount = Mathf.Abs(amount);
            
            CHP -= amount;

            if (CHP <= 0) {
                CHP = 0;
                PartyManager.instance.RegisterPartyMemberDead(this);
            }

            yield return StartCoroutine(pmvc.DisplayHPChange(true, false, false));
        }

        /// <summary>
        /// Reduce the PartyMember's current health points by a specified amount.false
        /// IEnumerator is used to make calling function wait for its completion
        /// </summary> 
        /// <param name="amount"> Amount of health points lost </param>
        public IEnumerator LoseMP(int amount) {
            CMP -= amount;
            if (CMP <= 0) {
                CMP = 0;
            }
            
            yield return (StartCoroutine(pmvc.DisplayMPChange(true, false)));
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
                yield return StartCoroutine(LoseHPNoYield(cost));
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
                int damage = CalculateAttackDamage(a);
                bool isCrit = CalculateAttackCrit(c);
                bool isStatus = CalculateAttackStatus(a, c);
                if (isCrit) {
                    damage = CalculateAttackDamageCrit(damage, c);
                    damage = CalculateAttackReductions(damage, a);
                }
                else {
                    damage = CalculateAttackReductions(damage, a);
                }
                
                pmvc.SetAttackAmount(damage, isCrit);

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
        /// Handles all logic when a partyMember's attack helps a partyMember (healing, buffs, etc.)
        /// </summary>
        /// <param name="a"> Attack targeting this partyMember </param>
        /// <param name="c"> Character targeting this </param>
        /// <returns></returns>
        public IEnumerator GetHelped(Attack a, Character c) {
            if (a.type == AttackConstants.HEALHP || a.type == AttackConstants.HEALHPSELF) {
                int healed = CalculateAttackHeal(a);
                bool isCrit = CalculateAttackCrit(c);
                bool isStatus = CalculateAttackStatus(a, c);

                if (isCrit) {
                    healed = CalculateAttackHealCrit(healed, c);
                }

                pmvc.SetAttackAmount(healed, isCrit);
                yield return StartCoroutine(pmvc.DisplayAttackHelped(a.animationClipName));
                yield return StartCoroutine(AddHPYield(healed));

                if (isStatus && CheckDeath() == false) {
                    AddStatusEffect(a.seName, a.seDuration, c);
                    UpdateStatusEffectValues();
                }
            }
            else if (a.type == AttackConstants.HEALMP || a.type == AttackConstants.HEALMPSELF) {
                int focused = CalculateAttackFocus(a);
                bool isCrit = CalculateAttackCrit(c);
                bool isStatus = CalculateAttackStatus(a, c);

                if (isCrit) {
                    focused = CalculateAttackFocusCrit(focused, c);
                }

                pmvc.SetAttackAmount(focused, isCrit);
                yield return StartCoroutine(pmvc.DisplayAttackHelped(a.animationClipName));
                if (CombatManager.instance.inCombat == true) {
                    yield return StartCoroutine(AddMPYield(focused));
                }
                else {
                    AddMP(focused);
                }

                if (isStatus && CheckDeath() == false) {
                    AddStatusEffect(a.seName, a.seDuration, c);
                    UpdateStatusEffectValues();
                }
            }
            else if (a.type == AttackConstants.BUFF || a.type == AttackConstants.BUFFSELF) {
                yield return StartCoroutine(pmvc.DisplayAttackHelped(a.animationClipName));
                AddStatusEffect(a.seName, a.seDuration, c);
                UpdateStatusEffectValues();
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
                StatusEffect newStatus;
                if (c != null && c.ID == this.ID && CombatManager.instance.inCombat == true) {
                    newStatus = new StatusEffect(seName, seDuration + 1);   // status effects proc the same turn they show up, so to keep the duration equal between all characters, add 1 if selfinduced
                }
                else {
                    newStatus = new StatusEffect(seName, seDuration);
                }
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
            int HPchange = 0;   // negative means HP gained
            int MPchange = 0;   // negative means MP gained
            int[] animationsToPlay = new int[] { 0 ,0, 0, 0, 0 }; 
            bool isCure = GetStatusEffect(StatusEffectConstants.CURE) != -1;

            foreach (StatusEffect se in statusEffects) {
                if (se.name == StatusEffectConstants.BURN) {
                    HPchange += se.value;
                    animationsToPlay[0] = 1;
                }
                else if (se.name == StatusEffectConstants.POISON) {
                    HPchange += se.value;
                    animationsToPlay[1] = 1;
                }
                else if (se.name == StatusEffectConstants.BLEED) {
                    int bleedDamage = se.value;
                    HPchange += bleedDamage;
                    if (se.afflicter != null && se.afflicter.CheckDeath() == false) {
                        ((Monster)(se.afflicter)).AddHP(bleedDamage);
                    }
                    animationsToPlay[2] = 1;
                }
                else if (se.name == StatusEffectConstants.REGENERATE) {
                    HPchange -= se.value;
                    animationsToPlay[3] = 1;
                }
                else if (se.name == StatusEffectConstants.FOCUS) {
                    MPchange -= se.value;
                    animationsToPlay[4] = 1;
                }
                else if (se.name == StatusEffectConstants.FATALWOUND) {
                    HPchange += se.value;
                    animationsToPlay[2] = 1;
                }

                if (isCure && se.isBuff == false) {
                    se.UpdateDuration(-2);
                }
                else {
                    se.UpdateDuration(-1);
                }
                
                if (se.duration == 0) {
                    seToRemove.Add(se);
                }
            }

            if (inCombat == true) { // if in combat, always yield to status effect animations
                pmvc.DisplayCleanUpStatusEffects(animationsToPlay);
                if (HPchange > 0) {
                    pmvc.SetAttackAmount(HPchange, false);
                    yield return StartCoroutine(LoseHP(HPchange));
                }
                else if (HPchange < 0) {
                    pmvc.SetAttackAmount(HPchange, false);
                    yield return StartCoroutine(AddHPYield(HPchange * -1));
                }
            }
            else {
                if (CHP - HPchange <= 0) {
                    HPchange = CHP - 1;
                }
                pmvc.DisplayCleanUpStatusEffects(animationsToPlay);
                if (HPchange > 0) {
                    StartCoroutine(LoseHP(HPchange));
                }
                else if (HPchange < 0) {
                    AddHP(HPchange * -1);
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

        /// <summary>
        /// Equip a candle, changing stats
        /// </summary>
        /// <param name="c"></param>
        /// <param name="index"> Equips a candle to one of the active candle slots (0, 1, or 2) </param>
        public void EquipCandle(Candle c, int index) {
            activeCandles[index] = c;
            if (className == ClassConstants.MAGE) {
                if (skills[(int)SkillConstants.mageSkills.CANDLEMANCY].skillEnabled == true) {
                    c.SetUses(c.uses * 2);
                }
            }
            SetAttackValues();          // candle attack values need to be set

            if (c.isUsable == true) {   // no need to recalculate stats if the equipped candle is unusable
                CalculateStats();
                UpdateStatusEffectValues();
            }
            pmvc.UpdateStats();
            pmvc.SetEquippedCandles();
        }

        /// <summary>
        /// Unequips a candle, changing stats
        /// </summary>
        /// <param name="index"> Equips a candle to one of the active candle slots (0, 1, or 2) </param>
        public void UnequipCandle(int index) {
            if (className == ClassConstants.MAGE) {
                if (skills[(int)SkillConstants.mageSkills.CANDLEMANCY].skillEnabled == true) {
                    activeCandles[index].SetUses(0);
                }
            }
            activeCandles[index] = null;

            CalculateStats();
            UpdateStatusEffectValues();
            pmvc.UpdateStats();
            pmvc.SetEquippedCandles();
        }

        /// <summary>
        /// Use a candle's attack 
        /// </summary>
        /// <param name="index"> Equips a candle to one of the active candle slots (0, 1, or 2) </param>
        public void UseCandle(int index) {
            StartCoroutine(GetHelped(activeCandles[index].a, this));
            activeCandles[index].Use(); 
        }

        /// <summary>
        /// Restore uses to all active candles
        /// </summary>
        public void Rekindle() {
            foreach(Candle c in activeCandles) {
                if (c != null) {
                    if (skills[(int)SkillConstants.mageSkills.CANDLEMANCY].skillEnabled == true) {
                        c.CandlemancyRekindle();
                    }
                    else {
                        c.Rekindle();
                    }
                }
            }
        }

        /// <summary>
        /// Enables a skill at a given index, changing stats
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool EnableSkill(int index) {
            bool statChange = false;

            if (skills[index].type == SkillConstants.ACTIVE) {
                if (attackNum < maxAttacks) {
                    skillPoints--;
                    attacks[attackNum] = skills[index].a;
                    skills[index].skillEnabled = true;

                    if (className == ClassConstants.WARRIOR) {
                        if (skills[(int)SkillConstants.warriorSkills.BLOODSWORN].skillEnabled == true) {
                            attacks[attackNum].costType = "HP";
                        }
                    }
                    else if (className == ClassConstants.MAGE) {
                        if (skills[(int)SkillConstants.mageSkills.PYROMANCY].skillEnabled == true) {
                            if (attacks[attackNum].seName == StatusEffectConstants.BURN) {
                                attacks[attackNum].seChance = attacks[attackNum].baseSeChance << 1;
                            }
                        }
                    }

                    attackNum++;
                    
                    return true;
                }
            }
            else if (skills[index].type == SkillConstants.PASSIVE) {
                skillPoints--;
                skills[index].skillEnabled = true;

                if (className == ClassConstants.WARRIOR) {
                    if (index == (int)SkillConstants.warriorSkills.BLOODSWORN) {
                        for (int i = 0; i < attackNum; i++) {
                            attacks[i].costType = "HP";
                        }
                        statChange = true;
                    }
                }
                else if (className == ClassConstants.MAGE) {
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
                    else if (index == (int)SkillConstants.mageSkills.CANDLEMANCY) {
                        foreach (Candle c in activeCandles) {
                            if (c != null) {
                                c.SetUses(c.uses * 2);
                            }
                        }
                    }
                }
                else if (className == ClassConstants.ARCHER) {
                    if (index == (int)(SkillConstants.archerSkills.SCAVENGER)) {
                        PartyManager.instance.itemDropMultiplier *= 1.5f;
                    }
                    if (index == (int)(SkillConstants.archerSkills.SURVIVALIST)) {
                        statChange = true;
                    }
                }
                else if (className == ClassConstants.ROGUE) {
                    if (index == (int)SkillConstants.rogueSkills.WAXTHIEF) {
                        PartyManager.instance.WAXDropMultiplier *= 1.5f;
                    }
                    if (index == (int)SkillConstants.rogueSkills.CLOAKED) {
                        statChange = true;
                    }
                    if (index == (int)SkillConstants.rogueSkills.DEADLY) {
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
                    skills[index].skillEnabled = false;
                    for (int i = 0; i < attackNum; i++) {   // find the attack that is going to be removed 
                        if (attacks[i].nameKey == skills[index].a.nameKey) {
                            attackIndex = i;
                            break;
                        }
                    }

                    // revert changes to an active skill caused by a passive skill
                    if (className == ClassConstants.WARRIOR) {
                        if (skills[(int)SkillConstants.warriorSkills.BLOODSWORN].skillEnabled == true) {
                            attacks[attackIndex].costType = "MP";
                        }
                    }
                    else if (className == ClassConstants.MAGE) {
                        if (skills[(int)SkillConstants.mageSkills.PYROMANCY].skillEnabled == true) {       
                            if (attacks[attackIndex].seName == StatusEffectConstants.BURN) {
                                attacks[attackIndex].seChance = attacks[attackIndex].baseSeChance;
                            }   
                        }
                    }

                    attackNum--;
                    for (int i = attackIndex; i < attackNum; i++) { // shift attacks back               
                        attacks[i] = attacks[i + 1];
                    }
                    attacks[attackNum] = noneAttack;

                    return true;
                }
            }
            else if (skills[index].type == SkillConstants.PASSIVE) {
                skillPoints++;
                skills[index].skillEnabled = false;

                if (className == ClassConstants.WARRIOR) {
                    if (index == (int)SkillConstants.warriorSkills.BLOODSWORN) {
                        for (int i = 0; i < attackNum; i++) {
                            attacks[i].costType = "MP";
                        }
                    }
                }
                if (className == ClassConstants.MAGE) {
                    if (index == (int)SkillConstants.mageSkills.PYROMANCY) {  
                        for (int i = 0; i < attackNum; i++) {
                            if (attacks[i].seName == StatusEffectConstants.BURN) {
                                attacks[i].seChance = attacks[i].baseSeChance;
                            }
                        }
                    }
                    else if (index == (int)SkillConstants.mageSkills.CRITICALMAGIC) {
                        statChange = true;     
                    }
                    else if (index == (int)SkillConstants.mageSkills.CANDLEMANCY) {
                        foreach (Candle c in activeCandles) {
                            if (c != null) {
                                c.SetUses((int)(c.uses * 0.5));     // rounds down by truncating
                            }
                        }
                    }
                }
                else if (className == ClassConstants.ARCHER) {
                    if (index == (int)(SkillConstants.archerSkills.SCAVENGER)) {
                        PartyManager.instance.itemDropMultiplier /= 1.5f;
                    }
                    else if (index == (int)(SkillConstants.archerSkills.SURVIVALIST)) {
                        statChange = true;
                    }
                }
                else if (className == ClassConstants.ROGUE) {
                    if (index == (int)SkillConstants.rogueSkills.WAXTHIEF) {
                        PartyManager.instance.WAXDropMultiplier /= 1.5f;
                    }
                    else if (index == (int)SkillConstants.rogueSkills.CLOAKED) {
                        statChange = true;
                    }
                    else if (index == (int)SkillConstants.rogueSkills.DEADLY) {
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
        /// Trigger a skill on a partyMember, applying its effects.
        /// This function is only called if some condition that isn't as simple as enabling the skill occurs
        /// </summary>
        /// <param name="className"> PartyMember class </param>
        /// <param name="index"> Index of skill </param>
        public void TriggerSkill(string className, int index, Character c) {
            if (className == ClassConstants.ROGUE) {
                if (index == (int)SkillConstants.rogueSkills.AMBUSHER) {
                    AddStatusEffect(StatusEffectConstants.ADVANTAGE, 1, c);
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
