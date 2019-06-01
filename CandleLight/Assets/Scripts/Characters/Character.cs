/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 11, 2019
* 
* The Character class is used to store information about characters that can act in combat. 
*
*/

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
        /// Lose health points
        /// </summary>
        /// <param name="amount"> Amount lost </param>
        /// <returns> Yields so that function may be called synchronously </returns>
        public virtual IEnumerator LoseHP(int amount) {
            CHP -= amount;

            yield break;
        }

        /// <summary>
        /// Lose mana points
        /// </summary>
        /// <param name="amount"> Amount lost </param>
        /// <returns> Yields so that function may be called synchronously </returns>
        public virtual IEnumerator LoseMP(int amount) {
            CMP -= amount;

            yield break;
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
            Debug.Log("George");
        }

        /// <summary>
        /// Logs attacks to the console for debugging
        /// </summary>
        public virtual void LogAttacks() {
            Debug.Log("1 " + attacks[0].nameKey + " 2 " + attacks[1].nameKey  + " 3 " + attacks[2].nameKey  + " 4 " + attacks[3].nameKey );
        }
    }
}