/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 11, 2019
* 
* The StatusEffect class is used to hold and retrieve information about a temporary change
* to a character's stats.
*/

using StatusEffectConstants = Constants.StatusEffectConstants;
using UnityEngine;

namespace Characters {

    [System.Serializable]
    public class StatusEffect {

        [field: SerializeField] public string animationClipName { get; private set; }   /// <value> Name of animation to be played when effect happens </value>
        [field: SerializeField] public string nameKey { get; set; }             /// <value> Key used to access localized text  </value>
        [field: SerializeField] public string name { get; private set; }        /// <value> Name of status effect </value>
        [field: SerializeField] public int value { get; private set; }          /// <value> Calculated value from formula </value>
        [field: SerializeField] public int duration;                            /// <value> Turn duration of status </value>      

        /// <summary>
        /// Constructor to initialize statusEffect's properties
        /// </summary>
        public StatusEffect(string name, int duration) {
            this.name = name;
            this.duration = duration;
            nameKey = name + "_status";
        }

        public void SetValue(Character c) {
            if (name == StatusEffectConstants.BURN) {
                value = (int)(1 + c.MATK * 0.2f);
            }
        }

        public string GetAnimationClipName(Character c) {
            if (c.GetType().Name == "Monster") {
                return "M" + name + "SEAnimation";
            }
            
            return "PM" + name + "SEAnimation";
        }
    }
}
