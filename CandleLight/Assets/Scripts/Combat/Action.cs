using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Action : MonoBehaviour
{
    public LocalizedText actionText;
    private Button b;
    private Image i;
    private Attack a;

    void Awake() {
        b = GetComponent<Button>();
        i = GetComponent<Image>();
    }
    
    public void SetKey(string key) {
        actionText.SetKey(key);
    }

    public void Disable() {
        b.enabled = false;
        i.raycastTarget = false;

    }

    public void Enable() {
        b.enabled = true;
        i.raycastTarget = true;
    }
}
