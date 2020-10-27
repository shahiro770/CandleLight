/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: July 4, 2019
* 
* The EventManager class manages the current scenarios (events) 
* the player will encounter based on the area they are in. 
*
*/

using Audio;
using Characters;
using CombatManager = Combat.CombatManager;
using Constants;
using General;
using Items;
using Menus.GameOverMenu;
using Menus.HelpMenu;
using Menus.OptionsMenu;
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
        public ParticleSystem ps0;                  /// <value> Particle system reference </value>
        public ParticleSystem ps1;                  /// <value> Particle system reference </value>
        public WindZone wz0;                         /// <value> Wind zone reference </value>
        public WindZone wz1;                         /// <value> Wind zone reference </value>
        public Image eventBackground;               /// <value> Image background for current event </value>
        public Image nextEventBackground;           /// <value> Image background for next event </value>
        public RewardsPanel rewardsPanel;           /// <value> RewardsPanel reference </value>
        public ToastPanel toastPanel0;              /// <value> ToastPanel reference </value>
        public ToastPanel toastPanel1;              /// <value> ToastPanel reference </value>
        public ToastPanel toastPanel2;              /// <value> ToastPanel reference </value>
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
        public Timer timer;                         /// <value> Timer reference </value>
        public Timer aromaTimer;                    /// <value> Timer reference to timer that counts down to game over </value>
        public OptionsMenu optionsMenu;             /// <value> OptionsMenu reference </value>
        public HelpMenu helpMenu;                   /// <value> OptionsMenu reference </value>
        public GameOverMenu gameOverMenu;           /// <value> gameOverMenu reference </value>

        public Special strangeBottle;       /// <value> Penultimate item to the plot is placed in the player's inventory at the start </value>
        public int subAreaProgress { get; private set; } = 0;   /// <value> When subareaProgress = 100, player is given the next event from the area </value>
        public string simEvent = "";        /// <value> event that when changed from the editor will be seen next if its in the subArea </value>

        private Area currentArea;            /// <value> Area to select subAreas from </value>
        private SubArea currentSubArea;      /// <value> SubArea to select events from </value>
        private Event currentEvent;          /// <value> Event being displayed </value>
        private Result currentResult;        /// <value> Result being obtained </value>
        private BackgroundPack[] bgPacks;    /// <value> Background packs loaded in memory </value>
        private Consumable[][] areaConsumables;   /// <value> Consumable items that can be found in the current area </value>
        private Gear[][] areaGear;          /// <value> Gear items that can be found in the current area </value>
        private Candle[][] areaCandles;     /// <value> Candle items that can be found in the current area </value>
        private Special[][] areaSpecials;   /// <value> Special items that can be found in the current area</value>
        private Consumable[] subAreaConsumables;   /// <value> Consumable items that can be found in the current subArea </value>
        private Gear[] subAreaGear;          /// <value> Gear items that can be found in the current subArea </value>
        private Candle[] subAreaCandles;     /// <value> Candle items that can be found in the current subArea </value>
        private Special[] subAreaSpecials;   /// <value> Special items that can be found in the current subArea</value>
        private Sprite[] girlSprites = new Sprite[2];   /// <value> Sprites used by the second partyMember </value>
        private List<int> midPoints = new List<int>();  /// <value> List of progress bar midpoints passed (which are just the index of each subArea according to the area </value>

        /* eventDisplay coordinates */
        private Vector3 pos1d1 = new Vector3(0, -20, 0);
        private Vector3 pos2d1 = new Vector3(-150, -20, 0);
        private Vector3 pos2d2 = new Vector3(150, -20, 0);
        private Vector3 pos3d1 = new Vector3(-275, -20, 0);
        private Vector3 pos3d2 = new Vector3(0, -20, 0);
        private Vector3 pos3d3 = new Vector3(275, -20, 0);

        private enum checkIndicators { NONE, STR, DEX, INT, LUK, ITEM, ITEMANDCLEAR, WAX, PASTITEM };
        private string[] monstersToSpawn;       /// <value> List of monsters to spawn </value>
        private string currentAreaName;         /// <value> Name of current area </value>
        private string nextSubArea = "";        /// <value> Name of next subArea to move to </value>
        private string mainQuestName = "theOnlyHope";   /// <value> Name of the main quest to add in the event player doesn't do tutorial </value>
        private float alphaLerpSpeed = 0.75f;   /// <value> Speed at which backgrounds fade in and out </value>
        private float colourLerpSpeed = 4f;     /// <value> Speed at which backgrounds change colour (for dimming) </value>
        private int bgPackNum = 0;              /// <value> Number of backgroundPacks </value>
        private int areaProgress = 0;           /// <value> Area progress increments by 1 for each main event the player completes </value>
        private int consumablesNum = 0;         /// <value> Number of consumables to be found in the subArea </value>
        private int gearNum = 0;                /// <value> Number of gear to be found in the subArea </value>
        private int candleNum = 0;              /// <value> Number of candles to be found in the subArea </value>
        private int specialNum = 0;             /// <value> Number of special to be found in the subArea </value>
        private int shopToastIndex = 0;         /// <value> Index for which toastPanel is being used as the shop's wax display </value>
        private int tutorialProg = 0;           /// <value> Current progress through the tutorial </value>
        private int noCombatCount = 0;          /// <value> Counter for how long the player has gone without a combat event </value>
        private int subAreaResets = 0;          /// <value> Number of times player has tried to save scum the subArea </value>
        private bool isReady = false;           /// <value> Wait until EventManager is ready before starting </value>
        private bool isNextEventMain = false;   /// <value> Flag for if the next even is a main event </value>
        private bool isBossFight = false;       /// <value> Flag for if the current result led to a boss fight </value>
        private bool displayStartEvent = true;  /// <value> Flag for start event to have different visual effects </value>
        private bool noShopInSubArea = true;    /// <value> Flag for if player hasn't found a shop in the subArea </value>

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
        /// Only used to check keyboard input
        /// </summary>
        void Update() {
            // Only pause if the game isn't loading or over
            if (Input.GetButtonDown("Cancel") == true && GameManager.instance.loadingScreen.activeSelf == false && gameOverMenu.gameObject.activeSelf == false
            && helpMenu.gameObject.activeSelf == false) {
                if (optionsMenu.gameObject.activeSelf == false) {
                    SetPauseMenu(true);
                    return;
                }
                else {
                    SetPauseMenu(false);
                    return;
                }
            }
            // Swap to formula text if combat text is currently visible
            if (Input.GetButtonDown("Shift") == true && eventDescription.displayedAttack != null) {
                eventDescription.SetFormulaText();
                return; 
            }
            // Swap back to attack text on shift button up
            if (Input.GetButtonUp("Shift") == true && eventDescription.displayedAttack != null) {
                eventDescription.SetAttackText(eventDescription.displayedAttack);
                return;
            }
        }

        /// <summary>
        /// Loads an Area from the database, waiting until all of subAreas, events,
        /// interactions, and results have been loaded before saying the area is ready.
        /// </summary>
        /// <param name="areaName"> Name of area to load </param>
        public void LoadArea(string areaName) {
            this.currentAreaName = areaName;
            currentArea = DataManager.instance.currentArea;
            currentArea.ResetSubAreas();
            currentSubArea = currentArea.GetSubArea("main" + currentAreaName);

            LoadBackgroundPacks();
            LoadGeneralInteractions();
            LoadGirlSprites();

            areaConsumables = DataManager.instance.areaConsumables;
            areaGear = DataManager.instance.areaGear;
            areaCandles = DataManager.instance.areaCandles;
            areaSpecials = DataManager.instance.areaSpecials;
            LoadSubAreaItems();

            isReady = true;
        }

        /// <summary>
        /// Reset game properties that are not tied to the partyManager
        /// TODO: Kinda don't like how partyManager has to reset stuff at the end of the game
        /// and EventMAnager resets stuff at the start of the game
        /// </summary>
        public void ResetGame() {
            GameManager.instance.monstersKilled = 0;
            GameManager.instance.WAXobtained = 0;
            GameManager.instance.totalEvents = 0;
            midPoints.Clear();

            timer.ResetTimer();
            timer.StartTimer(true);
            timer.SetVisible(UIManager.instance.isTimer);
            if (GameManager.instance.gsData.aromas[(int)AromaConstants.aromaConstants.WILTINGWINTERGREEN] == true) {
                aromaTimer.SetVisibleAromaTimer(true);
                aromaTimer.StartTimerAroma(true);
            }
            else {
                aromaTimer.SetVisibleAromaTimer(false);
            }
            PartyManager.instance.CalculateMultipliers();
        }
        
        /// <summary>
        /// Load backgroundPacks for the current area
        /// </summary>
        public void LoadBackgroundPacks() {
            bgPacks = DataManager.instance.bgPacks;

            for (int i = 0; i < bgPacks.Length; i++) {
                if (bgPacks[i] != null) {
                    bgPackNum++;
                }
            }
        }

        /// <summary>
        /// Loads consumables for the current subArea
        /// </summary>
        public void LoadSubConsumables(int index) {
            subAreaConsumables = areaConsumables[index];
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
        public void LoadSubGear(int index) {
            subAreaGear = areaGear[index];
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
        public void LoadSubCandles(int index) {
            subAreaCandles = areaCandles[index];
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
        public void LoadSubSpecials(int index) {
            subAreaSpecials = areaSpecials[index];
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
        /// Loads all of the sprites for the travelling companion of the player (the second partyMember).
        /// Since the girl's sprite is class-based, these need to be loaded right after the class is decided.
        /// </summary>
        public void LoadGirlSprites() {
            if (GameManager.instance.tutorialTriggers[(int)TutorialConstants.tutorialTriggers.isTutorial] == true) {
                girlSprites[0] = Resources.Load<Sprite>("Sprites/Classes/Girl" + PartyManager.instance.storedPartyMember + "0");
                girlSprites[1] = Resources.Load<Sprite>("Sprites/Classes/Girl" + PartyManager.instance.storedPartyMember + "1");
            }
            else {
                girlSprites[0] = Resources.Load<Sprite>("Sprites/Classes/Girl" + PartyManager.instance.GetPartyMembers()[1].className + "0");
                girlSprites[1] = Resources.Load<Sprite>("Sprites/Classes/Girl" + PartyManager.instance.GetPartyMembers()[1].className + "1");
            }
        }

        /// <summary>
        /// Load general interactions that many events might use
        /// TODO: find a better place to put this
        /// </summary>
        public void LoadGeneralInteractions() {
            Interaction travelInt = DataManager.instance.travelInt;
            Interaction fightInt =DataManager.instance.fightInt;
            Interaction tutorialInt = DataManager.instance.tutorialInt;
            actionsPanel.SetGeneralInteractions(travelInt, fightInt, tutorialInt);
        }

        /// <summary>
        /// Loads all items for a subArea
        /// </summary>
        public void LoadSubAreaItems() {
            int subAreaIndex = currentArea.GetSubAreaIndex(currentSubArea.name);

            LoadSubConsumables(subAreaIndex);
            LoadSubGear(subAreaIndex);
            LoadSubCandles(subAreaIndex);
            LoadSubSpecials(subAreaIndex);

            infoPanel.UpdateSubAreaCard(currentArea.GetSubAreaCard(subAreaIndex), currentArea.name + subAreaIndex);
        }

        /// <summary>
        /// Equips the party with starting weapons if starting without a tutorial, or save data gear and candles.
        /// </summary>
        public void EquipPartyItems() {
            List<PartyMember> pms = PartyManager.instance.GetPartyMembers();
            for (int i = 0; i < pms.Count; i++) {
                if (GameManager.instance.rData == null || GameManager.instance.rData.partyMemberDatas == null) {
                    pms[i].EquipGear(GenerateStartingWeapon(pms[i].className), ItemConstants.WEAPON);
                }
                else {
                    pms[i].EquipLoadedItems(GameManager.instance.rData.partyMemberDatas[i]);
                }
            }
        }

        public void LoadPartyStatusEffects() {
            if (GameManager.instance.rData != null && GameManager.instance.rData.partyMemberDatas != null) {
                List<PartyMember> pms = PartyManager.instance.GetPartyMembers();
                for (int i = 0; i < pms.Count; i++) {
                    pms[i].AddLoadedStatusEffects(GameManager.instance.rData.partyMemberDatas[i]);
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
            if (GameManager.instance.rData == null || GameManager.instance.rData.partyMemberDatas == null) {
                ResetGame();
            }
            else {  
                GameManager.instance.monstersKilled = GameManager.instance.rData.monstersKilled;
                GameManager.instance.WAXobtained = GameManager.instance.rData.WAXobtained;
                GameManager.instance.totalEvents = GameManager.instance.rData.totalEvents;
                subAreaResets = GameManager.instance.rData.subAreaResets;
                midPoints = GameManager.instance.rData.midPoints;

                timer.SetElapseTimed(GameManager.instance.rData.elapsedTime);
                timer.StartTimer(true);
                timer.SetVisible(UIManager.instance.isTimer);
                if (GameManager.instance.gsData.aromas[(int)AromaConstants.aromaConstants.WILTINGWINTERGREEN] == true) {
                    aromaTimer.SetVisibleAromaTimer(true);
                    aromaTimer.StartTimerAroma(true);
                }
                else {
                    aromaTimer.SetVisibleAromaTimer(false);
                }
            }

            LoadArea(areaName);
            while (isReady == false) {
                yield return null;
            }

            if (GameManager.instance.rData != null && GameManager.instance.rData.partyMemberDatas != null) {
                areaProgress = GameManager.instance.rData.areaProgress;

                gearPanel.LoadData(GameManager.instance.rData.spareGear);
                candlesPanel.LoadData(GameManager.instance.rData.spareCandles);
                specialPanel.LoadData(GameManager.instance.rData.spareSpecials);
                infoPanel.LoadData(GameManager.instance.rData.questData);
                EquipPartyItems();
                LoadPartyStatusEffects();
                foreach (Quest q in infoPanel.quests) {
                    currentArea.SwapEventAndSubEvent(q.startEvent, q.nextEvent);
                }
                if (subAreaResets >= 3) {   // player gets punished for abusing the save system's convenience
                    PartyManager.instance.AddSE(StatusEffectConstants.SCUM, 999);
                }
                PlaySubAreaBGM();
                AlterParticleSystem();
                currentEvent = currentSubArea.GetEvent(areaProgress);
                StartCoroutine(DisplayEvent());
            }
            else if (GameManager.instance.tutorialTriggers[(int)TutorialConstants.tutorialTriggers.isTutorial] == true) {
                areaProgress = 0;
                PlaySubAreaBGM();
                AlterParticleSystem();
                StartTutorial();
            }
            else {  // skip the tutorial
                areaProgress = 1;
                PlaySubAreaBGM();
                AlterParticleSystem();
                EquipPartyItems();
                AddQuestNoNotification(mainQuestName, "", "");  // main story quest (TODO: Make this a constant?)
                currentEvent = currentSubArea.GetEvent(areaProgress);
                SaveRunAndGeneralData();
                StartCoroutine(DisplayEvent());
            }
        }

        #endregion

        #region [Section 0] TutorialStuff

        /// <summary>
        /// Starts the tutorial by disabling most of the game's features
        /// </summary>
        public void StartTutorial() {
            tutorialProg = 0;
            itemsTabManager.SetTabsEmpty();
            utilityTabManager.SetTabsEmpty();
            actionsPanel.SetToggleButtonLocked(true);
            currentEvent = currentSubArea.GetEvent(areaProgress);
            StartCoroutine(DisplayEvent());
        }

        /// <summary>
        /// No differences between subevents and events in the tutorial (allowing for health and mana regeneration)
        /// </summary>
        /// <returns></returns>
        public IEnumerator DisplaySubEventTutorial() {
            tutorialProg++;
            yield return StartCoroutine(DisplayEvent());
        }

        /// <summary>
        /// Progresses the tutorial, revealing tabs, and temporary tutorial help panels
        /// </summary>
        public void ProgressTutorial() {
            if (tutorialProg == 0) {
                itemsTabManager.SetButtonInteractableAndName(2);
                itemsTabManager.ExciteTab(2);
                SetTutorialNotification("special0", 0);   
            }
            else if (tutorialProg == 1) {
                utilityTabManager.SetButtonInteractableAndName(0);
                utilityTabManager.ExciteTab(0);
                SetTutorialNotification("party0", 0);
            }
            else if (tutorialProg == 2) {
                PartyManager.instance.AddStoredPartyMember();
            }
            else if (tutorialProg == 3) {
                List<Item> startingWeapons = new List<Item>();
                startingWeapons.Add(GenerateStartingWeapon(PartyManager.instance.GetPartyMembers()[0].className));
                startingWeapons.Add(GenerateStartingWeapon(PartyManager.instance.GetPartyMembers()[1].className));
                eventDisplays[0].SetItemDisplays(startingWeapons);

                itemsTabManager.SetButtonInteractableAndName(0);
                itemsTabManager.ExciteTab(0);
                SetTutorialNotification("gear0", 0);
                tutorialProg++;
            }
            else if (tutorialProg == 4) {   // TODO: This is inconsistent with events that clear items when an interaction is used (like stingerBurrows)
                itemsTabManager.SetButtonInteractableAndName(1);
                tutorialProg++;
            }
            else if (tutorialProg == 5) {
                SetTutorialNotification("combat0", 0);
                tutorialProg++;
            }
            else if (tutorialProg == 6) {
                SetTutorialNotification("combat1", 0);
                tutorialProg++;
            }
            else if (tutorialProg == 7) {
                SetTutorialNotification("combat2", 0);
                tutorialProg++;
            }
            else if (tutorialProg == 8) {
                SetTutorialNotification("combat3", 0);
                tutorialProg++;
            }
            else if (tutorialProg == 9) {
                SetTutorialNotification("combat4", 0);
                tutorialProg++;
            }
            else if (tutorialProg == 10) {
                SetTutorialNotification("combat5", 0);
                tutorialProg++;
            }
            else if (tutorialProg == 11 && CombatManager.instance.inCombat == false) {
                SetTutorialNotification("skills0", 0);
            }
            // skills panel is enabled after first combat event
            else if (tutorialProg == 12) {
                utilityTabManager.SetButtonInteractableAndName(2);
                utilityTabManager.ExciteTab(2);
                AddQuest(currentResult.questName, "", "");
                toastPanel1.ShowArrow(425, -110, true);
            }
        }

        /// <summary>
        /// Makes a tutorial pop up when a tab button is clicked.
        /// This is done to make sure the player isn't overwhelmed by instructions too quickly.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="tabManager"> True means right tabManager, false means left tabManager</param>
        /// <returns></returns>
        public bool TutorialTabOnClick(int index, bool tabManager) {
            if (tutorialProg == 0 && tabManager == false && itemsTabManager.panels[index].GetPanelName() == PanelConstants.SPECIALPANEL) {
                SetTutorialNotification("special1", 1);
                HideTutorialArrow();
                return true;
            }
            else if (tutorialProg == 1 && tabManager == true &&  utilityTabManager.panels[index].GetPanelName() == PanelConstants.PARTYPANEL) {
                SetTutorialNotification("party1", 1);
                HideTutorialArrow();
                return true;
            }
            else if ((tutorialProg == 4 || tutorialProg == 5) && tabManager == false && itemsTabManager.panels[index].GetPanelName() == PanelConstants.GEARPANEL) {              
                SetTutorialNotification("gear1", 1);
                HideTutorialArrow();
                return true;
            }
            else if (tutorialProg == 11 && tabManager == true &&  utilityTabManager.panels[index].GetPanelName() == PanelConstants.SKILLSPANEL) {
                SetTutorialNotification("skills1", 1);
                HideTutorialArrow();
                return true;
            }
            else if (tutorialProg == 12 && tabManager == true &&  utilityTabManager.panels[index].GetPanelName() == PanelConstants.INFOPANEL) {
                SetTutorialNotification("info0");
                HideTutorialArrow();
                return true;
            }

            return false;
        }
        
        /// <summary>
        /// Generates the starting weapon for a partyMember with a given class
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public Gear GenerateStartingWeapon(string className) {
            Gear startingWeapon = null;
            if (className == "warrior") {
                startingWeapon = (Gear)GameManager.instance.DB.GetItemByNameID("WarriorWeapon-1", "Gear");
            }
            else if (className == "mage") {
                startingWeapon = (Gear)GameManager.instance.DB.GetItemByNameID("MageWeapon-1", "Gear");
            }
            else if (className == "archer") {
                startingWeapon = (Gear)GameManager.instance.DB.GetItemByNameID("ArcherWeapon-1", "Gear");
            }
            else if (className == "rogue") {
                startingWeapon = (Gear)GameManager.instance.DB.GetItemByNameID("RogueWeapon-1", "Gear");
            }
            
            return startingWeapon;
        }

        public Candle GenerateStartingCandle() {
            return (Candle)GameManager.instance.DB.GetItemByNameID("HPC0", "Candles");
        }
        
        public void EndTutorial() {
            tutorialProg++;             
            GameManager.instance.tutorialTriggers[(int)TutorialConstants.tutorialTriggers.isTutorial] = false; // if the player continues into the main game from the tutorial, base tutorial popups can no longer trigger
            subAreaProgress += 100;
            actionsPanel.SetToggleButtonLocked(false);
        }

        #endregion

        #region [Section 1] EventManagement

        /// <summary>
        /// Sets the currentSubArea
        /// </summary>
        /// <param name="subAreaName"></param>
        public void SetSubArea(string subAreaName) {
            currentSubArea = currentArea.GetSubArea(subAreaName);
            PlaySubAreaBGM();
            UpdateMidPoints();
            LoadSubAreaItems();
            subAreaProgress = 0;
            subAreaResets = 0;
            if (infoPanel.isOpen == true) {
                infoPanel.UpdateAmounts();
            }
        }

        /// <summary>
        /// Gets the next event
        /// </summary>
        public void GetNextEvent() {
            if (GameManager.instance.gsData.aromas[(int)AromaConstants.aromaConstants.LOSTLILAC] == true) {
                subAreaProgress += (int)(currentEvent.progressAmount * PartyManager.instance.PROGmultiplier);
            }
            else {
                subAreaProgress += currentEvent.progressAmount;
            }
            GameManager.instance.totalEvents += 1;

            if (isNextEventMain == true) {  // don't reset progress to 0 until after player leaves the current main event
                subAreaProgress = 0;
                isNextEventMain = false;
            }
            if (subAreaProgress >= 100) {
                subAreaProgress = 100;
                isNextEventMain = true;
            }
            if (infoPanel.isOpen) {
                infoPanel.UpdateAmounts();
            }

            if (isNextEventMain == true) {
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
            if (currentSubArea.name == "tombsGreyWastes") { // hack to deal with the path splitting TODO: Find a better way
                areaProgress = 6;
            }
            else {
                areaProgress++;;
            }
            noCombatCount = 0;
            noShopInSubArea = true;
            currentSubArea = currentArea.GetSubArea("main" + currentAreaName);
            currentEvent = currentSubArea.GetEvent(areaProgress);
            PlaySubAreaBGM();
            AlterParticleSystem();
            infoPanel.UpdateSubAreaCard(currentArea.GetSubAreaCard(0), currentArea.name + "0");
            SaveRunAndGeneralData();
            if (GameManager.instance.gsData.aromas[(int)AromaConstants.aromaConstants.WILTINGWINTERGREEN] == true) {
                aromaTimer.ResetTimer();
            }
        }

        /// <summary>
        /// Gets the next random event in the current subArea
        /// </summary>
        public void GetNextSubAreaEvent() {
            currentEvent = null;
            if (noShopInSubArea == true && subAreaProgress >= 85){  // guarantee the player a shop at the end of the subArea if they haven't encountered one yet
                noShopInSubArea = false;
                currentEvent = currentSubArea.GetEventByType(EventConstants.SHOP);  
            }
            if (currentEvent == null) {    // will go here by default, or if a forced shop event returns null because the subArea has no shop
                currentEvent = currentSubArea.GetEvent();
                if (noCombatCount >= 5 && currentEvent.type != EventConstants.COMBAT) {
                    int forcedCombatChance = Random.Range(0, 100);
                    if (forcedCombatChance < 66) { 
                        currentEvent = currentSubArea.GetEvent(EventConstants.COMBAT + currentAreaName);    //TODO: Make a function that 
                    }
                }
                else if (subAreaProgress < 35 && currentEvent.type == EventConstants.SHOP) {    // reroll to reduce odds of a shop while subAreaProgress is still less than 35
                    currentEvent = currentSubArea.GetEvent();
                }
            }
            if (simEvent != "") {
                currentEvent = currentSubArea.GetEvent(simEvent);
            }
        }

        /// <summary>
        /// Switches gameplay from exploring into turn-based combat with random monsters
        /// </summary>
        public void GetCombatEvent() {
            noCombatCount = 0;
            gearPanel.SetTakeable(false);
            candlesPanel.SetTakeable(false);
            skillsPanel.SetTogglable(false);
            specialPanel.SetTakeable(false);
            actionsPanel.SetToggleButtonInteractable(false);
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
                SetToastPanelsVisible(false);
                displayStartEvent = false;
                PartyManager.instance.SetActivePartyMember(PartyManager.instance.GetFirstPartyMemberAlive());
                statusPanel.DisplayPartyMember(PartyManager.instance.GetActivePartyMember().pmvc);
            }

            PartyManager.instance.RegenParty();
            if (GameManager.instance.tutorialTriggers[(int)TutorialConstants.tutorialTriggers.isTips] == true
            && GameManager.instance.tutorialTriggers[(int)TutorialConstants.tutorialTriggers.thirdPivotalMomentreached] == true
            && areaProgress == 2) {
                GameManager.instance.tutorialTriggers[(int)TutorialConstants.tutorialTriggers.thirdPivotalMomentreached] = false;
                SetTutorialNotification("save");
            }

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

                noCombatCount++;
                if (currentEvent.type == EventConstants.SHOP) {
                    noShopInSubArea = false;
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
            if (currentSubArea.name == "endGreyWastes") {   // for the game's ending in the GreyWastes subArea, add progress and trigger status effects in subEvents
                StartCoroutine(PartyManager.instance.TriggerStatuses(false));
                subAreaProgress += currentEvent.progressAmount;
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

                if (currentEvent.type == EventConstants.SHOP) {
                    noShopInSubArea = false;
                }
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
                StartCoroutine(DisplayGameOver(false));
            }
            else {
                StartCoroutine(AlterBackgroundColor(1f));
                if (isBossFight == true) {
                    PlayPostBossBGM();
                }
                actionsPanel.ClearAllActions();
                rewardsPanel.SetVisible(true);
                PartyManager.instance.SetActivePartyMember(PartyManager.instance.GetFirstPartyMemberAlive());   // prevents summoned partyMembers's inventories from appearing post combat
                if (GameManager.instance.tutorialTriggers[(int)TutorialConstants.tutorialTriggers.isTutorial] == true || 
                GameManager.instance.tutorialTriggers[(int)TutorialConstants.tutorialTriggers.isTips] == true) {
                    SetToastPanelsVisible(false);
                }

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
                GameManager.instance.monstersKilled += combatManager.monstersKilled.Count;
                if (infoPanel.isOpen) {
                    infoPanel.UpdateAmounts();
                }
                           
                gearPanel.SetTakeable(true);
                candlesPanel.SetTakeable(true);
                specialPanel.SetTakeable(true);
                skillsPanel.SetTogglable(true);
                actionsPanel.SetToggleButtonInteractable(true);

                if (GameManager.instance.tutorialTriggers[(int)TutorialConstants.tutorialTriggers.isTutorial] == false) {
                    actionsPanel.PostCombatActions(rewardsPanel.itemNum);
                }
                else {
                    actionsPanel.PostCombatActionsTutorial();
                    ProgressTutorial();
                }
                SetAllButtonsInteractable(true);
            }
        }

        /// <summary>
        /// Returns items found as a result of an interaction
        /// </summary>
        /// <param name="r"> Result containing items </param>
        /// <returns> List of items </returns>
        public List<Item> GetResultItems(Result r) {
            if (GameManager.instance.difficultyModifier == 0.75f) {     // normal mode has worse items
                r.DowngradeResult();
            }
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
                    }
                    else if (r.itemType == ItemConstants.SPECIAL) {
                        items.Add(new Special(subAreaSpecials[Random.Range(0, specialNum)]));
                    }
                }
            }

            if (GameManager.instance.tutorialTriggers[(int)TutorialConstants.tutorialTriggers.isTips] == true
            && GameManager.instance.tutorialTriggers[(int)TutorialConstants.tutorialTriggers.firstConsumable] == true && r.itemType == ItemConstants.CONSUMABLE) {
                GameManager.instance.tutorialTriggers[(int)TutorialConstants.tutorialTriggers.firstConsumable] = false;
                SetTutorialNotification("consumable");
            }
            else if (GameManager.instance.tutorialTriggers[(int)TutorialConstants.tutorialTriggers.isTips] == true  
            && GameManager.instance.tutorialTriggers[(int)TutorialConstants.tutorialTriggers.firstCandle] == true && r.itemType == ItemConstants.CANDLE) {
                GameManager.instance.tutorialTriggers[(int)TutorialConstants.tutorialTriggers.firstCandle] = false;
                SetTutorialNotification("candles0");
            }

            if (items.Count == 0) {
                Debug.LogError("No items were generated");
            }

            return items;
        }

        /// <summary>
        /// Get the gear dropped by a monster.
        /// If the monster drops non-gear items, use subarea item pools to find the item
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public Item GetResultItemsMonster(Monster m) {
            Result r = m.monsterReward;
            
            if (r.itemType == ItemConstants.GEAR) {  
                if (GameManager.instance.difficultyModifier == 0.75f) {     // normal mode has worse items
                    r.DowngradeResult();
                }
                r.GenerateResults();

                Gear g =  new Gear(m.monsterGear[Random.Range(0, m.monsterGear.Length - 1)]);
                g.RandomizeAmounts(r.itemQuality);

                if (g == null) {
                    Debug.LogError("Monster gear was not generated");
                }
                else {
                    return g;
                }
            }
            else {  // if monster were to drop something that isn't gear (extremely rare)
                return GetResultItems(r)[0];
            }
             
            return null;
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
                else if (i.checkIndicator == (int)checkIndicators.WAX) {
                    if (PartyManager.instance.WAX >= Random.Range((int)i.statThreshold * 0.6f, (int)i.statThreshold * 1.3f)) {
                        currentResult = i.GetResult(0); // good result
                    }
                    else {
                        currentResult = i.GetResultStartIndex(1); // bad result(s)
                    }
                }
                else if (i.checkIndicator == (int)checkIndicators.PASTITEM) {
                    if (GameManager.instance.pastItem != null) {
                        currentResult = i.GetResult(0); // event related to a past item if it exists
                    }
                    else {
                        currentResult = i.GetResultStartIndex(1);  
                    }
                }
                else {
                    int stat = PartyManager.instance.GetPrimaryStatAll(i.checkIndicator);
                    int threshold = (int)(Random.Range(i.statThreshold * 0.6f, i.statThreshold * 1.3f) * GameManager.instance.difficultyModifier) 
                    - (int)(PartyManager.instance.GetPrimaryStatAll((int)checkIndicators.LUK) * 0.2f);
                    if (threshold < 0) {
                        threshold = 0;
                    }

                    if (stat >= threshold) {
                        currentResult = i.GetResult(0); // good result
                        SetStatCheckNotification(true, i.checkIndicator, stat, threshold);
                    }
                    else {
                        currentResult = i.GetResultStartIndex(1); // bad result(s)
                        SetStatCheckNotification(false, i.checkIndicator, stat, threshold);
                    }
                }
            }
            else {
                currentResult = i.GetResult();
            }

            switch (currentResult.type) {
                case ResultConstants.NORESULT:
                    eventDescription.SetKey(currentResult.resultKey);
                    break;
                case ResultConstants.NORESULTANDLEAVE:
                    eventDescription.SetKey(currentResult.resultKey);
                    actionsPanel.TravelActions();
                    HideEventDisplayItemDisplays();
                    break; 
                case ResultConstants.TAKEALLITEMS:
                    TakeAllItems();
                    break;
                case ResultConstants.ITEM:
                    actionsPanel.SetItemActions();
                    eventDescription.SetKey(currentResult.resultKey);
                    DisplayResultItems(currentResult);
                    break;
                case ResultConstants.PASTITEM:
                    DisplayPastItems();
                    eventDescription.SetKey(currentResult.resultKey);
                    break;
                case ResultConstants.NEWINT:
                    actionsPanel.AddInteraction(currentResult.newIntName);
                    eventDescription.SetKey(currentResult.resultKey);
                    break;
                case ResultConstants.EVENT:
                    GetNextEvent();
                    break;
                case ResultConstants.SUBEVENT:     
                    currentEvent = currentSubArea.GetSubEvent(currentResult.subEventName);
                    yield return StartCoroutine(DisplaySubEvent());
                    break;
                case ResultConstants.ITEMWITHSUBEVENT:  // subEvents do not need result prompts
                    currentEvent = currentSubArea.GetSubEvent(currentResult.subEventName);
                    yield return StartCoroutine(DisplaySubEvent());
                    DisplayResultItems(currentResult);
                    break;
                case ResultConstants.PASTITEMANDSUBEVENT:  // subEvents do not need result prompts
                    currentEvent = currentSubArea.GetSubEvent(currentResult.subEventName);
                    yield return StartCoroutine(DisplaySubEvent());
                    DisplayPastItems();
                    break;
                case ResultConstants.SUBEVENTTUT:
                    currentEvent = currentSubArea.GetSubEvent(currentResult.subEventName);
                    yield return StartCoroutine(DisplaySubEventTutorial());
                    break;
                case ResultConstants.SUBAREA:
                    SetSubArea(currentResult.subAreaName0);
                    
                    GetNextEvent();
                    break;
                case ResultConstants.SUBAREAANDCOMBAT:     // boss fight
                    SetSubArea(currentResult.subAreaName0);

                    isBossFight = true;
                    monstersToSpawn = currentResult.GetMonstersToSpawn();

                    for (int j = 0; j < monstersToSpawn.Length; j++) {
                        if (monstersToSpawn[j] == "none") {
                            monstersToSpawn[j] = currentSubArea.GetMonsterToSpawn();
                        }
                    }
                    eventDescription.SetKey(currentResult.resultKey);
                    HideEventDisplays();     
                    GetCombatEvent();
                    break;
                case ResultConstants.SUBAREAANDCOMBATANDSUBAREA:
                    SetSubArea(currentResult.subAreaName0);

                    monstersToSpawn = currentResult.GetMonstersToSpawn();

                    for (int j = 0; j < monstersToSpawn.Length; j++) {
                        if (monstersToSpawn[j] == "none") {
                            monstersToSpawn[j] = currentSubArea.GetMonsterToSpawn();
                        }
                    }
                    eventDescription.SetKey(currentResult.resultKey);
                    HideEventDisplays();     
                    GetCombatEvent();
                    break;
                case ResultConstants.STATSINGLE:
                    eventDescription.SetKey(currentResult.resultKey);
                    ApplyResultStatChangesSingle(currentResult, ResultConstants.STATSINGLE);
                    break;
                case ResultConstants.STATALL:
                    eventDescription.SetKey(currentResult.resultKey);
                    ApplyResultStatChangesAll(currentResult, ResultConstants.STATALL);
                    CheckGameOver();
                    break;
                case ResultConstants.STATALLANDLEAVE:
                    eventDescription.SetKey(currentResult.resultKey);
                    ApplyResultStatChangesAll(currentResult, ResultConstants.STATALLANDLEAVE);
                    actionsPanel.TravelActions();
                    CheckGameOver();
                    break;
                case ResultConstants.STATALLANDITEMANDLEAVE:
                    eventDescription.SetKey(currentResult.resultKey);
                    ApplyResultStatChangesAll(currentResult, ResultConstants.STATALLANDITEMANDLEAVE);
                    DisplayResultItems(currentResult);
                    actionsPanel.SetItemActions();         
                    CheckGameOver();
                    break;
                case ResultConstants.STATALLANDENDSUBAREA:
                    ApplyResultStatChangesAll(currentResult, ResultConstants.STATALLANDENDSUBAREA);
                    subAreaProgress = 100;
                    GetNextEvent();
                    CheckGameOver();
                    break;
                case ResultConstants.PRECOMBAT:
                    eventDescription.FadeOut();
                    HideEventDisplays();
                    
                    actionsPanel.SetAllActionsUninteractable(); // hack
                    GetCombatEvent();
                    break;
                case ResultConstants.COMBAT:
                    monstersToSpawn = currentResult.GetMonstersToSpawn();

                    for (int j = 0; j < monstersToSpawn.Length; j++) {
                        if (monstersToSpawn[j] == "none") {
                            monstersToSpawn[j] = currentSubArea.GetMonsterToSpawn();
                        }
                    }
                    eventDescription.SetKey(currentResult.resultKey);
                    HideEventDisplays();     
                    GetCombatEvent();
                    break;
                case ResultConstants.COMBATWITHSIDEEFFECTS:
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
                    break;
                case ResultConstants.REVIVE:
                    if (PartyManager.instance.GetNumPartyMembersDead() > 0) {
                        PartyManager.instance.RevivePartyMembers();

                        eventDescription.SetKey(currentResult.resultKey);   
                    }
                    else {
                        changeSprite = false;
                        eventDescription.SetNoReviveText();
                    }
                    break;
                case ResultConstants.REVIVEANDLEAVE:   
                    if (PartyManager.instance.GetNumPartyMembersDead() > 0) { 
                        PartyManager.instance.RevivePartyMembers();

                        eventDescription.SetKey(currentResult.resultKey); 
                        actionsPanel.TravelActions();
                    }
                    else {
                        changeSprite = false;
                        eventDescription.SetNoReviveText();
                    }
                    break;
                case ResultConstants.REVIVEORSTATALL:   
                    ApplyResultStatChangesAll(currentResult, ResultConstants.STATALL);
                    if (PartyManager.instance.GetNumPartyMembersDead() > 0) { 
                        PartyManager.instance.RevivePartyMembers();
                    }

                    eventDescription.SetKey(currentResult.resultKey); 
                    break;
                case ResultConstants.SHOP:
                    UIManager.instance.inShop = true;

                    List<Item> items = GetResultItems(currentResult);
                    eventDisplays[0].SetItemDisplaysShop(items);
                    SetShopNotification();

                    eventDescription.SetKey(currentResult.resultKey);

                    if (GameManager.instance.tutorialTriggers[(int)TutorialConstants.tutorialTriggers.isTips] == true 
                    && GameManager.instance.tutorialTriggers[(int)TutorialConstants.tutorialTriggers.firstShop] == true) {
                        GameManager.instance.tutorialTriggers[(int)TutorialConstants.tutorialTriggers.firstShop] = false;
                        SetTutorialNotification("shop");
                    }
                    break;
                case ResultConstants.REKINDLE:
                    if (PartyManager.instance.IsCandlesEquipped() == true) {
                        PartyManager.instance.Rekindle();
                        eventDescription.SetKey(currentResult.resultKey);
                    }
                    else {
                        changeSprite = false;
                        eventDescription.SetNoRekindleText();
                    }
                    break;
                case ResultConstants.REKINDLEANDLEAVE:
                    if (PartyManager.instance.IsCandlesEquipped() == true) {
                        PartyManager.instance.Rekindle();

                        eventDescription.SetKey(currentResult.resultKey);
                        actionsPanel.TravelActions();
                    }
                    else {
                        changeSprite = false;
                        eventDescription.SetNoRekindleText();
                    }
                    break;
                case ResultConstants.QUEST:
                    currentArea.SwapEventAndSubEvent(currentEvent.name, currentResult.subEventName);

                    AddQuest(currentResult.questName, currentEvent.name, currentResult.subEventName);
                    eventDescription.SetKey(currentResult.resultKey);
                    break;
                case ResultConstants.QUESTANDLEAVE:
                    currentArea.SwapEventAndSubEvent(currentEvent.name, currentResult.subEventName);

                    AddQuest(currentResult.questName, currentEvent.name, currentResult.subEventName);
                    eventDescription.SetKey(currentResult.resultKey);
                    actionsPanel.TravelActions();
                    break;
                case ResultConstants.QUESTANDITEM:
                    DisplayResultItems(currentResult);
                    currentArea.SwapEventAndSubEvent(currentEvent.name, currentResult.subEventName);

                    AddQuest(currentResult.questName, currentEvent.name, currentResult.subEventName);
                    eventDescription.SetKey(currentResult.resultKey);
                    actionsPanel.SetItemActions();
                    break;
                case ResultConstants.QUESTCONTINUEANDNEWINT:
                    currentArea.SwapEventAndSubEvent(currentEvent.name, currentResult.subEventName);
                    UpdateQuest(currentResult.questName, currentResult.subEventName);
                    
                    actionsPanel.AddInteraction(currentResult.newIntName);
                    eventDescription.SetKey(currentResult.resultKey);
                    break;
                case ResultConstants.COMBATANDQUESTCONTINUE:
                    monstersToSpawn = currentResult.GetMonstersToSpawn();

                    for (int j = 0; j < monstersToSpawn.Length; j++) {
                        if (monstersToSpawn[j] == "none") {
                            monstersToSpawn[j] = currentSubArea.GetMonsterToSpawn();
                        }
                    }

                    currentArea.SwapEventAndSubEvent(currentEvent.name, currentResult.subEventName);
                    UpdateQuest(currentResult.questName, currentResult.subEventName);
                    HideEventDisplays();     
                    GetCombatEvent();

                    eventDescription.SetKey(currentResult.resultKey);
                    break;
                case ResultConstants.QUESTCOMPLETE:
                    currentArea.SwapEventAndSubEvent(currentEvent.name, currentResult.subEventName);
                    ApplyResultStatChangesAll(currentResult, ResultConstants.QUESTCOMPLETEANDNEWINT);

                    CompleteQuest(currentResult.questName);
                    eventDescription.SetKey(currentResult.resultKey);
                    break;
                case ResultConstants.QUESTCOMPLETEANDNEWINT:
                    currentArea.SwapEventAndSubEvent(currentEvent.name, currentResult.subEventName);
                    ApplyResultStatChangesAll(currentResult, ResultConstants.QUESTCOMPLETEANDNEWINT);

                    CompleteQuest(currentResult.questName);
                    actionsPanel.AddInteraction(currentResult.newIntName);
                    eventDescription.SetKey(currentResult.resultKey);
                    break;
                case ResultConstants.NEWINTANDTUT:
                    actionsPanel.AddInteraction(currentResult.newIntName);
                    eventDescription.SetKey(currentResult.resultKey);
                    ProgressTutorial();
                    break;
                case ResultConstants.END:
                    subAreaProgress = 100;
                    StartCoroutine(DisplayGameOver(true));
                    break;
                case ResultConstants.ENDTUT:
                    EndTutorial();
                    GetNextEvent();
                    break;
                case ResultConstants.STORESHOPITEMNEXTEVENT:
                    GameManager.instance.pastItem = eventDisplays[0].GetRandomItem();
                    GetNextEvent();
                    break;    
                default:
                    break;
            }

            if (i.GetSprite() != null && changeSprite == true) { // assumes only one event display changes during interactions (and only the middle one)
                if (i.GetSprite().name == "Girl0") {
                    eventDisplays[0].SetSprite(girlSprites[0]);
                }
                else if (i.GetSprite().name == "Girl1") {
                    eventDisplays[0].SetSprite(girlSprites[1]);
                }
                else {
                    eventDisplays[0].SetSprite(i.GetSprite());
                }
                if (eventDisplays[0].IsVisible() == false) {
                    eventDisplays[0].SetPosition(pos1d1);
                    eventDisplays[0].SetVisible(true);
                }
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
            bool[] changes = new bool[7];
            string[] amounts = new string[7];
            r.GenerateResults();

            if (r.HPAmount != 0) {
                StartCoroutine(PartyManager.instance.ChangeHPAll(r.HPAmount, type));
                changes[(int)ToastPanel.toastType.HP] = true;
                if (r.HPAmount > 0) {
                    amounts[(int)ToastPanel.toastType.HP] = "+" + r.HPAmount.ToString();
                }
                else {
                    amounts[(int)ToastPanel.toastType.HP] = r.HPAmount.ToString();
                }
            }
            if (r.MPAmount != 0) {
                PartyManager.instance.ChangeMPAll(r.MPAmount, type);
                changes[(int)ToastPanel.toastType.MP] = true;
                if (r.MPAmount > 0) {
                    amounts[(int)ToastPanel.toastType.MP] = "+" + r.MPAmount.ToString();
                }
                else {
                    amounts[(int)ToastPanel.toastType.MP] = r.MPAmount.ToString();
                }
            }
            if (r.EXPAmount != 0) {
                int EXPAmountToAdd = Mathf.Max((int)(r.EXPAmount * PartyManager.instance.EXPmultiplier), 1);
                PartyManager.instance.AddEXP(r.EXPAmount);
                changes[(int)ToastPanel.toastType.EXP] = true;
                amounts[(int)ToastPanel.toastType.EXP] = EXPAmountToAdd.ToString();
            }
            if (r.WAXAmount != 0) {
                if (r.WAXAmount >= 0) {
                    PartyManager.instance.AddWAX(r.WAXAmount);
                    amounts[(int)ToastPanel.toastType.WAX] = "+" + r.WAXAmount.ToString();
                }
                else {
                    PartyManager.instance.LoseWAX(r.WAXAmount * -1);
                    amounts[(int)ToastPanel.toastType.WAX] = r.WAXAmount.ToString();
                }
                changes[(int)ToastPanel.toastType.WAX] = true;
                
            }
            if (r.PROGAmount != 0) {
                int PROGAmountToAdd;
                if (r.PROGAmount < 0) {
                    PROGAmountToAdd =  Mathf.Min((int)(currentResult.PROGAmount * PartyManager.instance.PROGmultiplier), -1);
                }
                else {
                    PROGAmountToAdd =  Mathf.Max((int)(currentResult.PROGAmount * PartyManager.instance.PROGmultiplier), 1);
                }

                subAreaProgress += PROGAmountToAdd;
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
                if (r.PROGAmount > 0) {
                    amounts[(int)ToastPanel.toastType.PROGRESS] = "+" + PROGAmountToAdd.ToString();
                }
                else {
                    amounts[(int)ToastPanel.toastType.PROGRESS] = PROGAmountToAdd.ToString();
                }
                
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
        /// 
        /// </summary>
        public void DisplayPastItems() {
            List<Item> pastItems = new List<Item>();
            pastItems.Add(GameManager.instance.pastItem);
            eventDisplays[0].SetItemDisplays(pastItems);
        }

        /// <summary>
        /// Updates all WAX amount displays to show the accurate number
        /// </summary>
        public void UpdateWAXAmounts(){
            infoPanel.UpdateAmounts();
            if (UIManager.instance.inShop) {
                if (shopToastIndex == 0) {
                    toastPanel0.UpdateWAXAmount(); 
                }
                else {
                    toastPanel1.UpdateWAXAmount(); 
                }
            }
        }

        /// <summary>
        /// Updates the partyPanels and skillsPanels 
        /// </summary>
        /// <remark>
        /// This is used for when a new partyMember is added, as the partyPanel and skillsPanel
        /// won't immediately add the new pmds unless init() is called.
        /// </remark>
        public void UpdatePartyMembers() {
            partyPanel.Init(PartyManager.instance.GetPartyMembers());
            skillsPanel.Init();
        }

        /// <summary>
        /// Tell the infoPanel to add a quest
        /// </summary>
        /// <param name="questName"></param>
        public void AddQuest(string questName, string startEvent, string nextEvent) {
            infoPanel.AddQuest(questName, startEvent, nextEvent);
            SetQuestNotification(questName);
        }

        /// <summary>
        /// Tells the infoPanel to add a quest, but no notification for the player
        /// </summary>
        /// <param name="questName"></param>
        public void AddQuestNoNotification(string questName, string startEvent, string nextEvent) {
            infoPanel.AddQuest(questName, startEvent, nextEvent);
        }

        public void UpdateQuest(string questName, string nextEvent) {
            infoPanel.UpdateQuest(questName, nextEvent);
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
        /// Fade ps1's particles to specified alpha value
        /// </summary>
        /// <param name="targetAlpha"></param>
        /// <returns></returns>
        public IEnumerator FadeParticles(float targetAlpha) {
            float timeStartedLerping = Time.time;
            float timeSinceStarted = Time.time - timeStartedLerping;
            float percentageComplete = timeSinceStarted * alphaLerpSpeed;

            var main1 = ps1.main;
            var startColor = main1.startColor;
            var colorMax = main1.startColor.colorMax;
            var colorMin = main1.startColor.colorMin;

            float prevAlpha = main1.startColor.colorMin.a;
            float alphaValue = main1.startColor.colorMin.a;

             while (alphaValue != targetAlpha) {
                timeSinceStarted = Time.time - timeStartedLerping;
                percentageComplete = timeSinceStarted * alphaLerpSpeed;

                alphaValue = Mathf.Lerp(prevAlpha, targetAlpha, percentageComplete);

                startColor.colorMax = new Color(colorMax.r, colorMax.g, colorMax.b, alphaValue);
                startColor.colorMin =  new Color(colorMin.r, colorMin.g, colorMin.b, alphaValue);
                main1.startColor = startColor;
                yield return new WaitForEndOfFrame();
            }
        }

        /// <summary>
        /// Alters all of the background's colour values (r, g, b) to the specified value 
        /// </summary>
        /// <param name="targetColourValue"> Float between 0f and 1f </param>F
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

        public void AlterParticleSystem() {
            var main0 = ps0.main;
            var emission0 = ps0.emission;
            var main1 = ps1.main;
            var emission1 = ps1.emission;
            switch (areaProgress) {
                case 0:
                    wz0.windMain = 0;
                    emission0.rateOverTime = 0;
                    emission1.rateOverTime = 0;
                    break;
                case 1:
                    wz0.windMain = 0;
                    emission0.rateOverTime = 0;
                    emission1.rateOverTime = 0;
                    break;
                case 2:
                    wz0.windMain = 15;
                    emission0.rateOverTime = 6;
                    emission1.rateOverTime = 10; // get it started so when the fade effect happens later, the sprite is active
                    main0.startLifetime = 3f;
                    break;
                case 3:
                case 6:
                    wz0.windMain = 80;
                    wz1.windMain = 80;
                    emission0.rateOverTime = 60;
                    emission1.rateOverTime = 10;
                    main0.startLifetime = 1.25f;
                    main1.startLifetime = 1.25f;
                    StartCoroutine(FadeParticles(0.2f));
                    break;
                case 4:
                case 7:
                    wz0.windMain = 120;
                    wz1.windMain = 120;
                    emission0.rateOverTime = 80;
                    emission1.rateOverTime = 15;
                    main0.startLifetime = 1.25f;
                    main1.startLifetime = 1.25f;
                    StartCoroutine(FadeParticles(0.2f));
                    break;
                case 5:
                case 8:
                    wz0.windMain = 15;
                    wz1.windMain = 80;
                    emission0.rateOverTime = 6;
                    emission1.rateOverTime = 10;
                    main0.startLifetime = 3f;
                    main1.startLifetime = 3f;
                    StartCoroutine(FadeParticles(0));
                    break;
                default: 
                    break;
            }
        }

        /// <summary>
        /// Plays the subArea BGM (respective to areaProgress)
        /// </summary>
        public void PlaySubAreaBGM() {
            if (currentSubArea.name == "main" + currentAreaName) {
                AudioManager.instance.PlayBGM(currentAreaName + areaProgress);
            }
            else {
                AudioManager.instance.PlayBGM(currentSubArea.name);
            }  
        }

        public void PlayPostBossBGM() {
            if (currentAreaName == "GreyWastes") {
                AudioManager.instance.PlayBGM("Wind");
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
        /// Checks if the game is over due to all partyMembers dying outside of combat
        /// </summary>
        public void CheckGameOver() {
            if (PartyManager.instance.GetNumPartyMembersAlive() == 0) {
                StartCoroutine(DisplayGameOver(false));
            }
        }

        /// <summary>
        /// Updates the midpoints passed (i.e. indexes of subAreas visited in between the first and last subAreas)
        /// </summary>
        public void UpdateMidPoints() {
            if (currentAreaName == "GreyWastes") {
                int subAreaIndex = currentArea.GetSubAreaIndex(currentResult.subAreaName0);
                if (subAreaIndex >= 2 && subAreaIndex <= 7) {
                    midPoints.Add(subAreaIndex);
                }
            }
        }

        /// <summary>
        /// Makes everything uninteractable and brings the player to the main menu
        /// </summary>
        /// <returns></returns>
        public IEnumerator DisplayGameOver(bool isWin) {
            GameManager.instance.DeleteSaveData();
            timer.StartTimer(false);
            aromaTimer.StartTimerAroma(false);
            if (isWin == false) {
                SetAllButtonsInteractable(false);
                yield return new WaitForSeconds(2f);    // DRAMATIC PAUSE ON DEATH
                GameManager.instance.timeTaken = -1;
            }
            else {
                GameManager.instance.timeTaken = timer.GetElapsedTime();
            }

            if (currentAreaName == "GreyWastes") {
                int numMidPointsMissing = 3 - midPoints.Count;  // each area has a set number of midPoints that can be passed (3 for GreyWastes)
                for (int i = 0; i < numMidPointsMissing; i++) {
                    midPoints.Add(-1);
                }
            }

            gameOverMenu.Init(isWin, midPoints, currentArea.name, currentArea.GetSubAreaIndex(currentSubArea.name), subAreaProgress, timer.GetTime());
            gameOverMenu.SetVisible(true);
        }

        #endregion

        #region [Section 2] EventDisplays

        /// <summary>
        /// Displays the event sprites in the eventDisplays
        /// </summary>
        public void ShowEventDisplays() {
            if (currentEvent.spriteNum == 1) {
                if (currentEvent.eventSprites[0].name == "Girl0") {
                    eventDisplays[0].SetSprite(girlSprites[0]);
                }
                else {
                    eventDisplays[0].SetSprite(currentEvent.eventSprites[0]);
                }

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
        /// Toggles the actions panel to swap between combat actions and interaction actions
        /// </summary>
        public void ToggleInteractionAndCombatActions() {
            PartyManager.instance.GetActivePartyMember().SetAttackValues();
            actionsPanel.ToggleInteractionAndCombatActions(PartyManager.instance.GetActivePartyMember());
        }

        /// <summary>
        /// Take all items from an event display
        /// </summary>
        public void TakeAllItems() {
            if (eventDisplays[0].itemNum > 0) {
                eventDisplays[0].TakeAllItems();
                actionsPanel.UpdateTakeAll(eventDisplays[0].numSpareFull);
            }
            else if (rewardsPanel.itemNum > 0) {
                rewardsPanel.TakeAllItems();
                actionsPanel.UpdateTakeAll(rewardsPanel.numSpareFull);
            }
        }

        /// <summary>
        /// Updates the usability of the takeAll button
        /// </summary>
        public void UpdateTakeAll() {
            if (eventDisplays[0].itemNum > 0) {
                actionsPanel.UpdateTakeAll(eventDisplays[0].numSpareFull);
            }
            else if (rewardsPanel.itemNum > 0) {
                actionsPanel.UpdateTakeAll(rewardsPanel.numSpareFull);
            }
        }

        /// <summary>
        /// Returns the event display if it can have an item placed in an available
        /// item slot, null otherwise
        /// </summary>
        /// <returns></returns>
        public EventDisplay TryPlaceItem() {
            if (eventDisplays[0].numSpareFull < eventDisplays[0].itemSlots.Length) {
                return eventDisplays[0];
            }

            return null;
        }

        /// <summary>
        /// Updates the WAX values of any consumable items displayed in event displays
        /// </summary>
        public void UpdateWAXValues() {
            if (eventDisplays[0].itemNum > 0) {
                eventDisplays[0].UpdateWAXValues();
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
            toastPanel0.SetShopNotification();    
            shopToastIndex = 0;
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
        /// Sets a toastPanel to show a partyMember has joined
        /// </summary>
        /// <param name="questName"></param>
        public void SetPartyMembertNotification(string pmName) {
            if (toastPanel0.gameObject.activeSelf == true) {
                toastPanel1.SetPartyMemberNotification(pmName);
            }
            else {
                toastPanel0.SetPartyMemberNotification(pmName);
            }
        }

        /// <summary>
        /// Sets a notification panel to display a tutorialNotification, which stay on screen until event transitions,
        /// or some edge case removes them
        /// </summary>
        /// <param name="tutorialName"></param>
        /// <param name="panelNum"> 
        /// Some tutorials are tied to tab buttons, only show the relevant tutorial 
        /// if the corresponding tab is clicked at the right time
        /// </param>
        public void SetTutorialNotification(string tutorialName, int panelNum = -1) {
            if (panelNum == 0) {
                toastPanel0.SetTutorialNotification(tutorialName);
            }
            else if (panelNum == 1) {
                toastPanel1.SetTutorialNotification(tutorialName);
            }
            else {
                if (toastPanel0.gameObject.activeSelf == true) {
                    toastPanel1.SetTutorialNotification(tutorialName);
                }
                else {
                    toastPanel0.SetTutorialNotification(tutorialName);
                }
            }
        }

        /// <summary>
        /// Set a notification panel to show an achievement has been unlocked
        /// </summary>
        /// <param name="index"></param>
        /// <param name="isHighLayer"> Certain achievements may only be displayed on a gameover (which means they need to trump the sorting order) </param>
        /// <param name="panelNum"></param>
        public void SetAchievementNotification(int index, bool isHighLayer = false, int panelNum = -1) {
            if (panelNum == 0) {
                toastPanel0.SetAchievementNotification(GameManager.instance.achievementSprites[index], index, isHighLayer);
            }
            else if (panelNum == 1) {
                toastPanel1.SetAchievementNotification(GameManager.instance.achievementSprites[index], index, isHighLayer);
            }
            else {
                if (toastPanel0.gameObject.activeSelf == true) {
                    toastPanel1.SetAchievementNotification(GameManager.instance.achievementSprites[index], index, isHighLayer);
                }
                else {
                    toastPanel0.SetAchievementNotification(GameManager.instance.achievementSprites[index], index, isHighLayer);
                }
            }
        }

        /// <summary>
        /// Sets a notification letting the player know the results of a stat check
        /// </summary>
        /// <param name="isSucc"></param>
        /// <param name="checkIndicator"></param>
        /// <param name="stat"></param>
        /// <param name="threshold"></param>
        public void SetStatCheckNotification(bool isSucc, int checkIndicator, int stat, int threshold) {
            toastPanel2.SetStatCheckNotification(isSucc, checkIndicator, stat, threshold);
        }

        /// <summary>
        /// Controls the visibility of the toast panels
        /// </summary>
        /// <param name="value"></param>
        public void SetToastPanelsVisible(bool value) {
            toastPanel0.SetVisible(value);
            toastPanel1.SetVisible(value);
            toastPanel2.SetVisible(value);
        }

        /// <summary>
        /// Hides the arrow shown by tutorial toast panels
        /// </summary>
        public void HideTutorialArrow() {
            if (toastPanel0.isArrowHolder) {
                toastPanel0.HideArrow();
            }
            else if (toastPanel1.isArrowHolder) {
                toastPanel1.HideArrow();
            }
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
                actionsPanel.SetToggleButtonInteractable(true);
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
                actionsPanel.SetToggleButtonInteractable(false);
                actionsPanel.SetActionsUsable(true);
                partyPanel.DisableButtons();
                skillsPanel.SetInteractable(false);
                itemsTabManager.SetAllButtonsUninteractable();
                utilityTabManager.SetAllButtonsUninteractable();
            }
        }

        /// <summary>
        /// True to turn on the pause menu, false otherwise
        /// </summary>
        /// <param name="value"></param>
        public void SetPauseMenu(bool value) {
            if (value == true) {
                Time.timeScale = 0;
                optionsMenu.gameObject.SetActive(true);
            
            }
            else {
                Time.timeScale = 1;
                optionsMenu.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// True to turn on the pause menu, false otherwise
        /// </summary>
        /// <param name="value"></param>
        public void SetHelpMenu(bool value) {
            if (value == true) {
                Time.timeScale = 0;
                helpMenu.gameObject.SetActive(true);
            
            }
            else {
                Time.timeScale = 1;
                helpMenu.gameObject.SetActive(false);
            }
        }

        public void RestartRun() {
            GameManager.instance.DeleteSaveData();
            optionsMenu.cg.interactable = false;
            Time.timeScale = 1;
            if ((areaProgress == 0 || areaProgress == 1) && currentSubArea.name == ("main" + currentAreaName)) {
                AudioManager.instance.shouldChangeBGM = true;
            }

            string[] partyComposition = PartyManager.instance.GetPartyComposition();
            PartyManager.instance.ResetGame();
            foreach (string pm in partyComposition) {
                PartyManager.instance.AddPartyMember(pm);
            }
            GameManager.instance.StartLoadNextScene("Area");
        }

        /// <summary>
        /// Ends the run, returning to the main menu
        /// </summary>
        public void EndRun(bool isGameOver) {
            optionsMenu.cg.interactable = false;
            Time.timeScale = 1;
            SaveRunAndGeneralData(isGameOver);
            PartyManager.instance.ResetGame();
            GameManager.instance.StartLoadNextScene("MainMenu");
        }

        /// <summary>
        /// Ends the run returning to the main menu, not saving current game data
        /// </summary>
        public void EndRunNoSave() {
            optionsMenu.cg.interactable = false;
            Time.timeScale = 1;
            PartyManager.instance.ResetGame();
            GameManager.instance.StartLoadNextScene("MainMenu");
        }

        /// <summary>
        /// Opens the respective item panel that an itemDisplay belongs in
        /// </summary>
        /// <param name="id"></param>
        public void OpenItemPanel(ItemDisplay id) {
            if (id.type == ItemConstants.GEAR) {
                itemsTabManager.OpenPanel(0);
                // when taking a gear from a non-inventory slot, default to the first party member that can use it
                PartyMember pm = PartyManager.instance.GetAvailablePartyMember(id);
                if (pm != null){    
                    PartyManager.instance.SetActivePartyMember(pm);
                }
            }
            else if (id.type == ItemConstants.CANDLE) {
                itemsTabManager.OpenPanel(1);
            }
            else if (id.type == ItemConstants.SPECIAL) {
                itemsTabManager.OpenPanel(2);
            }
        }

        /// <summary>
        /// Calls the partyManager's rotation method to display 
        /// another partyMember (via arrow buttons)
        /// </summary>
        /// <param name="amount"></param>
        public void RotatePartyMember(int amount) {
            PartyManager.instance.RotatePartyMember(amount);
        }

        /// <summary>
        /// Save all data relevant to continuing a playthrough at a later time 
        /// </summary>
        public void SaveRunAndGeneralData(bool isGameOver = false) {
            if (isGameOver == false) {
                RunData data = new RunData(PartyManager.instance.GetPartyMemberDatas(), PartyManager.instance.WAX, 
                gearPanel.GetSpareGearData(), candlesPanel.GetSpareCandleData(), specialPanel.GetSpareSpecialData(), 
                infoPanel.GetData(), areaProgress, GameManager.instance.tutorialTriggers, GameManager.instance.monstersKilled,
                GameManager.instance.WAXobtained, GameManager.instance.totalEvents, timer.GetElapsedTime(), midPoints, subAreaResets + 1,
                GameManager.instance.difficultyModifier);
                GameManager.instance.SaveRunData(data);
            }

            GameManager.instance.SaveGeneralData();
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
