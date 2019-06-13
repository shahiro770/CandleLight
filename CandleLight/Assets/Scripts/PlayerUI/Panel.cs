/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: June 9, 2019
* 
* The Panel class manages various visuals that show the player the status of all members
* of their party. 
*
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerUI {

    public class Panel : MonoBehaviour {

        public virtual string GetPanelName() {
            return null;
        }
        
        public virtual Button GetNavigatableButton() {
            return null;
        }
    }
}