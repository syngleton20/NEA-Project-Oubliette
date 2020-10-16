using NEA_Project_Oubliette.Maps;

namespace NEA_Project_Oubliette.Entities
{
    ///<summary>Entity representing the player character</summary>
    internal sealed class Player : Entity, IDamageable
    {
        public static Player Instance { get; private set; }

        public int Health { get; private set; } = 100;
        public int MaxHealth { get; private set; } = 100;
        public bool IsDead { get; private set; }

        public Player(int startX, int startY) : base()
        {
            Instance = this;
            position = new Vector(startX, startY);

            if(Game.Current.CurrentMap.TryGetTile(position.X, position.Y, out Tile tile))
                tile.Occupy(this);
        }

        public override void Move(int deltaX, int deltaY)
        {
            base.Move(deltaX, deltaY);
            Game.Current.SetCameraPosition(position.X / Maps.Map.AREA_SIZE, position.Y / Maps.Map.AREA_SIZE);
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
            if(!IsDead)
            {
                IsDead = true;
                Game.Current.GameOver();
            }
        }
    }
}