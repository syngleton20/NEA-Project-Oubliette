using MySql.Data.MySqlClient;
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
            if(type == AccountType.Author) Console.WriteLine("      Email Address: ");

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
                UserAccount userAccount = CreateUserAccount(username, password);

                if(userAccount != null)
                {
                    if(type == AccountType.Author)
                    {
                        string email = GUI.TextField("    Email Address: ", 30);
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

        public static void UpgradeAccountMenu()
        {
            Display.Clear();
            GUI.Title("Upgrade Account");

            string email = GUI.TextField("Email Address: ", 30);
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

            // Display.Clear();
            // GUI.Title("Log In");

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

                    MySqlDataReader validityCheck = DatabaseManager.QuerySQL("SELECT * FROM User WHERE Password = @Password", password);
                    bool validPassword = validityCheck.HasRows;

                    validityCheck.Close();
                    validityCheck.Dispose();

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
                    if(Account.GetType() == typeof(AuthorAccount)) Display.WriteAtCentre("All your published maps will be taken down!");
                    Display.WriteAtCentre("This action is permanent!");

                    Console.WriteLine();

                    if(GUI.VerticalMenu("YES", "NO") <= 0)
                    {
                        Display.Clear();
                        GUI.Title("Account Settings");

                        string confirmation = GUI.TextField("Password: ", 20, "", true);
                        MySqlDataReader confirmationCheck = DatabaseManager.QuerySQL("SELECT * FROM User WHERE UserID = @UserID AND Password = @Password", Account.UserID, confirmation);
                        bool isValid = confirmationCheck.HasRows;

                        confirmationCheck.Close();
                        confirmationCheck.Dispose();

                        if(isValid)
                        {
                            MySqlDataReader authorCheck = DatabaseManager.QuerySQL("SELECT * FROM Author WHERE UserID = @UserID", Account.UserID);
                            bool isAuthor = authorCheck.HasRows;

                            authorCheck.Close();
                            authorCheck.Dispose();

                            if(isAuthor)
                            {
                                DatabaseManager.ExecuteDDL("DELETE FROM Map WHERE AuthorID = @AuthorID", (Account as AuthorAccount).AuthorID);
                                DatabaseManager.ExecuteDDL("DELETE FROM Author WHERE UserID = @UserID", Account.UserID);
                            }

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

        ///<summary>Creates a new user account, provided the username is unique</summary>
        public static UserAccount CreateUserAccount(string username, string password)
        {
            MySqlDataReader rowCheck = DatabaseManager.QuerySQL("SELECT * FROM User WHERE Username = @Username", username);
            bool hasRows = rowCheck.HasRows;

            rowCheck.Close();
            rowCheck.Dispose();

            if(!hasRows)
            {
                rowCheck.Close();

                DatabaseManager.ExecuteDDL("INSERT INTO User(Username, Password) VALUES (@Username, @Password)", username, password);
                MySqlDataReader result = DatabaseManager.QuerySQL("SELECT * FROM User WHERE Username = @Username AND Password = @Password", username, password);

                Account = new UserAccount(ref result);
                return Account as UserAccount;
            }

            return null;
        }

        ///<summary>Creates a new author account, provided a valid user account exists</summary>
        public static AuthorAccount CreateAuthorAccount(int userID, string email)
        {
            MySqlDataReader rowCheck = DatabaseManager.QuerySQL("SELECT * FROM Author WHERE UserID = @UserID", userID);
            bool hasRows = rowCheck.HasRows;

            rowCheck.Close();
            rowCheck.Dispose();

            if(!hasRows)
            {
                rowCheck.Close();

                DatabaseManager.ExecuteDDL("INSERT INTO Author(UserID, Email) VALUES (@UserID, @Email)", userID, email);
                MySqlDataReader result = DatabaseManager.QuerySQL("SELECT * FROM Author WHERE UserID = @UserID AND Email = @Email", userID, email);

                Account = new AuthorAccount(ref result);
                return Account as AuthorAccount;
            }

            return null;
        }

        ///<summary>Logs a user into their account, provided the username and password arguments are both valid</summary>
        public static bool LogIn(string username, string password)
        {
            MySqlDataReader result = DatabaseManager.QuerySQL("SELECT * FROM User WHERE Username = @Username", username);
            result.Read();

            if(!result.HasRows)
            {
                result.Close();
                result.Dispose();

                return false;
            }

            string resultingPassword = result.GetString("Password");

            if(resultingPassword == password)
            {
                Account = new UserAccount(ref result);

                result.Close();
                result.Dispose();

                result = DatabaseManager.QuerySQL("SELECT * FROM Author WHERE UserID = @UserID", Account.UserID);

                if(result.HasRows)
                    Account = new AuthorAccount(ref result);

                return true;
            }

            result.Close();
            result.Dispose();

            return false;
        }
    }
}