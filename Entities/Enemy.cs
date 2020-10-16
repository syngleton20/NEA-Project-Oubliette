using System;
using NEA_Project_Oubliette.Maps;

namespace NEA_Project_Oubliette.Entities
{
    internal class Enemy : Entity
    {
        public Enemy(int startX, int startY) : base() => position = new Vector(startX, startY);

        public override void Draw()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            base.Draw();
        }

        public override void Update()
        {
            foreach (Tile tile in Game.Current.CurrentMap.GetNeighbouringTiles(position.X, position.Y))
            {
                if(tile.Occupant == Player.Instance)
                {
                    // Hurt player
                }
            }
        }
    }
}