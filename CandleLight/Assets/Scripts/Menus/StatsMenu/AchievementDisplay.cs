/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 11, 2019
* 
* The AchievementDisplay class is used to display an achievement
*
*/

using PlayerUI;
using UnityEngine;
using UnityEngine.UI;

namespace Menus.Stats {

    public class AchievementDisplay : MonoBehaviour {

        /* external comp references */
        public Image imgBackground;
        public SpriteRenderer achievementIcon;
        public Tooltip t;
        public Button b;

        private Color unlockedColour;
        
        /// <summary>
        /// Set the tooltip sprite
        /// </summary>
        /// <param name="s"></param>
        public void SetSprite(Sprite s) {
            achievementIcon.sprite = s;
        }

        /// <summary>
        /// Set the achievement tooltip text and position
        /// </summary>
        /// <param name="index"></param>
        public void SetTooltip(int index) {
            t.SetKey("title", index + "_ach_title");
            t.SetKey("description", index + "_ach_des");
            t.SetImageDisplayBackgroundWidth(imgBackground.rectTransform.sizeDelta.x);
        }

        /// <summary>
        /// Set the unlocked colour 
        /// </summary>
        /// <param name="c"></param>
        public void SetUnlockedColor(Color c) {
            unlockedColour = c;
        }

        /// <summary>
        /// Show if an achievement is unlocked
        /// </summary>
        /// <param name="unlocked"> true for unlocked, false otherwise </param>
        public void SetColour(bool unlocked) {
            if (unlocked == true) {
                achievementIcon.color = unlockedColour;
                imgBackground.color = unlockedColour;
            }
            else {
                achievementIcon.color = new Color32(61, 61, 61, 255);
                imgBackground.color = achievementIcon.color;
            }
        }
    }
}