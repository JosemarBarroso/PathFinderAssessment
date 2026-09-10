// COM 5113 Sample Code - Nick Mitchell 2025
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinderAssessment
{
    using Grid = int[,];

    // The interface describes the methods all implementations should provide bodies for
    internal interface PathFinderInterface
    {
        // Method to find a path on the map from the start to the goal
        // The path is returned through a reference parameter.
        // The return parameter should indicate the success or otherwise of the search.
        public bool FindPath(Grid map, Coord start, Coord goal, ref LinkedList<Coord> path);

        // TODO (for a first) Add another method to expand one ply of the search tree.
        //      You may with to have ref parameters to pass out lists of coords on the open and closed lists too?
    }
}
