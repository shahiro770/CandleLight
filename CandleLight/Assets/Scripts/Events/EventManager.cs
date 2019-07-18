/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: July 4, 2019
* 
* The EventManager class manages the current scenarios (events) 
* the player will encounter based on the area they are in. 
*
*/

using CombatManager = Combat.CombatManager;
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

        /* external component references */
        public CombatManager combatManager;  /// <value> CombatManager reference </value>
        public EventDisplay[] eventDisplays = new EventDisplay[3]; /// <value> Displays for informational sprites that events might have </value>
        public EventDescription eventDescription;   /// <value> Display that describes the event in text </value>
        public Image eventBackground;               /// <value> Image background to event </value>
        public ActionsPanel actionsPanel;           /// <value> ActionsPanel reference </value>
        public PartyPanel partyPanel;               /// <value> PartyPanel reference </value>
        public StatusPanel statusPanel;             /// <value> StatusPanel reference </value>

        public float areaMultiplier { get; private set; } /// <value> Multiplier to results for events in the area </value>

        private Area currentArea;            /// <value> Area to select subAreas from </value>
        private SubArea currentSubArea;      /// <value> SubArea to select events from </value>
        private Event currentEvent;          /// <value> Event being displayed </value>
        private BackgroundPack[] bgPacks = new BackgroundPack[10];  /// <value> Background packs loaded in memory </value>

        /* eventDisplay coordinates */
        private Vector3 pos1d1 = new Vector3(0, 0, 0);
        private Vector3 pos2d1 = new Vector3(-150, 0, 0);
        private Vector3 pos2d2 = new Vector3(150, 0, 0);
        private Vector3 pos3d1 = new Vector3(-275, 0, 0);
        private Vector3 pos3d2 = new Vector3(0, 0, 0);
        private Vector3 pos3d3 = new Vector3(275, 0, 0);

        private string currentAreaName;     /// <value> Name of current area </value>
        private int bgPackNum = 0;          /// <value> Number of backgroundPacks </value>
        private int areaProgress = 0;       /// <value> Area progress increments by 1 for each main event the player completes </value>
        private int subAreaProgress = 0;    /// <value> When subareaProgress = 100, player is given the next event from the area </value>
        private bool isReady = false;       /// <value> Wait until EventManager is ready before starting </value>

        #region Initialization

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
        
        /// <summary>
        /// Load backgroundPacks for an area
        /// </summary>
        /// <param name="areaName"> Name of area that will have its bgPacks loaded </param>
        public void LoadBackgroundPacks(string areaName) {
            string[] bgPackNames = GameManager.instance.DB.GetBGPackNames(areaName);

            for (int i = 0; i < bgPackNames.Length; i++) {
                if (bgPackNames[i] != "none") {
                    bgPacks[i] = GameManager.instance.DB.GetBGPack(areaName, bgPackNames[i]);
                    bgPackNum++;
                }
            }
        }

        /// <summary>
        /// Sets a multiplier for results from events in the area
        /// (i.e. gold results get increased in later areas)
        /// </summary>
        private void SetAreaMultiplier() {
            switch(currentAreaName) {
                case "GreyWastes":
                    areaMultiplier = 1.0f;
                    break;
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

        #endregion

        #region EventManagement

        /// <summary>
        /// Displays the first event in an area (first event of the main subArea)
        /// </summary>
        public void GetStartEvent() {
            currentSubArea = currentArea.GetSubArea("main");
            currentEvent = currentSubArea.GetEvent(areaProgress);

            DisplayEvent();
        }

        /// <summary>
        /// Gets the next event triggered by an interaction
        /// </summary>
        /// <param name="i"></param>
        public void GetNextEvent(Interaction i) {
            Result r = i.GetResult();

            if (r.name != "none" && r.subAreaName != "none") {
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

        /// <summary>
        /// Gets the next event in the subArea "main" of an area
        /// </summary>
        public void GetNextMainEvent() {
            subAreaProgress = 0;
            areaProgress++;

            currentSubArea = currentArea.GetSubArea("main");
            currentEvent = currentSubArea.GetEvent(areaProgress);
        }

        /// <summary>
        /// Gets the next random event in the current subArea
        /// </summary>
        public void GetNextSubAreaEvent() {
            currentEvent = currentSubArea.GetEvent();
        }

        /// <summary>
        /// Switches gameplay from exploring into turn-based combat with random monsters
        /// </summary>
        public void GetCombatEvent() {
            string[] monstersToFight = new string[] {"Goblin LVL1", "Greyhide LVL1", "Greyhide Alpha LVL3", "Goblin LVL1"};
            StartCoroutine(combatManager.InitializeCombat(monstersToFight));
        }

        /// <summary>
        /// Displays the current event to the player
        /// </summary>
        public void DisplayEvent() {
            eventBackground.sprite = GetBGSprite(currentEvent.bgPackName);
            eventDescription.SetText(currentEvent.promptKey);
            ShowEventDisplays(currentEvent);

            statusPanel.DisplayPartyMember(PartyManager.instance.GetPartyMembers()[0]);
            actionsPanel.Init(currentEvent.isLeavePossible);
            actionsPanel.SetInteractionActions(currentEvent.interactions);
            partyPanel.SetHorizontalNavigation();
        }

        /// <summary>
        /// Returns a random sprite from a backgroundPack
        /// </summary>
        /// <param name="bgPackName"> Name of backgroundPack to load from </param>
        /// <returns></returns>
        public Sprite GetBGSprite(string bgPackName) {
            for (int i = 0; i < bgPackNum; i++) {
                if (bgPacks[i].name == bgPackName) {
                    if (currentEvent.specificBGSprite != -1) {
                        return bgPacks[i].GetBackground(currentEvent.specificBGSprite);
                    }

                    return bgPacks[i].GetBackground();
                }
            }

            Debug.LogError("BackgroundPack of name" + bgPackName +  "does not exist");
            return null;
        }

        #endregion

        #region EventDisplays

        /// <summary>
        /// Displays the event sprites in the eventDisplays
        /// </summary>
        /// <param name="e"> Event containing sprites </param>
        public void ShowEventDisplays(Event e) {
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
        public void HideEventDisplays() {
            foreach (EventDisplay ed in eventDisplays) {
                ed.SetVisible(false);
            }
        }

        #endregion
    }
}
