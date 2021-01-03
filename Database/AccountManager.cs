using System;

namespace NEA_Project_Oubliette.Database
{
    public enum AccountType { User, Author }

    ///<summary>Allows user and author accounts to be created, edited, logged into and out of, and deleted</summary>
    internal static class AccountManager
    {
        public static bool IsLoggedIn => Account != null;
        public static Account Account { get; private set; }

        ///<summary>Displays a GUI for creating either a user account or an author account</summary>
        public static void CreateAccountMenu()
        {
            string email = "";

            Display.Clear();
            GUI.Title("Create Account");

            Display.WriteAtCentre("What do you want to use this account for?");
            Console.WriteLine();

            AccountType type = (AccountType)GUI.VerticalMenu("Just playing maps", "Playing and publishing maps");

            Display.Clear();
            GUI.Title("Create Account");

            Console.WriteLine("           Username: ");
            Console.WriteLine("           Password: ");
            Console.WriteLine("  Password (Retype): ");

            if(type == AccountType.Author)
                Console.WriteLine("      Email Address: ");

            Console.CursorTop -= type == AccountType.Author ? 4 : 3;

            string username = GUI.TextField("         Username: ", 20);
            Console.WriteLine();

            if(string.IsNullOrWhiteSpace(username))
                return;

            string password = GUI.TextField("         Password: ", 20, "", true);
            Console.WriteLine();

            if(string.IsNullOrWhiteSpace(password))
                return;

            string passwordRetyped = GUI.TextField("Password (Retype): ", 20, "", true);
            Console.WriteLine();

            if(string.IsNullOrWhiteSpace(passwordRetyped))
                return;

            if(password == passwordRetyped)
            {
                if(type == AccountType.Author)
                {
                    email = GUI.TextField("    Email Address: ", 30);

                    if(!ValidateEmailAddress(email))
                    {
                        Display.Clear();
                        GUI.Title("Create Account - Error");

                        Display.WriteAtCentre("Cannot create author account. The email");
                        Display.WriteAtCentre("address provided is invalid.");

                        Console.WriteLine();
                        GUI.Confirm();
                        return;
                    }
                }

                UserAccount userAccount = CreateUserAccount(username, password);

                if(userAccount != null)
                {
                    if(type == AccountType.Author)
                    {
                        AuthorAccount authorAccount = CreateAuthorAccount(userAccount.UserID, email);

                        if(authorAccount != null)
                        {
                            Account = authorAccount;

                            Display.Clear();
                            GUI.Title("Create Account");

                            Display.WriteAtCentre($"Successfully created account {AccountManager.Account.Username}!");
                        }
                        else
                        {
                            Display.Clear();
                            GUI.Title("Create Account");

                            Display.WriteAtCentre("Could not upgrade account!");
                            Display.WriteAtCentre("This account is already an author account!");
                        }
                    }
                    else
                    {
                        Display.Clear();
                        GUI.Title("Create Account");

                        Display.WriteAtCentre($"Successfully created account {AccountManager.Account.Username}!");
                    }

                    Console.WriteLine();
                    GUI.Confirm();
                }
                else
                {
                    Display.Clear();
                    GUI.Title("Create Account");

                    Display.WriteAtCentre("Could not create account!");
                    Display.WriteAtCentre("Username was already taken!");

                    Console.WriteLine();
                    GUI.Confirm();
                }
            }
            else
            {
                Display.Clear();
                GUI.Title("Create Account");

                Display.WriteAtCentre("Passwords do not match!");
                Console.WriteLine();
                GUI.Confirm();
            }
        }

        ///<summary>Receives an email address from the user and uses it to upgrade a user account to an author account</summary>
        public static void UpgradeAccountMenu()
        {
            Display.Clear();
            GUI.Title("Upgrade Account");

            string email = GUI.TextField("Email Address: ", 30);

            if(string.IsNullOrWhiteSpace(email))
                return;

            if(!ValidateEmailAddress(email))
            {
                Display.Clear();
                GUI.Title("Create Account - Error");

                Display.WriteAtCentre("Cannot create author account. The email");
                Display.WriteAtCentre("address provided is invalid.");

                Console.WriteLine();
                GUI.Confirm();
                return;
            }

            AuthorAccount authorAccount = CreateAuthorAccount(Account.UserID, email);

            if(authorAccount != null)
            {
                Account = authorAccount;

                Display.Clear();
                GUI.Title("Create Account");

                Display.WriteAtCentre($"Successfully upgrade account {AccountManager.Account.Username}!");
            }
            else
            {
                Display.Clear();
                GUI.Title("Create Account");

                Display.WriteAtCentre("Could not upgrade account!");
                Display.WriteAtCentre("This account is already an author account!");
            }
        }

