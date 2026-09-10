// COM 5113 Pathfinding Assessment

using System;
using System.IO;

namespace PathFinderAssessment
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("====================================");
                Console.WriteLine("       PATHFINDING APPLICATION");
                Console.WriteLine("====================================");
                Console.WriteLine();

                // Build the path to the first lecturer test map.
                string mapFile = Path.Combine(
                    AppContext.BaseDirectory,
                    "Maps",
                    "test1Map.txt");

                Console.WriteLine("Loading map: test1Map.txt");
                Console.WriteLine();

                // Load the terrain map and its start/goal coordinates.
                int[,] map = MapLoader.LoadMap(
                    mapFile,
                    out Coord start,
                    out Coord goal);

                Console.WriteLine(
                    $"Map Size : {map.GetLength(0)} x {map.GetLength(1)}");

                Console.WriteLine(
                    $"Start    : ({start.Row}, {start.Col})");

                Console.WriteLine(
                    $"Goal     : ({goal.Row}, {goal.Col})");

                Console.WriteLine();

                // Select Breadth First Search through the factory.
                PathFinderInterface pathFinder =
                    PathFinderFactory.NewPathFinder(
                        Algorithm.BreadthFirst);

                // The final path will be placed into this custom
                // LinkedList by the search algorithm.
                LinkedList<Coord> path = new LinkedList<Coord>();

                Console.WriteLine("Running Breadth First Search...");
                Console.WriteLine();

                bool pathFound = pathFinder.FindPath(
                    map,
                    start,
                    goal,
                    ref path);

                if (pathFound)
                {
                    Console.WriteLine("Path found successfully.");

                    Console.WriteLine(
                        $"Number of coordinates in path: {path.Count()}");

                    Console.WriteLine();
                    Console.WriteLine("Path:");

                    // Display every coordinate from start to goal.
                    path.ForEach(coordinate =>
                    {
                        Console.WriteLine(
                            $"({coordinate.Row}, {coordinate.Col})");
                    });
                }
                else
                {
                    Console.WriteLine(
                        "No path could be found between the start and goal.");
                }

                Console.WriteLine();
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("ERROR:");
                Console.WriteLine(ex.Message);

                Console.WriteLine();
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }
    }
}