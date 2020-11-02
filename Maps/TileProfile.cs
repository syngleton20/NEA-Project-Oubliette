using System;

namespace NEA_Project_Oubliette
{
    ///<summary>Stores information on the visual appearance of a tile</summary>
    internal sealed class TileProfile
    {
        private char ascii;
        private string unicode;
        private bool isWalkable;

        private ConsoleColor backgroundColour;
        private ConsoleColor foregroundColour;

        public char Ascii => ascii;
        public string Unicode => unicode;
        public bool IsWalkable => isWalkable;

        public ConsoleColor BackgroundColour => backgroundColour;
        public ConsoleColor ForegroundColour => foregroundColour;

        public TileProfile(char ascii, string unicode, bool isWalkable)
        {
            this.ascii = ascii;
            this.unicode = unicode;
            this.isWalkable = isWalkable;

            this.backgroundColour = ConsoleColor.Black;
            this.foregroundColour = ConsoleColor.DarkGray;
        }

        public TileProfile(char ascii, string unicode, bool isWalkable, ConsoleColor backgroundColour, ConsoleColor foregroundColour)
        {
            this.ascii = ascii;
            this.unicode = unicode;
            this.isWalkable = isWalkable;
            this.backgroundColour = backgroundColour;
            this.foregroundColour = foregroundColour;
        }

        ///<summary>Draws a tile in Unicode form with the correct Background and Foreground colours</summary>
        public void Draw()
        {
            Console.BackgroundColor = backgroundColour;
            Console.ForegroundColor = foregroundColour;

            Display.Write(unicode);
        }
    }
}