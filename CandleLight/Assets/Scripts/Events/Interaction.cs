/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 11, 2019
* 
* The Interaction class is used to store information about an action that can be taken
* to talk to or interact with things in scenarios.
*
*/

using General;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

namespace Events {

    public class Interaction {

        public string name { get; private set; }
        public string nameKey { get; private set; }
        public Result[] results = new Result[4];         /// <value> Result of the action if its an event action </value>

        public int resultNum = 0;

        public Interaction(string name, string[] resultNames, string[] resultKeys, IDbConnection dbConnection) {
            Debug.Log("Interaction " + name);
            this.name = name;
            this.nameKey = name + "_int";

            for (int i = 0; i < resultNames.Length; i++) {
                Debug.Log(i);
                if (resultNames[i] != "none") {
                    results[i] = GameManager.instance.DB.GetResultByName(resultNames[i], resultKeys[i], dbConnection);
                    resultNum++;
                }
            }
        }
    }
}
