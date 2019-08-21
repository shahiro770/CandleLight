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

    public class Result {

        public string name { get; private set; }        /// <value> Name of result </value>
        public string type { get; private set; }
        public string subAreaName { get; private set; } /// <value> Name of subArea result leads to if possible, "none" otherwise </value>
        public string subEventName{ get; private set; } /// <value> Name of event result leads to if possible, "none" otherwise </value>
        public string resultKey;    /// <value> Localization key for displayed string for result </value>
        public int EXPAmount { get; private set; }      /// <value> Amount of EXP result gives </value>
        public int HPAmount { get; private set; }       /// <value> Amount of HP result gives </value>
        public int MPAmount { get; private set; }       /// <value> Amount of MP result gives </value>
        public int WAXAmount { get; private set; }      /// <value> Amount of WAX result gives </value>
        
        private float quantity;      /// <value> Quantity of result 0.5, 1, 2 (low, medium, high) </value>
        private float multiplier;    /// <value> Multiplier on result depending on area </value>
        private int minValue = 5;    /// <value> Min value for numeric rewards pre-multiplier </value>
        private int maxValue = 10;   /// <value> Max value for numeric rewards pre-multiplier </value>
        private int EXPChange;       /// <value> EXP affected 0, 1 (none, increased) </value>
        private int HPChange;        /// <value> HP affected 0, 1, -1 (none, increased, decreased) </value>
        private int MPChange;        /// <value> MP affected 0, 1, -1 (none, increased, decreased) </value>
        private int WAXChange;       /// <value> wax affected 0, 1, -1 (none, increased, decreased) </value>
        private bool isUnique;       /// <value> true if result can only occur once per dungeon, false otherwise </value>

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="resultName"> Name of result </param>
        /// <param name="resultKey"> String key for result </param>
        /// <param name="quantity"></param>
        /// <param name="changeValues"></param>
        public Result(string name, string type, bool isUnique, string resultKey, string quantity, int[] changeValues,
        string subAreaName, string subEventName) {
            this.name = name;
            this.type = type;
            this.isUnique = isUnique;
            this.resultKey = resultKey;
            this.EXPChange = changeValues[0];
            this.HPChange = changeValues[1];
            this.MPChange = changeValues[2];
            this.WAXChange = changeValues[3];
            this.subAreaName = subAreaName;
            this.subEventName = subEventName;
            
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
            EXPAmount = GenerateAmount(EXPChange);
            HPAmount = GenerateAmount(HPChange);
            MPAmount = GenerateAmount(MPChange);
            WAXAmount = GenerateAmount(WAXChange);
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
    }
}
