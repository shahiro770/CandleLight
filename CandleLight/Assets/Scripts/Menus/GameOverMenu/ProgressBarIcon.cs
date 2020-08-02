/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: January 23, 2019
* 
* The ProgressBarIcon class controls properties of an icon on the progress bar
*/

using System.Collections;
using UnityEngine;

namespace Menus.GameOverMenu {

    public class ProgressBarIcon : MonoBehaviour {

        public Animator a;
        public SpriteRenderer sr; 
        public RectTransform rt;

        /// <summary>
        /// Sets the position of the icon relative to its anchor on the parent
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SetPosition(int x, int y) {
            rt.anchoredPosition = new Vector3(x, y, 0.0f);
        }

        /// <summary>
        /// Sets the sprite
        /// </summary>
        /// <param name="s"></param>
        public void SetSprite(Sprite s) {
            sr.sprite = s;
        }

        /// <summary>
        /// Plays an animation
        /// </summary>
        /// <param name="trigger"> For a progressBarIcon, this may be "show", "hide", or "pop" </param>
        /// <returns> IEnumerator </returns>
        public void PlayAnimation(string trigger) {
            a.ResetTrigger(trigger);
            a.SetTrigger(trigger);
        }
    }
}