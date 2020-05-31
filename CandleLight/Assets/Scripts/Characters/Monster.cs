/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 11, 2019
* 
* The Monster class is used to store and manipulate information about the Monster. 
* It is always attached to a Monster gameObject.
*
* TO DO: Get rid of the monsterDisplayName and replace it with spriteName for tooltips
*
*/

using AttackConstants = Constants.AttackConstants;
using Combat;
using PartyManager = Party.PartyManager;
using Result = Events.Result;
using SkillConstants = Constants.SkillConstants;
using StatusEffectConstants = Constants.StatusEffectConstants;
using System.Collections;
using UnityEngine;

namespace Characters {

    public class Monster : Character {
        
        /* external component references */
        public MonsterDisplay md;

        [field: SerializeField] public Result monsterReward { get; private set; }       /// <value> Result monster gives on death </value>
        [field: SerializeField] public string monsterArea { get; private set; }         /// <value> Area where monster can be found </value>
        [field: SerializeField] public string monsterSize { get; private set; }         /// <value> String constant describing size of monster's sprite </value>
        [field: SerializeField] public string monsterNameID { get; private set; }       /// <value> NameID as referenced in database </value>
        [field: SerializeField] public string monsterSpriteName { get; private set; }   /// <value> Name of monster's sprite as referenced in resources </value>
        [field: SerializeField] public string monsterDisplayName { get; private set; }  /// <value> Monster name <value>
        [field: SerializeField] public string monsterAI { get; private set; }           /// <value> Monster's behaviour in combat </value>
        [field: SerializeField] public int multiplier { get; private set; }             /// <value> Multipler to EXP and WAX rewarded (due to being a boss, variant, etc) </value>
        [field: SerializeField] public int minLVL { get; private set; }                 /// <value> Minimum power level monster can spawn at </param>
        [field: SerializeField] public int maxLVL { get; private set; }                 /// <value> Maximum power level monster can spawn at </param>
        [field: SerializeField] public int EXP { get; private set; }                    /// <value> EXP monster gives on defeat </value>
        [field: SerializeField] public int WAX { get; private set; }                    /// <value> WAX monster gives on defeat </value>
        [field: SerializeField] public int dropChance { get; private set; }             /// <value> Chance of monster giving a result </value>
        [field: SerializeField] public int championChance { get; private set; }         /// <value> Chance of monster spawning with a champion buff </value>
        [field: SerializeField] public bool isReady { get; private set; }               /// <value> Flag for when monsterDisplay is done setting properties </value>

        #region [ Initialization ] Initialization

        /// <summary>
        /// Initializes the monster's properties and display
        /// </summary>
        /// <param name="monsterNameID"> Name of monster as referenced by the database </param>
        /// <param name="monsterSpriteName"> Name of monster's sprite, castle case </param>
        /// <param name="monsterDisplayName"> Name of monster in game, separated by spaces </param>
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
        public IEnumerator Init(string monsterNameID, string monsterSpriteName, string monsterDisplayName, string monsterArea, 
        string monsterSize, string monsterAI, int multiplier, int HP, int MP, int[] stats, Attack[] attacks,
        int dropChance, Result monsterReward, int championChance) {
            this.monsterNameID = monsterNameID;
            this.monsterSpriteName = monsterSpriteName;
            this.monsterDisplayName = monsterDisplayName;
            this.monsterArea = monsterArea;
            this.monsterAI = monsterAI;
            this.monsterReward = monsterReward;
            this.championChance = championChance;

            string[] LVLString = monsterNameID.Split(' ');
            this.minLVL = int.Parse(LVLString[1]);  // efficiency won't matter for numbers less than 1000
            this.maxLVL = int.Parse(LVLString[2]);
            base.Init(minLVL, HP, MP, stats, attacks);  // use minLVL for initialization, will use for scaling up on spawning
            CalculateStats(true);

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
            HP = (int)(STR * 2.25 + DEX * 1.5);
            MP = (int)(INT * 1.25 + LUK * 0.5);
            PATK = (int)(STR * 0.65 + DEX * 0.35);  // monsters have better primary scaling on PATK and MATK than players
            MATK = (int)(INT * 0.65 + LUK * 0.35);
            PDEF = (int)(STR * 0.1 + DEX * 0.05);
            MDEF = (int)(INT * 0.15 + LUK * 0.05);
            DOG = (int)(DEX * 0.2 + LUK * 0.1);
            ACC = (int)(DEX * 0.2 + STR * 0.1 + INT * 0.1) + defaultACC;
            critChance = (int)(LUK * 0.1) + baseCritChance;
            critMult = baseCritMult;

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
                    DOG -= (int)(DOG * 0.5);
                }
                else if (se.name == StatusEffectConstants.CHAMPIONHP) {
                    HP += (int)(HP * 0.66);
                }
                else if (se.name == StatusEffectConstants.CHAMPIONPATK) {
                    PATK += (int)(1 + PATK * 0.5);
                    HP += (int)(HP * 0.33);
                }
                else if (se.name == StatusEffectConstants.CHAMPIONMATK) {
                    MATK += (int)(1 + MATK * 0.5);
                    HP += (int)(HP * 0.33);
                }
                else if (se.name == StatusEffectConstants.CHAMPIONPDEF) {
                    PDEF += (int)(1 + PDEF * 0.5);
                    HP += (int)(HP * 0.33);
                }
                else if (se.name == StatusEffectConstants.CHAMPIONMDEF) {
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
            if (subAreaProgress < 35) { // prevent stronger monsters from appearing at the start of the area
                base.MultipleLVLUp(minLVL, Mathf.Max(minLVL, maxLVL - 1), this.multiplier);  
            }
            else {
                base.MultipleLVLUp(minLVL, maxLVL, this.multiplier);  
            }
            // it takes 5 LVL 1 enemies for a LVL 1 player to reach LVL 2
            // it takes 47 LVL 98 enemies for LVL 98 player to reach LVL 99
            // + LVL - 1 is meant to ease the grind in early levels
            this.EXP = (int)((Mathf.Pow(LVL, 1.65f) + ((STR + DEX + INT + LUK) / 10)) * this.multiplier);  
            this.WAX = LVL * this.multiplier;   

            md.SetTooltip();
            md.SetHealthBar();
        }

