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

        public string name { get; private set; }        /// <value> Name of the area </value>  

        private SubArea[] subAreas = new SubArea[10];   /// <value> Max SubArea amount is 10 </value>    
        private string themeColour;                     /// <value> Name of the colour used to accent UIs while in area </value>
        private int subAreasNum = 0;                    /// <value> Amount of subAreas </value>

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="areaName"> Name of the area </param>
        /// <param name="subAreaNames"> Names of each subArea in the area </param>
        /// <param name="themeColour"> Colour to theme the UI with while this area is being explored </param>
        /// <param name="dbConnection"> 
        /// dbConnection will be passed down to each subArea and other storage classes
        /// to fetch information to save memory.
        /// </param>
        public Area(string name, string[] subAreaNames, string themeColour, IDbConnection dbConnection) {
            this.name = name;
            this.themeColour = themeColour;

            for (int i = 0; i < subAreas.Length; i++) {
                string subAreaName = subAreaNames[i];
                if (subAreaName != "none") {
                    subAreas[i] = GameManager.instance.DB.GetSubAreaByAreaName(subAreaName, name, dbConnection);
                    subAreasNum++;
                }
            }
        }

        /// <summary>
        /// subAreas[0] will always be main
        /// </summary>
        /// <returns> A SubArea </returns>
        public SubArea GetSubArea(string name) {
            foreach (SubArea sa in subAreas) {
                if (sa.name == name) {
                    return sa;
                }
            }

           Debug.LogError("SubArea " + name + " does not exist");
           return null;
        }

        /// <summary>
        /// Returns a Color32 based on the theme colour
        /// </summary>
        /// <returns> Color32 </returns>
        public Color GetThemeColour() {
            switch (themeColour) {
                case "grey":
                    return new Color32(133, 133, 133, 255);
                default:
                    return new Color32(133, 133, 133, 255);
            }
        }

        /// <summary>
        /// Returns a Color32 based on the theme colour
        /// brighter than the primary theme colour
        /// </summary>
        /// <returns> Color32 </returns>
        public Color GetSecondaryThemeColour() {
            switch (themeColour) {
                case "grey":
                    return new Color32(180, 180, 180, 255);
                default:
                    return new Color32(133, 133, 133, 255);
            }
        }
    }
}
