/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: August 1, 2019
* 
* The TabManager class is used to manage tab buttons in the player UI's, displaying and hiding panels
* when one of the buttons the tab manager over-sees is clicked.
*
*/

using System.Collections;
using System.Collections.Generic;
using UIEffects;
using UnityEngine;
using UnityEngine.UI;


namespace PlayerUI {

    public class TabManager : MonoBehaviour {

        /* external component references */
        public ActionsPanel actionsPanel;
        public Button[] tabs = new Button[3];   // three tabs per panel
        public Panel[] panels = new Panel[3];
        public ButtonTransitionState[] btss = new ButtonTransitionState[3];

        private int currentIndex = 0;

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
    }
}