        public void GetBuffs(string[] championBuffs) {
            GetBossBuff();
            GetChampionBuff(championBuffs);
            md.UpdateTooltip();
            md.SetHealthBar();
        }

        public void GetBossBuff() {
            if (multiplier == 3) {      // boss monsters will always have an EXP multiplier of 3
                StatusEffect newStatus = new StatusEffect(StatusEffectConstants.BOSS, 999);
                AddStatusEffectPermanent(newStatus);
                md.AddStatusEffectDisplay(newStatus);
            }
        }


        /// <summary>
        /// Applies a champion buff to a monster at random
        /// </summary>
        /// <param name="championBuffs"> List of championBuffs that can be applied in the subArea (assumed length 3) </param>
        public void GetChampionBuff(string[] championBuffs) {
            bool isChampion = Random.Range(0, 100) < championChance;

            if (isChampion) {
                multiplier += 1;
                this.EXP = (int)((Mathf.Pow(LVL, 1.65f) + ((STR + DEX + INT + LUK) / 10)) * this.multiplier);  
                monsterReward.UpgradeResult();
                dropChance = 100;

                string championBuff = championBuffs[Random.Range(0, 3)];    // three champion buffs per subArea
                StatusEffect newStatus;
                switch (championBuff) {
                    case StatusEffectConstants.CHAMPIONATK:
                        {
                            if (PATK >= MATK) {
                                newStatus = new StatusEffect(StatusEffectConstants.CHAMPIONPATK, 999);
                            }
                            else {
                                newStatus = new StatusEffect(StatusEffectConstants.CHAMPIONMATK, 999);
                            }
                            AddStatusEffectPermanent(newStatus);
                            md.AddStatusEffectDisplay(newStatus);
                            break;
                        }     
                    case StatusEffectConstants.CHAMPIONDEF:
                        {
                            if (PDEF >= MDEF) {
                                newStatus = new StatusEffect(StatusEffectConstants.CHAMPIONPDEF, 999);
                            }
                            else {
                                newStatus = new StatusEffect(StatusEffectConstants.CHAMPIONMDEF, 999);
                            }
                            AddStatusEffectPermanent(newStatus);
                            md.AddStatusEffectDisplay(newStatus);
                            break;
                        }
                    case StatusEffectConstants.CHAMPIONHP:
                        {
                            newStatus = new StatusEffect(StatusEffectConstants.CHAMPIONHP, 999);
                            newStatus.SetValue(this, this);
                            AddStatusEffectPermanent(newStatus);
                            md.AddStatusEffectDisplay(newStatus);
                            break;
                        }
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

            StartCoroutine(md.DisplayHPChange(amount, false, "MplaceHolderEffect"));
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
            
            yield return StartCoroutine(md.DisplayHPChange(amount, true, animationClipName));
        }

        public IEnumerator TriggerStatuses() {
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
                    if (se.afflicter != null && se.afflicter.CheckDeath() == false) {
                        ((PartyMember)(se.afflicter)).AddHP(bleedDamage);
                    }
                    animationsToPlay[2] = 1;
                }
                else if (se.name == StatusEffectConstants.CHAMPIONHP) {
                    damageTaken -= se.value; 
                }

                se.UpdateDuration();
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

            if (damageTaken > 0) {
                yield return StartCoroutine(LoseHP(damageTaken, "MplaceHolderEffect"));
            }
            else if (damageTaken < 0) {     
                AddHP(damageTaken * -1);
            }

            RemoveStatusEffects();
        }

        /// <summary>
        /// Handles the calculations involved when attack hits this monster
        /// </summary>
        /// <param name="a"> Attack used on this character </param>
        /// <param name="c"> Character attacking this </param>
        /// <param name="animationClipName"> Animation clip to play of the attack used </param>
        /// <returns></returns>
        public IEnumerator GetAttacked(Attack a, Character c, string animationClipName) {
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

                yield return StartCoroutine(LoseHP(damage, animationClipName));

                // side effects from partyMember skills
                if (pmc.className == "Warrior") {
                    if (pmc.skills[(int)SkillConstants.warriorSkills.MANABLADE].skillEnabled == true) {
                        if (a.type == AttackConstants.PHYSICAL) {
                            pmc.AddMP((int)(damage * 0.25f));
                        }
                    }
                }

                if (isStatus) {
                    if (GetStatusEffect(a.seName) == -1) {  // no two statusEffects of the same type can be on at once
                        StatusEffect newStatus = new StatusEffect(a.seName, a.seDuration);
                        newStatus.SetValue(c, this);
                        AddStatusEffect(newStatus);
                        md.AddStatusEffectDisplay(newStatus);
                        md.UpdateTooltip();

                        if (a.seName == StatusEffectConstants.STUN) {
                            PartyManager.instance.TriggerSkillEnabled("Rogue", (int)SkillConstants.rogueSkills.AMBUSHER, pmc);
                        }
                    }
                }
                
                UpdateStatusEffectValues();
            }
            else {
                yield return StartCoroutine(DodgeAttack(animationClipName));
            }
        }

