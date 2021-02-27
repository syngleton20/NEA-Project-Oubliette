using System.Collections.Generic;
using System;

namespace NEA_Project_Oubliette.AStar
{
    ///<summary>Provides the shortest path from one point to another using the A* pathfinding algorithm</summary>
    internal static class Pathfinder
    {
        private static Grid currentGrid;

        ///<summary>Returns the absolute distance from one node to another</summary>
        private static int GetDistance(Node nodeA, Node nodeB)
        {
            int distanceX = Math.Abs(nodeA.POSITION.X - nodeB.POSITION.X), distanceY = Math.Abs(nodeA.POSITION.Y - nodeB.POSITION.Y); // These local variables calculate the absolute distance from node A to node B

            // The following if statement returns a distance based on the maximum length of either the x or y
            // distance, depending on which is lowest, in order to determine a diagonal distance for the distance
            // from node A to be aligned with node B. 14 is the diagonal move cost for each tile and 10
            // is the horizontal or vertical move cost for each tile
            if(distanceX > distanceY) return 14 * distanceY + 10 * (distanceX - distanceY);
            else return 14 * distanceX + 10 * (distanceY - distanceX);
        }

        ///<summary>Retraces a path backwards then reverses and returns an array as a result</summary>
        private static Node[] RetracePath(Node startNode, Node targetNode)
        {
            List<Node> path = new List<Node>();
            Node currentNode = targetNode;

            // Continuously cycles through the parent node of each node and adds it to the list of nodes
            // until the current node is the start node
            while(currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.Parent;
            }

            path.Reverse(); // Flips the order of the array
            return path.ToArray();
        }

        ///<summary>Calculates the shortest path from one position to another</summary>
        public static Node[] FindPath(Vector start, Vector target, Grid grid)
        {
            currentGrid = grid;
            Node startNode = grid.GetNode(start), targetNode = grid.GetNode(target);

            List<Node> openSet = new List<Node>();
            HashSet<Node> closedSet = new HashSet<Node>();

            openSet.Add(startNode);

            while(openSet.Count > 0)
            {
                Node currentNode = openSet[0];

                // Assigns to the current node if the node at the index of i's F cost is less than the
                // current node's F cost, or their F costs are equal, but the current node's H cost is lower
                for(int i = 1; i < openSet.Count; i++)
                    if(openSet[i].FCost < currentNode.FCost || openSet[i].FCost == currentNode.FCost && openSet[i].HCost < currentNode.HCost)
                        currentNode = openSet[i];

                // Takes the current node from the open set and adds it to the closed set
                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                if(currentNode == targetNode) return RetracePath(startNode, targetNode);

                foreach(Node neighbour in grid.GetNeighbours(currentNode))
                {
                    if(!neighbour.IsWalkable || closedSet.Contains(neighbour)) continue; // Skips this iteration if the neighbour node is not walkable or is part of the closed set
                    int newMovementCost = currentNode.GCost + GetDistance(currentNode, neighbour);

                    // Updates the costs and parent of the current neighbour node, then adds the neighbour to the open set
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