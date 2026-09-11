// COM 5113 Pathfinding Assessment

using System;

namespace PathFinderAssessment
{
    internal class AStar :
        PathFinderInterface,
        SteppablePathFinderInterface
    {
        // -------------------------------------------------------------
        // Stores the number of times OPEN was ordered during
        // the most recent A* search.
        // -------------------------------------------------------------
        public int OpenListSortCount { get; private set; }


        // =============================================================
        // STEP-BY-STEP SEARCH STATE
        // =============================================================

        private int[,]? stepMap;

        private Coord stepStart;
        private Coord stepGoal;

        private PriorityQueue<SearchNode>? stepOpen;

        private LinkedList<SearchNode>? stepClosed;

        private LinkedList<Coord>? stepOpenCoordinates;
        private LinkedList<Coord>? stepClosedCoordinates;

        private bool stepInitialised;
        private bool stepComplete;
        private bool stepPathFound;

        private LinkedList<Coord>? stepFinalPath;


        // =============================================================
        // NORMAL A* SEARCH
        // =============================================================

        public bool FindPath(
            int[,] map,
            Coord start,
            Coord goal,
            ref LinkedList<Coord> path)
        {
            // Reset count for a new search.
            OpenListSortCount = 0;


            // ---------------------------------------------------------
            // OPEN
            //
            // A* priority:
            //
            // f(n) = g(n) + h(n)
            //
            // Cost     = g(n)
            // Score    = h(n)
            // Estimate = Cost + Score
            // ---------------------------------------------------------
            PriorityQueue<SearchNode> open =
                new PriorityQueue<SearchNode>(
                    (first, second) =>
                        first.Estimate.CompareTo(
                            second.Estimate)
                );


            // CLOSED stores already-expanded nodes.
            LinkedList<SearchNode> closed =
                new LinkedList<SearchNode>();


            // ---------------------------------------------------------
            // START NODE
            // ---------------------------------------------------------
            int startHeuristic =
                SearchUtilities.ManhattanDistance(
                    start,
                    goal);


            SearchNode startNode =
                new SearchNode(
                    start,
                    0,
                    startHeuristic,
                    null);


            open.Enqueue(
                startNode);


            // ---------------------------------------------------------
            // SEARCH LOOP
            // ---------------------------------------------------------
            while (!open.IsEmpty())
            {
                SearchNode current =
                    open.Dequeue();


                // -----------------------------------------------------
                // GOAL CHECK
                // -----------------------------------------------------
                if (current.Position.Row ==
                    goal.Row &&
                    current.Position.Col ==
                    goal.Col)
                {
                    path =
                        SearchUtilities.buildPathList(
                            current);


                    OpenListSortCount =
                        open.SortCount;


                    return true;
                }


                // -----------------------------------------------------
                // SUCCESSORS
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
                    closed);


                // EAST
                TryProcessSuccessor(
                    map,
                    current.Position.Row,
                    current.Position.Col + 1,
                    current,
                    goal,
                    open,
                    closed);


                // SOUTH
                TryProcessSuccessor(
                    map,
                    current.Position.Row + 1,
                    current.Position.Col,
                    current,
                    goal,
                    open,
                    closed);


                // WEST
                TryProcessSuccessor(
                    map,
                    current.Position.Row,
                    current.Position.Col - 1,
                    current,
                    goal,
                    open,
                    closed);


                // Move expanded node to CLOSED.
                closed.PushBack(
                    current);
            }


            // No path found.
            path =
                new LinkedList<Coord>();


            OpenListSortCount =
                open.SortCount;


            return false;
        }


        // =============================================================
        // NORMAL A* SUCCESSOR PROCESSING
        // =============================================================

