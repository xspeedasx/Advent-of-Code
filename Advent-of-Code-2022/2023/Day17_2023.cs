using System.Diagnostics;
using System.Drawing;
using static Advent_of_Code_2022.Directions;

namespace Advent_of_Code_2022._2023;

public static class Day17_2023
{
    public static void Run(string testInputPath, string challengeInputPath)
    {
        var sw = new Stopwatch();
        sw.Start();

        Solve(File.ReadAllLines(testInputPath));
        //Solve(File.ReadAllLines(challengeInputPath));

        sw.Stop();
        Console.WriteLine($"Solution took: {sw.ElapsedMilliseconds} ms");
    }

    private static Dictionary<Point, long> _losses = null!;
    private static Dictionary<Point, List<Point>> _paths = null!;

    private record State(Point CurrentPos, Direction? CurentDir, long TotalHeatloss, int StraightCnt, List<Point> Path);

    private static void Solve(string[] lines)
    {
        int h = lines.Length;
        int w = lines[0].Length;
        var map = new Dictionary<Point, int>();
        for (var y = 0; y < h; y++)
        {
            for (var x = 0; x < w; x++)
            {
                map[new Point(x, y)] = int.Parse(lines[y][x] + "");
            }
        }
        
        _losses = new Dictionary<Point, long>();
        _paths = new Dictionary<Point, List<Point>>();

        var states = new Queue<State>();
        states.Enqueue(new State(new Point(0,0), null, 0, 0, new List<Point>()));
        var lastPos = new Point(w - 1, h - 1);

        while (states.Count > 0)
        {
            State state = states.Dequeue();
            if (state.CurrentPos == lastPos)
            {
                continue;
            }

            if (state.CurrentPos.Y > 0 && state.CurentDir != Direction.DOWN)
            {
                const Direction nextDir = Direction.UP;
                Point nextPos = state.CurrentPos + Offsets[nextDir];
                long nextLoss = state.TotalHeatloss + map[nextPos];

                int nextStraightCnt = state.CurentDir == nextDir
                    ? state.StraightCnt + 1
                    : 1;
                
                if(nextStraightCnt < 3 && (!_losses.TryGetValue(nextPos, out long lastLoss) || nextLoss <= lastLoss))
                {
                    var nextpath = state.Path.ToArray().ToList();
                    nextpath.Add(nextPos);                    
                    states.Enqueue(new State(nextPos, nextDir, nextLoss, nextStraightCnt, nextpath));
                    if(lastLoss == 0 || nextLoss < lastLoss)
                    {
                        _losses[nextPos] = nextLoss;
                        _paths[nextPos] = nextpath;
                    }
                }
            }
            if (state.CurrentPos.X > 0 && state.CurentDir != Direction.RIGHT)
            {
                const Direction nextDir = Direction.LEFT;
                Point nextPos = state.CurrentPos + Offsets[nextDir];
                long nextLoss = state.TotalHeatloss + map[nextPos];
                
                int nextStraightCnt = state.CurentDir == nextDir
                    ? state.StraightCnt + 1
                    : 1;
                    
                if(nextStraightCnt < 3 && (!_losses.TryGetValue(nextPos, out long lastLoss) || nextLoss <= lastLoss))
                {
                    var nextpath = state.Path.ToArray().ToList();
                    nextpath.Add(nextPos);                    
                    states.Enqueue(new State(nextPos, nextDir, nextLoss, nextStraightCnt, nextpath));
                    if(lastLoss == 0 || nextLoss < lastLoss)
                    {
                        _losses[nextPos] = nextLoss;
                        _paths[nextPos] = nextpath;
                    }
                }
            }
            if (state.CurrentPos.Y < h - 1 && state.CurentDir != Direction.UP)
            {
                const Direction nextDir = Direction.DOWN;
                Point nextPos = state.CurrentPos + Offsets[nextDir];
                long nextLoss = state.TotalHeatloss + map[nextPos];
                
                int nextStraightCnt = state.CurentDir == nextDir
                    ? state.StraightCnt + 1
                    : 1;
                    
                if(nextStraightCnt < 3 && (!_losses.TryGetValue(nextPos, out long lastLoss) || nextLoss <= lastLoss))
                {
                    var nextpath = state.Path.ToArray().ToList();
                    nextpath.Add(nextPos);                    
                    states.Enqueue(new State(nextPos, nextDir, nextLoss, nextStraightCnt, nextpath));
                    if(lastLoss == 0 || nextLoss < lastLoss)
                    {
                        _losses[nextPos] = nextLoss;
                        _paths[nextPos] = nextpath;
                    }
                }
            }
            if (state.CurrentPos.X < w - 1 && state.CurentDir != Direction.LEFT)
            {
                const Direction nextDir = Direction.RIGHT;
                Point nextPos = state.CurrentPos + Offsets[nextDir];
                long nextLoss = state.TotalHeatloss + map[nextPos];
                
                int nextStraightCnt = state.CurentDir == nextDir
                    ? state.StraightCnt + 1
                    : 1;
                    
                if(nextStraightCnt < 3 && (!_losses.TryGetValue(nextPos, out long lastLoss) || nextLoss <= lastLoss))
                {
                    var nextpath = state.Path.ToArray().ToList();
                    nextpath.Add(nextPos);                    
                    states.Enqueue(new State(nextPos, nextDir, nextLoss, nextStraightCnt, nextpath));
                    if(lastLoss == 0 || nextLoss < lastLoss)
                    {
                        _losses[nextPos] = nextLoss;
                        _paths[nextPos] = nextpath;
                    }
                }
            }
        }

        List<Point> lastPath = _paths[lastPos];
        Console.WriteLine($"ans: {_losses[lastPos]}. Path: {string.Join(",",lastPath)}");

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                var p = new Point(x, y);
                Console.Write(lastPath.Contains(p) ? '.' : lines[y][x]);
            }
            Console.WriteLine();
        }
    }
}