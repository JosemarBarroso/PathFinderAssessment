// COM 5113 Pathfinding Assessment

using System;

namespace PathFinderAssessment
{
    internal class BestFirst :
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

        private bool[,]? stepVisited;

        private LinkedList<Coord>? stepOpenCoordinates;
        private LinkedList<Coord>? stepClosedCoordinates;

        private bool stepInitialised;
        private bool stepComplete;
        private bool stepPathFound;

        private LinkedList<Coord>? stepFinalPath;


        // =============================================================
        // NORMAL BEST FIRST SEARCH
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
            // Best First Search orders OPEN using only the heuristic.
            // Lower heuristic value means higher priority.
            // ---------------------------------------------------------
            PriorityQueue<SearchNode> open =
                new PriorityQueue<SearchNode>(
                    (first, second) =>
                        first.Score.CompareTo(
                            second.Score)
                );


            // CLOSED stores nodes already expanded.
            LinkedList<SearchNode> closed =
                new LinkedList<SearchNode>();


            // Tracks discovered coordinates.
            bool[,] visited =
                new bool[
                    map.GetLength(0),
                    map.GetLength(1)
                ];


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


            visited[
                start.Row,
                start.Col] = true;


            // ---------------------------------------------------------
            // SEARCH LOOP
            // ---------------------------------------------------------
            while (!open.IsEmpty())
            {
                // Remove the node with the smallest heuristic.
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
                TryAddSuccessor(
                    map,
                    current.Position.Row - 1,
                    current.Position.Col,
                    current,
                    goal,
                    open,
                    visited);


                // EAST
                TryAddSuccessor(
                    map,
                    current.Position.Row,
                    current.Position.Col + 1,
                    current,
                    goal,
                    open,
                    visited);


                // SOUTH
                TryAddSuccessor(
                    map,
                    current.Position.Row + 1,
                    current.Position.Col,
                    current,
                    goal,
                    open,
                    visited);


                // WEST
                TryAddSuccessor(
                    map,
                    current.Position.Row,
                    current.Position.Col - 1,
                    current,
                    goal,
                    open,
                    visited);


                // Move expanded node to CLOSED.
                closed.PushBack(
                    current);
            }


            // OPEN became empty.
            path =
                new LinkedList<Coord>();


            return false;
        }


        // =============================================================
        // INITIALISE STEP-BY-STEP BEST FIRST SEARCH
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


            // OPEN is globally ordered by heuristic.
            stepOpen =
                new PriorityQueue<SearchNode>(
                    (first, second) =>
                        first.Score.CompareTo(
                            second.Score)
                );


            stepClosed =
                new LinkedList<SearchNode>();


            stepVisited =
                new bool[
                    map.GetLength(0),
                    map.GetLength(1)
                ];


            stepOpenCoordinates =
                new LinkedList<Coord>();


            stepClosedCoordinates =
                new LinkedList<Coord>();


            // ---------------------------------------------------------
            // CREATE START NODE
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


            stepOpen.Enqueue(
                startNode);


            stepOpenCoordinates.PushBack(
                start);


            stepVisited[
                start.Row,
                start.Col] = true;


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
        // EXECUTE ONE BEST FIRST SEARCH STEP
        // =============================================================

        public SearchStepResult Step()
        {
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


            // ---------------------------------------------------------
            // SEARCH ALREADY COMPLETE
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
            // REMOVE BEST NODE FROM OPEN
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
            // MOVE CURRENT TO CLOSED
            // ---------------------------------------------------------
            stepClosed.PushBack(
                current);


            stepClosedCoordinates.PushBack(
                current.Position);


            // ---------------------------------------------------------
            // CHECK WHETHER SEARCH HAS FAILED
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
        // NORMAL SUCCESSOR GENERATION
        // =============================================================

        private void TryAddSuccessor(
            int[,] map,
            int row,
            int col,
            SearchNode current,
            Coord goal,
            PriorityQueue<SearchNode> open,
            bool[,] visited)
        {
            // Outside terrain.
            if (!IsInsideMap(
                map,
                row,
                col))
            {
                return;
            }


            // Terrain value 0 = blocked.
            if (map[
                row,
                col] == 0)
            {
                return;
            }


            // Already discovered.
            if (visited[
                row,
                col])
            {
                return;
            }


            Coord successorPosition =
                new Coord(
                    row,
                    col);


            // Best First evaluates only Manhattan heuristic.
            int heuristic =
                SearchUtilities.ManhattanDistance(
                    successorPosition,
                    goal);


            SearchNode successor =
                new SearchNode(
                    successorPosition,
                    0,
                    heuristic,
                    current);


            // Priority queue automatically inserts
            // according to heuristic score.
            open.Enqueue(
                successor);


            // Mark discovered immediately.
            visited[
                row,
                col] = true;
        }


        // =============================================================
        // STEP SUCCESSOR GENERATION
        // =============================================================

        private void TryAddStepSuccessor(
            int row,
            int col,
            SearchNode current)
        {
            if (stepMap == null ||
                stepOpen == null ||
                stepVisited == null ||
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


            // Blocked.
            if (stepMap[
                row,
                col] == 0)
            {
                return;
            }


            // Already discovered.
            if (stepVisited[
                row,
                col])
            {
                return;
            }


            Coord successorPosition =
                new Coord(
                    row,
                    col);


            // Calculate Manhattan heuristic.
            int heuristic =
                SearchUtilities.ManhattanDistance(
                    successorPosition,
                    stepGoal);


            SearchNode successor =
                new SearchNode(
                    successorPosition,
                    0,
                    heuristic,
                    current);


            // Add to globally ordered OPEN.
            stepOpen.Enqueue(
                successor);


            // Store coordinate for GUI visualisation.
            stepOpenCoordinates.PushBack(
                successorPosition);


            // Mark discovered.
            stepVisited[
                row,
                col] = true;
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
        // CHECK MAP BOUNDS
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