using System;

namespace NEA_Project_Oubliette.Database
{
    internal sealed class AuthorAccount : UserAccount
    {
        private readonly int authorID;
        private string emailAddress;

        public int AuthorID => authorID;
        public string EmailAddress => emailAddress;

        public AuthorAccount(int userID, int authorID, string username, string emailAddress, string password)
        {
            this.userID = userID;
            this.authorID = authorID;
            this.username = username;
            this.emailAddress = emailAddress;
            this.password = password;
        }

        public override void Update(params string[] details)
        {
            base.Update(details);

            if(details.Length >= 3 && details[2] != "")
            {
                bool hasRows = DatabaseManager.QueryRowScalarValue("SELECT COUNT(*) FROM Author WHERE UserID = @UserID", userID) > 0;

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