/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 11, 2019
* 
* The SqliteHelper class is used as an interface for other database manager classes.
* It contains the outlines for what other databases in this program should have.
*
*/

using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Database {

    public class SqliteHelper {
        private string tag;                     /// <value> Name of database </value>
        private string dbConnectionString;      /// <value> File path to database file</value>
        public IDbConnection dbConnection;      /// <value> Open connection to database </value>

        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="databaseFile"> Name of the database file to access </param>
        public SqliteHelper(string databaseFileName) {
            tag = databaseFileName + ":\t";

            string dbPath = Path.Combine(Application.streamingAssetsPath, databaseFileName);  
            if (System.IO.File.Exists(dbPath)) {
                dbConnectionString = "URI=file:" + dbPath;
            }
            else {
                Debug.Log ("ERROR: the file DB named " + databaseFileName + " doesn't exist anywhere");
            }
        }
        
        /// <summary>
        /// Destructor to close connection on destruction
        /// </summary>
        ~SqliteHelper() {
            dbConnection.Close();
        }

        /// <summary>
        /// Returns a data entry by the entry's id and the table
        /// </summary>
        /// <param name="id"> Id of the entry (unique) </param>
        /// <param name="table"> Table to be accessed </param>
        public virtual void GetDataById(int id, string table) {
            Debug.Log(tag + "GetDataById is not implemented");
            throw null;
        }

        /// <summary>
        /// Returns an IDbConnection to interact with the database with
        /// </summary>
        /// <returns></returns>
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