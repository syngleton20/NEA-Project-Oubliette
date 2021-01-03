using NEA_Project_Oubliette.Maps;
using System;

namespace NEA_Project_Oubliette.Entities
{
    ///<summary>Base class for all mobile objects within a map</summary>
    internal abstract class Entity
    {
        protected int id;
        protected Vector position;

        public int Id => id;
        public Vector Position => position;

        public Entity() { }
        public Entity(string data) => Load(data);

        ///<summary>Moves the entity on the X and Y axes</summary>
        public virtual void Move(int deltaX, int deltaY)
        {
            if(Game.Current.CurrentMap.TryGetTile(position.X + deltaX, position.Y + deltaY, out Tile tile))
            {
                if(tile.IsWalkable && !tile.IsOccupied)
                {
                    if(Game.Current.CurrentMap.TryGetTile(position.X, position.Y, out Tile oldTile))
                        oldTile.Vacate();

                    position += new Vector(deltaX, deltaY);
                    tile.Occupy(this);
                }
            }
        }

        ///<summary>Draws the visual representation of the entity</summary>
        public virtual void Draw()
        {
            Console.Write("██");
            Console.ResetColor();
        }

        ///<summary>Performs entity-specific action after being pushed in a direction</summary>
        public virtual void Push(int deltaX, int deltaY, Entity pusher) { }

        ///<summary>Performs entity-specific action after receiving player input</summary>
        public virtual void Update() { }

        ///<summary>Performs final actions before the entity is removed from a collection</summary>
        public virtual void OnDestroy()
        {
            if(Game.Current.CurrentMap.TryGetTile(position, out Tile tile))
                tile.Vacate();
        }

        ///<summary>Sets entity-specific information after loading from a file</summary>
        public abstract void Load(string data);

        ///<summary>Provides save data in string format for saving to a file</summary>
        public abstract string Save();
    }
}