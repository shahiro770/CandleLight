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
                            newAttack = new Attack(name, reader.GetInt32(2), reader.GetString(3));
                        }
                        else {
                            newAttack = new Attack(name, reader.GetInt32(2), reader.GetString(4));
                        }
                    }

                    return newAttack;
                }
            }
        }
    }
}
