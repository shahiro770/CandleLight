/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 11, 2019
* 
* The GeneralSaveData class is used to store information needed to save data that won't be erased
* between runs
*
*/

using Items;

namespace General {

    [System.Serializable]
    public class GeneralSaveData {

        public HighScoreData[] hsds;        // data for all highscores 
        public ItemData pastItemData;       // information about an item from a past run (timekeeper gimick)
        public bool[] tutorialTriggers;     // list of tutorial notifications that have/haven't been triggered
        public bool[] achievementsUnlocked; // list of achievements that have been unlocked
        public bool[] aromas;               // list of aromas and their activeness (for menu purposes)
        public string[,] partyCombos;       // list of party combinations the player has beaten the game with
        public bool isTimer;                // flag for if the timer is visible
        public bool isFullscreen;           // flag for if the game is fullscreen
        public float version;               // version number
        public float difficultyModifier;    // difficulty modifier (0.75 is casual, 1 is normal) (for menu purposes)
        public float scoreModifier;         // score multiplier
        public float animationSpeed;        // speed at which animations play
        public float bgmVolume;             // background music volume
        public float sfxVolume;             // sound effects volume
        public float fastestTime;           // fastest time for clearing the game
        public int resolutionWidth;         // game width
        public int resolutionHeight;        // game height
        public int mostMonsters;            // most monsters killed in a run
        public int mostWAX;                 // most WAX earned in a run
        public int mostEvents;              // most events seen in a run
       
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

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="gsData"></param>
        public GeneralSaveData(GeneralSaveData gsData) {
            this.version = gsData.version;
            this.hsds = gsData.hsds;
            this.tutorialTriggers = gsData.tutorialTriggers;
            this.achievementsUnlocked = gsData.achievementsUnlocked;
            this.aromas = new bool[gsData.aromas.Length];
            for (int i = 0; i < this.aromas.Length; i++) {
                this.aromas[i] = gsData.aromas[i];
            }
            this.partyCombos = gsData.partyCombos;
            this.pastItemData = gsData.pastItemData;
            this.isTimer = gsData.isTimer;
            this.isFullscreen = gsData.isFullscreen;
            this.animationSpeed = gsData.animationSpeed;
            this.bgmVolume = gsData.bgmVolume;
            this.sfxVolume = gsData.sfxVolume;
            this.resolutionWidth = gsData.resolutionWidth;
            this.resolutionHeight = gsData.resolutionHeight;
            this.difficultyModifier = gsData.difficultyModifier;
            this.scoreModifier = gsData.scoreModifier;
            this.mostMonsters = gsData.mostMonsters;
            this.mostWAX = gsData.mostWAX;
            this.mostEvents = gsData.mostEvents;
            this.fastestTime = gsData.fastestTime;
        }
    }
}