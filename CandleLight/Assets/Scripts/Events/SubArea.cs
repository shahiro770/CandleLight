/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: July 4, 2019
* 
* The SubArea class stores all the events that can happen in part of an area.
* An area is composed of several subAreas to give the player the feeling of entering
* biomes with unique events.
*
*/

using General;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

namespace Events {

    public class SubArea {

        public Event[] events = new Event[5];   /// <value> 5 Events max per subArea</value>
        public string subAreaName;              
        public int eventNum = 0;              /// <value> Number of events in a subArea</value>
       
        public SubArea(string subAreaName, string[] eventNames, IDbConnection dbConnection) {
            Debug.Log("SubArea " + subAreaName);
            this.subAreaName = subAreaName;

            for (int i = 0; i < events.Length; i++) {
                string eventName = eventNames[i];
                
                if (eventName != "none") {
                    events[i] = GameManager.instance.DB.GetEventByName(eventName, dbConnection);
                    eventNum++;
                }
            }
        }

        /// <summary>
        /// Returns an event by index
        /// </summary>
        /// <param name="index"> Indice </param>
        /// <returns> An Event </returns>
        public Event GetEvent(int index) {
            return events[index];
        }
    }
}
