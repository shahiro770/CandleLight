/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 11, 2019
* 
* The StatusEffect class is used to hold and retrieve information about a temporary change
* to a character's stats.
*/

using ClassConstants = Constants.ClassConstants;
using SkillConstants = Constants.SkillConstants;
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
        [field: SerializeField] public bool isDispellable;                      /// <value> Flag for if the statusEffect can be removed </value>
        [field: SerializeField] public bool isBuff;                             /// <value> True if the statusEffect is a buff, false if debuff </value>
        [field: SerializeField] public bool plus;                               /// <value> Flag for if the statusEffect is more potent than normal </value>
        
        private int preValue = 0;  /// <value> Damage amount of status effect before reductions </value>

        /// <summary>
        /// Constructor to initialize statusEffect's properties
        /// </summary>
        public StatusEffect(string name, int duration) {
            this.name = name;
            this.duration = duration;
            
             switch(name) {
                case StatusEffectConstants.BURN:
                case StatusEffectConstants.POISON:
                case StatusEffectConstants.TAUNT:
                case StatusEffectConstants.FREEZE:
                case StatusEffectConstants.BLEED:
                case StatusEffectConstants.WEAKNESS:
                case StatusEffectConstants.ROOT:
                case StatusEffectConstants.STUN:
                case StatusEffectConstants.SHOCK:
                case StatusEffectConstants.TRAP:
                case StatusEffectConstants.FATALWOUND:
                    isBuff = false;
                    break;
                case StatusEffectConstants.RAGE:
                case StatusEffectConstants.ADVANTAGE:
                case StatusEffectConstants.REGENERATE:
                case StatusEffectConstants.GUARD:
                case StatusEffectConstants.CURE:           
                case StatusEffectConstants.BOSS:
                case StatusEffectConstants.MIRACLE:
                case StatusEffectConstants.CHAMPIONHP:
                case StatusEffectConstants.CHAMPIONPATK:
                case StatusEffectConstants.CHAMPIONMATK:
                case StatusEffectConstants.CHAMPIONPDEF:
                case StatusEffectConstants.CHAMPIONMDEF:
                    isBuff = true;
                    break;
            }

            nameKey = name + "_status";
        }

        public void SetDisplay(StatusEffectDisplay sed) {
            this.sed = sed;
        }

        public void DestroyDisplay() {
            //sed.UmirrorMirroringDisplay();
            UnityEngine.GameObject.Destroy(sed.gameObject);
        }

        /// <summary>
        /// Sets the value for a status effect (e.g. damage, defense buff amount)
        /// </summary>
        /// <param name="afflicter"> Character that caused the status effect </param>
        /// <param name="afflicted"> Character being affected by the status effect </param>
        public void SetValue(Character afflicter, Character afflicted) {
            this.afflicted = afflicted;
            
            if (name == StatusEffectConstants.BURN) {
                preValue = (int)(afflicter.MATK * 0.3f);

                if (afflicted.GetStatusEffect(StatusEffectConstants.BOSS) != -1) {
                    value = (int)(preValue * 0.5f) - afflicted.MDEF;
                }
                else { 
                    value = preValue - afflicted.MDEF;
                }
            }
            else if (name == StatusEffectConstants.POISON) {
                preValue = (int)(afflicted.HP * 0.08f);

                PartyMember pm = afflicted as PartyMember;
                if (pm != null && pm.className == ClassConstants.ARCHER && pm.skills[(int)SkillConstants.archerSkills.SURVIVALIST].skillEnabled == true) {
                    value = (int)(preValue * 0.5f);
                }
                else {
                    if (afflicted.GetStatusEffect(StatusEffectConstants.BOSS) != -1) {
                        value = (int)(preValue * 0.5f);
                    }
                    else { 
                        value = preValue;
                    }
                }
            }
            else if (name == StatusEffectConstants.TAUNT) {
                this.afflicter = afflicter;
            }
            else if (name == StatusEffectConstants.BLEED) {
                this.afflicter = afflicter;
                if (afflicter.bleedPlus == true) {
                    plus = true;
                    preValue = (int)(afflicter.PATK * 0.7f);
                }
                else {
                    preValue = (int)(afflicter.PATK * 0.4f);
                }

                PartyMember pm = afflicted as PartyMember;
                if (pm != null && pm.className == ClassConstants.ARCHER && pm.skills[(int)SkillConstants.archerSkills.SURVIVALIST].skillEnabled == true) {
                    value = (int)(preValue * 0.5f) - afflicted.PDEF;
                }
                else {
                    if (afflicted.GetStatusEffect(StatusEffectConstants.BOSS) != -1) {
                        value = (int)(preValue * 0.5f) - afflicted.PDEF;
                    }
                    else { 
                        value = preValue - afflicted.PDEF;
                    }
                }
            }
            else if (name == StatusEffectConstants.CHAMPIONHP || name == StatusEffectConstants.REGENERATE) {
                preValue = (int)(Mathf.Ceil((float)afflicted.HP * 0.05f));
                value = preValue;
            }
            else if (name == StatusEffectConstants.FATALWOUND) {
                preValue = (int)(afflicted.HP * 0.3f);
                value = preValue;
            }

            if (value < 0) {
                value = 0;
            }   
        }

        /// <summary>
        /// Updates the duration of the statusEffect and then updates the display
        /// </summary>
        /// <param name="value"> Amount duration is increased/decreased by </param>
        public void UpdateDuration(int value) {
            duration += value;
            if (duration < 0) {
                duration = 0;
            }
            sed.UpdateText();
        }

        /// <summary>
        /// Updates the value of the statusEffect and then updates the display
        /// </summary>
        public void UpdateValue() {
            if (name == StatusEffectConstants.BURN) {
                if (afflicted.GetStatusEffect(StatusEffectConstants.ROOT) != -1){
                     value = preValue * 2 - afflicted.MDEF ;
                }
                else {
                    value = preValue - afflicted.MDEF;
                }
            }
            else if (name == StatusEffectConstants.POISON) { // right now poison damage only needs to update for partyMembers
                PartyMember pm = afflicted as PartyMember;
                if (pm != null) {
                    if (pm.className == ClassConstants.ARCHER && pm.skills[(int)SkillConstants.archerSkills.SURVIVALIST].skillEnabled == true) {
                        value = (int)(preValue * 0.5f);
                    }
                    else {  // skill reducing poison damage was toggled off 
                        value = preValue;
                    }
                }
            }
            else if (name == StatusEffectConstants.BLEED) {
                PartyMember pm = afflicted as PartyMember;
                if (pm != null && pm.className == ClassConstants.ARCHER && pm.skills[(int)SkillConstants.archerSkills.SURVIVALIST].skillEnabled == true) {
                    value = (int)(preValue * 0.5f) - afflicted.PDEF;
                }
                else {
                    value = preValue - afflicted.PDEF;;
                }
            }

            if (value < 0) {
                value = 0;
            }   

            sed.UpdateValue();
        }
    }
}
