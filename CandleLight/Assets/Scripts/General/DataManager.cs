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
        
        public GameObject monster;                              /// <value> Monster Prefab to instantiate </value>
        public bool isReady { get; private set; } = false;      /// <value> Flag for when DataManager is done loading </value>
         
        private List<Monster> loadedMonsters = new List<Monster>();                     /// <value> Monsters components currently loaded </value>
        private List<GameObject> loadedMonsterGameObjects = new List<GameObject>();     /// <value> Monster GameObjects currently loaded for cloning </value>
        private string[] greywastes1Monsters = { "Goblin LVL1" , "Greyhide LVL1", "Greyhide Alpha LVL3" };  /// <value> List of monsters from the Grey Wastes </Value>

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
        /// Load monsters from Grey Wastes, will change this later to load from a given area
        /// </summary>
        public void Init() {
            StartCoroutine(LoadMonsters(greywastes1Monsters));
        }

        /// <summary>
        /// Returns a loaded monster game object
        /// </summary>
        /// <param name="monsterName"> Name of monster to load </param>
        /// <returns> Initialized monster game object </returns>
        public GameObject GetLoadedMonster(string monsterName) {
            int index = loadedMonsters.FindIndex(m => m.monsterNameID == monsterName);    
            return loadedMonsterGameObjects[index];
        }

        /// <summary>
        /// Loads all monsters with a given name
        /// </summary>
        /// <param name="monsterNames"> Array containing strings of all monsters to be loaded </param>
        /// <returns> IEnumerator to wait for each monster to be loaded before moving to the next </returns>
        private IEnumerator LoadMonsters(string[] monsterNames) {
            foreach (string monsterName in monsterNames) {
                GameObject newMonster = Instantiate(monster);
                Monster monsterComponent = newMonster.GetComponent<Monster>() ;
                Monster newMonsterComponent = monsterComponent;
                
                GameManager.instance.DB.GetMonsterByNameID(monsterName, newMonsterComponent);
                loadedMonsters.Add(monsterComponent);
                loadedMonsterGameObjects.Add(newMonster);

                while (newMonsterComponent.isReady == false) {
                    yield return null;
                }
                newMonster.SetActive(false);
                newMonster.transform.SetParent(this.transform);
            }

            isReady = true;
            yield break;
        }

        /// <summary>
        /// Destroy all loaded monster game objects and clears all references
        /// </summary>
        public void ClearMonsters() {
            foreach(GameObject lmgo in loadedMonsterGameObjects) {
                DestroyImmediate(lmgo); 
            }

            loadedMonsterGameObjects.Clear();
            loadedMonsters.Clear();
        }
    }
}
