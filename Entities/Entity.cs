using NEA_Project_Oubliette.Maps;
using System;

namespace NEA_Project_Oubliette.Entities
{
    ///<summary>Base class for all mobile objects within a map</summary>
    internal abstract class Entity
    {
        protected Vector position;

        public Vector Position => position;

        public Entity()
        {
            if(Game.Current.CurrentMap.TryGetTile(position.X, position.Y, out Tile tile))
                tile.Occupy(this);
        }

        ///<summary>Moves the entity on the X and Y axes</summary>
        public virtual void Move(int deltaX, int deltaY)
        {
            if(Game.Current.CurrentMap.TryGetTile(position.X + deltaX, position.Y + deltaY, out Tile tile))
                if(tile.Ascii == '.') position += new Vector(deltaX, deltaY);
        }

        ///<summary>Draws the visual representation of the entity</summary>
        public virtual void Draw()
        {
            Console.Write("██");
            Console.ResetColor();
        }

        ///<summary>Performs entity-specific action after receiving player input</summary>
        public virtual void Update() { }
    }
}