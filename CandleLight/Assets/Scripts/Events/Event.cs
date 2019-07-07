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

        public Interaction[] interactions = new Interaction[4]; /// <value> Max 4 interactions </value>
        public Sprite[] possibleBackgrounds = new Sprite[3];    /// <value> Possible backgrounds </value>
        public Sprite[] eventSprites = new Sprite[3];           /// <value> Event's sprite </value>
        
        public string eventName;        /// <value> Name of event </value>
        public string areaName;         /// <value> Name of area event occurs in for sprite purposes </value>
        public string promptKey;        /// <value> Key for string describing event to player </value>
        public int backgroundNum = 0;   /// <value> Number of backgrounds </value>
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
        public Event(string eventName, string areaName, string promptKey, string[] interactionNames, 
        bool isLeavePossible, int[] possibleBackgrounds, string[] eventSpriteNames, IDbConnection dbConnection) {
            string spritePath;

            this.eventName = eventName;
            this.areaName = areaName;
            this.promptKey = promptKey;
            this.isLeavePossible = isLeavePossible;

            for (int i = 0; i < this.eventSprites.Length; i++) {
                if (eventSpriteNames[i] != "none") {
                    this.eventSprites[i] = Resources.Load<Sprite>("Sprites/Events/" + eventSpriteNames[i]);
                    spriteNum++;
                }
            }

            for (int i = 0; i < this.possibleBackgrounds.Length; i++) {
                if (possibleBackgrounds[i] != -1) {
                    spritePath = "Sprites/Backgrounds/" + areaName + "/" + areaName + possibleBackgrounds[i];
                    Debug.Log(spritePath);
                    this.possibleBackgrounds[i] = Resources.Load<Sprite>(spritePath);
                    backgroundNum++;
                }
            }

            for (int i = 0; i < interactions.Length; i++) {
                string intName = interactionNames[i];

                if (intName != "none") {
                    interactions[i] = GameManager.instance.DB.GetInteractionByName(intName, dbConnection);
                }
            }
        }

        /// <summary>
        /// Returns a random background sprite from the array of possible backgroundSprites
        /// </summary>
        /// <returns></returns>
        public Sprite GetBackground() {
            return possibleBackgrounds[Random.Range(0, backgroundNum)];
        }
    }
}
