// COM 5113 Pathfinding Assessment

namespace PathFinderAssessment
{
    /// <summary>
    /// Represents the state of a pathfinding algorithm
    /// after one search expansion.
    /// </summary>
    internal class SearchStepResult
    {
        // The node expanded during this step.
        public Coord? CurrentNode { get; }

        // Coordinates currently waiting to be explored.
        public LinkedList<Coord> OpenList { get; }

        // Coordinates that have already been explored.
        public LinkedList<Coord> ClosedList { get; }

        // True when the search has finished.
        public bool IsComplete { get; }

        // True when the goal has been reached.
        public bool PathFound { get; }

        // Contains the final path when PathFound is true.
        public LinkedList<Coord>? Path { get; }


        public SearchStepResult(
            Coord? currentNode,
            LinkedList<Coord> openList,
            LinkedList<Coord> closedList,
            bool isComplete,
            bool pathFound,
            LinkedList<Coord>? path = null)
        {
            CurrentNode = currentNode;
            OpenList = openList;
            ClosedList = closedList;
            IsComplete = isComplete;
            PathFound = pathFound;
            Path = path;
        }
    }
}