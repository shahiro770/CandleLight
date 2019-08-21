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
        public string[] monsterPool;                /// <value> Pool of monsters subArea uses for random combat events </value>
        public string name;                         /// <value> Name of event </value>
        public string defaultBGPackName;            /// <value> Name of default backgroundPack for subArea</value>
        public int eventNum = 0;                    /// <value> Number of events in a subArea </value>
        public int subEventNum = 0;                 /// <value> Number of sub events in the subArea </value>
        private int minMonsterNum;                  /// <value> Minimum number of monsters per random encounter </value>
        private int maxMonsterNum;                  /// <value> Maximum number of monsters per random encounter </value>
        private int monsterNum = 0;                 /// <value> Number of monsters in monster pool</value>
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"> Name of subArea </param>
        /// <param name="eventNames"> String array of the names of each event in the subArea </param>
        /// <param name="dbConnection">
        /// dbConnection will be passed down to each subArea and other storage classes
        /// to fetch information to save memory.
        /// </param>
        public SubArea(string name, string[] eventNames, string[] monsterPool, int minMonsterNum, int maxMonsterNum, 
        string defaultBGPackName, IDbConnection dbConnection) {
            this.name = name;
            this.monsterPool = monsterPool;
            this.minMonsterNum = minMonsterNum;
            this.maxMonsterNum = maxMonsterNum;
            this.defaultBGPackName = defaultBGPackName;

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

            for (int i = 0; i < monsterPool.Length; i++) {
                if (monsterPool[i] != "none") {
                    monsterNum++;
                }
            }
        }

        /// <summary>
        /// Returns a random event from the subArea
        /// </summary>
        /// <returns> A random event </returns>
        /// <remark>
        /// Each event has a chance of occurence in an area.
        /// This chance number must be managed from within the database, 
        /// and the chance of all events combined must equal 100.
        /// </remark>
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

        /// <summary>
        /// Returns a specific event by name
        /// </summary>
        /// <param name="name"> Name of the event </param>
        /// <returns> An event from the events array </returns>
        public Event GetEvent(string name) {
            for (int i = 0; i < eventNum; i++) {
                if (events[i].name == name) {
                    return events[i];
                }
            }

            Debug.LogError("Event " + name + " does not exist");
            return null;
        }

        /// <summary>
        /// Returns an event by index
        /// </summary>
        /// <param name="index"> Indice in the events array </param>
        /// <returns> An event </returns>
        public Event GetEvent(int index) {
            return events[index];
        }

        /// <summary>
        /// Returns a specific subEvent by name
        /// </summary>
        /// <param name="name"> Name of the event </param>
        /// <returns> An event from the events array </returns>
        public Event GetSubEvent(string name) {
            for (int i = 0; i < subEventNum; i++) {
                if (subEvents[i].name == name) {
                    return subEvents[i];
                }
            }

            Debug.LogError("SubEvent " + name + " does not exist");
            return null;
        }

        /// <summary>
        /// Returns a random event prompt string based on the sub area's name for nothing events
        /// </summary>
        /// <returns> String that is a key text for a prompt in en.json </returns>
        /// <remark>
        /// There will always be exactly 4 random prompts per subArea
        /// </remark>
        public string GetNothingPrompt() {
            int index = Random.Range(0, 4);
            return  name + "_nothing_event_" + index.ToString();
        }

        /// <summary>
        /// Returns a random event prompt string based on the sub area's name for combat events
        /// </summary>
        /// <returns> String that is a key text for a prompt in en.json </returns>
        /// <remark>
        /// There will always be exactly 4 random prompts per subArea
        /// </remark>
        public string GetCombatPrompt() {
            int index = Random.Range(0, 4);
            return  name + "_combat_event_" + index.ToString();
        }

        /// <summary>
        /// Returns a random event prompt string based on the sub area's name for combat events
        /// that ended by killing all monsters
        /// </summary>
        /// <returns> String that is a key text for a prompt in en.json </returns>
        /// <remark>
        /// There will always be exactly 4 random prompts per subArea
        /// </remark>
        public string GetPostCombatPrompt() {
            int index = Random.Range(0, 4);
            return  name + "_post_combat_event_" + index.ToString();
        }

        /// <summary>
        /// Returns a random event prompt string based on the sub area's name for post combat events
        /// that were ended via fleeing
        /// </summary>
        /// <returns> String that is a key text for a prompt in en.json </returns>
        /// <remark>
        /// There will always be exactly 4 random prompts per subArea
        /// </remark>
        public string GetPostCombatFleePrompt() {
            int index = Random.Range(0, 4);
            return  name + "_flee_success_" + index.ToString();
        }

        /// <summary>
        /// Returns a list of the names of each monster being spawned
        /// </summary>
        /// <returns> String array </returns>
        public string[] GetMonstersToSpawn() {
            int numToSpawn = Random.Range(minMonsterNum, maxMonsterNum + 1);
            string[] monsterNames = new string[numToSpawn];

            for (int i = 0; i < numToSpawn; i++) {
                monsterNames[i] = this.monsterPool[Random.Range(0, monsterNum)];
            }

            return monsterNames;
        }
    }
}
