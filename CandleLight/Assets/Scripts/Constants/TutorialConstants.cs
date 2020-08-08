/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: March 17, 2019
* 
* The TutorialConstants class is used to store the tutorials passed
*
*/

namespace Constants {

    public static class TutorialConstants {

        public enum tutorialTriggers { 
            isTutorial,         /// <value> Flag for if the tutorial is enabled </value>
            isTips,             /// <value> Flag for if helpful tips should show up when possible </value>
            firstConsumable,    /// <value> Flag for if the player hasn't encountered their first consumable </value>
            firstCandle,        /// <value> Flag for if the player hasn't encountered their first candle </value> 
            firstShop,          /// <value> Flag for if the player hasn't encountered their first shop </value> 
            firstCandleCombat,  /// <value> Flag for if the player hasn't brought a candle into combat for the first time </value>
            firstChampion,      /// <value> Flag for if the player hasn't encountered their first champion monster </value>
            firstFailedSkillDisable,    /// <value> Flag for if the player fails to disable a skill (due to it being required to enable a column) </value>
            firstFailedSkillEnable      /// <value> Flag for if the player fails to enable a skill (due to have too many active skills) </value>
        };
    }
}