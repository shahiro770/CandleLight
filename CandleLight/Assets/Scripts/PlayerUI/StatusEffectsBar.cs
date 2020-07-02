/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 11, 2019
* 
* The EffectsBar class is used to display status effects a character might have on them.
* These includes buffs, and debuffs.
* Left out for now just because it doesn't look good.
*
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Characters;

namespace PlayerUI {

    public class StatusEffectsBar : MonoBehaviour {

        public StatusEffectDisplay[] seds;
        
        public void Init(PartyMember pm) {
            // for (int i = 0;i < seds.Length; i++) {
            //     if (i < pm.statusEffects.Count) {
            //         if (seds[i].gameObject.activeSelf == false) {
            //             seds[i].gameObject.SetActive(true);
            //         }
            //         seds[i].MirrorDisplay(pm.statusEffects[i]);
            //     }
            //     else {
            //         if (seds[i].gameObject.activeSelf == false) {
            //             break;
            //         }
            //         else {
            //             seds[i].UnmirrorDisplay();
            //             seds[i].gameObject.SetActive(false);
            //         }
            //     }
            // }
        }
    }
}
