/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: July 4, 2019
* 
* The EventManager class manages the current scenarios (events) 
* the player will encounter based on the area they are in. 
* TODO: Bug with partyMembers having status effects after dying (should be cleared)
*
*/

using CombatManager = Combat.CombatManager;
using Constants;
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

        public static EventManager instance;        /// <value> Singleton </value>

        /* external component references */
        public CombatManager combatManager;         /// <value> CombatManager reference </value>
        public EventDisplay[] eventDisplays = new EventDisplay[3];  /// <value> Displays for informational sprites that events might have </value>
        public EventDescription eventDescription;   /// <value> Display that describes the event in text </value>
        public Canvas eventCanvas;                  /// <value> Canvas holding all other canvases </value>
        public CanvasGroup eventBGCanvas;           /// <value> Current event's background sprite alpha controller </value>
        public CanvasGroup nextEventBGCanvas;       /// <value> Next event's background sprite alpha controller </value>
        public Image eventBackground;               /// <value> Image background for current event </value>
        public Image nextEventBackground;           /// <value> Image background for next event </value>
        public RewardsPanel rewardsPanel;           /// <value> RewardsPanel reference </value>
        public ToastPanel toastPanel0;              /// <value> ToastPanel reference </value>
        public ToastPanel toastPanel1;              /// <value> ToastPanel reference </value>
        public GearPanel gearPanel;                 /// <value> GearPanel reference </value>
        public CandlesPanel candlesPanel;           /// <value> CandlesPanel reference </value>
        public SpecialPanel specialPanel;           /// <value> SpecialPanel reference </value>
        public ActionsPanel actionsPanel;           /// <value> ActionsPanel reference </value>
        public PartyPanel partyPanel;               /// <value> PartyPanel reference </value>
        public SkillsPanel skillsPanel;             /// <value> SkillsPanel reference </value>
        public StatusPanel statusPanel;             /// <value> StatusPanel reference </value>
        public InfoPanel infoPanel;                 /// <value> InfoPanel reference </value>
        public TabManager itemsTabManager;          /// <value> Click on to display other panels with item information </value>
        public TabManager utilityTabManager;        /// <value> Click on to display other panels with utillity information </value>
        
        public Special strangeBottle;       /// <value> Penultimate item to the plot is placed in the player's inventory at the start </value>
        public float canvasWidth = 960;     /// <value> gameObject positions on the screen are scaled via the canvas, change this number if scaling changes </value>
        public float canvasHeight = 540;    /// <value> gameObject positions on the screen are scaled via the canvas, change this number if scaling changes </value>
        public float canvasScaleFactor = 1 / 0.01851852f;   /// <value> Factor to scale up position values in code</value>
        public float areaMultiplier { get; private set; }       /// <value> Multiplier to results for events in the area </value>
        public int subAreaProgress { get; private set; } = 0;   /// <value> When subareaProgress = 100, player is given the next event from the area </value>

        private Area currentArea;            /// <value> Area to select subAreas from </value>
        private SubArea currentSubArea;      /// <value> SubArea to select events from </value>
        private Event currentEvent;          /// <value> Event being displayed </value>
        private Result currentResult;        /// <value> Result being obtained </value>
        private BackgroundPack[] bgPacks;    /// <value> Background packs loaded in memory </value>
        private Consumable[] subAreaConsumables;   /// <value> Consumable items that can be found in the current subArea </value>
        private Gear[] subAreaGear;          /// <value> Gear items that can be found in the current subArea </value>
        private Candle[] subAreaCandles;     /// <value> Candle items that can be found in the current subArea </value>
        private Special[] subAreaSpecials;   /// <value> Special items that can be found in the current subArea</value>

        /* eventDisplay coordinates */
        private Vector3 pos1d1 = new Vector3(0, -20, 0);
        private Vector3 pos2d1 = new Vector3(-150, -20, 0);
        private Vector3 pos2d2 = new Vector3(150, -20, 0);
        private Vector3 pos3d1 = new Vector3(-275, -20, 0);
        private Vector3 pos3d2 = new Vector3(0, -20, 0);
        private Vector3 pos3d3 = new Vector3(275, -20, 0);

        private enum checkIndicators { NONE, STR, DEX, INT, LUK, ITEM, ITEMANDCLEAR };
        private string[] monstersToSpawn;       /// <value> List of monsters to spawn </value>
        private string currentAreaName;         /// <value> Name of current area </value>
        private string nextSubArea = "";        /// <value> Name of next subArea to move to </value>
        private int bgPackNum = 0;              /// <value> Number of backgroundPacks </value>
        private int areaProgress = 0;           /// <value> Area progress increments by 1 for each main event the player completes </value>
        private int consumablesNum = 0;         /// <value> Number of consumables to be found in the subArea </value>
        private int gearNum = 0;                /// <value> Number of gear to be found in the subArea </value>
        private int candleNum = 0;              /// <value> Number of candles to be found in the subArea </value>
        private int specialNum = 0;             /// <value> Number of special to be found in the subArea </value>
        private int shopToastIndex = 0;         /// <value> Index for which toastPanel is being used as the shop's wax display </value>
        private float alphaLerpSpeed = 0.75f;   /// <value> Speed at which backgrounds fade in and out </value>
        private float colourLerpSpeed = 4f;     /// <value> Speed at which backgrounds change colour (for dimming) </value>
        private bool isReady = false;           /// <value> Wait until EventManager is ready before starting </value>
        private bool displayStartEvent = true;  /// <value> Flag for start event to have different visual effects </value>

        #region [Initialization] Initialization 

        /// <summary>
        /// Awake to instantiate singleton
        /// </summary>
        void Awake() {
            Application.targetFrameRate = 60;

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
            currentSubArea = currentArea.GetSubArea("main" + currentAreaName);

            LoadBackgroundPacks();
            LoadGeneralInteractions();
            LoadSubAreaItems();

            isReady = true;
        }
        
        /// <summary>
        /// Load backgroundPacks for the current area
        /// </summary>
        public void LoadBackgroundPacks() {
            string[] bgPackNames = GameManager.instance.DB.GetBGPackNames(currentAreaName);
            bgPacks = new BackgroundPack[bgPackNames.Length];

            for (int i = 0; i < bgPackNames.Length; i++) {
                if (bgPackNames[i] != "none") {
                    bgPacks[i] = GameManager.instance.DB.GetBGPack(currentAreaName, bgPackNames[i]);
                    bgPackNum++;
                }
            }
        }

        /// <summary>
        /// Loads consumables for the current subArea
        /// </summary>
        public void LoadConsumables() {
            subAreaConsumables = GameManager.instance.DB.GetConsumablesBySubArea(currentSubArea.name);
            consumablesNum = 0;

            for (int i = 0; i < subAreaConsumables.Length; i++) {
                if (subAreaConsumables[i].nameID != "none") {
                    consumablesNum++;
                }
                else {
                    break;
                }
            }
        }

        /// <summary>
        /// Loads gear for the current subArea
        /// </summary>
        public void LoadGear() {
            subAreaGear = GameManager.instance.DB.GetGearBySubArea(currentSubArea.name);
            gearNum = 0;

            for (int i = 0; i < subAreaGear.Length; i++) {
                if (subAreaGear[i].nameID != "none") {
                    gearNum++;
                }
                else {
                    break;
                }
            }
        }

        /// <summary>
        /// Loads candles for the current subArea
        /// </summary>
        public void LoadCandles() {
            subAreaCandles = GameManager.instance.DB.GetCandlesBySubArea(currentSubArea.name);
            candleNum = 0;

            for (int i = 0; i < subAreaCandles.Length; i++) {
                if (subAreaCandles[i] != null) {
                    candleNum++;
                }
                else {
                    break;
                }
            }
        }

        /// <summary>
        /// Loads specials for the current subArea
        /// </summary>
        public void LoadSpecials() {
            subAreaSpecials = GameManager.instance.DB.GetSpecialsBySubArea(currentSubArea.name);
            specialNum = 0;

            for (int i = 0; i < subAreaSpecials.Length; i++) {
                if (subAreaSpecials[i] != null) {
                    specialNum++;
                }
                else {
                    break;
                }
            }

            if (currentSubArea.name == "mainGreyWastes") {
                strangeBottle = subAreaSpecials[0];
            }
        }

        /// <summary>
        /// Loads all items for a subArea
        /// </summary>
        public void LoadSubAreaItems() {
            LoadConsumables();
            LoadGear();
            LoadCandles();
            LoadSpecials();
        }

        /// <summary>
        /// Load general interactions that many events might use
        /// TODO: find a better place to put this
        /// </summary>
        public void LoadGeneralInteractions() {
            Interaction travelInt = GameManager.instance.DB.GetInteractionByName("travel");
            Interaction fightInt = GameManager.instance.DB.GetInteractionByName("fight");
            actionsPanel.SetGeneralInteractions(travelInt, fightInt);
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

        #region [Section 0] TutorialStuff
        
        public void GenerateStartingWeapon() {
            // Gear startingWeapon = null;
            // if (className == "warrior") {
            //     startingWeapon = (Gear)GameManager.instance.DB.GetItemByNameID("WarriorWeapon-1", "Gear");
            // }
            // else if (className == "mage") {
            //     startingWeapon = (Gear)GameManager.instance.DB.GetItemByNameID("MageWeapon-1", "Gear");
            // }
            // else if (className == "archer") {
            //     startingWeapon = (Gear)GameManager.instance.DB.GetItemByNameID("ArcherWeapon-1", "Gear");
            // }
            // else if (className == "rogue") {
            //     startingWeapon = (Gear)GameManager.instance.DB.GetItemByNameID("RogueWeapon-1", "Gear");
            // }

            // EquipGear(startingWeapon, "weapon");
        }

        #endregion

        #region [Section 1] EventManagement

        /// <summary>
        /// Displays the first event in an area (first event of the main subArea)
        /// </summary>
        public void GetStartEvent() {
            currentEvent = currentSubArea.GetEvent(areaProgress);

            StartCoroutine(DisplayEvent());
        }

        /// <summary>
        /// Gets the next event
        /// </summary>
        public void GetNextEvent() {
            actionsPanel.SetActionsUsable(true);
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
            

            StartCoroutine(DisplayEvent());
        }

        /// <summary>
        /// Gets the next event in the subArea "main" of an area
        /// </summary>
        public void GetNextMainEvent() {
            subAreaProgress = 0;
            
            if (currentSubArea.name == "tombsGreyWastes") {
                areaProgress = 6;
            }
            else {
                areaProgress++;;
            }
            currentSubArea = currentArea.GetSubArea("main" + currentAreaName);
            if (areaProgress >= currentSubArea.eventNum) {
                currentEvent = currentSubArea.GetEvent(currentSubArea.eventNum - 1);
            }
            else {
                currentEvent = currentSubArea.GetEvent(areaProgress);
            }
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
            gearPanel.SetTakeable(false);
            candlesPanel.SetTakeable(false);
            skillsPanel.SetTogglable(false);
            specialPanel.SetTakeable(false);
            StartCoroutine(AlterBackgroundColor(0.5f));
            StartCoroutine(combatManager.InitializeCombat(monstersToSpawn, currentSubArea.championBuffs, currentEvent.isLeavePossible));
        }

        /// <summary>
        /// Displays the current event to the player
        /// </summary>
        public IEnumerator DisplayEvent() {
            StartCoroutine(PartyManager.instance.TriggerStatuses(false));

            if (displayStartEvent == false) { 
                nextEventBackground.sprite = GetBGSprite(currentEvent.bgPackName);
                yield return StartCoroutine(TransitionToNextEvent());
            } 
            else {  // for very first event in an area, there is no need to visually transition (just blit onto screen)
                eventBackground.sprite = GetBGSprite(currentEvent.bgPackName);
                displayStartEvent = false;
                PartyManager.instance.SetActivePartyMember(PartyManager.instance.GetFirstPartyMemberAlive());
                statusPanel.DisplayPartyMember(PartyManager.instance.GetActivePartyMember().pmvc);
            }

            PartyManager.instance.RegenParty();

            if (currentEvent.type == EventConstants.COMBAT) {
                eventDescription.SetKeyAndFadeIn(currentSubArea.GetCombatPrompt());
                monstersToSpawn = currentSubArea.GetMonstersToSpawn();
                
                GetCombatEvent();
            }
            else {
                if (currentEvent.type == EventConstants.NOTHING) {
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

                actionsPanel.Init(currentEvent.isLeavePossible);
                actionsPanel.SetInteractionActions(currentEvent.interactions);
                SetAllButtonsInteractable(true);
            }
        }

        /// <summary>
        /// Displays the current event to the player, with no visual transitions unless the background image is specified
        /// </summary>
        public IEnumerator DisplaySubEvent() {
            if (currentSubArea.name == "endGreyWastes") {
                StartCoroutine(PartyManager.instance.TriggerStatuses(false));
            }
            if (currentEvent.specificBGSprite != -1) {
                nextEventBackground.sprite = GetBGSprite(currentEvent.bgPackName);
                yield return StartCoroutine(TransitionToNextEvent());
            }
            if (currentEvent.type == EventConstants.COMBAT) {
                eventDescription.SetKey(currentSubArea.GetCombatPrompt());
                monstersToSpawn = currentSubArea.GetMonstersToSpawn();
                
                GetCombatEvent();
            }
            else {
                if (currentEvent.type == EventConstants.NOTHING) {
                    if (currentEvent.specificBGSprite != -1) {
                        eventDescription.SetKeyAndFadeIn(currentSubArea.GetNothingPrompt());
                    }
                    else {
                        eventDescription.SetKey(currentSubArea.GetNothingPrompt());
                    }
                }
                else {
                    if (currentEvent.specificBGSprite != -1) {
                        eventDescription.SetKeyAndFadeIn(currentEvent.promptKey);
                    }
                    else {
                        eventDescription.SetKey(currentEvent.promptKey);
                    }
                }

                if (currentEvent.spriteNum > 0){
                    ShowEventDisplays();
                }
                else {
                    HideEventDisplays();
                }

                statusPanel.DisplayPartyMember(PartyManager.instance.GetFirstPartyMemberAlive().pmvc);
                PartyManager.instance.SetActivePartyMember(PartyManager.instance.GetActivePartyMember());
                actionsPanel.Init(currentEvent.isLeavePossible);
                actionsPanel.SetInteractionActions(currentEvent.interactions);
                SetAllButtonsInteractable(true);
            }
        }

        /// <summary>
        /// Displays post combat information such as the RewardsPanel, and prepares player to continue exploring
        /// TODO: Make the postCombat event have interactions in each action somehow
        /// </summary>
        /// <param name="endString"> String constant explaining how combat ended </param>
        public IEnumerator DisplayPostCombat(string endString) {
            if (endString == "DEFEAT") {
                StartCoroutine(DisplayGameOver());
            }
            else {
                StartCoroutine(AlterBackgroundColor(1f));
                actionsPanel.ClearAllActions();
                rewardsPanel.SetVisible(true);
                PartyManager.instance.SetActivePartyMember(PartyManager.instance.GetActivePartyMember());
                
                if (endString == "FLEE") {
                    eventDescription.SetKeyAndFadeIn(currentSubArea.GetPostCombatFleePrompt());
                }
                else if (endString == "VICTORY") {
                    if (currentResult.hasPostCombatPrompt == true) {
                        eventDescription.SetKeyAndFadeIn(currentSubArea.GetCustomPostCombatPrompt(currentResult.name));
                    }
                    else {
                        eventDescription.SetKeyAndFadeIn(currentSubArea.GetPostCombatPrompt());
                    }
                }

                yield return StartCoroutine(rewardsPanel.Init(PartyManager.instance.GetPartyMembers(), combatManager.monstersKilled));
                if (infoPanel.isOpen) {
                    infoPanel.UpdateAmounts();
                }
                           
                gearPanel.SetTakeable(true);
                candlesPanel.SetTakeable(true);
                specialPanel.SetTakeable(true);
                skillsPanel.SetTogglable(true);

                actionsPanel.PostCombatActions(rewardsPanel.itemNum);
                SetAllButtonsInteractable(true);
                
            }
        }

        /// <summary>
        /// Returns items found as a result of an interaction
        /// </summary>
        /// <param name="r"> Result containing items </param>
        /// <returns> List of items </returns>
        public List<Item> GetResultItems(Result r) {
            r.GenerateResults();

            List<Item> items = new List<Item>();
            if (r.specificItemAmount > 0) {
                for (int i = 0; i < r.itemAmount; i++) {
                    string specificItemName = r.specificItemNames[Random.Range(0, r.specificItemAmount)];
                    if (r.itemType == ItemConstants.CONSUMABLE) {
                        for (int j = 0; j < consumablesNum; j++) {
                            if (subAreaConsumables[j].nameID == specificItemName) {
                                items.Add(new Consumable(subAreaConsumables[j]));
                                ((Consumable)items[i]).RandomizeAmounts(r.itemQuality);
                                break;
                            }
                        }
                    }
                    else if (r.itemType == ItemConstants.GEAR) {
                        for (int j = 0; j < gearNum; j++) {
                            if (subAreaGear[j].nameID == specificItemName) {
                                items.Add(new Gear(subAreaGear[j]));
                                ((Gear)items[i]).RandomizeAmounts(r.itemQuality);
                                break;
                            }
                        }
                    }
                    else if (r.itemType == ItemConstants.CANDLE) {
                        for (int j = 0; j < candleNum; j++) {
                            if (subAreaCandles[j].nameID == specificItemName) {
                                items.Add(new Candle(subAreaCandles[j]));
                                break;
                            }
                        }
                    }
                    else if (r.itemType == ItemConstants.SPECIAL) {
                        for (int j = 0; j < specialNum; j++) {
                            if (subAreaSpecials[j].nameID == specificItemName) {
                                items.Add(new Special(subAreaSpecials[j]));
                                break;
                            }
                        }
                    }
                    if (items.Count == 0) {
                        Debug.LogError("Item " + specificItemName + " could not be generated");
                    }
                }
            }
            else {
                for (int i = 0; i < r.itemAmount; i++) {
                    if (r.itemType == ItemConstants.CONSUMABLE) {
                        items.Add(new Consumable(subAreaConsumables[Random.Range(0, consumablesNum)]));
                        ((Consumable)items[i]).RandomizeAmounts(r.itemQuality);
                    }
                    else if (r.itemType == ItemConstants.GEAR) {
                        items.Add(new Gear(subAreaGear[Random.Range(0, gearNum)]));
                         ((Gear)items[i]).RandomizeAmounts(r.itemQuality);
                    }
                    else if (r.itemType == ItemConstants.CANDLE) {
                        items.Add(new Candle(subAreaCandles[Random.Range(0, candleNum)]));
                        ((Candle)items[i]).RandomizeAmounts(r.itemQuality);
                    }
                    else if (r.itemType == ItemConstants.SPECIAL) {
                        items.Add(new Special(subAreaSpecials[Random.Range(0, specialNum)]));
                    }
                }
            }

            if (items.Count == 0) {
                Debug.LogError("No items were generated");
            }

            return items;
        }

        /// <summary>
        /// Does something depending on the interaction selected by the player
        /// </summary>
        /// <param name="i"> Interaction object </param>
        public IEnumerator Interact(Interaction i) {
            bool changeSprite = true; // flag to change the event's sprite to the result's sprite 

            if (i.checkIndicator != (int)checkIndicators.NONE) {  // events that are statChecks will have a good and bad outcome
                if (i.checkIndicator == (int)checkIndicators.ITEMANDCLEAR) {
                    if (specialPanel.CheckItem(i.itemToCheck, true) == true) {
                        currentResult = i.GetResult(0); // good result
                    }
                    else {
                        currentResult = i.GetResultStartIndex(1); // bad result(s)
                    }
                }
                else {
                    if (PartyManager.instance.GetPrimaryStatAll(i.checkIndicator) + 
                        (int)(PartyManager.instance.GetPrimaryStatAll((int)checkIndicators.LUK) * 0.2f) >= Random.Range((int)i.statThreshold * 0.6f, (int)i.statThreshold * 1.3f)) {
                        currentResult = i.GetResult(0); // good result
                    }
                    else {
                        currentResult = i.GetResultStartIndex(1); // bad result(s)
                    }
                }
            }
            else {
                currentResult = i.GetResult();
            }

            if (currentResult.type == ResultConstants.NORESULT) {
                eventDescription.SetKey(currentResult.resultKey);
            }
            else if (currentResult.type == ResultConstants.NORESULTANDLEAVE) {
                eventDescription.SetKey(currentResult.resultKey);
                actionsPanel.TravelActions();
                HideEventDisplayItemDisplays();
            }
            else if (currentResult.type == ResultConstants.TAKEALLITEMS) {
                TakeAllItems();
            }
            else if (currentResult.type == ResultConstants.ITEM) {
                actionsPanel.SetItemActions();
                eventDescription.SetKey(currentResult.resultKey);
                DisplayResultItems(currentResult);
            }
            else if (currentResult.type == ResultConstants.NEWINT) {
                actionsPanel.AddInteraction(currentResult.newIntName);
                eventDescription.SetKey(currentResult.resultKey);
            }
            else if (currentResult.type == ResultConstants.EVENT) {
                GetNextEvent();
            }
            else if (currentResult.type == ResultConstants.SUBEVENT) {       
                currentEvent = currentSubArea.GetSubEvent(currentResult.subEventName);
                yield return StartCoroutine(DisplaySubEvent());
            }
            else if (currentResult.type == ResultConstants.ITEMWITHSUBEVENT) {  // subEvents do not need result prompts
                currentEvent = currentSubArea.GetSubEvent(currentResult.subEventName);
                yield return StartCoroutine(DisplaySubEvent());
                DisplayResultItems(currentResult);
            }
            else if (currentResult.type == ResultConstants.SUBAREA) {
                if (currentResult.subAreaName0 != "none") { 
                    currentSubArea = currentArea.GetSubArea(currentResult.subAreaName0);
                    StartCoroutine(DataManager.instance.LoadMonsterDisplays(currentSubArea.monsterPool));
                    LoadSubAreaItems();
                    subAreaProgress = 0; 
                    if (infoPanel.isOpen == true) {
                        infoPanel.UpdateAmounts();
                    }
                }

                GetNextEvent();
            }
            else if (currentResult.type == ResultConstants.SUBAREAANDCOMBAT) {
                currentSubArea = currentArea.GetSubArea(currentResult.subAreaName0);
                StartCoroutine(DataManager.instance.LoadMonsterDisplays(currentSubArea.monsterPool));
                LoadSubAreaItems();
                subAreaProgress = 0;
                if (infoPanel.isOpen == true) {
                    infoPanel.UpdateAmounts();
                }

                monstersToSpawn = currentResult.GetMonstersToSpawn();

                for (int j = 0; j < monstersToSpawn.Length; j++) {
                    if (monstersToSpawn[j] == "none") {
                        monstersToSpawn[j] = currentSubArea.GetMonsterToSpawn();
                    }
                }
                eventDescription.SetKey(currentResult.resultKey);
                HideEventDisplays();     
                GetCombatEvent();
            }
            else if (currentResult.type == ResultConstants.SUBAREAANDCOMBATANDSUBAREA) {
                currentSubArea = currentArea.GetSubArea(currentResult.subAreaName0);
                StartCoroutine(DataManager.instance.LoadMonsterDisplays(currentSubArea.monsterPool));
                LoadSubAreaItems();
                subAreaProgress = 0;
                if (infoPanel.isOpen == true) {
                    infoPanel.UpdateAmounts();
                }

                monstersToSpawn = currentResult.GetMonstersToSpawn();

                for (int j = 0; j < monstersToSpawn.Length; j++) {
                    if (monstersToSpawn[j] == "none") {
                        monstersToSpawn[j] = currentSubArea.GetMonsterToSpawn();
                    }
                }
                eventDescription.SetKey(currentResult.resultKey);
                HideEventDisplays();     
                GetCombatEvent();
            }
            else if (currentResult.type == ResultConstants.STATSINGLE) {
                eventDescription.SetKey(currentResult.resultKey);
                ApplyResultStatChangesSingle(currentResult, ResultConstants.STATSINGLE);
            }
            else if (currentResult.type == ResultConstants.STATALL) {
                eventDescription.SetKey(currentResult.resultKey);
                ApplyResultStatChangesAll(currentResult, ResultConstants.STATALL);
                CheckGameOver();
            }
            else if (currentResult.type == ResultConstants.STATALLANDLEAVE) {
                eventDescription.SetKey(currentResult.resultKey);
                ApplyResultStatChangesAll(currentResult, ResultConstants.STATALLANDLEAVE);
                actionsPanel.TravelActions();
                CheckGameOver();
            }
            else if (currentResult.type == ResultConstants.STATALLANDITEMANDLEAVE) {
                eventDescription.SetKey(currentResult.resultKey);
                ApplyResultStatChangesAll(currentResult, ResultConstants.STATALLANDITEMANDLEAVE);
                DisplayResultItems(currentResult);
                actionsPanel.SetItemActions();         
                CheckGameOver();
            }
            else if (currentResult.type == ResultConstants.STATALLANDEVENT) {
                ApplyResultStatChangesAll(currentResult, ResultConstants.STATALLANDEVENT);
                GetNextEvent();
            }
            else if (currentResult.type == ResultConstants.PRECOMBAT) {
                eventDescription.FadeOut();
                HideEventDisplays();
                
                actionsPanel.SetAllActionsUninteractable(); // hack
                GetCombatEvent();
            }
            else if (currentResult.type == ResultConstants.COMBAT) {
                monstersToSpawn = currentResult.GetMonstersToSpawn();

                for (int j = 0; j < monstersToSpawn.Length; j++) {
                    if (monstersToSpawn[j] == "none") {
                        monstersToSpawn[j] = currentSubArea.GetMonsterToSpawn();
                    }
                }
                eventDescription.SetKey(currentResult.resultKey);
                HideEventDisplays();     
                GetCombatEvent();
            }
            else if (currentResult.type == ResultConstants.COMBATWITHSIDEEFFECTS) {
                monstersToSpawn = currentResult.GetMonstersToSpawn();

                for (int j = 0; j < monstersToSpawn.Length; j++) {
                    if (monstersToSpawn[j] == "none") {
                        monstersToSpawn[j] = currentSubArea.GetMonsterToSpawn();
                    }
                }

                if (currentResult.scope == "all") {
                    ApplyResultStatChangesAll(currentResult, ResultConstants.COMBATWITHSIDEEFFECTS);
                }
                else if (currentResult.scope == "single") {
                    ApplyResultStatChangesSingle(currentResult, ResultConstants.COMBATWITHSIDEEFFECTS);
                }

                eventDescription.SetKey(currentResult.resultKey);
                actionsPanel.PreCombatActions();
                CheckGameOver();
            }
            else if (currentResult.type == ResultConstants.REVIVE) {  
                if (PartyManager.instance.GetNumPartyMembersDead() > 0) {
                    PartyManager.instance.RevivePartyMembers();

                    eventDescription.SetKey(currentResult.resultKey);   
                }
                else {
                    changeSprite = false;
                    eventDescription.SetNoReviveText();
                }
            }
            else if (currentResult.type == ResultConstants.REVIVEANDLEAVE) {     
                if (PartyManager.instance.GetNumPartyMembersDead() > 0) { 
                    PartyManager.instance.RevivePartyMembers();

                    eventDescription.SetKey(currentResult.resultKey); 
                    actionsPanel.TravelActions();
                }
                else {
                    changeSprite = false;
                    eventDescription.SetNoReviveText();
                }
            }
            else if (currentResult.type == ResultConstants.SHOP) {
                UIManager.instance.inShop = true;

                List<Item> items = GetResultItems(currentResult);
                eventDisplays[0].SetItemDisplaysShop(items);
                SetShopNotification();

                eventDescription.SetKey(currentResult.resultKey);
            }
            else if (currentResult.type == ResultConstants.REKINDLE) {
                PartyManager.instance.Rekindle();

                eventDescription.SetKey(currentResult.resultKey);
            }
            else if (currentResult.type == ResultConstants.REKINDLEANDLEAVE) {
                PartyManager.instance.Rekindle();

                eventDescription.SetKey(currentResult.resultKey);
                actionsPanel.TravelActions();
            }
            else if (currentResult.type == ResultConstants.QUEST) {
                currentArea.SwapEventAndSubEvent(currentEvent.name, currentResult.subEventName);

                AddQuest(currentResult.questName);
                eventDescription.SetKey(currentResult.resultKey);
            }
            else if (currentResult.type == ResultConstants.QUESTANDLEAVE) {
                currentArea.SwapEventAndSubEvent(currentEvent.name, currentResult.subEventName);

                AddQuest(currentResult.questName);
                eventDescription.SetKey(currentResult.resultKey);
                actionsPanel.TravelActions();
            }
            else if (currentResult.type == ResultConstants.COMBATANDQUESTCONTINUE) {
                monstersToSpawn = currentResult.GetMonstersToSpawn();

                for (int j = 0; j < monstersToSpawn.Length; j++) {
                    if (monstersToSpawn[j] == "none") {
                        monstersToSpawn[j] = currentSubArea.GetMonsterToSpawn();
                    }
                }

                currentArea.SwapEventAndSubEvent(currentEvent.name, currentResult.subEventName);
                HideEventDisplays();     
                GetCombatEvent();

                eventDescription.SetKey(currentResult.resultKey);
            }
            else if (currentResult.type == ResultConstants.QUESTCOMPLETE) {
                currentArea.SwapEventAndSubEvent(currentEvent.name, currentResult.subEventName);
                ApplyResultStatChangesAll(currentResult, ResultConstants.QUESTCOMPLETEANDNEWINT);

                CompleteQuest(currentResult.questName);
                eventDescription.SetKey(currentResult.resultKey);
            }
            else if (currentResult.type == ResultConstants.QUESTCOMPLETEANDNEWINT) {
                currentArea.SwapEventAndSubEvent(currentEvent.name, currentResult.subEventName);
                ApplyResultStatChangesAll(currentResult, ResultConstants.QUESTCOMPLETEANDNEWINT);

                CompleteQuest(currentResult.questName);
                actionsPanel.AddInteraction(currentResult.newIntName);
                eventDescription.SetKey(currentResult.resultKey);
            }
            else if (currentResult.type == ResultConstants.END) {
                GameManager.instance.LoadNextScene("MainMenu");
            }

            if (i.GetSprite() != null && changeSprite == true) {
                eventDisplays[0].SetSprite(i.GetSprite());
                eventDisplays[0].SetPosition(pos1d1);
            }
        }

        /// <summary>
        /// Applies stat changes (HP, MP, STR, INT, DEX, LUK) to a single partyMember at random
        /// </summary>
        /// <param name="r"> Result containing the stats to be changed </param>
        public void ApplyResultStatChangesSingle(Result r, string type) {
            r.GenerateResults();

            if (r.HPAmount != 0) {
                PartyManager.instance.ChangeHPSingle(r.HPAmount, type);
            }
            if (r.MPAmount != 0) {
                PartyManager.instance.ChangeMPSingle(r.MPAmount, type);
            }
        }

        /// <summary>
        ///  Applies stat changes (HP, MP, STR, INT, DEX, LUK, EXP, status effects) to all partyMembers
        /// </summary>
        /// <param name="r"> Result containing the stats to be changed </param>
        public void ApplyResultStatChangesAll(Result r, string type) {
            bool[] changes = new bool[6];
            string[] amounts = new string[6];
            r.GenerateResults();

            if (r.HPAmount != 0) {
                StartCoroutine(PartyManager.instance.ChangeHPAll(r.HPAmount, type));
                changes[(int)ToastPanel.toastType.HP] = true;
                amounts[(int)ToastPanel.toastType.HP] = r.HPAmount.ToString();
            }
            if (r.MPAmount != 0) {
                PartyManager.instance.ChangeMPAll(r.MPAmount, type);
                changes[(int)ToastPanel.toastType.MP] = true;
                amounts[(int)ToastPanel.toastType.MP] = r.MPAmount.ToString();
            }
            if (r.EXPAmount != 0) {
                PartyManager.instance.AddEXP(r.EXPAmount);
                changes[(int)ToastPanel.toastType.EXP] = true;
                amounts[(int)ToastPanel.toastType.EXP] = r.EXPAmount.ToString();
            }
            if (r.PROGAmount != 0) {
                subAreaProgress += currentResult.PROGAmount;
                if (subAreaProgress > 100) {
                    subAreaProgress = 100;
                }
                else if (subAreaProgress < 0) {
                    subAreaProgress = 0;
                }
                if (infoPanel.isOpen == true) {
                    infoPanel.UpdateAmounts();
                }

                changes[(int)ToastPanel.toastType.PROGRESS] = true;
                amounts[(int)ToastPanel.toastType.PROGRESS] = currentResult.PROGAmount.ToString();
            }
            if (r.seName != "none") {
                PartyManager.instance.AddSE(r.seName, r.seDuration);
                changes[(int)ToastPanel.toastType.SE] = true;
                amounts[(int)ToastPanel.toastType.SE] = r.seName + "_title";
            }
            if (r.type == ResultConstants.QUESTCOMPLETE || r.type == ResultConstants.QUESTCOMPLETEANDNEWINT) {
                changes[(int)ToastPanel.toastType.QUESTCOMPLETE] = true;
                amounts[(int)ToastPanel.toastType.QUESTCOMPLETE] = r.questName;
            }

            SetNotification(changes, amounts);
        }

        /// <summary>
        /// Displays the items found in the item displays of a single event display
        /// </summary>
        /// <param name="r"> Result to have its items displayed </param>
        public void DisplayResultItems(Result r) {
            List<Item> items = GetResultItems(r);
            eventDisplays[0].SetItemDisplays(items);
        }

        /// <summary>
        /// Updates all WAX amount displays to show the accurate number
        /// </summary>
        public void UpdateWAXAmounts(){
            EventManager.instance.infoPanel.UpdateAmounts();
            if (UIManager.instance.inShop) {
                if (shopToastIndex == 0) {
                    EventManager.instance.toastPanel0.UpdateWAXAmount(); 
                }
                else {
                    EventManager.instance.toastPanel1.UpdateWAXAmount(); 
                }
            }
        }

        /// <summary>
        /// Tell the infoPanel to add a quest
        /// </summary>
        /// <param name="questName"></param>
        public void AddQuest(string questName) {
            infoPanel.AddQuest(questName);
            SetQuestNotification(questName);
        }

        /// <summary>
        /// Tell the infoPanel to remove a quest
        /// </summary>
        /// <param name="questName"></param>
        public void CompleteQuest(string questName) {
            infoPanel.CompleteQuest(questName);
        }

        /// <summary>
        /// Performs visual effects when moving to next event
        /// </summary>
        /// <returns> IEnumerator to time things properly </returns>
        public IEnumerator TransitionToNextEvent() {
            eventDescription.FadeOut();
            HideEventDisplays();
            rewardsPanel.SetVisible(false);
            SetToastPanelsVisible(false);
            SetAllButtonsInteractable(false, true);
            UIManager.instance.inShop = false;
            yield return (StartCoroutine(FadeBackgrounds()));
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
        /// <returns> Sprite </returns>
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

            Debug.LogError("BackgroundPack of name " + bgPackName +  " does not exist");
            return null;
        }

        /// <summary>
        /// Checks if the game is over (only due to partyMembers all being dead for now)
        /// </summary>
        public void CheckGameOver() {
            if (PartyManager.instance.GetNumPartyMembersAlive() == 0) {
                StartCoroutine(DisplayGameOver());
            }
        }

        /// <summary>
        /// Makes everything uninteractable and brings the player to the main menu
        /// </summary>
        /// <returns></returns>
        public IEnumerator DisplayGameOver() {
            SetAllButtonsInteractable(false);
            yield return new WaitForSeconds(1.5f);
            GameManager.instance.LoadNextScene("MainMenu");
        }

        #endregion

        #region [Section 2] EventDisplays

        /// <summary>
        /// Displays the event sprites in the eventDisplays
        /// </summary>
        public void ShowEventDisplays() {
            if (currentEvent.spriteNum != 0) {
                if (currentEvent.spriteNum == 1) {
                    eventDisplays[0].SetSprite(currentEvent.eventSprites[0]);

                    eventDisplays[0].SetPosition(pos1d1);

                    eventDisplays[0].SetVisible(true);
                }
                else if (currentEvent.spriteNum == 2) {
                    eventDisplays[0].SetSprite(currentEvent.eventSprites[0]);
                    eventDisplays[1].SetSprite(currentEvent.eventSprites[1]);

                    eventDisplays[0].SetPosition(pos2d1);
                    eventDisplays[1].SetPosition(pos2d2);

                    eventDisplays[0].SetVisible(true);
                    eventDisplays[1].SetVisible(true);
                }
                else {
                    eventDisplays[0].SetSprite(currentEvent.eventSprites[0]);
                    eventDisplays[1].SetSprite(currentEvent.eventSprites[1]);
                    eventDisplays[2].SetSprite(currentEvent.eventSprites[2]);

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
        
        public void HideEventDisplayItemDisplays() {
            foreach (EventDisplay ed in eventDisplays) {
                ed.SetItemsVisible(false);
            }
        }

        public void HideEventDisplayItemDisplays(int[] indices) {
            foreach (int index in indices) {
                eventDisplays[index].SetItemsVisible(false);
            }
        }

        /// <summary>
        /// Take all items from an event display
        /// </summary>
        public void TakeAllItems() {
            if (eventDisplays[0].itemNum > 0) {
                eventDisplays[0].TakeAllItems();
            }
            else if (rewardsPanel.itemNum > 0) {
                rewardsPanel.TakeAllItems();
            }
        }

        /// <summary>
        /// Returns the panel that the item is going towards
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Panel GetTargetPanel(string type) {
            if (type == ItemConstants.GEAR) {
                return gearPanel;
            } 
            else if (type == ItemConstants.CANDLE) {
                return candlesPanel;
            }  
            else if (type == ItemConstants.SPECIAL) {
                return specialPanel;
            }

            Debug.LogError("Panel for an item with type " + type + " does not exist");
            return null;             
        }

        /// <summary>
        /// Sets a toast notification
        /// </summary>
        /// <param name="types"> The values the notification is about (HP, MP, EXP, PROG, SE) </param>
        /// <param name="amounts"> The relevant amounts (or status effect name for SEs)</param>
        public void SetNotification(bool[] types, string[] amounts) {
            if (UIManager.instance.inShop == true || toastPanel0.gameObject.activeSelf == true) {
                toastPanel1.SetNotification(types, amounts);
            }
            else {
                toastPanel0.SetNotification(types, amounts);
            }
        }

        /// <summary>
        /// Turns one of the toast notifications into a temporary display showing the player's WAX
        /// Used primarily for shops so player doesn't have to tab back and forth between panels
        /// </summary>
        public void SetShopNotification() {
            if (toastPanel0.gameObject.activeSelf == true) {
                shopToastIndex = 1;
                toastPanel1.SetShopNotification();
            }
            else {
                shopToastIndex = 0;
                toastPanel0.SetShopNotification();    
            }
        }

        /// <summary>
        /// Sets a toastPanel to show a quest has been added
        /// </summary>
        /// <param name="questName"></param>
        public void SetQuestNotification(string questName) {
            if (toastPanel0.gameObject.activeSelf == true) {
                toastPanel1.SetQuestNotification(questName);
            }
            else {
                toastPanel0.SetQuestNotification(questName);
            }
        }

        /// <summary>
        /// Controls the visibility of the toast panels
        /// </summary>
        /// <param name="value"></param>
        public void SetToastPanelsVisible(bool value) {
            toastPanel0.SetVisible(value);
            toastPanel1.SetVisible(value);
        }

        #endregion

        /// <summary>
        /// Sets the interactability of all buttons in all panels
        /// </summary>
        /// <param name="value"> True for interactable, false otherwise </param>
        /// <param name="fadeOut"> True if special fade out animation should play for actionsPanel </param>
        public void SetAllButtonsInteractable(bool value, bool fadeOut = false) {
            if (value == true) {
                gearPanel.SetInteractable(true);
                candlesPanel.SetInteractable(true);
                specialPanel.SetInteractable(true);
                actionsPanel.SetAllActionsInteractable();
                partyPanel.EnableButtons();
                skillsPanel.SetInteractable(true);
                itemsTabManager.SetAllButtonsInteractable();
                utilityTabManager.SetAllButtonsInteractable();
            }
            else {
                gearPanel.SetInteractable(false);
                candlesPanel.SetInteractable(false);
                specialPanel.SetInteractable(false);
                if (fadeOut == true) {
                    actionsPanel.SetAllActionsUninteractableAndFadeOut();
                }
                else {
                    actionsPanel.SetAllActionsUninteractable();
                }
                actionsPanel.SetActionsUsable(true);
                partyPanel.DisableButtons();
                skillsPanel.SetInteractable(false);
                itemsTabManager.SetAllButtonsUninteractable();
                utilityTabManager.SetAllButtonsUninteractable();
            }
        }

        /// <summary>
        /// Opens the respective item panel that an itemDisplay belongs in
        /// </summary>
        /// <param name="id"></param>
        public void OpenItemPanel(ItemDisplay id) {
            if (id.type == ItemConstants.GEAR) {
                itemsTabManager.OpenPanel(0);
            }
            else if (id.type == ItemConstants.CANDLE) {
                itemsTabManager.OpenPanel(1);
            }
            else if (id.type == ItemConstants.SPECIAL) {
                itemsTabManager.OpenPanel(2);
            }
        }

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
