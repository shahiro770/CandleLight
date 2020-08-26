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

    public void GetClassStats(string pm, int STR, int DEX, int INT, int LUK, int start, int cap) {
        Debug.Log("PM: " + pm);
        Debug.Log("LVL: " + start + " STR: " + STR + " DEX: " + DEX + " INT: " + INT + " LUK: " + LUK);
        for (int i = start + 1; i <= cap; i++) {
            if (pm == "warrior") {
                STR += (int)(i * 1.5);
                DEX += (int)(i * 1.5);
                INT += (int)(i * 1.25);
                LUK += (int)(i * 1.25);
                Debug.Log("LVL: " + i + " STR: " + STR + " DEX: " + DEX + " INT: " + INT + " LUK: " + LUK);
            }
            else if (pm == "mage") {
                STR += (int)(i * 0.75);
                DEX += (int)(i * 1.25);
                INT += (int)(i * 1.75);
                LUK += (int)(i * 1.5);
                Debug.Log("LVL: " + i + " STR: " + STR + " DEX: " + DEX + " INT: " + INT + " LUK: " + LUK);
            }
            else if (pm == "archer") {
                STR += (int)(i * 1.25);
                DEX += (int)(i * 1.75);
                INT += (int)(i * 1.5);
                LUK += (int)(i);
                Debug.Log("LVL: " + i + " STR: " + STR + " DEX: " + DEX + " INT: " + INT + " LUK: " + LUK);
            }
            else if (pm == "rogue") {
                STR += (int)(i);
                DEX += (int)(i * 1.5);
                INT += (int)(i * 1.25);
                LUK += (int)(i * 2.25);
                Debug.Log("LVL: " + i + " STR: " + STR + " DEX: " + DEX + " INT: " + INT + " LUK: " + LUK);
            }
        }
    }

    void Awake() {
        //print(Random.Range((int)(Mathf.Max(1, 2f * 0.5)), (int)(2 * (1 + 0.5))));
        GetClassStats("warrior", 8, 4, 3, 3, 1, 4);
        GetClassStats("mage", 3, 4, 10, 3, 1, 4);
        GetClassStats("archer", 3, 9, 6, 2, 1, 4);
        GetClassStats("rogue", 4, 6, 2, 8, 1, 4);
        GetStats("goblin", 5, 3, 1, 1, 1, 4);
        GetStats("Greyhide", 0, 7, 1, 3, 1, 4);
        // GetStats("stinger", 13, 5, 4, 4, 3, 4);
        // GetStats("FloatingArm",7, 4, 12, 6, 2, 3);
        GetStats("VampireBat", 3, 11, 5, 4 , 2, 4);
        GetStats("Lesser Wraith", 3, 8, 13,	2, 2, 4);
        // GetStats("Stranglehead", 11,	7,	0,	5, 2, 4);
        // GetStats("Spinal", 9,	2,	11,	5, 2, 4);
    }
}