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

        [field: SerializeField] public Character afflicter;                     /// <value> Character that caused the status effect </value>    
        [field: SerializeField] public Character afflicted;                     /// <value> Character that has the status effect </value>    
        [field: SerializeField] public string nameKey { get; set; }             /// <value> Key used to access localized text  </value>
        [field: SerializeField] public string name { get; private set; }        /// <value> Name of status effect </value>
        [field: SerializeField] public int value { get; private set; }          /// <value> Calculated value from formula </value>
        [field: SerializeField] public int duration;                            /// <value> Turn duration of status </value>      

        private int preValue = 0;  /// <value> Damage amount of status effect before reductions </value>

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
            this.afflicted = afflicted;
            
            if (name == StatusEffectConstants.BURN) {
                preValue = (int)(1 + afflicter.MATK * 0.2f);
                value = preValue - afflicted.MDEF;
            }
            else if (name == StatusEffectConstants.POISON) {
                preValue = (int)(1 + afflicted.HP * 0.07f);
                value = preValue;
            }
            else if (name == StatusEffectConstants.TAUNT) {
                this.afflicter = afflicter;
            }
            else if (name == StatusEffectConstants.BLEED) {
                this.afflicter = afflicter;
                preValue = (int)(1 + afflicter.PATK * 0.4f);
                value = preValue - afflicted.PDEF;
            }

            if (value < 0) {
                value = 0;
            }   
        }

        /// <summary>
        /// Updates the duration of the statusEffect and then updates the display
        /// </summary>
        public void UpdateDuration() {
            duration--;
            sed.UpdateText();
        }

        /// <summary>
        /// Updates the value of the statusEffect and then updates the display
        /// </summary>
        public void UpdateValue() {
            if (name == StatusEffectConstants.BURN) {
                value = preValue - afflicted.MDEF;
            }
            else if (name == StatusEffectConstants.POISON) {
                value = preValue;
            }
            else if (name == StatusEffectConstants.BLEED) {
                value = preValue - afflicted.PDEF;
            }

            if (value < 0) {
                value = 0;
            }   

            sed.UpdateValue();
        }
    }
}
