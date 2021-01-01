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

        public UserAccount(int userID, string username, string password)
        {
            this.userID = userID;
            this.username = username;
            this.password = password;
        }

        public override void Update(params string[] details)
        {
            if(details[0] != "")
            {
                bool hasRows = DatabaseManager.QueryRowScalarValue("SELECT COUNT(*) FROM User WHERE UserID = @UserID", userID) > 0;

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
                bool hasRows = DatabaseManager.QueryRowScalarValue("SELECT COUNT(*) FROM User WHERE UserID = @UserID", userID) > 0;

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