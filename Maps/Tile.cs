using NEA_Project_Oubliette.Entities;
using System;

namespace NEA_Project_Oubliette.Maps
{
    ///<summary>Part of a map which can contain one occupant entity</summary>
    internal class Tile
    {
        private bool isWalkable;
        private char ascii;

        private TileProfile profile;
        private Entity occupant;

        public bool IsWalkable => isWalkable && !IsOccupied;
        public bool IsOccupied => occupant != null;
        public char Ascii => ascii;

        public Entity Occupant => occupant;

        public Tile(char ascii, TileSet tileSet)
        {
            this.ascii = ascii;

            profile = tileSet.GetProfileFromAscii(ascii);
            isWalkable = true;
        }

        ///<summary>Draws the tile in Unicode form</summary>
        public void Draw() => profile.Draw();

        ///<summary>Assigns an entity to this tile</summary>
        public void Occupy(Entity entity) => occupant = entity;

        ///<summary>Removes the occupant entity from this tile</summary>
        public void Vacate() => occupant = null;
    }
}