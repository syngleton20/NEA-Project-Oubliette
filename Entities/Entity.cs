using NEA_Project_Oubliette.Maps;
using System;

namespace NEA_Project_Oubliette.Entities
{
    ///<summary>Base class for all mobile objects within a map</summary>
    internal abstract class Entity
    {
        protected Vector position;

        public Vector Position => position;

        public Entity() { }

        public virtual void Move(int deltaX, int deltaY)
        {
            if(Game.Current.CurrentMap.TryGetTile(position.X + deltaX, position.Y + deltaY, out Tile tile))
                if(tile.Ascii == '.') position += new Vector(deltaX, deltaY);
        }

        public virtual void Draw()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("██");
            Console.ResetColor();
        }
    }
}