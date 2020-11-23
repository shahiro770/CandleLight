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
using StatusEffectConstant = Constants.StatusEffectConstant;
using UnityEngine;

namespace Characters {

    [System.Serializable]
    public class StatusEffect {
        
        /* External component reference  */
        public StatusEffectDisplay sed;

        [field: SerializeField] public Character afflicter;                     /// <value> Character that caused the status effect </value>    
        [field: SerializeField] public Character afflicted;                     /// <value> Character that has the status effect </value>    
        [field: SerializeField] public StatusEffectConstant name { get; private set; }        /// <value> Name of status effect </value>
        [field: SerializeField] public int value { get; private set; }          /// <value> Calculated value from formula </value>
        [field: SerializeField] public int duration;                            /// <value> Turn duration of status </value>      
        [field: SerializeField] public bool isDispellable;                      /// <value> Flag for if the statusEffect can be removed </value>
        [field: SerializeField] public bool isBuff;                             /// <value> True if the statusEffect is a buff, false if debuff </value>
        [field: SerializeField] public bool plus;                               /// <value> Flag for if the statusEffect is more potent than normal </value>
        
        private int preValue = 0;  /// <value> Damage amount of status effect before reductions </value>

        /// <summary>
        /// Constructor to initialize statusEffect's properties
        /// </summary>
        public StatusEffect(StatusEffectConstant name, int duration) {
            this.name = name;
            
            this.duration = duration;
            
             switch(name) {
                case StatusEffectConstant.BURN:
                case StatusEffectConstant.POISON:
                case StatusEffectConstant.TAUNT:
                case StatusEffectConstant.FREEZE:
                case StatusEffectConstant.FROSTBITE:
                case StatusEffectConstant.BLEED:
                case StatusEffectConstant.WEAKNESS:
                case StatusEffectConstant.ROOT:
                case StatusEffectConstant.STUN:
                case StatusEffectConstant.SHOCK:
                case StatusEffectConstant.TRAP:
                case StatusEffectConstant.MARIONETTE:
                case StatusEffectConstant.SILENCE:
                case StatusEffectConstant.BREAK:
                case StatusEffectConstant.TANGIBLE:
                case StatusEffectConstant.SCUM:
                case StatusEffectConstant.FATALWOUND:
                    isBuff = false;
                    break;
                case StatusEffectConstant.RAGE:
                case StatusEffectConstant.ADVANTAGE:
                case StatusEffectConstant.VAMPIRE:
                case StatusEffectConstant.REGENERATE:
                case StatusEffectConstant.GUARD:
                case StatusEffectConstant.CURE:           
                case StatusEffectConstant.MIRACLE:
                case StatusEffectConstant.BARRIER:
                case StatusEffectConstant.NIMBLE:
                case StatusEffectConstant.ETHEREAL:
                case StatusEffectConstant.BOSS:
                case StatusEffectConstant.FAMILIAR:
                case StatusEffectConstant.CHAMPIONHP:
                case StatusEffectConstant.CHAMPIONPATK:
                case StatusEffectConstant.CHAMPIONMATK:
                case StatusEffectConstant.CHAMPIONPDEF:
                case StatusEffectConstant.CHAMPIONMDEF:
                    isBuff = true;
                    break;
            }
        }

