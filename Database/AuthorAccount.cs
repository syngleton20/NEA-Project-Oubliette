using MySql.Data.MySqlClient;
using System;

namespace NEA_Project_Oubliette.Database
{
    internal sealed class AuthorAccount : UserAccount
    {
        private readonly int authorID;
        private string emailAddress;

        public int AuthorID => authorID;
        public string EmailAddress => emailAddress;

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

        public override void Update(params string[] details)
        {
            base.Update(details);

            if(details.Length >= 3 && details[2] != "")
            {
                MySqlDataReader presenceCheck = DatabaseManager.QuerySQL("SELECT * FROM Author WHERE UserID = @UserID", userID);
                bool hasRows = presenceCheck.HasRows;

                presenceCheck.Close();
                presenceCheck.Dispose();

                if(hasRows)
                {
                    emailAddress = details[2];
                    DatabaseManager.ExecuteDDL("UPDATE Author SET Email = @Email WHERE UserID = @UserID", emailAddress, userID);

                    Display.Clear();
                    GUI.Title("Account Settings");

                    Display.WriteAtCentre("Successfully updated your email address!");
                    Console.WriteLine();
                    GUI.Confirm();
                }
                else
                {
                    Display.Clear();
                    GUI.Title("Account Settings - Error");
                    Display.WriteAtCentre("Could not update email address!");
                    GUI.Confirm();
                }
            }
        }
    }
}