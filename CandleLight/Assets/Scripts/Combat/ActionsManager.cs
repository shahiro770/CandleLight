using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Combat;

public class ActionsManager : MonoBehaviour {

    public CombatManager cm { get; set; }
    public Action[] actions = new Action[5];

    private Action selectedAction;
    private EventSystem es;

    void Awake() {
        es = EventSystem.current;
    }

    //public void SetInteractionActions(Interaction[] interactions) {}

    public void SetAttackActions(Attack[] attacks) {
        for (int i = 0; i < attacks.Length; i++) {
            actions[i].SetAction("attack", attacks[i]);
        }
        actions[actions.Length - 1].SetAction("flee");

        SetInitialNavigation();
    }

    public void AttackActionSelected(Action a) {
        selectedAction = a;
        for (int i = 0; i < actions.Length - 1;i++) {
            if (actions[i].actionType != "none") {
                actions[i].Disable();  
            } 
        }
        actions[actions.Length - 1].SetAction("undo");

        selectedAction = a;
        cm.PreparePMAttack(a);
        a.SelectAction();
    }

    public void DisableAllActions() {
        for (int i = 0; i < actions.Length;i++) {
            if (actions[i].actionType != "none") {
                actions[i].Disable();  
            } 
        }
    }

    public void DisableAllActionInteractions() {
        for (int i = 0; i < actions.Length;i++) {
            if (actions[i].actionType != "none") {
                actions[i].Disable();  
            } 
        }
    }

    public void EnableAllActions() {
        UndoAttackActionSelected();
        es.SetSelectedGameObject(actions[0].b.gameObject);
    }

    public void UndoAttackActionSelected() {
        for (int i = 0; i < actions.Length ;i++) {
            if (actions[i].actionType != "none") {
                actions[i].Enable();  
            }
            if (actions[i] == selectedAction) {
                actions[i].UnselectAction();
                selectedAction = null;
            }
        }
        actions[actions.Length - 1].SetAction("flee");
        
        cm.UndoPMAction();
    }
    
    public void SelectAction(Action a) {
        if (a.actionType == "attack") {
            AttackActionSelected(a);
        }
        else if (a.actionType == "undo") {
            UndoAttackActionSelected();
        }
        else if (a.actionType == "run") {

        }
    }

    public Button GetActionButton(int index) {
        return actions[index].b;
    }

    public void ResetFifthButtonNavigation() {
         Button b = actions[4].GetComponent<Button>();
         Navigation n = b.navigation;

        //if (phase == "actionSelect") {
        n.selectOnUp = actions[2].isEnabled ? n.selectOnUp : actions[0].GetComponent<Button>();
        b.navigation = n;
    }

    public void SetButtonNavigation(int index, string direction, Button b2) {
         Button b = actions[index].GetComponent<Button>();
         Navigation n = b.navigation;

        if (direction == "up") {
            n.selectOnUp = b2;
            b.navigation = n;
        }
        else if (direction == "right") {
            n.selectOnRight = b2;
            b.navigation = n;
        }
        else if (direction == "down") {
            n.selectOnDown = b2;
            b.navigation = n;
        }
        else if (direction == "left") {
            n.selectOnLeft = b2;
            b.navigation = n;
        }
    }

    private void SetInitialNavigation() {
        for (int i = 0; i < actions.Length; i++) {
            if (actions[i].isEnabled) {
                Button b = actions[i].GetComponent<Button>();
                Navigation n = b.navigation;
                if (i == 0) {
                    //n.selectOnRight = actions[1].isEnabled ? n.selectOnRight : 
                    n.selectOnDown = actions[2].isEnabled ? n.selectOnDown : actions[4].GetComponent<Button>();
                    n.selectOnRight = actions[1].isEnabled ? n.selectOnRight : null;
                    b.navigation = n;
                }    
                else if (i == 1) {
                    n.selectOnDown = actions[3].isEnabled ? n.selectOnDown : actions[4].GetComponent<Button>();
                    b.navigation = n;
                }
                else if (i == 2) {
                    //n.selectOnRight = actions[3].isEnabled ? n.selectOnRight : 
                }
                else if (i == 4) {
                    n.selectOnUp = actions[2].isEnabled ? n.selectOnUp : actions[0].GetComponent<Button>();
                    b.navigation = n;
                }
            }
        }             
    }
}
