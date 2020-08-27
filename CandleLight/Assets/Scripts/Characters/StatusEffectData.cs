/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 11, 2019
* 
* The StatusEffectData class is used to store information about a statusEffect a partyMember had
* when saving
*
*/

namespace Characters {

    [System.Serializable]
    public class StatusEffectData {
      
        public string name;          
        public int duration;      
        public int preValue;                                                   
        public bool plus;                              

        public StatusEffectData(string name, int duration, int preValue, bool plus) {
            this.name = name;
            this.preValue = preValue;
            this.duration = duration;
            this.plus = plus;
        }
    }
}