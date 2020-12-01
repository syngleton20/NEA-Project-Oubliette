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

            if(clear) Display.Clear();
        }

        ///<summary>Prints a warning message on the screen, then clears the screen</summary>
        public static void Warning(object message, bool clear = true)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message.ToString());
            Console.Write("Press any key to continue... ");
            Console.ReadKey(true);

            if(clear) Display.Clear();
        }

        ///<summary>Prints an error message on the screen, then exits the program</summary>
        public static void Error(object message, bool clear = true)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message.ToString());
            Console.Write("Press any key to end the program... ");
            Console.ReadKey(true);
        }

        ///<summary>Prints an error message on the screen, then exits the program</summary>
        public static void Error(Exception exception, bool clear = true)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Message:");
            Console.WriteLine(exception.Message);
            Console.WriteLine("\nSource:");
            Console.WriteLine(exception.Source);
            Console.WriteLine("\nStack Trace:");
            Console.WriteLine(exception.StackTrace);
            Console.Write("Press any key to end the program... ");
            Console.ReadKey(true);
        }
    }
}