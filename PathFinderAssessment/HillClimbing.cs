// COM 5113 Sample Code - Nick Mitchell 2025

using System;

namespace PathFinderAssessment
{
    internal class HillClimbing : PathFinderInterface
    {
        // Finds a path from start to goal using
        // Hill Climbing Search.
        public bool FindPath(
            int[,] map,
            Coord start,
            Coord goal,
            ref LinkedList<Coord> path)
        {
            // OPEN contains nodes waiting to be expanded.
            //
            // Hill Climbing behaves similarly to DFS because
            // the next preferred node is placed at the front.
            Stack<SearchNode> open = new Stack<SearchNode>();

            // CLOSED contains nodes already expanded.
            Stack<SearchNode> closed = new Stack<SearchNode>();

            // Tracks coordinates already discovered.
            bool[,] visited = new bool[
                map.GetLength(0),
                map.GetLength(1)
            ];

            // Calculate the heuristic value of the start node.
            int startHeuristic =
                SearchUtilities.ManhattanDistance(start, goal);

            // Create the starting node.
            SearchNode startNode = new SearchNode(
                start,
                0,
                startHeuristic,
                null
            );

            // Add start to OPEN.
            open.Push(startNode);

            // Mark start as discovered.
            visited[start.Row, start.Col] = true;


            while (!open.IsEmpty())
            {
                // Remove the node at the front/top of OPEN.
                SearchNode current = open.Pop();


                // Check whether the goal has been reached.
                if (current.Position.Row == goal.Row &&
                    current.Position.Col == goal.Col)
                {
                    path =
                        SearchUtilities.buildPathList(current);

                    return true;
                }


                // Temporary list for the successors generated
                // from the current node.
                LinkedList<SearchNode> tempList =
                    new LinkedList<SearchNode>();


                // -------------------------------------------------
                // Generate successors in normal rule order:
                //
                // North → East → South → West
                //
                // Each successor is inserted into tempList
                // according to its heuristic value.
                // -------------------------------------------------


                // NORTH
                TryAddSuccessor(
                    map,
                    current.Position.Row - 1,
                    current.Position.Col,
                    current,
                    goal,
                    tempList,
                    visited
                );


                // EAST
                TryAddSuccessor(
                    map,
                    current.Position.Row,
                    current.Position.Col + 1,
                    current,
                    goal,
                    tempList,
                    visited
                );


                // SOUTH
                TryAddSuccessor(
                    map,
                    current.Position.Row + 1,
                    current.Position.Col,
                    current,
                    goal,
                    tempList,
                    visited
                );


                // WEST
                TryAddSuccessor(
                    map,
                    current.Position.Row,
                    current.Position.Col - 1,
                    current,
                    goal,
                    tempList,
                    visited
                );


                // -------------------------------------------------
                // tempList is sorted from smallest heuristic
                // to largest heuristic.
                //
                // Because OPEN is a Stack, we must push the
                // worst nodes first and the best node last.
                //
                // That leaves the node with the smallest
                // heuristic at the top of OPEN.
                // -------------------------------------------------

                // tempList is sorted from best heuristic to worst.
                //
                // Remove from the back (worst first) and push onto OPEN.
                // Because Stack.Push adds to the front, the best node
                // will eventually end up at the top of OPEN.
                while (!tempList.IsEmpty())
                {
                    SearchNode node = tempList.PopBack();

                    open.Push(node);
                }


                // Move the current node to CLOSED.
                closed.Push(current);
            }


            // OPEN became empty before the goal was found.
            path = new LinkedList<Coord>();

            return false;
        }


        // -------------------------------------------------------------
        // Generates one valid successor and inserts it into the
        // temporary list according to Manhattan distance.
        // -------------------------------------------------------------
        private void TryAddSuccessor(
            int[,] map,
            int row,
            int col,
            SearchNode current,
            Coord goal,
            LinkedList<SearchNode> tempList,
            bool[,] visited)
        {
            // Ignore coordinates outside the terrain map.
            if (!IsInsideMap(map, row, col))
            {
                return;
            }


            // Terrain value 0 represents a blocked location.
            if (map[row, col] == 0)
            {
                return;
            }


            // Ignore coordinates already discovered.
            if (visited[row, col])
            {
                return;
            }


            Coord successorPosition =
                new Coord(row, col);


            // Hill Climbing evaluates successors using
            // Manhattan distance to the goal.
            int heuristic =
                SearchUtilities.ManhattanDistance(
                    successorPosition,
                    goal
                );


            // Create the successor SearchNode.
            SearchNode successor = new SearchNode(
                successorPosition,
                0,
                heuristic,
                current
            );


            // Insert the successor into tempList in ascending
            // heuristic order.
            //
            // Smaller Score means closer to the goal.
            tempList.InsertSorted(
                successor,
                (first, second) =>
                    first.Score.CompareTo(second.Score)
            );


            // Mark the coordinate as discovered immediately.
            visited[row, col] = true;
        }


        // -------------------------------------------------------------
        // Transfers the sorted temporary list into OPEN.
        // -------------------------------------------------------------
        private void PushTemporaryListOntoOpen(
            LinkedList<SearchNode> tempList,
            Stack<SearchNode> open)
        {
            // We need to place nodes onto the Stack in reverse
            // order because Stack.Push adds to the front.
            //
            // A second Stack gives us that reversal without
            // exposing LinkedList internals.
            Stack<SearchNode> reverseStack =
                new Stack<SearchNode>();


            // tempList is ordered from best to worst.
            tempList.ForEach(node =>
            {
                reverseStack.Push(node);
            });


            // reverseStack now gives us worst to best.
            //
            // Push those onto OPEN so the best node ends up
            // at the top/front and is expanded next.
            while (!reverseStack.IsEmpty())
            {
                open.Push(
                    reverseStack.Pop()
                );
            }
        }


        // -------------------------------------------------------------
        // Checks whether the supplied coordinate is inside the map.
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