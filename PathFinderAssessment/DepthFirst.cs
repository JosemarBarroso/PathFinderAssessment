// COM 5113 Sample Code - Nick Mitchell 2025

using System;

namespace PathFinderAssessment
{
    internal class DepthFirst :
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
        // NORMAL COMPLETE DFS
        // =============================================================

        public bool FindPath(
            int[,] map,
            Coord start,
            Coord goal,
            ref LinkedList<Coord> path)
        {
            Stack<SearchNode> open =
                new Stack<SearchNode>();

            Stack<SearchNode> closed =
                new Stack<SearchNode>();


            bool[,] visited =
                new bool[
                    map.GetLength(0),
                    map.GetLength(1)
                ];


            SearchNode startNode =
                new SearchNode(
                    start,
                    0,
                    0,
                    null);


            open.Push(
                startNode);


            visited[
                start.Row,
                start.Col] = true;


            while (!open.IsEmpty())
            {
                SearchNode current =
                    open.Pop();


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
                // PUSH IN REVERSE ORDER:
                //
                // West
                // South
                // East
                // North
                //
                // Therefore the effective DFS processing order is:
                //
                // North
                // East
                // South
                // West
                // -----------------------------------------------------

                // WEST
                TryAddSuccessor(
                    map,
                    current.Position.Row,
                    current.Position.Col - 1,
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


                // EAST
                TryAddSuccessor(
                    map,
                    current.Position.Row,
                    current.Position.Col + 1,
                    current,
                    open,
                    visited);


                // NORTH
                TryAddSuccessor(
                    map,
                    current.Position.Row - 1,
                    current.Position.Col,
                    current,
                    open,
                    visited);


                closed.Push(
                    current);
            }


            path =
                new LinkedList<Coord>();


            return false;
        }


        // =============================================================
        // INITIALISE STEP-BY-STEP DFS
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


            SearchNode startNode =
                new SearchNode(
                    start,
                    0,
                    0,
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
        // EXECUTE ONE DFS EXPANSION
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


            // Already complete.
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


            // Nothing left to search.
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
            // CHECK GOAL
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
            // ADD SUCCESSORS
            //
            // Push reverse order:
            //
            // West → South → East → North
            //
            // So effective expansion order becomes:
            //
            // North → East → South → West
            // ---------------------------------------------------------

            // WEST
            TryAddStepSuccessor(
                current.Position.Row,
                current.Position.Col - 1,
                current);


            // SOUTH
            TryAddStepSuccessor(
                current.Position.Row + 1,
                current.Position.Col,
                current);


            // EAST
            TryAddStepSuccessor(
                current.Position.Row,
                current.Position.Col + 1,
                current);


            // NORTH
            TryAddStepSuccessor(
                current.Position.Row - 1,
                current.Position.Col,
                current);


            // ---------------------------------------------------------
            // MOVE CURRENT TO CLOSED
            // ---------------------------------------------------------

            stepClosed.Push(
                current);


            stepClosedCoordinates.PushBack(
                current.Position);


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
        // NORMAL DFS SUCCESSOR HELPER
        // =============================================================

        private void TryAddSuccessor(
            int[,] map,
            int row,
            int col,
            SearchNode current,
            Stack<SearchNode> open,
            bool[,] visited)
        {
            if (!IsInsideMap(
                map,
                row,
                col))
            {
                return;
            }


            if (map[
                row,
                col] == 0)
            {
                return;
            }


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


            SearchNode successor =
                new SearchNode(
                    successorPosition,
                    0,
                    0,
                    current);


            open.Push(
                successor);


            visited[
                row,
                col] = true;
        }


        // =============================================================
        // STEP DFS SUCCESSOR HELPER
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


            if (!IsInsideMap(
                stepMap,
                row,
                col))
            {
                return;
            }


            if (stepMap[
                row,
                col] == 0)
            {
                return;
            }


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


            SearchNode successor =
                new SearchNode(
                    successorPosition,
                    0,
                    0,
                    current);


            // Actual DFS OPEN stack.
            stepOpen.Push(
                successor);


            // GUI OPEN list.
            stepOpenCoordinates.PushBack(
                successorPosition);


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