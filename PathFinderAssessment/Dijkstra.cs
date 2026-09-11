// COM 5113 Pathfinding Assessment

using System;

namespace PathFinderAssessment
{
    internal class Dijkstra :
        PathFinderInterface,
        SteppablePathFinderInterface
    {
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
        // NORMAL DIJKSTRA SEARCH
        // =============================================================

        public bool FindPath(
            int[,] map,
            Coord start,
            Coord goal,
            ref LinkedList<Coord> path)
        {
            // ---------------------------------------------------------
            // OPEN
            //
            // Ordered by accumulated path cost.
            // Lowest cost has highest priority.
            // ---------------------------------------------------------
            PriorityQueue<SearchNode> open =
                new PriorityQueue<SearchNode>(
                    (first, second) =>
                        first.Cost.CompareTo(
                            second.Cost)
                );


            // ---------------------------------------------------------
            // CLOSED
            //
            // Stores nodes whose minimum cost has already been settled.
            // ---------------------------------------------------------
            LinkedList<SearchNode> closed =
                new LinkedList<SearchNode>();


            // ---------------------------------------------------------
            // START NODE
            // ---------------------------------------------------------
            SearchNode startNode =
                new SearchNode(
                    start,
                    0,
                    0,
                    null);


            open.Enqueue(
                startNode);


            // ---------------------------------------------------------
            // SEARCH LOOP
            // ---------------------------------------------------------
            while (!open.IsEmpty())
            {
                // Remove cheapest node from OPEN.
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

                    return true;
                }


                // -----------------------------------------------------
                // GENERATE SUCCESSORS
                //
                // North → East → South → West
                // -----------------------------------------------------

                // NORTH
                TryProcessSuccessor(
                    map,
                    current.Position.Row - 1,
                    current.Position.Col,
                    current,
                    open,
                    closed);


                // EAST
                TryProcessSuccessor(
                    map,
                    current.Position.Row,
                    current.Position.Col + 1,
                    current,
                    open,
                    closed);


                // SOUTH
                TryProcessSuccessor(
                    map,
                    current.Position.Row + 1,
                    current.Position.Col,
                    current,
                    open,
                    closed);


                // WEST
                TryProcessSuccessor(
                    map,
                    current.Position.Row,
                    current.Position.Col - 1,
                    current,
                    open,
                    closed);


                // Current node is now settled.
                closed.PushBack(
                    current);
            }


            // No path found.
            path =
                new LinkedList<Coord>();


            return false;
        }


        // =============================================================
        // NORMAL DIJKSTRA SUCCESSOR PROCESSING
        // =============================================================

        private void TryProcessSuccessor(
            int[,] map,
            int row,
            int col,
            SearchNode current,
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


            // Terrain 0 is blocked.
            if (map[
                row,
                col] == 0)
            {
                return;
            }


            // ---------------------------------------------------------
            // DO NOT REOPEN A CLOSED NODE
            //
            // With non-negative terrain costs, once Dijkstra removes
            // a node from OPEN, its cheapest cost has been settled.
            // ---------------------------------------------------------
            SearchNode? closedNode =
                closed.Find(
                    node =>
                        node.Position.Row == row &&
                        node.Position.Col == col);


            if (closedNode != null)
            {
                return;
            }


            Coord successorPosition =
                new Coord(
                    row,
                    col);


            // Cost of entering the successor terrain.
            int movementCost =
                map[
                    row,
                    col];


            int newCost =
                current.Cost +
                movementCost;


            // ---------------------------------------------------------
            // CHECK WHETHER SUCCESSOR IS ALREADY IN OPEN
            // ---------------------------------------------------------
            SearchNode? existingNode =
                open.Find(
                    node =>
                        node.Position.Row == row &&
                        node.Position.Col == col);


            if (existingNode != null)
            {
                // Existing route is already cheaper or equal.
                if (newCost >=
                    existingNode.Cost)
                {
                    return;
                }


                // -----------------------------------------------------
                // CHEAPER ROUTE FOUND
                //
                // Remove before updating because the current queue
                // position was based on the old cost.
                // -----------------------------------------------------
                open.Remove(
                    existingNode);


                existingNode.Cost =
                    newCost;


                existingNode.Predecessor =
                    current;


                // Reinsert so queue order is recalculated.
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
                    0,
                    current);


            open.Enqueue(
                successor);
        }


        // =============================================================
        // INITIALISE STEP-BY-STEP DIJKSTRA SEARCH
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


            stepOpen =
                new PriorityQueue<SearchNode>(
                    (first, second) =>
                        first.Cost.CompareTo(
                            second.Cost)
                );


            stepClosed =
                new LinkedList<SearchNode>();


            stepOpenCoordinates =
                new LinkedList<Coord>();


            stepClosedCoordinates =
                new LinkedList<Coord>();


            SearchNode startNode =
                new SearchNode(
                    start,
                    0,
                    0,
                    null);


            stepOpen.Enqueue(
                startNode);


            stepOpenCoordinates.PushBack(
                start);


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
        // EXECUTE ONE DIJKSTRA EXPANSION
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
            // SEARCH ALREADY FINISHED
            // ---------------------------------------------------------
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


            // ---------------------------------------------------------
            // OPEN EMPTY
            // ---------------------------------------------------------
            if (stepOpen.IsEmpty())
            {
                stepComplete =
                    true;


                stepPathFound =
                    false;


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
            // REMOVE CHEAPEST NODE
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
            // GENERATE SUCCESSORS
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
            // SEARCH FAILURE CHECK
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
                stepOpenCoordinates == null)
            {
                return;
            }


            // Outside terrain.
            if (!IsInsideMap(
                stepMap,
                row,
                col))
            {
                return;
            }


            // Blocked terrain.
            if (stepMap[
                row,
                col] == 0)
            {
                return;
            }


            // ---------------------------------------------------------
            // CLOSED CHECK
            // ---------------------------------------------------------
            SearchNode? closedNode =
                stepClosed.Find(
                    node =>
                        node.Position.Row == row &&
                        node.Position.Col == col);


            if (closedNode != null)
            {
                return;
            }


            int movementCost =
                stepMap[
                    row,
                    col];


            int newCost =
                current.Cost +
                movementCost;


            // ---------------------------------------------------------
            // OPEN CHECK
            // ---------------------------------------------------------
            SearchNode? existingNode =
                stepOpen.Find(
                    node =>
                        node.Position.Row == row &&
                        node.Position.Col == col);


            if (existingNode != null)
            {
                // Existing route is already as good or better.
                if (newCost >=
                    existingNode.Cost)
                {
                    return;
                }


                // -----------------------------------------------------
                // CHEAPER ROUTE FOUND
                // -----------------------------------------------------
                stepOpen.Remove(
                    existingNode);


                existingNode.Cost =
                    newCost;


                existingNode.Predecessor =
                    current;


                // Reinsert according to updated cost.
                stepOpen.Enqueue(
                    existingNode);


                // The coordinate is already present in the GUI OPEN
                // coordinate list, so no extra coordinate is added.
                return;
            }


            // ---------------------------------------------------------
            // NEW SUCCESSOR
            // ---------------------------------------------------------
            Coord successorPosition =
                new Coord(
                    row,
                    col);


            SearchNode successor =
                new SearchNode(
                    successorPosition,
                    newCost,
                    0,
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