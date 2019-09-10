/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 11, 2019
* 
* The Character class is used to store information about characters that can act in combat. 
*
*/

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
        [field: SerializeField] public Attack[] attacks { get; set; }     /// <value> List of known attacks (length 4) </value>

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
            this.HP = HP;
            this.CHP = HP;
            this.MP = MP;
            this.CMP = MP;
            this.STR = stats[0];
            this.DEX = stats[1];
            this.INT = stats[2];
            this.LUK = stats[3];
            this.attacks = attacks; 
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
            STR += LVL * 2 * multiplier;
            DEX += LVL * 2 * multiplier;
            INT += LVL * 2 * multiplier;
            LUK += LVL * 2 * multiplier;
            HP += (int)((STR * 0.5) + (DEX * 0.5) * multiplier);
            MP += (int)((INT * 0.5) + (LUK * 0.5) * multiplier);
            CHP = HP;
            CMP = MP;
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
        /// Logs stats to console for debugging
        /// </summary>
        public virtual void LogStats() {
            Debug.Log(LVL + " " + HP + " " + MP + " " + STR + " " + DEX + " " + INT + " " + LUK);
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