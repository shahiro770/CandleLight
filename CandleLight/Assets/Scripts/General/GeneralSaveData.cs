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
        public ItemData pastItemData;
        public bool[] tutorialTriggers;
        public bool[] achievementsUnlocked;
        public string[,] partyCombos;
        public bool isTimer;
        public float animationSpeed;
        public float bgmVolume;
        public float sfxVolume;
        public int mostEnemies;
        public int mostWAX;
        public int mostEvents;
        public float fastestTime;
       
        public GeneralSaveData(ItemData pastItemData, HighScoreData[] hsds, bool[] tutorialTriggers, bool[] achievementsUnlocked, string[,] partyCombos, bool isTimer, 
        float animationSpeed, float bgmVolume, float sfxVolume, int mostEnemies = -1, int mostWAX = -1, int mostEvents = -1, float fastestTime = -1) {
            this.hsds = hsds;
            this.tutorialTriggers = tutorialTriggers;
            this.achievementsUnlocked = achievementsUnlocked;
            this.partyCombos = partyCombos;
            this.pastItemData = pastItemData;
            this.isTimer = isTimer;
            this.animationSpeed = animationSpeed;
            this.bgmVolume = bgmVolume;
            this.sfxVolume = sfxVolume;
            this.mostEnemies = mostEnemies;
            this.mostWAX = mostWAX;
            this.mostEvents = mostEvents;
            this.fastestTime = fastestTime;
        }
    }
}