/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: June 21, 2019
* 
* The DamageText class is used to display text a character has taken in response to damage.
* For now, it is only used for monsters.
*
*/

using Localization;
using TMPro;
using UnityEngine;

namespace PlayerUI {

    public class DamageText : MonoBehaviour {
        
        public TextMeshProUGUI meshText;    /// <value> Text mesh pros are not always loaded in time, so store reference to guarantee access </value>
        public Animator textAnimator;       /// <value> Animator for damaged text </value>
        
        [field: SerializeField] private string missText; /// <value> Localized text for the word "lost" </value>

        /// <summary>
        /// Start to initialize missText, so it doesn't need to be localized multiple times
        /// </summary>
        void Start() {
            missText = LocalizationManager.instance.GetLocalizedValue("miss_text");
        }

        /// <summary>
        /// Unhides the text and sets the amount
        /// </summary>
        /// <param name="amount"> Amount to display </param>
        public void ShowDamage(int amount) {
            SetText(amount);
            gameObject.SetActive(true);
        }

        /// <summary>
        /// Unhides the text and writes that the attack missed
        /// </summary>
        public void ShowDodged() {
            meshText.text = missText;
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
