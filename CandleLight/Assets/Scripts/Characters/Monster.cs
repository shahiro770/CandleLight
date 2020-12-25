/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 11, 2019
* 
* The Monster class is used to store and manipulate information about the Monster. 
* It is always attached to a Monster gameObject.
*
*/

using achievementConstants = Constants.AchievementConstants.achievementConstants;
using AttackConstants = Constants.AttackConstants;
using ClassConstants = Constants.ClassConstants;
using Constants;
using Combat;
using EventManager = Events.EventManager;
using GameManager = General.GameManager;
using Items;
using PartyManager = Party.PartyManager;
using Result = Events.Result;
using ResultConstants = Constants.ResultConstants;
using SkillConstants = Constants.SkillConstants;
using Enum = System.Enum;
using System.Collections;
using UnityEngine;

namespace Characters {

    public class Monster : Character {
        
        /* external component references */
        public MonsterDisplay md;

        [field: SerializeField] public Result monsterReward { get; private set; }       /// <value> Result monster gives on death </value>
        [field: SerializeField] public Gear[] monsterGear { get; set; }                 /// <value> If monster drops gear, preload the gear into an array for fast access  </value>
        [field: SerializeField] public string monsterArea { get; private set; }         /// <value> Area where monster can be found </value>
        [field: SerializeField] public string monsterSize { get; private set; }         /// <value> String constant describing size of monster's sprite </value>
        [field: SerializeField] public string monsterNameID { get; private set; }       /// <value> NameID as referenced in database </value>
        [field: SerializeField] public string monsterSpriteName { get; private set; }   /// <value> Name of monster's sprite as referenced in resources </value>
        [field: SerializeField] public string monsterAI { get; private set; }           /// <value> Monster's behaviour in combat </value>
        [field: SerializeField] public StatusEffectConstant permanentBuff { get; private set; }       /// <value> Name of buff monster will always spawn with </value>
        [field: SerializeField] public float multiplier { get; private set; }           /// <value> Multipler to EXP and WAX rewarded (due to being a boss, variant, etc) </value>
        [field: SerializeField] public float difficultyModifier { get; private set; } = 1f;  /// <value> Stat modifier applied based on difficulty </value>
        [field: SerializeField] public int minLVL { get; private set; }                 /// <value> Minimum power level monster can spawn at </param>
        [field: SerializeField] public int maxLVL { get; private set; }                 /// <value> Maximum power level monster can spawn at </param>
        [field: SerializeField] public int bonusPDEF { get; private set; }              /// <value> Bonus PDEF added to the monster's stats </value>
        [field: SerializeField] public int bonusMDEF { get; private set; }              /// <value> Bonus MDEF added to the monster's stats </value>
        [field: SerializeField] public int EXP { get; private set; }                    /// <value> EXP monster gives on defeat </value>
        [field: SerializeField] public int WAX { get; private set; }                    /// <value> WAX monster gives on defeat </value>
        [field: SerializeField] public int dropChance { get; private set; }             /// <value> Chance of monster giving a result </value>
        [field: SerializeField] public int lastAttackIndex { get; set; } = -1;          /// <value> Index of attack last used (-1 if it hasn't attacked yet) </value>
        [field: SerializeField] public int hardModeAttackIndex { get; set; } = -1;      /// <value> Index of attack that can only be used in hard mode </value>
        [field: SerializeField] public bool isChampion { get; private set; }            /// <value> Flag for when monsterDisplay is done setting properties </value>
        [field: SerializeField] public bool isReady { get; private set; }               /// <value> Flag for when monsterDisplay is done setting properties </value>

        #region [ Initialization ] Initialization

