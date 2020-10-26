using System;
using NEA_Project_Oubliette.Maps;

namespace NEA_Project_Oubliette.Entities
{
    ///<summary>Base class for all hostile entities which will follow and attack the player entity</summary>
    internal class Enemy : Entity, IDamageable
    {
        public Enemy(int startX, int startY) => position = new Vector(startX, startY);
        public Enemy(string data) : base(data) => Load(data);

        public int Health { get; private set; } = 20;
        public int MaxHealth { get; private set; } = 20;
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

        public override void Load(string data)
        {
            string[] parts = data.Split(' ');
            id = parts[1].ToInt();
            position = parts[2].ToVector();
            Health = int.Parse(parts[3]);
        }

        public override string Save() => $"E {id} {position.ToString()} {Health}";
    }
}