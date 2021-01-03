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

        public Enemy(int startX, int startY) => position = new Vector(startX, startY);
        public Enemy(string data) : base(data) => Load(data);

        public int Health { get; private set; } = 20;
        public int MaxHealth { get; private set; } = 20;
        public bool IsDead { get; private set; }

        public override void Draw()
        {
            if(!takingDamage) Console.ForegroundColor = hasSeenPlayer ? ConsoleColor.Red : ConsoleColor.DarkRed;
            base.Draw();
        }

        public override void Update()
        {
            if(hasSeenPlayer)
            {
                foreach (Tile tile in Game.Current.CurrentMap.GetNeighbouringTiles(position))
                    if(tile.IsOccupied)
                        if(tile.Occupant.GetType() == typeof(Player))
                            Player.Instance.TakeDamage(strength);

                if(currentPath.Length <= 0 || Player.Instance.Position != lastPlayerPosition)
                {
                    lastPlayerPosition = Player.Instance.Position;
                    currentPath = Pathfinder.FindPath(position, Player.Instance.Position, Game.Current.CurrentMap.Grid);
                    currentNodeIndex = 0;
                }

                if(currentPath.Length > 0)
                {
                    if(currentNodeIndex < (currentPath.Length - 1))
                    {
                        Vector movement = (currentPath[currentNodeIndex].POSITION - position).Normalise();
                        Move(movement.X, movement.Y);

                        if(position == currentPath[currentNodeIndex].POSITION) currentNodeIndex++;
                    }
                }
            }
            else if((Player.Instance.Position / Map.AREA_SIZE) == (Position / Map.AREA_SIZE))
                hasSeenPlayer = true;
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
                Console.ForegroundColor = ConsoleColor.White;
                Draw();

                Thread.Sleep(100);
                takingDamage = false;

                Console.SetCursorPosition(Display.Offset.X + (position.X * 2) % (Map.AREA_SIZE * 2), Display.Offset.Y + (position.Y % Map.AREA_SIZE));
                Console.ForegroundColor = ConsoleColor.Red;
                Draw();
            }
        }

        public void Die()
        {
            IsDead = true;

            if(Game.Current.CurrentMap.TryGetTile(position, out Tile tile))
                tile.Vacate();

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