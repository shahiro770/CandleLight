/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 11, 2019
* 
* The Attack class is used to store information about an action monsters and players can take
* to hurt another's feelings.
*
*/

using LocalizationManager = Localization.LocalizationManager;
using StatusEffectConstant = Constants.StatusEffectConstant;
using UnityEngine;

namespace Combat {

    [System.Serializable]
    public class Attack {

        [field: SerializeField] public string animationClipName { get; private set; }   /// <value> Name of animation to be played when used </value>
        [field: SerializeField] public string costType { get;  set; }           /// <value> Cost type (HP, MP) </value>
        [field: SerializeField] public string nameKey { get; set; }             /// <value> Key used to access localized text  </value>
        [field: SerializeField] public string name { get; private set; }        /// <value> Name of attack </value>
        [field: SerializeField] public string type { get; private set; }        /// <value> Type of attack </value>
        [field: SerializeField] public string costFormula { get; set; }         /// <value> Formula for calculating cost to use attack </value>      
        [field: SerializeField] public string damageFormula { get; private set; }     /// <value> Formula for calculating damage dealt by attack </value>
        [field: SerializeField] public int scope { get; private set; }          /// <value> int (enum) describing the possible targets the attack affects </value>
        [field: SerializeField] public string seNameKey { get; private set; }   /// <value> String for the seName in event description </value>
        [field: SerializeField] public StatusEffectConstant seName { get; private set; }         /// <value> Status effect of attack (enum) </value>
        [field: SerializeField] public int seDuration { get; private set; }     /// <value> Number of turns status effect lasts </value>       
        [field: SerializeField] public int baseSeChance { get; private set; }   /// <value> Base chance of status effect occurring </value>
        [field: SerializeField] public int seChance { get; set; }               /// <value> Chance of status effect occurring </value>       
        
        public int attackValue;  /// <value> Amount related to attack </value>
        public int costValue;    /// <value> Amount related to cost </value>

        /// <summary>
        /// Constructor to initialize attack's properties
        /// </summary>
        /// <param name="name"> Name of attack </param>
        /// <param name="type"> Type of attack </param>
        /// <param name="formula"> Amount of damage attack deals </param>
        /// <param name="costType"> Type of cost (MP or HP) </param>
        /// <param name="cost"> Cost of attack (in MP or HP) </param>
        /// <param name="scope"> String describing the targets this attack hits </param>
        /// <param name="animationClipName"> Animation clip to play when attack is used </param>
        public Attack(string name, string type, string damageFormula, StatusEffectConstant seName, int seDuration, 
        int seChance, string costType, string costFormula, int scope, string animationClipName) {
            this.name = name;
            this.type = type;
            this.damageFormula = damageFormula;
            this.seName = seName;
            if (seName != 0) {   // HACK cause all atatcks except the noneAttack in a partyMember are loaded AFTER the localizationMAnager is accessible
                seNameKey = LocalizationManager.instance.GetLocalizedValue(nameof(seName) + "_ed");
            }
            this.seDuration = seDuration;
            this.baseSeChance = seChance;
            this.seChance = seChance;
            this.costType = costType;
            this.costFormula = costFormula;            
            this.scope = scope;
            this.animationClipName = animationClipName; 
            nameKey = name + "_attack";
        }
    }
}
