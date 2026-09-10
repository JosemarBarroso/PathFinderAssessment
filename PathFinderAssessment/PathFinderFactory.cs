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
        HillClimbing,
        BestFirst,
        Dijkstras
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
                    pathFinder = new HillClimbing();
                    break;

                case Algorithm.BestFirst:
                    pathFinder = new BestFirst();
                    break;

                case Algorithm.Dijkstras:
                    pathFinder = new Dijkstra();
                    break;

                default:
                    pathFinder = new BreadthFirst();
                    break;
            }

            return pathFinder;
        }
    }
}