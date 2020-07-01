/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: June 21, 2019
* 
* The StatsPanel class is used to display all of the stats of a partyMember.
*
*/

using Characters;
using EventManager = Events.EventManager;
using Localization;
using PanelConstants = Constants.PanelConstants;
using PartyManager = Party.PartyManager;
using UnityEngine.UI;

namespace PlayerUI {

    public class StatsPanel : Panel {
        
        /* external component references */
        public Bar HPBar;
        public Bar MPBar;
        public EXPBar pmEXPBar;
        public Image headerBackground;
        public Image nextLVLBackground;
        public LocalizedText headerText;   /// <value> Text displaying current level </value>
        public LocalizedText STRText;   /// <value> Text displaying STR </value>
        public LocalizedText DEXText;   /// <value> Text displaying DEX </value>
        public LocalizedText INTText;   /// <value> Text displaying INT </value>
        public LocalizedText LUKText;   /// <value> Text displaying LUK </value>
        public LocalizedText PATKText;  /// <value> Text displaying PATK </value>
        public LocalizedText MATKText;  /// <value> Text displaying MATK </value>
        public LocalizedText PDEFText;  /// <value> Text displaying PDEF </value>
        public LocalizedText MDEFText;  /// <value> Text displaying MDEF </value>
        public LocalizedText ACCText;   /// <value> Text displaying ACC </value>
        public LocalizedText DOGText;   /// <value> Text displaying DOG </value>
        public LocalizedText CRITText;  /// <value> Text displaying CRIT% </value>
        public LocalizedText NextLVLText;  /// <value> Text displaying EXP to next level </value>
        public TooltipTextMesh STRtt;   /// <value> </value>
        public TooltipTextMesh DEXtt;   /// <value> </value>
        public TooltipTextMesh INTtt;   /// <value> </value>
        public TooltipTextMesh LUKtt;   /// <value> </value>
        public TooltipTextMesh PATKtt;  /// <value> </value>
        public TooltipTextMesh MATKtt;  /// <value> </value>
        public TooltipTextMesh PDEFtt;  /// <value> </value>
        public TooltipTextMesh MDEFtt;  /// <value> </value>
        public TooltipTextMesh ACCtt;   /// <value> </value>
        public TooltipTextMesh DOGtt;   /// <value> </value>
        public TooltipTextMesh CRITtt;  /// <value> </value>


        public PartyMemberVisualController pmvc;    /// <value> pmvc of pm currently being displayed</value>
        private string LVLText = LocalizationManager.instance.GetLocalizedValue("LVL_label");   /// <value> "LVL" </value>
        public bool isOpen = false; /// <value> Flag for if panel is open </value>

        /// <summary>
        /// Set all of the tooltips on the statsPanel
        /// </summary>
        void Start() {
            STRtt.SetKey("title", "STR_stat_title");
            STRtt.SetKey("subtitle", "STR_stat_sub");
            STRtt.SetKey("description", "STR_stat_des");
            DEXtt.SetKey("title", "DEX_stat_title");
            DEXtt.SetKey("subtitle", "DEX_stat_sub");
            DEXtt.SetKey("description", "DEX_stat_des");
            INTtt.SetKey("title", "INT_stat_title");
            INTtt.SetKey("subtitle", "INT_stat_sub");
            INTtt.SetKey("description", "INT_stat_des");
            LUKtt.SetKey("title", "LUK_stat_title");
            LUKtt.SetKey("subtitle", "LUK_stat_sub");
            LUKtt.SetKey("description", "LUK_stat_des");
            PATKtt.SetKey("title", "PATK_stat_title");
            PATKtt.SetKey("subtitle", "PATK_stat_sub");
            PATKtt.SetKey("description", "PATK_stat_des");
            MATKtt.SetKey("title", "MATK_stat_title");
            MATKtt.SetKey("subtitle", "MATK_stat_sub");
            MATKtt.SetKey("description", "MATK_stat_des");
            PDEFtt.SetKey("title", "PDEF_stat_title");
            PDEFtt.SetKey("subtitle", "PDEF_stat_sub");
            PDEFtt.SetKey("description", "PDEF_stat_des");
            MDEFtt.SetKey("title", "MDEF_stat_title");
            MDEFtt.SetKey("subtitle", "MDEF_stat_sub");
            MDEFtt.SetKey("description", "MDEF_stat_des");
            ACCtt.SetKey("title", "ACC_stat_title");
            ACCtt.SetKey("subtitle", "ACC_stat_sub");
            ACCtt.SetKey("description", "ACC_stat_des");
            DOGtt.SetKey("title", "DOG_stat_title");
            DOGtt.SetKey("subtitle", "DOG_stat_sub");
            DOGtt.SetKey("description", "DOG_stat_des");
            CRITtt.SetKey("title", "CRIT_stat_title");
            CRITtt.SetKey("subtitle", "CRIT_stat_sub");
            CRITtt.SetKey("description", "CRIT_stat_des");
        }

