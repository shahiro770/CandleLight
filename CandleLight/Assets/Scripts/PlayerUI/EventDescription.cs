/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: June 18, 2019
* 
* The EventDescription gives a description of the current event.
* This can be a prompt for the event, a monster's attack name, and etc.
*
*/

using Characters;
using Localization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerUI {

    public class EventDescription : MonoBehaviour {
        
        public LocalizedText eventText;     /// <value> Text for event </value>
        public Image textBackground;        /// <value> Image behind text </value>
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="textKey"></param>
        public void SetText(string textKey) {
            textBackground.gameObject.SetActive(true);
            eventText.SetKey(textKey);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="amount"></param>
        public void SetText(PartyMember pm, int amount) {
            eventText.SetDamageText(pm.memberName, amount);
        }

        /// <summary>
        /// 
        /// </summary>
        public void ClearText() {
            textBackground.gameObject.SetActive(false);
            eventText.Clear();
        }
    }
}
