using MySql.Data.MySqlClient;

namespace NEA_Project_Oubliette.Database
{
    internal sealed class Account
    {
        private readonly string password;

        private readonly int userID;
        private readonly int? authorID = null;

        private readonly string username;
        private readonly string emailAddress;

        public string Username => username;

        public bool IsAuthor => authorID != null && emailAddress != null;

        public Account(ref MySqlDataReader reader)
        {
            reader.Read();
            userID = reader.GetInt32("UserID");

            reader.Read();
            username = reader.GetString("Username");

            reader.Read();
            password = reader.GetString("Password");

            reader.Close();
            reader.Dispose();
        }
    }
}