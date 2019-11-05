/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 11, 2019
* 
* The GameDB class is used to access data in the game's database (GameDB)
* It is used to fetch information of monsters, party members, items, and more.
*
*/

using Characters;
using Combat;
using Events;
using Items;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Database {

    public class GameDB : SqliteHelper {
    
        const int maxAttacks = 4;               /// <value> Max number of attacks per character </value>
        const string dbName = "GameDB.db";      /// <value> Name of database file to be accessed </value>

        public GameDB() : base(dbName) {}
        
        /// <summary>
        /// Fetches information about a particular monster from the database to initialize a Monster component
        /// </summary>
        /// <param name="nameID"> Name of the monster along with the monster's level </param>
        /// <param name="monster"> Monster game object to be be initialized with fetched values</param>
        public void GetMonsterByNameID(string monsterName, Monster monster) {
            using(dbConnection = base.GetConnection()) {

                dbConnection.Open();

                using (IDbCommand dbcmd = dbConnection.CreateCommand()) { // using will call dispose when done, which calls close
                    dbcmd.CommandText = "SELECT * FROM Monsters WHERE NameID = '" + monsterName + "'";
                    
                    using (IDataReader reader = dbcmd.ExecuteReader()) {
                        string monsterNameID = "";
                        string monsterSpriteName = "";
                        string monsterDisplayName = "";
                        string monsterArea = "";
                        string monsterSize = "";
                        string monsterAI = "";
                        int multiplier = 0;
                        int HP = 0;
                        int MP = 0;
                        int dropChance = 0;
                        int[] stats = {};
                        Attack[] attacks = new Attack[4];
                        Result monsterReward = null;

                        if (reader.Read()) {
                            monsterNameID = reader.GetString(1);
                            monsterSpriteName = reader.GetString(2);
                            monsterDisplayName = reader.GetString(3);
                            monsterArea = reader.GetString(4);
                            monsterSize = reader.GetString(5);
                            monsterAI = reader.GetString(6);
                            multiplier = reader.GetInt32(7);
                            HP = reader.GetInt32(8); 
                            MP = reader.GetInt32(9);
                            stats = new int[] { reader.GetInt32(10), reader.GetInt32(11), reader.GetInt32(12), reader.GetInt32(13) };

                            for (int i = 0; i < maxAttacks; i++) {
                                string attackName = reader.GetString(14 + i);
                                attacks[i] = GetAttack(attackName, true, dbConnection);
                            }
                            dropChance = reader.GetInt32(18);
                            monsterReward = GetResultByName(reader.GetString(19), "", dbConnection);                     
                        }
                        else {
                            Debug.LogError("Monster " + monsterName + " does not exist in the DB");
                        }

                        monster.StartCoroutine(monster.Init(monsterNameID, monsterSpriteName, monsterDisplayName, monsterArea, 
                        monsterSize, monsterAI, multiplier, HP, MP, stats, attacks, dropChance, monsterReward)); 
                    }
                }
            }          
        }

        /// <summary>
        /// Initializes a partyMember game object
        /// </summary>
        /// <param name="className"> Class of the partyMember </param>
        /// <param name="pm"> PartyMember game object to be initialized with fetched values</param>
        public void GetPartyMemberByClass(string className, PartyMember pm) {
            using(dbConnection = base.GetConnection()) {

                dbConnection.Open();

                using (IDbCommand dbcmd = dbConnection.CreateCommand()) { // using will call dispose when done, which calls close
                    dbcmd.CommandText = "SELECT * FROM PartyMembers WHERE Class = '" + className + "'";
                    
                    using (IDataReader reader = dbcmd.ExecuteReader()) {
                        string[] personalInfo = new string[4];
                        int LVL = 0;
                        int EXP = 0;
                        int CHP = 0;    // storing these for the future; the player leaving mid dungeon will need these values saved
                        int CMP = 0;
                        int[] stats = {};
                        Attack[] attacks = new Attack[4];

                        if (reader.Read()) {
                            personalInfo[0] = reader.GetString(1);
                            personalInfo[1] = reader.GetString(2);
                            personalInfo[2] = reader.GetString(3);
                            personalInfo[3] = reader.GetString(4);
                            
                            LVL = reader.GetInt32(5);
                            EXP = reader.GetInt32(6);
                            CHP = reader.GetInt32(7); 
                            CMP = reader.GetInt32(8);
                            stats = new int[] { reader.GetInt32(9), reader.GetInt32(10), reader.GetInt32(11), reader.GetInt32(12) };

                            for (int i = 0; i < maxAttacks; i++) {
                                string attackName = reader.GetString(13 + i);
                                attacks[i] = GetAttack(attackName, false, dbConnection);
                            }
                        }
                        else {
                            Debug.LogError("PartyMember with className " + className + " does not exist in the DB");
                        }
                        // need to figure out how to attach this information to a monster gameObject, can't use new
                        pm.Init(personalInfo, LVL, EXP, CHP, CMP, stats, attacks); 
                    }
                }
            }          
        }

        /// <summary>
        /// Returns an attack from the Attacks table
        /// </summary>
        /// <param name="name"> Name of the attack</param>
        /// <param name="dbConnection"> IDbConnectino to get attack with </param>
        /// <returns> Returns an Attack with the information initialized </returns>
        public Attack GetAttack(string name, bool isMonster, IDbConnection dbConnection) {
            using (IDbCommand dbcmd = dbConnection.CreateCommand()) {
                dbcmd.CommandText = "SELECT * FROM Attacks WHERE Name = '" + name + "'";

                using (IDataReader reader = dbcmd.ExecuteReader()) {
                    Attack newAttack = null;

                    string type = "";
                    string formula = "";
                    string costType = "";
                    string scope = "";
                    string animationClipName = "";
                    string seName = "";
                    int seChance = 0;
                    int seDuration = 0;
                    int cost = 0;
                    

                    if (reader.Read()) {
                        type = reader.GetString(2);
                        formula = reader.GetString(3);
                        seName = reader.GetString(4);
                        seDuration = reader.GetInt32(5);
                        seChance = reader.GetInt32(6);
                        costType = reader.GetString(7);
                        cost = reader.GetInt32(8);
                        scope = reader.GetString(9);
                        if (isMonster) {
                            animationClipName = reader.GetString(10);
                        }
                        else {
                            animationClipName = reader.GetString(11);
                        }

                        newAttack = new Attack(name, type, formula, seName, seDuration ,seChance, costType, cost, scope, animationClipName);
                    }
                    
                    if (newAttack == null) {
                        Debug.LogError("Attack " + name + " does not exist in the DB");
                    }

                    return newAttack;
                }
            }
        }

        /// <summary>
        /// Returns an Area from the Areas table
        /// </summary>
        /// <param name="areaName"> Name of the area </param>
        /// <returns> An Area, passing it the dbconnection for it to fetch all information it needs </returns>
        public Area GetAreaByName(string areaName) {
            using(dbConnection = base.GetConnection()) {

                dbConnection.Open();

                using (IDbCommand dbcmd = dbConnection.CreateCommand()) {
                    dbcmd.CommandText = "SELECT * FROM Areas WHERE Name = '" + areaName + "'";

                    using (IDataReader reader = dbcmd.ExecuteReader()) {
                        Area newArea = null;
                        string[] subAreaNames = new string[10];
                        string themeColour = "";

                        if (reader.Read()) {
                            subAreaNames[0] = reader.GetString(2);
                            subAreaNames[1] = reader.GetString(3);
                            subAreaNames[2] = reader.GetString(4);
                            subAreaNames[3] = reader.GetString(5);
                            subAreaNames[4] = reader.GetString(6);
                            subAreaNames[5] = reader.GetString(7);
                            subAreaNames[6] = reader.GetString(8);
                            subAreaNames[7] = reader.GetString(9);
                            subAreaNames[8] = reader.GetString(10);
                            subAreaNames[9] = reader.GetString(11);
                            
                            themeColour = reader.GetString(12);

                            newArea = new Area(areaName, subAreaNames, themeColour, dbConnection);
                        }
                        else {
                            Debug.LogError("SubArea " + areaName + " does not exist in the DB");
                        }

                        return newArea;
                    }
                }
            }
        }

        /// <summary>
        /// Returns a SubArea from the SubAreas table
        /// </summary>
        /// <param name="subAreaName"> Name of sub area </param>
        /// <param name="areaName"> Name of area subArea occurs in </param>
        /// <param name="dbConnection"> Reuse dbConnection to save memory </param>
        /// <returns> A SubArea, passing it the dbconnection for it to fetch all information it needs </returns>
        public SubArea GetSubAreaByAreaName(string subAreaName, string areaName, IDbConnection dbConnection) {
            using (IDbCommand dbcmd = dbConnection.CreateCommand()) {
                dbcmd.CommandText = "SELECT * FROM SubAreas WHERE Name = '" + subAreaName + "'";

                using (IDataReader reader = dbcmd.ExecuteReader()) {
                    SubArea newSubArea = null;
                    string name = "";
                    int minMonsterNum = 0;
                    int maxMonsterNum = 0;
                    string defaultBGPackName = "";
                    string[] subAreaEvents = new string[10];
                    int[] subAreaEventChances = new int[10];
                    string[] monsterPool;
                    
                    if (reader.Read()) {
                        subAreaEvents[0] = reader.GetString(2);
                        subAreaEvents[1] = reader.GetString(4);
                        subAreaEvents[2] = reader.GetString(6);
                        subAreaEvents[3] = reader.GetString(8);
                        subAreaEvents[4] = reader.GetString(10);
                        subAreaEvents[5] = reader.GetString(12);
                        subAreaEvents[6] = reader.GetString(14);
                        subAreaEvents[7] = reader.GetString(16);
                        subAreaEvents[8] = reader.GetString(18);
                        subAreaEvents[9] = reader.GetString(20);

                        subAreaEventChances[0] = reader.GetInt32(3);
                        subAreaEventChances[1] = reader.GetInt32(5);
                        subAreaEventChances[2] = reader.GetInt32(7);
                        subAreaEventChances[3] = reader.GetInt32(9);
                        subAreaEventChances[4] = reader.GetInt32(11);
                        subAreaEventChances[5] = reader.GetInt32(13);
                        subAreaEventChances[6] = reader.GetInt32(15);
                        subAreaEventChances[7] = reader.GetInt32(17);
                        subAreaEventChances[8] = reader.GetInt32(19);
                        subAreaEventChances[9] = reader.GetInt32(21);

                        name = reader.GetString(1);
                        monsterPool = GetMonsterPool(reader.GetString(22), dbConnection);
                        minMonsterNum = reader.GetInt32(23);
                        maxMonsterNum = reader.GetInt32(24);
                        defaultBGPackName = reader.GetString(25);

                        newSubArea = new SubArea(name, areaName, subAreaEvents, subAreaEventChances, monsterPool, minMonsterNum, maxMonsterNum, 
                        defaultBGPackName, dbConnection);
                    }
                    else {
                        Debug.LogError("SubArea " + subAreaName + " does not exist in the DB");
                    }

                    return newSubArea;
                }
            }
        }

        /// <summary>
        /// Returns the name of a monster from the MonsterPools table
        /// </summary>
        /// <param name="name"> Name of the attack</param>
        /// <param name="dbConnection"> IDbConnectino to get attack with </param>
        /// <returns> Returns an Attack with the information initialized </returns>
        public string[] GetMonsterPool(string name, IDbConnection dbConnection) {
            using (IDbCommand dbcmd = dbConnection.CreateCommand()) {
                dbcmd.CommandText = "SELECT * FROM MonsterPools WHERE Name = '" + name + "'";

                using (IDataReader reader = dbcmd.ExecuteReader()) {
                    string[] monsterPool = new string[10];

                    if (reader.Read()) {
                        monsterPool[0] = reader.GetString(2);
                        monsterPool[1] = reader.GetString(3);
                        monsterPool[2] = reader.GetString(4);
                        monsterPool[3] = reader.GetString(5);
                        monsterPool[4] = reader.GetString(6);
                        monsterPool[5] = reader.GetString(7);
                        monsterPool[6] = reader.GetString(8);
                        monsterPool[7] = reader.GetString(9);
                        monsterPool[8] = reader.GetString(10);
                        monsterPool[9] = reader.GetString(11);
                    }
                    else {
                        Debug.LogError("MonsterPool " + name + " does not exist in the DB");
                    }

                    return monsterPool;
                }
            }
        }

        /// <summary>
        /// Returns an Event from the Events table
        /// </summary>
        /// <param name="eventName"> Name of event </param>
        /// <param name="areaName"> Name of area event occurs in </param>
        /// <param name="eventChance"> Chance of event occuring </param>
        /// <param name="dbConnection"> Reuse dbConnection to save memory </param>
        /// <returns> An Event, passing it the dbconnection for it to fetch all information it needs </returns>
        public Events.Event GetEventByName(string eventName, string areaName, int eventChance, IDbConnection dbConnection) {
            using (IDbCommand dbcmd = dbConnection.CreateCommand()) {
                dbcmd.CommandText = "SELECT * FROM Events WHERE Name = '" + eventName + "'";

                using (IDataReader reader = dbcmd.ExecuteReader()) {
                    Events.Event newEvent = null;
                    string[] eventInteractions = new string[5];
                    string[] eventSprites = new string[3];
                    string type = "";
                    string bgPackName = "";
                    int[] possibleBackgrounds = new int[4];
                    int progressAmount = 0;
                    int specificBGSprite = -1;
                    bool isLeavePossible = false;

                    if (reader.Read()) {
                        type = reader.GetString(3);
                        progressAmount = reader.GetInt32(4);
                        eventInteractions[0] = reader.GetString(5);
                        eventInteractions[1] = reader.GetString(6);
                        eventInteractions[2] = reader.GetString(7);
                        eventInteractions[3] = reader.GetString(8);
                        eventInteractions[4] = reader.GetString(9);
                        isLeavePossible = reader.GetBoolean(10);
                        bgPackName = reader.GetString(11);
                        specificBGSprite = reader.GetInt32(12);
                        eventSprites[0] = reader.GetString(13);
                        eventSprites[1] = reader.GetString(14);
                        eventSprites[2] = reader.GetString(15);

                        newEvent = new Events.Event(eventName, areaName, type, eventChance, progressAmount,
                        eventInteractions, isLeavePossible, bgPackName, specificBGSprite, eventSprites, dbConnection);
                    }
                    else {
                         Debug.LogError("Event " + eventName + " does not exist in the DB");
                    }

                    return newEvent;
                } 
            }
        }

        /// <summary>
        /// Returns an Interaction from the Interactions table
        /// </summary>
        /// <param name="intName"> Name of interaction </param>
        /// <param name="dbConnection"> Reuse dbConnection to save memory </param>
        /// <returns> An Interaction, passing it the dbconnection for it to fetch all information it needs </returns>
        public Interaction GetInteractionByName(string intName,  IDbConnection dbConnection) {
            using (IDbCommand dbcmd = dbConnection.CreateCommand()) {
                dbcmd.CommandText = "SELECT * FROM Interactions WHERE Name = '" + intName + "'";

                using (IDataReader reader = dbcmd.ExecuteReader()) {
                    Interaction newInt = null;
                    string[] intResults = new string[4];
                    string[] intResultKeys = new string[4];
                    string[] intSprites = new String[4];
                    string resultName = "";
                    bool isSingleUse = false;

                    if (reader.Read()) {
                        resultName = reader.GetString(1);
                        intResults[0] = reader.GetString(2);
                        intResults[1] = reader.GetString(4);
                        intResults[2] = reader.GetString(6);
                        intResults[3] = reader.GetString(8);

                        intSprites[0] = reader.GetString(3);
                        intSprites[1] = reader.GetString(5);
                        intSprites[2] = reader.GetString(7);
                        intSprites[3] = reader.GetString(9);

                        isSingleUse = reader.GetBoolean(10);

                        newInt = new Interaction(resultName, intResults, intSprites, isSingleUse, dbConnection);
                    }
                    else {
                         Debug.LogError("Interaction " + intName + " does not exist in the DB");
                    }
                
                    return newInt;
                }
            }
        }

        /// <summary>
        /// Returns an Interaction from the Interactions table. Overloaded to use its own dbconnection.
        /// </summary>
        /// <param name="intName"> Name of interaction </param>
        /// <returns> An Interaction, passing it the dbconnection for it to fetch all information it needs </returns>
        public Interaction GetInteractionByName(string intName) {
            using(dbConnection = base.GetConnection()) {
                dbConnection.Open();
                return GetInteractionByName(intName, dbConnection);
            }
        }

        /// <summary>
        /// Returns a Result from the Results Table
        /// </summary>
        /// <param name="resultName"> Name of the result </param>
        /// <param name="resultKey"> Key for string to be displayed when result is shown to player </param>
        /// <param name="dbConnection"> Reuse dbConnection to save memory </param>
        /// <returns> A result </returns>
        public Result GetResultByName(string resultName, string resultKey, IDbConnection dbConnection) {
            using (IDbCommand dbcmd = dbConnection.CreateCommand()) {
                dbcmd.CommandText = "SELECT * FROM Results WHERE Name = '" + resultName + "'";

                using (IDataReader reader = dbcmd.ExecuteReader()) {
                    Result newResult = null;
                    string name = "";
                    string type = "";
                    string quantity = "";
                    string scope = "";
                    string subAreaName = "";
                    string subEventName = "";
                    string itemType = "";
                    string[] specificItemNames = new string[3];
                    string itemQuality = "";
                    string[] specificMonsterNames = new string[5];
                    int[] resultChanges = new int[4];
                    int monsterCount = 0;
                    bool isUnique = false;

                    if (reader.Read()) {
                        name = reader.GetString(1);
                        type = reader.GetString(2);
                        isUnique = reader.GetBoolean(3);
                        quantity = reader.GetString(4);
                        scope = reader.GetString(5);
                        resultChanges[0] = reader.GetInt32(6);
                        resultChanges[1] = reader.GetInt32(7);
                        resultChanges[2] = reader.GetInt32(8);
                        resultChanges[3] = reader.GetInt32(9);
                        subAreaName = reader.GetString(10);
                        subEventName = reader.GetString(11);
                        monsterCount = reader.GetInt32(12);
                        specificMonsterNames[0] = reader.GetString(13);
                        specificMonsterNames[1] = reader.GetString(14);
                        specificMonsterNames[2] = reader.GetString(15);
                        specificMonsterNames[3] = reader.GetString(16);
                        specificMonsterNames[4] = reader.GetString(17);
                        itemType = reader.GetString(18);
                        specificItemNames[0] = reader.GetString(19);
                        specificItemNames[1] = reader.GetString(20);
                        specificItemNames[2] = reader.GetString(21);
                        itemQuality = reader.GetString(22);

                        newResult = new Result(name, resultKey, type, isUnique, quantity, scope, resultChanges, subAreaName, subEventName,
                        monsterCount, specificMonsterNames, itemType, specificItemNames, itemQuality);
                    }
                    else {
                         Debug.LogError("Result " + resultName + " does not exist in the DB");
                    }

                    return newResult;
                }
            }
        }

        /// <summary>
        /// Returns the names of all of the background packs for an area
        /// </summary>
        /// <param name="areaName"></param>
        /// <returns></returns>
        public string[] GetBGPackNames(string areaName) {
            using(dbConnection = base.GetConnection()) {

                dbConnection.Open();

                using (IDbCommand dbcmd = dbConnection.CreateCommand()) {

                    dbcmd.CommandText = "SELECT * FROM BackgroundPackNames WHERE Name = '" + areaName + "'";

                    using (IDataReader reader = dbcmd.ExecuteReader()) {
                        string[] bgPackNames = new string[10];

                        if (reader.Read()) {
                            bgPackNames[0] = reader.GetString(2);
                            bgPackNames[1] = reader.GetString(3);
                            bgPackNames[2] = reader.GetString(4);
                            bgPackNames[3] = reader.GetString(5);
                            bgPackNames[4] = reader.GetString(6);
                            bgPackNames[5] = reader.GetString(7);
                            bgPackNames[6] = reader.GetString(8);
                            bgPackNames[7] = reader.GetString(9);
                            bgPackNames[8] = reader.GetString(10);
                            bgPackNames[9] = reader.GetString(11);
                        }
                        else {
                            Debug.LogError("BackgroundPackNames " + areaName + " does not exist in the DB");
                        }

                        return bgPackNames;
                    }
                }
            }
        }

        /// <summary>
        /// Returns a background pack for an area
        /// </summary>
        /// <param name="areaName"></param>
        /// <param name="bgPackName"></param>
        /// <returns></returns>
        public BackgroundPack GetBGPack(string areaName, string bgPackName) {
            using(dbConnection = base.GetConnection()) {

                dbConnection.Open();

                using (IDbCommand dbcmd = dbConnection.CreateCommand()) {

                    dbcmd.CommandText = "SELECT * FROM BackgroundPacks WHERE Area = '" + areaName + "' AND Name = '" + bgPackName + "'";

                    using (IDataReader reader = dbcmd.ExecuteReader()) {
                        BackgroundPack newPack = null;
                        int[] spriteIndicies = new int[10];

                        if (reader.Read()) {
                            spriteIndicies[0] = reader.GetInt32(3);
                            spriteIndicies[1] = reader.GetInt32(4);
                            spriteIndicies[2] = reader.GetInt32(5);
                            spriteIndicies[3] = reader.GetInt32(6);
                            spriteIndicies[4] = reader.GetInt32(7);
                            spriteIndicies[5] = reader.GetInt32(8);
                            spriteIndicies[6] = reader.GetInt32(9);
                            spriteIndicies[7] = reader.GetInt32(10);
                            spriteIndicies[8] = reader.GetInt32(11);
                            spriteIndicies[9] = reader.GetInt32(12);

                            newPack = new BackgroundPack(reader.GetString(2), areaName, spriteIndicies);
                        }
                        else {
                            Debug.LogError("BackgroundPack " + bgPackName + " with " + areaName + " does not exist in the DB");
                        }

                        return newPack;
                    }
                }
            }
        }

        /// <summary>
        /// Returns all of the consumables that can be found in a subArea
        /// </summary>
        /// <param name="subAreaName"></param>
        /// <returns></returns>
        public Consumable[] GetConsumablesBySubArea (string subAreaName) {
            using(dbConnection = base.GetConnection()) {

                dbConnection.Open();

                using (IDbCommand dbcmd = dbConnection.CreateCommand()) {

                    dbcmd.CommandText = "SELECT * FROM ConsumablesItemPools WHERE name = '" + subAreaName + "'";

                    using (IDataReader reader = dbcmd.ExecuteReader()) {
                        string[] consumableNameIDs = new string[10];
                        Consumable[] consumableItems = new Consumable[10];

                        if (reader.Read()) {
                            consumableNameIDs[0] = reader.GetString(2);
                            consumableNameIDs[1] = reader.GetString(3);
                            consumableNameIDs[2] = reader.GetString(4);
                            consumableNameIDs[3] = reader.GetString(5);
                            consumableNameIDs[4] = reader.GetString(6);
                            consumableNameIDs[5] = reader.GetString(7);
                            consumableNameIDs[6] = reader.GetString(8);
                            consumableNameIDs[7] = reader.GetString(9);
                            consumableNameIDs[8] = reader.GetString(10);
                            consumableNameIDs[9] = reader.GetString(11);

                            for (int i = 0; i < consumableNameIDs.Length; i++) {
                                consumableItems[i] = (Consumable)GetItemByNameID(consumableNameIDs[i], "consumable", dbConnection);
                            }
                        }
                        else {
                            Debug.LogError("ConsumableItemPool " + subAreaName + " does not exist in the DB");
                        }

                        return consumableItems;
                    }
                }
            }
        }
        
        /// <summary>
        /// Returns all of the gear that can be found in a subArea
        /// </summary>
        /// <param name="subAreaName"></param>
        /// <returns></returns>
        public Gear[] GetGearBySubArea (string subAreaName) {
            using(dbConnection = base.GetConnection()) {

                dbConnection.Open();

                using (IDbCommand dbcmd = dbConnection.CreateCommand()) {

                    dbcmd.CommandText = "SELECT * FROM GearItemPools WHERE name = '" + subAreaName + "'";

                    using (IDataReader reader = dbcmd.ExecuteReader()) {
                        string[] gearNameIDs = new string[10];
                        Gear[] gearItems = new Gear[10];

                        if (reader.Read()) {
                            gearNameIDs[0] = reader.GetString(2);
                            gearNameIDs[1] = reader.GetString(3);
                            gearNameIDs[2] = reader.GetString(4);
                            gearNameIDs[3] = reader.GetString(5);
                            gearNameIDs[4] = reader.GetString(6);
                            gearNameIDs[5] = reader.GetString(7);
                            gearNameIDs[6] = reader.GetString(8);
                            gearNameIDs[7] = reader.GetString(9);
                            gearNameIDs[8] = reader.GetString(10);
                            gearNameIDs[9] = reader.GetString(11);

                            for (int i = 0; i < gearNameIDs.Length; i++) {
                                gearItems[i] = (Gear)GetItemByNameID(gearNameIDs[i], "gear", dbConnection);
                            }
                        }
                        else {
                            Debug.LogError("ConsumableItemPool " + subAreaName + " does not exist in the DB");
                        }

                        return gearItems;
                    }
                }
            }
        }

        /// <summary>
        /// Returns an item from either the gear table or consumables table
        /// </summary>
        /// <param name="name"> Name of the item </param>
        /// <param name="type"> Type of the item (consumable or gear) </param>
        /// <param name="dbConnection"> IDbConnectino to get attack with </param>
        /// <returns> Returns an Attack with the information initialized </returns>
        public Item GetItemByNameID(string nameID, string type, IDbConnection dbConnection) {
            using (IDbCommand dbcmd = dbConnection.CreateCommand()) {

                if (type == "consumable") {
                     dbcmd.CommandText = "SELECT * FROM Consumables WHERE NameID = '" + nameID + "'";
                }
                else if (type == "gear") {
                    dbcmd.CommandText = "SELECT * FROM Gear WHERE NameID = '" + nameID + "'";
                }

                using (IDataReader reader = dbcmd.ExecuteReader()) {
                    Item newItem = null;

                    string subType = "";
                    string className = "";
                    string[] effects = new string[3];
                    int[] values = new int[3];

                    if (reader.Read()) {
                        if (type == "consumable") {
                            subType = reader.GetString(2);

                            effects[0] = reader.GetString(3);
                            effects[1] = reader.GetString(5);
                            effects[2] = reader.GetString(7);

                            values[0] = reader.GetInt32(4);
                            values[1] = reader.GetInt32(6);
                            values[2] = reader.GetInt32(8);

                             newItem = new Consumable(nameID, type, subType, effects, values);
                        }
                        else if (type == "gear") {
                            subType = reader.GetString(2);
                            className = reader.GetString(3);

                            effects[0] = reader.GetString(4);
                            effects[1] = reader.GetString(6);
                            effects[2] = reader.GetString(8);

                            values[0] = reader.GetInt32(5);
                            values[1] = reader.GetInt32(7);
                            values[2] = reader.GetInt32(9);
   
                            newItem = new Gear(nameID, type, subType, className, effects, values);
                        }
                    }
                    
                    if (newItem == null) {
                        Debug.LogError("Item " + nameID + " does not exist in the DB");
                    }

                    return newItem;
                }
            }
        }
    }
}
