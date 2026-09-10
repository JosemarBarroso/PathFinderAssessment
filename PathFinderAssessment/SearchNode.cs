// COM 5113 Sample Code - Nick Mitchell 2025

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinderAssessment
{
    // Coordinate used to represent a location in the terrain grid.
    // Coordinates use (row, column), starting from 0.
    public readonly struct Coord
    {
        public int Row { get; }
        public int Col { get; }

        public Coord(int row, int col)
        {
            Row = row;
            Col = col;
        }

        // Makes coordinates easier to display while debugging.
        public override string ToString()
        {
            return $"({Row}, {Col})";
        }
    }

    // Represents one node/state used by the pathfinding algorithms.
    public class SearchNode
    {
        // Position of this node in the grid.
        public Coord Position { get; set; }

        // Cost accumulated from the start node to this node.
        public int Cost { get; set; }

        // Heuristic score used by informed search algorithms.
        public int Score { get; set; }

        // Combined estimate used later by A*.
        // Cost = g(n)
        // Score = h(n)
        // Estimate = f(n) = g(n) + h(n)
        public int Estimate
        {
            get
            {
                return Cost + Score;
            }
        }

        // Previous node in the discovered path.
        // The start node has no predecessor, so this is nullable.
        public SearchNode? Predecessor { get; set; }

        // Constructor
        public SearchNode(
            Coord position,
            int cost = 0,
            int score = 0,
            SearchNode? predecessor = null)
        {
            Position = position;
            Cost = cost;
            Score = score;
            Predecessor = predecessor;
        }
    }

    // Utility methods shared by the search algorithms.
    public static class SearchUtilities
    {
        // Builds the final path by following predecessor references
        // backwards from the goal node to the start node.
        public static LinkedList<Coord> buildPathList(SearchNode? goal)
        {
            LinkedList<Coord> path = new LinkedList<Coord>();

            // SearchNode is nullable because there may be no path.
            SearchNode? current = goal;

            while (current != null)
            {
                // Push to the front because we are following
                // the path backwards from goal to start.
                path.PushFront(current.Position);

                current = current.Predecessor;
            }

            return path;
        }

        // Calculates Manhattan distance between two grid coordinates.
        //
        // Only North, East, South and West movement is allowed,
        // so Manhattan distance is appropriate.
        public static int ManhattanDistance(Coord current, Coord goal)
        {
            int rowDistance = Math.Abs(current.Row - goal.Row);
            int columnDistance = Math.Abs(current.Col - goal.Col);

            return rowDistance + columnDistance;
        }
    }
}