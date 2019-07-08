/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: July 4, 2019
* 
* The Result class is used to hold the results of an event, for changes to EXP, HP, MP, and wax.
* For now it doesn't allow for items to be found, or permanent stat changes, or partyMember changes.
*/

using UnityEngine;

namespace Events {

    public class Result {

        private int minValue = 5;   /// <value> Min value for numeric rewards pre-multiplier </value>
        private int maxValue = 10;  /// <value> Max value for numeric rewards pre-multiplier </value>

        public string resultName;   /// <value> Name of result </value>
        public string resultKey;    /// <value> Localization key for displayed string for result </value>
        public string subAreaName;
        public string subEventName;
        public float quantity;      /// <value> Quantity of result 0.5, 1, 2 (low, medium, high) </value>
        public float multiplier;    /// <value> Multiplier on result depending on area </value>
        public int EXPChange;       /// <value> EXP affected 0, 1 (none, increased) </value>
        public int HPChange;        /// <value> HP affected 0, 1, -1 (none, increased, decreased) </value>
        public int MPChange;        /// <value> MP affected 0, 1, -1 (none, increased, decreased) </value>
        public int waxChange;       /// <value> wax affected 0, 1, -1 (none, increased, decreased) </value>
        public bool isUnique;       /// <value> true if result can only occur once per dungeon, false otherwise </value>


        public int EXPAmount { get; private set; } 
        public int HPAmount { get; private set; } 
        public int MPAmount { get; private set; } 
        public int waxAmount { get; private set; } 
        

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="resultName"> Name of result </param>
        /// <param name="resultKey"> String key for result </param>
        /// <param name="quantity"></param>
        /// <param name="changeValues"></param>
        public Result(string resultName, bool isUnique, string resultKey, string quantity, int[] changeValues,
        string subAreaName, string subEventName) {
            this.resultName = resultName;
            this.isUnique = isUnique;
            this.resultKey = resultKey;
            this.EXPChange = changeValues[0];
            this.HPChange = changeValues[1];
            this.MPChange = changeValues[2];
            this.waxChange = changeValues[3];
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
        /// Returns the generated results
        /// </summary>
        /// <returns></returns>
        public int[] GetResults() {
            GenerateResults();
            return new int[] { EXPAmount, HPAmount, MPAmount, waxAmount };
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
            waxAmount = GenerateAmount(waxChange);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public int GenerateAmount(int mode) {
            if (mode == 0) {
                return 0;
            }
            else if (mode == 1) {
                return (int)(Random.Range(minValue, maxValue) * multiplier * quantity);
            }
            else {  // mode == -1
                return (int)(Random.Range(minValue, maxValue) * multiplier * quantity * -1);
            }
        }
    }
}
