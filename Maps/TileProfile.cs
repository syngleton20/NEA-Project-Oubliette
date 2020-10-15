using System;

namespace NEA_Project_Oubliette
{
    ///<summary>Stores information on the visual appearance of a tile</summary>
    internal sealed class TileProfile
    {
        private char ascii;
        private string unicode;

        private ConsoleColor backgroundColour;
        private ConsoleColor foregroundColour;

        public char Ascii => ascii;
        public string Unicode => unicode;

        public ConsoleColor BackgroundColour => backgroundColour;
        public ConsoleColor ForegroundColour => foregroundColour;

        public TileProfile(string rawProfile)
        {
            string[] parts = rawProfile.Split(',');

            ascii = parts[0][0];
            unicode = parts[1];

            if(parts.Length > 2)
            {
                backgroundColour = (ConsoleColor)int.Parse(parts[2]);
                foregroundColour = (ConsoleColor)int.Parse(parts[3]);
            }
            else
            {
                backgroundColour = ConsoleColor.Black;
                foregroundColour = ConsoleColor.Gray;
            }
        }

        ///<summary>Draws a tile in Unicode form with the correct Background and Foreground colours</summary>
        public void Draw()
        {
            Console.BackgroundColor = backgroundColour;
            Console.ForegroundColor = foregroundColour;
            Console.Write(unicode);
        }
    }
}