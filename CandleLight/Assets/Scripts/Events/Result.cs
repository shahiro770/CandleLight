/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 11, 2019
* 
* The Result class is used to hold the results of an event, for changes to EXP, HP, MP, and wax.
* For now it doesn't allow for items to be found, or permanent stat changes, or partyMember changes.
*/

using Actions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Events {

    public class Result {

        private int minValue = 5;
        private int maxValue = 10;

        public string resultName;
        public string resultKey;
        public float quantity;      /// <value> Quantity of result 0.5, 1, 2 (low, medium, high) </value>
        public float multiplier;    /// <value> Multiplier on result depending on area </value>
        public int EXPChange;       /// <value> EXP affected 0, 1 (none, increased) </value>
        public int HPChange;        /// <value> HP affected 0, 1, -1 (none, increased, decreased) </value>
        public int MPChange;        /// <value> MP affected 0, 1, -1 (none, increased, decreased) </value>
        public int waxChange;       /// <value> wax affected 0, 1, -1 (none, increased, decreased) </value>

        public int EXPAmount { get; private set; } 
        public int HPAmount { get; private set; } 
        public int MPAmount { get; private set; } 
        public int waxAmount { get; private set; } 
        

        public Result(string resultName, string resultKey, string quantity, int[] changeValues) {
            Debug.Log("Result " + resultName);
            this.resultName = resultName;
            this.resultKey = resultKey;
            this.EXPChange = changeValues[0];
            this.HPChange = changeValues[1];
            this.MPChange = changeValues[2];
            this.waxChange = changeValues[3];
            
            if (quantity == "low") {
                this.quantity = 0.5f;
            }
            else if (quantity == "med") {
                this.quantity = 1;
            }
            else if (quantity == "high") {
                this.quantity = 2;
            }

            SetEventMultiplier();
            GenerateResults();

            Debug.Log("Done Result");
        }

        public void GenerateResults() {
            EXPAmount = GenerateAmount(EXPChange);
            HPAmount = GenerateAmount(HPChange);
            MPAmount = GenerateAmount(MPChange);
            waxAmount = GenerateAmount(waxChange);
        }

        public int[] GetResults() {
            return new int[] { EXPAmount, HPAmount, MPAmount, waxAmount };
        }

        public void SetEventMultiplier() {
            multiplier = EventManager.instance.areaMultiplier;
        }

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
