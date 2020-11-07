using NEA_Project_Oubliette.Maps;

namespace NEA_Project_Oubliette.AStar
{
    ///<summary>Provides pathfinding information linked to a single tile</summary>
    internal sealed class Node
    {
        private readonly Tile TILE;

        public int GCost;
        public int HCost;

        public int FCost => GCost + HCost;
        public bool IsWalkable => TILE.IsWalkable;

        public Node Parent;
        public readonly Vector POSITION;

        public Node(Tile tile)
        {
            TILE = tile;
            POSITION = TILE.Position;
        }
    }
}