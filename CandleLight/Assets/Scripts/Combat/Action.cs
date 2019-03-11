using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Action : MonoBehaviour
{
    public Button b { get; set; }
    public LocalizedText actionText;
    public bool isEnabled { get; set; } = true;
    public string actionType { get; set; }
    public Attack a { get; set; }
    //public Color32 hoverColor;

    private ButtonTransitionState bts;
    private Image i;
    private TextMeshProUGUI t;

    void Awake() {
        b = GetComponent<Button>();
        bts = GetComponent<ButtonTransitionState>();
        i = GetComponent<Image>();
        t = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetAction(string actionType) {
        if (actionType == "flee") {
            SetKey("flee_action");
        }
        else if (actionType == "undo") {
            SetKey("undo_action");
        }

        this.actionType = actionType;
    }

    public void SetAction(string actionType, Attack a) {
        if (actionType == "attack") {
            if (a.nameKey == "none_attack") {
                this.actionType = "none";
                Disable();
            } else {
                this.a = a;
                this.actionType = actionType;
            }
            SetKey(a.nameKey);
        }
    }

    public void SelectAction() {
        bts.SetColor("pressed");
    }

    public void UnselectAction() {
        bts.SetColor("normal");
    }
    
    public void SetKey(string key) {
        actionText.SetKey(key);
    }

    // lowkey prevent the button from being spammed
    public void DisableInteraction() {
        b.interactable = false;
    }

    public void Disable() {
        UnselectAction();
        isEnabled = false;
        b.interactable = false;
        i.raycastTarget = false;
    }

    public void Enable() {
        isEnabled = true;
        b.interactable = true;
        i.raycastTarget = true;
    }
}