        /// <summary>
        /// Handles the calculations involved when attacked by an attack that only has a statusEffect
        /// associated with it
        /// </summary>
        /// <param name="a"></param>
        /// <param name="c"> Character statusing this </param>
        /// <param name="animationClipName"></param>
        /// <returns></returns>
        public IEnumerator GetStatusEffected(Attack a, Character c, string animationClipName) {
            bool attackHit = CalculateAttackHit(c);
           
            if (attackHit) {
                yield return StartCoroutine(md.DisplayAttackEffect(animationClipName));
                if (GetStatusEffect(a.seName) == -1) {  // no two tatusEffects of the same type can be on at once
                    StatusEffect newStatus = new StatusEffect(a.seName, a.seDuration);
                    newStatus.SetValue(c, this);
                    AddStatusEffect(newStatus);
                    md.AddStatusEffectDisplay(newStatus);
                    md.UpdateTooltip();

                    UpdateStatusEffectValues();
                }
            }
            else {
                yield return StartCoroutine(DodgeAttack(animationClipName));
            }
        }

        /// <summary>
        /// Applies a statusEffect to itself
        /// </summary>
        /// <param name="a"></param>
        public void GetStatusEffectedSelf(Attack a) {
            int index = statusEffects.FindIndex(se => se.name == a.seName);
             if (index == -1) {  // no two tatusEffects of the same type can be on at once
                StatusEffect newStatus = new StatusEffect(a.seName, a.seDuration);
                newStatus.SetValue(this, this);
                AddStatusEffect(newStatus);
                md.AddStatusEffectDisplay(newStatus);
                md.UpdateTooltip();

                UpdateStatusEffectValues();
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
        /// Logs primary stat information for debugging
        /// </summary>
        public override void LogPrimaryStats() {
            Debug.Log(monsterDisplayName);
            base.LogPrimaryStats();
        }

        /// <summary>
        /// Logs monster's display name for debugging
        /// </summary>
        public override void LogName() {
            Debug.Log("Name " + monsterDisplayName);
        }
    }
}
