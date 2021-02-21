using NEA_Project_Oubliette.Maps;
using System.Text;
using System;

namespace NEA_Project_Oubliette
{
    ///<summary>Methods for writing non-contiguous or column-based text to the console</summary>
    internal static class Display
    {
        private static int lastCursorX = -1;
        private static int offsetX = (int)Math.Floor((double)(Console.BufferWidth / 2) - Map.AREA_SIZE);

        public static Vector Offset => new Vector(offsetX, 4);
        public static Vector CursorPosition => new Vector(Console.CursorLeft, Console.CursorTop);

        ///<summary>Clears the console buffer and resets all necessary values</summary>
        public static void Clear()
        {
            Console.ResetColor();
            Console.Clear();
            Console.SetCursorPosition(0, 0);

            Console.CursorVisible = false;
        }

        ///<summary>Clears a line in the console of any characters</summary>
        public static void ClearLine()
        {
            Console.ResetColor();
            Console.CursorLeft = 0;

            for(int i = 0; i < Console.BufferWidth; i++)
                Console.Write(' ');

            Console.CursorTop--;
        }

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

        ///<summary>Writes horizontally-centred text at the bottom of the console buffer</summary>
        public static void WriteAtCentreBottom(string text)
        {
            string[] lines = text.Split('\n');

            for(int i = 0; i < lines.Length; i++)
            {
                int centre = (int)Math.Floor((double)(Console.BufferWidth / 2) - (lines[i].Length / 2));
                Console.ForegroundColor = ConsoleColor.DarkGray;

                Console.SetCursorPosition(centre, Console.BufferHeight - 2 - (lines.Length - i) + 1);
                Console.WriteLine(lines[i]);
                Console.ResetColor();
            }
        }

        ///<summary>Displays horizonally-centred text in a 'ribbon' drawn over whatever was underneath</summary>
        public static void ShowRibbonMessage(object text)
        {
            Console.SetCursorPosition(0, Offset.Y + ((Map.AREA_SIZE / 2) - 2));

            for(int i = 0; i < 3; i++)
            {
                ClearLine();
                Console.WriteLine();
            }

            Console.CursorTop -= 2;
            WriteAtCentre(text);
        }

        ///<summary>Draws a map at the centre of the screen using a predefined offset vector</summary>
        public static void DrawMap(Map map, int drawX, int drawY)
        {
            int centre = (int)Math.Floor((double)(Console.BufferWidth / 2) - Map.AREA_SIZE);
            Console.SetCursorPosition(centre, 4);
            map.Draw(drawX, drawY);
        }

        public static void Banner()
        {
            Display.Clear();
            Console.WriteLine();
            Console.WriteLine("     ███   █   █  █████  █      █████  █████  █████████████  █████\n    ██ ██  █  ╒█  █╜ │█  █       ╙█ │  █ │     │██ │ │╙██    █╯  │\n    █│  █  █  ╞█  █  │█  █        █ │  █╒╛     │█    │ ╰█    █    \n    █   █  █  │█  ████   █        █    █████   │█    │  █    █████\n    █  ╭█  █  │█  █╫╯ █  █╮       █    █╯       █    │  █    █ │  \n    ██ ██  ██ ██  █╜ ╭█  █┼╮     ╭█    █        █       █    █    \n     ███   ╰███   █████  █████  █████  █████    █       █    █████");
        }

        ///<summary>Returns a string separated into two parts over the entire width of the console buffer</summary>
        public static string SplitStringOverBufferWidth(string leftPart, string rightPart)
        {
            StringBuilder splitString = new StringBuilder();

            splitString.Append(leftPart);
            splitString.Append(' ');

            for(int i = splitString.Length; i < (Console.BufferWidth - rightPart.Length) - (Environment.OSVersion.Platform == PlatformID.Win32NT ? 4 : 5); i++) splitString.Append('.');

            splitString.Append(' ');
            splitString.Append(rightPart);

            return splitString.ToString();
        }
    }
}