/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: July 4, 2019
* 
* The EventManager class manages the current scenarios (events) 
* the player will encounter based on the area they are in. 
*
*/

using Party;
using General;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using PlayerUI;
using System.Collections.Generic;

namespace Events {

    public class EventManager : MonoBehaviour {

        public static EventManager instance; /// <value> Singleton </value>

        private Area currentArea;           
        private SubArea currentSubArea;
        private Event currentEvent;
        private BackgroundPack[] bgPacks = new BackgroundPack[10];

        private Vector3 pos1d1 = new Vector3(0, 0, 0);
        private Vector3 pos2d1 = new Vector3(-150, 0, 0);
        private Vector3 pos2d2 = new Vector3(150, 0, 0);
        private Vector3 pos3d1 = new Vector3(-275, 0, 0);
        private Vector3 pos3d2 = new Vector3(0, 0, 0);
        private Vector3 pos3d3 = new Vector3(275, 0, 0);
        private string currentAreaName;
        private int packNum = 0;
        private int areaProgress = 0;       /// <value> Area progress increments by 1 for each main event the player completes </value>
        private int subAreaProgress = 0;    /// <value> When subareaProgress = 100, player is given the next event from the area </value>
        private bool isReady = false;       /// <value> Wait until EventManager is ready before starting </value>

        public float areaMultiplier { get; private set; } /// <value> Multiplier to results for events in the area </value>

        public EventDisplay[] eventDisplays = new EventDisplay[3]; /// <value> Displays for informational sprites that events might have </value>
        public EventDescription eventDescription;   /// <value> Display that describes the event in text </value>
        public Image eventBackground;               /// <value> Image background to event </value>
        public ActionsPanel actionsPanel;           /// <value> ActionsPanel reference </value>
        public PartyPanel partyPanel;               /// <value> PartyPanel reference </value>
        public StatusPanel statusPanel;


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
            StartCoroutine(StartArea(GameManager.instance.areaName));
        }

        /// <summary>
        /// Loads an Area from the database, waiting until all of subAreas, events,
        /// interactions, and results have been loaded before saying the area is ready.
        /// </summary>
        /// <param name="areaName"> Name of area to load </param>
        public void LoadArea(string areaName) {
            this.currentAreaName = areaName;
            currentArea = GameManager.instance.DB.GetAreaByName(areaName);

            LoadBackgroundPacks(areaName);
            SetAreaMultiplier();

            isReady = true;
        }

        public void LoadBackgroundPacks(string areaName) {
            string[] bgPackNames = GameManager.instance.DB.GetBGPackNames(areaName);

            for (int i = 0; i < bgPackNames.Length; i++) {
                if (bgPackNames[i] != "none") {
                    bgPacks[i] = GameManager.instance.DB.GetBGPack(areaName, bgPackNames[i]);
                    packNum++;
                }
            }
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
        /// Displays the first event in an area (first event of the main subArea)
        /// </summary>
        public void GetStartEvent() {
            currentSubArea = currentArea.GetSubArea("main");
            currentEvent = currentSubArea.GetEvent(areaProgress);

            DisplayEvent();
        }

        public void GetNextEvent(Interaction i) {
            Result r = i.GetResult();

            if (r.resultName != "none" && r.subAreaName != "none") {
                subAreaProgress = 0;
                currentSubArea = currentArea.GetSubArea(r.subAreaName);
                currentEvent = currentSubArea.GetEvent();     
            }
            else {
                subAreaProgress+= currentEvent.progressAmount;
                if (subAreaProgress >= 100) {
                    GetNextMainEvent();
                }
                else {
                    GetNextSubAreaEvent();
                }
            }

            DisplayEvent();
        }

        public void GetNextMainEvent() {
            subAreaProgress = 0;
            areaProgress++;

            currentSubArea = currentArea.GetSubArea("main");
            currentEvent = currentSubArea.GetEvent(areaProgress);
        }

        public void GetNextSubAreaEvent() {
            currentEvent = currentSubArea.GetEvent();
        }

        /// <summary>
        /// Displays the current event to the player
        /// </summary>
        public void DisplayEvent() {
            eventBackground.sprite = GetBGSprite(currentEvent.bgPackName);
            eventDescription.SetText(currentEvent.promptKey);
            DisplayEventSprites(currentEvent);

            statusPanel.DisplayPartyMember(PartyManager.instance.GetPartyMembers()[0]);
            actionsPanel.Init(currentEvent.isLeavePossible);
            actionsPanel.SetInteractionActions(currentEvent.interactions);
            partyPanel.SetHorizontalNavigation();
        }

        public Sprite GetBGSprite(string packName) {
            for (int i = 0; i < packNum; i++) {
                if (bgPacks[i].packName == packName) {
                    if (currentEvent.specificBGSprite != -1) {
                        return bgPacks[i].GetBackground(currentEvent.specificBGSprite);
                    }

                    return bgPacks[i].GetBackground();
                }
            }

            Debug.LogError("BackgroundPack of name" + packName +  "does not exist");
            return null;
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
