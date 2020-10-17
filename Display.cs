using NEA_Project_Oubliette.Maps;
using System;

namespace NEA_Project_Oubliette
{
    ///<summary>Methods for writing non-contiguous or column-based text to the console</summary>
    internal static class Display
    {
        private static int lastCursorX = -1;

        public static Vector CursorPosition => new Vector(Console.CursorLeft, Console.CursorTop);

        ///<summary>Writes to the console, keeping a reference to the cursor's X coordinate if necessary</summary>
        public static void Write(object text = null)
        {
            if(lastCursorX < 0) lastCursorX = CursorPosition.X;
            Console.Write(text?.ToString());
        }

        ///<summary>Writes a line to the console while retaining the cursor's original X coordinate</summary>
        public static void WriteLine(object text = null)
        {
            Console.WriteLine(text?.ToString());
            Console.CursorLeft = lastCursorX;
        }

        ///<summary>Writes text at the horiztonal centre of the console buffer</summary>
        public static void WriteAtCentre(object text)
        {
            int centre = (int)Math.Floor((double)(Console.BufferWidth / 2) - (text.ToString().Length / 2));
            Console.SetCursorPosition(centre, Console.CursorTop);
            Console.WriteLine(text.ToString());
        }
    }
}