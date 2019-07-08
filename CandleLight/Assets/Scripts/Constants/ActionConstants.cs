/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: March 17, 2019
* 
* The ActionConstants class is used to store the types of actions as constants.
*
*/

namespace Constants {

    public static class ActionConstants {

        public static readonly string FLEE = "flee";        /// <value> Flee from combat event </value>
        public static readonly string UNDO = "undo";        /// <value> Undo previous action </value>
        public static readonly string NONE = "none";        /// <value> Does nothing </value>
        public static readonly string ATTACK = "attack";    /// <value> Interacts with monsters </value>
        public static readonly string INTERACTION = "interaction";   /// <value> Interacts with events with no monsters </value>
        public static readonly string LEAVE = "leave";      /// <value> Leave non-combat event </value>
    }
}