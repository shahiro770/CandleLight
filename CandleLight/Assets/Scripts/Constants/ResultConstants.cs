/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: March 17, 2019
* 
* The ActionConstants class is used to store the types of actions as constants.
*
*/

namespace Constants {

    public static class ResultConstants {

        public static readonly string NORESULT = "noResult";
        public static readonly string ITEM = "item";        /// <value> Item can optionally be taken </value>
        public static readonly string SUBAREA = "subArea";  /// <value> Player is moved to a new subArea </value>
        public static readonly string EVENT = "event";      /// <value> Player is thrown into a specific event </value>
        public static readonly string STATSINGLE = "statSingle";        /// <value> partyMembers all recieve an instant stat change </value>
        public static readonly string STATALL = "statAll";    /// <value> partyMembers all recieve an instant stat change </value>
        public static readonly string PRECOMBAT = "preCombat";
        public static readonly string COMBAT = "combat";
        public static readonly string COMBATWITHSIDEEFFECTS = "combatWithSideEffects";
    }
}