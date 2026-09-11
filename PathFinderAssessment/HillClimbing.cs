// COM 5113 Sample Code - Nick Mitchell 2025

using System;

namespace PathFinderAssessment
{
    internal class HillClimbing :
        PathFinderInterface,
        SteppablePathFinderInterface
    {
        // =============================================================
        // STEP-BY-STEP SEARCH STATE
        // =============================================================

        private int[,]? stepMap;

        private Coord stepStart;
        private Coord stepGoal;

        private Stack<SearchNode>? stepOpen;
        private Stack<SearchNode>? stepClosed;

        private bool[,]? stepVisited;

        private LinkedList<Coord>? stepOpenCoordinates;
        private LinkedList<Coord>? stepClosedCoordinates;

        private bool stepInitialised;
        private bool stepComplete;
        private bool stepPathFound;

        private LinkedList<Coord>? stepFinalPath;


        // =============================================================
        // NORMAL COMPLETE HILL CLIMBING SEARCH
        // =============================================================

        public bool FindPath(
            int[,] map,
            Coord start,
            Coord goal,
            ref LinkedList<Coord> path)
        {
            // OPEN stores nodes waiting to be expanded.
            Stack<SearchNode> open =
                new Stack<SearchNode>();


            // CLOSED stores nodes already expanded.
            Stack<SearchNode> closed =
                new Stack<SearchNode>();


            // Tracks discovered coordinates.
            bool[,] visited =
                new bool[
                    map.GetLength(0),
                    map.GetLength(1)
                ];


            // Calculate heuristic for start.
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


            open.Push(
                startNode);


            visited[
                start.Row,
                start.Col] = true;


            while (!open.IsEmpty())
            {
                // Remove the preferred node.
                SearchNode current =
                    open.Pop();


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
                // TEMPORARY SUCCESSOR LIST
                // -----------------------------------------------------
                LinkedList<SearchNode> tempList =
                    new LinkedList<SearchNode>();


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
                    tempList,
                    visited);


                // EAST
                TryAddSuccessor(
                    map,
                    current.Position.Row,
                    current.Position.Col + 1,
                    current,
                    goal,
                    tempList,
                    visited);


                // SOUTH
                TryAddSuccessor(
                    map,
                    current.Position.Row + 1,
                    current.Position.Col,
                    current,
                    goal,
                    tempList,
                    visited);


                // WEST
                TryAddSuccessor(
                    map,
                    current.Position.Row,
                    current.Position.Col - 1,
                    current,
                    goal,
                    tempList,
                    visited);


                // -----------------------------------------------------
                // tempList is sorted from best heuristic to worst.
                //
                // Because OPEN is a stack, remove the worst node first
                // and push it onto OPEN.
                //
                // The best node will therefore be pushed last and
                // become the next node popped.
                // -----------------------------------------------------
                while (!tempList.IsEmpty())
                {
                    SearchNode node =
                        tempList.PopBack();


                    open.Push(
                        node);
                }


                // Move current to CLOSED.
                closed.Push(
                    current);
            }


            // No path found.
            path =
                new LinkedList<Coord>();


            return false;
        }


        // =============================================================
        // INITIALISE STEP-BY-STEP HILL CLIMBING
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
                new Stack<SearchNode>();


            stepClosed =
                new Stack<SearchNode>();


            stepVisited =
                new bool[
                    map.GetLength(0),
                    map.GetLength(1)
                ];


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


            stepOpen.Push(
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
        // EXECUTE ONE HILL CLIMBING EXPANSION
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


            // Search already finished.
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


            // OPEN is empty.
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
            // POP ONE NODE FROM OPEN
            // ---------------------------------------------------------
            SearchNode current =
                stepOpen.Pop();


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
            // TEMPORARY SUCCESSOR LIST
            // ---------------------------------------------------------
            LinkedList<SearchNode> tempList =
                new LinkedList<SearchNode>();


            // ---------------------------------------------------------
            // GENERATE SUCCESSORS
            //
            // North → East → South → West
            // ---------------------------------------------------------

            // NORTH
            TryAddStepSuccessor(
                current.Position.Row - 1,
                current.Position.Col,
                current,
                tempList);


            // EAST
            TryAddStepSuccessor(
                current.Position.Row,
                current.Position.Col + 1,
                current,
                tempList);


            // SOUTH
            TryAddStepSuccessor(
                current.Position.Row + 1,
                current.Position.Col,
                current,
                tempList);


            // WEST
            TryAddStepSuccessor(
                current.Position.Row,
                current.Position.Col - 1,
                current,
                tempList);


            // ---------------------------------------------------------
            // MOVE SORTED TEMP LIST INTO OPEN
            // ---------------------------------------------------------
            while (!tempList.IsEmpty())
            {
                SearchNode node =
                    tempList.PopBack();


                stepOpen.Push(
                    node);


                stepOpenCoordinates.PushBack(
                    node.Position);
            }


            // ---------------------------------------------------------
            // MOVE CURRENT TO CLOSED
            // ---------------------------------------------------------
            stepClosed.Push(
                current);


            stepClosedCoordinates.PushBack(
                current.Position);


            // If OPEN is now empty, no route exists.
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
            LinkedList<SearchNode> tempList,
            bool[,] visited)
        {
            // Outside map.
            if (!IsInsideMap(
                map,
                row,
                col))
            {
                return;
            }


            // Blocked terrain.
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


            // Sort temporary successors by heuristic.
            tempList.InsertSorted(
                successor,
                (first, second) =>
                    first.Score.CompareTo(
                        second.Score));


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
            SearchNode current,
            LinkedList<SearchNode> tempList)
        {
            if (stepMap == null ||
                stepVisited == null)
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


            // Insert into the temporary list in
            // ascending heuristic order.
            tempList.InsertSorted(
                successor,
                (first, second) =>
                    first.Score.CompareTo(
                        second.Score));


            // Mark immediately to prevent duplicates.
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