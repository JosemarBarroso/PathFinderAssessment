// COM 5113 Sample Code - Nick Mitchell 2025
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinderAssessment
{
    // This is the outline of a class for you to follow...
    internal class BreadthFirst : PathFinderInterface
    {
        // Implement method mandated by Interface:
        public bool FindPath(int[,] map, Coord start, Coord goal, ref LinkedList<Coord> path)
        {
            // 1. Create the Open List and Closed List
            var open   = new Queue<SearchNode>();
            var closed = new Queue<SearchNode>();

            // 2. Enqueue the initial location onto the Open List 
            open.Enqueue(start);
            SearchNode current = null;

            // 3.Until the exit is reached or the OpenList is empty, do the following: 
            //     a.Dequeue the first coordinate from the OpenList and call it Current

            // TODO: Implement the algorithm fully!
                current = open.Dequeue();


            // Assuming current is the cound goal, construct the path working backwards           
            path = SearchUtilities.buildPathList(current);

            return false;
        }
    }
}
