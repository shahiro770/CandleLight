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
using Localization;
using Events;
using PanelConstants = Constants.PanelConstants;
using Party;
using PlayerUI;
using System.Collections;
using UnityEngine;

namespace Characters {

    public class PartyMember : Character {
        
        /* external component references */
        public Bar statusPanelHPBar { get; private set; }   /// <value> Visual for health points in status panel </value>
        public Bar statusPanelMPBar { get; private set; }   /// <value> Visual for mana points in status panel </value>
        public Bar partyPanelHPBar { get; private set; }    /// <value> Visual for health points in party panel </value>
        public Bar partyPanelMPBar { get; private set; }    /// <value> Visual for mana points in party panel </value>
        public EXPBar partyPanelEXPBar { get; private set; }   /// <value> Visual for experience points in party panel </value>
        public EXPBar rewardsPanelEXPBar { get; private set; } /// <value> Visual for experience points in rewards panel</value>
        public LocalizedText rewardsPanelLVLText { get; private set; }      /// <value> Visual for LVL in rewards panel</value>
        public PartyMemberDisplay pmdPartyPanel { get; private set; }       /// <value> Visual for party member's status in party panel </value>
        public PartyMemberDisplay pmdRewardsPanel { get; private set; }     /// <value> Visual for party member's status in party panel </value>  

        public string className { get; set; }       /// <value> Warrior, Mage, Archer, or Thief </value>
        public string subClassName { get; set; }    /// <value> Class specializations </value>
        public string memberName { get; set; }      /// <value> Name of the party member </value>
        public string race { get; set; }            /// <value> Human, Lizardman, Undead, etc. </value>
        public int EXP { get; set; }                /// <value> Current amount of experience points </value>
        public int EXPToNextLevel { get; set; }     /// <value> Total experience points to reach next level </value>
        public bool doneEXPGaining { get; private set; } = false;   /// <value> Total experience points to reach next level </value>

        /// <summary>
        /// When a PartyMember GO is instantiated, it needs to have its values initialized
        /// </summary> 
        /// <param name="personalInfo"> className, subClassName, memberName, and race in an array </param>
        /// <param name="LVL"> Power level </param>
        /// <param name="HP"> Health points </param>
        /// <param name="MP"> Mana Points </param>
        /// <param name="attacks"> List of known attacks (length 4)</param>
        public void Init(string[] personalInfo, int LVL, int EXP, int HP, int MP, int[] stats, Attack[] attacks) {
            base.Init(LVL, HP, MP, stats, attacks);
            this.EXP = EXP;
            this.EXPToNextLevel = CalcEXPToNextLevel(LVL);
            this.className = personalInfo[0];
            this.subClassName = personalInfo[1];
            this.memberName = personalInfo[2];
            this.race = personalInfo[3];
        }
        
        /// <summary>
        /// Sets the partyMemberDisplay of this partyMember 
        /// </summary>
        /// <param name="pmd"> PartyMemberDisplay to show this partyMember's information </param>
        /// <param name="panelName"> Name of panel </param>
        /// <param name="HPBar"> HPBar reference </param>
        /// <param name="MPBar"> MPBar reference </param>
        public void SetPartyMemberDisplay(PartyMemberDisplay pmd, string panelName, Bar HPBar, Bar MPBar) {
            this.pmdPartyPanel = pmd;
            SetHPAndMPBar(panelName, HPBar, MPBar);
        }

