/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 11, 2019
* 
* The PartyMember class is used to store information about a member of the party. 
* It is always attached to a PartyMember GameObject.
*
*/

using Combat;
using PanelConstants = Constants.PanelConstants;
using PlayerUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Characters {

    public class PartyMember : Character {
        
        private Bar statusPanelHPBar { get; set; }  /// <value> Visual for health points in status panel </value>
        private Bar statusPanelMPBar { get; set; }  /// <value> Visual for mana points in status panel </value>
        private Bar partyPanelHPBar { get; set; }   /// <value> Visual for health points in party panel </value>
        private Bar partyPanelMPBar { get; set; }   /// <value> Visual for mana points in party panel </value>
        public string className { get; set; }       /// <value> Warrior, Mage, Archer, or Thief </value>
        public string subClassName { get; set; }    /// <value> Class specializations </value>
        public string memberName { get; set; }      /// <value> Name of the party member </value>
        public string race { get; set; }            /// <value> Human, Lizardman, Undead, etc. </value>

        /// <summary>
        /// When a PartyMember GO is instantiated, it needs to have its values initialized
        /// </summary> 
        /// <param name="personalInfo"> className, subClassName, memberName, and race in an array </param>
        /// <param name="LVL"> Power level </param>
        /// <param name="HP"> Health points </param>
        /// <param name="MP"> Mana Points </param>
        /// <param name="attacks"> List of known attacks (length 4)</param>
        public void Init(string[] personalInfo, int LVL, int HP, int MP, int[] stats, Attack[] attacks) {
            base.Init(LVL, HP, MP, stats, attacks);
            this.className = personalInfo[0];
            this.subClassName = personalInfo[1];
            this.memberName = personalInfo[2];
            this.race = personalInfo[3];
        }

        /// <summary>
        /// Sets the HP and MP bars of a panel to reflect this partyMember's HP and MP
        /// </summary>
        /// <param name="panel"> Name of panel </param>
        /// <param name="HPBar"> HPBar reference </param>
        /// <param name="MPBar"> MPBar reference </param>
        public void SetHPAndMPBar(string panelName, Bar HPBar, Bar MPBar) {
            if (panelName == PanelConstants.STATUSPANEL) {
                statusPanelHPBar = HPBar;
                statusPanelMPBar = MPBar;
            }
            else if (panelName == PanelConstants.PARTYPANEL) {
                partyPanelHPBar = HPBar;
                partyPanelMPBar = MPBar;
            }

            HPBar.SetMaxAndCurrent(HP, CHP);
            MPBar.SetMaxAndCurrent(MP, CMP);
        }

        /// <summary>
        /// Reduce the PartyMember's current health points by a specified amount.false
        /// IEnumerator is used to make calling function wait for its completion
        /// </summary> 
        /// <param name="amount"> Amount of health points lost </param>
        /// <param name="isActive"> Active party member is the one who's information 
        /// is displayed in the status panel, true if active, false otherwise </param>
        public IEnumerator LoseHP(int amount, bool isActive) {
            CHP -= amount;
            if (CHP < 0) {
                CHP = 0;
            }
            
            if (isActive) {
                statusPanelHPBar.SetCurrent(CHP);  // update visually
            }
            partyPanelHPBar.SetCurrent(CHP);

            yield break;
        }

        /// <summary>
        /// Reduce the PartyMember's current health points by a specified amount.false
        /// IEnumerator is used to make calling function wait for its completion
        /// </summary> 
        /// <param name="amount"> Amount of health points lost </param>
        /// <param name="isActive"> Active party member is the one who's information 
        /// is displayed in the status panel, true if active, false otherwise </param>
        public IEnumerator LoseMP(int amount, bool isActive) {
            CMP -= amount;
            
            if (isActive) {
                statusPanelMPBar.SetCurrent(CMP);  // update visually
            }
            partyPanelMPBar.SetCurrent(CMP);

            yield break;
        }

        /// <summary>
        /// Reduce MP or HP after an attack is used depending on the attack's costs
        /// </summary>
        /// <param name="costType"> HP or MP </param>
        /// <param name="cost"> Amount to be lost </param>
        /// <returns> IEnumerator to pay cost before attack animations play </returns>
        public IEnumerator PayAttackCost(string costType, int cost) {
            if (costType == "MP") {
                yield return StartCoroutine(LoseMP(cost , true));
            } 
            else if (costType == "HP") {
                yield return StartCoroutine(LoseHP(cost , true));
            }
        }

        /// <summary>
        /// Check if the party member is dead
        /// </summary>
        /// <returns></returns>
        public bool CheckDeath() {
            return CHP == 0;
        }

        /// <summary>
        /// Log stats informaton about the PartyMember for debugging
        /// </summary> 
        public override void LogStats() {
            base.LogStats();
        }

        /// <summary>
        /// Log party member's class name
        /// </summary> 
        public override void LogName() {
            Debug.Log("PartyMemberName " + className);
        }
    }
}
