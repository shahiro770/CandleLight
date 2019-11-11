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

using AssetManagers;
using Combat;
using StatusEffectConstants = Constants.StatusEffectConstants;
using System.Collections;
using System.Collections.Generic;
using Result = Events.Result;
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
        [field: SerializeField] public int attackNum { get; private set; } = 0;         /// <value> Number of attacks monster has (max 4) </value>
        [field: SerializeField] public int selectedAttackIndex { get; private set; }    /// <value> Index of attack selected </value>
        [field: SerializeField] public int dropChance { get; private set; }             /// <value> Chance of monster giving a result </value>
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
        int dropChance, Result monsterReward) {
            this.monsterNameID = monsterNameID;
            this.monsterSpriteName = monsterSpriteName;
            this.monsterDisplayName = monsterDisplayName;
            this.monsterArea = monsterArea;
            this.monsterAI = monsterAI;
            this.monsterReward = monsterReward;

            string[] LVLString = monsterNameID.Split(' ');
            this.minLVL = int.Parse(LVLString[1]);  // efficiency won't matter for numbers less than 1000
            this.maxLVL = int.Parse(LVLString[2]);
            base.Init(minLVL, HP, MP, stats, attacks);  // use minLVL for initialization, will use for scaling up on spawning

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

            foreach (Attack a in attacks) {
                if (a.name != "none") {
                    attackNum++;
                }
            }

            StartCoroutine(md.Init(this));

            this.isReady = true;
            yield break;
        }

        /// <summary>
        /// Sets the monster's level between its minimum level and maximum level
        /// </summary>
        public void MultipleLVLUp() {
            base.MultipleLVLUp(minLVL, maxLVL, this.multiplier);  
            // it takes 5 LVL 1 enemies for a LVL 1 player to reach LVL 2
            // it takes 47 LVL 98 enemies for LVL 98 player to reach LVL 99
            // + LVL - 1 is meant to ease the grind in early levels
            this.EXP = (int)((Mathf.Pow(LVL, 1.65f) + ((STR + DEX + INT + LUK) / 10)) * this.multiplier);  
            this.WAX = LVL * this.multiplier;   

            md.SetTooltip();
            md.SetHealthBar();
        }

        #endregion

        #region [ Section 0 ] Combat Information

        /// <summary>
        /// Returns the monster's selected attack based on its AI
        /// </summary>
        /// <returns> An Attack object to be used </returns>
        public Attack SelectAttack() {
            if (monsterAI == "random" || monsterAI == "weakHunter") {
                selectedAttackIndex = Random.Range(0, attackNum);
            }

            return attacks[selectedAttackIndex];  
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
            int[] animationsToPlay = new int[] { 0 ,0 }; 

            foreach (StatusEffect se in statusEffects) {
                if (se.name == StatusEffectConstants.BURN) {
                    damageTaken += CalculateStatusEffectReductions(se);
                    animationsToPlay[0] = 1;
                }
                else if (se.name == StatusEffectConstants.POISON) {
                    damageTaken += CalculateStatusEffectReductions(se);
                    animationsToPlay[1] = 1;
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

            // TODO: Make this play multiple animations overtop one another
            if (damageTaken > 0) { //might be a bad way to check cause 0 damage is the ting
                yield return StartCoroutine(LoseHP(damageTaken, "MplaceHolderEffect"));
            }

            foreach (StatusEffect se in seToRemove) {
                se.DestroyDisplay();
                statusEffects.Remove(se);
            }
            seToRemove.Clear();
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

                if (isStatus) {
                    int index = statusEffects.FindIndex(se => se.name == a.seName);
                    if (index == -1) {  // no two tatusEffects of the same type can be on at once
                        StatusEffect newStatus = new StatusEffect(a.seName, a.seDuration);
                        newStatus.SetValue(this, c);
                        AddStatusEffect(newStatus);
                        md.AddStatusEffectDisplay(newStatus);
                    }
                }
            }
            else {
                yield return StartCoroutine(DodgeAttack(animationClipName));
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
        /// Check if monster is dead
        /// </summary>
        /// <returns> True if monster is dead, false otherwise </returns>
        public bool CheckDeath() {
            return CHP == 0;
        }

        /// <summary>
        /// Returns true if the monster should drop an item
        /// </summary>
        /// <returns></returns>
        public bool CheckItemDrop() {
            if (Random.Range(0, 100) < dropChance) {
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
