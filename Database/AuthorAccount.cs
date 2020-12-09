using MySql.Data.MySqlClient;

namespace NEA_Project_Oubliette.Database
{
    internal sealed class AuthorAccount : UserAccount
    {
        private readonly int authorID;
        private readonly string emailAddress;

        public AuthorAccount(ref MySqlDataReader reader)
        {
            reader.Read();
            authorID = reader.GetInt32("AuthorID");

            reader.Read();
            userID = reader.GetInt32("UserID");

            reader.Read();
            emailAddress = reader.GetString("Email");

            reader.Close();
            reader.Dispose();

            reader = DatabaseManager.QuerySQL("SELECT Username, Password FROM User WHERE UserID = @UserID", userID);

            reader.Read();
            username = reader.GetString("Username");

            reader.Read();
            password = reader.GetString("Password");

            reader.Close();
            reader.Dispose();
        }
    }
}