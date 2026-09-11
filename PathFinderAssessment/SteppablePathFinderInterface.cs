// COM 5113 Pathfinding Assessment

namespace PathFinderAssessment
{
    /// <summary>
    /// Optional interface for algorithms that support
    /// step-by-step graphical visualisation.
    ///
    /// This is separate from PathFinderInterface so that
    /// the existing complete FindPath() implementations
    /// remain unchanged.
    /// </summary>
    internal interface SteppablePathFinderInterface
    {
        /// <summary>
        /// Prepares an algorithm for a new step-by-step search.
        /// </summary>
        void InitialiseStepSearch(
            int[,] map,
            Coord start,
            Coord goal);


        /// <summary>
        /// Executes one expansion of the search algorithm.
        /// </summary>
        SearchStepResult Step();
    }
}