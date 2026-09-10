// COM 5113 Pathfinding Assessment

using System;
using System.IO;

namespace PathFinderAssessment
{
    // Responsible for loading terrain maps from text files.
    internal static class MapLoader
    {
        // Loads the map, start coordinate and goal coordinate
        // from the supplied text file.
        public static int[,] LoadMap(
            string filePath,
            out Coord start,
            out Coord goal)
        {
            // Make sure the requested file exists.
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(
                    "The map file could not be found.",
                    filePath);
            }

            // Read all lines from the map file.
            string[] lines = File.ReadAllLines(filePath);

            // A valid file must contain at least:
            // dimensions, start, goal and some map data.
            if (lines.Length < 4)
            {
                throw new InvalidDataException(
                    "The map file does not contain enough data.");
            }

            // ---------------------------------------------------------
            // FIRST LINE: number of rows and columns
            // Example:
            // 12 12
            // ---------------------------------------------------------
            string[] dimensions = lines[0].Split(
                ' ',
                StringSplitOptions.RemoveEmptyEntries);

            if (dimensions.Length != 2)
            {
                throw new InvalidDataException(
                    "Invalid map dimensions.");
            }

            int rows = int.Parse(dimensions[0]);
            int columns = int.Parse(dimensions[1]);

            // ---------------------------------------------------------
            // SECOND LINE: starting coordinate
            // Example:
            // 8 3
            // ---------------------------------------------------------
            string[] startValues = lines[1].Split(
                ' ',
                StringSplitOptions.RemoveEmptyEntries);

            if (startValues.Length != 2)
            {
                throw new InvalidDataException(
                    "Invalid start coordinate.");
            }

            start = new Coord(
                int.Parse(startValues[0]),
                int.Parse(startValues[1]));

            // ---------------------------------------------------------
            // THIRD LINE: goal coordinate
            // Example:
            // 2 4
            // ---------------------------------------------------------
            string[] goalValues = lines[2].Split(
                ' ',
                StringSplitOptions.RemoveEmptyEntries);

            if (goalValues.Length != 2)
            {
                throw new InvalidDataException(
                    "Invalid goal coordinate.");
            }

            goal = new Coord(
                int.Parse(goalValues[0]),
                int.Parse(goalValues[1]));

            // Make sure the file contains the expected number
            // of terrain rows.
            if (lines.Length < rows + 3)
            {
                throw new InvalidDataException(
                    "The map file contains fewer rows than expected.");
            }

            // Create the two-dimensional terrain array.
            int[,] map = new int[rows, columns];

            // ---------------------------------------------------------
            // Remaining lines contain the terrain grid.
            // ---------------------------------------------------------
            for (int row = 0; row < rows; row++)
            {
                string[] values = lines[row + 3].Split(
                    ' ',
                    StringSplitOptions.RemoveEmptyEntries);

                // Every terrain row must contain the correct
                // number of columns.
                if (values.Length != columns)
                {
                    throw new InvalidDataException(
                        $"Map row {row} does not contain {columns} columns.");
                }

                for (int col = 0; col < columns; col++)
                {
                    int terrain = int.Parse(values[col]);

                    // Valid terrain values are 0, 1, 2 and 3.
                    if (terrain < 0 || terrain > 3)
                    {
                        throw new InvalidDataException(
                            $"Invalid terrain value {terrain} at ({row}, {col}).");
                    }

                    map[row, col] = terrain;
                }
            }

            // Validate start and goal coordinates.
            if (!IsValidCoordinate(map, start))
            {
                throw new InvalidDataException(
                    "The start coordinate is outside the map.");
            }

            if (!IsValidCoordinate(map, goal))
            {
                throw new InvalidDataException(
                    "The goal coordinate is outside the map.");
            }

            // Start and goal cannot be walls.
            if (map[start.Row, start.Col] == 0)
            {
                throw new InvalidDataException(
                    "The start coordinate is on a blocked cell.");
            }

            if (map[goal.Row, goal.Col] == 0)
            {
                throw new InvalidDataException(
                    "The goal coordinate is on a blocked cell.");
            }

            return map;
        }


        // Checks whether a coordinate is inside the map.
        private static bool IsValidCoordinate(
            int[,] map,
            Coord coordinate)
        {
            return coordinate.Row >= 0 &&
                   coordinate.Row < map.GetLength(0) &&
                   coordinate.Col >= 0 &&
                   coordinate.Col < map.GetLength(1);
        }
    }
}