using MySql.Data.MySqlClient;

namespace NEA_Project_Oubliette.Database
{
    internal class UserAccount : Account
    {
        protected int userID;

        protected string username;
        protected string password;

        public override int UserID => userID;
        public override string Username => username;

        protected UserAccount() { }

        public UserAccount(ref MySqlDataReader reader)
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