        private void TryProcessSuccessor(
            int[,] map,
            int row,
            int col,
            SearchNode current,
            Coord goal,
            PriorityQueue<SearchNode> open,
            LinkedList<SearchNode> closed)
        {
            // Outside terrain.
            if (!IsInsideMap(
                map,
                row,
                col))
            {
                return;
            }


            // Terrain 0 = blocked.
            if (map[
                row,
                col] == 0)
            {
                return;
            }


            Coord successorPosition =
                new Coord(
                    row,
                    col);


            // Terrain cost of entering successor.
            int movementCost =
                map[
                    row,
                    col];


            int newCost =
                current.Cost +
                movementCost;


            // ---------------------------------------------------------
            // FIND SAME COORDINATE IN OPEN
            // ---------------------------------------------------------
            SearchNode? openNode =
                open.Find(
                    node =>
                        node.Position.Row == row &&
                        node.Position.Col == col);


            // ---------------------------------------------------------
            // FIND SAME COORDINATE IN CLOSED
            // ---------------------------------------------------------
            SearchNode? closedNode =
                closed.Find(
                    node =>
                        node.Position.Row == row &&
                        node.Position.Col == col);


            SearchNode? existingNode =
                openNode ??
                closedNode;


            // ---------------------------------------------------------
            // EXISTING ROUTE IS ALREADY AS GOOD OR BETTER
            // ---------------------------------------------------------
            if (existingNode != null &&
                newCost >= existingNode.Cost)
            {
                return;
            }


            int heuristic =
                SearchUtilities.ManhattanDistance(
                    successorPosition,
                    goal);


            // ---------------------------------------------------------
            // EXISTING NODE WITH CHEAPER ROUTE FOUND
            // ---------------------------------------------------------
            if (existingNode != null)
            {
                // If currently in OPEN, remove before updating.
                if (openNode != null)
                {
                    open.Remove(
                        existingNode);
                }


                // If currently in CLOSED, reopen it.
                if (closedNode != null)
                {
                    closed.Remove(
                        existingNode);
                }


                existingNode.Cost =
                    newCost;


                existingNode.Score =
                    heuristic;


                existingNode.Predecessor =
                    current;


                // Reinsert according to new f(n).
                open.Enqueue(
                    existingNode);


                return;
            }


            // ---------------------------------------------------------
            // NEW SUCCESSOR
            // ---------------------------------------------------------
            SearchNode successor =
                new SearchNode(
                    successorPosition,
                    newCost,
                    heuristic,
                    current);


            open.Enqueue(
                successor);
        }


        // =============================================================
        // INITIALISE STEP-BY-STEP A*
        // =============================================================

        public void InitialiseStepSearch(
            int[,] map,
            Coord start,
            Coord goal)
        {
            stepMap =
                map;


            stepStart =
                start;


            stepGoal =
                goal;


            OpenListSortCount =
                0;


            stepOpen =
                new PriorityQueue<SearchNode>(
                    (first, second) =>
                        first.Estimate.CompareTo(
                            second.Estimate)
                );


            stepClosed =
                new LinkedList<SearchNode>();


            stepOpenCoordinates =
                new LinkedList<Coord>();


            stepClosedCoordinates =
                new LinkedList<Coord>();


            int startHeuristic =
                SearchUtilities.ManhattanDistance(
                    start,
                    goal);


            SearchNode startNode =
                new SearchNode(
                    start,
                    0,
                    startHeuristic,
                    null);


            stepOpen.Enqueue(
                startNode);


            stepOpenCoordinates.PushBack(
                start);


            OpenListSortCount =
                stepOpen.SortCount;


            stepInitialised =
                true;


            stepComplete =
                false;


            stepPathFound =
                false;


            stepFinalPath =
                null;
        }


        // =============================================================
        // EXECUTE ONE A* SEARCH STEP
        // =============================================================

