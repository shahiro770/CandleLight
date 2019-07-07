/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: July 4, 2019
* 
* The EventManager class manages the current scenarios (events) 
* the player will encounter based on the area they are in. 
*
*/

using General;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using PlayerUI;

namespace Events {

    public class EventManager : MonoBehaviour {

        public static EventManager instance; /// <value> Singleton </value>

        private Area currentArea;           
        private SubArea currentSubArea;
        private Event currentEvent;

        private Vector3 pos1d1 = new Vector3(0, 0, 0);
        private Vector3 pos2d1 = new Vector3(-150, 0, 0);
        private Vector3 pos2d2 = new Vector3(150, 0, 0);
        private Vector3 pos3d1 = new Vector3(-275, 0, 0);
        private Vector3 pos3d2 = new Vector3(0, 0, 0);
        private Vector3 pos3d3 = new Vector3(275, 0, 0);
        private string currentAreaName;
        private bool isReady = false;       /// <value> Wait until EventManager is ready before starting </value>

        public float areaMultiplier { get; private set; } /// <value> Multiplier to results for events in the area </value>

        public EventDisplay[] eventDisplays = new EventDisplay[3]; /// <value> Displays for informational sprites that events might have </value>
        public EventDescription eventDescription;   /// <value> Display that describes the event in text </value>
        public Image eventBackground;               /// <value> Image background to event </value>
        public ActionsPanel actionsPanel;           /// <value> ActionsPanel reference </value>



        /// <summary>
        /// Awake to instantiate singleton
        /// </summary>
        void Awake() {
            if (instance == null) {
                instance = this;
            }
            else if (instance != this) {
                DestroyImmediate (gameObject);
                instance = this;
            }
        }

        /// <summary>
        /// Starts the player in the GreyWastes area for now
        /// </summary>
        void Start() {
            LoadArea("GreyWastes");
        
            StartCoroutine(StartArea("GreyWastes"));
        }

        /// <summary>
        /// Loads an Area from the database, waiting until all of subAreas, events,
        /// interactions, and results have been loaded before saying the area is ready.
        /// </summary>
        /// <param name="areaName"> Name of area to load </param>
        public void LoadArea(string areaName) {
            this.currentAreaName = areaName;
            currentArea = GameManager.instance.DB.GetAreaByName(areaName);
            SetAreaMultiplier();

            isReady = true;
        }

        /// <summary>
        /// Waits until the area is done being loaded before starting the player's
        /// adventure with their first event in the area.
        /// </summary>
        /// <param name="areaName"> Name of area </param>
        /// <returns> Yields to wait for area to load </returns>
        public IEnumerator StartArea(string areaName) {
            LoadArea(areaName);
            while (isReady == false) {
                yield return null;
            }
            GetStartEvent();
        }

        /// <summary>
        /// Displays the first event in an area (always the first subArea in the area)
        /// </summary>
        public void GetStartEvent() {
            currentSubArea = currentArea.GetSubArea(0);
            currentEvent = currentSubArea.GetEvent(0);

            DisplayCurrentEvent();
        }

        /// <summary>
        /// Displays the current event to the player
        /// </summary>
        public void DisplayCurrentEvent() {
            eventBackground.sprite = currentEvent.GetBackground();
            eventDescription.SetText(currentEvent.promptKey);
            DisplayEventSprites(currentEvent);
        }

        /// <summary>
        /// Sets a multiplier for results from events in the area
        /// (e.g. gold results get increased in later areas)
        /// </summary>
        private void SetAreaMultiplier() {
            switch(currentAreaName) {
                case "GreyWastes":
                    areaMultiplier = 1.0f;
                    break;
            }
        }

        /// <summary>
        /// Displays the event sprites in the eventDisplays
        /// </summary>
        /// <param name="e"> Event containing sprites </param>
        public void DisplayEventSprites(Event e) {
            if (e.spriteNum != 0) {
                if (e.spriteNum == 1) {
                    eventDisplays[0].SetImage(e.eventSprites[0]);

                    eventDisplays[0].SetPosition(pos1d1);

                    eventDisplays[0].SetVisible(true);
                }
                else if (e.spriteNum == 2) {
                    eventDisplays[0].SetImage(e.eventSprites[0]);
                    eventDisplays[1].SetImage(e.eventSprites[1]);

                    eventDisplays[0].SetPosition(pos2d1);
                    eventDisplays[1].SetPosition(pos2d2);

                    eventDisplays[0].SetVisible(true);
                    eventDisplays[1].SetVisible(true);
                }
                else {
                    eventDisplays[0].SetImage(e.eventSprites[0]);
                    eventDisplays[1].SetImage(e.eventSprites[1]);
                    eventDisplays[2].SetImage(e.eventSprites[2]);

                    eventDisplays[0].SetPosition(pos3d1);
                    eventDisplays[1].SetPosition(pos3d2);
                    eventDisplays[2].SetPosition(pos3d3);

                    eventDisplays[0].SetVisible(true);
                    eventDisplays[1].SetVisible(true);
                    eventDisplays[2].SetVisible(true);
                }
            }
        }

        /// <summary>
        /// Hides the eventDisplays
        /// </summary>
        public void HideEventSprites() {
            foreach (EventDisplay ed in eventDisplays) {
                ed.SetVisible(false);
            }
        }
    }
}
