using System;
using MySql.Data.MySqlClient;

namespace NEA_Project_Oubliette.Database
{
    public enum AccountType { User, Author }

    ///<summary>Allows user and author accounts to be created, edited, logged into and out of, and deleted</summary>
    internal static class AccountManager
    {

        public static bool IsLoggedIn => Account != null;
        public static Account Account { get; private set; }

        ///<summary>Displays a GUI for creating either a user account or an author account</summary>
        public static void CreateAccountMenu(AccountType type)
        {
            if(type == AccountType.User)
            {
                Display.Clear();
                GUI.Title("Create Account - User");

                string username = GUI.TextField("Username: ", 20);

                if(string.IsNullOrWhiteSpace(username))
                    return;

                Display.Clear();
                GUI.Title("Create Account - User");

                string password = GUI.TextField("Password: ", 20);

                if(string.IsNullOrWhiteSpace(password))
                    return;

                Display.Clear();
                GUI.Title("Create Account - User");

                string passwordRetyped = GUI.TextField("Password (Retype): ", 20);

                if(string.IsNullOrWhiteSpace(passwordRetyped))
                    return;

                if(password == passwordRetyped)
                {
                    if(CreateUserAccount(username, password))
                    {
                        Display.Clear();
                        GUI.Title("Create Account - User");

                        Display.WriteAtCentre($"Successfully created account {AccountManager.Account.Username}!");
                        Display.WriteAtCentre("Press any key to continue...");

                        Input.GetKeyDown();
                    }
                    else
                    {
                        Display.Clear();
                        GUI.Title("Create Account - User");

                        Display.WriteAtCentre("Could not create account!");
                        Display.WriteAtCentre("Username was already taken!");
                        Display.WriteAtCentre("Press any key to continue...");

                        Input.GetKeyDown();
                    }
                }
                else
                {
                    Display.Clear();
                    GUI.Title("Create Account - User");

                    Display.WriteAtCentre("Passwords do not match!");
                    Display.WriteAtCentre("Press any key to continue...");

                    Input.GetKeyDown();
                }
            }
            else
            {
                Display.Clear();
                GUI.Title("Create Account - User");
            }
        }

        ///<summary>Displays a GUI for logging into either a user account or an author account</summary>
        public static void LogInMenu()
        {
            Display.Clear();
            GUI.Title("Log In");

            string username = GUI.TextField("Username: ", 20);

            if(string.IsNullOrWhiteSpace(username))
                return;

            Display.Clear();
            GUI.Title("Log In");

            string password = GUI.TextField("Password: ", 20);

            if(string.IsNullOrWhiteSpace(password))
                return;

            if(!LogIn(username, password))
            {
                Display.Clear();
                GUI.Title("Log In");

                Display.WriteAtCentre("Username or password was incorrect!");
                Display.WriteAtCentre("Press any key to continue...");

                Input.GetKeyDown();
            }
            else
            {
                Display.Clear();
                GUI.Title("Log In - Successful");

                Display.WriteAtCentre($"Successfully logged in as {AccountManager.Account.Username}!");
                Display.WriteAtCentre("Press any key to continue...");

                Input.GetKeyDown();
            }
        }

        ///<summary>Creates a new user account, provided the username is unique</summary>
        public static bool CreateUserAccount(string username, string password)
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

                Account = new Account(ref result);
                return true;
            }

            return false;
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
                Account = new Account(ref result);

                result.Close();
                result.Dispose();

                return true;
            }

            result.Close();
            result.Dispose();

            return false;
        }

        ///<summary>Logs the user out of their account, if they are logged in</summary>
        public static void LogOut()
        {
            if(Account != null)
                Account = null;
        }
    }
}