/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 11, 2019
* 
* The PartyMemberData class is used to store information about a member of the party for saving 
* Serialization is a pain.
*
*/

using Attack = Combat.Attack;
using Characters;
using Items;
using Skills;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Party {

    [System.Serializable]
    public class PartyMemberData {

        public int LVL;             /// <value> Power level </value>
        public int CHP;             /// <value> Current health points </value>
        public int CMP;             /// <value> Current mana points </value>
        public int baseSTR;         /// <value> Strength </value>
        public int baseDEX;         /// <value> Dexterity </value>
        public int baseINT;         /// <value> Intelligence </value>
        public int baseLUK;         /// <value> Luck </value>
        
        public string className;    /// <value> Warrior, Mage, Archer, or Rogue </value>
        public string subClassName; /// <value> Class specializations </value>
        public string pmName;       /// <value> Name of the partyMember </value>
        public string race;         /// <value> Human, Lizardman, Undead, etc. </value>
        public int EXP;             /// <value> Current amount of experience points </value>
        public int skillPoints;    

        public Attack[] attacks;    /// <value> List of known attacks (length 4) </value>
        public Skill[] skills;
        public ItemData[] equippedGear = new ItemData[3];
        public ItemData[] equippedCandles = new ItemData[3];

        public PartyMemberData(PartyMember pm) {
            LVL = pm.LVL;
            CHP = pm.CHP;
            CMP = pm.CMP;
            baseSTR = pm.baseSTR;
            baseDEX = pm.baseDEX;
            baseINT = pm.baseINT;
            baseLUK = pm.baseLUK;
            attacks = pm.attacks;

            this.EXP = pm.EXP;
            this.className = pm.className;
            this.subClassName = pm.subClassName;
            this.race = pm.race;
            this.skills = pm.skills;
            this.skillPoints = pm.skillPoints;

            if (pm.weapon != null) {
                equippedGear[0] = pm.weapon.GetItemData();
            }
            if (pm.secondary != null) {
                equippedGear[1] = pm.secondary.GetItemData();
            }
            if (pm.armour != null) {
                equippedGear[2] = pm.armour.GetItemData();
            }

            for (int i = 0 ; i < equippedCandles.Length; i++) {
                if (pm.activeCandles[i] != null) {
                    equippedCandles[i] = pm.activeCandles[i].GetItemData();
                }
            }
        }

        /// <summary>
        /// PartyMember data constructor for when the player is in the tutorial
        /// (Contiuning the game from the tutorial will not have saved the second partyMember
        /// as it they aren't in the party at the start)
        /// </summary>
        /// <param name="className"></param>
        public PartyMemberData(string className) {
            this.className = className;
        }
    }
}