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
using Events;
using Localization;
using PanelConstants = Constants.PanelConstants;
using Party;
using PlayerUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Characters {

    public class PartyMemberVisualController : MonoBehaviour {
        
        /* external component references */
        public Bar statusPanelHPBar { get; private set; }   /// <value> Visual for health points in status panel </value>
        public Bar statusPanelMPBar { get; private set; }   /// <value> Visual for mana points in status panel </value>
        public Bar partyPanelHPBar { get; private set; }    /// <value> Visual for health points in party panel </value>
        public Bar partyPanelMPBar { get; private set; }    /// <value> Visual for mana points in party panel </value>
        public Bar statsPanelHPBar { get; private set; }    /// <value> Visual for health points in stats panel </value>
        public Bar statsPanelMPBar { get; private set; }    /// <value> Visual for mana points in stats panel </value>
        public EventDescription eventDescription;           /// <value> Display that describes the event in text </value>
        public EXPBar statsPanelEXPBar { get; private set; }   /// <value> Visual for experience points in stats panel </value>
        public EXPBar rewardsPanelEXPBar { get; private set; } /// <value> Visual for experience points in rewards panel</value>
        public GearPanel gearPanel;     /// <value> gearPanel reference </value>
        public StatusPanel statusPanel; /// <value> statusPanel reference </value>
        public PartyPanel partyPanel;   /// <value> partyPanel reference </value>
        public SkillsPanel skillsPanel; /// <value> skillsPanel reference </value>
        public LocalizedText rewardsPanelLVLText { get; private set; }      /// <value> Visual for LVL in rewards panel</value>
        public PartyMemberDisplay pmdPartyPanel { get; private set; }       /// <value> Visual for partyMember in party panel </value>
        public PartyMemberDisplay pmdRewardsPanel { get; private set; }     /// <value> Visual for partyMember in rewardsPanel </value>  
        public PartyMemberDisplay pmdSkillsPanel { get; private set; }      /// <value> Visual for partyMember in skillsPanel </value>  
        public ItemDisplay weapon;      /// <value> weapon reference </value>
        public ItemDisplay secondary;   /// <value> secondary reference </value>
        public ItemDisplay armour;      /// <value> armour reference </value>

        public Sprite partyMemberSprite { get; private set; }   /// <value> Icon sprite of the partyMember </value>
        public Sprite[] skillSprites = new Sprite[12];
        public Color32 partyMemberColour { get; private set; }  /// <value> Theme colour of partyMember </value>
        
        private PartyMember pm;      /// <value> PartyMember object visual controller is referring to </value>
        private int damageTaken = 0; /// <value> Amount of damage taken to display via the eventDescription </value>
        private bool isCrit = false; /// <value> Flag for if eventDescription will mention the attack dealt a critical hit </value>

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

            for (int i = 0; i < pm.skills.Length; i++) {
                if (pm.skills[i] != null) {    // temporary until all 12 skills for each class get filled out
                    skillSprites[i] = Resources.Load<Sprite>("Sprites/Skills/" + pm.className + "/" + pm.skills[i].name);
                }
            }
        }
        
        /// <summary>
        /// Sets the partyMemberDisplay of displayed partyMember.
        /// Also initializes the eventDescription component reference, which is always
        /// guaranteed to happen as the partyMemberDisplay will always be initialized on moving
        /// to the Area scene.
        /// </summary>
        /// <param name="pmd"> PartyMemberDisplay to show this partyMember's information </param>
        /// <param name="panelName"> Name of panel </param>
        /// <param name="HPBar"> HPBar reference </param>
        /// <param name="MPBar"> MPBar reference </param>
        public void SetPartyMemberDisplay(PartyMemberDisplay pmd, string panelName, Bar HPBar, Bar MPBar) {
            this.pmdPartyPanel = pmd;
            SetHPAndMPBar(panelName, HPBar, MPBar);
            SetExternalDisplayComponents();
        }

        /// <summary>
        /// Sets the partyMemberDisplay for the rewards panel
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
        /// Sets the partyMemberDisplay for the skillsPanel
        /// </summary>
        /// <param name="pmd"></param>
        public void SetPartyMemberDisplaySkillsPanel(PartyMemberDisplay pmd) {
            this.pmdSkillsPanel = pmd;
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
            else if (panelName == PanelConstants.STATSPANEL) {
                statsPanelHPBar = HPBar;
                statsPanelMPBar = MPBar;
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
            else if (panelName == PanelConstants.STATSPANEL) {
                statsPanelHPBar = null;
                statsPanelMPBar = null;
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
            else if (panelName == PanelConstants.STATSPANEL) {
                statsPanelEXPBar = EXPBar;
            }

            EXPBar.SetEXPBar(pm.EXPToNextLVL, pm.EXP);
        }

        /// <summary>
        /// Removes the EXPBar reference from a panel
        /// </summary>
        /// <param name="panelName"></param>
        public void UnsetEXPBar(string panelName) {
            if (panelName == PanelConstants.REWARDSPANEL) {
                rewardsPanelEXPBar = null;
            }
            else if (panelName == PanelConstants.STATSPANEL) {
                statsPanelEXPBar = null;
            }
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
        /// Sets references to components that will display information about a partyMember
        /// </summary>
        public void SetExternalDisplayComponents() {
            eventDescription = EventManager.instance.eventDescription;
            partyPanel = EventManager.instance.partyPanel;
            gearPanel = EventManager.instance.gearPanel;
            statusPanel = EventManager.instance.statusPanel;
            skillsPanel = EventManager.instance.skillsPanel;
        }

        /// <summary>
        /// Sets the partyMember's equipped gear
        /// </summary>
        public void SetEquippedGear() {
            weapon = gearPanel.weaponSlot.currentItemDisplay;
            secondary = gearPanel.secondarySlot.currentItemDisplay;
            armour = gearPanel.armourSlot.currentItemDisplay;
        }

        /// <summary>
        /// Sets the damage taken and if the attack was a crit
        /// </summary>
        /// <param name="damageTaken"> Amount of damage taken </param>
        /// <param name="isCrit"> Flag for if attack that this character was a crit </param>
        public void SetDamageTaken(int damageTaken, bool isCrit) {
            this.damageTaken = damageTaken;
            this.isCrit = isCrit;
        }

        /// <summary>
        /// Returns an array of stats
        /// </summary>
        /// <returns></return-s>
        public int[] GetStats() {
            return new int[] { pm.LVL, pm.STR, pm.DEX, pm.INT, pm.LUK, 
            pm.PATK, pm.MATK, pm.PDEF, pm.MDEF, pm.ACC, pm.DOG, pm.critChance, pm.EXPToNextLVL, pm.EXP };
        }

        /// <summary>
        /// Sets the stored partyMember as the active partyMember
        /// </summary>
        public void SetActivePartyMember() {
            PartyManager.instance.SetActivePartyMember(pm);
        }
        
        /// <summary>
        /// Changes all panels to show the stored partyMember's information
        /// </summary>
        public void DisplayActivePartyMember() {
            if (partyPanel.isOpen) {
                partyPanel.DisplayActivePartyMember(pmdPartyPanel);
            }
            statusPanel.DisplayPartyMember(this);
            if (gearPanel.isOpen) {
                gearPanel.Init(this);
            }
            if (skillsPanel.isOpen) {
                skillsPanel.Init();
            }
        }

        /// <summary>
        /// Gets the name of this partyMember
        /// </summary>
        /// <returns></returns>
        public string GetPartyMemberName() {
            return pm.pmName;
        }

        /// <summary>
        /// Updates the HP and MP bars to have the correct max amounts
        /// </summary>
        /// <remark>
        /// Primary use is to correct fill amounts upon leveling up
        /// </remark>
        public void UpdateHPAndMPBars() {
            if (statusPanelHPBar != null) {
                statusPanelHPBar.SetMaxAndCurrent(pm.HP, pm.CHP);
                statusPanelMPBar.SetMaxAndCurrent(pm.MP, pm.CMP);
            }
            if (partyPanelHPBar != null) {
                partyPanelHPBar.SetMaxAndCurrent(pm.HP, pm.CHP);
                partyPanelMPBar.SetMaxAndCurrent(pm.MP, pm.CMP);
            }
        }

        /// <summary>
        /// Updates the statsPanel to show the correct stats
        /// </summary>
        public void UpdateStats() {
            if (statsPanelEXPBar != null) {
                pmdPartyPanel.UpdateStatsPanel();
            }
            UpdateHPAndMPBars();
        }

        /// <summary>
        /// Sets the max and current amounts of an EXP bar.
        /// If the current amount  equals the max amount, set the current amount to 0
        /// </summary>
        /// <param name="maxAmount"> Positve int </param>
        /// <param name="currentAmount"> Positive int less than or equal to maxAmount</param>
        /// <returns></returns>
        public IEnumerator DisplayEXPChange(int maxAmount, int currentAmount, int LVLtoDisplay) {
            if (rewardsPanelEXPBar && rewardsPanelEXPBar.gameObject.activeInHierarchy) { 
                if (currentAmount == maxAmount) {
                    yield return (StartCoroutine(rewardsPanelEXPBar.SetCurrent(maxAmount)));
                    yield return new WaitForSeconds(0.5f);
                    rewardsPanelEXPBar.SetMaxAndCurrentImmediate(pm.EXPToNextLVL, 0);
                    rewardsPanelLVLText.SetText("LVL " + LVLtoDisplay);
                }
                else {
                    yield return (StartCoroutine(rewardsPanelEXPBar.SetCurrent(currentAmount)));
                }
            }
            else if (statsPanelEXPBar && statsPanelEXPBar.gameObject.activeSelf) {
                UpdateStats();
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
            if (statsPanelHPBar != null) {
                statsPanelHPBar.SetCurrent(pm.CHP);
            }
            if (EventManager.instance.partyPanel.isOpen) {
                partyPanelHPBar.SetCurrent(pm.CHP);
                if (isLoss && CombatManager.instance.inCombat == true) {
                    if (isCrit) {
                        eventDescription.SetPMDamageCritText(pm, damageTaken);
                        yield return (StartCoroutine(pmdPartyPanel.PlayCritDamagedAnimation()));
                        isCrit = false;
                        damageTaken = 0;
                    }
                    else {
                        eventDescription.SetPMDamageText(pm, damageTaken);
                        yield return (StartCoroutine(pmdPartyPanel.PlayDamagedAnimation()));
                    }
                    if (pm.CHP == 0) {
                        yield return new WaitForSeconds(0.5f);
                    }
                }
            }
            else {
                if (isLoss && CombatManager.instance.inCombat) {
                    eventDescription.SetPMDamageText(pm, damageTaken);
                }
                yield return new WaitForSeconds(0.75f);
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
            if (statsPanelMPBar != null) {
                statsPanelMPBar.SetCurrent(pm.CMP);
            }
            if (EventManager.instance.partyPanel.isOpen == true) {
                partyPanelMPBar.SetCurrent(pm.CMP);
            }

            yield break;
        }

        /// <summary>
        /// Plays animation when an attack is dodged and has eventDescription write it out
        /// </summary>
        /// <returns> IEnumerator cause animations </returns>
        public IEnumerator DisplayAttackDodged() {
            eventDescription.SetPMDodgeText(pm);
            if (EventManager.instance.partyPanel.isOpen == true) {
                yield return (StartCoroutine(pmdPartyPanel.PlayDodgedAnimation()));
            }
            else {
                yield return new WaitForSeconds(0.75f);
            }
        }

        // /// <summary>
        // /// Displays the animatino of an effect (usually buffs)
        // /// </summary>
        // /// <param name="animationClipName"> Name of animation clip to set </param>
        // /// <returns></returns>
        // public IEnumerator DisplayEffect(string animationClipName) {
        //     yield return (StartCoroutine(pmdPartyPanel.PlayStatusEffectAnimation(animationClipName)));
        // }

        /// <summary>
        /// Tell partyMemberDisplay to add a statusEffect
        /// </summary>
        /// <param name="se"></param>
        public void AddStatusEffectDisplay(StatusEffect se) {
            pmdPartyPanel.AddStatusEffectDisplay(se);
        }

        /// <summary>
        /// Plays status effect animations that trigger during the PartyMemberCleanUp 
        /// </summary>
        /// <param name="animationsToPlay"></param>
        public void DisplayCleanUpStatusEffects(int[] animationsToPlay) {
            pmdPartyPanel.PlayCleanUpStatusEffectAnimations(animationsToPlay);
        }
    }
}
