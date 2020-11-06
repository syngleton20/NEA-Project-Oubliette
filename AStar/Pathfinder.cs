using System.Collections.Generic;
using System;

namespace NEA_Project_Oubliette.AStar
{
    internal static class Pathfinder
    {
        private static Grid currentGrid;

        private static int GetDistance(Node nodeA, Node nodeB)
        {
            int distanceX = Math.Abs(nodeA.Position.X - nodeB.Position.X);
            int distanceY = Math.Abs(nodeA.Position.Y - nodeB.Position.Y);

            if(distanceX > distanceY)
                return 14 * distanceY + 10 * (distanceX - distanceY);

            return 14 * distanceX + 10 * (distanceY - distanceX);
        }

        private static Node[] RetracePath(Node startNode, Node targetNode)
        {
            List<Node> path = new List<Node>();
            Node currentNode = targetNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.Parent;
            }

            path.Reverse();
            return path.ToArray();
        }

        public static Node[] FindPath(Vector start, Vector target, Grid grid)
        {
            currentGrid = grid;
            Node startNode = grid.GetNode(start), targetNode = grid.GetNode(target);

            List<Node> openSet = new List<Node>();
            HashSet<Node> closedSet = new HashSet<Node>();

            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet[0];

                for (int i = 1; i < openSet.Count; i++)
                    if(openSet[i].FCost < currentNode.FCost || openSet[i].FCost == currentNode.FCost && openSet[i].HCost < currentNode.HCost)
                        currentNode = openSet[i];

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                if(currentNode == targetNode)
                    return RetracePath(startNode, targetNode);

                foreach (Node neighbour in grid.GetNeighbours(currentNode))
                {
                    if(!neighbour.IsWalkable || closedSet.Contains(neighbour))
                        continue;

                    int newMovementCost = currentNode.GCost + GetDistance(currentNode, neighbour);

                    if(newMovementCost < neighbour.GCost || !openSet.Contains(neighbour))
                    {
                        neighbour.GCost = newMovementCost;
                        neighbour.HCost = GetDistance(neighbour, targetNode);
                        neighbour.Parent = currentNode;

                        if(!openSet.Contains(neighbour)) openSet.Add(neighbour);
                    }
                }
            }

            return new Node[0];
        }
    }
}