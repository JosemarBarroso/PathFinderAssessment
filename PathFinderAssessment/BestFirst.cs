// COM 5113 Pathfinding Assessment

using System;

namespace PathFinderAssessment
{
    internal class BestFirst : PathFinderInterface
    {
        // Finds a path from start to goal using
        // Best First Search.
        public bool FindPath(
            int[,] map,
            Coord start,
            Coord goal,
            ref LinkedList<Coord> path)
        {
            // OPEN is ordered by heuristic score.
            // Lower heuristic value means higher priority.
            PriorityQueue<SearchNode> open =
                new PriorityQueue<SearchNode>(
                    (first, second) =>
                        first.Score.CompareTo(second.Score)
                );

            // CLOSED stores nodes that have already been expanded.
            LinkedList<SearchNode> closed =
                new LinkedList<SearchNode>();

            // Tracks whether a coordinate has already been discovered.
            bool[,] visited = new bool[
                map.GetLength(0),
                map.GetLength(1)
            ];

            // Calculate heuristic for the start node.
            int startHeuristic =
                SearchUtilities.ManhattanDistance(start, goal);

            // Create the start SearchNode.
            SearchNode startNode = new SearchNode(
                start,
                0,
                startHeuristic,
                null
            );

            // Add start node to OPEN.
            open.Enqueue(startNode);

            // Mark the start coordinate as discovered.
            visited[start.Row, start.Col] = true;


            // Continue until OPEN becomes empty.
            while (!open.IsEmpty())
            {
                // Remove the node with the lowest heuristic score.
                SearchNode current = open.Dequeue();


                // Check whether we have reached the goal.
                if (current.Position.Row == goal.Row &&
                    current.Position.Col == goal.Col)
                {
                    path =
                        SearchUtilities.buildPathList(current);

                    return true;
                }


                // Generate successors in the required order:
                //
                // North → East → South → West


                // NORTH
                TryAddSuccessor(
                    map,
                    current.Position.Row - 1,
                    current.Position.Col,
                    current,
                    goal,
                    open,
                    visited
                );


                // EAST
                TryAddSuccessor(
                    map,
                    current.Position.Row,
                    current.Position.Col + 1,
                    current,
                    goal,
                    open,
                    visited
                );


                // SOUTH
                TryAddSuccessor(
                    map,
                    current.Position.Row + 1,
                    current.Position.Col,
                    current,
                    goal,
                    open,
                    visited
                );


                // WEST
                TryAddSuccessor(
                    map,
                    current.Position.Row,
                    current.Position.Col - 1,
                    current,
                    goal,
                    open,
                    visited
                );


                // Add the expanded node to CLOSED.
                closed.PushBack(current);
            }


            // No path was found.
            path = new LinkedList<Coord>();

            return false;
        }


        // Creates a valid successor and adds it to OPEN.
        private void TryAddSuccessor(
            int[,] map,
            int row,
            int col,
            SearchNode current,
            Coord goal,
            PriorityQueue<SearchNode> open,
            bool[,] visited)
        {
            // Ignore coordinates outside the map.
            if (!IsInsideMap(map, row, col))
            {
                return;
            }


            // Terrain value 0 is blocked.
            if (map[row, col] == 0)
            {
                return;
            }


            // Do not add coordinates already discovered.
            if (visited[row, col])
            {
                return;
            }


            // Create the successor coordinate.
            Coord successorPosition =
                new Coord(row, col);


            // Best First Search uses only the heuristic
            // when deciding which node to expand next.
            int heuristic =
                SearchUtilities.ManhattanDistance(
                    successorPosition,
                    goal
                );


            // Create the successor node.
            SearchNode successor =
                new SearchNode(
                    successorPosition,
                    0,
                    heuristic,
                    current
                );


            // Add successor to OPEN in heuristic order.
            open.Enqueue(successor);


            // Mark the coordinate as discovered.
            visited[row, col] = true;
        }


        // Checks whether a coordinate lies inside the map.
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