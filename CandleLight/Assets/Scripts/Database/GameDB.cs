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
                        int LVL = 0;
                        int multiplier = 0;
                        int HP = 0;
                        int MP = 0;
                        int[] stats = {};
                        Attack[] attacks = new Attack[4];

                        if (reader.Read()) {
                            monsterNameID = reader.GetString(1);
                            monsterSpriteName = reader.GetString(2);
                            monsterDisplayName = reader.GetString(3);
                            monsterArea = reader.GetString(4);
                            monsterSize = reader.GetString(5);
                            monsterAI = reader.GetString(6);
                            LVL = reader.GetInt32(7);
                            multiplier = reader.GetInt32(8);
                            HP = reader.GetInt32(9); 
                            MP = reader.GetInt32(10);
                            stats = new int[] { reader.GetInt32(11), reader.GetInt32(12), reader.GetInt32(13), reader.GetInt32(14) };

                            for (int i = 0; i < maxAttacks; i++) {
                                string attackName = reader.GetString(15 + i);
                                attacks[i] = GetAttack(attackName, true, dbConnection);
                            }
                        }
                        else {
                            Debug.LogError("Monster " + monsterName + " does not exist in the DB");
                        }
                        monster.StartCoroutine(monster.Init(monsterNameID, monsterSpriteName, monsterDisplayName, monsterArea, 
                        monsterSize, monsterAI, LVL, multiplier, HP, MP, stats, attacks)); 
                    }
                }
            }          
        }

        /// <summary>
        /// Initializes a party member game object
        /// </summary>
        /// <param name="className"> Class of the party member </param>
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
                        int HP = 0;
                        int MP = 0;
                        int[] stats = {};
                        Attack[] attacks = new Attack[4];

                        if (reader.Read()) {
                            personalInfo[0] = reader.GetString(1);
                            personalInfo[1] = reader.GetString(2);
                            personalInfo[2] = reader.GetString(3);
                            personalInfo[3] = reader.GetString(4);
                            
                            LVL = reader.GetInt32(5);
                            EXP = reader.GetInt32(6);
                            HP = reader.GetInt32(7); 
                            MP = reader.GetInt32(8);
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
                        pm.Init(personalInfo, LVL, EXP, HP, MP, stats, attacks); 
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

                    if (reader.Read()) {
                        if (isMonster) {
                            newAttack = new Attack(name, reader.GetInt32(2), reader.GetString(3), reader.GetInt32(4), reader.GetString(5), reader.GetString(6));
                        }
                        else {
                            newAttack = new Attack(name, reader.GetInt32(2), reader.GetString(3), reader.GetInt32(4), reader.GetString(5), reader.GetString(7));
                        }
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
        /// <param name="dbConnection"> Reuse dbConnection to save memory </param>
        /// <returns> A SubArea, passing it the dbconnection for it to fetch all information it needs </returns>
        public SubArea GetSubAreaByAreaName(string subAreaName, IDbConnection dbConnection) {
            using (IDbCommand dbcmd = dbConnection.CreateCommand()) {
                dbcmd.CommandText = "SELECT * FROM SubAreas WHERE Name = '" + subAreaName + "'";

                using (IDataReader reader = dbcmd.ExecuteReader()) {
                    SubArea newSubArea = null;
                    string name = "";
                    int minMonsterNum = 0;
                    int maxMonsterNum = 0;
                    string defaultBGPackName = "";
                    string[] subAreaEvents = new string[10];
                    string[] monsterPool;
                    
                    if (reader.Read()) {
                        subAreaEvents[0] = reader.GetString(2);
                        subAreaEvents[1] = reader.GetString(3);
                        subAreaEvents[2] = reader.GetString(4);
                        subAreaEvents[3] = reader.GetString(5);
                        subAreaEvents[4] = reader.GetString(6);
                        subAreaEvents[5] = reader.GetString(7);
                        subAreaEvents[6] = reader.GetString(8);
                        subAreaEvents[7] = reader.GetString(9);
                        subAreaEvents[8] = reader.GetString(10);
                        subAreaEvents[9] = reader.GetString(11);

                        name = reader.GetString(1);
                        monsterPool = GetMonsterPool(reader.GetString(12), dbConnection);
                        minMonsterNum = reader.GetInt32(13);
                        maxMonsterNum = reader.GetInt32(14);
                        defaultBGPackName = reader.GetString(15);

                        newSubArea = new SubArea(name, subAreaEvents, monsterPool, minMonsterNum, maxMonsterNum, 
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
        /// <param name="dbConnection"> Reuse dbConnection to save memory </param>
        /// <returns> An Event, passing it the dbconnection for it to fetch all information it needs </returns>
        public Events.Event GetEventByName(string eventName, IDbConnection dbConnection) {
            using (IDbCommand dbcmd = dbConnection.CreateCommand()) {
                dbcmd.CommandText = "SELECT * FROM Events WHERE Name = '" + eventName + "'";

                using (IDataReader reader = dbcmd.ExecuteReader()) {
                    Events.Event newEvent = null;
                    string[] eventInteractions = new string[5];
                    string[] eventSprites = new string[3];
                    int[] possibleBackgrounds = new int[4];

                    if (reader.Read()) {
                        eventInteractions[0] = reader.GetString(6);
                        eventInteractions[1] = reader.GetString(7);
                        eventInteractions[2] = reader.GetString(8);
                        eventInteractions[3] = reader.GetString(9);
                        eventInteractions[4] = reader.GetString(10);

                        eventSprites[0] = reader.GetString(14);
                        eventSprites[1] = reader.GetString(15);
                        eventSprites[2] = reader.GetString(16);

                        newEvent = new Events.Event(reader.GetString(1), reader.GetString(2), reader.GetInt32(3), reader.GetInt32(4), reader.GetString(5),
                        eventInteractions, reader.GetBoolean(11), reader.GetString(12), reader.GetInt32(13), eventSprites, dbConnection);
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

                    if (reader.Read()) {
                        intResults[0] = reader.GetString(2);
                        intResults[1] = reader.GetString(5);
                        intResults[2] = reader.GetString(8);
                        intResults[3] = reader.GetString(11);

                        intResultKeys[0] = reader.GetString(3);
                        intResultKeys[1] = reader.GetString(6);
                        intResultKeys[2] = reader.GetString(9);
                        intResultKeys[3] = reader.GetString(12);

                        intSprites[0] = reader.GetString(4);
                        intSprites[1] = reader.GetString(7);
                        intSprites[2] = reader.GetString(10);
                        intSprites[3] = reader.GetString(13);

                        newInt = new Interaction(reader.GetString(1), intResults, intResultKeys, intSprites, dbConnection);
                    }
                    else {
                         Debug.LogError("Interaction " + intName + " does not exist in the DB");
                    }
                
                    return newInt;
                }
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
                    int[] resultChanges = new int[4];

                    if (reader.Read()) {
                        resultChanges[0] = reader.GetInt32(4);
                        resultChanges[1] = reader.GetInt32(5);
                        resultChanges[2] = reader.GetInt32(6);
                        resultChanges[3] = reader.GetInt32(7);

                        newResult = new Result(reader.GetString(1), reader.GetBoolean(2), resultKey, reader.GetString(3), resultChanges, 
                        reader.GetString(8), reader.GetString(9));
                    }
                    else {
                         Debug.LogError("Result " + resultName + " does not exist in the DB");
                    }

                    return newResult;
                }
            }
        }

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
    }
}