        /// <summary>
        /// Initializes the monster's properties and display
        /// </summary>
        /// <param name="monsterNameID"> Name of monster as referenced by the database </param>
        /// <param name="monsterSpriteName"> Name of monster's sprite, castle case </param>
        /// <param name="monsterArea"> Area of monster to get file path to sprite, castle case </param>
        /// <param name="monsterSize"> Size of monster (small, medium, large) </param>
        /// <param name="monsterAI"> Pattern for how monster attacks </param>
        /// <param name="minLVL"> Minimum power level monster can spawn at </param>
        /// <param name="maxLVL"> Maximum power level monster can spawn at </param>
        /// <param name="multiplier"> Multiplier on rewards monster gives such as WAX and EXP </param>
        /// <param name="HP"> Max health points </param>
        /// <param name="MP"> Max mana points </param>
        /// <param name="stats"> STR, DEX, INT, LUK </param>
        /// <param name="attacks"> List of known attacks (length 4) </param>
        /// <param name="dropChance"> Chance of monster dropping something </param>
        /// <param name="monsterReward"> Result from monster dying </param>
        public IEnumerator Init(string monsterNameID, string monsterSpriteName, string monsterArea, 
        string monsterSize, string monsterAI, int multiplier, int HP, int MP, int[] stats, int bonusPDEF, int bonusMDEF, Attack[] attacks,
        int hardModeAttackIndex, int dropChance, Result monsterReward, int championChance, StatusEffectConstant permanentBuff) {
            this.monsterNameID = monsterNameID;
            this.monsterSpriteName = monsterSpriteName;
            this.monsterArea = monsterArea;
            this.monsterAI = monsterAI;
            this.bonusPDEF = bonusPDEF;
            this.bonusMDEF = bonusMDEF;
            this.monsterReward = monsterReward;
            if (monsterReward.type == ResultConstants.ITEM) {   // if the monster is going to drop gear, store it inside the monster
                if (monsterReward.itemType == ItemConstants.GEAR) {
                    int gearNum = 0;
                    for (int i = 0; i < monsterReward.specificItemNames.Length; i++) {
                        if (monsterReward.specificItemNames[i] != "none") {
                            gearNum++;
                        }
                    }
                    monsterGear = new Gear[gearNum];      
                    for (int i = 0; i < gearNum; i++) {
                        monsterGear[i] = (Gear)(GameManager.instance.DB.GetItemByNameID(monsterReward.specificItemNames[i], "Gear"));
                    }
                }
            }
            this.championChance = championChance;
            this.permanentBuff = permanentBuff;

            string[] LVLString = monsterNameID.Split(' ');
            this.minLVL = int.Parse(LVLString[1]);  // efficiency won't matter for numbers less than 1000
            this.maxLVL = int.Parse(LVLString[2]);
            base.Init(minLVL, stats, attacks);  // use minLVL for initialization, will use for scaling up on spawning
            CalculateStats(true);

            this.hardModeAttackIndex = hardModeAttackIndex;
            this.dropChance = dropChance;
            this.multiplier = multiplier;
            this.monsterSize = monsterSize;

            // max number of status effects will vary depending on a monster's size
             if (monsterSize == "small" || monsterSize == "extraSmall") {
                maxStatusEffects = 5;
            } 
            else if (monsterSize == "medium") {
                maxStatusEffects = 6;
            }
             else if (monsterSize == "large") {
                maxStatusEffects = 10;
            }

            StartCoroutine(md.Init(this));

            this.isReady = true;
            yield break;
        }

