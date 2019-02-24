using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionsManager : MonoBehaviour {

    public Action[] actions = new Action[5];

    public void SetActionTexts(Attack[] attacks) {
        for (int i = 0; i < attacks.Length; i++) {
            actions[i].SetKey(attacks[i].nameKey);
            if (attacks[i].nameKey == "none_attack") {
                actions[i].Disable();
            }
        }
        actions[actions.Length - 1].SetKey("flee_action");
    }

    public void TakeAction() {
        
    }
}
