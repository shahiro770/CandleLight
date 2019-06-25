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
        public void GetMonsterByNameID(string nameID, Monster monster) {
            using(dbConnection = base.GetConnection()) {

                dbConnection.Open();

                using (IDbCommand dbcmd = dbConnection.CreateCommand()) { // using will call dispose when done, which calls close
                    dbcmd.CommandText = "SELECT * FROM Monsters WHERE NameID = '" + nameID + "'";
                    
                    using (IDataReader reader = dbcmd.ExecuteReader()) {
                        string monsterNameID = "";
                        string monsterSpriteName = "";
                        string monsterDisplayName = "";
                        string monsterArea = "";
                        string monsterSize = "";
                        string monsterAI = "";
                        int LVL = 0;
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
                            HP = reader.GetInt32(8); 
                            MP = reader.GetInt32(9);
                            stats = new int[] { reader.GetInt32(10), reader.GetInt32(11), reader.GetInt32(12), reader.GetInt32(13) };

                            for (int i = 0; i < maxAttacks; i++) {
                                string attackName = reader.GetString(14 + i);
                                attacks[i] = GetAttack(attackName, true, dbConnection);
                            }

                        }
                        monster.StartCoroutine(monster.Init(monsterNameID, monsterSpriteName, monsterDisplayName, monsterArea, monsterSize, monsterAI, LVL, HP, MP, stats, attacks)); 
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
                            HP = reader.GetInt32(6); 
                            MP = reader.GetInt32(7);
                            stats = new int[] { reader.GetInt32(8), reader.GetInt32(9), reader.GetInt32(10), reader.GetInt32(11) };

                            for (int i = 0; i < maxAttacks; i++) {
                                string attackName = reader.GetString(12 + i);
                                attacks[i] = GetAttack(attackName, false, dbConnection);
                            }
                        }
                        // need to figure out how to attach this information to a monster gameObject, can't use new
                        pm.Init(personalInfo, LVL, HP, MP, stats, attacks); 
                    }
                }
            }          
        }

        /// <summary>
        /// Returns an attack from the attack table
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
                            newAttack = new Attack(name, reader.GetInt32(2), reader.GetString(3), reader.GetInt32(4), reader.GetString(5));
                        }
                        else {
                            newAttack = new Attack(name, reader.GetInt32(2), reader.GetString(3), reader.GetInt32(4), reader.GetString(6));
                        }
                    }
                    
                    if (newAttack == null) {
                        Debug.LogError("The name of the searched attack doesn't exist");
                    }

                    return newAttack;
                }
            }
        }

        public Area GetAreaByName(string areaName) {
             using(dbConnection = base.GetConnection()) {

                dbConnection.Open();

                using (IDbCommand dbcmd = dbConnection.CreateCommand()) {
                    dbcmd.CommandText = "SELECT * FROM Areas WHERE Name = '" + areaName + "'";

                    using (IDataReader reader = dbcmd.ExecuteReader()) {
                        Area newArea = null;
                        string[] subAreaNames = new string[10];

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

                            newArea = new Area(areaName, subAreaNames, dbConnection);
                        }

                        return newArea;
                    }
                }
            }
        }

        public SubArea GetSubAreaByAreaName(string subAreaName, IDbConnection dbConnection) {
            using (IDbCommand dbcmd = dbConnection.CreateCommand()) {
                dbcmd.CommandText = "SELECT * FROM SubAreas WHERE Name = '" + subAreaName + "'";

                using (IDataReader reader = dbcmd.ExecuteReader()) {
                    SubArea newSubArea = null;
                    string[] subAreaEvents = new string[5];

                    if (reader.Read()) {
                        subAreaEvents[0] = reader.GetString(2);
                        subAreaEvents[1] = reader.GetString(3);
                        subAreaEvents[2] = reader.GetString(4);
                        subAreaEvents[3] = reader.GetString(5);
                        subAreaEvents[4] = reader.GetString(6);

                        newSubArea = new SubArea(reader.GetString(1), subAreaEvents, dbConnection);
                    }

                    return newSubArea;
                }
            }
        }

        public Events.Event GetEventByName(string eventName, IDbConnection dbConnection) {
            using (IDbCommand dbcmd = dbConnection.CreateCommand()) {
                dbcmd.CommandText = "SELECT * FROM Events WHERE Name = '" + eventName + "'";

                using (IDataReader reader = dbcmd.ExecuteReader()) {
                    Events.Event newEvent = null;
                    string[] eventInteractions = new string[5];
                    string[] eventSprites = new string[3];
                    int[] possibleBackgrounds = new int[4];

                    if (reader.Read()) {
                        eventInteractions[0] = reader.GetString(4);
                        eventInteractions[1] = reader.GetString(5);
                        eventInteractions[2] = reader.GetString(6);
                        eventInteractions[3] = reader.GetString(7);
                        eventInteractions[4] = reader.GetString(8);

                        possibleBackgrounds[0] = reader.GetInt32(10);
                        possibleBackgrounds[1] = reader.GetInt32(11);
                        possibleBackgrounds[2] = reader.GetInt32(12);
                        possibleBackgrounds[3] = reader.GetInt32(13);

                        eventSprites[0] = reader.GetString(14);
                        eventSprites[1] = reader.GetString(15);
                        eventSprites[2] = reader.GetString(16);

                        newEvent = new Events.Event(reader.GetString(1), reader.GetString(2), reader.GetString(3),
                        eventInteractions, reader.GetBoolean(9), possibleBackgrounds, eventSprites, dbConnection);
                    }

                    return newEvent;
                }
            }
        }

        public Interaction GetInteractionByName(string intName, IDbConnection dbConnection) {
            using (IDbCommand dbcmd = dbConnection.CreateCommand()) {
                dbcmd.CommandText = "SELECT * FROM Interactions WHERE Name = '" + intName + "'";

                using (IDataReader reader = dbcmd.ExecuteReader()) {
                    Interaction newInt = null;
                    string[] intResults = new string[4];
                    string[] intResultKeys = new string[4];

                    if (reader.Read()) {
                        intResults[0] = reader.GetString(2);
                        intResults[1] = reader.GetString(4);
                        intResults[2] = reader.GetString(6);
                        intResults[3] = reader.GetString(8);

                        intResultKeys[0] = reader.GetString(3);
                        intResultKeys[1] = reader.GetString(5);
                        intResultKeys[2] = reader.GetString(7);
                        intResultKeys[3] = reader.GetString(9);

                        newInt = new Interaction(reader.GetString(1), intResults, intResultKeys, dbConnection);
                    }

                    return newInt;
                }
            }
        }

        public Result GetResultByName(string resultName, string resultKey, IDbConnection dbConnection) {
            using (IDbCommand dbcmd = dbConnection.CreateCommand()) {
                dbcmd.CommandText = "SELECT * FROM Results WHERE Name = '" + resultName + "'";

                using (IDataReader reader = dbcmd.ExecuteReader()) {
                    Result newResult = null;
                    int[] resultChanges = new int[4];

                    if (reader.Read()) {
                        resultChanges[0] = reader.GetInt32(3);
                        resultChanges[1] = reader.GetInt32(4);
                        resultChanges[2] = reader.GetInt32(5);
                        resultChanges[3] = reader.GetInt32(6);

                        newResult = new Result(reader.GetString(1), resultKey, reader.GetString(2), resultChanges);
                    }

                    return newResult;
                }
            }
        }
    }
}
