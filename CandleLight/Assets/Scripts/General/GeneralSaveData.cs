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
        public bool[] aromas;
        public string[,] partyCombos;
        public bool isTimer;
        public bool isFullscreen;
        public float version;
        public float difficultyModifier;
        public float scoreModifier;
        public float animationSpeed;
        public float bgmVolume;
        public float sfxVolume;
        public float fastestTime;
        public int resolutionWidth;
        public int resolutionHeight;
        public int mostMonsters;
        public int mostWAX;
        public int mostEvents;
       
        // some values are defaulted just cause they'll be swapped with more logic in gameManager anyways
        public GeneralSaveData(float version, ItemData pastItemData, HighScoreData[] hsds, bool[] tutorialTriggers, bool[] achievementsUnlocked, bool[] aromas,
        string[,] partyCombos, bool isTimer, float scoreModifier, float animationSpeed, float bgmVolume, float sfxVolume, bool isFullscreen, 
        int resolutionWidth, int resolutionHeight, float difficultyModifier = 0.75f, int mostMonsters = -1, 
        int mostWAX = -1, int mostEvents = -1, float fastestTime = -1) {
            this.version = version;
            this.hsds = hsds;
            this.tutorialTriggers = tutorialTriggers;
            this.achievementsUnlocked = achievementsUnlocked;
            this.aromas = aromas;
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
            this.scoreModifier = scoreModifier;
            this.mostMonsters = mostMonsters;
            this.mostWAX = mostWAX;
            this.mostEvents = mostEvents;
            this.fastestTime = fastestTime;
        }
    }
}