         /// <summary>
        /// Calculates secondary stats based off of the 4 primary stats
        /// </summary>
        /// <param name="setCurrent"> Flag for if CHP and CMP should equal new HP and MP values </param>
        protected override void CalculateStats(bool setCurrent = false) {
            HP = (int)((STR * 2.25 + DEX * 1.5) * difficultyModifier);
            MP = (int)((INT * 1.25 + LUK * 0.5) * difficultyModifier);
            PATK = (int)((STR * 0.65 + DEX * 0.35) * difficultyModifier);  // monsters have better primary scaling on PATK and MATK than players
            MATK = (int)((INT * 0.65 + LUK * 0.35) * difficultyModifier);
            PDEF = (int)(((STR * 0.15 + DEX * 0.075) + bonusPDEF) * difficultyModifier);
            MDEF = (int)(((INT * 0.15 + LUK * 0.075) + bonusMDEF) * difficultyModifier);
            DOG = (int)((DEX * 0.15 + LUK * 0.25) * difficultyModifier);
            ACC = (int)(DEX * 0.2 + STR * 0.1 + INT * 0.1) + defaultACC;
            critChance = (int)(LUK * 0.15) + baseCritChance;
            critMult = baseCritMult;
            burnPlus = false;
            poisonPlus = false;
            bleedPlus = false;

            foreach (StatusEffect se in statusEffects) {
                if (se.name == StatusEffectConstant.TAUNT || se.name == StatusEffectConstant.RAGE) {
                    PATK += (int)(PATK * 0.5);
                }
                else if (se.name == StatusEffectConstant.FREEZE) {
                    ACC -= (int)(ACC * 0.35);
                    PDEF -= (int)(PDEF * 0.35);
                }
                else if (se.name == StatusEffectConstant.WEAKNESS) {
                    PATK -= (int)(PATK * 0.3);
                }
                else if (se.name == StatusEffectConstant.ADVANTAGE) {
                    critChance += 50;
                }
                else if (se.name == StatusEffectConstant.ROOT) {
                    DOG -= (int)(DOG * 0.5);
                }
                else if (se.name == StatusEffectConstant.GUARD) {
                    PDEF += Mathf.Max(1, PDEF);
                }
                else if (se.name == StatusEffectConstant.VAMPIRE) {
                    bleedPlus = true;
                }
                else if (se.name == StatusEffectConstant.BARRIER) {
                    MDEF += Mathf.Max(1, MDEF);
                }
                else if (se.name == StatusEffectConstant.MARIONETTE) {
                    DOG -= DOG;
                }
                else if (se.name == StatusEffectConstant.NIMBLE) {
                    DOG += DOG;
                }
                else if (se.name == StatusEffectConstant.ETHEREAL) {
                    MATK += (int)(MATK * 0.5);
                }
                else if (se.name == StatusEffectConstant.BREAK) {
                    PDEF -= Mathf.Max(2, (int)(PDEF * 0.5));
                }
                else if (se.name == StatusEffectConstant.CHAMPIONHP) {
                    HP += (int)(HP * 0.66);
                }
                else if (se.name == StatusEffectConstant.CHAMPIONPATK) {
                    PATK += (int)(1 + PATK * 0.5);
                    HP += (int)(HP * 0.33);
                }
                else if (se.name == StatusEffectConstant.CHAMPIONMATK) {
                    MATK += (int)(1 + MATK * 0.5);
                    HP += (int)(HP * 0.33);
                }
                else if (se.name == StatusEffectConstant.CHAMPIONPDEF) {
                    PDEF += (int)(1 + PDEF * 0.5);
                    HP += (int)(HP * 0.33);
                }
                else if (se.name == StatusEffectConstant.CHAMPIONMDEF) {
                    MDEF += (int)(1 + MDEF * 0.5);
                    HP += (int)(HP * 0.33);
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
        /// Sets the monster's level between its minimum level and maximum level
        /// </summary>
        public void MultipleLVLUp(int subAreaProgress) {
            if (subAreaProgress < 50) { // prevent stronger monsters from appearing at the start of the area (note this affects boss fights)
                base.MultipleLVLUp(minLVL, Mathf.Max(minLVL, maxLVL - 1));  
            }
            else {
                base.MultipleLVLUp(Mathf.Min(minLVL + 1, maxLVL), maxLVL);  
            }
            // it takes 5 LVL 1 monster for a LVL 1 player to reach LVL 2
            // it takes 47 LVL 98 monsters for LVL 98 player to reach LVL 99
            this.EXP = (int)((Mathf.Pow(LVL, 1.65f) + ((STR + DEX + INT + LUK) / 10)) * this.multiplier * (1 / difficultyModifier));  
            this.WAX = (int)(Mathf.Pow(LVL, 1.65f) * this.multiplier * (1 / difficultyModifier)) + 1;  

            md.SetTooltip();
            md.SetHealthBar();
        }

        /// <summary>
        /// Apply all buffs monster may get on spawn
        /// </summary>
        /// <param name="championBuffs"></param>
        public void GetBuffs(string[] championBuffs) {
            GetPermanentBuff();
            GetBossBuff();
            if (difficultyModifier >= 1f) {
                GetChampionBuff(championBuffs);
            }
            if (GameManager.instance.gsData.aromas[(int)AromaConstants.aromaConstants.VICIOUSVINEYARD] == true) {
                GetRandomBuff();
            }
            md.UpdateTooltip();
            md.SetHealthBar();
        }
        
        /// <summary>
        /// If monster always spawns with a given buff, apply it
        /// </summary>
        public void GetPermanentBuff() {
            if (permanentBuff != StatusEffectConstant.NONE) {
                StatusEffect newStatus = new StatusEffect(permanentBuff, 999);
                AddStatusEffectPermanent(newStatus);
                md.AddStatusEffectDisplay(newStatus);
            }
        }
        
        /// <summary>
        /// Give boss buff if monster's multiplier is high enough 
        /// </summary>
        public void GetBossBuff() {
            if (multiplier == 4) {      // boss monsters will always have an EXP multiplier of 4
                StatusEffect newStatus = new StatusEffect(StatusEffectConstant.BOSS, 999);
                AddStatusEffectPermanent(newStatus);
                md.AddStatusEffectDisplay(newStatus);

                if (PartyManager.instance.GetHighestPartyMemberLVL() >= LVL) {    // boss levels up if the player is higher LVL than them
                    LVLUp();
                    if (difficultyModifier == 1f) {
                        championChance = 100;
                    }
                    else {
                        championChance = 0;
                        CalculateStats(true);
                    }
                }
            }
        }

        /// <summary>
        /// Applies a champion buff to a monster at random
        /// TODO: Make AddStatusEffectPermanent similar to AddStatusEffect
        /// </summary>
        /// <param name="championBuffs"> List of championBuffs that can be applied in the subArea (assumed length 3) </param>
        public void GetChampionBuff(string[] championBuffs) {
            if (multiplier == 4) {      // boss monsters can only be champion if GetBossBuff() allows it (otherwise too op)
                isChampion = Random.Range(0, 100) < championChance;
            }
            else {
                isChampion = Random.Range(0, 100) < (championChance + PartyManager.instance.bonusChampionChance);
            }

            if (isChampion == true) {
                multiplier += 0.5f;
                this.EXP = (int)((Mathf.Pow(LVL, 1.65f) + ((STR + DEX + INT + LUK) / 10)) * this.multiplier * (1 / difficultyModifier));  
                this.WAX = (int)(Mathf.Pow(LVL + 1, 1.65f) * this.multiplier * (1 / difficultyModifier)); 
                monsterReward.UpgradeResult();
                dropChance = 100;

                StatusEffectConstant championBuff = (StatusEffectConstant)Enum.Parse(typeof(StatusEffectConstant), championBuffs[Random.Range(0, championBuffs.Length)]);
                StatusEffect newStatus;
                switch (championBuff) {
                    case StatusEffectConstant.CHAMPIONATK:
                        {
                            if (PATK >= MATK) {
                                newStatus = new StatusEffect(StatusEffectConstant.CHAMPIONPATK, 999);
                            }
                            else {
                                newStatus = new StatusEffect(StatusEffectConstant.CHAMPIONMATK, 999);
                            }
                            AddStatusEffectPermanent(newStatus);
                            md.AddStatusEffectDisplay(newStatus);
                            break;
                        }     
                    case StatusEffectConstant.CHAMPIONDEF:
                        {
                            if (PDEF >= MDEF) {
                                newStatus = new StatusEffect(StatusEffectConstant.CHAMPIONPDEF, 999);
                            }
                            else {
                                newStatus = new StatusEffect(StatusEffectConstant.CHAMPIONMDEF, 999);
                            }
                            AddStatusEffectPermanent(newStatus);
                            md.AddStatusEffectDisplay(newStatus);
                            break;
                        }
                    case StatusEffectConstant.CHAMPIONHP:
                        {
                            newStatus = new StatusEffect(StatusEffectConstant.CHAMPIONHP, 999);
                            newStatus.SetValue(this, this);
                            AddStatusEffectPermanent(newStatus);
                            md.AddStatusEffectDisplay(newStatus);
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// Apply a random buff to the monster.
        /// If champion, monster gets two buffs
        /// </summary>
        public void GetRandomBuff() {
            int buffNum = isChampion == false ? 1 : 2;

            for (int i = 0; i < buffNum; i++) {
                int buffIndex = Random.Range(0, 100);
                bool isCommon = Random.Range(0, 100) < 75;
                
                if (isCommon) {
                    if (buffIndex < 20) {
                        AddStatusEffect(StatusEffectConstant.GUARD, 2, this);
                    }
                    else if (buffIndex < 40) {
                        AddStatusEffect(StatusEffectConstant.BARRIER, 2, this);
                    }
                    else if (buffIndex < 60) {
                        AddStatusEffect(StatusEffectConstant.REGENERATE, 2, this);
                    }
                    else if (buffIndex < 80) {
                        AddStatusEffect(StatusEffectConstant.CURE, 4, this);
                    }
                    else { // if (buffIndex < 100) 
                        AddStatusEffect(StatusEffectConstant.NIMBLE, 2, this);
                    }
                }
                else {  // rare buff pool
                    if (buffIndex < 50) {
                        if (PATK >= MATK) {
                            AddStatusEffect(StatusEffectConstant.RAGE, 2, this);
                        }
                        else {
                            AddStatusEffect(StatusEffectConstant.ETHEREAL, 2, this);
                        }
                    }
                    else {
                        AddStatusEffect(StatusEffectConstant.MIRACLE, 0, this);    // buffs by default get +1 to their duration
                    }
                }
            }
        }
        

        /// <summary>
        /// Applies changes to a monster based on the game's difficulty
        /// </summary>
        public void ApplyDifficultyChanges() {
            if (GameManager.instance.gsData.difficultyModifier == 0.75f) {
                if (monsterNameID != "TutorialGreyhide 1 1") {  // tutorial greyhide needs 10 hp for tutorial to complete properly
                    difficultyModifier = 0.75f;
                    if (hardModeAttackIndex != -1) {
                        for (int i = hardModeAttackIndex; i < attacks.Length - 1; i++) {
                            attacks[i] = attacks[i + 1];
                        }
                    }
                    md.SetMonsterAnimatorClips();
                    attackNum--;
                    CalculateStats(true);
                }
            }
        }

        #endregion

        #region [ Section 0 ] Combat Information

        public void AddHP(int amount) {    
            CHP += amount;

            if (CHP > HP) {
                CHP = HP;
            }

            StartCoroutine(md.DisplayHPChange(amount, false, false, "MplaceHolderEffect"));
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

            yield return StartCoroutine(md.DisplayHPChange(amount, false, true, "MplaceHolderEffect"));
        }

        /// <summary>
        /// Reduce monster's HP
        /// </summary>
        /// <param name="amount"> Amount of HP to lose, not negative </param>
        /// <param name="animationClipName"> Name of clip to play when monster is attacked </param>
        /// <returns> Starts coroutine of monster being attacked, before yielding control </returns>
        public IEnumerator LoseHP(int amount, string animationClipName) {
            // some sources such as results will use negative numbers to indicate loss
            amount = Mathf.Abs(amount);

            CHP -= amount;
            if (CHP < 0) {
                CHP = 0;
            }
            
            if (GameManager.instance.gsDataCurrent.achievementsUnlocked[(int)achievementConstants.LOTTADAMAGE] == false) {
                if (amount >= 50) {
                    GameManager.instance.gsDataCurrent.achievementsUnlocked[(int)achievementConstants.LOTTADAMAGE] = true;
                    EventManager.instance.SetAchievementNotification((int)achievementConstants.LOTTADAMAGE);
                }
            }
            yield return StartCoroutine(md.DisplayHPChange(amount, true, false, animationClipName));
        }

        public IEnumerator TriggerStatuses() {
            int damageTaken = 0;
            int[] animationsToPlay = new int[] { 0 ,0, 0, 0, 0 }; 
            bool isCure = GetStatusEffect(StatusEffectConstant.CURE) != -1;

            foreach (StatusEffect se in statusEffects) {
                if (se.name == StatusEffectConstant.BURN) {
                    damageTaken += se.value;
                    animationsToPlay[0] = 1;
                }
                else if (se.name == StatusEffectConstant.POISON) {

                    damageTaken += se.value;
                    animationsToPlay[1] = 1;
                }
                else if (se.name == StatusEffectConstant.BLEED) {
                    int bleedDamage = se.value;
                    damageTaken += bleedDamage;
                    if (se.afflicter != null && se.afflicter.CheckDeath() == false) {
                        ((PartyMember)(se.afflicter)).AddHP(bleedDamage);
                    }
                    animationsToPlay[2] = 1;
                }
                else if (se.name == StatusEffectConstant.FROSTBITE) {
                    damageTaken += se.value;
                    animationsToPlay[3] = 1;
                }
                else if (se.name == StatusEffectConstant.CHAMPIONHP || se.name == StatusEffectConstant.REGENERATE) {
                    damageTaken -= se.value; 
                    animationsToPlay[4] = 1;
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

            if (animationsToPlay[0] == 1) {
                md.PlayBurnAnimation();
            }
            if (animationsToPlay[1] == 1) {
                md.PlayPoisonAnimation();
            }
            if (animationsToPlay[2] == 1) {
                md.PlayBleedAnimation();
            }
            if (animationsToPlay[3] == 1) {
                md.PlayFrostbiteAnimation();
            }
            if (animationsToPlay[4] == 1) {
                md.PlayRegenerateAnimation();
            }

            if (damageTaken > 0) {
                yield return StartCoroutine(LoseHP(damageTaken, "MplaceHolderEffect")); // IMPORTANT: This placeholder must be as long as for all se animations
            }
            else if (damageTaken < 0) {     
                yield return StartCoroutine(AddHPYield(damageTaken * -1));
            }

            if (seToRemove.Count > 0) {
                RemoveStatusEffects();
                md.UpdateTooltip();
            }
        }

        /// <summary>
        /// Handles the calculations involved when attack hits this monster
        /// </summary>
        /// <param name="a"> Attack used on this character </param>
        /// <param name="c"> Character attacking this </param>
        /// <returns></returns>
        public IEnumerator GetAttacked(Attack a, Character c) {
            bool attackHit = CalculateAttackHit(c);
            PartyMember pmc = (PartyMember)c;
           
            if (attackHit) {
                int damage = CalculateAttackDamage(a);
                bool isCrit = CalculateAttackCrit(c);
                bool isStatus = CalculateAttackStatus(a, c);
                if (isCrit) {
                    damage = CalculateAttackDamageCrit(damage, c);
                    damage = CalculateAttackReductions(damage, a);
                    md.SetCrit();
                }
                else {
                     damage = CalculateAttackReductions(damage, a);
                }

                yield return StartCoroutine(LoseHP(damage, a.animationClipName));

                // side effects from partyMember skills
                if (pmc.className == ClassConstants.WARRIOR) {
                    if (pmc.skills[(int)SkillConstants.WarriorSkills.MANABLADE].skillEnabled == true) {
                        if (a.type == AttackConstants.PHYSICAL) {
                            pmc.AddMP((int)(damage * 0.25f));
                        }
                    }
                }

                if (isStatus == true && CheckDeath() == false) {
                   AddStatusEffect(a.seName, a.seDuration, c);
                }

                // side effects from partyMember on hit effects
                for (int i = 0; i < c.onHitChances.Length; i++) {
                    if (c.onHitChances[i] != 0) {
                        if (CalculateOnHitHit(i, c) == true) {
                            if (i == 0) {
                                AddStatusEffect(StatusEffectConstant.POISON, 2, c);
                            }
                            else if (i == 1) {
                                AddStatusEffect(StatusEffectConstant.WEAKNESS, 2, c);
                            }
                            else if (i == 2) {
                                AddStatusEffect(StatusEffectConstant.BLEED, 2, c);
                            }
                            else if (i == 3) {
                                AddStatusEffect(StatusEffectConstant.ROOT, 2, c);
                            }
                            else if (i == 4) {
                                AddStatusEffect(StatusEffectConstant.SHOCK, 2, c);
                            }
                        }
                    }
                }
            }
            else {
                yield return StartCoroutine(DodgeAttack(a.animationClipName));
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
                yield return StartCoroutine(md.DisplayAttackEffect(a.animationClipName));
                AddStatusEffect(a.seName, a.seDuration, c);
            }
            else {
                yield return StartCoroutine(DodgeAttack(a.animationClipName));
            }
        }

        /// <summary>
        /// Handles all logic when a monster's attack helps a monster (healing, buffs, etc.)
        /// </summary>
        /// <param name="a"> Attack targeting this monster </param>
        /// <param name="c"> Character targeting this </param>
        /// <returns></returns>
        public IEnumerator GetHelped(Attack a, Character c) {
            if (a.type == AttackConstants.HEALHP || a.type == AttackConstants.HEALHPSELF) {
                int healed = CalculateAttackHeal(a);
                bool isCrit = CalculateAttackCrit(c);
                bool isStatus = CalculateAttackStatus(a, c);

                if (isCrit == true) {
                    healed = CalculateAttackHealCrit(healed, c);
                }

                yield return StartCoroutine(AddHPYield(healed));

                if (isStatus == true && CheckDeath() == false) {
                    AddStatusEffect(a.seName, a.seDuration, c);
                }
            }
            else if (a.type == AttackConstants.BUFF || a.type == AttackConstants.BUFFSELF) { // TODO: Make monster's receiving the buff play an animation that is yielded to if its casted by another monster
                AddStatusEffect(a.seName, a.seDuration, c);
            }
        }

        /// <summary>
        /// Handles all logic and visual effects upon dodging an attack
        /// </summary>
        /// <param name="animationClipName"></param>
        /// <returns></returns>
        public IEnumerator DodgeAttack(string animationClipName) {
            yield return StartCoroutine(md.DisplayAttackDodged(animationClipName));
        }

        /// <summary>
        /// Adds a status effect to the monster
        /// </summary>
        /// <param name="seName"> Name of the statusEffect </param>
        /// <param name="seDuration"> Duration of the statusEffect </param>
        /// <param name="c"> Character afflicting the statusEffect on this character, can be null for some effects </param>
        public void AddStatusEffect(StatusEffectConstant seName, int seDuration, Character c) {
            if (CanBeStatusEffected() == true) {
                if (seName == StatusEffectConstant.RBW) {    // for archer's cursed roots, rbw randomly chooses
                    int index = Random.Range(0, 3);
                    switch(index) {
                        case 0:
                            seName = StatusEffectConstant.BLEED;
                            break;
                        case 1:
                            seName = StatusEffectConstant.WEAKNESS;
                            break;
                        case 2:
                            seName = StatusEffectConstant.ROOT;
                            break;
                        default:
                            break;
                    }
                }
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
                md.AddStatusEffectDisplay(newStatus);

                UpdateStatusEffectValues();
                md.UpdateTooltip();         // update the tooltip last in case a status effect changes PDEF or MDEF when combined with another

                if (seName == StatusEffectConstant.STUN) {
                    PartyManager.instance.TriggerSkillEnabled(ClassConstants.ROGUE, (int)SkillConstants.RogueSkills.AMBUSHER, c);
                }
                if (GameManager.instance.gsDataCurrent.achievementsUnlocked[(int)achievementConstants.ANTIMASKERS] == false) {
                    if (statusEffects.Count >= 5) {
                        GameManager.instance.gsDataCurrent.achievementsUnlocked[(int)achievementConstants.ANTIMASKERS] = true;
                        EventManager.instance.SetAchievementNotification((int)achievementConstants.ANTIMASKERS);
                    }  
                }
            }
        }

        /// <summary>
        /// Returns true if the monster should drop an item
        /// </summary>
        /// <returns></returns>
        public bool CheckItemDrop(float multiplier) {
            if (Random.Range(0, 100) < (int)(dropChance * multiplier)) {
                return true;
            }

            return false;
        }

        #endregion

        /// <summary>
        /// Returns true if the pmvc is no longer animating anything
        /// </summary>
        /// <returns></returns>
        public bool IsDoneAnimating() {
            return md.isAnimating == false;
        }

        /// <summary>
        /// Logs primary stat information for debugging
        /// </summary>
        public override void LogPrimaryStats() {
            Debug.Log(monsterNameID);
            base.LogPrimaryStats();
        }

        /// <summary>
        /// Logs monster's display name for debugging
        /// </summary>
        public override void LogName() {
            Debug.Log("Name " + monsterNameID);
        }
    }
}
