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

                // -----------------------------------------------------
                // STEP 1: Select a terrain map.
                // -----------------------------------------------------
                string mapFileName = SelectMap();

                string mapFile = Path.Combine(
                    AppContext.BaseDirectory,
                    "Maps",
                    mapFileName);

                Console.WriteLine();
                Console.WriteLine($"Loading map: {mapFileName}");
                Console.WriteLine();

                // -----------------------------------------------------
                // STEP 2: Load the map, start and goal coordinates.
                // -----------------------------------------------------
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

                // -----------------------------------------------------
                // STEP 3: Select a search algorithm.
                // -----------------------------------------------------
                Algorithm selectedAlgorithm =
                    SelectAlgorithm();

                // -----------------------------------------------------
                // STEP 4: Create the selected pathfinding object
                // through the factory.
                // -----------------------------------------------------
                PathFinderInterface pathFinder =
                    PathFinderFactory.NewPathFinder(
                        selectedAlgorithm);

                // Custom LinkedList that will contain
                // the final route.
                LinkedList<Coord> path =
                    new LinkedList<Coord>();

                string algorithmName =
                    GetAlgorithmName(selectedAlgorithm);

                Console.WriteLine();
                Console.WriteLine(
                    $"Running {algorithmName}...");
                Console.WriteLine();

                // -----------------------------------------------------
                // STEP 5: Execute the selected search algorithm.
                // -----------------------------------------------------
                bool pathFound = pathFinder.FindPath(
                    map,
                    start,
                    goal,
                    ref path);

                // -----------------------------------------------------
                // STEP 6: Display results.
                // -----------------------------------------------------
                if (pathFound)
                {
                    Console.WriteLine(
                        "Path found successfully.");

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

                    // -------------------------------------------------
                    // A* specific information.
                    // -------------------------------------------------
                    if (pathFinder is AStar aStar)
                    {
                        Console.WriteLine();

                        Console.WriteLine(
                            $"Open List sort count: {aStar.OpenListSortCount}");
                    }

                    // -------------------------------------------------
                    // STEP 7: Write the route to an output text file.
                    // -------------------------------------------------
                    int? sortCount = null;

                    // A* additionally supplies its Open List
                    // ordering count.
                    if (pathFinder is AStar aStarResult)
                    {
                        sortCount =
                            aStarResult.OpenListSortCount;
                    }

                    string outputFile =
                        PathWriter.WritePath(
                            mapFileName,
                            algorithmName,
                            path,
                            sortCount);

                    Console.WriteLine();

                    Console.WriteLine(
                        $"Path file created: {outputFile}");
                }
                else
                {
                    Console.WriteLine(
                        "No path could be found between the start and goal.");

                    // Display A* specific information even
                    // when no route is found.
                    if (pathFinder is AStar aStar)
                    {
                        Console.WriteLine();

                        Console.WriteLine(
                            $"Open List sort count: {aStar.OpenListSortCount}");
                    }
                }

                Console.WriteLine();
                Console.WriteLine(
                    "Press any key to exit...");

                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("ERROR:");
                Console.WriteLine(ex.Message);

                Console.WriteLine();
                Console.WriteLine(
                    "Press any key to exit...");

                Console.ReadKey();
            }
        }


        // =============================================================
        // MAP SELECTION
        // =============================================================
        private static string SelectMap()
        {
            while (true)
            {
                Console.WriteLine(
                    "Select Terrain Map:");

                Console.WriteLine();

                Console.WriteLine("1. test1Map.txt");
                Console.WriteLine("2. test2Map.txt");
                Console.WriteLine("3. test3Map.txt");
                Console.WriteLine("4. test4Map.txt");
                Console.WriteLine("5. test5Map.txt");
                Console.WriteLine("6. test6Map.txt");

                Console.WriteLine();

                Console.Write(
                    "Enter map number (1-6): ");

                string? input =
                    Console.ReadLine();

                switch (input)
                {
                    case "1":
                        return "test1Map.txt";

                    case "2":
                        return "test2Map.txt";

                    case "3":
                        return "test3Map.txt";

                    case "4":
                        return "test4Map.txt";

                    case "5":
                        return "test5Map.txt";

                    case "6":
                        return "test6Map.txt";

                    default:
                        Console.WriteLine();

                        Console.WriteLine(
                            "Invalid selection. Please enter a number from 1 to 6.");

                        Console.WriteLine();
                        break;
                }
            }
        }


        // =============================================================
        // ALGORITHM SELECTION
        // =============================================================
        private static Algorithm SelectAlgorithm()
        {
            while (true)
            {
                Console.WriteLine(
                    "Select Search Algorithm:");

                Console.WriteLine();

                Console.WriteLine(
                    "1. Breadth First Search");

                Console.WriteLine(
                    "2. Depth First Search");

                Console.WriteLine(
                    "3. Hill Climbing");

                Console.WriteLine(
                    "4. Best First Search");

                Console.WriteLine(
                    "5. Dijkstra's Search");

                Console.WriteLine(
                    "6. A* Search");

                Console.WriteLine();

                Console.Write(
                    "Enter algorithm number (1-6): ");

                string? input =
                    Console.ReadLine();

                switch (input)
                {
                    case "1":
                        return Algorithm.BreadthFirst;

                    case "2":
                        return Algorithm.DepthFirst;

                    case "3":
                        return Algorithm.HillClimbing;

                    case "4":
                        return Algorithm.BestFirst;

                    case "5":
                        return Algorithm.Dijkstras;

                    case "6":
                        return Algorithm.AStar;

                    default:
                        Console.WriteLine();

                        Console.WriteLine(
                            "Invalid selection. Please enter a number from 1 to 6.");

                        Console.WriteLine();
                        break;
                }
            }
        }


        // =============================================================
        // Returns a user-friendly algorithm name.
        // =============================================================
        private static string GetAlgorithmName(
            Algorithm algorithm)
        {
            switch (algorithm)
            {
                case Algorithm.BreadthFirst:
                    return "Breadth First Search";

                case Algorithm.DepthFirst:
                    return "Depth First Search";

                case Algorithm.HillClimbing:
                    return "Hill Climbing Search";

                case Algorithm.BestFirst:
                    return "Best First Search";

                case Algorithm.Dijkstras:
                    return "Dijkstra's Search";

                case Algorithm.AStar:
                    return "A* Search";

                default:
                    return "Unknown Search";
            }
        }
    }
}