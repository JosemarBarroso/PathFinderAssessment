// COM 5113 Sample Code - Nick Mitchell 2025

using System;

namespace PathFinderAssessment
{
    internal class BreadthFirst :
        PathFinderInterface,
        SteppablePathFinderInterface
    {
        // =============================================================
        // STEP-BY-STEP SEARCH STATE
        // =============================================================

        private int[,]? stepMap;

        private Coord stepStart;
        private Coord stepGoal;

        private Queue<SearchNode>? stepOpen;
        private Queue<SearchNode>? stepClosed;

        private bool[,]? stepVisited;

        // Separate coordinate lists are maintained for the GUI.
        private LinkedList<Coord>? stepOpenCoordinates;
        private LinkedList<Coord>? stepClosedCoordinates;

        private bool stepInitialised;
        private bool stepComplete;
        private bool stepPathFound;

        private LinkedList<Coord>? stepFinalPath;


        // =============================================================
        // NORMAL COMPLETE BFS
        // =============================================================

        // Finds a path from the start coordinate to the goal coordinate
        // using Breadth First Search (BFS).
        public bool FindPath(
            int[,] map,
            Coord start,
            Coord goal,
            ref LinkedList<Coord> path)
        {
            // OPEN contains nodes that have been discovered
            // but have not yet been expanded.
            //
            // BFS uses a Queue because it follows FIFO ordering.
            Queue<SearchNode> open =
                new Queue<SearchNode>();

            // CLOSED contains nodes that have already been expanded.
            Queue<SearchNode> closed =
                new Queue<SearchNode>();

            // Store whether each coordinate has already been discovered.
            //
            // This prevents the same grid location from being added to
            // OPEN more than once.
            bool[,] visited =
                new bool[
                    map.GetLength(0),
                    map.GetLength(1)
                ];


            // Create the first SearchNode from the starting coordinate.
            SearchNode startNode =
                new SearchNode(
                    start,
                    0,
                    0,
                    null);


            // Add the starting node to OPEN.
            open.Enqueue(startNode);

            // Mark the starting coordinate as discovered.
            visited[start.Row, start.Col] = true;


            // Keep searching while OPEN still contains nodes.
            while (!open.IsEmpty())
            {
                // BFS always removes the oldest node from OPEN.
                SearchNode current =
                    open.Dequeue();


                // Check whether the current node is the goal.
                if (current.Position.Row == goal.Row &&
                    current.Position.Col == goal.Col)
                {
                    path =
                        SearchUtilities.buildPathList(
                            current);

                    return true;
                }


                // -----------------------------------------------------
                // Generate successors:
                //
                // North
                // East
                // South
                // West
                // -----------------------------------------------------

                // NORTH
                TryAddSuccessor(
                    map,
                    current.Position.Row - 1,
                    current.Position.Col,
                    current,
                    open,
                    visited);


                // EAST
                TryAddSuccessor(
                    map,
                    current.Position.Row,
                    current.Position.Col + 1,
                    current,
                    open,
                    visited);


                // SOUTH
                TryAddSuccessor(
                    map,
                    current.Position.Row + 1,
                    current.Position.Col,
                    current,
                    open,
                    visited);


                // WEST
                TryAddSuccessor(
                    map,
                    current.Position.Row,
                    current.Position.Col - 1,
                    current,
                    open,
                    visited);


                // Move current to CLOSED.
                closed.Enqueue(current);
            }


            // OPEN became empty before reaching the goal.
            path =
                new LinkedList<Coord>();

            return false;
        }


        // =============================================================
        // INITIALISE STEP-BY-STEP BFS
        // =============================================================

        public void InitialiseStepSearch(
            int[,] map,
            Coord start,
            Coord goal)
        {
            stepMap = map;

            stepStart = start;
            stepGoal = goal;


            // Create fresh OPEN and CLOSED structures.
            stepOpen =
                new Queue<SearchNode>();

            stepClosed =
                new Queue<SearchNode>();


            // Create visited structure.
            stepVisited =
                new bool[
                    map.GetLength(0),
                    map.GetLength(1)
                ];


            // Coordinate lists are used by the GUI visualiser.
            stepOpenCoordinates =
                new LinkedList<Coord>();

            stepClosedCoordinates =
                new LinkedList<Coord>();


            // Create start node.
            SearchNode startNode =
                new SearchNode(
                    start,
                    0,
                    0,
                    null);


            // Add start node to OPEN.
            stepOpen.Enqueue(startNode);

            stepOpenCoordinates.PushBack(start);


            // Mark start as discovered.
            stepVisited[start.Row, start.Col] =
                true;


            // Reset search status.
            stepInitialised = true;
            stepComplete = false;
            stepPathFound = false;

            stepFinalPath = null;
        }


        // =============================================================
        // EXECUTE ONE BFS EXPANSION
        // =============================================================

        public SearchStepResult Step()
        {
            // Search must be initialised before Step() can be used.
            if (!stepInitialised ||
                stepMap == null ||
                stepOpen == null ||
                stepClosed == null ||
                stepVisited == null ||
                stepOpenCoordinates == null ||
                stepClosedCoordinates == null)
            {
                throw new InvalidOperationException(
                    "Step search has not been initialised.");
            }


            // If the search already finished, simply return
            // its final state.
            if (stepComplete)
            {
                return new SearchStepResult(
                    null,
                    CopyCoordinateList(
                        stepOpenCoordinates),
                    CopyCoordinateList(
                        stepClosedCoordinates),
                    true,
                    stepPathFound,
                    stepFinalPath);
            }


            // No nodes remain in OPEN.
            if (stepOpen.IsEmpty())
            {
                stepComplete = true;
                stepPathFound = false;

                return new SearchStepResult(
                    null,
                    CopyCoordinateList(
                        stepOpenCoordinates),
                    CopyCoordinateList(
                        stepClosedCoordinates),
                    true,
                    false,
                    null);
            }


            // ---------------------------------------------------------
            // REMOVE ONE NODE FROM OPEN
            // ---------------------------------------------------------

            SearchNode current =
                stepOpen.Dequeue();


            // Remove its coordinate from the GUI OPEN list.
            stepOpenCoordinates.Remove(
                current.Position);


            // ---------------------------------------------------------
            // CHECK GOAL
            // ---------------------------------------------------------

            if (current.Position.Row == stepGoal.Row &&
                current.Position.Col == stepGoal.Col)
            {
                stepComplete = true;
                stepPathFound = true;

                stepFinalPath =
                    SearchUtilities.buildPathList(
                        current);


                return new SearchStepResult(
                    current.Position,
                    CopyCoordinateList(
                        stepOpenCoordinates),
                    CopyCoordinateList(
                        stepClosedCoordinates),
                    true,
                    true,
                    stepFinalPath);
            }


            // ---------------------------------------------------------
            // EXPAND CURRENT NODE IN N/E/S/W ORDER
            // ---------------------------------------------------------

            // NORTH
            TryAddStepSuccessor(
                current.Position.Row - 1,
                current.Position.Col,
                current);


            // EAST
            TryAddStepSuccessor(
                current.Position.Row,
                current.Position.Col + 1,
                current);


            // SOUTH
            TryAddStepSuccessor(
                current.Position.Row + 1,
                current.Position.Col,
                current);


            // WEST
            TryAddStepSuccessor(
                current.Position.Row,
                current.Position.Col - 1,
                current);


            // ---------------------------------------------------------
            // MOVE CURRENT NODE TO CLOSED
            // ---------------------------------------------------------

            stepClosed.Enqueue(current);

            stepClosedCoordinates.PushBack(
                current.Position);


            // If nothing remains in OPEN after this expansion,
            // then no path exists.
            if (stepOpen.IsEmpty())
            {
                stepComplete = true;
                stepPathFound = false;
            }


            // Return a snapshot of the current search state.
            return new SearchStepResult(
                current.Position,
                CopyCoordinateList(
                    stepOpenCoordinates),
                CopyCoordinateList(
                    stepClosedCoordinates),
                stepComplete,
                stepPathFound,
                stepFinalPath);
        }


        // =============================================================
        // NORMAL BFS SUCCESSOR HELPER
        // =============================================================

        private void TryAddSuccessor(
            int[,] map,
            int row,
            int col,
            SearchNode current,
            Queue<SearchNode> open,
            bool[,] visited)
        {
            // Coordinate must be inside the map.
            if (!IsInsideMap(
                map,
                row,
                col))
            {
                return;
            }


            // Terrain 0 is blocked.
            if (map[row, col] == 0)
            {
                return;
            }


            // Do not rediscover an existing coordinate.
            if (visited[row, col])
            {
                return;
            }


            Coord successorPosition =
                new Coord(
                    row,
                    col);


            SearchNode successor =
                new SearchNode(
                    successorPosition,
                    0,
                    0,
                    current);


            open.Enqueue(
                successor);


            visited[row, col] =
                true;
        }


        // =============================================================
        // STEP BFS SUCCESSOR HELPER
        // =============================================================

        private void TryAddStepSuccessor(
            int row,
            int col,
            SearchNode current)
        {
            // These have already been checked by Step(),
            // but the null checks keep this method safe.
            if (stepMap == null ||
                stepOpen == null ||
                stepVisited == null ||
                stepOpenCoordinates == null)
            {
                return;
            }


            // Coordinate must be inside the map.
            if (!IsInsideMap(
                stepMap,
                row,
                col))
            {
                return;
            }


            // Terrain 0 is blocked.
            if (stepMap[row, col] == 0)
            {
                return;
            }


            // Already discovered.
            if (stepVisited[row, col])
            {
                return;
            }


            Coord successorPosition =
                new Coord(
                    row,
                    col);


            SearchNode successor =
                new SearchNode(
                    successorPosition,
                    0,
                    0,
                    current);


            // Add successor to the actual BFS OPEN queue.
            stepOpen.Enqueue(
                successor);


            // Add successor to the GUI OPEN list.
            stepOpenCoordinates.PushBack(
                successorPosition);


            // Mark immediately to avoid duplicates.
            stepVisited[row, col] =
                true;
        }


        // =============================================================
        // COPY COORDINATE LIST
        // =============================================================

        // A copy is returned to the GUI rather than exposing the
        // algorithm's internal Open/Closed lists directly.
        private LinkedList<Coord> CopyCoordinateList(
            LinkedList<Coord> source)
        {
            LinkedList<Coord> copy =
                new LinkedList<Coord>();


            source.ForEach(
                coordinate =>
                {
                    copy.PushBack(
                        coordinate);
                });


            return copy;
        }


        // =============================================================
        // MAP BOUNDARY CHECK
        // =============================================================

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