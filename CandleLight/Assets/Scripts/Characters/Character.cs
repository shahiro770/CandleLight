/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 11, 2019
* 
* The Character class is used to store information about characters that can act in combat. 
*
*/

using AttackConstants = Constants.AttackConstants;
using StatusEffectConstants = Constants.StatusEffectConstants;
using Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Characters {

    public abstract class Character : MonoBehaviour {

        [field: SerializeField] public int ID { get; set; }               /// <value> Unique identification number in combat </value>
        [field: SerializeField] public int LVL { get; set; }              /// <value> Power level </value>
        [field: SerializeField] public int HP { get; set; }               /// <value> Max health points </value>
        [field: SerializeField] public int CHP { get; set; }              /// <value> Current health points </value>
        [field: SerializeField] public int MP { get; set; }               /// <value> Max mana points </value>
        [field: SerializeField] public int CMP { get; set; }              /// <value> Current mana points </value>
        [field: SerializeField] public float MPRegen { get; set; }        /// <value> Percentage amount of mana points regenerated between events </value>
        [field: SerializeField] public float HPRegen { get; set; }        /// <value> Percentage amount of health points regenerated between events </value>
        [field: SerializeField] public int baseSTR { get; set; }          /// <value> Strength </value>
        [field: SerializeField] public int baseDEX { get; set; }          /// <value> Dexterity </value>
        [field: SerializeField] public int baseINT { get; set; }          /// <value> Intelligence </value>
        [field: SerializeField] public int baseLUK { get; set; }          /// <value> Luck </value>
        [field: SerializeField] public int STR { get; set; }              /// <value> Strength after all modifiers </value>
        [field: SerializeField] public int DEX { get; set; }              /// <value> Dexterity after all modifiers  </value>
        [field: SerializeField] public int INT { get; set; }              /// <value> Intelligence after all modifiers </value>
        [field: SerializeField] public int LUK { get; set; }              /// <value> Luck after all modifiers </value>
        [field: SerializeField] public int PATK { get; set; }             /// <value> Physical attack </value>
        [field: SerializeField] public int MATK { get; set; }             /// <value> Magical attack </value>
        [field: SerializeField] public int PDEF { get; set; }             /// <value> Physical defense </value>
        [field: SerializeField] public int MDEF { get; set; }             /// <value> Magical defense </value>
        [field: SerializeField] public int DOG { get; set; }              /// <value> Dodge rating </value>
        [field: SerializeField] public int ACC { get; set; }              /// <value> Accuracy rating </value>
        [field: SerializeField] public int tempACC = 0;                   /// <value> Bonus accuracy accumulated by missing </value>
        [field: SerializeField] public int critChance { get; set; }       /// <value> % chance to crit </value>
        [field: SerializeField] public int attackNum { get; set; } = 0;   /// <value> Number of attacks monster has (max 4) </value>
        [field: SerializeField] public int championChance { get; set; }   /// <value> Additional percentage chance of a champion monster spawning for PartyMembers, or base chance for Monsters </value>
        [field: SerializeField] public int turnCounter { get; set; }      /// <value> Counter for number of turns this character has taken in a new instance of combat </value>
        [field: SerializeField] public float critMult { get; set; }       /// <value> Critical damage multiplier </value>
        [field: SerializeField] public bool bleedPlus { get; set; } = false;    /// <value> Flag for if this character can inflict stronger bleeds </value>
        [field: SerializeField] public Attack[] attacks { get; set; }     /// <value> List of known attacks (length 4) </value>
        [field: SerializeField] public List<StatusEffect> statusEffects { get; set; }     /// <value> List of afflicted status effects </value>
        
        protected List<StatusEffect> seToRemove = new List <StatusEffect>();    /// <value> Status effects to remove </value>
        protected float baseHPRegen = 0.06f;                                /// <value> Base percentage of max MP recovered between events </value>
        protected float baseMPRegen = 0.12f;                                /// <value> Base percentage of max HP recovered between events </value>
        protected float baseCritMult = 1.5f;                                /// <value> Base crit attack damage multiplier </value>
        protected float tempACCBonus = 0.05f;                              /// <value> % of base ACC tempACC increments by </value>
        protected int minAttacks = 1;
        protected int maxAttacks = 4;
        protected int maxStatusEffects = 10;                                /// <value> Max number of status effects that can be on a character </value>
        protected int baseCritChance = 5;                                   /// <value> Base chance of an attack doing critMultiplier* damage </value>
        protected int defaultACC = 95;                                      /// <value> Base accuracy rating </value>
       
        /// <summary>
        /// Initializes character properties
        /// </summary>
        /// <param name="LVL"> Power level </param>
        /// <param name="HP"> Max health points</param>
        /// <param name="MP"> Max mana points </param>
        /// <param name="stats"> baseSTR, baseDEX, baseINT, and baseLUK </param>
        /// <param name="attacks"> List of known attacks (length 4) </param>
        public virtual void Init(int LVL, int[] stats, Attack[] attacks) {
            this.LVL = LVL;
            this.baseSTR = stats[0];
            STR = baseSTR;
            this.baseDEX = stats[1];
            DEX = baseDEX;
            this.baseINT = stats[2];
            INT = baseINT;
            this.baseLUK = stats[3];
            LUK = baseLUK;
            this.attacks = attacks; 

            foreach (Attack a in attacks) {
                if (a.name != "none") {
                    attackNum++;
                }
            }
        }
        
        /// <summary>
        /// Calculates secondary stats based off of the 4 primary stats
        /// </summary>
        /// <param name="setCurrent"> Flag for if CHP and CMP should equal new HP and MP values </param>
        protected virtual void CalculateStats(bool setCurrent = false) { }

        /// <summary>
        /// Levels up a character a somewhat random number of times
        /// </summary>
        /// <param name="minLVL"> Minimum level the character can be (assumed to be current level of the character) </param>
        /// <param name="maxLVL"> Maximum level the character can be </param>
        /// <param name="multiplier"> Bonus multiplier on stats (monsters might have modifiers) </param>
        public void MultipleLVLUp(int minLVL, int maxLVL, int multiplier) {            
            int gainedLVLs = Random.Range(minLVL, maxLVL + 1) - LVL;

            for (int i = 0; i < gainedLVLs; i++) {
                LVLUp(multiplier);
            }            
        }

        /// <summary>
        /// Levels up the character, raising its stats
        /// </summary>
        /// <param name="multiplier"> Bonus multiplier on stats </param>
        public virtual void LVLUp(int multiplier = 1) {
            LVL += 1;
            baseSTR += (int)(LVL * 0.5 + baseSTR * 0.3 * multiplier);
            STR = baseSTR;
            baseDEX += (int)(LVL * 0.5 + baseDEX * 0.3 * multiplier);
            DEX = baseDEX;
            baseINT += (int)(LVL * 0.5 + baseINT * 0.3 * multiplier);
            INT = baseINT;
            baseLUK += (int)(LVL * 0.5 + baseLUK * 0.3 * multiplier);
            LUK = baseLUK;
            CalculateStats(true);
        }

        /// <summary>
        /// Returns true if the partyMember is dead
        /// </summary>
        /// <returns></returns>
        public bool CheckDeath() {
            return CHP == 0;
        }

        /// <summary>
        /// Returns the value from an attack, based on the attack's formula
        /// </summary>
        /// <param name="attack"> Attack of object </param>
        /// <returns> Integer amount </returns>
        public int GetAttackValue(Attack a) {
            Mathos.Parser.MathParser parser = new Mathos.Parser.MathParser();

            parser.LocalVariables.Add("LVL", LVL);
            parser.LocalVariables.Add("HP", HP);
            parser.LocalVariables.Add("CHP", CHP);
            parser.LocalVariables.Add("MP", MP);
            parser.LocalVariables.Add("CMP", CMP);
            parser.LocalVariables.Add("STR", STR);
            parser.LocalVariables.Add("DEX", DEX);
            parser.LocalVariables.Add("INT", INT);
            parser.LocalVariables.Add("LUK", LUK);
            parser.LocalVariables.Add("PATK", PATK);
            parser.LocalVariables.Add("MATK", MATK);
            parser.LocalVariables.Add("PDEF", PDEF);
            parser.LocalVariables.Add("MDEF", MDEF);

            return (int)parser.Parse(a.damageFormula);
        }

        public int GetCostValue(Attack a) {
            Mathos.Parser.MathParser parser = new Mathos.Parser.MathParser();

            parser.LocalVariables.Add("LVL", LVL);

            return (int)parser.Parse(a.costFormula);
        }

        /// <summary>
        /// Set each attack's attack value
        /// </summary>
        public virtual void SetAttackValues() {
            foreach(Attack a in attacks) {
                if (a.nameKey != "none_attack") {
                    a.attackValue = GetAttackValue(a);
                    a.costValue = GetCostValue(a);
                }
            }
        }

        /// <summary>
        /// Determines if an attack aimed at this character hits or misses
        /// </summary>
        /// <param name="c"> Other character attacking </param>
        /// <returns> True if attack hits, false otherwise </returns>
        /// <remark> 
        /// Base accuracy is 95
        /// Every time an attack misses, the attacking character gets increasing bonus accuracy,
        /// resetting that bonus back to 0 upon hitting.
        /// </remark>
        protected bool CalculateAttackHit(Character c) {
            int hitChance = c.ACC + c.tempACC - DOG;

            if (hitChance > 100) {
                hitChance = 100;
            }
            else if (hitChance < 0) {
                hitChance = 0;
            }

            bool attackHit = Random.Range(0, 100) < hitChance;

            if (attackHit) {
                c.tempACC = 0;
            }
            else {
                c.tempACC += (int)(c.ACC * tempACCBonus + c.tempACC);  
            }

            return attackHit;            
        }

        /// <summary>
        /// Calculates if an attack from another character is a critical hit, and amplifies the damage
        /// based on that character's critical hit damage multiplier.
        /// </summary>
        /// <param name="amount"> Amount of damage </param>
        /// <param name="c"> Other character </param>
        /// <returns> Amount after potential critical multiplier </returns>
        protected bool CalculateAttackCrit(Character c) {
            bool attackCrit = Random.Range(0, 100) < c.critChance;
            
            return attackCrit;
        }

        /// <summary>
        /// Calculates the base damage of an attack against this character
        /// </summary>
        /// <param name="a"> Attack object </param>
        /// <returns> The amount of damage taken </returns>
        public int CalculateAttackDamage(Attack a) {
            if (a.type == AttackConstants.MAGICAL && GetStatusEffect(StatusEffectConstants.SHOCK) != -1) {
                return (int)(a.attackValue * 1.5f);
            }
            return a.attackValue;
        }

        /// <summary>
        /// Calculates damage of an attack against this character after all reductions (armour, skills, etc.)
        /// </summary>
        /// <param name="damage"> Damage amount</param>
        /// <param name="a"> Attack </param>
        /// <returns></returns>
        public int CalculateAttackReductions(int damage, Attack a) {
             if (a.type == AttackConstants.PHYSICAL) {
                damage = damage - PDEF;
            }
            else if (a.type == AttackConstants.MAGICAL) {
                damage = damage - MDEF; 
            }
            if (damage < 0) {
                damage = 0;
            }  
            
            return damage;
        }

        /// <summary>
        /// Calculates the damage of a critical attack against this character
        /// </summary>
        /// <param name="amount"> Damage value coming in </param>
        /// <param name="c"> Character who attacked this character </param>
        /// <returns></returns>
        protected int CalculateAttackDamageCrit(int amount, Character c) {
            return (int)(amount * c.critMult);
        }

        protected bool CalculateAttackStatus(Attack a, Character c) {
            bool attackStatus = Random.Range(0, 100) < a.seChance;

            return attackStatus;
        }

        /// <summary>
        /// Calculates the amount healed by an attack against this character
        /// </summary>
        /// <param name="a"> Attack object </param>
        /// <returns> The amount of HP healed </returns>
        public int CalculateAttackHeal(Attack a) { 
            return a.attackValue;
        }

        /// <summary>
        /// Calculates the healing of a critical heal against this character
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public int CalculateAttackHealCrit(int amount, Character c) {
            return (int)(amount * c.critMult);
        }

        /// <summary>
        /// Calculates the amount of mana restored by an attack against this character
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public int CalculateAttackFocus(Attack a) {
            return a.attackValue;
        }

        /// <summary>
        /// Calculates the amount of mana restored by an attack against this character
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public int CalculateAttackFocusCrit(int amount, Character c) {
            return (int)(amount * c.critMult);
        }

        protected void AddStatusEffect(StatusEffect se) {
            if (statusEffects.Count < maxStatusEffects) {
                statusEffects.Add(se);
                CalculateStats();
            }
        }

        protected void AddStatusEffectPermanent(StatusEffect se) {
            if (statusEffects.Count < maxStatusEffects) {
                statusEffects.Add(se);
                CalculateStats(true);
            }
        }

        /// <summary>
        /// Updates the values on statusEffects
        /// </summary>
        protected void UpdateStatusEffectValues() {
             foreach (StatusEffect se in statusEffects) {
                se.UpdateValue();
            }
        }

        /// <summary>
        /// Removes all status effects in the seToRemove list, undoing their effects
        /// </summary>
        /// <remarks>
        /// As a side effect, due to triggerStatuses always being called at the end of most
        /// stat relevant actions (such as at the end of a combat turn, or transitioning between events),
        /// this forces a stat recalculation for a character, meaning stat changes from candles 
        /// are removed at the end of a turn after a candle is made unusable during combat.
        /// </remarks>
        public void RemoveStatusEffects() {
            foreach (StatusEffect se in seToRemove) {
                se.DestroyDisplay();
                statusEffects.Remove(se);
            }
            seToRemove.Clear();
            CalculateStats();   // TODO: Convert all status effects to ints instead of strings
        }

        /// <summary>
        /// Removes all status effects in the seToRemove list, but doesn't recalculate stats
        /// </summary>
        public void RemoveStatusEffectsNoCalculate() {
            foreach (StatusEffect se in seToRemove) {
                se.DestroyDisplay();
                statusEffects.Remove(se);
            }
            seToRemove.Clear();
        }

        /// <summary>
        /// Removes all status effects
        /// </summary>
        public void RemoveAllStatusEffects() {
            foreach (StatusEffect se in statusEffects) {
                seToRemove.Add(se);
            }
            foreach (StatusEffect se in seToRemove) {
                se.DestroyDisplay();
                statusEffects.Remove(se);
            }
            seToRemove.Clear();
            CalculateStats();   // TODO: Convert all status effects to ints instead of strings
        }

        /// <summary>
        /// Returns the index of a status effect by name
        /// </summary>
        /// <param name="seName"> Name of statusEffect to find </param>
        /// <returns></returns>
        public int GetStatusEffect(string seName) {
            return statusEffects.FindIndex(se => se.name == seName);
        }

        /// <summary>
        /// Returns a status effect at a given index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public StatusEffect GetStatusEffect(int index) {
            return statusEffects[index];
        }

        /// <summary>
        /// Logs stats to console for debugging
        /// </summary>
        public virtual void LogPrimaryStats() {
            Debug.Log(LVL + " " + HP + " " + MP + " " + STR + " " + DEX + " " + INT + " " + LUK);
        }

        /// <summary>
        /// Logs stats to console for debugging
        /// </summary>
        public virtual void LogSecondaryStats() {
            Debug.Log("PDEF: " + PDEF + " MDEF: " + MDEF + " acc: " + ACC + " tempAcc:" + tempACC + " dog: " + DOG);
        }

        /// <summary>
        /// Logs a string to the console for debugging
        /// </summary>
        public virtual void LogName() {
            Debug.Log("No Name");
        }

        /// <summary>
        /// Logs attacks to the console for debugging
        /// </summary>
        public virtual void LogAttacks() {
            Debug.Log("1 " + attacks[0].nameKey + " 2 " + attacks[1].nameKey  + " 3 " + attacks[2].nameKey  + " 4 " + attacks[3].nameKey );
        }
    }
}