using System.Text;
using System;

namespace NEA_Project_Oubliette
{
    ///<summary>Provides visual interface tools to improve ease-of-use for users</summary>
    internal static class GUI
    {
        ///<summary>Writes a title followed by a horizontal separator</summary>
        public static void Title(string title)
        {
            Console.Clear();
            Console.ResetColor();

            Console.CursorTop++;
            Display.WriteAtCentre(title);
            HorizontalSeparator();
        }

        ///<summary>Draws a line separating different sections of GUI</summary>
        public static void HorizontalSeparator()
        {
            Console.Write("  ");

            for (int i = 2; i < (Environment.OSVersion.Platform == PlatformID.Win32NT ? Console.BufferWidth - 1 : Console.BufferWidth - 3); i++)
                Console.Write('─');

            Console.WriteLine('─');
            if(Environment.OSVersion.Platform == PlatformID.Unix) Console.WriteLine();
        }

        ///<summary>Displays a value with a minimum and maximum value in gauge format</summary>
        public static void HorizontalBar(int currentValue, int maxValue, string label, ConsoleColor colour = ConsoleColor.DarkRed)
        {
            Console.WriteLine("  " + label);
            Console.Write("  ");

            Console.ForegroundColor = colour;

            for (int i = 0; i < currentValue; i++)
                Console.Write('█');

            for (int i = currentValue; i < maxValue; i++)
                Console.Write('░');

            Console.ResetColor();
        }

        ///<summary>Shows a large number of items in a vertical scroll view</summary>
        public static void VerticalScrollView(string[] items)
        {
            int startX = Console.CursorLeft, startY = Console.CursorTop;

            for (int i = 0; i < items.Length; i++)
            {
                Console.WriteLine("  " + items[i]);

                if(Console.CursorTop >= (Console.BufferHeight - 4))
                {
                    Console.WriteLine();
                    Display.WriteAtCentre("Press any key to continue... ");
                    Input.GetKeyDown();

                    Console.SetCursorPosition(startX, startY);

                    for (int j = 0; j < Console.CursorTop; j++)
                    {
                        Display.ClearLine();
                        Console.WriteLine();
                    }

                    Console.SetCursorPosition(startX, startY);
                }
            }
        }

        ///<summary>Draws a vertical list menu which returns the selected index</summary>
        public static int VerticalMenu(params string[] choices)
        {
            Console.ResetColor();

            int startX = Console.CursorLeft, startY = Console.CursorTop;
            int choiceIndex = 0;

            bool hasChosen = false;

            do
            {
                Console.SetCursorPosition(startX, startY);

                for (int i = 0; i < choices.Length; i++)
                {
                    Console.ResetColor();
                    Console.Write("  ");

                    if (i == choiceIndex) Console.BackgroundColor = ConsoleColor.Red;

                    Console.Write(choices[i]);

                    for (int j = Console.CursorLeft; j < (Environment.OSVersion.Platform == PlatformID.Win32NT ? Console.BufferWidth : Console.BufferWidth - 2); j++)
                        Console.Write(' ');

                    if(Environment.OSVersion.Platform == PlatformID.Unix) Console.WriteLine();
                }

                Console.ResetColor();

                switch (Input.GetKeyDown())
                {
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.W:
                        if (choiceIndex > 0) choiceIndex--;
                        else choiceIndex = (choices.Length - 1);
                        break;

                    case ConsoleKey.DownArrow:
                    case ConsoleKey.S:
                        if (choiceIndex < (choices.Length - 1)) choiceIndex++;
                        else choiceIndex = 0;
                        break;

                    case ConsoleKey.Spacebar:
                    case ConsoleKey.Enter:
                        hasChosen = true;
                        break;
                }
            }
            while (!hasChosen);

            Console.Clear();
            return choiceIndex;
        }

