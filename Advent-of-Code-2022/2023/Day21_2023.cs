using System.Collections;
using System.Diagnostics;
using System.Drawing;
using static Advent_of_Code_2022.Directions;

namespace Advent_of_Code_2022._2023;

public static class Day21_2023
{
    public static void Run(string testInputPath, string challengeInputPath)
    {
        var sw = new Stopwatch();
        sw.Start();

        //Solve(File.ReadAllLines(testInputPath), 6);
        Solve(File.ReadAllLines(challengeInputPath), 64);

        sw.Stop();
        Console.WriteLine($"Solution took: {sw.ElapsedMilliseconds} ms");
    }

    private static void Solve(string[] map, int numSteps)
    {
        var start = Point.Empty;
        var h = map.Length;
        var w = map[0].Length;
        
        for (int y = 0; y < h; y++)
        {
            var idx = map[y].IndexOf('S');
            if (idx >= 0)
            {
                start = new Point(idx, y);
            }
        }

        var positions = new HashSet<Point>
        {
            start
        };

        for (int i = 0; i < numSteps; i++)
        {
            var newPositions = new HashSet<Point>();
            foreach (Point pos in positions)
            {
                foreach ((Direction dir, Size offset) in Offsets)
                {
                    Point nextPos = pos + offset;
                    if(nextPos.X < 0) continue;
                    if(nextPos.Y < 0) continue;
                    if (nextPos.X >= w) continue;
                    if (nextPos.Y >= h) continue;
                    if (map[nextPos.Y][nextPos.X] == '#') continue;
                    newPositions.Add(nextPos);
                }
            }

            positions = newPositions;
        }

        Console.WriteLine($"Answer: {positions.Count}");
        // Console.WriteLine($"Answer: {distances.Count(x => x.Value == numSteps)}");
        //
        // for (int y = 0; y < h; y++)
        // {
        //     for (int x = 0; x < w; x++)
        //     {
        //         var pt = new Point(x, y);
        //         if (distances.ContainsKey(pt))
        //         {
        //             Console.Write("" + distances[pt] % 10);
        //         }
        //         else
        //         {
        //             Console.Write(map[y][x]);
        //         }
        //
        //     }
        //     Console.WriteLine();
        // }
    }
}