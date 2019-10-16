/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: July 4, 2019
* 
* The Result class is used to hold the results of an event, for changes to EXP, HP, MP, and WAX.
* For now it doesn't allow for items to be found, or permanent stat changes, or partyMember changes.
*/

using UnityEngine;

namespace Events {

    [System.Serializable]
    public class Result {

        [field: SerializeField] public string name { get; private set; }        /// <value> Name of result </value>
        [field: SerializeField] public string type { get; private set; }
        [field: SerializeField] public string scope { get; private set; }
        [field: SerializeField] public string subAreaName { get; private set; } /// <value> Name of subArea result leads to if possible, "none" otherwise </value>
        [field: SerializeField] public string subEventName{ get; private set; } /// <value> Name of event result leads to if possible, "none" otherwise </value>
        [field: SerializeField] public string resultKey;    /// <value> Localization key for displayed string for result </value>
        [field: SerializeField] public string itemType { get; private set; }                /// <value> Type of item (gear, consumables) </value>
        [field: SerializeField] public string[] specificItemNames { get; private set; }     /// <value> Names of specific items dropped </value>
        [field: SerializeField] public string itemQuality { get; private set; }             /// <value> String quality of item (low, med, high) </value>
        [field: SerializeField] public string[] specificMonsterNames;           /// <value> Names of monsters this event spawns </value>
        [field: SerializeField] public int EXPAmount { get; private set; }      /// <value> Amount of EXP result gives </value>
        [field: SerializeField] public int HPAmount { get; private set; }       /// <value> Amount of HP result gives </value>
        [field: SerializeField] public int MPAmount { get; private set; }       /// <value> Amount of MP result gives </value>
        [field: SerializeField] public int WAXAmount { get; private set; }      /// <value> Amount of WAX result gives </value>
        [field: SerializeField] public int itemAmount { get; private set; }     /// <value> Amount of items result gives </value>
        [field: SerializeField] public int specificItemAmount { get; private set; } = 0;    /// <value> Amount of specific items result gives </value>

        [field: SerializeField] private float quantity;      /// <value> Quantity of result 0.5, 1, 2 (low, medium, high) </value>
        [field: SerializeField] private float multiplier;    /// <value> Multiplier on result depending on area </value>
        [field: SerializeField] private int minValue = 5;    /// <value> Min value for numeric rewards pre-multiplier </value>
        [field: SerializeField] private int maxValue = 10;   /// <value> Max value for numeric rewards pre-multiplier </value>
        [field: SerializeField] private int EXPChange;       /// <value> EXP affected 0, 1 (none, increased) </value>
        [field: SerializeField] private int HPChange;        /// <value> HP affected 0, 1, -1 (none, increased, decreased) </value>
        [field: SerializeField] private int MPChange;        /// <value> MP affected 0, 1, -1 (none, increased, decreased) </value>
        [field: SerializeField] private int WAXChange;       /// <value> wax affected 0, 1, -1 (none, increased, decreased) </value>
        [field: SerializeField] private int monsterCount;    /// <value> MAx number of monsters this event can spawn </value>
        [field: SerializeField] private bool isUnique;       /// <value> true if result can only occur once per dungeon, false otherwise </value>

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="resultName"> Name of result </param>
        /// <param name="resultKey"> String key for result </param>
        /// <param name="quantity"></param>
        /// <param name="changeValues"></param>
        public Result(string name, string resultKey, string type, bool isUnique, string quantity, string scope,  int[] changeValues,
        string subAreaName, string subEventName, int monsterCount, string[] specificMonsterNames, string itemType, string[] specificItemNames, string itemQuality) {
            this.name = name;
            this.resultKey = resultKey;
            this.type = type;
            this.isUnique = isUnique;
            this.scope = scope;
            this.EXPChange = changeValues[0];
            this.HPChange = changeValues[1];
            this.MPChange = changeValues[2];
            this.WAXChange = changeValues[3];
            this.subAreaName = subAreaName;
            this.subEventName = subEventName;
            this.monsterCount = monsterCount;
            this.specificMonsterNames = specificMonsterNames;
            this.itemType = itemType;
            this.specificItemNames = specificItemNames;
            for (int i = 0; i < specificItemNames.Length; i++) {
                if (specificItemNames[i] != "none") {
                    specificItemAmount++;
                }
            }

            this.itemQuality = itemQuality;
            
            if (quantity == "none") {
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

            SetEventMultiplier();
        }

        /// <summary>
        /// Sets the multiplier for the results
        /// </summary>
        public void SetEventMultiplier() {
            multiplier = EventManager.instance.areaMultiplier;
        }

        /// <summary>
        /// Generates the values for EXP, HP, MP, and wax that the result will give
        /// </summary>
        public void GenerateResults() {
            if (type == "item" || type == "itemWithSubEvent") {
                itemAmount = (int)Random.Range(Mathf.Max(quantity, 1), quantity + 1);
            }
            else if (type == "combatWithSideEffects" || type == "statAll") {
                EXPAmount = GenerateAmount(EXPChange);
                HPAmount = GenerateAmount(HPChange);
                MPAmount = GenerateAmount(MPChange);
                WAXAmount = GenerateAmount(WAXChange);
            }
        }

        /// <summary>
        /// Generates an integer number between the minValue and maxValue
        /// </summary>
        /// <param name="mode"> 0, 1 or -1 indicating if the amount should be 0, positive, or negative respectively </param>
        /// <returns> Int </returns>
        public int GenerateAmount(int mode) {
            if (mode == 0) {
                return 0;
            }
            else if (mode == 1) {
                return (int)((float)Random.Range(minValue, maxValue) * multiplier * quantity);
            }
            else {  // mode == -1
                return (int)((float)Random.Range(minValue, maxValue) * multiplier * quantity * -1);
            }
        }
        
        public string[] GetMonstersToSpawn() {
            string[] monsterNames = new string[monsterCount];

            for (int i = 0; i < monsterNames.Length; i++) {
                monsterNames[i] = specificMonsterNames[i];
            }

            return monsterNames;
        }
    }
}
