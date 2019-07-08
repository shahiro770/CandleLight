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

        public Event[] events = new Event[10];      /// <value> 10 Events max per subArea </value>
        public Event[] subEvents = new Event[10];   /// <value> 10 SubEvents max per subArea </value>
        public string subAreaName;              
        public int eventNum = 0;              /// <value> Number of events in a subArea</value>
        public int subEventNum = 0;
       
        public SubArea(string subAreaName, string[] eventNames, IDbConnection dbConnection) {
            this.subAreaName = subAreaName;

            for (int i = 0; i < events.Length; i++) {
                string eventName = eventNames[i];
                
                if (eventName != "none") {
                    Event newEvent = GameManager.instance.DB.GetEventByName(eventName, dbConnection);
                    
                    if (newEvent.chance != 0) {
                        events[eventNum] = newEvent;
                        eventNum++;
                    }
                    else {
                        subEvents[subEventNum] = newEvent;
                        subEventNum++;
                    }   
                }
            }
        }

        public Event GetEvent() {
            int eventChance = Random.Range(0, 100);

            for (int i = 0; i < eventNum; i++) {
                eventChance -= events[i].chance;

                if (eventChance < 0) {
                    return events[i];
                }
            }

            return null;
        }

        public Event GetEvent(string name) {
            for (int i = 0; i < eventNum; i++) {
                if (events[i].eventName == name) {
                    return events[i];
                }
            }

            Debug.LogError("Event " + name + " does not exist");
            return null;
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
