/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 11, 2019
* 
* The PartyMemberVisualController class is used to send updates to all visuals related
* to a partyMember class.
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

    public class PartyMemberVisualController : MonoBehaviour {
        
        /* external component references */
         public Bar statusPanelHPBar { get; private set; }   /// <value> Visual for health points in status panel </value>
        public Bar statusPanelMPBar { get; private set; }   /// <value> Visual for mana points in status panel </value>
        public Bar partyPanelHPBar { get; private set; }    /// <value> Visual for health points in party panel </value>
        public Bar partyPanelMPBar { get; private set; }    /// <value> Visual for mana points in party panel </value>
        public EXPBar partyPanelEXPBar { get; private set; }   /// <value> Visual for experience points in party panel </value>
        public EXPBar rewardsPanelEXPBar { get; private set; } /// <value> Visual for experience points in rewards panel</value>
        public LocalizedText rewardsPanelLVLText { get; private set; }      /// <value> Visual for LVL in rewards panel</value>
        public PartyMemberDisplay pmdPartyPanel { get; private set; }       /// <value> Visual for partyMember's status in party panel </value>
        public PartyMemberDisplay pmdRewardsPanel { get; private set; }     /// <value> Visual for partyMember's status in party panel </value>  
        
        public Sprite partyMemberSprite { get; private set; }   /// <value> Icon sprite of the partyMember </value>
        public Color32 partyMemberColour { get; private set; }  /// <value> Theme colour of partyMember </value>

        private PartyMember pm;     /// <value> PartyMember object visual controller is referring to </value>

        /// <summary>
        /// Initializes with basic information using a given partyMember
        /// </summary>
        /// <param name="pm"> partyMember object </param>
        public void Init(PartyMember pm) {
            this.pm = pm;
            partyMemberSprite = Resources.Load<Sprite>("Sprites/Combat/PartyMemberIcons/" + pm.className + "PMIcon");
            
            if (pm.className == "Warrior") {
                partyMemberColour = new Color32(189, 29, 0, 255);
            }
            else if (pm.className == "Mage") {
                partyMemberColour = new Color32(0, 152, 220, 255);
            }
            else if (pm.className == "Archer") {
                partyMemberColour = new Color32(90, 197, 79, 255);
            }
            else if (pm.className == "Thief") {
                partyMemberColour = new Color32(255, 235, 87, 255);
            }
        
        }
        
        /// <summary>
        /// Sets the partyMemberDisplay of displayed partyMember 
        /// </summary>
        /// <param name="pmd"> PartyMemberDisplay to show this partyMember's information </param>
        /// <param name="panelName"> Name of panel </param>
        /// <param name="HPBar"> HPBar reference </param>
        /// <param name="MPBar"> MPBar reference </param>
        public void SetPartyMemberDisplay(PartyMemberDisplay pmd, string panelName, Bar HPBar, Bar MPBar) {
            this.pmdPartyPanel = pmd;
            SetHPAndMPBar(panelName, HPBar, MPBar);
        }

        /// <summary>
        /// Sets the partyMemberDisplay for the rewards panel of displayed partyMember
        /// </summary>
        /// <param name="pmd"> PartyMemberDisplay object </param>
        /// <param name="panelName"> String </param>
        /// <param name="EXPBarRef"> EXPBar object </param>
        /// <param name="LVLTextRef"> LocalizedText object </param>
        public void SetPartyMemberDisplayRewardsPanel(PartyMemberDisplay pmd, string panelName, EXPBar EXPBarRef, LocalizedText LVLTextRef) {
            this.pmdRewardsPanel = pmd;
            SetEXPBar(panelName, EXPBarRef);
            SetLVLText(panelName, LVLTextRef);
        }

        /// <summary>
        /// Sets the HP and MP bars of a panel to reflect displayed partyMember's HP and MP
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

            HPBar.SetMaxAndCurrent(pm.HP, pm.CHP);
            MPBar.SetMaxAndCurrent(pm.MP, pm.CMP);
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
        /// Sets the EXP bar reference
        /// </summary>
        /// <param name="panelName"> Name of panel </param>
        /// <param name="EXPBar"> EXPBar object reference </param>
        public void SetEXPBar(string panelName, EXPBar EXPBar) {
            if (panelName == PanelConstants.REWARDSPANEL) {
                rewardsPanelEXPBar = EXPBar;
            }

            rewardsPanelEXPBar.SetEXPBar(pm.EXPToNextLVL, pm.EXP);
        }

        /// <summary>
        /// Sets the LVLText reference
        /// </summary>
        /// <param name="panelName"> Name of panel </param>
        /// <param name="LVLText"> Localized text object  </param>
        private void SetLVLText(string panelName, LocalizedText LVLText) {
            if (panelName == PanelConstants.REWARDSPANEL) {
                rewardsPanelLVLText = LVLText;
            }

            rewardsPanelLVLText.SetText("LVL " + pm.LVL);
        }

        /// <summary>
        /// Sets the max and current amounts of an EXP bar.
        /// If the current amount  equals the max amount, set the current amount to 0
        /// </summary>
        /// <param name="maxAmount"> Positve int </param>
        /// <param name="currentAmount"> Positive int less than or equal to maxAmount</param>
        /// <returns></returns>
        public IEnumerator DisplayEXPChange(int maxAmount, int currentAmount) {
            if (rewardsPanelEXPBar) { 
                if (currentAmount == maxAmount) {
                    yield return (StartCoroutine(rewardsPanelEXPBar.SetCurrent(maxAmount)));
                    yield return new WaitForSeconds(0.5f);
                    rewardsPanelEXPBar.SetMaxAndCurrentImmediate(pm.EXPToNextLVL, 0);
                    rewardsPanelLVLText.SetText("LVL " + pm.LVL);
                }
                else {
                    yield return (StartCoroutine(rewardsPanelEXPBar.SetCurrent(currentAmount)));
                }
            }
        }

        /// <summary>
        /// Updates the current fill amount on all HPBars to show HP being added or lost
        /// </summary>
        /// <param name="isLoss"> Flag for if damaged animation should play </param>
        /// <returns> IEnumerator for animations </returns>
        public IEnumerator DisplayHPChange(bool isLoss) {
            if (statusPanelHPBar != null) {
                statusPanelHPBar.SetCurrent(pm.CHP);  
            }
            if (EventManager.instance.partyPanel.isOpen == true) {
                partyPanelHPBar.SetCurrent(pm.CHP);
                if (isLoss) {
                    yield return (StartCoroutine(pmdPartyPanel.PlayDamagedAnimation()));
                    if (pm.CHP == 0) {
                        yield return new WaitForSeconds(0.75f);
                    }
                }
                else {
                    yield return new WaitForSeconds(1.3f);
                }
            } 
        }

        /// <summary>
        /// Updates the current fill amount on all MPBars to show MP being added or lost
        /// </summary>
        /// <param name="isLoss"> Flag for if damaged animation should play (does nothing right now) </param>
        /// <returns> IEnumerator for animations </returns>
        public IEnumerator DisplayMPChange(bool isLoss) {
            if (statusPanelMPBar != null) {
                statusPanelMPBar.SetCurrent(pm.CMP);  
            }
            if (EventManager.instance.partyPanel.isOpen == true) {
                partyPanelMPBar.SetCurrent(pm.CMP);
            }

            yield break;
        }
    }
}
