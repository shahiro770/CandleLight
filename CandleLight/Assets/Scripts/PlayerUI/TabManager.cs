/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: August 1, 2019
* 
* The TabManager class is used to manage tab buttons in the player UI's, displaying and hiding panels
* when one of the buttons the tab manager over-sees is clicked.
*
*/

using EventManager = Events.EventManager;
using GameManager = General.GameManager;
using Localization;
using PanelConstants = Constants.PanelConstants;
using PartyManager = Party.PartyManager;
using TutorialConstants = Constants.TutorialConstants;
using UIEffects;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerUI {

    public class TabManager : MonoBehaviour {

        /* external component references */ 
        public ActionsPanel actionsPanel;       /// <value> actionsPanel reference </value>
        public Button[] tabs = new Button[3];   /// <value> Three tabs per tabManager </value>
        public LocalizedText[] tabTexts = new LocalizedText[3];    /// <value> Text for each tab </value>
        public Panel[] panels = new Panel[3];   /// <value> Panel references </value>
        public ButtonTransitionState[] btss = new ButtonTransitionState[3]; /// <value> Indicate which tabs are selected </value>
        public bool triggerTutorial = false;    /// <value> Flag for if the next tab button click will trigger a tutorial notification </value>

        private int currentIndex = 0;

        /// <summary>
        /// Sets the pressed colour block and opens the partyPanel to begin
        /// </summary>
        void Start() {
            ColorBlock pressedBlock = tabs[0].colors;

            pressedBlock.normalColor = new Color32(255, 255, 255, 255);
            pressedBlock.highlightedColor = new Color32(255, 255, 255, 200);
            pressedBlock.pressedColor = new Color32(255, 255, 255, 255);
            pressedBlock.disabledColor = new Color32(255, 255, 255, 255);

            for (int i = 0; i < btss.Length; i++) { //(ButtonTransitionState bts in btss) {
                btss[i].SetColorBlock("pressed", pressedBlock);
            }
            if (panels[0].GetPanelName() == PanelConstants.PARTYPANEL) {
                btss[1].SetColorBlock("normalAlternate", UIManager.instance.orangeBlock);
            }
        
            if (GameManager.instance.tutorialTriggers[(int)TutorialConstants.tutorialTriggers.isTutorial] == false) {                   // panels aren't opened immediatel in the tutorial
                if (panels[0].GetPanelName() == PanelConstants.PARTYPANEL) {  // right tabManager
                    OpenPanel(0);
                    if (PartyManager.instance.GetSkillPointsAll() > 0) {    // only excite if there are skill points unspent (which may not be the case on continue)
                        ShowSkillPointsInTab(1);
                        HighlightTab(1);
                    }
                }
                else {  // left tabManager
                    OpenPanel(0);
                }
            }
        }

        /// <summary>
        /// Opens a panel based on the inputted index, while hiding the previous tab
        /// </summary>
        /// <param name="index"> Index in the panels array to open </param>
        public void OpenPanel(int index) {
            panels[currentIndex].gameObject.SetActive(false);
            btss[currentIndex].SetColor("normal");
            
            currentIndex = index;
            panels[currentIndex].gameObject.SetActive(true);
            btss[currentIndex].SetColor("pressed");

            if (tabTexts[index].key.EndsWith("_(!)")) {
                tabTexts[index].SetKey(tabTexts[index].key.Substring(0, tabTexts[index].key.Length - 4)); 
            }
            if (triggerTutorial == true) {  // for certain conditions, prevent another tutorial from popping up when clicked
                if (EventManager.instance.TutorialTabOnClick(index, panels[0].GetPanelName() == PanelConstants.PARTYPANEL) == true) {
                    triggerTutorial = false;
                }
            }
        }

        /// <summary>
        /// Sets all tab buttons interactable
        /// </summary>
        public void SetAllButtonsInteractable() {
            for (int i = 0; i < tabs.Length; i++) {
                if (tabTexts[i].key != "empty_tab") {   // prevents tutorial doesn't re-enable everything
                    tabs[i].interactable = true;
                }
            }
        }

        /// <summary>
        /// Sets a specific tab button interactable
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void SetButtonInteractable(int index, bool value) {
            tabs[index].interactable = value;
        }

        public void SetAllButtonsUninteractable() {
            foreach (Button b in tabs) {
                b.interactable = false;
            }
        }

        /// <summary>
        /// Highlights a specific tab
        /// </summary>
        /// <param name="index"></param>
        public void HighlightTab(int index) {
            btss[index].SetColor("normalAlternate"); 
        }

        /// <summary>
        /// Excites a tab at the index, giving it !!!!!!!!!
        /// </summary>
        /// <param name="index"> Index of tab button </param>
        public void ExciteTab(int index) {
            if (tabTexts[index].key == "empty_tab") {
                SetButtonInteractableAndName(index);
            }
            if (tabTexts[index].key.EndsWith("_(!)") == false) {
                tabTexts[index].SetKey(tabTexts[index].key + "_(!)");
            }
            btss[index].SetColor("highlighted");    // draw more attention 
        }

        /// <summary>
        /// Show the total number of unspect skill points in a tab
        /// </summary>
        /// <param name="index"></param>
        public void ShowSkillPointsInTab(int index) {
            int sp = PartyManager.instance.GetSkillPointsAll();
            if (sp > 0) {
                if (tabTexts[index].key == "empty_tab") {   // tabs may be empty during the tutorial, also prevents text from showing up when a partyMember is added during the tutorial
                    SetButtonInteractableAndName(index);
                }
                tabTexts[index].SetKeyAndAppendNoSpace(tabTexts[index].key, "(" + sp +  ")");
            }
            else {
                tabTexts[index].SetKey(tabTexts[index].key);
            }
        }

        /// <summary>
        /// Hides tab buttons
        /// </summary>
        public void SetTabsEmpty() {
            for (int i = 0; i < tabs.Length; i++) {
                tabs[i].interactable = false;
                tabTexts[i].SetKey("empty_tab");
                panels[i].gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Gives a tab button a name and makes it interactable (tutorial only)
        /// </summary>
        /// <param name="index"></param>
        public void SetButtonInteractableAndName(int index) {
            if (panels[0].GetPanelName() == PanelConstants.GEARPANEL) {  // right tabManager
                if (index == 0) {
                    tabTexts[index].SetKey("gear_tab");
                }
                else if (index == 1) {
                    tabTexts[index].SetKey("candles_tab");
                }
                else if (index == 2) {
                    tabTexts[index].SetKey("special_tab");
                }
            }
            else {
                if (index == 0) {
                    tabTexts[index].SetKey("party_tab");
                }
                else if (index == 1) {
                    tabTexts[index].SetKey("skills_tab");
                }
                else if (index == 2) {
                    tabTexts[index].SetKey("info_tab");
                }
            }
            triggerTutorial = true;
            tabs[index].interactable = true;
        }
    }
}