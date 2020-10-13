using System;

namespace NEA_Project_Oubliette
{
    ///<summary>Part of a map which can contain one occupant entity</summary>
    internal class Tile
    {
        private bool isWalkable;
        private char ascii;

        public bool IsWalkable => isWalkable;
        public char Ascii => ascii;

        public Tile(char ascii)
        {
            this.ascii = ascii;
            isWalkable = true;
        }

        ///<summary>Draws the tile in Unicode form</summary>
        public void Draw()
        {
            Console.Write(ascii + ascii); // Writes double ascii characters until TileSet is introduced in the next commit
        }
    }
}