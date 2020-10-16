using NEA_Project_Oubliette.Maps;

namespace NEA_Project_Oubliette.Entities
{
    ///<summary>Entity representing the player character</summary>
    internal sealed class Player : Entity
    {
        public static Player Instance { get; private set; }

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
    }
}