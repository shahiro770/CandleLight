/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 11, 2019
* 
* The Character class is used to store information about characters that can act in combat. 
*
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour {
    public int ID { get; set; }
    public int LVL { get; set; }
    public int HP { get; set; }
    public int CHP { get; set; }  // current HP
    public int MP { get; set; }
    public int CMP { get; set; } // current MP
    public int STR { get; set; }
    public int DEX { get; set; }
    public int INT { get; set; }
    public int LUK { get; set; }
    public Attack[] attacks { get; set; }

    public virtual void Init(int LVL, int HP, int MP, int[] stats, Attack[] attacks) {
        this.LVL = LVL;
        this.HP = HP;
        this.CHP = HP;
        this.MP = MP;
        this.CMP = MP;
        this.STR = stats[0];
        this.DEX = stats[1];
        this.INT = stats[2];
        this.LUK = stats[3];
        this.attacks = attacks; 
    }

    public virtual IEnumerator LoseHP(int amount) {
        CHP -= amount;

        yield break;
    }

    public virtual IEnumerator LoseMP(int amount) {
        CMP -= amount;

        yield break;
    }

    public virtual void LogStats() {
        Debug.Log(LVL + " " + HP + " " + MP + " " + STR + " " + DEX + " " + INT + " " + LUK);
    }

    public virtual void LogName() {
        Debug.Log("George");
    }

    public virtual void LogAttacks() {
        Debug.Log("1 " + attacks[0].nameKey + " 2 " + attacks[1].nameKey  + " 3 " + attacks[2].nameKey  + " 4 " + attacks[3].nameKey );
    }
}