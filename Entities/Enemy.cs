using System;
using NEA_Project_Oubliette.Maps;

namespace NEA_Project_Oubliette.Entities
{
    internal class Enemy : Entity, IDamageable
    {
        public Enemy(int startX, int startY)
        {
            position = new Vector(startX, startY);

            if(Game.Current.CurrentMap.TryGetTile(position.X, position.Y, out Tile tile))
                tile.Occupy(this);
        }

        public int Health { get; private set; } = 100;
        public int MaxHealth { get; private set; } = 100;
        public bool IsDead { get; private set; }


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
                    Player.Instance.TakeDamage(10);
            }
        }

        public void Heal(int amount = 1)
        {
            Health += amount;
            if(Health > MaxHealth) Health = MaxHealth;
        }

        public void TakeDamage(int amount = 1)
        {
            Health -= amount;
            if(Health <= 0 && !IsDead) Die();
        }

        public void Die()
        {
            IsDead = true;
            Game.Current.CurrentMap.Collection.Remove(this);
        }
    }
}