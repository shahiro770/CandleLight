/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 11, 2019
* 
* The SaveData class is used to store information needed to start the game relative to where the player left off. 
*
*/

using Items;
using Characters;
using Party;
using Quest = PlayerUI.Quest;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace General {

    [System.Serializable]
    public class SaveData {

        public PartyMemberData[] partyMemberDatas;
        public ItemData[] spareGear;
        public ItemData[] spareCandles;
        public ItemData[] spareSpecials;
        public string[][] quests;
        public int WAX;
        public int subAreaProgress;
        public int areaProgress;
        public string currentSubAreaName;

        public SaveData(PartyMemberData[] partyMemberDatas, int WAX, ItemData[] spareGear, ItemData[] spareCandles, ItemData[] spareSpecials, Quest[] quests) {
            this.partyMemberDatas = partyMemberDatas;
            this.WAX = WAX;
            this.spareGear = spareGear;
            this.spareCandles = spareCandles;
            this.spareSpecials = spareSpecials;
            this.quests = new string[quests.Length][];
            for (int i = 0; i < this.quests.Length; i++) {
                this.quests[i] = quests[i].GetQuestData();
            }
        }
    }
}