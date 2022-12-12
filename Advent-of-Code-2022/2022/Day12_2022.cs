using System.Drawing;
using System.Text;

namespace Advent_of_Code_2022._2022;

public static class Day12_2022
{
    public static void Run(string testInputPath, string challengeInputPath)
    {
        // Solve(File.ReadAllLines(testInputPath));
        // Solve(File.ReadAllLines(challengeInputPath));
        Solve(File.ReadAllLines(testInputPath), false);
        Solve(File.ReadAllLines(challengeInputPath), false);
    }
    
    private static void Solve(string[] lines, bool goingUp)
    {
        var steps = 0;
        
        Dictionary<Point, int> distances = new();
        var w = lines[0].Length;
        var h = lines.Length;
        
        (Point start, Point end, int[][] map) = ParseMap(lines);

        if (goingUp)
        {
            ProcessPoint(start, 0);
            if (!distances.TryGetValue(end, out var endDist))
            {
                Console.WriteLine("No end :(");
            }
            Console.WriteLine($"end: {endDist}");
        }
        else
        {
            ProcessPoint(end, 0);

            var aPositions = new List<Point>();
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    if (map[y][x] == 0)
                    {
                        aPositions.Add(new Point(x, y));
                    }
                }
            }
            
            var closest = aPositions.Select(x => distances.ContainsKey(x) ? distances[x] : int.MaxValue).Min();
            Console.WriteLine($"Closest: {closest}");
            PrintDistances();
        }
        
        void ProcessPoint(Point p, int distFromStart)
        {
            // steps++;
            //
            // if (steps > 0 && steps % 2 == 0)
            // {
            //     PrintDistances();
            // }
            
            if (distances.TryGetValue(p, out var pDist))
            {
                if (pDist <= distFromStart)
                    return;
                distances[p] = distFromStart;
            }
            else
            {
                distances.Add(p, distFromStart);
            }

            Point left = p with { X = p.X - 1 };
            if (p.X > 0 && ValidDirection(p, left))
            {
                ProcessPoint(left, distFromStart + 1);
            }
            Point right = p with { X = p.X + 1 };
            if (p.X < w-1 && ValidDirection(p, right))
            {
                ProcessPoint(right, distFromStart + 1);
            }
            Point up = p with { Y = p.Y - 1 };
            if (p.Y > 0 && ValidDirection(p, up))
            {
                ProcessPoint(up, distFromStart + 1);
            }
            Point down = p with { Y = p.Y + 1 };
            if (p.Y < h-1 && ValidDirection(p, down))
            {
                ProcessPoint(down, distFromStart + 1);
            }
        }
    
        bool ValidDirection(Point current, Point next)
        {
            var currElev = map[current.Y][current.X];
            var nextElev = map[next.Y][next.X];
            //return Math.Abs(currElev - nextElev) <= 1;
            return goingUp  
                ? nextElev - currElev <= 1
                : nextElev - currElev >= -1;
        }
        
        void PrintDistances()
        {
            const int cellw = 4;
            var sb = new StringBuilder();

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    if (distances.TryGetValue(new Point(x, y), out var d))
                    {
                        //sb.Append(("*" + (char)(map[y][x] + 'a')).PadLeft(cellw));
                        sb.Append(d.ToString().PadLeft(cellw));
                    }
                    else
                    {
                        sb.Append(((char)(map[y][x] + 'a')).ToString().PadLeft(cellw));
                    }
                }

                sb.AppendLine();
            }

            File.WriteAllText("distances.txt", sb.ToString());
        }
    }
    
    private static (Point Start, Point End, int[][] map) ParseMap(string[] lines)
    {
        var w = lines[0].Length;
        var h = lines.Length;
        Point start = default;
        Point end = default;

        var map = new int[w][];
        Console.WriteLine($"map: {w} x {h}");
        for (int y = 0; y < h; y++)
        {
            map[y] = new int[w];
            for (int x = 0; x < w; x++)
            {
                var c = lines[y][x];
                var elevation = c - 'a';
                if (c == 'S')
                {
                    elevation = 0;
                    start = new Point(x, y);
                }
                else if (c == 'E')
                {
                    elevation = 'z' - 'a';
                    end = new Point(x, y);
                }

                map[y][x] = elevation;
            }
        }

        return (start, end, map);
    }
}
