using System.Diagnostics;
using System.Drawing;

namespace Advent_of_Code_2022._2023;

public static class Day11_2023
{
    public static void Run(string testInputPath, string challengeInputPath)
    {
        var sw = new Stopwatch();
        sw.Start();

        // part 1
        // Solve(File.ReadAllLines(testInputPath), 2);
        // Solve(File.ReadAllLines(challengeInputPath), 2);

        // part 2
        // Solve(File.ReadAllLines(testInputPath), 10);
        // Solve(File.ReadAllLines(testInputPath), 100);
        Solve(File.ReadAllLines(challengeInputPath), 1000000);
        sw.Stop();
        Console.WriteLine($"Solution took: {sw.ElapsedMilliseconds} ms");
    }

    private static void Solve(string[] map, int expansionSize)
    {
        (HashSet<int> rows, HashSet<int> cols) = FindExpansionRowsCols(map);
        int h = map.Length;
        int w = map[0].Length;

        var galaxies = new List<Point>();
        
        for (var y = 0; y < h; y++)
        {
            for (var x = 0; x < w; x++)
            {
                if (map[y][x] == '#')
                {
                    galaxies.Add(new Point(x, y));
                }
            }

            //Console.WriteLine(map[y]);
        }

        Console.WriteLine($"found {galaxies.Count} galaxies, {Math.Pow(galaxies.Count-1, 2)/2} pairs.");

        long sum = 0;
        for (int i = 0; i < galaxies.Count - 1; i++)
        {
            for (int j = i+1; j < galaxies.Count; j++)
            {
                Point a = galaxies[i];
                Point b = galaxies[j];
                int minX = Math.Min(a.X, b.X);
                int maxX = Math.Max(a.X, b.X);
                int minY = Math.Min(a.Y, b.Y);
                int maxY = Math.Max(a.Y, b.Y); 
                int emptiesHorizontal = cols.Count(x => x > minX && x < maxX);
                int emptiesVertical = rows.Count(y => y > minY && y < maxY);
                long dist = Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y) + (long)(emptiesHorizontal * (expansionSize - 1)) +
                           (long)(emptiesVertical * (expansionSize-1));
                //Console.WriteLine($"Between galaxy {i+1} ({a}) and galaxy {j+1} ({b}): {dist}");
                sum += dist;
            }
        }

        Console.WriteLine($"Answer: {sum}");
    }

    private static (HashSet<int> rows, HashSet<int> cols) FindExpansionRowsCols(string[] map)
    {
        int h = map.Length;
        int w = map[0].Length;
        HashSet<int> cols = Enumerable.Range(0, w).ToHashSet();
        HashSet<int> rows = Enumerable.Range(0, h).ToHashSet();

        for (var y = 0; y < h; y++)
        {
            for (var x = 0; x < w; x++)
            {
                if (map[y][x] == '#')
                {
                    cols.Remove(x);
                    rows.Remove(y);
                }
            }
        }

        return (rows, cols);
    }
}