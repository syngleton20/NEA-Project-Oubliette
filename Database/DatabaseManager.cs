using MySql.Data.MySqlClient;
using System.Data;

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
            catch { }
        }

        ///<summary>Attempts to close a connection to the database, if the connection is open in the first place</summary>
        public static void Disconnect()
        {
            if(connection != null && connection.State == ConnectionState.Open)
                connection.Close();
        }

        ///<summary>Attempts to execute a DDL command on the database</summary>
        public static void ExecuteDDL(string ddl)
        {
            try
            {
                using(command = new MySqlCommand(ddl, connection))
                    command.ExecuteNonQuery();

                command.Dispose();
            }
            catch (MySqlException exception) { Debug.Error(exception); }
        }

        ///<summary>Attempts to return the result of an SQL query</summary>
        public static MySqlDataReader QuerySQL(string sql)
        {
            try
            {
                using (command = new MySqlCommand(sql, connection))
                    return command.ExecuteReader();
            }
            catch (MySqlException exception)
            {
                Debug.Error(exception);
                return null;
            }
        }
    }
}