        ///<summary>Displays a GUI for logging into either a user account or an author account</summary>
        public static void LogInMenu()
        {
            Display.Clear();
            GUI.Title("Log In");

            Console.WriteLine("  Username: ");
            Console.WriteLine("  Password: ");

            Console.CursorTop -= 2;

            string username = GUI.TextField("Username: ", 20);

            if(string.IsNullOrWhiteSpace(username))
                return;

            Console.WriteLine();
            string password = GUI.TextField("Password: ", 20, "", true);

            if(string.IsNullOrWhiteSpace(password))
                return;

            if(!LogIn(username, password))
            {
                Display.Clear();
                GUI.Title("Log In");

                Display.WriteAtCentre("Username or password was incorrect!");
                Console.WriteLine();
                GUI.Confirm();
            }
            else
            {
                Display.Clear();
                GUI.Title("Log In - Successful");

                Display.WriteAtCentre($"Successfully logged in as {AccountManager.Account.Username}!");
                Console.WriteLine();
                GUI.Confirm();
            }
        }

        ///<summary>Logs the user out of their account, if they are logged in</summary>
        public static void LogOut()
        {
            Display.Clear();
            GUI.Title("Log Out");

            Display.WriteAtCentre("Are you sure you want to log out?");
            Console.WriteLine();

            if(Account != null && GUI.VerticalMenu("YES", "NO") <= 0)
                Account = null;
        }

        ///<summary>Allows users and authors to edit their account details as they see fit</summary>
        public static void AccountSettingsMenu()
        {
            Display.Clear();
            GUI.Title("Account Settings");

            switch (GUI.VerticalMenu("Back", "Change Username", "Change Password", "Change Email", "Delete Account"))
            {
                case 0:
                    break;

                case 1:
                    GUI.Title("Account Settings");

                    string username = GUI.TextField("Username: ", 20, Account.Username);
                    Account.Update(username);
                    break;

                case 2:
                    GUI.Title("Account Settings");
                    string password = GUI.TextField("Current Password: ", 20, "", true);
                    bool validPassword = DatabaseManager.QueryRowScalarValue("SELECT COUNT(*) FROM User WHERE Password = @Password", password) > 0;

                    if(validPassword)
                    {
                        Display.Clear();
                        GUI.Title("Account Settings");

                        string newPassword = GUI.TextField("New Password: ", 20, "", true);
                        Account.Update("", newPassword);
                    }
                    else
                    {
                        Display.Clear();
                        GUI.Title("Account Settings - Error");
                        Display.WriteAtCentre("Incorrect password!");
                        GUI.Confirm();
                    }
                    break;

                case 3:
                    if(Account.GetType() != typeof(AuthorAccount))
                    {
                        Display.Clear();
                        GUI.Title("Account Settings - Error");
                        Display.WriteAtCentre("This account is not a registered author account!");
                        Display.WriteAtCentre("Please upgrade your account if you wish to start");
                        Display.WriteAtCentre("publishing maps.");

                        Console.WriteLine();
                        GUI.Confirm();
                    }
                    else
                    {
                        Display.Clear();
                        GUI.Title("Account Settings");

                        string email = GUI.TextField("Email Address: ", 30, (Account as AuthorAccount).EmailAddress);
                        Account.Update("", "", email);
                    }
                    break;

                case 4:
                    Display.Clear();
                    GUI.Title("Account Settings");

                    Display.WriteAtCentre("Are you sure you want to delete your account?");

                    if(Account.GetType() == typeof(AuthorAccount))
                        Display.WriteAtCentre("All your published maps will be taken down!");

                    Display.WriteAtCentre("This action is permanent!");

                    Console.WriteLine();

                    if(GUI.VerticalMenu("YES", "NO") <= 0)
                    {
                        Display.Clear();
                        GUI.Title("Account Settings");

                        string confirmation = GUI.TextField("Password: ", 20, "", true);
                        bool isValid = DatabaseManager.QueryRowScalarValue("SELECT COUNT(*) FROM User WHERE UserID = @UserID AND Password = @Password", Account.UserID, confirmation) > 0;

                        if(isValid)
                        {
                            bool isAuthor = DatabaseManager.QueryRowScalarValue("SELECT COUNT(*) FROM Author WHERE UserID = @UserID", Account.UserID) > 0;

                            if(isAuthor)
                            {
                                DatabaseManager.ExecuteDDL("DELETE FROM Map WHERE AuthorID = @AuthorID", (Account as AuthorAccount).AuthorID);
                                DatabaseManager.ExecuteDDL("DELETE FROM Author WHERE UserID = @UserID", Account.UserID);
                            }

                            DatabaseManager.ExecuteDDL("DELETE FROM Scoreboard WHERE UserID = @UserID", Account.UserID);
                            DatabaseManager.ExecuteDDL("DELETE FROM User WHERE UserID = @UserID", Account.UserID);
                            Account = null;
                        }
                        else
                        {
                            Display.Clear();
                            GUI.Title("Account Settings - Error");

                            Display.WriteAtCentre("Incorrect password!");
                            Console.WriteLine();
                            GUI.Confirm();
                        }
                    }
                    break;
            }
        }