        /// <summary>
        /// Update the statsPanel with relevant information and visuals when opened
        /// </summary>
        void OnEnable() {
            isOpen = true;
            Init(PartyManager.instance.GetActivePartyMember().pmvc);
        }

        /// <summary>
        /// Set isOpen to false on disabling so relevant interactions don't happen
        /// </summary>
        void OnDisable() {
            isOpen = false;
            if (this.pmvc) {
                this.pmvc.UnsetEXPBar(PanelConstants.STATSPANEL);
                this.pmvc.UnsetHPAndMPBar(PanelConstants.STATSPANEL);
                this.pmvc = null;
            }
        }

        /// <summary>
        /// Initialize the stats panel to display the stats of a given partyMember.
        /// This includes HP, MP, and EXP bars.
        /// </summary>
        /// <param name="pmvc"></param>
        public void Init(PartyMemberVisualController pmvc) {
            if (this.pmvc) {
                this.pmvc.UnsetEXPBar(PanelConstants.STATSPANEL);
                this.pmvc.UnsetHPAndMPBar(PanelConstants.STATSPANEL);
            }

            this.pmvc = pmvc;
            pmvc.SetEXPBar(PanelConstants.STATSPANEL, pmEXPBar);
            pmvc.SetHPAndMPBar(PanelConstants.STATSPANEL, HPBar, MPBar);
            
            int[] stats = pmvc.GetStats();
            headerBackground.color = pmvc.partyMemberColour;
            headerText.SetText(pmvc.GetPartyMemberName() + " " + LVLText +  " " + stats[0]);
            nextLVLBackground.color = EventManager.instance.GetSecondaryThemeColour();
            
            SetPartyMemberStats(pmvc.GetStats());
        }

        /// <summary>
        /// Sets all of the LocalizedTexts to display the relevant stats
        /// </summary>
        /// <param name="stats"> Integer array of stats </param>
        public void SetPartyMemberStats(int[] stats) {
            STRText.SetText(stats[1].ToString());
            DEXText.SetText(stats[2].ToString());
            INTText.SetText(stats[3].ToString());
            LUKText.SetText(stats[4].ToString());
            PATKText.SetText(stats[5].ToString());
            MATKText.SetText(stats[6].ToString());
            PDEFText.SetText(stats[7].ToString());
            MDEFText.SetText(stats[8].ToString());
            ACCText.SetText(stats[9].ToString());
            DOGText.SetText(stats[10].ToString());
            CRITText.SetText(stats[11] + "%");
            NextLVLText.SetText((stats[12] - stats[13]).ToString());
        }

        /// <summary>
        /// Returns the name of this panel
        /// </summary>
        /// <returns> Name of panel </returns>
        public override string GetPanelName() {
            return PanelConstants.STATSPANEL;
        }
    }        
}
