/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: July 4, 2019
* 
* The Result class is used to hold the results of an event, for changes to EXP, HP, MP, and WAX.
* For now it doesn't allow for items to be found, or permanent stat changes, or partyMember changes.
*/

using ResultConstants = Constants.ResultConstants;
using StatusEffectConstant = Constants.StatusEffectConstant;
using UnityEngine;

namespace Events {

    [System.Serializable]
    public class Result {

        [field: SerializeField] public string name { get; private set; }        /// <value> Name of result </value>
        [field: SerializeField] public string type { get; private set; }
        [field: SerializeField] public string scope { get; private set; }
        [field: SerializeField] public string subAreaName0 { get; private set; } /// <value> Name of first subArea result leads to if possible, "none" otherwise </value>
        [field: SerializeField] public string subAreaName1 { get; private set; } /// <value> Name of second subArea result leads to if possible, "none" otherwise </value>
        [field: SerializeField] public string subEventName{ get; private set; } /// <value> Name of event result leads to if possible, "none" otherwise </value>
        [field: SerializeField] public string resultKey;    /// <value> Localization key for displayed string for result </value>
        [field: SerializeField] public string itemType { get; private set; }                /// <value> Type of item (gear, consumables) </value>
        [field: SerializeField] public string[] specificItemNames { get; private set; }     /// <value> Names of specific items dropped </value>
        [field: SerializeField] public string itemQuality { get; private set; }             /// <value> String quality of item (low, med, high) </value>
        [field: SerializeField] public string[] specificMonsterNames;           /// <value> Names of monsters this event spawns </value>
        [field: SerializeField] public string newIntName;                       /// <value> Name of interaction to load if result has one </value>
        [field: SerializeField] public StatusEffectConstant seName;             /// <value> Name of the status effect this result causes </value>
        [field: SerializeField] public string questName;                        /// <value> Name of the quest this result gives</value>
        [field: SerializeField] public int EXPAmount { get; private set; }      /// <value> Amount of EXP result gives </value>
        [field: SerializeField] public int HPAmount { get; private set; }       /// <value> Amount of HP result gives </value>
        [field: SerializeField] public int MPAmount { get; private set; }       /// <value> Amount of MP result gives </value>
        [field: SerializeField] public int WAXAmount { get; private set; }      /// <value> Amount of WAX result gives </value>
        [field: SerializeField] public int PROGAmount { get; private set; }     /// <value> Amount of progress result gives </value>
        [field: SerializeField] public int itemAmount { get; private set; }     /// <value> Amount of items result gives </value>
        [field: SerializeField] public int specificItemAmount { get; private set; } = 0;    /// <value> Amount of specific items result gives </value>
        [field: SerializeField] public int seDuration { get; private set; } = 0;        /// <value> Duration of status effect </value>
        [field: SerializeField] public bool hasPostCombatPrompt { get; private set; }   /// <value> true if result has a custom post combat prompt for the subarea </value>

        [field: SerializeField] public int baseEXPAmount { get; private set; }      /// <value> Amount of EXP result gives </value>
        [field: SerializeField] public int baseHPAmount { get; private set; }       /// <value> Amount of HP result gives </value>
        [field: SerializeField] public int baseMPAmount { get; private set; }       /// <value> Amount of MP result gives </value>
        [field: SerializeField] public int baseWAXAmount { get; private set; }      /// <value> Amount of WAX result gives </value>
        [field: SerializeField] public int basePROGAmount { get; private set; }     /// <value> Amount of progress result gives </value>
        [field: SerializeField] private float quantity;      /// <value> Quantity of result 0.5, 1, 2 (low, medium, high) </value>
        [field: SerializeField] private int monsterCount;    /// <value> MAx number of monsters this event can spawn </value>
        [field: SerializeField] private bool isUnique;       /// <value> true if result can only occur once per dungeon, false otherwise </value>
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="resultName"> Name of result </param>
        /// <param name="resultKey"> String key for result </param>
        /// <param name="quantity"></param>
        /// <param name="changeValues"></param>
        public Result(string name, string resultKey, string type, bool isUnique, string quantity, string scope,  int[] amounts,
        string subAreaName0, string subAreaName1, string subEventName, int monsterCount, string[] specificMonsterNames, string itemType, string[] specificItemNames, string itemQuality,
        string newIntName, StatusEffectConstant seName, int seDuration, bool hasPostCombatPrompt, string questName) {
            this.name = name;
            this.resultKey = resultKey;
            this.type = type;
            this.isUnique = isUnique;
            this.scope = scope;
            this.baseEXPAmount = amounts[0];
            this.baseHPAmount = amounts[1];
            this.baseMPAmount = amounts[2];
            this.baseWAXAmount = amounts[3];
            this.basePROGAmount = amounts[4];
            this.subAreaName0 = subAreaName0;
            this.subAreaName1 = subAreaName1;
            this.subEventName = subEventName;
            this.monsterCount = monsterCount;
            this.specificMonsterNames = specificMonsterNames;
            this.itemType = itemType;
            this.specificItemNames = specificItemNames;
            this.newIntName = newIntName;
            this.seName = seName;
            this.seDuration = seDuration;
            this.hasPostCombatPrompt = hasPostCombatPrompt;
            this.questName = questName;

            for (int i = 0; i < specificItemNames.Length; i++) {
                if (specificItemNames[i] != "none") {
                    specificItemAmount++;
                }
            }

            this.itemQuality = itemQuality;
            
            if (quantity == "none") {   // for items, this leads to exactly one item being generated
                this.quantity = 0;
            }
            else if (quantity == "low") {
                this.quantity = 0.5f;
            }
            else if (quantity == "med") {
                this.quantity = 1;
            }
            else if (quantity == "high") {
                this.quantity = 2;
            }
        }

        /// <summary>
        /// Generates the values for EXP, HP, MP, and wax that the result will give
        /// </summary>
        public void GenerateResults() {
            itemAmount = (int)Random.Range(Mathf.Max(quantity, 1), quantity + 2);
            EXPAmount = (int)(baseEXPAmount * Random.Range(quantity, quantity + 1));
            HPAmount = (int)(baseHPAmount * Random.Range(quantity, quantity + 1));
            MPAmount = (int)(baseMPAmount * Random.Range(quantity, quantity + 1));
            WAXAmount = (int)(baseWAXAmount * Random.Range(quantity, quantity + 1));
            PROGAmount = (int)(basePROGAmount * Random.Range(quantity, quantity + 1));
        }
        
        public string[] GetMonstersToSpawn() {
            string[] monsterNames = new string[monsterCount];

            for (int i = 0; i < monsterNames.Length; i++) {
                monsterNames[i] = specificMonsterNames[i];
            }

            return monsterNames;
        }

        /// <summary>
        /// Upgrades the result to be higher quality
        /// </summary>
        public void UpgradeResult() {
            if (itemQuality == "trash") {
                itemQuality = "low";
            }
            else if (itemQuality == "low") {
                itemQuality = "med";
            }
            else if (itemQuality == "med") {
                itemQuality = "high";
            }
            else if (itemQuality == "high") {
                itemQuality = "perfect";
            }
        }

        /// <summary>
        /// Downgrades the result to be of lower quality
        /// </summary>
        public void DowngradeResult() {
            if (itemQuality == "low") {
                itemQuality = "trash";
            }
            else if (itemQuality == "med") {
                itemQuality = "low";
            }
            else if (itemQuality == "high") {
                itemQuality = "med";
            }
            else if (itemQuality == "perfect") {
                itemQuality = "high";
            }
        }
    }
}
