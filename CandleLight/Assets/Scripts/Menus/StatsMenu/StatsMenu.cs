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
using UnityEngine;

namespace Menus.Stats {

    public class StatsMenu : MonoBehaviour {

        /* external component references*/
        public HighScore[] highScores;

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
                greyWastesSprites[i] =  Resources.Load<Sprite>("Sprites/SubAreas/GreyWastes/" + i);
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
    }
}