/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: August 1, 2019
* 
* The TabManager class is used to manage tab buttons in the player UI's, displaying and hiding panels
* when one of the buttons the tab manager over-sees is clicked.
*
*/

using Localization;
using PanelConstants = Constants.PanelConstants;
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

            if (panels[0].GetPanelName() == PanelConstants.PARTYPANEL) {  // right tabManager
                OpenPanel(0);
            }
            else {  // left tabManager
                OpenPanel(0);
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
        }

        public void SetAllButtonsInteractable() {
            foreach (Button b in tabs) {
                b.interactable = true;
            }
        }

        public void SetAllButtonsUninteractable() {
            foreach (Button b in tabs) {
                b.interactable = false;
            }
        }

        /// <summary>
        /// Excites a tab at the index, giving it !!!!!!!!!
        /// </summary>
        /// <param name="index"> Index of tab button </param>
        public void ExciteTab(int index) {
            if (tabTexts[index].key.EndsWith("_(!)") == false) {
                tabTexts[index].SetKey(tabTexts[index].key + "_(!)");
            }
        }
    }
}