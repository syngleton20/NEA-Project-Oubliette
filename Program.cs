using System;

namespace NEA_Project_Oubliette
{
    ///<summary>Controls the flow of the program</summary>
    internal static class Program
    {
        ///<summary>The method which is called when the program is run</summary>
        private static void Main(string[] args)
        {
            FileHandler.Setup();
            Window.Setup();

            while (true)
            {
                GUI.Title("Main Menu");

                switch (GUI.VerticalMenu("New Game", "Continue", "Map Editor (Coming Soon)", "Online Maps (Coming Soon)", "Log In (Coming Soon)", "Quit"))
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
                        Debug.Warning("This feature is still in development. Sorry :(");
                        break;

                    case 3:
                        Console.Clear();
                        Debug.Warning("This feature is still in development. Sorry :(");
                        break;

                    case 4:
                        Console.Clear();
                        Debug.Warning("This feature is still in development. Sorry :(");
                        break;

                    case 5:
                        Environment.Exit(0);
                        break;
                }
            }
        }
    }
}