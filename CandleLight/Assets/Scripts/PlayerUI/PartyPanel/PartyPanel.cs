/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: June 9, 2019
* 
* The PartyPanel class manages various visuals that show the player the status of all members
* of their party. 
*
*/

using Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerUI {

    public class PartyPanel : MonoBehaviour {

        public PartyMemberDisplay[] pmDisplays = new PartyMemberDisplay[4];     /// <value> List of partymembers to display </value>
        
        /// <summary>
        /// Displays each partymember's information
        /// </summary>
        /// <param name="pms"></param>
        public void Init(List<PartyMember> pms) {
             for (int i = 0; i < pms.Count; i++) {
                pmDisplays[i].gameObject.SetActive(true);
                pmDisplays[i].Init(pms[i]);
            }
        }
    }
}
