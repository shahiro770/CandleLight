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
using General;
using Items;
using Party;
using PlayerUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Events {

    public class EventManager : MonoBehaviour {

        public static EventManager instance; /// <value> Singleton </value>

        /* external component references */
        public CombatManager combatManager;         /// <value> CombatManager reference </value>
        public EventDisplay[] eventDisplays = new EventDisplay[3]; /// <value> Displays for informational sprites that events might have </value>
        public EventDescription eventDescription;   /// <value> Display that describes the event in text </value>
        public CanvasGroup eventBGCanvas;           /// <value> Current event's background sprite alpha controller </value>
        public CanvasGroup nextEventBGCanvas;       /// <value> Next event's background sprite alpha controller </value>
        public Image eventBackground;               /// <value> Image background for current event </value>
        public Image nextEventBackground;           /// <value> Image background for next event </value>
        public RewardsPanel rewardsPanel;           /// <value> RewardsPanel reference </value>
        public ActionsPanel actionsPanel;           /// <value> ActionsPanel reference </value>
        public PartyPanel partyPanel;               /// <value> PartyPanel reference </value>
        public StatusPanel statusPanel;             /// <value> StatusPanel reference </value>
        public InfoPanel infoPanel;                 /// <value> InfoPanel reference </value>
        public TabManager utilityTabManager;           /// <value> Click on to display other panels </value>

        public float areaMultiplier { get; private set; }       /// <value> Multiplier to results for events in the area </value>
        public int subAreaProgress { get; private set; } = 0;   /// <value> When subareaProgress = 100, player is given the next event from the area </value>

        private Area currentArea;            /// <value> Area to select subAreas from </value>
        private SubArea currentSubArea;      /// <value> SubArea to select events from </value>
        private Event currentEvent;          /// <value> Event being displayed </value>
        private BackgroundPack[] bgPacks = new BackgroundPack[10];  /// <value> Background packs loaded in memory </value>
        private BackgroundPack bgPackPrev = null;
        private Sprite WAXSprite;
        private Sprite HPSprite;

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
        private float alphaLerpSpeed = 0.75f;   /// <value> Speed at which backgrounds fade in and out </value>
        private float colourLerpSpeed = 4f;     /// <value> Speed at which backgrounds change colour (for dimming) </value>
        private bool isReady = false;       /// <value> Wait until EventManager is ready before starting </value>
        private bool displayStartEvent = true;  /// <value> Flag for start event to have different visual effects </value>

        #region [Initialization] Initialization 

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
        /// TODO: Need to move this up to the GameManager, so that area scene doesn't start
        /// until after this area is loaded.
        /// </summary>
        /// <param name="areaName"> Name of area to load </param>
        public void LoadArea(string areaName) {
            this.currentAreaName = areaName;
            SetAreaMultiplier();
            currentArea = GameManager.instance.DB.GetAreaByName(areaName);

            LoadBackgroundPacks();
            LoadGeneralSprites();

            isReady = true;
        }
        
        /// <summary>
        /// Load backgroundPacks for an area
        /// </summary>
        /// <param name="areaName"> Name of area that will have its bgPacks loaded </param>
        public void LoadBackgroundPacks() {
            string[] bgPackNames = GameManager.instance.DB.GetBGPackNames(currentAreaName);

            for (int i = 0; i < bgPackNames.Length; i++) {
                if (bgPackNames[i] != "none") {
                    bgPacks[i] = GameManager.instance.DB.GetBGPack(currentAreaName, bgPackNames[i]);
                    bgPackNum++;
                }
            }
        }

        public void LoadGeneralSprites() {
            HPSprite = Resources.Load<Sprite>("Sprites/Interactions/WAXItem");
            
            switch(currentAreaName) {
                case "GreyWastes":
                    WAXSprite = Resources.Load<Sprite>("Sprites/Interactions/HealingEffect");
                    break;
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

        #region [Section 0] EventManagement

        /// <summary>
        /// Displays the first event in an area (first event of the main subArea)
        /// </summary>
        public void GetStartEvent() {
            currentSubArea = currentArea.GetSubArea("main");
            currentEvent = currentSubArea.GetEvent(areaProgress);

            StartCoroutine(DisplayEvent());
        }

        /// <summary>
        /// Gets the next event triggered by an interaction
        /// </summary>
        /// <param name="i"></param>
        public void GetNextEvent(Interaction i) {
            Result r = null;
            if (i != null) {   
                r = i.GetResult();
            }

            /* Check interaction to see if its sending player to a new subArea */
            if (r != null && r.name != "none" && r.subAreaName != "none") { 
                currentSubArea = currentArea.GetSubArea(r.subAreaName);
                subAreaProgress = 0;  
                if (infoPanel.isOpen) {
                    infoPanel.UpdateAmounts();
                }
                GetNextSubAreaEvent();
            }
            else {
                subAreaProgress += currentEvent.progressAmount;
                if (subAreaProgress >= 100) {
                    subAreaProgress = 100;
                }
                if (infoPanel.isOpen) {
                    infoPanel.UpdateAmounts();
                }

                if (subAreaProgress == 100) {
                    GetNextMainEvent();
                }
                else {
                    GetNextSubAreaEvent();
                }
            }

            StartCoroutine(DisplayEvent());
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
            string[] monstersToFight = currentSubArea.GetMonstersToSpawn();
            StartCoroutine(combatManager.InitializeCombat(monstersToFight));
        }

        /// <summary>
        /// Displays the current event to the player
        /// </summary>
        public IEnumerator DisplayEvent() {
            if (!displayStartEvent) {
                nextEventBackground.sprite = GetBGSprite(currentEvent.bgPackName);
                yield return StartCoroutine(TransitionToNextEvent());
            } 
            else {
                eventBackground.sprite = GetBGSprite(currentEvent.bgPackName);
                displayStartEvent = false;
            }

            if (currentEvent.promptKey == "combat_event") {
                StartCoroutine(AlterBackgroundColor(0.5f));
                eventDescription.SetKeyAndFadeIn(currentSubArea.GetCombatPrompt());
                GetCombatEvent();
            }
            else {
                if (currentEvent.promptKey == "nothing_event") {
                    eventDescription.SetKeyAndFadeIn(currentSubArea.GetNothingPrompt());
                }
                else {
                    eventDescription.SetKeyAndFadeIn(currentEvent.promptKey);
                }

                if (currentEvent.spriteNum > 0){
                    ShowEventDisplays();
                }
                else {
                    HideEventDisplays();
                }

                statusPanel.DisplayPartyMember(PartyManager.instance.GetPartyMembers()[0]);
                actionsPanel.Init(currentEvent.isLeavePossible);
                actionsPanel.SetInteractionActions(currentEvent.interactions);
                actionsPanel.SetAllActionsInteractable();
                utilityTabManager.SetAllButtonsInteractable();
                actionsPanel.SetHorizontalNavigation(partyPanel);
            }
        }

        /// <summary>
        /// Displays post combat information such as the RewardsPanel, and prepares player to continue exploring
        /// TODO Make the postCombat event have interactions in each action somehow
        /// </summary>
        /// <param> Flag for if combat ended due to fleeing </param>
        public IEnumerator DisplayPostCombat(bool isFleeSuccessful) {
            StartCoroutine(AlterBackgroundColor(1f));
            actionsPanel.SetPostCombatActions();
            actionsPanel.SetAllActionsUninteractable();
            rewardsPanel.SetVisible(true);
            if (isFleeSuccessful) {
                eventDescription.SetKeyAndFadeIn(currentSubArea.GetPostCombatFleePrompt());
            }
            else {
                eventDescription.SetKeyAndFadeIn(currentSubArea.GetPostCombatPrompt());
            }
            yield return StartCoroutine(rewardsPanel.Init(PartyManager.instance.GetPartyMembers(), combatManager.monstersKilled));
            if (infoPanel.isOpen) {
                infoPanel.UpdateAmounts();
            }
            actionsPanel.SetAllActionsInteractable();
        }

        public void DisplayInteraction(Interaction i) {
            Result r = i.GetResult();
            r.GenerateResults();

            List<Item> items = new List<Item>();

            if (i.GetSprite() != null) {
                eventDisplays[0].SetImage(i.GetSprite());
                eventDisplays[0].SetPosition(pos1d1);
            }

            if (r.EXPAmount > 0) {
                //items.Add(new Item("EXP", r.EXPAmount, EXPSprite));
            }
            if (r.HPAmount > 0) {
                items.Add(new Item("HP", r.HPAmount, HPSprite));
            }
            if (r.MPAmount > 0) {
                //items.Add(new Item("MP", r.MPAmount, MPSprite));
            }
            if (r.WAXAmount > 0) {
                items.Add(new Item("WAX", r.WAXAmount, WAXSprite));
            }

            if (items.Count > 0) {
                eventDisplays[0].SetItemDisplays(items);
            }
        }

        /// <summary>
        /// Performs visual effects when moving to next event
        /// </summary>
        /// <returns></returns>
        public IEnumerator TransitionToNextEvent() {
            eventDescription.FadeOut();
            HideEventDisplays();
            rewardsPanel.SetVisible(false);
            actionsPanel.SetAllActionsUninteractableAndFadeOut();
            utilityTabManager.SetAllButtonsUninteractable();
            yield return StartCoroutine(FadeBackgrounds());
        }

        /// <summary>
        /// Fades the current event's background out and the next event's background in
        /// </summary>
        /// <returns> IEnumerator for smooth animation </returns>
        public IEnumerator FadeBackgrounds() {
            float timeStartedLerping = Time.time;
            float timeSinceStarted = Time.time - timeStartedLerping;
            float percentageComplete = timeSinceStarted * alphaLerpSpeed;

            float alphaValue;
            float eventBGWidthValue;
            float eventBGHeightValue;

            while (nextEventBGCanvas.alpha < 1) {
                timeSinceStarted = Time.time - timeStartedLerping;
                percentageComplete = timeSinceStarted * alphaLerpSpeed;

                alphaValue = Mathf.Lerp(0, 1, percentageComplete);
                eventBGWidthValue = Mathf.Lerp(960, 1920, percentageComplete);
                eventBGHeightValue = Mathf.Lerp(300, 600, percentageComplete);

                eventBGCanvas.alpha = 1 - alphaValue;
                nextEventBGCanvas.alpha = alphaValue;
                eventBackground.rectTransform.sizeDelta = new Vector2(eventBGWidthValue, eventBGHeightValue);

                yield return new WaitForEndOfFrame();
            }

            eventBackground.sprite = nextEventBackground.sprite;
            eventBackground.rectTransform.sizeDelta = new Vector2(960, 300);
            eventBGCanvas.alpha = 1;
            nextEventBGCanvas.alpha = 0;          
        }

        /// <summary>
        /// Alters all of the background's colour values (r, g, b) to the specified value 
        /// </summary>
        /// <param name="targetColourValue"> Float between 0f and 1f </param>
        /// <returns> IEnumerator for smooth animation </returns>
        public IEnumerator AlterBackgroundColor(float targetColourValue) {
            float timeStartedLerping = Time.time;
            float timeSinceStarted = Time.time - timeStartedLerping;
            float percentageComplete = timeSinceStarted * colourLerpSpeed;

            float prevColorValue = eventBackground.color.r;
            float newColorValue;
            while (eventBackground.color.r != targetColourValue) {
                timeSinceStarted = Time.time - timeStartedLerping;
                percentageComplete = timeSinceStarted * colourLerpSpeed;

                newColorValue = Mathf.Lerp(prevColorValue, targetColourValue, percentageComplete);
                Color newColor = eventBackground.color;
                newColor.r = newColorValue;
                newColor.g = newColorValue;
                newColor.b = newColorValue;

                eventBackground.color = newColor;

                yield return new WaitForEndOfFrame();
            }
        }

        /// <summary>
        /// Returns a random sprite from a backgroundPack
        /// </summary>
        /// <param name="bgPackName"> Name of backgroundPack to load from </param>
        /// <returns></returns>
        public Sprite GetBGSprite(string bgPackName) {
            string confirmedBGPackName = bgPackName;

            if (confirmedBGPackName == "default") { // for events such as combat and nothing, use the last used bgPack
                confirmedBGPackName = currentSubArea.defaultBGPackName;
            }

            for (int i = 0; i < bgPackNum; i++) {
                if (bgPacks[i].name == confirmedBGPackName) {
                    if (currentEvent.specificBGSprite != -1) {
                        return bgPacks[i].GetBackground(currentEvent.specificBGSprite);
                    }

                    return bgPacks[i].GetBackground();
                }
            }

            Debug.LogError("BackgroundPack of name " + bgPackName +  "does not exist");
            return null;
        }

        #endregion

        #region [Section 1] EventDisplays

        /// <summary>
        /// Displays the event sprites in the eventDisplays
        /// </summary>
        /// <param name="e"> Event containing sprites </param>
        public void ShowEventDisplays() {
            if (currentEvent.spriteNum != 0) {
                if (currentEvent.spriteNum == 1) {
                    eventDisplays[0].SetImage(currentEvent.eventSprites[0]);

                    eventDisplays[0].SetPosition(pos1d1);

                    eventDisplays[0].SetVisible(true);
                }
                else if (currentEvent.spriteNum == 2) {
                    eventDisplays[0].SetImage(currentEvent.eventSprites[0]);
                    eventDisplays[1].SetImage(currentEvent.eventSprites[1]);

                    eventDisplays[0].SetPosition(pos2d1);
                    eventDisplays[1].SetPosition(pos2d2);

                    eventDisplays[0].SetVisible(true);
                    eventDisplays[1].SetVisible(true);
                }
                else {
                    eventDisplays[0].SetImage(currentEvent.eventSprites[0]);
                    eventDisplays[1].SetImage(currentEvent.eventSprites[1]);
                    eventDisplays[2].SetImage(currentEvent.eventSprites[2]);

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

        /// <summary>
        /// Hides specific eventDisplays
        /// </summary>
        public void HideEventDisplays(int[] indices) {
            foreach (int index in indices) {
                eventDisplays[index].SetVisible(false);
            }
        }

        #endregion

        /// <summary>
        /// Returns a Color32 based on the theme colour of the current area
        /// </summary>
        /// <returns> Color32 </returns>
        public Color32 GetThemeColour() {
            return currentArea.GetThemeColour();
        }

        /// <summary>
        /// Returns a Color32 based on the theme colour of the current area,
        /// brighter than the primary theme colour
        /// </summary>
        /// <returns> Color32 </returns>
        public Color32 GetSecondaryThemeColour() {
            return currentArea.GetSecondaryThemeColour();
        } 
    }
}
