using MySql.Data.MySqlClient;
using System;

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

        public override void Update(params string[] details)
        {
            MySqlDataReader presenceCheck = null;

            if(details[0] != "")
            {
                presenceCheck = DatabaseManager.QuerySQL("SELECT * FROM User WHERE UserID = @UserID", userID);
                bool hasRows = presenceCheck.HasRows;

                presenceCheck.Close();
                presenceCheck.Dispose();

                if(hasRows)
                {
                    username = details[0];
                    DatabaseManager.ExecuteDDL("UPDATE User SET Username = @Username WHERE UserID = @UserID", username, userID);

                    Display.Clear();
                    GUI.Title("Account Settings");

                    Display.WriteAtCentre("Successfully updated your username!");
                    Console.WriteLine();
                    GUI.Confirm();
                }
                else
                {
                    Display.Clear();
                    GUI.Title("Account Settings - Error");
                    Display.WriteAtCentre("Could not update username!");
                    GUI.Confirm();
                }
            }
            if(details.Length >= 2 && details[1] != "")
            {
                presenceCheck = DatabaseManager.QuerySQL("SELECT * FROM User WHERE UserID = @UserID", userID);
                bool hasRows = presenceCheck.HasRows;

                presenceCheck.Close();
                presenceCheck.Dispose();

                if(hasRows)
                {
                    password = details[1];
                    DatabaseManager.ExecuteDDL("UPDATE User SET Password = @Password WHERE UserID = @UserID", password, userID);

                    Display.Clear();
                    GUI.Title("Account Settings");

                    Display.WriteAtCentre("Successfully updated your password!");
                    Console.WriteLine();
                    GUI.Confirm();
                }
                else
                {
                    Display.Clear();
                    GUI.Title("Account Settings - Error");
                    Display.WriteAtCentre("Could not update password!");
                    GUI.Confirm();
                }
            }
        }
    }
}