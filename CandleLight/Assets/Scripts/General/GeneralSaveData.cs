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
        public bool isFullscreen;
        public float difficultyModifier;
        public float animationSpeed;
        public float bgmVolume;
        public float sfxVolume;
        public float fastestTime;
        public int resolutionWidth;
        public int resolutionHeight;
        public int mostEnemies;
        public int mostWAX;
        public int mostEvents;

        // some values are defaulted just cause they'll be swapped with more logic in gameManager anyways
        public GeneralSaveData(ItemData pastItemData, HighScoreData[] hsds, bool[] tutorialTriggers, bool[] achievementsUnlocked, string[,] partyCombos, bool isTimer, 
        float animationSpeed, float bgmVolume, float sfxVolume, bool isFullscreen, int resolutionWidth, int resolutionHeight, 
        float difficultyModifier = 0.75f, int mostEnemies = -1, int mostWAX = -1, int mostEvents = -1, float fastestTime = -1) {
            this.hsds = hsds;
            this.tutorialTriggers = tutorialTriggers;
            this.achievementsUnlocked = achievementsUnlocked;
            this.partyCombos = partyCombos;
            this.pastItemData = pastItemData;
            this.isTimer = isTimer;
            this.isFullscreen = isFullscreen;
            this.animationSpeed = animationSpeed;
            this.bgmVolume = bgmVolume;
            this.sfxVolume = sfxVolume;
            this.resolutionWidth = resolutionWidth;
            this.resolutionHeight = resolutionHeight;
            this.difficultyModifier = difficultyModifier;
            this.mostEnemies = mostEnemies;
            this.mostWAX = mostWAX;
            this.mostEvents = mostEvents;
            this.fastestTime = fastestTime;
        }
    }
}