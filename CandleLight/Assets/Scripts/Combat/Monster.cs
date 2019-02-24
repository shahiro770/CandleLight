/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 11, 2019
* 
* The Monster class is used to store information about the Monster. 
*
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Monster : Character {

    //public RectTransform rect;
    public Image monsterSprite;
    public Bar HPBar;

    public string monsterName { get; set; }

    public void Init(string monsterName, string monsterArea, string monsterSize, int LVL, int HP, int MP, int[] stats, Attack[] attacks) {
        base.Init(LVL, HP, MP, stats, attacks);
        this.monsterName = monsterName;
        string spritePath = "Sprites/Enemies/" + monsterArea + "/" +  monsterName;
        monsterSprite.sprite = Resources.Load<Sprite>(spritePath);
        if (monsterSize == "Small") {
            monsterSprite.GetComponent<RectTransform>().localScale = new Vector3(0.9f, 0.9f, 0);
        } else if (monsterSize == "Medium") {
            monsterSprite.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 0);
        } else if (monsterSize == "Large") {
            monsterSprite.GetComponent<RectTransform>().localScale = new Vector3(1.2f, 1.2f, 0);
        }
                

                //RectTransform rt = canvas.GetComponent<RectTransform>();
                //rt.sizeDelta = new Vector2(100, 100);
        HPBar.Init(HP, CHP, monsterSprite.sprite);
    }

    public override void LogStats() {
        Debug.Log(monsterName);
        base.LogStats();
    }

    public override void LogName() {
        Debug.Log("Name " + monsterName);
    }


    /* /// use this for initialization
    void Start() {
        monsterSprite.sprite = SO.monsterSprite;
        
        health = SO.health;
    }*/
}
