

using ClassConstants = Constants.ClassConstants;
using UnityEngine;

namespace Items {

    [System.Serializable]
    public class ItemData {

        public string nameID;                           /// <value> Name of item </value>
        public string type { get; private set; }        /// <value> Type of item (consumable, weapon, secondary, armor) </value>
        public string subType { get; private set; }     /// <value> SubType of item (varies depending on type) </value>
        public string className = ClassConstants.ANY;    /// <value> Class that can use this item </value>

        public string[] effects = new string[3];            /// <value> List of effects </value>
        public int[] values = new int[3];                   /// <value> List of values associated with effects </value>
        public int effectsNum = 0;                          /// <value> Number of effects </value>

        public ItemData(Item i) {
            this.nameID = i.nameID;
            this.type = i.type;
            this.subType = i .subType;
            this.className = i.className;
            this.effects = i.effects;
            this.values = i.values;
            this.effectsNum = i.effectsNum;
        }
    }
}