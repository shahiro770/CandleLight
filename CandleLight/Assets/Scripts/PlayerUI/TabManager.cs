/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: August 1, 2019
* 
* The TabManager class is used to manage tab buttons in the player UI's, displaying and hiding panels
* when one of the buttons the tab manager over-sees is clicked.
*
*/

using PanelConstants = Constants.PanelConstants;
using UIEffects;
using UnityEngine;
using UnityEngine.UI;


namespace PlayerUI {

    public class TabManager : MonoBehaviour {

        /* external component references */ 
        public ActionsPanel actionsPanel;       /// <value> Trigger navigation settting whenever tabs switch </value>
        public Button[] tabs = new Button[3];   /// <value> Three tabs per tabManager </value>
        public Panel[] panels = new Panel[3];   /// <value> Panel references </value>
        public ButtonTransitionState[] btss = new ButtonTransitionState[3]; /// <value> Indicate which tabs are selected </value>

        private int currentIndex = 0;

        /// <summary>
        /// Sets the colorBlock to have the correct pressed colorBlocks
        /// </summary>
        void Awake() {
            ColorBlock pressedBlock = tabs[0].colors;

            pressedBlock.normalColor = new Color32(255, 255, 255, 255);
            pressedBlock.highlightedColor = new Color32(255, 255, 255, 200);
            pressedBlock.pressedColor = new Color32(255, 255, 255, 255);
            pressedBlock.disabledColor = new Color32(255, 255, 255, 255);

            foreach(ButtonTransitionState bts in btss) {
               bts.SetColorBlock("pressed", pressedBlock);
            }
        }

        /// <summary>
        /// Open the partyPanel to begin
        /// </summary>
        void Start() {
            if (panels[0].GetPanelName() == PanelConstants.PARTYPANEL) {  // right tabManager
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
            actionsPanel.SetHorizontalNavigation(panels[index]);
        }

        public void SetAllButtonsInteractable() {
            foreach (Button b in tabs) {
                b.interactable = true;
            }

            if (panels[0].name == PanelConstants.PARTYPANEL) {  // right tabManager
                Navigation n = tabs[0].navigation;
                n.selectOnLeft = actionsPanel.GetNavigatableButtonRight();
            }
        }

        public void SetAllButtonsUninteractable() {
            foreach (Button b in tabs) {
                b.interactable = false;
            }
        }
    }
}