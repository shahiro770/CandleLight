/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 11, 2019
* 
* The HighScoreData class is used to store information needed to store a highscore
*
*/

namespace General {

    [System.Serializable]
    public class HighScoreData {
        
        public string areaName;
        public string class0; 
        public string class1;
        public int score;
        public int subAreaIndex;

        public HighScoreData(string areaName, string class0, string class1, int score, int subAreaIndex) {
            this.areaName = areaName;
            this.class0 = class0;
            this.class1 = class1;
            this.score = score;
            this.subAreaIndex = subAreaIndex;
        }
    }
}