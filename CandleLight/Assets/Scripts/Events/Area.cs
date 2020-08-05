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
        public int subAreasNum  { get; private set; }= 0;   /// <value> Amount of subAreas </value>

        private SubArea[] subAreas = new SubArea[10];   /// <value> Max SubArea amount is 10 </value>    
        private Sprite[] subAreaCards = new Sprite[10]; /// <value> Sprites depicting the subArea's core message </value>
        private string themeColour;                     /// <value> Name of the colour used to accent UIs while in area </value>
       
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"> Name of the area </param>
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
                    subAreaCards[i] = Resources.Load<Sprite>("Sprites/SubAreas/" + name + "/" + i);
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
        /// Returns the index of a given subArea's name 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int GetSubAreaIndex(string name) {
            for (int i = 0; i < subAreas.Length; i++) {
                if (subAreas[i].name == name) {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// subAreas[0] will always be main
        /// </summary>
        /// <returns> A SubArea </returns>
        public SubArea GetSubArea(int index) {
            return (subAreas[index]);
        }

        public Sprite GetSubAreaCard(int index) {
            return subAreaCards[index];
        }

        /// <summary>
        /// Swaps all events and subevents in all subareas with the specified names.
        /// This is done as quests may take place over multiple subAreas, hence if a quest is 
        /// started in one subArea, its next part must be prepared in the next.
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="subEventName"></param>
        public void SwapEventAndSubEvent(string eventName, string subEventName) {
            for (int i = 0; i < subAreasNum; i++) {
                subAreas[i].SwapEventAndSubEvent(eventName, subEventName);
            }
        }

        /// <summary>
        /// Resets all subAreas to their original event pools
        /// </summary>
        public void ResetSubAreas() {
            for (int i = 0; i < subAreasNum; i++) {
                subAreas[i].ResetEvents();
            }
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
                    return new Color32(180, 180, 180, 255);
            }
        }
    }
}
