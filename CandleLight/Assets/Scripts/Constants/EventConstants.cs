/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: March 17, 2019
* 
* The EventConstants class is used to store the types events as constants
*
*/

namespace Constants {

    public static class EventConstants {

        public static readonly string EXPLORE = "explore";  /// <value> Most events go in here </value>
        public static readonly string MAIN = "main";        /// <value> Related to the current area's story </value>
        public static readonly string NOTHING = "nothing";  /// <value> Nothing </value>
        public static readonly string COMBAT = "combat";    /// <value> Combat </value>
        public static readonly string SHOP = "shop";        /// <value> Buy things </value>
    }
}