// COM 5113 Pathfinding Assessment

using System;

namespace PathFinderAssessment
{
    internal class AStar : PathFinderInterface
    {
        // Stores the number of times the Open List
        // was placed into priority order during the
        // most recent search.
        public int OpenListSortCount { get; private set; }


        // Finds a lowest-cost path using A* Search.
        public bool FindPath(
            int[,] map,
            Coord start,
            Coord goal,
            ref LinkedList<Coord> path)
        {
            // Reset the count whenever a new search begins.
            OpenListSortCount = 0;


            // ---------------------------------------------------------
            // OPEN
            //
            // A* orders OPEN using:
            //
            // f(n) = g(n) + h(n)
            //
            // Cost     = g(n)
            // Score    = h(n)
            // Estimate = g(n) + h(n)
            // ---------------------------------------------------------
            PriorityQueue<SearchNode> open =
                new PriorityQueue<SearchNode>(
                    (first, second) =>
                        first.Estimate.CompareTo(second.Estimate)
                );


            // CLOSED contains nodes that have already
            // been expanded.
            LinkedList<SearchNode> closed =
                new LinkedList<SearchNode>();


            // Calculate the heuristic for the starting node.
            int startHeuristic =
                SearchUtilities.ManhattanDistance(
                    start,
                    goal
                );


            // Create the starting node.
            SearchNode startNode =
                new SearchNode(
                    start,
                    0,
                    startHeuristic,
                    null
                );


            // Add start to OPEN.
            open.Enqueue(startNode);


            // Continue searching until OPEN is empty.
            while (!open.IsEmpty())
            {
                // Remove the node with the smallest
                // estimated total path cost.
                SearchNode current =
                    open.Dequeue();


                // -----------------------------------------------------
                // Goal test
                // -----------------------------------------------------
                if (current.Position.Row == goal.Row &&
                    current.Position.Col == goal.Col)
                {
                    // Build the final route using predecessor links.
                    path =
                        SearchUtilities.buildPathList(current);

                    // Store the Open List ordering count.
                    OpenListSortCount =
                        open.SortCount;

                    return true;
                }


                // -----------------------------------------------------
                // Generate successors in normal rule order:
                //
                // North → East → South → West
                // -----------------------------------------------------


                // NORTH
                TryProcessSuccessor(
                    map,
                    current.Position.Row - 1,
                    current.Position.Col,
                    current,
                    goal,
                    open,
                    closed
                );


                // EAST
                TryProcessSuccessor(
                    map,
                    current.Position.Row,
                    current.Position.Col + 1,
                    current,
                    goal,
                    open,
                    closed
                );


                // SOUTH
                TryProcessSuccessor(
                    map,
                    current.Position.Row + 1,
                    current.Position.Col,
                    current,
                    goal,
                    open,
                    closed
                );


                // WEST
                TryProcessSuccessor(
                    map,
                    current.Position.Row,
                    current.Position.Col - 1,
                    current,
                    goal,
                    open,
                    closed
                );


                // Once Current has been expanded,
                // place it into CLOSED.
                closed.PushBack(current);
            }


            // OPEN became empty before reaching the goal.
            path = new LinkedList<Coord>();

            OpenListSortCount =
                open.SortCount;

            return false;
        }


        // -------------------------------------------------------------
        // Processes one possible A* successor.
        // -------------------------------------------------------------
        private void TryProcessSuccessor(
            int[,] map,
            int row,
            int col,
            SearchNode current,
            Coord goal,
            PriorityQueue<SearchNode> open,
            LinkedList<SearchNode> closed)
        {
            // ---------------------------------------------------------
            // Rule 1:
            // Ignore coordinates outside the map.
            // ---------------------------------------------------------
            if (!IsInsideMap(map, row, col))
            {
                return;
            }


            // ---------------------------------------------------------
            // Rule 2:
            // Terrain value 0 represents a wall.
            // ---------------------------------------------------------
            if (map[row, col] == 0)
            {
                return;
            }


            Coord successorPosition =
                new Coord(row, col);


            // ---------------------------------------------------------
            // Calculate the new accumulated cost.
            //
            // Terrain values:
            //
            // 1 = open terrain
            // 2 = wood
            // 3 = water
            // ---------------------------------------------------------
            int movementCost =
                map[row, col];


            int newCost =
                current.Cost + movementCost;


            // ---------------------------------------------------------
            // Look for the same coordinate in OPEN.
            // ---------------------------------------------------------
            SearchNode? openNode =
                open.Find(node =>
                    node.Position.Row == row &&
                    node.Position.Col == col
                );


            // ---------------------------------------------------------
            // Look for the same coordinate in CLOSED.
            // ---------------------------------------------------------
            SearchNode? closedNode =
                closed.Find(node =>
                    node.Position.Row == row &&
                    node.Position.Col == col
                );


            // The coordinate may already exist in either list.
            SearchNode? existingNode =
                openNode ?? closedNode;


            // ---------------------------------------------------------
            // Lecturer A* rule:
            //
            // If n is already on OPEN or CLOSED and
            //
            // newCost >= existing cost
            //
            // then ignore this new route.
            // ---------------------------------------------------------
            if (existingNode != null &&
                newCost >= existingNode.Cost)
            {
                return;
            }


            // Calculate Manhattan heuristic h(n).
            int heuristic =
                SearchUtilities.ManhattanDistance(
                    successorPosition,
                    goal
                );


            // ---------------------------------------------------------
            // CASE 1:
            //
            // Successor already exists but this is a cheaper route.
            // ---------------------------------------------------------
            if (existingNode != null)
            {
                // If it currently belongs to OPEN,
                // remove it before changing its values.
                if (openNode != null)
                {
                    open.Remove(existingNode);
                }


                // If it currently belongs to CLOSED,
                // remove it from CLOSED.
                //
                // The lecturer requires a CLOSED node to be
                // reopened when a better route is discovered.
                if (closedNode != null)
                {
                    closed.Remove(existingNode);
                }


                // Update the node using the cheaper route.
                existingNode.Cost =
                    newCost;

                existingNode.Score =
                    heuristic;

                existingNode.Predecessor =
                    current;


                // Put the improved node back onto OPEN.
                //
                // InsertSorted will restore the correct
                // f(n) priority ordering.
                open.Enqueue(existingNode);

                return;
            }


            // ---------------------------------------------------------
            // CASE 2:
            //
            // The coordinate exists on neither OPEN nor CLOSED.
            // Create a new node.
            // ---------------------------------------------------------
            SearchNode successor =
                new SearchNode(
                    successorPosition,
                    newCost,
                    heuristic,
                    current
                );


            // Add the new node to OPEN according to:
            //
            // Estimate = Cost + Score
            //          = g(n) + h(n)
            open.Enqueue(successor);
        }


        // -------------------------------------------------------------
        // Checks whether a coordinate is inside the terrain map.
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