        public void SetPartyMemberDisplayRewardsPanel(PartyMemberDisplay pmd, string panelName, EXPBar EXPBarRef, LocalizedText LVLTextRef) {
            this.pmdRewardsPanel = pmd;
            SetEXPBar(panelName, EXPBarRef);
            SetLVLText(panelName, LVLTextRef);
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

        public void SetEXPBar(string panelName, EXPBar EXPBar) {
            if (panelName == PanelConstants.REWARDSPANEL) {
                rewardsPanelEXPBar = EXPBar;
            }

            rewardsPanelEXPBar.SetEXPBar(this, EXPToNextLevel, EXP);
        }

        private void SetLVLText(string panelName, LocalizedText LVLText) {
            if (panelName == PanelConstants.REWARDSPANEL) {
                rewardsPanelLVLText = LVLText;
            }

            rewardsPanelLVLText.SetText("LVL " + LVL);
        }

        /// <summary>
        /// Removes the HPBar and MPBar references depending on the panel
        /// </summary>
        /// <param name="panelName"> Name of panel </param>
        public void UnsetHPAndMPBar(string panelName) {
            if (panelName == PanelConstants.STATUSPANEL) {
                statusPanelHPBar = null;
                statusPanelMPBar = null;
            }
            else if (panelName == PanelConstants.PARTYPANEL) {
                partyPanelHPBar = null;
                partyPanelMPBar = null;
            }
        }

        /// <summary>
        /// Sets EXPToNextLevel based off of a math
        /// </summary>
        public int CalcEXPToNextLevel(int LVL) {
            return (10 * (LVL + LVL - 1)) / 2; 
        }

        /// <summary>
        /// Increases EXP and updates visuals that care
        /// Maybe change this to AddEXP
        /// </summary>
        /// <param name="amount"> Amount of EXP gained </param>
        public IEnumerator GainEXP(int amount) {
            doneEXPGaining = false;
            EXP += amount;

            if (EXP >= EXPToNextLevel) {
                int overflow = EXP;       

                while (overflow >= EXPToNextLevel) { // small chance player might level up more than once
                    LVL += 1;
                    overflow -= EXPToNextLevel;
                    if (rewardsPanelEXPBar) { 
                        yield return (StartCoroutine(rewardsPanelEXPBar.SetCurrent(EXPToNextLevel)));
                        yield return new WaitForSeconds(0.5f);
                        EXPToNextLevel = CalcEXPToNextLevel(LVL);
                        rewardsPanelEXPBar.SetMaxAndCurrentImmediate(EXPToNextLevel, 0);
                        rewardsPanelLVLText.SetText("LVL " + LVL);
                    }
                    else {
                        EXPToNextLevel = CalcEXPToNextLevel(LVL);
                    }
                }
                EXP = overflow;
                yield return (StartCoroutine(rewardsPanelEXPBar.SetCurrent(EXP)));
            }
            else {
                yield return (StartCoroutine(rewardsPanelEXPBar.SetCurrent(EXP)));
            }

            doneEXPGaining = true;         
        }

        /// <summary>
        /// Reduce the PartyMember's current health points by a specified amount.false
        /// IEnumerator is used to make calling function wait for its completion
        /// TODO: Cleanup logic so that AddHP manages whether or not the player recieve HP under
        /// various circumstances (e.g. only revival skills can bring a partyMember back from 0 CHP)
        /// </summary> 
        /// <param name="amount"> Amount of health points lost </param>
        /// <param name="isActive"> 
        /// Active party member is the partMember who's information 
        /// is displayed in the status panel, true if active, false otherwise 
        /// </param>
        public void AddHP(int amount) {    
            if (CHP == 0) { // Reviving a dead partyMember if CHP was originally 0 and addHp is allowed
                PartyManager.instance.RegisterPartyMemberAlive(this);
            }

            CHP += amount;

            if (CHP > HP) {
                CHP = HP;
            }

            if (statusPanelHPBar != null) {
                statusPanelHPBar.SetCurrent(CHP);  
            }
            if (EventManager.instance.partyPanel.isOpen == true) {
                partyPanelHPBar.SetCurrent(CHP);
            } 
        }

        public void AddMP(int amount) {
            CMP += amount;

            if (CMP > MP) {
                CMP = MP;
            }
            
            if (statusPanelMPBar != null) {
                statusPanelMPBar.SetCurrent(CMP);  
            }
            if (EventManager.instance.partyPanel.isOpen == true) {
                partyPanelMPBar.SetCurrent(CMP);
            }
        }

        public void Regen() {
            AddHP((int)(Mathf.Ceil((float)HP * 0.05f)));
            AddMP((int)(Mathf.Ceil((float)MP * 0.05f)));
        }

        /// <summary>
        /// Reduce the PartyMember's current health points by a specified amount.false
        /// IEnumerator is used to make calling function wait for its completion
        /// </summary> 
        /// <param name="amount"> Amount of health points lost </param>
        /// <param name="isActive"> 
        /// Active party member is the partMember who's information 
        /// is displayed in the status panel, true if active, false otherwise 
        /// </param>
        public IEnumerator LoseHP(int amount) {
            // some sources such as results will use negative numbers to indicate loss
            amount = Mathf.Abs(amount);
            
            CHP -= amount;

            if (CHP < 0) {
                CHP = 0;
            }
            
            if (statusPanelHPBar != null) {
                statusPanelHPBar.SetCurrent(CHP);  
            }
            if (EventManager.instance.partyPanel.isOpen == true) {
                partyPanelHPBar.SetCurrent(CHP);
                yield return (StartCoroutine(pmdPartyPanel.PlayDamagedAnimation()));
            }
            else {
                yield return new WaitForSeconds(1.3f);
            }

            if (CHP == 0) { // make death more dramatic
                PartyManager.instance.RegisterPartyMemberDead(this);
                yield return new WaitForSeconds(0.75f);
            }
            
            yield break;
        }

        /// <summary>
        /// Reduce the PartyMember's current health points by a specified amount.false
        /// IEnumerator is used to make calling function wait for its completion
        /// </summary> 
        /// <param name="amount"> Amount of health points lost </param>
        /// <param name="isActive"> 
        /// Active party member is the party member who's information 
        /// is displayed in the status panel, true if active, false otherwise 
        /// </param>
        public IEnumerator LoseMP(int amount) {
            CMP -= amount;
            
            if (statusPanelMPBar != null) {
                statusPanelMPBar.SetCurrent(CMP);  
            }
            if (EventManager.instance.partyPanel.isOpen == true) {
                partyPanelMPBar.SetCurrent(CMP);
            }

            yield break;
        }

        /// <summary>
        /// Check if the party member is dead
        /// </summary>
        /// <returns></returns>
        public bool CheckDeath() {
            return CHP == 0;
        }

        /// <summary>
        /// Reduce MP or HP after an attack is used depending on the attack's costs
        /// </summary>
        /// <param name="costType"> HP or MP </param>
        /// <param name="cost"> Amount to be lost </param>
        /// <returns> IEnumerator to pay cost before attack animations play </returns>
        public IEnumerator PayAttackCost(string costType, int cost) {
            if (costType == "MP") {
                yield return StartCoroutine(LoseMP(cost));
            } 
            else if (costType == "HP") {
                yield return StartCoroutine(LoseHP(cost));
            }
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
