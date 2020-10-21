using System;

namespace NEA_Project_Oubliette
{
    ///<summary>Controls the size and style of the game window</summary>
    internal static class Window
    {
        public const int DEFAULT_WIDTH = 54;
        public const int DEFAULT_HEIGHT = 28;

        ///<summary>Called from the Main() method in the Program class</summary>
        public static void Setup()
        {
            Console.Title = "Project Oubliette";
            Console.CursorVisible = false;

            Console.ResetColor();
            Console.Clear();

            ResizeWindow(DEFAULT_WIDTH, DEFAULT_HEIGHT);
        }

        ///<summary>Changes the width and height of the game window</summary>
        public static void ResizeWindow(int width, int height)
        {
            if(Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                Console.SetWindowSize(width, height);
                Console.SetBufferSize(width, height);
            }
        }
    }
}