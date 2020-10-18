using NEA_Project_Oubliette.Entities;
using NEA_Project_Oubliette.Maps;
using System;

namespace NEA_Project_Oubliette
{
    ///<summary>Provides methods for easily debugging the program</summary>
    internal static class Debug
    {
        ///<summary>Runs code for testing certain functionality</summary>
        public static void Test()
        {
            // Write test code here
            Game game = new Game(GameType.Game, "test2.map");
            Debug.Warning(game.CurrentMap.Collection.Array.Length);

            while (true)
            {
                GUI.Title("Main Menu");

                switch (GUI.VerticalMenu("New Game", "Continue (Coming Soon)", "Level Editor (Coming Soon)", "Online Maps (Coming Soon)", "Log In (Coming Soon)", "Quit"))
                {
                    case 0:
                        game.Start();
                        break;

                    case 1:
                        game.Start(); // change later
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

        ///<summary>Logs a message to the debug.log file in /data/</summary>
        public static void Log(object message) => FileHandler.WriteToFile("data/debug.log", message.ToString(), true);

        ///<summary>Prints the result of a boolean condition to the screen, then clears the screen</summary>
        public static void Assert(bool condition, bool clear = true)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(condition.ToString());
            Console.Write("Press any key to continue... ");
            Console.ReadKey(true);

            if(clear) Window.Clear();
        }

        ///<summary>Prints a warning message on the screen, then clears the screen</summary>
        public static void Warning(object message, bool clear = true)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message.ToString());
            Console.Write("Press any key to continue... ");
            Console.ReadKey(true);

            if(clear) Window.Clear();
        }

        ///<summary>Prints an error message on the screen, then exits the program</summary>
        public static void Error(object message, bool clear = true)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message.ToString());
            Console.Write("Press any key to end the program... ");
            Console.ReadKey(true);
        }
    }
}