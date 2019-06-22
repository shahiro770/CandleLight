/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: June 21, 2019
* 
* The DamageText class is used to display text a character has taken in response to damage.
* For now, it is primarily used for monsters, wit
*
*/

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerUI {

    public class DamageText : MonoBehaviour {
        
        public TextMeshProUGUI meshText;    /// <value> Text mesh pros are not always loaded in time, so store reference to guarantee access </value>
        public Animator textAnimator;       /// <value> Animator for damaged text </value>

        /// <summary>
        /// Unhides the text and sets the amount
        /// </summary>
        /// <param name="amount"> Amount to display </param>
        public void ShowDamage(int amount) {
            SetText(amount);
            gameObject.SetActive(true);
        }

        /// <summary>
        /// Hides the text
        /// </summary>
        public void HideDamage() {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Sets the integer amount to display
        /// </summary>
        /// <param name="amount"> Amount to display </param>
        private void SetText(int amount) {
            meshText.text = amount.ToString();
        }        
    }
}
