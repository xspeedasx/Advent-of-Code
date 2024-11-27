using System.Diagnostics;
using System.Drawing;
using System.Security.Cryptography;

namespace Advent_of_Code._2023;

public static class Day14_2023
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

    private static void Solve(string[] map)
    {
        var balls = new HashSet<Point>();
        var rocks = new HashSet<Point>();
        for (var y = 0; y < map.Length; y++)
        {
            for (int x = 0; x < map[y].Length; x++)
            {
                char c = map[y][x];
                if (c == 'O')
                {
                    balls.Add(new Point(x, y));
                }

                if (c == '#')
                {
                    rocks.Add(new Point(x, y));
                }
            }
        }

        Size mapSize = new Size(map[0].Length, map.Length);

        Dictionary<string, HashSet<Point>> cache = new();
        LinkedList<string> loopDetectionList = new();
        HashSet<Point> newBalls = balls;
        var targetCycles = 1_000_000_000;
        for (int i = 0; i < targetCycles; i++)
        {
            string hash1 = GetHash(newBalls);
            // Console.WriteLine($"Cycle {i}: {hash1}");
            if (cache.TryGetValue(hash1, out HashSet<Point>? value))
            {
                newBalls = value;
            }
            else
            {
                newBalls = Roll(mapSize, newBalls, rocks, "N");
                // Console.WriteLine("after N");
                // PrintState(map, newBalls);
                newBalls = Roll(mapSize, newBalls, rocks, "W");
                // Console.WriteLine("after W");
                // PrintState(map, newBalls);
                newBalls = Roll(mapSize, newBalls, rocks, "S");
                // Console.WriteLine("after S");
                // PrintState(map, newBalls);
                newBalls = Roll(mapSize, newBalls, rocks, "E");
                // Console.WriteLine("after E");
                // PrintState(map, newBalls);
                // Console.WriteLine($"after {i} cycles");
                // PrintState(map, newBalls);
                cache[hash1] = newBalls;
                
                string hash2 = GetHash(newBalls);

                LinkedListNode<string>? foundHash = loopDetectionList.Find(hash2);
                if(foundHash is null)
                {
                    loopDetectionList.AddLast(hash2);
                }
                else
                {
                    var cnt = 0;
                    while (foundHash != null)
                    {
                        foundHash = foundHash.Next;
                        cnt++;
                    }

                    int cycles = cnt * ((targetCycles - i) / cnt);
                    Console.WriteLine($"Loop detected! Loop size: {cnt}. Skipping {cycles:# ### ### ###} cycles");
                    i += cycles;

                }
                
            }
            
            if(i > 0 && i % 10 == 0)
            {
                Console.WriteLine($"Cycle {i:# ### ### ###} / {targetCycles} cache size: {cache.Count:# ### ### ###}");
            }
        }

        if(Debugger.IsAttached)
        {
            // PrintState(map, newBalls);
        }

        int load = newBalls.Sum(b => map.Length - b.Y);
        Console.WriteLine($"Load: {load}");
    }

    private static void PrintState(string[] map, HashSet<Point> balls)
    {
        for (var y = 0; y < map.Length; y++)
        {
            for (int x = 0; x < map[y].Length; x++)
            {
                char c = map[y][x];
                if (balls.Contains(new Point(x, y)))
                {
                    Console.Write('O');
                }
                else if (c == '#')
                {
                    Console.Write('#');
                }
                else
                {
                    Console.Write('.');
                }
            }

            Console.WriteLine();
        }
    }

    private static string GetHash(HashSet<Point> balls)
    {
        var bytes = new byte[balls.Count * 8];
        var byteidx = 0;
        foreach (Point ball in balls.OrderBy(x => x.Y).ThenBy(x => x.X))
        {
            var xBytes = BitConverter.GetBytes(ball.X);
            Buffer.BlockCopy(xBytes, 0, bytes, byteidx, 4);
            byteidx += 4;
            var yBytes = BitConverter.GetBytes(ball.Y);
            Buffer.BlockCopy(yBytes, 0, bytes, byteidx, 4);
            byteidx += 4;
        }
    
        return Convert.ToBase64String(MD5.HashData(bytes));
    }

    private static HashSet<Point> Roll(Size mapSize, HashSet<Point> balls, HashSet<Point> rocks, string direction)
    {
        var newBalls = new HashSet<Point>();

        /*if (direction is "N")
        {
            foreach (Point ball in balls)
            {
                var nextObstacles = rocks.Union(newBalls).Where(r => r.X == ball.X && r.Y < ball.Y).ToList();
                if (nextObstacles.Count == 0)
                {
                    newBalls.Add(ball with { Y = 0 });
                }
                else
                {
                    newBalls.Add(ball with { Y = nextObstacles.Max(r => r.Y + 1) });
                }
            }
        } 
        else if (direction is "S")
        {
            foreach (Point ball in balls)
            {
                var nextObstacles = rocks.Union(newBalls).Where(r => r.X == ball.X && r.Y > ball.Y).ToList();
                if (nextObstacles.Count == 0)
                {
                    newBalls.Add(ball with { Y = mapSize.Height-1 });
                }
                else
                {
                    newBalls.Add(ball with { Y = nextObstacles.Min(r => r.Y - 1) });
                }
            }
        }
        else if (direction is "W")
        {
            foreach (Point ball in balls)
            {
                var nextObstacles = rocks.Union(newBalls).Where(r => r.Y == ball.Y && r.X < ball.X).ToList();
                if (nextObstacles.Count == 0)
                {
                    newBalls.Add(ball with { X = 0 });
                }
                else
                {
                    newBalls.Add(ball with { X = nextObstacles.Max(r => r.X + 1) });
                }
            }
        } 
        else if (direction is "E")
        {
            foreach (Point ball in balls)
            {
                var nextObstacles = rocks.Union(newBalls).Where(r => r.Y == ball.Y && r.X > ball.X).ToList();
                if (nextObstacles.Count == 0)
                {
                    newBalls.Add(ball with { X = mapSize.Width-1 });
                }
                else
                {
                    newBalls.Add(ball with { X = nextObstacles.Min(r => r.X - 1) });
                }
            }
        }
        
        return newBalls;*/

        if (direction is "N")
        {
            foreach (Point ball in balls)
            {
                int x = ball.X;
                int y = ball.Y;
                bool settled = false;
                while (y >= 0)
                {
                    var newBall = new Point(x, y);
                    // char c = map[y][x];
                    // if (c == '#' || newBalls.Contains(newBall))
                    if (rocks.Contains(newBall) || newBalls.Contains(newBall))
                    {
                        while (!newBalls.Add(new Point(x, y + 1)))
                        {
                            y++;
                        }
                        settled = true;
                        break;
                    }

                    if (y == 0)
                    {
                        while (!newBalls.Add(newBall))
                        {
                            y++;
                        }
                        settled = true;
                        break;
                    }

                    y--;
                }

                if (settled)
                {
                    continue;
                }

                Console.WriteLine($"Ball: {ball}");
                throw new Exception("shouldn't reach here..");
            }
        }
        else if (direction is "S")
        {
            foreach (Point ball in balls)
            {
                int x = ball.X;
                int y = ball.Y;
                bool settled = false;
                while (y <= mapSize.Height-1)
                {
                    var newBall = new Point(x, y);
                    // char c = map[y][x];
                    // if (c == '#' || newBalls.Contains(newBall))
                    if (rocks.Contains(newBall) || newBalls.Contains(newBall))
                    {
                        while (!newBalls.Add(new Point(x, y - 1)))
                        {
                            y--;
                        }
                        settled = true;
                        break;
                    }

                    if (y == mapSize.Height-1)
                    {
                        while (!newBalls.Add(newBall))
                        {
                            y--;
                        }
                        settled = true;
                        break;
                    }

                    y++;
                }

                if (settled)
                {
                    continue;
                }

                Console.WriteLine($"Ball: {ball}");
                throw new Exception("shouldn't reach here..");
            }
        }
        else if (direction is "W")
        {
            foreach (Point ball in balls)
            {
                int x = ball.X;
                int y = ball.Y;
                bool settled = false;
                while (x >= 0)
                {
                    var newBall = new Point(x, y);
                    // char c = map[y][x];
                    // if (c == '#' || newBalls.Contains(newBall))
                    if (rocks.Contains(newBall) || newBalls.Contains(newBall))
                    {
                        while (!newBalls.Add(new Point(x + 1, y)))
                        {
                            x++;
                        }
                        settled = true;
                        break;
                    }

                    if (x == 0)
                    {
                        while (!newBalls.Add(newBall))
                        {
                            x++;
                        }
                        settled = true;
                        break;
                    }

                    x--;
                }

                if (settled)
                {
                    continue;
                }

                Console.WriteLine($"Ball: {ball}");
                throw new Exception("shouldn't reach here..");
            }
        }
        else if (direction is "E")
        {
            foreach (Point ball in balls)
            {
                int x = ball.X;
                int y = ball.Y;
                bool settled = false;
                while (x <= mapSize.Width-1)
                {
                    var newBall = new Point(x, y);
                    // char c = map[y][x];
                    // if (c == '#' || newBalls.Contains(newBall))
                    if (rocks.Contains(newBall) || newBalls.Contains(newBall))
                    {
                        while (!newBalls.Add(new Point(x - 1, y)))
                        {
                            x--;
                        }
                        settled = true;
                        break;
                    }

                    if (x == mapSize.Width-1)
                    {
                        while (!newBalls.Add(newBall))
                        {
                            x--;
                        }
                        settled = true;
                        break;
                    }

                    x++;
                }

                if (settled)
                {
                    continue;
                }

                Console.WriteLine($"Ball: {ball}");
                throw new Exception("shouldn't reach here..");
            }
        }

        return newBalls;
    }
}