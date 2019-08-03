/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: July 4, 2019
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

        public string name { get; private set; }        /// <value> Name of interaction </value>
        public string nameKey { get; private set; }     /// <value> Localization of interaction </value>
        public int resultIndex { get; private set; }    /// <value> Index of the result chosen </value>

        private Result[] results = new Result[4];       /// <value> Result of the action if its an event action </value>
        private Sprite[] intSprites = new Sprite[4];    /// <value> List of sprites corresponding to each event action </value>
        private int resultNum = 0;                      /// <value> Amount of results for an interaction </value>

        /// <summary>
        /// Constructor, each array is of length 4
        /// </summary>
        /// <param name="name"> String name of interaction </param>
        /// <param name="resultNames"> String array of result names </param>
        /// <param name="resultKeys"> Keys for localization </param>
        /// <param name="spriteNames"> String array of each sprite </param>
        /// dbConnection will be passed down to each subArea and other storage classes
        /// to fetch information to save memory.
        /// </param>
        public Interaction(string name, string[] resultNames, string[] resultKeys, string[] spriteNames, IDbConnection dbConnection) {
            this.name = name;
            this.nameKey = name + "_int";

            for (int i = 0; i < resultNames.Length; i++) {
                if (resultNames[i] != "none") {
                    results[i] = GameManager.instance.DB.GetResultByName(resultNames[i], resultKeys[i], dbConnection);
                    if (spriteNames[i] != "none") {
                        intSprites[i] = Resources.Load<Sprite>("Sprites/Interactions/" + spriteNames[i]);
                    }
                    resultNum++;
                }
            }
        }

        /// <summary>
        /// Returns a random result
        /// </summary>
        /// <returns> A random result </returns>
        public Result GetResult() {
            resultIndex = Random.Range(0, resultNum);

            return results[resultIndex];
        }

        /// <summary>
        /// Returns the sprite related to the chosen result
        /// </summary>
        /// <returns> Sprite </returns>
        public Sprite GetSprite() {
            return intSprites[resultIndex];
        }
    }
}
