/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: August 1, 2019
* 
* The 
*
*/

using Localization;
using PanelConstants = Constants.PanelConstants;
using Party;
using UnityEngine;
using UnityEngine.UI;
using Events;

namespace PlayerUI {

    public class InfoPanel : Panel {

        /* external component reference */
        public SpriteRenderer subAreaCard;
        public Image WAXImage;
        public Image progressImage;
        public LocalizedText subAreaName;
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
        public void AddQuest(string questName, string startEvent, string currentEvent) {
            for (int i = 0; i < quests.Length; i++) {
                if (quests[i].questName == "") {
                    quests[i].SetQuest(questName, startEvent, currentEvent);
                    break;
                }
            }
        }

        /// <summary>
        /// Updates the respective quest with what the next event will be
        /// </summary>
        /// <param name="questName"></param>
        /// <param name="nextEvent"></param>
        public void UpdateQuest(string questName, string nextEvent) {
            foreach (Quest q in quests) {
                if (q.questName == questName) {
                    q.UpdateQuestProgress(nextEvent);
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
        /// Return all quest data
        /// </summary>
        /// <returns></returns>
        public string[][] GetData() {
            string[][] questData = new string[quests.Length][];
            for (int i = 0; i < this.quests.Length; i++) {
                questData[i] = quests[i].GetQuestData();
            }

            return questData;
        }

        /// <summary>
        /// Load all quest data
        /// </summary>
        /// <param name="questData"></param>
        public void LoadData(string[][] questData) {
            for (int i = 0; i < questData.Length; i++) {
                if (questData[i][0] != "") {
                    AddQuest(questData[i][0], questData[i][1], questData[i][2]);
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

        /// <summary>
        /// Updates the subArea card and title
        /// </summary>
        /// <param name="subAreaCardSprite"></param>
        /// <param name="cardName"></param>
        public void UpdateSubAreaCard(Sprite subAreaCardSprite, string cardName) {
            subAreaCard.sprite = subAreaCardSprite;
            subAreaName.SetKey(cardName);
        }

        public override string GetPanelName() {
            return PanelConstants.INFOPANEL;
        }
    }
}