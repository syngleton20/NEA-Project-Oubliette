using System.Runtime.InteropServices;
using System.Text;
using System;

namespace NEA_Project_Oubliette
{
    ///<summary>Controls the size and style of the game window</summary>
    internal static class Window
    {
        /*

        I found out about and implemented the macOS native system() function from here:
        https://stackoverflow.com/questions/31522500/mono-resize-terminal-on-mac-os-x.
        I have modified the system() function call in ResizeWindow() to suit my needs.

        */

        [DllImport("libc")]
        private static extern int system(string exec);

        public const int DEFAULT_WIDTH = 68;
        public const int DEFAULT_HEIGHT = 36;

        ///<summary>Called from the Main() method in the Program class</summary>
        public static void Setup()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.Title = "Project Oubliette";

            Console.TreatControlCAsInput = true;
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
            else system($@"printf '\e[8;{height};{width}t'");
        }
    }
}