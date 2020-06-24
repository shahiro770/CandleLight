/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: July 4, 2019
* 
* The Event class is used to store information about a scenario the player might find
* themselves in, holding backgrounds that might be displayed, sprites, text prompts, and more.
*
*/

using General;
using System.Data;
using UnityEngine;
using Eventconstants = Constants.EventConstants;

namespace Events {

    public class Event {
        
        public Interaction[] interactions  { get; private set; } = new Interaction[5]; /// <value> All events will have 5 interactions </value>
        public Sprite[] eventSprites { get; private set; } = new Sprite[3];            /// <value> Event's sprite </value>
        
        public string name { get; private set; }            /// <value> Name of event </value>
        public string type { get; private set; }            /// <value> Type of event </value>
        public string areaName { get; private set; }        /// <value> Name of area event occurs in for sprite purposes </value>
        public string bgPackName { get; private set; }      /// <value> Name of sprite array that this event uses to pick potential sprites </value>
        public string promptKey { get; private set; }       /// <value> Key for string describing event to player </value>
        public int chance { get; set; }                     /// <value> Chance of event occuring </value>
        public int progressAmount { get; private set; }     /// <value> Amount of points event contributes towards reaching the next main event </value>
        public int specificBGSprite { get; private set; }   /// <value> Index of specific background sprite in bgPack </value>
        public int spriteNum { get; private set; } = 0;     /// <value> Number of sprites </value>
        public bool isLeavePossible { get; private set; }   /// <value> True if leaving the event is possible, false otherwise </value>

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="eventName"> Name of event </param>
        /// <param name="areaName"> Name of area the event occurs in </param>
        /// <param name="type"> Type of event </param>
        /// <param name="promptKey"> Key for string describing event to player </param>
        /// <param name="interactionNames"> Names of interactions </param>
        /// <param name="isLeavePossible"> True if leaving the event is possible, false otherwise </param>
        /// <param name="possibleBackgrounds"> Array of indices for possible background sprites for an area </param>
        /// <param name="eventSpriteNames"> Names of sprites event displays </param>
        /// <param name="dbConnection"> 
        /// dbConnection will be passed down to each subArea and other storage classes
        /// to fetch information to save memory.
        /// </param>
        public Event(string name, string areaName, string type, int chance, int progressAmount, string[] interactionNames, 
        bool isLeavePossible, string bgPackName, int specificBGSprite, string[] eventSpriteNames, IDbConnection dbConnection) {
            this.name = name;
            this.areaName = areaName;
            this.type = type;
            this.chance = chance;
            this.progressAmount = progressAmount;
            this.promptKey = name + "_event";
            this.isLeavePossible = isLeavePossible;
            this.bgPackName = bgPackName;
            this.specificBGSprite = specificBGSprite;

            for (int i = 0; i < this.eventSprites.Length; i++) {
                if (eventSpriteNames[i] != "none") {
                    this.eventSprites[i] = Resources.Load<Sprite>("Sprites/Events/" + eventSpriteNames[i]);
                    spriteNum++;
                }
            }
            
            for (int i = 0; i < interactions.Length; i++) {
                string intName = interactionNames[i];
                interactions[i] = GameManager.instance.DB.GetInteractionByName(intName, dbConnection);
            }
        }

        /// <summary>
        /// Logs event details, such its name, type, and the area it should take place in
        /// </summary>
        public void LogEvent() {
            Debug.Log("Event: " + name + " type: " + type + " areaName: " + areaName);
        }
    }
}
