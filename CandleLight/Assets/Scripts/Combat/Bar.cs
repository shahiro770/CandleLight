using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bar : MonoBehaviour
{
    public Image frontFill;
    public LocalizedText text; 
    private float maxAmount { get; set; }
    private float currentAmount { get; set; }
    
    public void Init(int maxAmount, int currentAmount) {
        this.maxAmount = maxAmount;
        this.currentAmount = currentAmount;
        SetDisplay();
    }

    public void Init (int maxAmount, int currentAmount, Vector2 size) {
        this.maxAmount = maxAmount;
        this.currentAmount = currentAmount;
        SetWidth(size.x);
        SetDisplay();
    }

    private void SetWidth(float width) {
        RectTransform rt = GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(width, rt.sizeDelta.y);
    }

    public void SetCurrent(int currentAmount) {
        this.currentAmount = currentAmount;
        SetDisplay();
    }

    private void SetDisplay() {
        text.Append(currentAmount.ToString());
        frontFill.fillAmount = currentAmount / maxAmount;
    }
}
