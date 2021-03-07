using NEA_Project_Oubliette.AStar;
using NEA_Project_Oubliette.Maps;
using System.Threading;
using System;

namespace NEA_Project_Oubliette.Entities
{
    ///<summary>Base class for all hostile entities which will follow and attack the player entity</summary>
    internal class Enemy : Entity, IDamageable
    {
        private int strength = 1, currentNodeIndex;
        private bool hasSeenPlayer, takingDamage;

        private Vector lastPlayerPosition;
        private Node[] currentPath = new Node[0];

        public int Health { get; private set; } = 20;
        public int MaxHealth { get; private set; } = 20;
        public bool IsDead { get; private set; }

        public Enemy(int startX, int startY) => position = new Vector(startX, startY);
        public Enemy(string data) : base(data) => Load(data);

        public override void Draw()
        {
            if(!takingDamage) Console.ForegroundColor = hasSeenPlayer ? ConsoleColor.Red : ConsoleColor.DarkRed; // Enemies turn a lighter shade of red when they have seen and are now pursuing the player
            base.Draw();
        }

        public override void Update()
        {
            if(hasSeenPlayer)
            {
                // Deals damage to the player if they are within a close proximity of this enemy
                if(Game.Current == null) return;

                foreach(Tile tile in Game.Current.CurrentMap.GetNeighbouringTiles(position))
                    if(tile.IsOccupied)
                        if(tile.Occupant.GetType() == typeof(Player))
                            Player.Instance.TakeDamage(strength);

                // Calculates a new path to the player using the A* pathfinding algorithm if no path has been calculated or if the player has moved
                if(currentPath.Length <= 0 || Player.Instance.Position != lastPlayerPosition)
                {
                    lastPlayerPosition = Player.Instance.Position;
                    currentPath = Pathfinder.FindPath(position, Player.Instance.Position, Game.Current.CurrentMap.Grid);
                    currentNodeIndex = 0;
                }

                // Moves this enemy if a path has been calculated
                if(currentPath.Length > 0)
                {
                    if(currentNodeIndex < (currentPath.Length - 1))
                    {
                        Vector movement = (currentPath[currentNodeIndex].POSITION - position).Normalise(); // Clamps the current delta movement's magnitude to 1
                        Move(movement.X, movement.Y);

                        if(position == currentPath[currentNodeIndex].POSITION) currentNodeIndex++;
                    }
                }
            }
            else if((Player.Instance.Position / Map.AREA_SIZE) == (Position / Map.AREA_SIZE)) hasSeenPlayer = true; // CHANGE THIS!
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
                // Causes this enemy to flash white when they take damage
                takingDamage = true;

                Console.SetCursorPosition(Display.Offset.X + (position.X * 2) % (Map.AREA_SIZE * 2), Display.Offset.Y + (position.Y % Map.AREA_SIZE));
                Console.ForegroundColor = ConsoleColor.White;
                Draw();

                Thread.Sleep(100); // Waits for one second
                takingDamage = false;

                // Draws the enemy red again
                Console.SetCursorPosition(Display.Offset.X + (position.X * 2) % (Map.AREA_SIZE * 2), Display.Offset.Y + (position.Y % Map.AREA_SIZE));
                Console.ForegroundColor = ConsoleColor.Red;
                Draw();
            }
        }

        public void Die()
        {
            IsDead = true;
            if(Game.Current.CurrentMap.TryGetTile(position, out Tile tile)) tile.Vacate();

            Game.Current.CurrentMap.Collection.Remove(this);
            Player.Instance.IncrementScore();
        }

        public override void Load(string data)
        {
            string[] parts = data.Split(' ');
            id = parts[1].ToInt();
            position = parts[2].ToVector();
            Health = int.Parse(parts[3]);

            if(parts.Length > 4) hasSeenPlayer = parts[4].ToBool();
        }

        public override string Save() => $"E {id} {position.ToString()} {Health} {hasSeenPlayer.BoolToInt()}";
    }
}