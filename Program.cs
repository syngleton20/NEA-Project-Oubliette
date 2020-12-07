using NEA_Project_Oubliette.Database;
using NEA_Project_Oubliette.Editing;
using System;

namespace NEA_Project_Oubliette
{
    ///<summary>Controls the flow of the program</summary>
    internal static class Program
    {
        ///<summary>The method which is called when the program is run</summary>
        private static void Main(string[] args)
        {
            Console.WriteLine();
            Console.WriteLine("  Loading...");

            FileHandler.Setup();
            Window.Setup();

            ConnectToDatabase();

            while (true)
            {
                GUI.Title("Main Menu");
                Display.WriteAtCentreBottom((AccountManager.IsLoggedIn ? AccountManager.Account.Username : "Not Logged In") + '\n' + (DatabaseManager.IsConnected ? "Connected" : "Not Connected"));
                Console.SetCursorPosition(0, 4);

                switch (GUI.VerticalMenu("New Game", "Continue", "Map Editor", "Online Maps (Coming Soon)", DatabaseManager.IsConnected ? "Account" : "Account (Not Connected)", "Quit"))
                {
                    case 0:
                        Game.Current = new Game(GameType.Game, "start.map");
                        Game.Current.Start();
                        break;

                    case 1:
                        GUI.Title("Load from Save File");

                        int slotIndex = SaveManager.ChooseSlotToLoad();
                        if(slotIndex < 0) break;

                        Game.Current = SaveManager.Load(slotIndex);
                        Game.Current.Start();
                        break;

                    case 2:
                        Console.Clear();
                        MapEditor.New();
                        break;

                    case 3:
                        Console.Clear();
                        Debug.Warning("This feature is still in development. Sorry :(");
                        break;

                    case 4:
                        ConnectToDatabase();

                        if(!DatabaseManager.IsConnected)
                        {
                            Console.Clear();
                            GUI.Title("Cannot Connect");

                            Display.WriteAtCentre("Cannot connect to the database!");
                            Display.WriteAtCentre("Press any key to continue...");

                            Input.GetKeyDown();
                            break;
                        }

                        Console.Clear();
                        GUI.Title("Account");

                        if(AccountManager.IsLoggedIn)
                        {
                            switch (GUI.VerticalMenu("Back", "Create Account", "Account Settings", "Log Out"))
                            {
                                case 0:
                                    break;

                                case 1:
                                    Debug.Warning("This feature will be added in the next commit!");
                                    break;

                                case 2:
                                    Debug.Warning("This feature will be added in the next commit!");
                                    break;

                                case 3:
                                    AccountManager.LogOut();
                                    break;
                            }
                        }
                        else
                        {
                            switch (GUI.VerticalMenu("Back", "Create Account", "Log In"))
                            {
                                case 0:
                                    break;

                                case 1:
                                    AccountManager.CreateAccountMenu(AccountType.User);
                                    break;

                                case 2:
                                    AccountManager.LogInMenu();
                                    break;
                            }
                        }
                        break;

                    case 5:
                        Environment.Exit(0);
                        break;
                }
            }

        }

        private static void ConnectToDatabase()
        {
            Display.Clear();

            Console.WriteLine();
            Console.WriteLine("  Attempting to connect to Database...");

            DatabaseManager.Connect();
            Display.Clear();
        }
    }
}