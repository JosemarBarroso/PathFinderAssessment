// COM 5113 Pathfinding Assessment

using System;

namespace PathFinderAssessment
{
    internal class Dijkstra : PathFinderInterface
    {
        // Finds the lowest-cost path from start to goal
        // using Dijkstra's Search.
        public bool FindPath(
            int[,] map,
            Coord start,
            Coord goal,
            ref LinkedList<Coord> path)
        {
            // OPEN is ordered using accumulated path cost.
            // Smaller Cost means higher priority.
            PriorityQueue<SearchNode> open =
                new PriorityQueue<SearchNode>(
                    (first, second) =>
                        first.Cost.CompareTo(second.Cost)
                );


            // The start node has a path cost of zero
            // and no predecessor.
            SearchNode startNode = new SearchNode(
                start,
                0,
                0,
                null
            );


            // Add the starting node to OPEN.
            open.Enqueue(startNode);


            // Continue searching until OPEN becomes empty.
            while (!open.IsEmpty())
            {
                // Remove the node with the smallest path cost.
                SearchNode current = open.Dequeue();


                // Check whether the goal has been reached.
                if (current.Position.Row == goal.Row &&
                    current.Position.Col == goal.Col)
                {
                    path =
                        SearchUtilities.buildPathList(current);

                    return true;
                }


                // Generate successors in normal order:
                //
                // North → East → South → West


                // NORTH
                TryProcessSuccessor(
                    map,
                    current.Position.Row - 1,
                    current.Position.Col,
                    current,
                    open
                );


                // EAST
                TryProcessSuccessor(
                    map,
                    current.Position.Row,
                    current.Position.Col + 1,
                    current,
                    open
                );


                // SOUTH
                TryProcessSuccessor(
                    map,
                    current.Position.Row + 1,
                    current.Position.Col,
                    current,
                    open
                );


                // WEST
                TryProcessSuccessor(
                    map,
                    current.Position.Row,
                    current.Position.Col - 1,
                    current,
                    open
                );
            }


            // No route to the goal was found.
            path = new LinkedList<Coord>();

            return false;
        }


        // -------------------------------------------------------------
        // Processes one possible successor.
        // -------------------------------------------------------------
        private void TryProcessSuccessor(
            int[,] map,
            int row,
            int col,
            SearchNode current,
            PriorityQueue<SearchNode> open)
        {
            // Ignore coordinates outside the map.
            if (!IsInsideMap(map, row, col))
            {
                return;
            }


            // Terrain value 0 is not traversable.
            if (map[row, col] == 0)
            {
                return;
            }


            Coord successorPosition =
                new Coord(row, col);


            // The cost of entering the successor cell is
            // determined by its terrain value:
            //
            // 1 = open terrain
            // 2 = wood
            // 3 = water
            int movementCost = map[row, col];


            // Dijkstra:
            //
            // newCost =
            // current cost + cost of moving to successor.
            int newCost =
                current.Cost + movementCost;


            // Look for this coordinate already on OPEN.
            SearchNode? existingNode =
                open.Find(node =>
                    node.Position.Row == row &&
                    node.Position.Col == col
                );


            // ---------------------------------------------------------
            // If the successor already exists on OPEN:
            // ---------------------------------------------------------
            if (existingNode != null)
            {
                // If the existing route is already as cheap
                // or cheaper, ignore the new route.
                if (newCost >= existingNode.Cost)
                {
                    return;
                }


                // A cheaper route has been found.
                //
                // Remove the existing node before changing its cost
                // because its position in the PriorityQueue was based
                // on the old value.
                open.Remove(existingNode);


                // Update the existing SearchNode.
                existingNode.Cost = newCost;
                existingNode.Predecessor = current;


                // Reinsert it so OPEN is correctly ordered again.
                open.Enqueue(existingNode);

                return;
            }


            // ---------------------------------------------------------
            // The coordinate is not currently on OPEN.
            // Create a new SearchNode.
            // ---------------------------------------------------------
            SearchNode successor =
                new SearchNode(
                    successorPosition,
                    newCost,
                    0,
                    current
                );


            // Add it to OPEN according to accumulated cost.
            open.Enqueue(successor);
        }


        // -------------------------------------------------------------
        // Checks map boundaries.
        // -------------------------------------------------------------
        private bool IsInsideMap(
            int[,] map,
            int row,
            int col)
        {
            return row >= 0 &&
                   row < map.GetLength(0) &&
                   col >= 0 &&
                   col < map.GetLength(1);
        }
    }
}