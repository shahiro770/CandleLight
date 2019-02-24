/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 11, 2019
* 
* The GameDB class is used to access data in the game's database (GameDB)
* It is used to fetch information of monsters, party members, items, and more.
*
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DataBank {
    public class GameDB : SqliteHelper {
    
        const int maxAttacks = 4;
        const string dbName = "GameDB.db";

        public GameDB() : base(dbName) {}
        
        public void GetMonsterByNameID(string nameID, Monster monster) {
            using(dbConnection = base.GetConnection()) {

                dbConnection.Open();

                using (IDbCommand dbcmd = dbConnection.CreateCommand()) { // using will call dispose when done, which calls close
                    dbcmd.CommandText = "SELECT * FROM Monsters WHERE NameID = '" + nameID + "'";
                    
                    using (IDataReader reader = dbcmd.ExecuteReader()) {
                        string monsterName = "";
                        string monsterArea = "";
                        string monsterSize = "";
                        int LVL = 0;
                        int HP = 0;
                        int MP = 0;
                        int[] stats = {};
                        Attack[] attacks = new Attack[4];

                        if (reader.Read()) {
                            monsterName = reader.GetString(2);
                            monsterArea = reader.GetString(3);
                            monsterSize = reader.GetString(4);
                            LVL = reader.GetInt32(5);
                            HP = reader.GetInt32(6); 
                            MP = reader.GetInt32(7);
                            stats = new int[] { reader.GetInt32(8), reader.GetInt32(9), reader.GetInt32(10), reader.GetInt32(11) };

                            for (int i = 0; i < maxAttacks; i++) {
                                string attackName = reader.GetString(12 + i);
                                attacks[i] = GetAttack(attackName, dbConnection);
                            }

                        }
                        // need to figure out how to attach this information to a monster gameObject, can't use new
                        monster.Init(monsterName, monsterArea, monsterSize, LVL, HP, MP, stats, attacks); 
                    }
                }
            }          
        }

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
                                attacks[i] = GetAttack(attackName, dbConnection);
                            }
                        }
                        // need to figure out how to attach this information to a monster gameObject, can't use new
                        pm.Init(personalInfo, LVL, HP, MP, stats, attacks); 
                    }
                }
            }          
        }

        // returns an attack and its information, 
        public Attack GetAttack(string name, IDbConnection dbConnection) {
            using (IDbCommand dbcmd = dbConnection.CreateCommand()) {
                dbcmd.CommandText = "SELECT * FROM Attacks WHERE Name = '" + name + "'";

                using (IDataReader reader = dbcmd.ExecuteReader()) {
                    Attack newAttack = null;

                    if (reader.Read()) {
                        newAttack = new Attack( name, reader.GetInt32(2));
                    }

                    return newAttack;
                }
            }
        }
    }
}
