using System.Drawing;
using System.Numerics;
using System.Text;

namespace Advent_of_Code_2022._2022;

public static class Day14_2022
{
    public static void Run(string testInputPath, string challengeInputPath)
    {
        //Solve(File.ReadAllLines(testInputPath));
        //Solve(File.ReadAllLines(challengeInputPath));
        Solve(File.ReadAllLines(testInputPath), true);
        Solve(File.ReadAllLines(challengeInputPath), true);
    }

    private static void Solve(IEnumerable<string> lines, bool withFloor = false)
    {
        List<List<Point>> paths = ParsePaths(lines, out int maxy);
        // int w = maxx - minx + 1;
        // int h = maxy + 1;
        //char[][] map = RenderMap(paths, w, h, minx);
        
        Dictionary<Point, char> map = RenderMap(paths);

        Point? settled = new Point(int.MaxValue, int.MaxValue);
        var steps = 0;

        var sourcePoint = new Point(500, 0);
        
        while (settled != null)
        {
            steps++;
            if(withFloor && map.ContainsKey(sourcePoint))
                break;
            settled = DropSand(ref map, maxy, withFloor);

            // if (steps == 24)
            // {
            // }
            // if (steps == 100)
            //     break;
        }

        DrawMap(map, maxy, withFloor);
        Console.WriteLine($"{steps-1} units of sand come to rest before sand starts flowing into the abyss below");
    }

    private static void DrawMap(Dictionary<Point,char> map, int maxy, bool withFloor)
    {
        IEnumerable<IGrouping<int, KeyValuePair<Point, char>>> lines = map.GroupBy(x => x.Key.Y);
        var minx = int.MaxValue;
        var maxx = int.MinValue;
        foreach (var p in map.Keys)
        {
            if (p.X < minx) minx = p.X;
            if (p.X > maxx) maxx = p.X;
        }

        var sb = new StringBuilder();

        for (var y = 0; y <= maxy + (withFloor ? 2 : 0); y++)
        {
            if (withFloor && y == maxy + (withFloor ? 2 : 0))
            {
                sb.AppendLine(new string('#', maxx - minx + 1));
                continue;
            }

            for (int x = minx; x <= maxx; x++)
            {
                if (x == 500 && y == 0)
                {
                    sb.Append('S');
                    continue;
                }
                var currentPoint = new Point(x,y);
                var c = map.ContainsKey(currentPoint) ? map[currentPoint] : '.';
                sb.Append(c);
            }

            sb.AppendLine();
        }
        
        File.WriteAllText("day14.txt", sb.ToString());
    }

    private static Point? DropSand(ref Dictionary<Point, char> map, int maxy, bool withFloor = false)
    {
        // int w = map[0].Length;
        // int h = map.Length;
        
        var sand = new Point(500, 0);

        while (true)
        {
            //check down
            if (!withFloor && sand.Y == maxy)
            {
                // falls out of map
                return null;
            }

            if (withFloor && sand.Y == maxy + 1)
            {
                map[sand] = 'o';
                return sand;
            }

            Point mapDown = sand with { Y = sand.Y + 1 };
            if (!map.ContainsKey(mapDown))
            {
                sand = mapDown;
                continue;
            }
            
            // check left
            Point mapLeftDown = sand with { Y = sand.Y + 1, X = sand.X - 1 };
            if (!map.ContainsKey(mapLeftDown))
            {
                sand = mapLeftDown;
                continue;
            }
            // check left
            Point mapRightDown = sand with { Y = sand.Y + 1, X = sand.X + 1 };
            if (!map.ContainsKey(mapRightDown))
            {
                sand = mapRightDown;
                continue;
            }
            
            map[sand] = 'o';
            return sand;
        }
    }

    private static Dictionary<Point, char> RenderMap(List<List<Point>> paths)
    {
        // var map = new char[h][];
        // for (int i = 0; i < h; i++)
        // {
        //     map[i] = new String('.', w).ToCharArray();
        // }
        var map = new Dictionary<Point, char>();
        foreach (List<Point> path in paths)
        {
            if (path.Count == 1)
            {
                Point p = path[0];
                map[p] = '#';
                continue;
            }

            for (var i = 0; i < path.Count - 1; i++)
            {
                Point from = path[i];
                Point to = path[i + 1];
                var diff = new Point(to.X - from.X, to.Y - from.Y);
                int dist = Math.Max(Math.Abs(diff.X), Math.Abs(diff.Y));
                var step = new Point(Math.Clamp(diff.X, -1, 1), Math.Clamp(diff.Y, -1, 1));
                for (var j = 0; j <= dist; j++)
                {
                    var currentPos = new Point(from.X + step.X * j, from.Y + step.Y * j);
                    //map[currentPos.Y][currentPos.X-minx] = '#';
                    map[currentPos] = '#';
                }
            }
        }

        return map;
    }

    private static List<List<Point>> ParsePaths(IEnumerable<string> lines, out int maxy)
    {
        maxy = int.MinValue;
        
        var paths = new List<List<Point>>();
        foreach (string line in lines)
        {
            string[] nodes = line.Split("->", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            var path = new List<Point>();
            
            foreach (string node in nodes)
            {
                string[] splits = node.Split(',');
                int x = int.Parse(splits[0]);
                int y = int.Parse(splits[1]);

                if (y > maxy) maxy = y;
                path.Add(new Point(x, y));
            }
            
            paths.Add(path);
        }

        return paths;
    }
}
