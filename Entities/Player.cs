using NEA_Project_Oubliette.Maps;
using System.Threading;
using System;

namespace NEA_Project_Oubliette.Entities
{
    ///<summary>Entity representing the player character</summary>
    internal sealed class Player : Entity, IDamageable
    {
        private int strength = 4;
        private bool takingDamage;

        public int Health { get; private set; } = 20;
        public int MaxHealth { get; private set; } = 20;
        public bool IsDead { get; private set; }

        public static Player Instance { get; private set; }

        public Player(int startX, int startY) : base()
        {
            Instance = this;
            position = new Vector(startX, startY);

            Game.Current.SetCameraPosition(position.X / Map.AREA_SIZE, position.Y / Map.AREA_SIZE);
        }

        public Player(string data) : base(data)
        {
            Instance = this;
            Load(data);

            Game.Current.SetCameraPosition(position.X / Map.AREA_SIZE, position.Y / Map.AREA_SIZE);
        }

        public override void Draw()
        {
            if(!takingDamage) Console.ForegroundColor = ConsoleColor.White;
            base.Draw();
        }

        public override void Move(int deltaX, int deltaY)
        {
            if(Game.Current.CurrentMap.TryGetTile(position.X + deltaX, position.Y + deltaY, out Tile tile))
            {
                if(tile.IsWalkable)
                {
                    if(tile.IsOccupied)
                    {
                        if(tile.Occupant.GetType() == typeof(Enemy))
                            (tile.Occupant as Enemy).TakeDamage(strength);
                    }
                    else
                    {
                        if(Game.Current.CurrentMap.TryGetTile(position.X, position.Y, out Tile oldTile))
                            oldTile.Vacate();

                        position += new Vector(deltaX, deltaY);
                        tile.Occupy(this);
                    }
                }
            }

            Game.Current.SetCameraPosition(position.X / Map.AREA_SIZE, position.Y / Map.AREA_SIZE);
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
            else
            {
                takingDamage = true;

                Console.SetCursorPosition(Display.Offset.X + (position.X * 2) % (Map.AREA_SIZE * 2), Display.Offset.Y + (position.Y % Map.AREA_SIZE));
                Console.ForegroundColor = ConsoleColor.Red;
                Draw();

                Thread.Sleep(100);
                takingDamage = false;

                Console.SetCursorPosition(Display.Offset.X + (position.X * 2) % (Map.AREA_SIZE * 2), Display.Offset.Y + (position.Y % Map.AREA_SIZE));
                Console.ForegroundColor = ConsoleColor.White;
                Draw();
            }
        }

        public void Die() => IsDead = true;

        public override void Load(string data)
        {
            string[] parts = data.Split(' ');
            id = parts[1].ToInt();
            position = parts[2].ToVector();
            Health = int.Parse(parts[3]);
        }

        public override void OnDestroy() => Instance = null;
        public override string Save() => $"P {id} {position.ToString()} {Health}";
    }
}