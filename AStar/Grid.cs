using NEA_Project_Oubliette.Maps;
using System.Collections.Generic;

namespace NEA_Project_Oubliette.AStar
{
    internal sealed class Grid
    {
        private Vector mapSize;
        private Node[,] nodes;
        private Map map;

        public Grid(Map map, Tile[,] tiles)
        {
            this.map = map;
            mapSize = new Vector(map.Width, map.Height);

            nodes = new Node[mapSize.Y, mapSize.X]; // remember the order!
            Vector bottomLeft = new Vector(0, mapSize.Y);

            for (int y = 0; y < mapSize.Y; y++)
                for (int x = 0; x < mapSize.X; x++)
                    nodes[y, x] = new Node(tiles[y, x].IsWalkable, new Vector(x, y));
        }

        public Node GetNode(Vector position) => nodes[position.Y, position.X];

        public List<Node> GetNeighbours(Node node)
        {
            List<Node> neighbours = new List<Node>();

            for (int y = -1; y <= 1; y++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    if(x == 0 && y == 0) continue;

                    int checkX = node.Position.X + x;
                    int checkY = node.Position.Y + y;

                    if(checkX >= 0 && checkX < mapSize.X && checkY >= 0 && checkY < mapSize.Y)
                        neighbours.Add(nodes[checkY, checkX]);
                }
            }

            return neighbours;
        }
    }
}