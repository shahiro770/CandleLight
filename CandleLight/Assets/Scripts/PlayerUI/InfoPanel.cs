/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: August 1, 2019
* 
* The 
*
*/

using Localization;
using Party;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Events;

namespace PlayerUI {

    public class InfoPanel : Panel {

        /* external component reference */
        public Image WAXImage;
        public Image progressImage;
        public LocalizedText amountTextWAX; /// <value> Display the amount of the WAX the party has earned </value>
        public LocalizedText amountTextPROG; /// <value> Display the progress amount in the current subArea </value>
        public Quest[] quests;
        public bool isOpen = false;

        private Color32 mainTheme;
        private Color32 secondaryTheme;

        /// <summary>
        /// Update the InfoPanel with relevant information and visuals when opened
        /// </summary>
        void OnEnable() {
            isOpen = true;
            SetThemeButtons();
            UpdateAmounts();
        }

        /// <summary>
        /// Set isOpen to false on disabling so relevant interactions don't happen
        /// </summary>
        void OnDisable() {
            isOpen = false;
        }

        /// <summary>
        /// Adds a quest to one of the quest displays
        /// </summary>
        /// <param name="questName"></param>
        public void AddQuest(string questName) {
            for (int i = 0; i < quests.Length; i++) {
                if (quests[i].questName == "") {
                    quests[i].SetQuest(questName);
                    break;
                }
            }
        }

        /// <summary>
        /// Removes a specific quest from the quest displays
        /// </summary>
        /// <param name="questName"></param>
        public void CompleteQuest(string questName) {
            for (int i = 0; i < quests.Length; i++) {
                if (quests[i].questName == questName) {
                    quests[i].CompleteQuest();
                    break;
                }
            }
        }

        /// <summary>
        /// Colours the buttons that are themed by the area
        /// TODO: This fails if the eventManager doesn't have the area (which it doesn't while loading)
        /// Will need the area to be loaded in a higher state such as gameManager for this to work 
        /// if this panel were the first viewed
        /// </summary>
        private void SetThemeButtons() {
            mainTheme = EventManager.instance.GetThemeColour();
            secondaryTheme = EventManager.instance.GetSecondaryThemeColour();

            WAXImage.color = mainTheme;
            progressImage.color = secondaryTheme;
        }

        /// <summary>
        /// Updates the WAX and progress amounts displayed by their respective displays
        /// </summary>
        public void UpdateAmounts() {
            amountTextWAX.SetText(PartyManager.instance.WAX.ToString());
            amountTextPROG.SetText(EventManager.instance.subAreaProgress.ToString() + "%");
        }
    }
}