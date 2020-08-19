/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 11, 2019
* 
* The HighScore class is used to display a highscore
* and all pertinent details (such as party composition)
*
*/

using General;
using Localization;
using UnityEngine;
using UnityEngine.UI;

namespace Menus.Stats {

    public class HighScore : MonoBehaviour {

        public GameObject visualHolder;
        public Image background;
        public LocalizedText subAreaName;
        public LocalizedText scoreAmountText; 
        public SpriteRenderer pm0Sprite;
        public SpriteRenderer pm1Sprite;
        public SpriteRenderer subAreaCard;
        public StatsMenu statsMenu;

        /// <summary>
        /// Iniitalize with properties
        /// </summary>
        /// <param name="areaName"></param>
        /// <param name="class0"></param>
        /// <param name="class1"></param>
        /// <param name="score"></param>
        /// <param name="subAreaIndex"></param>
        public void Init(string areaName, string class0, string class1, int score, int subAreaIndex) {
            visualHolder.SetActive(true);
            pm0Sprite.sprite = statsMenu.GetClassSprite(class0);
            pm1Sprite.sprite = statsMenu.GetClassSprite(class1);
            scoreAmountText.SetText(score.ToString());

            if (areaName == "GreyWastes") {
                background.color = new Color32(39, 39, 39, 255);
            }
            subAreaCard.sprite = statsMenu.GetSubAreaSprite(areaName, subAreaIndex);
            subAreaName.SetKey(areaName + subAreaIndex);
        }

        /// <summary>
        /// Initialize from highscore data
        /// </summary>
        /// <param name="hsd"></param>
        public void Init(HighScoreData hsd) {
            visualHolder.SetActive(true);
            pm0Sprite.sprite = statsMenu.GetClassSprite(hsd.class0);
            pm1Sprite.sprite = statsMenu.GetClassSprite(hsd.class1);
            scoreAmountText.SetText(hsd.score.ToString());

            if (hsd.areaName == "GreyWastes") {
                background.color = new Color32(39, 39, 39, 255);
            }
            subAreaCard.sprite = statsMenu.GetSubAreaSprite(hsd.areaName, hsd.subAreaIndex);
            subAreaName.SetKey(hsd.areaName + hsd.subAreaIndex);
        }

        /// <summary>
        /// Init an empty highscore (i.e. player hasn't played the game enough times yet)
        /// </summary>
        public void Init() {
            visualHolder.SetActive(false);
        }
    }
}