        ///<summary>Draws a horizontal list menu which returns the selected index</summary>
        public static int HorizontalMenu(params string[] choices)
        {
            Console.ResetColor();

            int startX = Console.CursorLeft, startY = Console.CursorTop;
            int choiceIndex = 0;

            bool hasChosen = false;

            do
            {
                Console.SetCursorPosition(startX, startY);
                Console.Write("  ");

                for (int i = 0; i < choices.Length; i++)
                {
                    Console.ResetColor();

                    if (i == choiceIndex) Console.BackgroundColor = ConsoleColor.Red;

                    Console.Write(' ' + choices[i] + ' ');
                }

                Console.ResetColor();

                switch (Input.GetKeyDown())
                {
                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.A:
                        if (choiceIndex > 0) choiceIndex--;
                        else choiceIndex = (choices.Length - 1);
                        break;

                    case ConsoleKey.RightArrow:
                    case ConsoleKey.D:
                        if (choiceIndex < (choices.Length - 1)) choiceIndex++;
                        else choiceIndex = 0;
                        break;

                    case ConsoleKey.Spacebar:
                    case ConsoleKey.Enter:
                        hasChosen = true;
                        break;
                }
            }
            while (!hasChosen);

            Console.Clear();
            return choiceIndex;
        }

        ///<summary>Displays a vertical menu containing 'YES' and 'NO', and returns a boolean value based on user input</summary>
        public static bool YesOrNo(string question)
        {
            Console.ResetColor();

            if(question.Contains('\n'))
            {
                string[] lines = question.Split('\n');

                for (int i = 0; i < lines.Length; i++)
                    Display.WriteAtCentre(lines[i]);
            }
            else Display.WriteAtCentre(question);

            Console.WriteLine();

            return VerticalMenu("YES", "NO") == 0;
        }

        public static string TextField(string label, int maxLength, string defaultValue = "")
        {
            int startX = Console.CursorLeft, startY = Console.CursorTop, cursorPosition = defaultValue.Length;
            bool hasFinished = false;

            StringBuilder text = new StringBuilder();

            do
            {
                Display.ClearLine();
                Console.CursorVisible = true;

                Console.Write("  " + label);
                Console.Write(text.ToString());

                Console.CursorLeft -= text.Length;
                Console.CursorLeft += cursorPosition;

                switch (Input.GetKeyDown())
                {
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.UpArrow:
                        break;

                    case ConsoleKey.LeftArrow:
                        if(cursorPosition > 0) cursorPosition--;
                        break;

                    case ConsoleKey.RightArrow:
                        if(cursorPosition < (text.Length - 1)) cursorPosition++;
                        break;

                    case ConsoleKey.Escape:
                        Console.CursorVisible = false;
                        Display.Clear();
                        return "";

                    case ConsoleKey.Enter:
                        hasFinished = true;
                        break;

                    case ConsoleKey.Backspace:
                        if(text.Length <= 1){
                            text.Clear();
                            cursorPosition = 0;
                            break;
                        }

                        if(cursorPosition > 0 && cursorPosition < (text.Length - 1)) text.Remove(cursorPosition, 1);
                        else
                        {
                            if(cursorPosition <= 0)
                            {
                                text.Remove(0, 1);
                                cursorPosition = 0;
                            }

                            if(cursorPosition >= (text.Length - 1)) text.Remove(text.Length - 1, 1);
                        }

                        if(cursorPosition > 0) cursorPosition--;
                        break;

                    default:
                        if(text.Length >= maxLength) break;

                        if(text.Length <= 0)
                        {
                            text.Append(Input.KeyChar);
                            cursorPosition++;
                            break;
                        }

                        if(cursorPosition > 0 && cursorPosition < (text.Length - 1)) text.Insert(cursorPosition, Input.KeyChar);
                        else
                        {
                            if(cursorPosition <= 0) text.Insert(0, Input.KeyChar);
                            if(cursorPosition >= (text.Length - 1)) text.Append(Input.KeyChar);
                        }

                        cursorPosition++;
                        break;
                }

                Console.CursorVisible = false;
                Console.SetCursorPosition(startX, startY);
            }
            while(!hasFinished);

            Console.CursorVisible = false;
            Display.Clear();

            return text.ToString();
        }
    }
}