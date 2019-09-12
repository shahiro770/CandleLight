/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 11, 2019
* 
* The Character class is used to store information about characters that can act in combat. 
*
*/

using AttackConstants = Constants.AttackConstants;
using Combat;
using UnityEngine;

namespace Characters {

    public abstract class Character : MonoBehaviour {

        [field: SerializeField] public int ID { get; set; }               /// <value> Unique identification number in combat </value>
        [field: SerializeField] public int LVL { get; set; }              /// <value> Power level </value>
        [field: SerializeField] public int HP { get; set; }               /// <value> Max health points </value>
        [field: SerializeField] public int CHP { get; set; }              /// <value> Current health points </value>
        [field: SerializeField] public int MP { get; set; }               /// <value> Max mana points </value>
        [field: SerializeField] public int CMP { get; set; }              /// <value> Current mana points </value>
        [field: SerializeField] public int STR { get; set; }              /// <value> Strength </value>
        [field: SerializeField] public int DEX { get; set; }              /// <value> Dexterity </value>
        [field: SerializeField] public int INT { get; set; }              /// <value> Intelligence </value>
        [field: SerializeField] public int LUK { get; set; }              /// <value> Luck </value>
        [field: SerializeField] public int pAtk { get; set; }             /// <value> Physical attack </value>
        [field: SerializeField] public int mAtk { get; set; }             /// <value> Magical attack </value>
        [field: SerializeField] public int pDef { get; set; }             /// <value> Physical defense </value>
        [field: SerializeField] public int mDef { get; set; }             /// <value> Magical defense </value>
        [field: SerializeField] public int dodge { get; set; }            /// <value> Dodge rating </value>
        [field: SerializeField] public int acc { get; set; }              /// <value> Accuracy rating </value>
        [field: SerializeField] public int tempAcc = 0;                   /// <value> Bonus accuracy accumulated by missing </value>
        [field: SerializeField] public int critChance { get; set; }       /// <value> % chance to crit </value>
        [field: SerializeField] public float critMult { get; set; }       /// <value> Critical damage multiplier </value>
        [field: SerializeField] public Attack[] attacks { get; set; }     /// <value> List of known attacks (length 4) </value>

        private int baseCritChance = 5;                                   /// <value> Base chance of critting </value>
        private int baseAcc = 90;                                         /// <value> Base accuracy rating </value>
        private float baseCritMult = 1.5f;                                /// <value> Base crit attack damage multiplier </value>

        /// <summary>
        /// Initializes character properties
        /// </summary>
        /// <param name="LVL"> Power level </param>
        /// <param name="HP"> Max health points</param>
        /// <param name="MP"> Max mana points </param>
        /// <param name="stats"> STR, DEX, INT, and LUK</param>
        /// <param name="attacks"> List of known attacks (length 4) </param>
        public virtual void Init(int LVL, int HP, int MP, int[] stats, Attack[] attacks) {
            this.LVL = LVL;
            this.STR = stats[0];
            this.DEX = stats[1];
            this.INT = stats[2];
            this.LUK = stats[3];
            this.attacks = attacks; 

            CalculateSecondaryStats(true);
        }
        
        /// <summary>
        /// Calculates secondary stats based off of the 4 primary stats
        /// </summary>
        /// <param name="setCurrent"> Flag for if CHP and CMP should equal new HP and MP values </param>
        private void CalculateSecondaryStats(bool setCurrent = false) {
           HP = (int)(STR * 3 + DEX * 1.5);
           MP = (int)(INT * 2 + LUK * 1.5);
           pAtk = (int)(STR * 0.5 + DEX * 0.25);
           mAtk = (int)(INT * 0.5 + LUK * 0.25); 
           pDef = (int)(STR * 0.1 + DEX * 0.05);
           mDef = (int)(INT * 0.15 + LUK * 0.05);
           dodge = (int)(DEX * 0.2 + LUK * 0.1);
           acc = (int)(DEX * 0.3) + baseAcc;
           critChance = (int)(LUK * 0.1) + baseCritChance;
           critMult = baseCritMult;

           if (critChance > 100) {
               critChance = 100;
           }

           if (setCurrent) {
               CHP = HP;
               CMP = MP;
           }
        }

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
            STR += (int)(LVL * 1.5 * multiplier);
            DEX += (int)(LVL * 1.5 * multiplier);
            INT += (int)(LVL * 1.5 * multiplier);
            LUK += (int)(LVL * 1.5 * multiplier);
            CalculateSecondaryStats(true);
        }

        /// <summary>
        /// Returns the value from an attack, based on the attack's formula
        /// </summary>
        /// <param name="attack"> Attack of object </param>
        /// <returns> Integer amount </returns>
        public int GetAttackValue(Attack attack) {
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
            parser.LocalVariables.Add("pAtk", pAtk);
            parser.LocalVariables.Add("mAtk", mAtk);
            parser.LocalVariables.Add("pDef", pDef);
            parser.LocalVariables.Add("mDef", mDef);

            return (int)parser.Parse(attack.formula);
        }

        /// <summary>
        /// Set each attack's attack value
        /// </summary>
        public void SetAttackValues() {
            foreach(Attack a in attacks) {
                if (a.nameKey != "none_attack") {
                    a.attackValue = GetAttackValue(a);
                }
            }
        }

        /// <summary>
        /// Determines if an attack aimed at this character hits or misses
        /// </summary>
        /// <param name="c"> Other character attacking </param>
        /// <returns> True if attack hits, false otherwise </returns>
        /// <remark> 
        /// Base chance to hit is 90%.
        /// Every time an attack misses, the attacking character gets increasing bonus accuracy,
        /// resetting that bonus back to 0 upon hitting.
        /// </remark>
        protected bool CalculateAttackHit(Character c) {
            int hitChance = baseAcc + c.acc + c.tempAcc - dodge;

            if (hitChance > 100) {
                hitChance = 100;
            }
            else if (hitChance < 0) {
                hitChance = 0;
            }

            bool attackHit = Random.Range(0, 100) < hitChance;

            if (attackHit) {
                c.tempAcc = 0;
            }
            else {
                c.tempAcc += (int)(4 + c.tempAcc);  
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
            return a.attackValue;
        }

        /// <summary>
        /// Calculates damage of an attack against this character after all reductions (armor, skills, etc.)
        /// </summary>
        /// <param name="damage"> Damage amount</param>
        /// <param name="a"> Attack </param>
        /// <returns></returns>
        public int CalculateAttackReductions(int damage, Attack a) {
             if (a.type == AttackConstants.PHYSICAL) {
                damage = damage - pDef;
            }
            else if (a.type == AttackConstants.MAGICAL) {
                damage = damage - mDef; 
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
            Debug.Log("pDef: " + pDef + " mDef: " + mDef + " acc: " + acc + " tempAcc:" + tempAcc + " dodge: " + dodge);
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