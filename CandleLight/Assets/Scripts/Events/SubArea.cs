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
using System.Data;
using UnityEngine;

namespace Events {

    public class SubArea {

        public Event[] events;                      /// <value> Events in the subArea </value>
        public Event[] subEvents;                   /// <value> SubEvents in the subArea (events with chance 0) </value>
        public string[] monsterPool;                /// <value> Pool of monsters subArea uses for random combat events </value>
        public string[] championBuffs;              /// <value> List of champion buffs that can occur a monster in this subArea (max 3) </value>
        public int[] monsterChances;                /// <value> Chance for each monster in the subArea to appear (must total to 100) </value>
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
        public SubArea(string name, string areaName, string[] eventNames, int[] eventChances, string[] monsterPool, int[] monsterChances, int minMonsterNum, int maxMonsterNum, 
        string defaultBGPackName, string[] championBuffs, IDbConnection dbConnection) {
            this.name = name;
            this.events = new Event[eventNames.Length];
            this.subEvents = new Event[eventNames.Length];
            this.monsterPool = monsterPool;
            this.monsterChances = monsterChances;
            this.minMonsterNum = minMonsterNum;
            this.maxMonsterNum = maxMonsterNum;
            this.defaultBGPackName = defaultBGPackName;
            this.championBuffs = championBuffs;

            for (int i = 0; i < events.Length; i++) {
                string eventName = eventNames[i];
                
                if (eventName != "none") {
                    Event newEvent = GameManager.instance.DB.GetEventByName(eventName, areaName, eventChances[i], dbConnection);
                    
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

                if (eventChance <= 0) {
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

        public Event GetEventByType(string type) {
            for (int i = 0; i < eventNum; i++) {
                if (events[i].type == type) {
                    return events[i];
                }
            }

            return null;
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
        /// Swaps an event and subevent, swapping their positions in the subEvents and events
        /// lists, as well as chances of occuring
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="subName"></param>
        public void SwapEventAndSubEvent(string eventName, string subName) {
            int index0 = -1;
            int index1 = -1;
            int tempChance = -1;
            Event tempEvent;

            for (int i = 0; i < eventNum; i++) {
                if (events[i].name == eventName) {
                    index0 = i; 
                    break;
                }
            }
            for (int i = 0; i < subEventNum; i++) {
                if (subEvents[i].name == subName) {
                    index1 = i;
                    break;
                }
            }

            if (index0 != -1 && index1 != -1) {   // only swap if both quest events are present
                tempEvent = events[index0];
                tempChance = events[index0].chance;

                events[index0] = subEvents[index1];
                subEvents[index1] = tempEvent;
                events[index0].chance  = subEvents[index1].chance;
                subEvents[index1].chance = tempChance;
            }
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
        /// Returns a random event prompt string based on the sub area's name and a custom string for combat events
        /// that ended by killing all monsters
        /// </summary>
        /// <param name="customName"> Custom string to base post combat prompt off of</param>
        /// <returns> String that is a key text for a prompt in en.json </returns>
        /// <remark>
        /// There will always be exactly 4 random prompts per custom string prompt (cause variety is nice)
        /// </remark>
        public string GetCustomPostCombatPrompt(string customName) {
            int index = Random.Range(0, 4);
            return name + "_" + customName + "_post_combat_event_" + index.ToString();
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
        /// Returns a list of the names of each monster being spawned.
        /// List length is assumed to be at most 5.
        /// </summary>
        /// <returns> String array </returns>
        public string[] GetMonstersToSpawn() {
            int numToSpawn = Random.Range(minMonsterNum, maxMonsterNum + 1);
            string[] monsterNames = new string[numToSpawn];

            for (int i = 0; i < numToSpawn; i++) {
                int monsterChance = Random.Range(0, 100);

                for (int j = 0; j < monsterNum; j++) {
                    monsterChance -= monsterChances[j];

                    if (monsterChance <= 0) {
                        monsterNames[i] = monsterPool[j];
                        break;
                    }
                }
            }

            return monsterNames;
        }

        public string GetMonsterToSpawn() {
            int monsterChance = Random.Range(0, 100);

                for (int i = 0; i < monsterNum; i++) {
                    monsterChance -= monsterChances[i];

                    if (monsterChance <= 0) {
                        return monsterPool[i];
                        
                    }
                }

            return null;
        }
    }
}
