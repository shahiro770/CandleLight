/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 11, 2019
* 
* The PartyMemberVisualController class is used to send updates to all visuals related
* to a partyMember class.
*
*/

using Audio;
using ClassConstants = Constants.ClassConstants;
using Combat;
using Events;
using GameManager = General.GameManager;
using Localization;
using PanelConstants = Constants.PanelConstants;
using Party;
using PlayerUI;
using System.Collections;
using UnityEngine;

namespace Characters {

    public class PartyMemberVisualController : MonoBehaviour {
        
        /* external component references */
        public Bar statusPanelHPBar { get; private set; }       /// <value> Visual for health points in status panel </value>
        public Bar statusPanelMPBar { get; private set; }       /// <value> Visual for mana points in status panel </value>
        public Bar partyPanelHPBar { get; private set; }        /// <value> Visual for health points in party panel </value>
        public Bar partyPanelMPBar { get; private set; }        /// <value> Visual for mana points in party panel </value>
        public Bar statsPanelHPBar { get; private set; }        /// <value> Visual for health points in stats panel </value>
        public Bar statsPanelMPBar { get; private set; }        /// <value> Visual for mana points in stats panel </value>
        public EventDescription eventDescription;               /// <value> Display that describes the event in text </value>
        public EXPBar statsPanelEXPBar { get; private set; }   /// <value> Visual for experience points in stats panel </value>
        public EXPBar rewardsPanelEXPBar { get; private set; } /// <value> Visual for experience points in rewards panel</value>
        public ActionsPanel actionsPanel;       /// <value> actionsPanel reference </value>
        public GearPanel gearPanel;             /// <value> gearPanel reference </value>
        public CandlesPanel candlesPanel;       /// <value> candlesPanel reference </value>
        public StatusPanel statusPanel;         /// <value> statusPanel reference </value>
        public PartyPanel partyPanel;           /// <value> partyPanel reference </value>
        public SkillsPanel skillsPanel;         /// <value> skillsPanel reference </value>
        public TabManager utilityTabManager;    /// <value> right tabManager reference </value>
        public LocalizedText rewardsPanelLVLText { get; private set; }      /// <value> Visual for LVL in rewards panel</value>
        public PartyMemberDisplay pmdPartyPanel { get; private set; }       /// <value> Visual for partyMember in party panel </value>
        public PartyMemberDisplay pmdRewardsPanel { get; private set; }     /// <value> Visual for partyMember in rewardsPanel </value>  
        public PartyMemberDisplay pmdSkillsPanel { get; private set; }      /// <value> Visual for partyMember in skillsPanel </value>  
        public ItemDisplay weapon;          /// <value> weapon reference </value>
        public ItemDisplay secondary;       /// <value> secondary reference </value>
        public ItemDisplay armour;          /// <value> armour reference </value>
        public ItemDisplay[] activeCandles; /// <value> candles reference </value>

        public Sprite partyMemberSprite { get; private set; }   /// <value> Icon sprite of the partyMember </value>
        public Sprite[] skillSprites = new Sprite[12];
        public Color32 partyMemberColour { get; private set; }  /// <value> Theme colour of partyMember </value>
        public bool isAnimating = false;   /// <value> Flag for if this pmvc's pmd is still in an animation </value>

        private PartyMember pm;         /// <value> PartyMember object visual controller is referring to </value>
        private int attackAmount = 0;   /// <value> Amount of damage taken/ healed to display via the eventDescription </value>
        private bool isCrit = false;    /// <value> Flag for if eventDescription will mention the attack dealt a critical hit/heal </value>

