using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bar : MonoBehaviour
{
    public Image frontFill;
    public LocalizedText text; 
    private int maxAmount { get; set; }
    private int currentAmount { get; set; }
    
    public void Init(int maxAmount, int currentAmount) {
        this.maxAmount = maxAmount;
        this.currentAmount = currentAmount;
        SetDisplay();
    }

    public void Init (int maxAmount, int currentAmount, Sprite s) {
        this.maxAmount = maxAmount;
        this.currentAmount = currentAmount;
        SetWidthAndPos(s.bounds.max.y);
        Debug.Log(s.bounds.min.x);
        Debug.Log(s.bounds.min.y);
        Debug.Log(s.bounds.max.x);
        Debug.Log(s.bounds.max.y);
        SetDisplay();
    }

    private void SetWidthAndPos(float width) {
        RectTransform parent = transform.parent.GetComponent<RectTransform>();
        //parent.sizeDelta = new Vector2(width, 16);
        //parent.position
        
    }

    private void SetDisplay() {
        text.Append(currentAmount.ToString());
        frontFill.fillAmount = currentAmount / maxAmount;
    }
}
