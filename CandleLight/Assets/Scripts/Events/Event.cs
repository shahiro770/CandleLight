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

namespace Events {

    public class Event {
        
        public Interaction[] interactions = new Interaction[5]; /// <value> Max 4 interactions </value>
        public Sprite[] eventSprites = new Sprite[3];           /// <value> Event's sprite </value>
        
        public string eventName;        /// <value> Name of event </value>
        public string areaName;         /// <value> Name of area event occurs in for sprite purposes </value>
        public string bgPackName;       /// <value> Name of sprite array that this event uses to pick potential sprites </value>
        public string promptKey;        /// <value> Key for string describing event to player </value>
        public int chance;
        public int progressAmount;
        public int specificBGSprite;     /// <value> Index of specific background sprite in bgPack </value>
        public int spriteNum = 0;       /// <value> Number of sprites </value>
        public bool isLeavePossible;    /// <value> True if leaving the event is possible, false otherwise </value>


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="eventName"> Name of event </param>
        /// <param name="areaName"> Name of area event occurs in </param>
        /// <param name="promptKey"> Key for string describing event to player </param>
        /// <param name="interactionNames"> Names of interactions </param>
        /// <param name="isLeavePossible"> True if leaving the event is possible, false otherwise </param>
        /// <param name="possibleBackgrounds"> Array of indices for possible background sprites for an area </param>
        /// <param name="eventSpriteNames"> Names of sprites event displays </param>
        /// <param name="dbConnection"> 
        /// dbConnection will be passed down to each subArea and other storage classes
        /// to fetch information to save memory.
        /// </param>
        public Event(string eventName, string areaName, int chance, int progressAmount, string promptKey, string[] interactionNames, 
        bool isLeavePossible, string bgPackName, int specificBGSprite, string[] eventSpriteNames, IDbConnection dbConnection) {
            this.eventName = eventName;
            this.areaName = areaName;
            this.chance = chance;
            this.progressAmount = progressAmount;
            this.promptKey = promptKey;
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
    }
}
