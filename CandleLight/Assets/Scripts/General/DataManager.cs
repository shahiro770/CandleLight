/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: June 1, 2019
* 
* The DataManager class is used to preload in data from downloaded asset bundles.
* For now it only preloads monsters.
*
*/

using Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace General {

    public class DataManager : MonoBehaviour {

        public static DataManager instance;                     /// <value> global instance </value>
        
        public GameObject monsterDisplay;                       /// <value> Monster Prefab to instantiate </value>
        public bool isReady { get; private set; } = false;      /// <value> Flag for when DataManager is done loading </value>
         
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
        /// Returns a loaded monster game object
        /// </summary>
        /// <param name="monsterName"> Name of monster to load </param>
        /// <returns> Initialized monsterDisplay game object </returns>
        public GameObject GetLoadedMonsterDisplay(string monsterName) {
            int index = loadedMonsters.FindIndex(m => m.monsterNameID == monsterName);    
            return loadedMonsterDisplayGameObjects[index];
        }

        /// <summary>
        /// Loads all monsters with a given name
        /// </summary>
        /// <param name="monsterNames"> Array containing strings of all monsters to be loaded </param>
        /// <returns> IEnumerator to wait for each monster to be loaded before moving to the next </returns>
        public IEnumerator LoadMonsterDisplays(string[] monsterNames) {
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

            isReady = true;
            yield break;
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
