/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 11, 2019
* 
* The SqliteHelper class is used as an interface for other database classes.
* It contains the outlines for what other databases in this program should have.
*
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Data.Sqlite;
using UnityEngine;
using System.Data;

namespace DataBank
{
    public class SqliteHelper
    {
        private string tag;
        private string dbConnectionString;
        public IDbConnection dbConnection;

        public SqliteHelper(string databaseFile)
        {
            tag = databaseFile + ":\t";
            dbConnectionString = "URI=file:" + Application.dataPath + "/Scripts/Database/" + databaseFile ;
        }

        ~SqliteHelper()
        {
            dbConnection.Close();
        }

        // virtual functions
        public virtual void GetDataById(int id, string table)
        {
            Debug.Log(tag + "GetDataById is not implemented");
            throw null;
        }

        public IDbConnection GetConnection() {
            return new SqliteConnection(dbConnectionString);
        }

        /* public virtual IDataReader GetDataByString(string str)
        {
            Debug.Log(tag + "This function is not implemented");
            throw null;
        }

        public virtual void DeleteDataById(int id)
        {
            Debug.Log(tag + "This function is not implemented");
            throw null;
        }

        public virtual void DeleteDataByString(string id)
        {
            Debug.Log(tag + "This function is not implemented");
            throw null;
        }

        public virtual IDataReader GetAllData()
        {
            Debug.Log(tag + "This function is not implemented");
            throw null;
        }

        public virtual void DeleteAllData()
        {
            Debug.Log(tag + "This function is not implemented");
            throw null;
        }

        public virtual IDataReader GetNumOfRows()
        {
            Debug.Log(tag + "This function is not implemented");
            throw null;
        }

        //helper functions
        public IDataReader GetAllData(string tableName)
        {
            IDbCommand dbcmd = dbConnection.CreateCommand();
            dbcmd.CommandText =
                "SELECT * FROM " + tableName;
            IDataReader reader = dbcmd.ExecuteReader();
            return reader;
        }

        public void DeleteAllData(string tableName)
        {
            IDbCommand dbcmd = dbConnection.CreateCommand();
            dbcmd.CommandText = "DROP TABLE IF EXISTS " + tableName;
            dbcmd.ExecuteNonQuery();
        }

        public IDataReader GetNumOfRows(string tableName)
        {
            IDbCommand dbcmd = dbConnection.CreateCommand();
            dbcmd.CommandText =
                "SELECT COALESCE(MAX(id)+1, 0) FROM " + tableName;
            IDataReader reader = dbcmd.ExecuteReader();
            return reader;
        } 

        public void Close () {
            dbConnection.Close ();
        } */
    }
}