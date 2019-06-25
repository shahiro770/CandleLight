/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 11, 2019
* 
* The Event class is used to hold information for an event the player randomly encounters.
*
*/

using Actions;
using General;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Events {

    public class Event {

        public System.Random rnd = new System.Random();
        public Interaction[] interactions = new Interaction[4];
        public Sprite[] possibleBackgrounds = new Sprite[3];
        public Sprite[] eventSprites = new Sprite[3];         /// <value> Event's sprite </value>
        
        public string eventName;
        public string areaName;
        public string promptKey;
        public int backgroundNum = 0;           /// <value> Number of possible backgrounds </value>
        public int spriteNum = 0;
        public bool isLeavePossible;


        public Event(string eventName, string areaName, string promptKey, string[] interactionNames, 
        bool isLeavePossible, int[] possibleBackgrounds, string[] eventSpriteNames, IDbConnection dbConnection) {
            Debug.Log("Event " + eventName);
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

            string spritePath;

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

        public Sprite GetBackground() {
            return possibleBackgrounds[Random.Range(0, backgroundNum)];
        }
    }
}