        /// <summary>
        /// Initialize the status effect from saved data
        /// </summary>
        /// <param name="seData"></param>
        /// <returns></returns>
        public StatusEffect(StatusEffectData seData) : this(seData.name, seData.duration) {
            preValue = seData.preValue;
            plus = seData.plus;
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
            
            if (name == StatusEffectConstant.BURN) {
                 if (afflicter.burnPlus == true) {
                    plus = true;
                    preValue = (int)(afflicter.MATK * 0.5f);
                }
                else {
                    preValue = (int)(afflicter.MATK * 0.3f);
                }
                value = preValue;

                if (afflicted.GetStatusEffect(StatusEffectConstant.BOSS) != -1) {
                    value = (int)(value * 0.5f);
                }
                if (afflicted.GetStatusEffect(StatusEffectConstant.ROOT) != -1){
                    value *= 2;
                }
                value -= afflicted.MDEF;
            }
            if (name == StatusEffectConstant.FROSTBITE) {
                preValue = (int)(afflicter.PATK * 0.3f);
                value = preValue;

                if (afflicted.GetStatusEffect(StatusEffectConstant.BOSS) != -1) {
                    value = (int)(value * 0.5f);
                }
                if (afflicted.GetStatusEffect(StatusEffectConstant.FREEZE) != -1){
                    value *= 2;
                }
                value -= afflicted.MDEF;
            }
            else if (name == StatusEffectConstant.POISON) {
                if (afflicter != null && afflicter.poisonPlus == true) {
                    plus = true;
                    preValue = (int)(afflicted.HP * 0.15f);
                }
                else {
                    preValue = (int)(afflicted.HP * 0.1f);
                }

                PartyMember pm = afflicted as PartyMember;
                if (pm != null && pm.className == ClassConstants.ARCHER && pm.skills[(int)SkillConstants.ArcherSkills.SURVIVALIST].skillEnabled == true) {
                    value = 0;
                }
                else {
                    if (afflicted.GetStatusEffect(StatusEffectConstant.BOSS) != -1) {
                        value = (int)(preValue * 0.5f);
                    }
                    else { 
                        value = preValue;
                    }
                }
            }
            else if (name == StatusEffectConstant.TAUNT || name == StatusEffectConstant.MARIONETTE) {
                this.afflicter = afflicter;
            }
            else if (name == StatusEffectConstant.BLEED) {
                this.afflicter = afflicter;
                if (afflicter.bleedPlus == true) {
                    plus = true;
                    preValue = (int)(afflicter.PATK * 0.6f);
                }
                else {
                    preValue = (int)(afflicter.PATK * 0.4f);
                }

                PartyMember pm = afflicted as PartyMember;
                if (pm != null && pm.className == ClassConstants.ARCHER && pm.skills[(int)SkillConstants.ArcherSkills.SURVIVALIST].skillEnabled == true) {
                    value = 0;
                }
                else {
                    if (afflicted.GetStatusEffect(StatusEffectConstant.BOSS) != -1) {
                        value = (int)(preValue * 0.5f) - afflicted.PDEF;
                    }
                    else { 
                        value = preValue - afflicted.PDEF;
                    }
                }
            }
            else if (name == StatusEffectConstant.REGENERATE) {
                preValue = (int)(Mathf.Ceil((float)afflicted.HP * 0.05f));
                value = preValue;
            }
            else if (name == StatusEffectConstant.CHAMPIONHP) {
                preValue = (int)(Mathf.Ceil((float)(afflicted.HP * 1.66) * 0.05f)); // the bonus hp is not calculated in initially for champion HP
                value = preValue;
            }
            else if (name == StatusEffectConstant.FATALWOUND) {
                preValue = (int)(afflicted.HP * 0.33f);
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
            if (name == StatusEffectConstant.BURN) {
                value = preValue;

                if (afflicted.GetStatusEffect(StatusEffectConstant.BOSS) != -1) {
                    value = (int)(value * 0.5f);
                }
                if (afflicted.GetStatusEffect(StatusEffectConstant.ROOT) != -1){
                    value *= 2;
                }
                value -= afflicted.MDEF;
            }
            if (name == StatusEffectConstant.FROSTBITE) {
                value = preValue;

                if (afflicted.GetStatusEffect(StatusEffectConstant.BOSS) != -1) {
                    value = (int)(value * 0.5f);
                }
                if (afflicted.GetStatusEffect(StatusEffectConstant.FREEZE) != -1){
                    value *= 2;
                }
                value -= afflicted.MDEF;
            }
            else if (name == StatusEffectConstant.POISON) { // right now poison damage only needs to update for partyMembers
                PartyMember pm = afflicted as PartyMember;
                if (pm != null) {
                    if (pm.className == ClassConstants.ARCHER && pm.skills[(int)SkillConstants.ArcherSkills.SURVIVALIST].skillEnabled == true) {
                        value = 0;
                    }
                    else {  // skill reducing poison damage was toggled off 
                        value = preValue;
                    }
                }
            }
            else if (name == StatusEffectConstant.BLEED) {
                PartyMember pm = afflicted as PartyMember;
                if (pm != null && pm.className == ClassConstants.ARCHER && pm.skills[(int)SkillConstants.ArcherSkills.SURVIVALIST].skillEnabled == true) {
                    value = 0;
                }
                else if (afflicted.GetStatusEffect(StatusEffectConstant.BOSS) != -1) {  // if not a partyMember, bleed damage can also be halved from being a boss
                    value = (int)(preValue * 0.5f) - afflicted.PDEF;
                }
                else {
                    value = preValue - afflicted.PDEF;
                }
            }

            if (value < 0) {
                value = 0;
            }   

            sed.UpdateValue();
        }

        /// <summary>
        /// Return  this status effect's pertinent save data (to reinitialize from a load)
        /// </summary>
        /// <returns></returns>
        public StatusEffectData GetStatusEffectData() {
            return new StatusEffectData(name, duration, preValue, plus);
        }
    }
}
