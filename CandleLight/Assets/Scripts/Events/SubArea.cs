using General;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

namespace Events {

    public class SubArea {

        public Event[] events = new Event[5];
        public string subAreaName;
        public int subAreaNum = 0;
       
        public SubArea(string subAreaName, string[] eventNames, IDbConnection dbConnection) {
            Debug.Log("SubArea " + subAreaName);
            this.subAreaName = subAreaName;

            for (int i = 0; i < events.Length; i++) {
                string eventName = eventNames[i];

                if (eventName != "none") {
                    events[i] = GameManager.instance.DB.GetEventByName(eventName, dbConnection);
                    subAreaNum++;
                }
            }

            
        }

        public Event GetEvent(int index) {
            return events[index];
        }
    }
}
