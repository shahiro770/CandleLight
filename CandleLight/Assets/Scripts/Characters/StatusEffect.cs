/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 11, 2019
* 
* The StatusEffect class is used to hold and retrieve information about a temporary change
* to a character's stats.
*/

using StatusEffectDisplay = PlayerUI.StatusEffectDisplay;
using StatusEffectConstants = Constants.StatusEffectConstants;
using UnityEngine;

namespace Characters {

    [System.Serializable]
    public class StatusEffect {
        
        /* External component reference  */
        public StatusEffectDisplay sed;

        [field: SerializeField] public Character tauntTarget;                   /// <value> Target to attack when taunted </value>      
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

        public void SetDisplay(StatusEffectDisplay sed) {
            this.sed = sed;
        }

        public void DestroyDisplay() {
            UnityEngine.GameObject.Destroy(sed.gameObject);
        }

        /// <summary>
        /// Sets the value for a status effect (e.g. damage, defense buff amount)
        /// </summary>
        /// <param name="afflicter"></param>
        /// <param name="afflicted"></param>
        public void SetValue(Character afflicter, Character afflicted) {
            if (name == StatusEffectConstants.BURN) {
                value = (int)(1 + afflicter.MATK * 0.2f);
            }
            else if (name == StatusEffectConstants.POISON) {
                value = (int)(1 + afflicted.HP * 0.07f);
            }
            else if (name == StatusEffectConstants.TAUNT) {
                tauntTarget = afflicter;
            }
        }

        public void UpdateDuration() {
            duration--;
            sed.UpdateText();
        }
    }
}
