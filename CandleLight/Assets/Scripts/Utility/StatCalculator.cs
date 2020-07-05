using UnityEngine;

public class StatCalculator : MonoBehaviour{

    public void GetStats(string monster, int STR, int DEX, int INT, int LUK, int start, int cap) {
        Debug.Log("Monster: " + monster);
         Debug.Log("LVL: " + start + " STR: " + STR + " DEX: " + DEX + " INT: " + INT + " LUK: " + LUK);
        for (int i = start + 1; i <= cap; i++) {
            STR += (int)(i * 0.5 + STR * 0.3);
            DEX += (int)(i * 0.5 + DEX * 0.3);
            INT += (int)(i * 0.5 + INT * 0.3);
            LUK += (int)(i * 0.5 + LUK * 0.3);
            Debug.Log("LVL: " + i + " STR: " + STR + " DEX: " + DEX + " INT: " + INT + " LUK: " + LUK);
        }
    }

    void Awake() {
        // GetStats("goblin", 5, 3, 1, 1, 1, 4);
        // GetStats("greyhide", 0, 7, 1, 3, 1, 4);
        // GetStats("stinger", 13, 5, 4, 4, 3, 4);
        GetStats("floatingArm",7, 4, 12, 6, 2, 3);
        GetStats("vampireBat", 3, 9, 5, 4 , 2, 4);
        GetStats("wraith", 4, 6, 13,	2, 2, 4);
        GetStats("stranglehead", 11,	7,	0,	5, 2, 4);
        GetStats("spinal", 9,	2,	11,	5, 2, 4);
    }
}