namespace NEA_Project_Oubliette.AStar
{
    internal sealed class Node
    {
        private bool isWalkable;
        private Vector position;

        public int GCost { get; set; }
        public int HCost { get; set; }

        public int FCost => GCost + HCost;

        public bool IsWalkable => isWalkable;

        public Vector Position => position;
        public Node Parent { get; set; }

        public Node(bool isWalkable, Vector position)
        {
            this.isWalkable = isWalkable;
            this.position = position;
        }
    }
}