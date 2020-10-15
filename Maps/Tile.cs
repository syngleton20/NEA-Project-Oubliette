using System;

namespace NEA_Project_Oubliette.Maps
{
    ///<summary>Part of a map which can contain one occupant entity</summary>
    internal class Tile
    {
        private bool isWalkable;
        private char ascii;

        private TileProfile profile;

        public bool IsWalkable => isWalkable;
        public char Ascii => ascii;

        public Tile(char ascii, TileSet tileSet)
        {
            this.ascii = ascii;

            profile = tileSet.GetProfileFromAscii(ascii);
            isWalkable = true;
        }

        ///<summary>Draws the tile in Unicode form</summary>
        public void Draw() => profile.Draw();
    }
}