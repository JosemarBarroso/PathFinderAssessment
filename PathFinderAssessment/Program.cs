// COM 5113 Sample Code - Nick Mitchell 2025
namespace PathFinderAssessment
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // TODO: Read the map file and store the data

            // Below is some dummy data
            int[,] map = new int[,]
            {   { 0, 1, 2, 3 },
                { 1, 2, 3, 0 },
                { 2, 3, 0, 1 },
                { 3, 0, 1, 2 }  };
            Coord start = new Coord();
            Coord goal  = new Coord();

            // TODO: Ask the user to choose the algorithm
            Algorithm chosenAlgorith = Algorithm.BreadthFirst;

            // Instantiate the chosen pathfinder
            PathFinderInterface myPathFinder = PathFinderFactory.NewPathFinder(chosenAlgorith);

            // A place to store the path
            LinkedList<Coord> path = new LinkedList<Coord>();

            // Call the pathfinder.
            myPathFinder.FindPath(map, start, goal, ref path);

            // TODO: Display the path however you want.

            Console.WriteLine("Goodbye, World!");
        }
    }
}
