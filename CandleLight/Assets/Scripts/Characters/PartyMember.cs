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
        public int skillPoints { get; set; }        /// <value> Skillpoints currently avialable </value>
        public bool doneEXPGaining { get; private set; } = true;   /// <value> Total experience points to reach next level </value>
        
        public PartyMember summon = null;       /// <value> Summoned familiar of this partyMember </value>
        public PartyMember summoner = null;     /// <value> PartyMember that summoned this </value>
        public Attack noneAttack = new Attack("none", "physical", "0", null, 0, 0, "MP", "0", "single", "none");
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
            base.Init(LVL, stats, attacks);
            CalculateStats(true);
            this.EXP = EXP;
            this.EXPToNextLVL = CalculateEXPToNextLVL(LVL);
            this.className = personalInfo[0];
            this.subClassName = personalInfo[1];
            this.race = personalInfo[3];
            this.skills = skills;
            skillPoints = 1;

            pmvc.Init(this);
        }

        /// <summary>
        /// Initialize a partyMember using saved data
        /// </summary>
        /// <param name="pmData"></param>
        public void Init(PartyMemberData pmData) {
            base.Init(pmData.LVL, new int[] { pmData.baseSTR, pmData.baseDEX, pmData.baseINT, pmData.baseLUK }, pmData.attacks);
            this.CHP = pmData.CHP;
            this.CMP = pmData.CMP;
            this.EXP = pmData.EXP;
            this.EXPToNextLVL = CalculateEXPToNextLVL(LVL);
            this.className = pmData.className;
            this.subClassName = pmData.subClassName;
            this.race = pmData.race;
            this.skills = pmData.skills;
            foreach (Skill s in skills) {   // can't serialize colours, so need to create colour from float values saved in skills
                if (s != null) {
                    s.InitColour();
                }
            }
            this.skillPoints = pmData.skillPoints;

            pmvc.Init(this);
            
        }

        /// <summary>
        /// Equip a partyMember's gear and candles using saved data
        /// </summary>
        /// <param name="pmData"></param>
        public void EquipLoadedItems(PartyMemberData pmData) {
            if (pmData.equippedGear[0] != null) {
                EquipGear(new Gear(pmData.equippedGear[0]), "weapon");
            }
            if (pmData.equippedGear[1] != null) {
                EquipGear(new Gear(pmData.equippedGear[1]), "secondary");
            }
            if (pmData.equippedGear[2] != null) {
                EquipGear(new Gear(pmData.equippedGear[2]), "armour");
            }

            for (int i = 0; i < pmData.equippedCandles.Length; i++) {
                if (pmData.equippedCandles[i] != null) {
                    EquipCandle(new Candle(pmData.equippedCandles[i]), i);
                }
            }
        }

        /// <summary>
        /// Add all of the partyMember's saved status effects
        /// </summary>
        /// <param name="pmData"></param>
        public void AddLoadedStatusEffects(PartyMemberData pmData) {
            for (int i = 0; i < pmData.seDatas.Length; i++) {
                AddStatusEffect(pmData.seDatas[i]);
            }
        }

        /// <summary>
        /// Assigns a name to this partyMember (these are locked due to story)
        /// </summary>
        /// <param name="id"></param>
        public void GenerateName(int partyNum) {
            if (partyNum == 0) {
                this.pmName = "Holst";
            }
            else if (partyNum == 1) {
                this.pmName = "Flaytz";
            }
        }

        /// <summary>
        /// Sets EXPToNextLevel based off of a math
        /// </summary>
        /// <param name="level"> Level to calculate EXP to next level for </param>
        public int CalculateEXPToNextLVL(int LVL) {
            // it takes 4 LVL 1 enemies for a LVL 1 player to reach LVL 2
            // it takes 47 LVL 98 enemies for LVL 98 player to reach LVL 99
            return 2 + (int)(6 * Mathf.Pow(LVL, 2.35f) + LVL); 
        }

        /// <summary>
        /// Levels up a partyMember character; overriding base for different scaling for classes
        /// </summary>
        /// <param name="multiplier"> Multiplier because base needed it, won't be used here </param>
        public override void LVLUp() {
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

            float HPpercent =  (float)CHP / HP;
            float MPpercent = (float)CMP / MP;
            CalculateStats();

            CHP = (int) (HP * HPpercent);
            CMP = (int) (MP * MPpercent);

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

            EXPToNextLVL = CalculateEXPToNextLVL(LVL);
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

            /* primary stat changes from skills */
            if (className == ClassConstants.ARCHER) {
                if (skills[(int)SkillConstants.archerSkills.BLOOMINGFORTUNE].skillEnabled == true) {
                    LUK *= 2;
                }
                if (skills[(int)SkillConstants.archerSkills.FLEETFOOTED].skillEnabled == true) {
                    DEX += 5;
                }
            }

            /* secondary stats */
            HP = (int)(STR * 2.25 + DEX * 1.25);
            MP = (int)(INT * 1.5 + LUK * 0.75);
            PATK = (int)(STR * 0.5 + DEX * 0.25);
            MATK = (int)(INT * 0.5 + LUK * 0.25); 
            PDEF = (int)(STR * 0.125 + DEX * 0.065);
            MDEF = (int)(INT * 0.125 + LUK * 0.065);
            DOG = (int)(LUK * 0.2 + DEX * 0.1);
            ACC = (int)(DEX * 0.2 + STR * 0.1 + INT * 0.1) + defaultACC;
            if (LVL != 0) {
                critChance = (int)(LUK * 0.15) + baseCritChance;
            }
            else {
                critChance = 0; // tutorial prevents crits from the party so all tutorial blurbs can show up
            }
            critMult = baseCritMult;
            HPRegen = baseHPRegen;
            MPRegen = baseMPRegen;
            championChance = 0;
            burnPlus = false;
            poisonPlus = false;
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
                            case "BURNPLUS":
                                burnPlus = true;
                                break;
                            case "POISONPLUS":
                                poisonPlus = true;
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
                        case "BURNPLUS":
                            burnPlus = true;
                            break;
                        case "POISONPLUS":
                            poisonPlus = true;
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
                if (skills[(int)SkillConstants.warriorSkills.STEADFAST].skillEnabled == true) {
                    HP += (int)(HP * 0.15f);
                }
                if (skills[(int)SkillConstants.warriorSkills.BLOODSWORN].skillEnabled == true) {
                    PATK += (int)(PATK * 0.3f);
                }
            }
            else if (className == ClassConstants.MAGE) {
                if (skills[(int)SkillConstants.mageSkills.CRITICALMAGIC].skillEnabled == true) {
                     critChance += 10;
                }
                if (skills[(int)SkillConstants.mageSkills.THIRDEYE].skillEnabled == true) {
                    MPRegen *= 2f;
                }
                if (skills[(int)SkillConstants.mageSkills.MANASHIELD].skillEnabled == true) {
                    MDEF += 2;
                }
                if (skills[(int)SkillConstants.mageSkills.FIERYVEIL].skillEnabled == true) {
                    burnPlus = true;
                }
            }
            else if (className == ClassConstants.ARCHER) {
                if (skills[(int)SkillConstants.archerSkills.SURVIVALIST].skillEnabled == true) {
                    PDEF += 1;
                }
            }
            else if (className == ClassConstants.ROGUE) {
                if (skills[(int)SkillConstants.rogueSkills.CLOAKED].skillEnabled == true) {
                    //int DOGBoost = (int)(DOG * 0.15f); // or 15% (whichever is greater) will be implemented if the game gets dlc
                    DOG += 15; //>= DOGBoost ? 15 : DOGBoost;
                }
                if (skills[(int)SkillConstants.rogueSkills.DEADLY].skillEnabled == true) {
                    PATK += 5;
                }
                if (skills[(int)SkillConstants.rogueSkills.KILLERINSTINCT].skillEnabled == true) {
                    critMult += 0.5f;
                }
                if (skills[(int)SkillConstants.rogueSkills.RITUALDAGGERS].skillEnabled == true) {
                    MATK *= 2;
                }
            }

            /* secondary stat changes from status effects */
            foreach (StatusEffect se in statusEffects) {
                if (se.name == StatusEffectConstants.TAUNT || se.name == StatusEffectConstants.RAGE) {
                    PATK += (int)(PATK * 0.5);
                }
                else if (se.name == StatusEffectConstants.FREEZE) {
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
                else if (se.name == StatusEffectConstants.BARRIER) {
                    MDEF += MDEF;
                }
                else if (se.name == StatusEffectConstants.MARIONETTE) {
                    DOG -= DOG;
                }
                else if (se.name == StatusEffectConstants.SCUM) {
                    HPRegen *= 0.5f;
                    MPRegen *= 0.5f;
                }
            }
            
            if (setCurrent) {
                CHP = HP;
                CMP = MP;
            }

            if (HP <= 0) {
                HP = 1;
            }
            if (MP <= 0) {
                MP = 0;
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

        #region [ Section 0 ] Summoned Partymember Logic
        
        /// <summary>
        /// Initialize the partyMember, specifically for summon
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="summoner"></param>
        public void InitSummon(PartyMember pm, PartyMember summoner) {
            base.Init(pm.LVL, new int[] { pm.STR, pm.DEX, pm.INT, pm.LUK }, pm.attacks);
            this.EXP = 0;
            this.className = pm.className;
            this.subClassName = pm.subClassName;
            this.race = pm.race;
            this.skills = pm.skills;
            summoner.summon = this;
            this.summoner = summoner;
            GenerateSummonName(className);
            LVLUpSummon(summoner);
            pmvc.Init(this);
        }

        /// <summary>
        /// Assigns a name to this partyMember assuming its a summon (slightly random)
        /// </summary>
        /// <param name="id"></param>
        public void GenerateSummonName(string className) {
            if (className == "frostGolem") {
                string[] names = new string[] { "Frosty", "Chilly", "Krysthal", "Snowball", "Flurry", "Jack" }; // shout out to tristhal
                this.pmName = names[Random.Range(0, names.Length)];
            }
        }

        /// <summary>
        /// Adds the summoned familiar buff (doesn't do anything besides a visual indicator)
        /// </summary>
        public void GetSummonBuff() {
            StatusEffect newStatus = new StatusEffect(StatusEffectConstants.FAMILIAR, 999);
            AddStatusEffectPermanent(newStatus);
            pmvc.AddStatusEffectDisplay(newStatus);
        }

        /// <summary>
        /// LVL up the summon to the summoner's level and add a portion of their stats
        /// </summary>
        /// <param name="summoner"></param>
        public void LVLUpSummon(PartyMember summoner) {
            for (int i = 1; i < summoner.LVL; i++) {
                LVL += 1;
                baseSTR += (int)(LVL * 0.3 + baseSTR * 0.3);
                STR = baseSTR;
                baseDEX += (int)(LVL * 0.3 + baseDEX * 0.3);
                DEX = baseDEX;
                baseINT += (int)(LVL * 0.3 + baseINT * 0.3);
                INT = baseINT;
                baseLUK += (int)(LVL * 0.3 + baseLUK * 0.3);
                LUK = baseLUK;
            }      
            this.EXPToNextLVL = CalculateEXPToNextLVL(LVL);
            skillPoints = 0;

            // summons get part of the summoner's stats
            baseSTR += (int)(summoner.STR * 0.33f); 
            baseDEX += (int)(summoner.DEX * 0.33f);
            baseINT += (int)(summoner.INT * 0.33f);
            baseLUK += (int)(summoner.LUK * 0.33f);
            CalculateStats(true);
        }

        /// <summary>
        /// Handles animations for the summon's "summoning" (do a fade in if its the initial summon, otherwise just particle
        /// animation on restore)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="isRestore"></param>
        /// <returns></returns>
        public IEnumerator GetSummoned(Attack a, bool isRestore) {
            if (isRestore == true) {
                yield return StartCoroutine(pmvc.DisplaySummonRestored(a.animationClipName));
            }
            else {
                yield return StartCoroutine(pmvc.DisplaySummon(a.animationClipName));
            }
        }

        #endregion

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
                    EXPToNextLVL = CalculateEXPToNextLVL(LVL + 1);   // need this value to change the EXP display, but can't LVL up until after bar fills
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

            // side effects from partyMember skills may alter calculations
            bool skillAltered = false;
            if (className == ClassConstants.MAGE) {
                if (skills[(int)SkillConstants.mageSkills.MANASHIELD].skillEnabled == true) {
                    if (CMP > 0) {
                        skillAltered = true;
                        CHP -= (int)Mathf.Ceil(amount * 0.75f);
                        StartCoroutine(LoseMP((int)Mathf.Floor(amount * 0.25f)));  
                    }
                }
            }
            if (skillAltered == false) {
                 CHP -= amount;
            }

            if (CHP <= 0) {
                CHP = 0;
                PartyManager.instance.RegisterPartyMemberDead(this);
                if (summon != null) {
                    StartCoroutine(summon.LoseHPNoYield(summon.CHP));
                }
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
        /// <param name="c"> Character attacking this </param>
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
                // side effects from partyMember skills
                if (className == ClassConstants.WARRIOR) {
                    if (skills[(int)SkillConstants.warriorSkills.VAMPIRICMAIL].skillEnabled == true) {
                        if (Random.Range(0, 100) < 35) {
                            Monster cm = (Monster)c;    // TODO: If partyMember can ever be attacked by a partyMember, need to if this
                            cm.AddStatusEffect(StatusEffectConstants.BLEED, 2, this); 
                        }
                    }
                }
                else if (className == ClassConstants.MAGE) {
                    if (skills[(int)SkillConstants.mageSkills.FIERYVEIL].skillEnabled == true) {
                        int burnChance = 30;
                        if (skills[(int)SkillConstants.mageSkills.PYROMANCY].skillEnabled == true) {
                            burnChance *= 2;
                        }
                        if (Random.Range(0, 100) < burnChance) {
                            Monster cm = (Monster)c;    // TODO: If partyMember can ever be attacked by a partyMember, need to if this
                            cm.AddStatusEffect(StatusEffectConstants.BURN, 3, this); 
                        }
                    }
                }
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
                AddStatusEffect(a.seName, a.seDuration, c);

                // side effects from partyMember skills
                if (className == ClassConstants.WARRIOR) {
                    if (skills[(int)SkillConstants.warriorSkills.VAMPIRICMAIL].skillEnabled == true) {
                        if (Random.Range(0, 100) < 50) {
                            Monster cm = (Monster)c;    // TODO: If partyMember can ever be attacked by a partyMember, need to if this
                            cm.AddStatusEffect(StatusEffectConstants.BLEED, 2, this); 
                        }
                    }
                }
                else if (className == ClassConstants.MAGE) {
                    if (skills[(int)SkillConstants.mageSkills.FIERYVEIL].skillEnabled == true) {
                        int burnChance = 33;
                        if (skills[(int)SkillConstants.mageSkills.PYROMANCY].skillEnabled == true) {
                            burnChance *= 2;
                        }
                        if (Random.Range(0, 100) < burnChance) {
                            Monster cm = (Monster)c;    // TODO: If partyMember can ever be attacked by a partyMember, need to if this
                            cm.AddStatusEffect(StatusEffectConstants.BURN, 3, this); 
                        }
                    }
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
                }
            }
            else if (a.type == AttackConstants.BUFF || a.type == AttackConstants.BUFFSELF) {
                yield return StartCoroutine(pmvc.DisplayAttackHelped(a.animationClipName));
                AddStatusEffect(a.seName, a.seDuration, c);
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
            if (statusEffects.Count < maxStatusEffects) {
                int existingIndex = GetStatusEffect(seName);
                if (existingIndex != -1) {  // reapply the status effect if its already applied
                    seToRemove.Add(statusEffects[existingIndex]);
                    RemoveStatusEffectsNoCalculate();   // will be recalculating anyways on AddStatusEffect
                }

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

                UpdateStatusEffectValues();
            }
        }

        /// <summary>
        /// Adds a status effect to the partyMember from save data
        /// </summary>
        /// <param name="seData"> Data </param>
        public void AddStatusEffect(StatusEffectData seData ) {   
            StatusEffect newStatus = new StatusEffect(seData);
            newStatus.SetValue(null, this);
            AddStatusEffect(newStatus);
            pmvc.AddStatusEffectDisplay(newStatus);

            UpdateStatusEffectValues();
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
            int[] animationsToPlay = new int[] { 0 ,0, 0, 0, 0, 0 }; 
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
                else if (se.name == StatusEffectConstants.FROSTBITE) {
                    HPchange -= se.value;
                    animationsToPlay[5] = 1;
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
        /// <param name="index"> Unequips a candle from one of the active candle slots (0, 1, or 2) </param>
        public void UnequipCandle(int index) {
            if (className == ClassConstants.MAGE) {
                if (skills[(int)SkillConstants.mageSkills.CANDLEMANCY].skillEnabled == true) {
                    activeCandles[index].SetUses((int)(activeCandles[index].uses * 0.5f));
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
            if (activeCandles[index].a.type != AttackConstants.DEBUFF && CheckDeath() == false) {    // don't let the player waste charges of WeaknessC0 or similar candles meant only for combat
                StartCoroutine(GetHelped(activeCandles[index].a, this));
                activeCandles[index].Use(); 
            }
        }

        /// <summary>
        /// Restore uses to all active candles
        /// </summary>
        public void Rekindle() {
            foreach(Candle c in activeCandles) {
                if (c != null) {
                    if (className == ClassConstants.MAGE && skills[(int)SkillConstants.mageSkills.CANDLEMANCY].skillEnabled == true) {
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
            int replacedSkillIndex = 0;
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
                    else if (className == ClassConstants.ROGUE) {
                        if (skills[(int)SkillConstants.rogueSkills.RITUALDAGGERS].skillEnabled == true) {
                            attacks[attackNum].costFormula += " * 2";
                        }
                    }

                    attackNum++;
                    SetAttackValues();
                    pmvc.UpdateCombatActions();
                    return true;
                }
            }
            else if (skills[index].type == SkillConstants.PASSIVE) {
                skillPoints--;
                skills[index].skillEnabled = true;

                if (className == ClassConstants.WARRIOR) {
                    if (index == (int)SkillConstants.warriorSkills.STEADFAST) {
                        statChange = true;
                    }
                    else if (index == (int)SkillConstants.warriorSkills.BLOODSWORN) {
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
                    else if (index == (int)SkillConstants.mageSkills.MANASHIELD) {
                        statChange = true;
                    }
                }
                else if (className == ClassConstants.ARCHER) {
                    if (index == (int)(SkillConstants.archerSkills.SCAVENGER)) {
                        PartyManager.instance.itemDropMultiplier *= 1.5f;
                    }
                    else if (index == (int)(SkillConstants.archerSkills.SURVIVALIST)) {
                        statChange = true;
                    }
                    else if (index == (int)(SkillConstants.archerSkills.BLOOMINGFORTUNE)) {
                        statChange = true;
                    }
                    else if (index == (int)(SkillConstants.archerSkills.FLEETFOOTED)) {
                        statChange = true;
                    }
                }
                else if (className == ClassConstants.ROGUE) {
                    if (index == (int)SkillConstants.rogueSkills.WAXTHIEF) {
                        PartyManager.instance.WAXDropMultiplier *= 1.5f;
                        pmvc.UpdateWAXValues();
                    }
                    else if (index == (int)SkillConstants.rogueSkills.CLOAKED) {
                        statChange = true;
                    }
                    else if (index == (int)SkillConstants.rogueSkills.DEADLY) {
                        statChange = true;
                    }
                    else if (index == (int)SkillConstants.rogueSkills.KILLERINSTINCT) {
                        statChange = true;
                    }
                    else if (index == (int)SkillConstants.rogueSkills.RITUALDAGGERS) {
                        for (int i = 0; i < attackNum; i++) {
                            attacks[i].costFormula += " * 1.5";
                        }
                        statChange = true;
                    }
                }

                if (statChange == true) {
                    CalculateStats();
                    UpdateStatusEffectValues();
                    pmvc.UpdateHPAndMPBars();
                    pmvc.UpdateStats();
                }

                return true;
            }
            else if (skills[index].type == SkillConstants.UPGRADE) {
                if (skills[index].upgradeSkill == -2 || skills[index].upgradeSkill == -1) { // replacing the first or second attack
                    replacedSkillIndex = 2 + skills[index].upgradeSkill;
                    skillPoints--;
                    skills[index].storedAttack = attacks[2 + skills[index].upgradeSkill];   // store the original attack
                    attacks[replacedSkillIndex] = skills[index].a;              // replace the attack
                    skills[index].skillEnabled = true;

                    if (className == ClassConstants.WARRIOR) {
                        if (skills[(int)SkillConstants.warriorSkills.BLOODSWORN].skillEnabled == true) {
                            attacks[replacedSkillIndex].costType = "HP";
                            skills[index].storedAttack.costType = "MP";     // revert any skill changes to the original attack 
                        }
                    }
                    else if (className == ClassConstants.MAGE) {
                        if (skills[(int)SkillConstants.mageSkills.PYROMANCY].skillEnabled == true) {
                            if (attacks[replacedSkillIndex].seName == StatusEffectConstants.BURN) {
                                attacks[replacedSkillIndex].seChance = skills[replacedSkillIndex].storedAttack.baseSeChance << 1;
                                skills[index].storedAttack.seChance = skills[index].storedAttack.baseSeChance >> 1;     // revert any skill changes to the original attack
                            }
                        }
                    }
                    SetAttackValues();
                    pmvc.UpdateCombatActions();
                    return true;
                }
                else if (skills[skills[index].upgradeSkill].skillEnabled == true) { // TODO: Make logic to check if previous skill is enabled
                    return true;
                }
            }

            return false;
        }

        public bool DisableSkill(int index) {
            int replacedSkillIndex = 0;
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
                    else if (className == ClassConstants.ROGUE) {
                        if (skills[(int)SkillConstants.rogueSkills.RITUALDAGGERS].skillEnabled == true) {
                            attacks[attackIndex].costFormula =  attacks[attackIndex].costFormula.Remove(attacks[attackIndex].costFormula.Length - 4, 4);
                        }
                    }

                    attackNum--;
                    for (int i = attackIndex; i < attackNum; i++) { // shift attacks back               
                        attacks[i] = attacks[i + 1];
                    }
                    attacks[attackNum] = noneAttack;
                    SetAttackValues();
                    pmvc.UpdateCombatActions();
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
                    else if (index == (int)SkillConstants.warriorSkills.STEADFAST) {
                        statChange = true;
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
                    else if (index == (int)SkillConstants.mageSkills.MANASHIELD) {
                        statChange = true;
                    }
                }
                else if (className == ClassConstants.ARCHER) {
                    if (index == (int)(SkillConstants.archerSkills.SCAVENGER)) {
                        PartyManager.instance.itemDropMultiplier /= 1.5f;
                    }
                    else if (index == (int)(SkillConstants.archerSkills.SURVIVALIST)) {
                        statChange = true;
                    }
                    else if (index == (int)(SkillConstants.archerSkills.BLOOMINGFORTUNE)) {
                        statChange = true;
                    }
                    else if (index == (int)(SkillConstants.archerSkills.FLEETFOOTED)) {
                        statChange = true;
                    }
                }
                else if (className == ClassConstants.ROGUE) {
                    if (index == (int)SkillConstants.rogueSkills.WAXTHIEF) {
                        PartyManager.instance.WAXDropMultiplier /= 1.5f;
                        pmvc.UpdateWAXValues();
                    }
                    else if (index == (int)SkillConstants.rogueSkills.CLOAKED) {
                        statChange = true;
                    }
                    else if (index == (int)SkillConstants.rogueSkills.DEADLY) {
                        statChange = true;
                    }
                    else if (index == (int)SkillConstants.rogueSkills.KILLERINSTINCT) {
                        statChange = true;
                    }
                    else if (index == (int)SkillConstants.rogueSkills.RITUALDAGGERS) {  
                        statChange = true;   
                        for (int i = 0; i < attackNum; i++) { // all strings will be at least length 6 due to the appended characters
                            attacks[i].costFormula = attacks[i].costFormula.Remove(attacks[i].costFormula.Length - 6, 6);
                        }
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
            else if (skills[index].type == SkillConstants.UPGRADE) {
                if (skills[index].upgradeSkill == -2 || skills[index].upgradeSkill == -1) {  // replacing the first or second attack
                    skillPoints++;
                    replacedSkillIndex = 2 + skills[index].upgradeSkill;
                    attacks[replacedSkillIndex] = skills[index].storedAttack;
                    skills[index].storedAttack = null;   
                    skills[index].skillEnabled = false;

                    if (className == ClassConstants.WARRIOR) {
                        if (skills[(int)SkillConstants.warriorSkills.BLOODSWORN].skillEnabled == true) {
                            attacks[replacedSkillIndex].costType = "HP";
                        }
                    }
                    else if (className == ClassConstants.MAGE) {
                        if (skills[(int)SkillConstants.mageSkills.PYROMANCY].skillEnabled == true) {
                            if (attacks[replacedSkillIndex].seName == StatusEffectConstants.BURN) {
                                attacks[replacedSkillIndex].seChance = attacks[replacedSkillIndex].baseSeChance << 1;
                            }
                        }
                    }
                    SetAttackValues();
                    pmvc.UpdateCombatActions();
                    return true;
                }
                else if (skills[skills[index].upgradeSkill].skillEnabled == true) { // TODO: Make logic to check if previous skill is enabled
                    return true;
                }
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
        /// Returns true if the pmvc is no longer animating anything
        /// </summary>
        /// <returns></returns>
        public bool IsDoneAnimating() {
            return pmvc.isAnimating == false;
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
