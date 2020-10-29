/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 11, 2019
* 
* The StatsMenu class controls all components used to display
* various data regarding the player's runs (highscores, achievements, and cool stuff)
*
*/

using ClassConstants = Constants.ClassConstants;
using General;
using Localization;
using TimeSpan = System.TimeSpan;
using UnityEngine;

namespace Menus.Stats {

    public class StatsMenu : MonoBehaviour {

        /* external component references */
        public AchievementDisplay[] achievements;
        public HighScore[] highScores;
        public LocalizedText monstersAmount;
        public LocalizedText WAXAmount;
        public LocalizedText eventsAmount;
        public LocalizedText timeAmount;

        private Sprite[] greyWastesSprites;
        private Sprite warriorIcon;
        private Sprite mageIcon;
        private Sprite archerIcon;
        private Sprite rogueIcon;

        private int greyWastesSubAreas = 10;        /// <value> Constant number of subAreas for the GreyWastes </value>

        /// <summary>
        /// Awake to load sprites so they don't have to be reloaded on every enable
        /// </summary>
        void Awake() {
            warriorIcon = Resources.Load<Sprite>("Sprites/Classes/WarriorIcon");
            mageIcon = Resources.Load<Sprite>("Sprites/Classes/MageIcon");
            archerIcon = Resources.Load<Sprite>("Sprites/Classes/ArcherIcon");
            rogueIcon = Resources.Load<Sprite>("Sprites/Classes/RogueIcon");

            greyWastesSprites = new Sprite[greyWastesSubAreas];
            for (int i = 0; i < greyWastesSubAreas; i++) {
                greyWastesSprites[i] =  Resources.Load<Sprite>("Sprites/SubAreas/GreyWastesLarge/" + i);
            }
            for (int i = 0; i < achievements.Length; i++) {
                achievements[i].SetUnlockedColor(GetAchievementColor(i));
            }
        }

        /// <summary>
        /// Show all highscores on enable
        /// </summary>
        void OnEnable() {
            for (int i = 0; i < highScores.Length; i++) {
                HighScoreData hsd = GameManager.instance.gsData.hsds[i];
                if (hsd != null && hsd.areaName != null) {
                    highScores[i].Init(hsd);
                }
                else {
                    highScores[i].Init();
                }
            }

            for (int i = 0; i < achievements.Length; i++) {
                achievements[i].SetSprite(GameManager.instance.achievementSprites[i]);
                achievements[i].SetTooltip(i);
                achievements[i].SetColour(GameManager.instance.achievementsUnlocked[i]);
            }

            monstersAmount.SetText(GameManager.instance.gsData.mostMonsters.ToString());
            WAXAmount.SetText(GameManager.instance.gsData.mostWAX.ToString());
            eventsAmount.SetText(GameManager.instance.gsData.mostEvents.ToString());
            if (GameManager.instance.gsData.fastestTime == -1) {
                timeAmount.SetText("--");
            }
            else {
                timeAmount.SetText(TimeSpan.FromSeconds(GameManager.instance.gsData.fastestTime).ToString(@"hh\:mm\:ss\.ff"));
            }
        }

        /// <summary>
        /// Returns the specific class sprite
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public Sprite GetClassSprite(string className) {
            if (className == ClassConstants.WARRIOR) {
                return warriorIcon;
            }
            else if (className == ClassConstants.MAGE) {
                return mageIcon;
            }
            else if (className == ClassConstants.ARCHER) {
                return archerIcon;
            }
            else if (className == ClassConstants.ROGUE) {
                return rogueIcon;
            }

            return null;
        }

        /// <summary>
        /// Returns the subArea card
        /// </summary>
        /// <param name="areaName"></param>
        /// <param name="subAreaIndex"></param>
        /// <returns></returns>
        public Sprite GetSubAreaSprite(string areaName, int subAreaIndex) {
            if (areaName == "GreyWastes") {
                return greyWastesSprites[subAreaIndex];
            }
            
            return null;
        }

        /// <summary>
        /// Hard code the achievement's unlocked colour for easy use
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Color GetAchievementColor(int index) {
            switch (index) {
                case 0:
                    return new Color32(133, 133, 133, 255);
                case 1:
                    return new Color32(234, 50, 60, 255);
                case 2:
                    return new Color32(92, 138, 57, 255);
                case 3:
                    return new Color32(255, 205, 2, 255);
                case 4:
                    return new Color32(155, 66, 28, 255);
                default:
                    return new Color32(61, 61, 61, 255);
            }
        }
    }
}