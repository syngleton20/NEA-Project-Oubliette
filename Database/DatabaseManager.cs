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
        private static MySqlDataReader reader;
        private static MySqlDataAdapter adapter;

        public static bool IsConnected => connection != null ? connection.State == ConnectionState.Open : false;

        public static MySqlDataReader Reader => reader;
        public static MySqlDataAdapter Adapter => adapter;

        ///<summary>Closes and disposes of the command, reader and adapter fields</summary>
        private static void ResetAll()
        {
            if(command != null)
                command.Dispose();

            if(reader != null)
            {
                reader.Close();
                reader.Dispose();
            }

            if(adapter != null)
                adapter.Dispose();
        }

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

        ///<summary>Adds parameters to a DDL command or SQL query</summary>
        private static MySqlCommand ParameteriseCommand(string commandText, object[] parameters)
        {
            ResetAll();

            command = new MySqlCommand(commandText, connection);
            List<string> sqlParams = new List<string>();

            string[] parts = commandText.Split(' ', '(', ')', '=', ',');

            for(int i = 0; i < parts.Length; i++)
            {
                if(parts[i].Length <= 0) continue;
                if(parts[i][0] == '@') sqlParams.Add(parts[i]);
            }

            for(int i = 0; i < sqlParams.Count; i++)
            {
                command.Parameters.Add(sqlParams[i], GetSqlType(parameters[i].GetType()));
                command.Parameters[sqlParams[i]].Value = parameters[i];
            }

            return command;
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
            ResetAll();

            try
            {
                using(command = ParameteriseCommand(ddl, parameters))
                    command.ExecuteNonQuery();
            }
            catch(MySqlException exception) { Debug.Error(exception); }
        }

        ///<summary>Attempts to assign to the Reader property</summary>
        public static void QuerySQL(string sql, params object[] parameters)
        {
            ResetAll();

            try
            {
                using(command = ParameteriseCommand(sql, parameters))
                    reader = command.ExecuteReader();
            }
            catch(MySqlException exception) { Debug.Error(exception); }
        }

        ///<summary>Attempts to return the row count of an SQL query in the form of an integer scalar value</summary>
        public static int QueryRowScalarValue(string sql, params object[] parameters)
        {
            ResetAll();

            try
            {
                using(command = ParameteriseCommand(sql, parameters))
                    return command.ExecuteScalar().ToString().ToInt();
            }
            catch(MySqlException exception)
            {
                Debug.Error(exception);
                return 0;
            }
        }

        ///<summary>Attempts to fill and return a DataTable containing the result from an SQL query</summary>
        public static DataTable QuerySQLIntoTable(string sql, params object[] parameters)
        {
            ResetAll();

            try
            {
                using(command = ParameteriseCommand(sql, parameters))
                {
                    using(adapter = new MySqlDataAdapter(command))
                    {
                        using(DataTable table = new DataTable())
                        {
                            adapter.Fill(table);
                            return table;
                        }
                    }
                }
            }
            catch(MySqlException exception)
            {
                Debug.Error(exception);
                return null;
            }
        }
    }
}