/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 11, 2019
* 
* The Monster class is used to store and manipulate information about the Monster. 
* It is always attached to a Monster gameObject.
*
*/

using AssetManagers;
using Combat;
using General;
using PlayerUI;
using System.Collections;
using System.Collections.Generic;
using UIEffects;
using UnityEngine;
using UnityEngine.UI;

namespace Characters {

    public class Monster : Character {
        
        /* external component references */
        public MonsterDisplay md;

        [field: SerializeField] public string monsterArea { get; private set; }         /// <value> Area where monster can be found </value>
        [field: SerializeField] public string monsterSize { get; private set; }         /// <value> String constant describing size of monster's sprite </value>
        [field: SerializeField] public string monsterNameID { get; private set; }       /// <value> NameID as referenced in database </value>
        [field: SerializeField] public string monsterSpriteName { get; private set; }   /// <value> Name of monster's sprite as referenced in resources </value>
        [field: SerializeField] public string monsterDisplayName { get; private set; }  /// <value> Monster name <value>
        [field: SerializeField] public string monsterAI { get; private set; }           /// <value> Monster's behaviour in combat </value>
        [field: SerializeField] public int multiplier { get; private set; }             /// <value> Multipler to EXP and WAX rewarded (due to being a boss, variant, etc) </value>
        [field: SerializeField] public int EXP { get; private set; }                    /// <value> EXP monster gives on defeat </value>
        [field: SerializeField] public int WAX { get; private set; }                    /// <value> WAX monster gives on defeat </value>
        [field: SerializeField] public int attackNum { get; private set; } = 0;         /// <value> Number of attacks monster has (max 4) </value>
        [field: SerializeField] public int selectedAttackIndex { get; private set; }    /// <value> Index of attack selected </value>
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
        /// <param name="LVL"> Power level </param>
        /// <param name="multiplier"> Multiplier on rewards monster gives such as WAX and EXP </param>
        /// <param name="HP"> Max health points </param>
        /// <param name="MP"> Max mana points </param>
        /// <param name="stats"> STR, DEX, INT, LUK </param>
        /// <param name="attacks"> List of known attacks (length 4) </param>
        public IEnumerator Init(string monsterNameID, string monsterSpriteName, string monsterDisplayName, string monsterArea, 
        string monsterSize, string monsterAI, int LVL, int multiplier, int HP, int MP, int[] stats, Attack[] attacks) {
            base.Init(LVL, HP, MP, stats, attacks);            
            this.monsterNameID = monsterNameID;
            this.monsterSpriteName = monsterSpriteName;
            this.monsterDisplayName = monsterDisplayName;
            this.monsterArea = monsterArea;
            this.monsterAI = monsterAI;
            this.multiplier = multiplier;
            this.EXP = (LVL * LVL) * this.multiplier;
            this.WAX = LVL * this.multiplier;
            this.monsterSize = monsterSize;

            foreach (Attack a in attacks) {
                if (a.name != "none") {
                    attackNum++;
                }
            }

            StartCoroutine(md.Init(this));

            this.isReady = true;
            yield break;
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
            
            yield return StartCoroutine(md.DisplayLostHP(amount, animationClipName));
        }

        /// <summary>
        /// Check if monster is dead
        /// </summary>
        /// <returns> True if monster is dead, false otherwise </returns>
        public bool CheckDeath() {
            return CHP == 0;
        }

        #endregion

        /// <summary>
        /// Logs information for debugging
        /// </summary>
        public override void LogStats() {
            Debug.Log(monsterDisplayName);
            base.LogStats();
        }

        /// <summary>
        /// Logs monster's display name for debugging
        /// </summary>
        public override void LogName() {
            Debug.Log("Name " + monsterDisplayName);
        }
    }
}
