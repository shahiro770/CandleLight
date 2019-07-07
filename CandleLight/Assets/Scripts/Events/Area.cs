/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: July 4, 2019
* 
* The Area class is used to store information about a dungeon the player is in.
* It holds a number of SubAreas, each with events for the player to discover.
*
*/

using General;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

namespace Events {

    public class Area {

        public SubArea[] subAreas = new SubArea[10];    /// <value> Max SubArea amount is 10 </value>
        public string areaName;                         /// <value> Name of the area </value>      
        public int subAreasNum = 0;                     /// <value> Amount of subAreas </value>

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="areaName"> Name of the area </param>
        /// <param name="subAreaNames"> Names of each subArea in the area </param>
        /// <param name="dbConnection"> 
        /// dbConnection will be passed down to each subArea and other storage classes
        /// to fetch information to save memory.
        /// </param>
        public Area(string areaName, string[] subAreaNames, IDbConnection dbConnection) {
            Debug.Log("Area " + areaName);
            this.areaName = areaName;

            for (int i = 0; i < subAreas.Length; i++) {
                string subAreaName = subAreaNames[i];
                if (subAreaName != "none") {
                    Debug.Log(subAreaName);
                    subAreas[i] = GameManager.instance.DB.GetSubAreaByAreaName(subAreaName, dbConnection);
                    subAreasNum++;
                }
            }

            
        }

        /// <summary>
        /// subAreas[0] will always be the start area
        /// subAreas[subAreaNum - 1] will be the exit
        /// subAreas[subAreaNum - 2] will be the boss
        /// </summary>
        /// <returns> A SubArea </returns>
        public SubArea GetSubArea(int index) {
            return subAreas[index];
        }
    }
}
