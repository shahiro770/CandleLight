/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: June 1, 2019
* 
* The DataManager class is used to load in data so all
* loading of database stuff happens at the start
*
*/

using Characters;
using Events;
using Items;
using Party;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace General {

    public class DataManager : MonoBehaviour {

        public static DataManager instance;                     /// <value> global instance </value>
        
        public GameObject monsterDisplay;                       /// <value> Monster Prefab to instantiate </value>
        public float areaMultiplier { get; private set; }       /// <value> Multiplier to results for events in the area </value>
        public bool isReady { get; private set; } = false;      /// <value> Flag for if dataManager is done loading data </value>

        public Area currentArea;                 /// <value> Area to select subAreas from </value>
        public Consumable[][] areaConsumables;   /// <value> Consumable items that can be found in the current area </value>
        public Gear[][] areaGear;                /// <value> Gear items that can be found in the current area </value>
        public Candle[][] areaCandles;           /// <value> Candle items that can be found in the current area </value>
        public Special[][] areaSpecials;         /// <value> Special items that can be found in the current area</value>
        public BackgroundPack[] bgPacks;         /// <value> Background packs loaded in memory </value>
        public Interaction travelInt;
        public Interaction fightInt;
        public Interaction tutorialInt;

        private List<Monster> loadedMonsters = new List<Monster>();     /// <value> Monsters components currently loaded to reference respective gameObjects </value>
        private List<GameObject> loadedMonsterDisplayGameObjects = new List<GameObject>();     /// <value> Monster GameObjects currently loaded for cloning </value>

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
        /// Loads an Area from the database, waiting until all of subAreas, events,
        /// interactions, and results have been loaded before saying the area is ready.
        /// </summary>
        /// <param name="areaName"> Name of area to load </param>
        public IEnumerator LoadArea(string areaName) {
            currentArea = GameManager.instance.DB.GetAreaByName(areaName);
            SetAreaMultiplier();

            LoadBackgroundPacks();
            LoadGeneralInteractions();
            LoadAreaItems();
            PartyManager.instance.LoadSummons();
            yield return StartCoroutine(LoadMonsterDisplays());

            isReady = true;
        }

        /// <summary>
        /// Sets a multiplier for results from events in the area
        /// (i.e. gold results get increased in later areas)
        /// </summary>
        public void SetAreaMultiplier() {
            switch(currentArea.name) {
                case "GreyWastes":
                    areaMultiplier = 1.0f;
                    break;
            }
        }

        /// <summary>
        /// Load backgroundPacks for the current area
        /// </summary>
        public void LoadBackgroundPacks() {
            string[] bgPackNames = GameManager.instance.DB.GetBGPackNames(currentArea.name);
            bgPacks = new BackgroundPack[bgPackNames.Length];

            for (int i = 0; i < bgPackNames.Length; i++) {
                if (bgPackNames[i] != "none") {
                    bgPacks[i] = GameManager.instance.DB.GetBGPack(currentArea.name, bgPackNames[i]);
                }
            }
        }

        /// <summary>
        /// Load general interactions that many events might use
        /// </summary>
        public void LoadGeneralInteractions() {
            travelInt = GameManager.instance.DB.GetInteractionByName("travel");
            fightInt = GameManager.instance.DB.GetInteractionByName("fight");
            tutorialInt = GameManager.instance.DB.GetInteractionByName("loneGreyhide5");
        }

        public void LoadAreaItems() {
            areaConsumables = new Consumable[currentArea.subAreasNum][];
            areaGear = new Gear[currentArea.subAreasNum][];
            areaCandles = new Candle[currentArea.subAreasNum][];
            areaSpecials = new Special[currentArea.subAreasNum][];
            Consumable[] subAreaConsumables;
            Gear[] subAreaGear; 
            Candle[] subAreaCandles;
            Special[] subAreaSpecials; 

            for (int i = 0; i < currentArea.subAreasNum; i++) {
                subAreaConsumables = GameManager.instance.DB.GetConsumablesBySubArea(currentArea.GetSubArea(i).name);
                areaConsumables[i] = subAreaConsumables;
            }

            for (int i = 0; i < currentArea.subAreasNum; i++) {
                subAreaGear = GameManager.instance.DB.GetGearBySubArea(currentArea.GetSubArea(i).name);
                areaGear[i] = subAreaGear;
            }

            for (int i = 0; i < currentArea.subAreasNum; i++) {
                subAreaCandles = GameManager.instance.DB.GetCandlesBySubArea(currentArea.GetSubArea(i).name);
                areaCandles[i] = subAreaCandles;
            }

            for (int i = 0; i < currentArea.subAreasNum; i++) {
                subAreaSpecials = GameManager.instance.DB.GetSpecialsBySubArea(currentArea.GetSubArea(i).name);
                areaSpecials[i] = subAreaSpecials;
            }
        }

        /// <summary>
        /// Returns a loaded monster game object
        /// </summary>
        /// <param name="monsterName"> Name of monster to load </param>
        /// <returns> Initialized monsterDisplay game object </returns>
        public GameObject GetLoadedMonsterDisplay(string monsterNameID) {
            int index = loadedMonsters.FindIndex(m => m.monsterNameID == monsterNameID);    
            return loadedMonsterDisplayGameObjects[index];
        }

        /// <summary>
        /// Loads all monsters with a given name
        /// </summary>
        /// <param name="monsterNames"> Array containing strings of all monsters to be loaded </param>
        /// <returns> IEnumerator to wait for each monster to be loaded before moving to the next </returns>
        public IEnumerator LoadMonsterDisplays() {
            for (int i = 0; i < currentArea.subAreasNum; i++) {
                string[] monsterNames = currentArea.GetSubArea(i).monsterPool;
                foreach (string monsterName in monsterNames) {
                    if (monsterName == "none") {
                        break;
                    };
                    GameObject newMonsterDisplay = Instantiate(monsterDisplay);
                    
                    Monster monsterComponent = newMonsterDisplay.GetComponent<Monster>();
                    Monster newMonsterComponent = monsterComponent;

                    GameManager.instance.DB.GetMonsterByNameID(monsterName, newMonsterComponent);

                    while (newMonsterComponent.isReady == false) {
                        yield return null;
                    }
                    newMonsterDisplay.SetActive(false);

                    loadedMonsters.Add(monsterComponent);
                    loadedMonsterDisplayGameObjects.Add(newMonsterDisplay);
                    newMonsterDisplay.transform.SetParent(this.transform);
                }
            } 

            yield break;
        }

        /// <summary>
        /// Instantiated monster components don't copy their monsterGear[]
        /// so it needs to be copied over manually
        /// </summary>
        /// <param name="monsterNameID"></param>
        /// <returns></returns>
        public Gear[] GetMonsterGear(string monsterNameID) {
            return loadedMonsters.Find(m => m.monsterNameID == monsterNameID).monsterGear;  
        }

        /// <summary>
        /// Destroy all loaded monster game objects and clears all references
        /// </summary>
        public void ClearMonsters() {
            foreach(GameObject lmdgo in loadedMonsterDisplayGameObjects) {
                DestroyImmediate(lmdgo); 
            }

            loadedMonsterDisplayGameObjects.Clear();
            loadedMonsters.Clear();
        }
    }
}