        public SearchStepResult Step()
        {
            if (!stepInitialised ||
                stepMap == null ||
                stepOpen == null ||
                stepClosed == null ||
                stepOpenCoordinates == null ||
                stepClosedCoordinates == null)
            {
                throw new InvalidOperationException(
                    "Step search has not been initialised.");
            }


            // ---------------------------------------------------------
            // ALREADY FINISHED
            // ---------------------------------------------------------
            if (stepComplete)
            {
                OpenListSortCount =
                    stepOpen.SortCount;


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


            // ---------------------------------------------------------
            // OPEN EMPTY
            // ---------------------------------------------------------
            if (stepOpen.IsEmpty())
            {
                stepComplete =
                    true;


                stepPathFound =
                    false;


                OpenListSortCount =
                    stepOpen.SortCount;


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
            // REMOVE NODE WITH LOWEST f(n)
            // ---------------------------------------------------------
            SearchNode current =
                stepOpen.Dequeue();


            stepOpenCoordinates.Remove(
                current.Position);


            // ---------------------------------------------------------
            // GOAL CHECK
            // ---------------------------------------------------------
            if (current.Position.Row ==
                stepGoal.Row &&
                current.Position.Col ==
                stepGoal.Col)
            {
                stepComplete =
                    true;


                stepPathFound =
                    true;


                stepFinalPath =
                    SearchUtilities.buildPathList(
                        current);


                OpenListSortCount =
                    stepOpen.SortCount;


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
            // SUCCESSORS
            //
            // North → East → South → West
            // ---------------------------------------------------------

            // NORTH
            TryProcessStepSuccessor(
                current.Position.Row - 1,
                current.Position.Col,
                current);


            // EAST
            TryProcessStepSuccessor(
                current.Position.Row,
                current.Position.Col + 1,
                current);


            // SOUTH
            TryProcessStepSuccessor(
                current.Position.Row + 1,
                current.Position.Col,
                current);


            // WEST
            TryProcessStepSuccessor(
                current.Position.Row,
                current.Position.Col - 1,
                current);


            // ---------------------------------------------------------
            // MOVE CURRENT TO CLOSED
            // ---------------------------------------------------------
            stepClosed.PushBack(
                current);


            stepClosedCoordinates.PushBack(
                current.Position);


            // ---------------------------------------------------------
            // UPDATE SORT COUNT
            // ---------------------------------------------------------
            OpenListSortCount =
                stepOpen.SortCount;


            // ---------------------------------------------------------
            // FAILURE CHECK
            // ---------------------------------------------------------
            if (stepOpen.IsEmpty())
            {
                stepComplete =
                    true;


                stepPathFound =
                    false;
            }


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
        // STEP SUCCESSOR PROCESSING
        // =============================================================

        private void TryProcessStepSuccessor(
            int row,
            int col,
            SearchNode current)
        {
            if (stepMap == null ||
                stepOpen == null ||
                stepClosed == null ||
                stepOpenCoordinates == null ||
                stepClosedCoordinates == null)
            {
                return;
            }


            // Outside map.
            if (!IsInsideMap(
                stepMap,
                row,
                col))
            {
                return;
            }


            // Blocked.
            if (stepMap[
                row,
                col] == 0)
            {
                return;
            }


            Coord successorPosition =
                new Coord(
                    row,
                    col);


            int movementCost =
                stepMap[
                    row,
                    col];


            int newCost =
                current.Cost +
                movementCost;


            // ---------------------------------------------------------
            // FIND IN OPEN
            // ---------------------------------------------------------
            SearchNode? openNode =
                stepOpen.Find(
                    node =>
                        node.Position.Row == row &&
                        node.Position.Col == col);


            // ---------------------------------------------------------
            // FIND IN CLOSED
            // ---------------------------------------------------------
            SearchNode? closedNode =
                stepClosed.Find(
                    node =>
                        node.Position.Row == row &&
                        node.Position.Col == col);


            SearchNode? existingNode =
                openNode ??
                closedNode;


            // ---------------------------------------------------------
            // IGNORE IF EXISTING ROUTE IS AS GOOD OR BETTER
            // ---------------------------------------------------------
            if (existingNode != null &&
                newCost >= existingNode.Cost)
            {
                return;
            }


            int heuristic =
                SearchUtilities.ManhattanDistance(
                    successorPosition,
                    stepGoal);


            // ---------------------------------------------------------
            // CHEAPER ROUTE TO EXISTING NODE
            // ---------------------------------------------------------
            if (existingNode != null)
            {
                // -----------------------------------------------------
                // EXISTING NODE IS IN OPEN
                // -----------------------------------------------------
                if (openNode != null)
                {
                    stepOpen.Remove(
                        existingNode);


                    // Coordinate already exists in GUI OPEN list,
                    // so do not add it again.
                }


                // -----------------------------------------------------
                // EXISTING NODE IS IN CLOSED
                //
                // Lecturer A* rule:
                // reopen CLOSED node if cheaper path is found.
                // -----------------------------------------------------
                if (closedNode != null)
                {
                    stepClosed.Remove(
                        existingNode);


                    stepClosedCoordinates.Remove(
                        existingNode.Position);


                    // It is being moved back to OPEN.
                    stepOpenCoordinates.PushBack(
                        existingNode.Position);
                }


                existingNode.Cost =
                    newCost;


                existingNode.Score =
                    heuristic;


                existingNode.Predecessor =
                    current;


                stepOpen.Enqueue(
                    existingNode);


                return;
            }


            // ---------------------------------------------------------
            // BRAND NEW SUCCESSOR
            // ---------------------------------------------------------
            SearchNode successor =
                new SearchNode(
                    successorPosition,
                    newCost,
                    heuristic,
                    current);


            stepOpen.Enqueue(
                successor);


            stepOpenCoordinates.PushBack(
                successorPosition);
        }


        // =============================================================
        // COPY COORDINATE LIST
        // =============================================================

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
        // CHECK MAP BOUNDARIES
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