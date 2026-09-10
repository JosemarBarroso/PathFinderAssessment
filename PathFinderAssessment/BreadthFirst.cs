// COM 5113 Sample Code - Nick Mitchell 2025

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinderAssessment
{
    internal class BreadthFirst : PathFinderInterface
    {
        // Finds a path from the start coordinate to the goal coordinate
        // using Breadth First Search (BFS).
        public bool FindPath(
            int[,] map,
            Coord start,
            Coord goal,
            ref LinkedList<Coord> path)
        {
            // OPEN contains nodes that have been discovered
            // but have not yet been expanded.
            //
            // BFS uses a Queue because it follows FIFO ordering.
            Queue<SearchNode> open = new Queue<SearchNode>();

            // CLOSED contains nodes that have already been expanded.
            Queue<SearchNode> closed = new Queue<SearchNode>();

            // Store whether each coordinate has already been discovered.
            //
            // This prevents the same grid location from being added to
            // OPEN more than once.
            bool[,] visited = new bool[
                map.GetLength(0),
                map.GetLength(1)
            ];

            // Create the first SearchNode from the starting coordinate.
            //
            // The start node has:
            // Cost = 0
            // Score = 0
            // Predecessor = null
            SearchNode startNode = new SearchNode(
                start,
                0,
                0,
                null
            );

            // Add the starting node to OPEN.
            open.Enqueue(startNode);

            // Mark the starting coordinate as discovered.
            visited[start.Row, start.Col] = true;

            // Keep searching while OPEN still contains nodes.
            while (!open.IsEmpty())
            {
                // BFS always removes the oldest node from OPEN.
                SearchNode current = open.Dequeue();

                // Check whether the current node is the goal.
                if (current.Position.Row == goal.Row &&
                    current.Position.Col == goal.Col)
                {
                    // Follow the predecessor references backwards
                    // to construct the final path.
                    path = SearchUtilities.buildPathList(current);

                    return true;
                }

                // -----------------------------------------------------
                // Generate successors in the required clockwise order:
                //
                // North
                // East
                // South
                // West
                // -----------------------------------------------------

                // NORTH
                TryAddSuccessor(
                    map,
                    current.Position.Row - 1,
                    current.Position.Col,
                    current,
                    open,
                    visited
                );

                // EAST
                TryAddSuccessor(
                    map,
                    current.Position.Row,
                    current.Position.Col + 1,
                    current,
                    open,
                    visited
                );

                // SOUTH
                TryAddSuccessor(
                    map,
                    current.Position.Row + 1,
                    current.Position.Col,
                    current,
                    open,
                    visited
                );

                // WEST
                TryAddSuccessor(
                    map,
                    current.Position.Row,
                    current.Position.Col - 1,
                    current,
                    open,
                    visited
                );

                // After all successors have been generated,
                // move the current node to CLOSED.
                closed.Enqueue(current);
            }

            // If OPEN becomes empty before the goal is found,
            // no valid path exists.
            path = new LinkedList<Coord>();

            return false;
        }


        // -------------------------------------------------------------
        // Helper method used to generate and validate a successor node.
        // -------------------------------------------------------------
        private void TryAddSuccessor(
            int[,] map,
            int row,
            int col,
            SearchNode current,
            Queue<SearchNode> open,
            bool[,] visited)
        {
            // First check that the coordinate is inside the map.
            if (!IsInsideMap(map, row, col))
            {
                return;
            }

            // Terrain value 0 represents a wall / blocked cell.
            // Therefore it cannot be traversed.
            if (map[row, col] == 0)
            {
                return;
            }

            // Do not add a coordinate that has already been discovered.
            //
            // A coordinate is marked visited as soon as it enters OPEN.
            // This prevents duplicate nodes being added to OPEN.
            if (visited[row, col])
            {
                return;
            }

            // Create the coordinate for the successor.
            Coord successorPosition = new Coord(row, col);

            // Create the new SearchNode.
            //
            // BFS does not use terrain costs to decide which node
            // should be expanded next, so Cost and Score remain 0 here.
            //
            // current is stored as the predecessor so that the final
            // route can later be reconstructed.
            SearchNode successor = new SearchNode(
                successorPosition,
                0,
                0,
                current
            );

            // Add the new node to the back of OPEN.
            open.Enqueue(successor);

            // Mark it immediately so another node cannot add
            // the same coordinate again.
            visited[row, col] = true;
        }


        // -------------------------------------------------------------
        // Checks whether a coordinate lies inside the map boundaries.
        // -------------------------------------------------------------
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