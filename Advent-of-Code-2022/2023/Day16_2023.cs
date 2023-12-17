using System.Diagnostics;
using System.Drawing;

namespace Advent_of_Code_2022._2023;

public static class Day16_2023
{
    public static void Run(string testInputPath, string challengeInputPath)
    {
        var sw = new Stopwatch();
        sw.Start();

        //Solve(File.ReadAllLines(testInputPath));
        Solve(File.ReadAllLines(challengeInputPath));

        sw.Stop();
        Console.WriteLine($"Solution took: {sw.ElapsedMilliseconds} ms");
    }

    private static Dictionary<Direction, Size> Offsets = new()
    {
        [Direction.UP] = new Size(0, -1),
        [Direction.DOWN] = new Size(0, 1),
        [Direction.LEFT] = new Size(-1, 0),
        [Direction.RIGHT] = new Size(1, 0)
    };

    private enum Direction
    {
        UP,
        DOWN,
        LEFT,
        RIGHT
    };

    private static void Solve(string[] map)
    {
        int h = map.Length;
        int w = map[0].Length;
        var tasks = new List<Task<long>>();

        long part1 = CalcSum(map, new Point(-1, 0), Direction.RIGHT);
        Console.WriteLine($"Part 1: {part1}");
        for (int x = 0; x < w; x++)
        {
            int x1 = x;
            tasks.Add(new Task<long>(() => CalcSum(map, new Point(x1, -1), Direction.DOWN)));
            int x2 = x;
            tasks.Add(new Task<long>(() => CalcSum(map, new Point(x2, h), Direction.UP)));
        }
        for (int y = 0; y < h; y++)
        {
            int y1 = y;
            tasks.Add(new Task<long>(() => CalcSum(map, new Point(-1, y1), Direction.RIGHT)));
            int y2 = y;
            tasks.Add(new Task<long>(() => CalcSum(map, new Point(w, y2), Direction.LEFT)));
        }

        foreach (Task<long> task in tasks)
        {
            task.Start();
        }

        var results = Task.WhenAll(tasks).Result;
        Console.WriteLine($"Part 2: {results.Max()}");
    }

    private static long CalcSum(string[] map, Point startPos, Direction startDir)
    {
        int h = map.Length;
        int w = map[0].Length;
        var tileDirs = new HashSet<Direction>?[h, w];
        var lazors = 1;
        Traverse(startPos, startDir, lazors);

        long sum = 0;
        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                HashSet<Direction> dirs = tileDirs[y,x];
                if (dirs == null)
                {
                    // Console.Write('.');
                    continue;
                }
                
                // Console.Write(dirs.Count > 0 ? '#' : '.');
                sum += dirs.Count > 0 ? 1 : 0;
            }

            //Console.WriteLine();
        }

        return sum;

        void Traverse(Point pos, Direction dir, int lIdx)
        {
            Size dirOffset = Offsets[dir];
            pos += dirOffset;
            if (pos.X < 0 || pos.X >= w || pos.Y < 0 || pos.Y >= h)
            {
                return;
            }

            //energized[pos.Y, pos.X]++;
            tileDirs[pos.Y, pos.X] ??= new HashSet<Direction>();
            
            if (!tileDirs[pos.Y, pos.X].Add(dir))
            {
                return;
            } 
            //Console.WriteLine($"lazor {lIdx} pos: {pos}, encountered: {map[pos.Y][pos.X]}");

            switch (map[pos.Y][pos.X])
            {
                case '|':
                    if (dir == Direction.LEFT || dir == Direction.RIGHT)
                    {
                        lazors++;
                        Traverse(pos, Direction.UP, lIdx);
                        Traverse(pos, Direction.DOWN, lazors);
                    }
                    else
                    {
                        Traverse(pos, dir, lIdx);
                    }
                    break;
                case '-':
                    if (dir == Direction.UP || dir == Direction.DOWN)
                    {
                        lazors++;
                        Traverse(pos, Direction.LEFT, lIdx);
                        Traverse(pos, Direction.RIGHT, lazors);
                    }
                    else
                    {
                        Traverse(pos, dir, lIdx);
                    }
                    break;
                case '/':
                    if (dir == Direction.UP)
                    {
                        Traverse(pos, Direction.RIGHT, lIdx);
                    } 
                    else if (dir == Direction.DOWN)
                    {
                        Traverse(pos, Direction.LEFT, lIdx);
                    } 
                    else if (dir == Direction.LEFT)
                    {
                        Traverse(pos, Direction.DOWN, lIdx);
                    } 
                    else if (dir == Direction.RIGHT)
                    {
                        Traverse(pos, Direction.UP, lIdx);
                    }
                    break;
                case '\\':
                    if (dir == Direction.UP)
                    {
                        Traverse(pos, Direction.LEFT, lIdx);
                    } 
                    else if (dir == Direction.DOWN)
                    {
                        Traverse(pos, Direction.RIGHT, lIdx);
                    } 
                    else if (dir == Direction.LEFT)
                    {
                        Traverse(pos, Direction.UP, lIdx);
                    } 
                    else if (dir == Direction.RIGHT)
                    {
                        Traverse(pos, Direction.DOWN, lIdx);
                    }
                    break;
                default:                        
                    Traverse(pos, dir, lIdx);
                    break;
            }
        }
    }
}