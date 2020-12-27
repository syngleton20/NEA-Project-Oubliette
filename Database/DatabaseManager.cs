using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Data;
using System;

namespace NEA_Project_Oubliette.Database
{
    ///<summary>Provides methods for connecting and disconnecting to and from the NEA_Project_Oubliette database, as well as executing DDL commands and SQL queries</summary>
    internal static class DatabaseManager
    {
        private static MySqlConnection connection;
        private static MySqlCommand command;

        public static bool IsConnected => connection != null ? connection.State == ConnectionState.Open : false;

        ///<summary>Creates a paramaterised connection string using data from the dbprofile.txt file</summary>
        private static string ConnectionString()
        {
            if(FileHandler.FileExists("dbprofile.txt"))
            {
                string[] databaseProfile = FileHandler.ReadFile("dbprofile.txt").Split('\n');
                string connectionString = $"server=localhost;Database=NEA_Project_Oubliette;user={databaseProfile[0]};port=3306;password={databaseProfile[1]};";

                return connectionString;
            }

            return "";
        }

        ///<summary>Tries to return an SqlDbType from a .NET core Type</summary>
        private static MySqlDbType GetSqlType(Type type)
        {
            if(type == typeof(int)) return MySqlDbType.Int32;
            if(type == typeof(float)) return MySqlDbType.Float;
            if(type == typeof(bool)) return MySqlDbType.Bit;
            if(type == typeof(string)) return MySqlDbType.VarChar;

            throw new FormatException($"Cannot convert '{type.ToString()}' to an SqlDbType!");
        }

        ///<summary>Attempts to open a connection to the database</summary>
        public static void Connect()
        {
            if(IsConnected) return;

            try
            {
                if(ConnectionString() == "") return;

                connection = new MySqlConnection(ConnectionString());
                connection.Open();
            }
            catch
            {
                Display.Clear();
                GUI.Title("Connection - Failed");

                Display.WriteAtCentre("Could not connect to the database!");
                Console.WriteLine();
                GUI.Confirm();
            }
        }

        ///<summary>Attempts to close a connection to the database, if the connection is open in the first place</summary>
        public static void Disconnect()
        {
            if(connection != null && connection.State == ConnectionState.Open)
                connection.Close();
        }

        ///<summary>Attempts to execute a DDL command on the database</summary>
        public static void ExecuteDDL(string ddl, params object[] parameters)
        {
            try
            {
                List<string> ddlParams = new List<string>();
                string[] parts = ddl.Split(' ', '(', ')', '=', ',');

                for (int i = 0; i < parts.Length; i++)
                {
                    if(parts[i].Length <= 0) continue;
                    if(parts[i][0] == '@') ddlParams.Add(parts[i]);
                }

                using(command = new MySqlCommand(ddl, connection))
                {
                    for (int i = 0; i < ddlParams.Count; i++)
                    {
                        command.Parameters.Add(ddlParams[i], GetSqlType(parameters[i].GetType()));
                        command.Parameters[ddlParams[i]].Value = parameters[i];
                    }

                    command.ExecuteNonQuery();
                }
            }
            catch (MySqlException exception) { Debug.Error(exception); }
        }

        ///<summary>Attempts to return the result of an SQL query</summary>
        public static MySqlDataReader QuerySQL(string sql, params object[] parameters)
        {
            try
            {
                List<string> sqlParams = new List<string>();
                string[] parts = sql.Split(' ', '(', ')', '=', ',');

                for (int i = 0; i < parts.Length; i++)
                {
                    if(parts[i].Length <= 0) continue;
                    if(parts[i][0] == '@') sqlParams.Add(parts[i]);
                }

                using (command = new MySqlCommand(sql, connection))
                {
                    for (int i = 0; i < sqlParams.Count; i++)
                    {
                        command.Parameters.Add(sqlParams[i], GetSqlType(parameters[i].GetType()));
                        command.Parameters[sqlParams[i]].Value = parameters[i];
                    }

                    return command.ExecuteReader();
                }
            }
            catch (MySqlException exception)
            {
                Debug.Error(exception);
                return null;
            }
        }

        ///<summary>Attempts to fill and return a DataTable containing the result from an SQL query</summary>
        public static DataTable QuerySQLIntoTable(string sql, params object[] parameters)
        {
            try
            {
                List<string> sqlParams = new List<string>();
                string[] parts = sql.Split(' ', '(', ')', '=', ',');

                for (int i = 0; i < parts.Length; i++)
                {
                    if(parts[i].Length <= 0) continue;
                    if(parts[i][0] == '@') sqlParams.Add(parts[i]);
                }

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                    {
                        using (DataTable table = new DataTable())
                        {
                            adapter.Fill(table);
                            return table;
                        }
                    }
                }
            }
            catch (MySqlException exception)
            {
                Debug.Error(exception);
                return null;
            }
        }
    }
}