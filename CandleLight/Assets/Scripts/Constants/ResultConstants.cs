/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: March 17, 2019
* 
* The ResultConstants class is used to store the types of results as constants.
* NOTE: IF THE RESULT AFFECTS ITEMS, MAKE SURE IN Result.cs TO ADD IT TO THE ITEM GENERATING IF STATEMENT
*
*/

namespace Constants {

    public static class ResultConstants {

        public const string NORESULT = "noResult";        /// <value> Nothing </value>
        public const string NORESULTANDLEAVE = "noResultAndLeave";    /// <value> Nothing and leave </value>
        public const string ITEM = "item";                /// <value> Item can optionally be taken </value>
        public const string ITEMWITHSUBEVENT = "itemWithSubEvent";            /// <value> Items can optionally be taken, and player is thrown into a specific event </value>
        public const string SUBAREA = "subArea";          /// <value> Player is moved to a new subArea </value>
        public const string EVENT = "event";              /// <value> Player is thrown into a random event </value>
        public const string SUBEVENT = "subEvent";        /// <value> Player is thrown into a specific event </value>
        public const string STATSINGLE = "statSingle";    /// <value> A single partyMember all recieve an instant stat change </value>
        public const string STATALL = "statAll";          /// <value> All partyMembers all recieve an instant stat change </value>
        public const string STATALLANDLEAVE = "statAllAndLeave";      /// <value> All partyMembers receive an instant stat change, and then can leave </value> 
        public const string PRECOMBAT = "preCombat";      /// <value> Not sure what this is for </value>
        public const string COMBAT = "combat";            /// <value> Player must fight monsters </value>
        public const string COMBATWITHSIDEEFFECTS = "combatWithSideEffects";  /// <value> Something happens before player fights monsters </value>
        public const string COMBATANDSUBEVENT = "combatAndSubEvent";          /// <value> Combat, then a subEvent </value>
        public const string TAKEALLITEMS = "takeAllItems";                    /// <value> Take all items available </value>
        public const string SUBAREAANDCOMBAT = "subAreaAndCombat";    /// <value> Move to new subArea, immediately fight specific enemies </value>
        public const string SUBAREAANDCOMBATANDSUBAREA = "subAreaAndCombatAndSubArea";    /// <value> Move to new subArea, immediately fight specific enemies, and then move to another subArea on clearing </value>
        public const string END = "end";              /// <value> Ends the area, returning to the main menu </value>
        public const string REVIVE = "revive";        /// <value> Bring all dead partyMembers back to life at 30% HP and MP </value>
        public const string REVIVEANDLEAVE = "reviveAndLeave";        /// <value> Bring all dead partyMembers back to life at 30% HP and MP and leave </value>
        public const string REVIVEORSTATALL = "reviveOrStatAll";      /// <value> Statall effect applies to anyone alive, revive applies to those who are dead </value>
        public const string NEWINT = "newInt";        /// <value> Add a new interaction to the list of available ones </value>
        public const string SHOP = "shop";            /// <value> Player can purchase items here </value>
        public const string REKINDLE = "rekindle";    /// <value> All active candle uses are restored </value>
        public const string REKINDLEANDLEAVE = "rekindleAndLeave";    /// <value> All active candle uses are restored and leave the event </value>
        public const string STATALLANDITEMANDLEAVE = "statAllAndItemAndLeave";    /// <value> Get some stats changes, and item, and  leave the event </value>
        public const string STATALLANDEVENT = "statAllAndEvent";      /// <value> Get a stat change and move to the next event </value>
        public const string QUEST = "quest";          /// <value> Swaps a specific subEvent with the current event in the subArea, and registers a quest </value>
        public const string QUESTANDITEM = "questAndItem";          /// <value> Swaps a specific subEvent with the current event in the subArea, and registers a quest, and receive an item </value>
        public const string QUESTANDLEAVE = "questAndLeave";          /// <value> Swaps a specific subEvent with the current event in the subArea, and registers a quest, and then leave </value>
        public const string QUESTCONTINUE = "questContinue";          /// <value> Swaps a specific subEvent with the current event in the subArea, continuing a quest </value>
        public const string QUESTCONTINUEANDNEWINT = "questContinueAndNewInt";    /// <value> Swaps a specific subEvent with the current event in the subArea, continuing a quest and adding a new int </value>
        public const string COMBATANDQUESTCONTINUE = "combatAndQuestContinue";    /// <value> Combat, and then swaps a specific subEvent with the current event in the subArea, continuing a quest </value>
        public const string QUESTCOMPLETE = "questComplete";        /// <value> Ends the quest, swapping a specific subEvent the current event in all subAreas, providing stat changes  </value>
        public const string QUESTCOMPLETEANDNEWINT = "questCompleteAndNewInt";  /// <value> Ends the quest, swapping a specific subEvent the current event in the subArea, providing stat changes and a new interaction </value>
        public const string NEWINTANDTUT = "newIntAndTut";    /// <value> Adds a new interaction, and progresses the tutorial, and in one hacky edge case, adds a quest </value>
        public const string SUBEVENTTUT = "subEventTut";      /// <value> Tutorial subevents don't behave normally </value>
        public const string ITEMANDNEWINTTUT = "itemAndNewIntTut";  /// <value> Progresses the tutorial, gives the player items, and a new interaction </value>
        public const string ENDTUT = "endTut";          /// <value> Ends the tutorial </value>
        public const string STORESHOPITEMNEXTEVENT = "storeShopItemNextEvent";  /// <value> Store a random item in the shop for the next run </value>
        public const string PASTITEM = "pastItem";      /// <value> Receieve an item from a past run </value>
        public const string PASTITEMANDSUBEVENT = "pastItemWithSubEvent";       /// <value> Receieve an item from a past run with a subEvent right after </value>
    }
}