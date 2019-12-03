/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: March 17, 2019
* 
* The ResultConstants class is used to store the types of results as constants.
*
*/

namespace Constants {

    public static class ResultConstants {

        public static readonly string NORESULT = "noResult";        /// <value> Nothing </value>
        public static readonly string ITEM = "item";                /// <value> Item can optionally be taken </value>
        public static readonly string ITEMWITHSUBEVENT = "itemWithSubEvent";            /// <value> Items can optionally be taken, and player is thrown into a specific event </value>
        public static readonly string SUBAREA = "subArea";          /// <value> Player is moved to a new subArea </value>
        public static readonly string EVENT = "event";              /// <value> Player is thrown into a random event </value>
        public static readonly string SUBEVENT = "subEvent";        /// <value> Player is thrown into a specific event </value>
        public static readonly string STATSINGLE = "statSingle";    /// <value> A single partyMember all recieve an instant stat change </value>
        public static readonly string STATALL = "statAll";          /// <value> All partyMembers all recieve an instant stat change </value>
        public static readonly string STATALLANDLEAVE = "statAllAndLeave";   /// <value> All partyMembers receive an instant stat change, and then can leave </value> 
        public static readonly string PRECOMBAT = "preCombat";      /// <value> Not sure what this is for </value>
        public static readonly string COMBAT = "combat";            /// <value> Player must fight monsters </value>
        public static readonly string COMBATWITHSIDEEFFECTS = "combatWithSideEffects";  /// <value> Something happens before player fights monsters </value>
        public static readonly string TAKEALLITEMS = "takeAllItems";                    /// <value> Take all items available </value>
        public static readonly string SUBAREAANDCOMBAT = "subAreaAndCombat";    /// <value> Move to new subArea, immediately fight specific enemies </value>
        public static readonly string SUBAREAANDCOMBATANDSUBAREA = "subAreaAndCombatAndSubArea";    /// <value> Move to new subArea, immediately fight specific enemies, and then move to another subArea on clearing </value>
        public static readonly string PROGRESS = "progress";    /// <value> Changes progress amount </value>
        public static readonly string END = "end";              /// <value> Ends the area, returning to the main menu </value>
    }
}