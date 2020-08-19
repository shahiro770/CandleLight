/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 11, 2019
* 
* The SaveData class is used to store information needed to start the game relative to where the player left off. 
*
*/

using Items;
using Party;
using System.Collections.Generic;

namespace General {

    [System.Serializable]
    public class SaveData {

        public PartyMemberData[] partyMemberDatas;
        public ItemData[] spareGear;
        public ItemData[] spareCandles;
        public ItemData[] spareSpecials;
        public string[][] questData;
        public List<int> midPoints;
        public bool[] tutorialTriggers;
        public float elapsedTime;
        public int WAX;
        public int subAreaProgress;
        public int areaProgress;
        public int monstersKilled = 0;                  /// <value> Number of monsters killed </value>
        public int WAXobtained = 0;                     /// <value> Amount of WAX obtained (doesn't matter if its spent) </value>
        public int totalEvents = 0;                     /// <value> Total number of events visited </value>

        public SaveData(PartyMemberData[] partyMemberDatas, int WAX, ItemData[] spareGear, ItemData[] spareCandles, ItemData[] spareSpecials, 
        string[][] questData, int areaProgress, bool[] tutorialTriggers, int monstersKilled, int WAXObtained, int totalEvents, 
        float elapsedTime, List<int> midPoints) {
            this.partyMemberDatas = partyMemberDatas;
            this.WAX = WAX;
            this.spareGear = spareGear;
            this.spareCandles = spareCandles;
            this.spareSpecials = spareSpecials;
            this.questData = questData;
            this.areaProgress = areaProgress;
            this.tutorialTriggers = tutorialTriggers;
            this.monstersKilled = monstersKilled;
            this.WAXobtained = WAXObtained;
            this.totalEvents = totalEvents;
            this.elapsedTime = elapsedTime;
            this.midPoints = midPoints;
        }
    }
}