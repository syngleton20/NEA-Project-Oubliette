using System;

namespace NEA_Project_Oubliette
{
    ///<summary>Provides simple methods for receiving input from the keyboard</summary>
    internal static class Input
    {
        ///<summary>Halts the program and waits to return a keycode from the keyboard</summary>
        public static ConsoleKey GetKeyDown() => Console.ReadKey(true).Key;
    }
}