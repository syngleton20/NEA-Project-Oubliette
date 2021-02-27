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

            while(true)
            {
                Display.Banner();
                Display.WriteAtCentreBottom((AccountManager.IsLoggedIn ? AccountManager.Account.Username : "Not Logged In"));
                Console.SetCursorPosition(0, 9);

                switch(GUI.VerticalMenu("New Game", "Load Game", "Map Editor", "Online Maps", "Account", "Scoreboard", "Quit"))
                {
                    case 0:
                        Game.Current = new Game(GameType.Game, "start.map");
                        Game.Current.Start();
                        break;

                    case 1:
                        GUI.Title("Load Game");

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
                        GUI.Title("Online Maps");

                        switch(GUI.VerticalMenu("Back", "Browse", "Downloads", "Upload"))
                        {
                            case 0: break;

                            case 1:
                                if(AccountManager.Account != null) MapBrowser.ShowBrowserMenu();
                                else
                                {
                                    Display.Clear();
                                    GUI.Title("Online Maps - Error");

                                    Display.WriteAtCentre("You need to be logged in to browse online maps.");
                                    Console.WriteLine();
                                    GUI.Confirm();
                                }
                                break;

                            case 2:
                                MapBrowser.ShowDownloadsMenu();
                                break;

                            case 3:
                                if(AccountManager.Account != null) MapBrowser.ShowUploadMenu();
                                else
                                {
                                    Display.Clear();
                                    GUI.Title("Online Maps - Error");

                                    Display.WriteAtCentre("You need to be logged in to an");
                                    Display.WriteAtCentre("author account to upload maps.");
                                    Console.WriteLine();
                                    GUI.Confirm();
                                }
                                break;
                        }
                        break;

                    case 4:
                        ConnectToDatabase();
                        if(!DatabaseManager.IsConnected) break;

                        Console.Clear();
                        GUI.Title("Account");

                        if(AccountManager.IsLoggedIn)
                        {
                            if(AccountManager.Account.GetType() == typeof(AuthorAccount))
                            {
                                switch(GUI.VerticalMenu("Back", "Account Settings", "Log Out"))
                                {
                                    case 0: break;

                                    case 1:
                                        AccountManager.AccountSettingsMenu();
                                        break;

                                    case 2:
                                        AccountManager.LogOut();
                                        break;
                                }
                            }
                            else
                            {
                                switch(GUI.VerticalMenu("Back", "Upgrade Account", "Account Settings", "Log Out"))
                                {
                                    case 0: break;

                                    case 1:
                                        AccountManager.UpgradeAccountMenu();
                                        break;

                                    case 2:
                                        AccountManager.AccountSettingsMenu();
                                        break;

                                    case 3:
                                        AccountManager.LogOut();
                                        break;
                                }
                            }
                        }
                        else
                        {
                            switch(GUI.VerticalMenu("Back", "Create Account", "Log In"))
                            {
                                case 0: break;

                                case 1:
                                    AccountManager.CreateAccountMenu();
                                    break;

                                case 2:
                                    AccountManager.LogInMenu();
                                    break;
                            }
                        }
                        break;

                    case 5:
                        ConnectToDatabase();
                        if(!DatabaseManager.IsConnected) break;

                        Scoreboard.DisplayAll();
                        break;

                    case 6:
                        Program.QuitGame();
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

        public static void QuitGame()
        {
            Display.Clear();
            GUI.Title("Are you sure?");

            Display.WriteAtCentre("Are you sure you want to quit?");
            Display.WriteAtCentre("Any progress you have made will not be saved.");

            Console.WriteLine();

            if(GUI.VerticalMenu("YES", "NO") <= 0) Environment.Exit(0);
        }
    }
}