        ///<summary>Validates an email address by checking if one and only one at symbol is present in the email address and if there are characters on either side of the at symbol</summary>
        private static bool ValidateEmailAddress(string email)
        {
            bool foundAtSymbol = false;

            if(email.Contains('@'))
            {
                for(int i = 0; i < email.Length; i++)
                {
                    if(email[i] == '@')
                    {
                        if(!foundAtSymbol)
                        {
                            if(i <= 0 || i >= (email.Length - 1)) return false;
                            else foundAtSymbol = true;
                        }
                        else return false;
                    }
                }
            }

            return foundAtSymbol;
        }

        ///<summary>Creates a new user account, provided the username is unique</summary>
        public static UserAccount CreateUserAccount(string username, string password)
        {
            bool hasRows = DatabaseManager.QueryRowScalarValue("SELECT COUNT(*) FROM User WHERE Username = @Username", username) > 0;

            if(!hasRows)
            {
                DatabaseManager.ExecuteDDL("INSERT INTO User(Username, Password) VALUES (@Username, @Password)", username, password);
                DatabaseManager.QuerySQL("SELECT * FROM User WHERE Username = @Username AND Password = @Password", username, password);

                DatabaseManager.Reader.Read();
                int userID = DatabaseManager.Reader.GetInt32("UserID");

                Account = new UserAccount(userID, username, password);
                return Account as UserAccount;
            }

            return null;
        }

        ///<summary>Creates a new author account, provided a valid user account exists</summary>
        public static AuthorAccount CreateAuthorAccount(int userID, string email)
        {
            bool hasRows = DatabaseManager.QueryRowScalarValue("SELECT COUNT(*) FROM Author WHERE UserID = @UserID", userID) > 0;

            if(!hasRows)
            {
                DatabaseManager.ExecuteDDL("INSERT INTO Author(UserID, Email) VALUES (@UserID, @Email)", userID, email);
                DatabaseManager.QuerySQL("SELECT * FROM Author WHERE UserID = @UserID AND Email = @Email", userID, email);

                DatabaseManager.Reader.Read();
                int authorID = DatabaseManager.Reader.GetInt32("AuthorID");

                DatabaseManager.QuerySQL("SELECT * FROM User WHERE UserID = @UserID", userID);

                DatabaseManager.Reader.Read();
                string username = DatabaseManager.Reader.GetString("Username");

                DatabaseManager.Reader.Read();
                string password = DatabaseManager.Reader.GetString("Password");

                Account = new AuthorAccount(userID, authorID, username, email, password);
                return Account as AuthorAccount;
            }

            return null;
        }

        ///<summary>Logs a user into their account, provided the username and password arguments are both valid</summary>
        public static bool LogIn(string username, string password)
        {
            DatabaseManager.QuerySQL("SELECT * FROM User WHERE Username = @Username", username);
            bool hasRows = DatabaseManager.Reader.HasRows;

            if(!hasRows)
                return false;

            DatabaseManager.Reader.Read();
            string resultingPassword = DatabaseManager.Reader.GetString("Password");

            if(resultingPassword == password)
            {
                DatabaseManager.Reader.Read();
                int userID = DatabaseManager.Reader.GetInt32("UserID");

                Account = new UserAccount(userID, username, password);
                bool isAuthor = DatabaseManager.QueryRowScalarValue("SELECT COUNT(*) FROM Author WHERE UserID = @UserID", Account.UserID) > 0;

                if(isAuthor)
                {
                    DatabaseManager.QuerySQL("SELECT * FROM Author WHERE UserID = @UserID", userID);

                    DatabaseManager.Reader.Read();
                    int authorID = DatabaseManager.Reader.GetInt32("AuthorID");

                    DatabaseManager.Reader.Read();
                    string emailAddress = DatabaseManager.Reader.GetString("Email");

                    Account = new AuthorAccount(userID, authorID, username, emailAddress, password);
                }
                else Account = new UserAccount(userID, username, password);

                return true;
            }

            return false;
        }
    }
}