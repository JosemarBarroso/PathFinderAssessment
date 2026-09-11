// COM 5113 Pathfinding Assessment

using System;
using System.IO;

namespace PathFinderAssessment
{
    internal static class PathWriter
    {
        // Writes the calculated path to a text file.
        //
        // Example output file:
        // test1Path_AStar.txt
        public static string WritePath(
            string mapFileName,
            string algorithmName,
            LinkedList<Coord> path,
            int? openListSortCount = null)
        {
            // Remove "Map.txt" from the original filename.
            //
            // Example:
            // test1Map.txt -> test1
            string mapName =
                Path.GetFileNameWithoutExtension(mapFileName);

            if (mapName.EndsWith(
                "Map",
                StringComparison.OrdinalIgnoreCase))
            {
                mapName =
                    mapName.Substring(
                        0,
                        mapName.Length - 3);
            }

            // Remove spaces and symbols from the algorithm name
            // so it is suitable for a filename.
            string cleanAlgorithmName =
                algorithmName
                    .Replace(" ", "")
                    .Replace("'", "")
                    .Replace("*", "Star");

            // Create the output filename.
            string outputFileName =
                $"{mapName}Path_{cleanAlgorithmName}.txt";

            // Store output files inside the project's
            // Output folder beside the executable.
            string outputDirectory =
                Path.Combine(
                    AppContext.BaseDirectory,
                    "Output");

            // Create the folder if it does not already exist.
            Directory.CreateDirectory(outputDirectory);

            string outputFilePath =
                Path.Combine(
                    outputDirectory,
                    outputFileName);

            // Write all coordinates to the file.
            using (StreamWriter writer =
                new StreamWriter(outputFilePath))
            {
                path.ForEach(coordinate =>
                {
                    // Required coordinate format:
                    // row column
                    writer.WriteLine(
                        $"{coordinate.Row} {coordinate.Col}");
                });

                // A* additionally records the Open List
                // ordering count.
                if (openListSortCount.HasValue)
                {
                    writer.WriteLine(
                        $"Open List sort count: {openListSortCount.Value}");
                }
            }

            // Return the complete path so Program.cs
            // can tell the user where the file was created.
            return outputFilePath;
        }
    }
}