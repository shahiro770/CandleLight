/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 11, 2019
* 
* The GeneralSaveData class is used to store information needed to store data that won't be erased
*
*/

using Items;

namespace General {

    [System.Serializable]
    public class GeneralSaveData {

        public HighScoreData[] hsds;
        public ItemData pastItem;
        public bool[] tutorialTriggers;
        public bool isTimer;
        public float animationSpeed;
        public float bgmVolume;
        public float sfxVolume;
       
        public GeneralSaveData(ItemData pastItem, HighScoreData[] hsds, bool[] tutorialTriggers, bool isTimer, float animationSpeed, float bgmVolume, float sfxVolume) {
            this.hsds = hsds;
            this.tutorialTriggers = tutorialTriggers;
            this.pastItem = pastItem;
            this.isTimer = isTimer;
            this.animationSpeed = animationSpeed;
            this.bgmVolume = bgmVolume;
            this.sfxVolume = sfxVolume;
        }
    }
}