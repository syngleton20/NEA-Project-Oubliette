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

        public Tile(char ascii)
        {
            this.ascii = ascii;

            profile = TileSet.GetProfileFromAscii(ascii);
            isWalkable = profile.IsWalkable;
        }

        ///<summary>Draws the tile in Unicode form</summary>
        public void Draw() => profile.Draw();

        ///<summary>Assigns an entity to this tile</summary>
        public void Occupy(Entity entity) => occupant = entity;

        ///<summary>Removes the occupant entity from this tile</summary>
        public void Vacate() => occupant = null;

        public override string ToString() => profile.Unicode;
    }
}