// COM 5113 Sample Code - Nick Mitchell 2025

using System;

namespace PathFinderAssessment
{
    internal class DepthFirst : PathFinderInterface
    {
        // Finds a path from start to goal using
        // Depth First Search (DFS).
        public bool FindPath(
            int[,] map,
            Coord start,
            Coord goal,
            ref LinkedList<Coord> path)
        {
            // OPEN stores discovered nodes that are waiting
            // to be expanded.
            //
            // DFS uses a Stack because it follows
            // Last-In, First-Out (LIFO) behaviour.
            Stack<SearchNode> open = new Stack<SearchNode>();

            // CLOSED stores nodes that have already been expanded.
            Stack<SearchNode> closed = new Stack<SearchNode>();

            // Tracks coordinates that have already been discovered.
            // This prevents the same coordinate being added repeatedly.
            bool[,] visited = new bool[
                map.GetLength(0),
                map.GetLength(1)
            ];

            // Create the starting search node.
            SearchNode startNode = new SearchNode(
                start,
                0,
                0,
                null
            );

            // Add the starting node to OPEN.
            open.Push(startNode);

            // Mark the starting coordinate as discovered.
            visited[start.Row, start.Col] = true;


            // Continue while there are still nodes to explore.
            while (!open.IsEmpty())
            {
                // DFS removes the most recently added node.
                SearchNode current = open.Pop();


                // Check whether the current node is the goal.
                if (current.Position.Row == goal.Row &&
                    current.Position.Col == goal.Col)
                {
                    // Reconstruct the path using predecessor links.
                    path = SearchUtilities.buildPathList(current);

                    return true;
                }


                // -----------------------------------------------------
                // IMPORTANT:
                //
                // The lecturer requires normal processing order:
                //
                // North → East → South → West
                //
                // However, because DFS uses a Stack and PushFront(),
                // successors must be ADDED in reverse order:
                //
                // West → South → East → North
                //
                // This means North will be the first successor
                // removed from the Stack.
                // -----------------------------------------------------


                // WEST
                TryAddSuccessor(
                    map,
                    current.Position.Row,
                    current.Position.Col - 1,
                    current,
                    open,
                    visited
                );


                // SOUTH
                TryAddSuccessor(
                    map,
                    current.Position.Row + 1,
                    current.Position.Col,
                    current,
                    open,
                    visited
                );


                // EAST
                TryAddSuccessor(
                    map,
                    current.Position.Row,
                    current.Position.Col + 1,
                    current,
                    open,
                    visited
                );


                // NORTH
                TryAddSuccessor(
                    map,
                    current.Position.Row - 1,
                    current.Position.Col,
                    current,
                    open,
                    visited
                );


                // Move the expanded node into CLOSED.
                closed.Push(current);
            }


            // OPEN became empty before the goal was found.
            path = new LinkedList<Coord>();

            return false;
        }


        // -------------------------------------------------------------
        // Creates and adds a valid successor to the DFS stack.
        // -------------------------------------------------------------
        private void TryAddSuccessor(
            int[,] map,
            int row,
            int col,
            SearchNode current,
            Stack<SearchNode> open,
            bool[,] visited)
        {
            // Ignore coordinates outside the map.
            if (!IsInsideMap(map, row, col))
            {
                return;
            }


            // Terrain value 0 represents a blocked cell.
            if (map[row, col] == 0)
            {
                return;
            }


            // Ignore coordinates that have already been discovered.
            if (visited[row, col])
            {
                return;
            }


            // Create the coordinate for the successor.
            Coord successorPosition = new Coord(
                row,
                col
            );


            // Create the SearchNode.
            //
            // DFS does not use terrain cost or heuristic values
            // when choosing which node to expand.
            //
            // The current node becomes the predecessor so that
            // the final route can be reconstructed.
            SearchNode successor = new SearchNode(
                successorPosition,
                0,
                0,
                current
            );


            // Add the successor to the top/front of OPEN.
            open.Push(successor);


            // Mark it as discovered immediately.
            visited[row, col] = true;
        }


        // -------------------------------------------------------------
        // Checks whether a coordinate is inside the map boundaries.
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