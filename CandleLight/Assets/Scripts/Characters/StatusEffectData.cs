/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 11, 2019
* 
* The StatusEffectData class is used to store information about a statusEffect a partyMember had
* when saving
*
*/

using StatusEffectConstant = Constants.StatusEffectConstant;

namespace Characters {

    [System.Serializable]
    public class StatusEffectData {
      
        public StatusEffectConstant name;          
        public int duration;      
        public int preValue;                                                   
        public bool plus;                              

        public StatusEffectData(StatusEffectConstant name, int duration, int preValue, bool plus) {
            this.name = name;
            this.preValue = preValue;
            this.duration = duration;
            this.plus = plus;
        }
    }
}