        /// <summary>
        /// Initializes with basic information using a given partyMember
        /// </summary>
        /// <param name="pm"> partyMember object </param>
        public void Init(PartyMember pm) {
            this.pm = pm;
            activeCandles = new ItemDisplay[3]; // due to object intialization orders, if this isnt dont here, it'll just be null
            if (PartyManager.instance.GetClassCount(pm.className) == 0) {   // don't show alternate colour scheme if its the first partyMember to be added
                partyMemberSprite = Resources.Load<Sprite>("Sprites/Classes/" + char.ToUpper(pm.className[0]) + pm.className.Substring(1) + "Icon");
                if (pm.className == ClassConstants.WARRIOR) {
                    partyMemberColour = new Color32(189, 29, 0, 255);
                }
                else if (pm.className == ClassConstants.MAGE || pm.className == ClassConstants.FROSTGOLEM) {
                    partyMemberColour = new Color32(0, 152, 220, 255);
                }
                else if (pm.className ==  ClassConstants.ARCHER) {
                    partyMemberColour = new Color32(90, 197, 79, 255);
                }
                else if (pm.className == ClassConstants.ROGUE) {
                    partyMemberColour = new Color32(255, 205, 2, 255);
                }
            }
            else {
                partyMemberSprite = Resources.Load<Sprite>("Sprites/Classes/" + char.ToUpper(pm.className[0]) + pm.className.Substring(1) + "IconAlt");
                if (pm.className == ClassConstants.WARRIOR) {
                    partyMemberColour = new Color32(236, 100, 75, 255);
                }
                else if (pm.className == ClassConstants.MAGE || pm.className == ClassConstants.FROSTGOLEM) {
                    partyMemberColour = new Color32(0, 111, 135, 255);
                }
                else if (pm.className ==  ClassConstants.ARCHER) {
                    partyMemberColour = new Color32(0, 156, 65, 255);
                }
                else if (pm.className == ClassConstants.ROGUE) {
                    partyMemberColour = new Color32(133, 122, 61, 255);
                }
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
                
                HPBar.SetMaxAndCurrent(pm.HP, pm.CHP);
                MPBar.SetMaxAndCurrent(pm.MP, pm.CMP);
            }
            else if (panelName == PanelConstants.PARTYPANEL) {
                partyPanelHPBar = HPBar;
                partyPanelMPBar = MPBar;
                        
                HPBar.SetMaxAndCurrent(pm.HP, pm.CHP);
                MPBar.SetMaxAndCurrent(pm.MP, pm.CMP);
            }
            else if (panelName == PanelConstants.STATSPANEL) {
                statsPanelHPBar = HPBar;
                statsPanelMPBar = MPBar;

                HPBar.SetMaxAndCurrentDisplayCurrentOverMax(pm.HP, pm.CHP);
                MPBar.SetMaxAndCurrentDisplayCurrentOverMax(pm.MP, pm.CMP);
            }
        }

