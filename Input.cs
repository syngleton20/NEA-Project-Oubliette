using System;

namespace NEA_Project_Oubliette
{
    ///<summary>Provides simple methods for receiving input from the keyboard</summary>
    internal static class Input
    {
        private static ConsoleKeyInfo lastInput;

        public static bool IsControlKeyDown => lastInput.Modifiers.HasFlag(ConsoleModifiers.Control);
        public static bool IsShiftKeyDown => lastInput.Modifiers.HasFlag(ConsoleModifiers.Shift);
        public static bool IsAltKeyDown => lastInput.Modifiers.HasFlag(ConsoleModifiers.Alt);

        ///<summary>Halts the program and waits to return a keycode from the keyboard</summary>
        public static ConsoleKey GetKeyDown()
        {
            lastInput = Console.ReadKey(true);
            return lastInput.Key;
        }
    }
}