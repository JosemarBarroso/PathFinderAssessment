// COM 5113 Sample Code - Nick Mitchell 2025

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinderAssessment
{
    // More algorithms will be added as we implement them.
    enum Algorithm
    {
        BreadthFirst,
        DepthFirst,
        HillClimbing
    }

    internal class PathFinderFactory
    {
        // Creates the appropriate pathfinder implementation.
        public static PathFinderInterface NewPathFinder(Algorithm algorithm)
        {
            PathFinderInterface pathFinder;

            switch (algorithm)
            {
                case Algorithm.BreadthFirst:
                    pathFinder = new BreadthFirst();
                    break;

                case Algorithm.DepthFirst:
                    pathFinder = new DepthFirst();
                    break;

                case Algorithm.HillClimbing:
                    // Temporary fallback until HillClimbing is implemented.
                    pathFinder = new BreadthFirst();
                    break;

                default:
                    pathFinder = new BreadthFirst();
                    break;
            }

            return pathFinder;
        }
    }
}