        /// <summary>
        /// Update the HP and MP bars of a given panel
        /// </summary>
        /// <param name="panelName"></param>
        public void UpdateHPAndMPBar(string panelName) {
            if (panelName == PanelConstants.STATUSPANEL) {
                statusPanelHPBar.SetMaxAndCurrent(pm.HP, pm.CHP);
                statusPanelMPBar.SetMaxAndCurrent(pm.MP, pm.CMP);
            }
            else if (panelName == PanelConstants.PARTYPANEL) {
                partyPanelHPBar.SetMaxAndCurrent(pm.HP, pm.CHP);
                partyPanelMPBar.SetMaxAndCurrent(pm.MP, pm.CMP);
            }
            else if (panelName == PanelConstants.STATSPANEL) {
                statsPanelHPBar.SetMaxAndCurrentDisplayCurrentOverMax(pm.HP, pm.CHP);
                statsPanelMPBar.SetMaxAndCurrentDisplayCurrentOverMax(pm.MP, pm.CMP);
            }
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

        public void SetStatusBar(StatusEffectsBar seBar) {
            seBar.Init(pm);
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
            actionsPanel = EventManager.instance.actionsPanel;
            partyPanel = EventManager.instance.partyPanel;
            gearPanel = EventManager.instance.gearPanel;
            candlesPanel = EventManager.instance.candlesPanel;
            statusPanel = EventManager.instance.statusPanel;
            skillsPanel = EventManager.instance.skillsPanel;
            utilityTabManager = EventManager.instance.utilityTabManager;
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
        /// Sets the partyMember's equipped candles
        /// </summary>
        public void SetEquippedCandles() {
            activeCandles[0] = candlesPanel.activeCandles[0].currentItemDisplay;
            activeCandles[1] = candlesPanel.activeCandles[1].currentItemDisplay;
            activeCandles[2] = candlesPanel.activeCandles[2].currentItemDisplay;
        }

        /// <summary>
        /// Sets the damage taken/amount healed and if the attack was a crit
        /// </summary>
        /// <param name="attackAmount"> Amount (damage/heal) from the attack </param>
        /// <param name="isCrit"> Flag for if attack that this character was a crit </param>
        public void SetAttackAmount(int attackAmount, bool isCrit) {
            this.attackAmount = attackAmount;
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
        /// Tells combatManager that this pmvc's pm has been selected as an attack target by the player
        /// </summary>
        public void SelectPartyMember() {
            CombatManager.instance.SelectPartyMember(pm);
        }

        /// <summary>
        /// Displays the active partyMember in combat, ignoring checks
        /// that would make the partyPanel and statusPanel not show the activePartyMember
        /// when in combat
        /// </summary>
        public void DisplayActivePartyMemberCombat() {
            partyPanel.DisplayActivePartyMemberCombat(pmdPartyPanel);
            skillsPanel.DisplayActivePartyMemberCombat(pmdSkillsPanel);
            statusPanel.DisplayPartyMember(this);
            
            if (gearPanel.isOpen == true) {
                gearPanel.Init(pm);
            }
            else if (candlesPanel.isOpen == true) {
                candlesPanel.Init(pm);
            }

            if (skillsPanel.isOpen == true) {
                skillsPanel.Init();
            }
        }

        /// <summary>
        /// Changes all panels to show the stored partyMember's information
        /// </summary>
        public void DisplayActivePartyMember() {
            if (CombatManager.instance.inCombat == false) {
                partyPanel.DisplayActivePartyMember(pmdPartyPanel);
                skillsPanel.DisplayActivePartyMember(pmdSkillsPanel);
                statusPanel.DisplayPartyMember(this);
                if (actionsPanel.isStoringInt == true) {
                    pm.SetAttackValues();
                    actionsPanel.DisplayPartyMemberNoFifth(pm);
                }
            }

            if (gearPanel.isOpen == true) {
                gearPanel.Init(pm);
            }
            else if (candlesPanel.isOpen == true) {
                candlesPanel.Init(pm);
            }
            if (skillsPanel.isOpen == true) {
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
            UpdateCombatActions();
        }

        /// <summary>
        /// Tell actions panel to update displayed combat actions
        /// </summary>
        public void UpdateCombatActions() {
            if (actionsPanel != null) {
                actionsPanel.UpdateCombatActions(pm);
            }
        }

        /// <summary>
        /// Tell the eventManager to update any eventDisplays due to the multiplier on WAX changing
        /// </summary>
        public void UpdateWAXValues() {
            EventManager.instance.UpdateWAXValues();
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
        /// <param name="isHealAnim"> Flag for if there is a healing animation that needs to be yielded to </param>
        /// <param name="isEventDes"> Flag for if there should be yielding at all (otherwise no animations or event description) </param>
        /// <returns> IEnumerator for animations </returns>
        public IEnumerator DisplayHPChange(bool isLoss, bool isHealAnim = false, bool isYield = true) {
            isAnimating = true;
            if (statusPanelHPBar != null) {
                statusPanelHPBar.SetCurrent(pm.CHP);  
            }
            if (statsPanelHPBar != null) {
                statsPanelHPBar.SetCurrentDisplayCurrentOverMax(pm.CHP);
            }

            if (isYield == false) {  // no event description means no yielding or animations (only if an attack uses hp as a cost)
                if (partyPanel.isOpen == true) {
                    partyPanelHPBar.SetCurrent(pm.CHP);
                }
            }
            else if (partyPanel.isOpen == true) {
                partyPanelHPBar.SetCurrent(pm.CHP);
                
                if (isLoss == true && CombatManager.instance.inCombat == true) {
                    if (isCrit == true) {
                        eventDescription.SetPMDamageCritText(pm, attackAmount);
                        yield return (StartCoroutine(pmdPartyPanel.PlayCritDamagedAnimation()));
                        isCrit = false;
                    }
                    else {
                        eventDescription.SetPMDamageText(pm, attackAmount);
                        yield return (StartCoroutine(pmdPartyPanel.PlayDamagedAnimation()));
                    }
                    if (pm.CHP == 0) {
                        yield return new WaitForSeconds(0.5f / GameManager.instance.gsDataCurrent.animationSpeed);
                    }
                }
                else if (isLoss == false && CombatManager.instance.inCombat == true && isHealAnim == true) {
                    if (attackAmount < 0) { // heals coming from statusEffects are negative values
                        attackAmount *= -1;
                    }
                    if (isCrit == true) {
                        eventDescription.SetPMHealCritText(pm, attackAmount);
                        isCrit = false;
                    }
                    else {
                        eventDescription.SetPMHealText(pm, attackAmount);
                    }
                    yield return new WaitForSeconds(1f / GameManager.instance.gsDataCurrent.animationSpeed);
                }
            }
            else {
                if (isLoss == true && CombatManager.instance.inCombat == true) {
                    if (isCrit == true) {
                        eventDescription.SetPMDamageCritText(pm, attackAmount);
                        isCrit = false;
                    }
                    else {
                        eventDescription.SetPMDamageText(pm, attackAmount);
                    }
                }
                else if (isLoss == false && CombatManager.instance.inCombat == true && isHealAnim == true) {
                    if (attackAmount < 0) { // heals coming from statusEffects are negative values
                        attackAmount *= -1;
                    }
                    if (isCrit == true) {
                        eventDescription.SetPMHealCritText(pm, attackAmount);
                        isCrit = false;
                    }
                    else {
                        eventDescription.SetPMHealText(pm, attackAmount);
                    }
                }
                yield return new WaitForSeconds(1f / GameManager.instance.gsDataCurrent.animationSpeed);
            }
            isAnimating = false;
        }

        /// <summary>
        /// Updates the current fill amount on all MPBars to show MP being added or lost
        /// </summary>
        /// <param name="isLoss"> Flag for if damaged animation should play </param>
        /// <param name="isFocusAnim"> Flag for if there is a focus animation that needs to be yielded to </param>
        /// <returns> IEnumerator for animations </returns>
        public IEnumerator DisplayMPChange(bool isLoss, bool isFocusAnim) {
            if (statusPanelMPBar != null) {
                statusPanelMPBar.SetCurrent(pm.CMP);  
            }
            if (statsPanelMPBar != null) {
                statsPanelMPBar.SetCurrentDisplayCurrentOverMax(pm.CMP);
            }
            if (EventManager.instance.partyPanel.isOpen == true) {
                partyPanelMPBar.SetCurrent(pm.CMP);
            }

            if (isFocusAnim == true) {
                if (isCrit == true) {
                    eventDescription.SetPMFocusCritText(pm, attackAmount);
                    isCrit = false;
                }
                else {
                    eventDescription.SetPMFocusText(pm, attackAmount);
                }

                yield return new WaitForSeconds(1f / GameManager.instance.gsDataCurrent.animationSpeed);
            }

            yield break;
        }

        /// <summary>
        /// Plays animation when an attack is dodged and has eventDescription write it out
        /// </summary>
        /// <returns> IEnumerator cause animations </returns>
        public IEnumerator DisplayAttackDodged() {
            eventDescription.SetPMDodgeText(pm);
            isAnimating = true;
            if (EventManager.instance.partyPanel.isOpen == true) {
                yield return (StartCoroutine(pmdPartyPanel.PlayDodgedAnimation()));
            }
            else {
                yield return new WaitForSeconds(0.75f / GameManager.instance.gsDataCurrent.animationSpeed);
            }
            isAnimating = false;
        }

        /// <summary>
        /// Plays buffing/healing animation on the pmd
        /// </summary>
        /// <param name="animationClipName"> Name of animation to play </param>
        /// <returns></returns>
        public IEnumerator DisplayAttackHelped(string animationClipName) {
            if (EventManager.instance.partyPanel.isOpen == true) {
                EventManager.instance.partyPanel.SetStatsPanel(false);  // if outside of combat, statsPanel might be open during animations
                yield return (StartCoroutine(pmdPartyPanel.PlayEffectAnimation(animationClipName)));
            }
            else {
                yield return new WaitForSeconds(0.75f / GameManager.instance.gsDataCurrent.animationSpeed);
            }
        }

        /// <summary>
        /// Play the animation for the initial summoning of a summoned partyMember
        /// </summary>
        /// <param name="animationClipName"></param>
        /// <returns></returns>
        public IEnumerator DisplaySummon(string animationClipName) {
            if (EventManager.instance.partyPanel.isOpen == true) {
                StartCoroutine(pmdPartyPanel.PlayEffectAnimation(animationClipName));
                yield return (StartCoroutine(pmdPartyPanel.PlaySpawnAnimation()));  
            }
            else {
                yield return new WaitForSeconds(0.75f / GameManager.instance.gsDataCurrent.animationSpeed);
            }
        }

        /// <summary>
        /// Play the animation for restoring a summoned partyMember (which is just DisplaySummon with n fade)
        /// </summary>
        /// <param name="animationClipName"></param>
        /// <returns></returns>
        public IEnumerator DisplaySummonRestored(string animationClipName) {
            if (EventManager.instance.partyPanel.isOpen == true) {
                yield return StartCoroutine(pmdPartyPanel.PlayEffectAnimation(animationClipName));
            }
            else {
                yield return new WaitForSeconds(0.75f / GameManager.instance.gsDataCurrent.animationSpeed);
            }
        }

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
            if (partyPanel.isOpen == true) {
                pmdPartyPanel.PlayCleanUpStatusEffectAnimations(animationsToPlay);
            }
        }

        public void ShowLVLUp() {
            AudioManager.instance.PlaySFX("LVLUp");
            ExciteSkillsTab(); 
        }

        public void ExciteSkillsTab() {
            utilityTabManager.ShowSkillPointsInTab(1);  // 1 is skills tab index
            utilityTabManager.HighlightTab(1);          // Only on leveling up should the tab be highlighted (if player chooses not to spec, its on them after clicking)
            if (skillsPanel.isOpen) {
                pmdSkillsPanel.UpdateSkillPointsText(pm.skillPoints);
            }
        }

        public void UpdateSkillsTab() {
            if (utilityTabManager != null) {
                utilityTabManager.ShowSkillPointsInTab(1); // 1 is skills tab index
            }
        }
    }
}
