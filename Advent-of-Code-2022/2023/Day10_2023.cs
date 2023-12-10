using System.Diagnostics;
using System.Drawing;

namespace Advent_of_Code_2022._2023;

public static class Day10_2023
{
    public static void Run(string testInputPath, string challengeInputPath)
    {
        // part 1 & 2
        var sw = new Stopwatch();
        sw.Start();

        //Solve(File.ReadAllLines(testInputPath));
        var testpath2 = Path.Combine(
            Path.GetDirectoryName(testInputPath)!,
            Path.GetFileNameWithoutExtension(testInputPath) + "_2.txt");
        //Solve(File.ReadAllLines(testpath2));
        Solve(File.ReadAllLines(challengeInputPath));

        sw.Stop();
        Console.WriteLine($"Solution took: {sw.ElapsedMilliseconds} ms");
    }

    private static void Solve(string[] map)
    {
        int w = map[0].Length;
        int h = map.Length;
        Point s = new Point(0, 0);
        for (int y = 0; y < h; y++)
        {
            if (map[y].Contains('S'))
            {
                s = new Point(map[y].IndexOf('S'), y);
                Console.WriteLine($"start at: {s}");
            }
        }

        char? sReplacement = DetermineTile(map, s);
        Console.WriteLine($"S seems to be: {sReplacement}");
        var path = new HashSet<Point>();
        var stepCnt = 0;
        Point currentPos = s;
        Point? prevPos = null;
        do
        {
            List<Point> nextSteps = GetPossibleDirections(map, currentPos);
            if(Debugger.IsAttached)
            {
                Console.WriteLine($"current: {currentPos} {map[currentPos.Y][currentPos.X]}");
                Console.WriteLine("next steps:");
                foreach (Point nextStep in nextSteps)
                {
                    Console.WriteLine($"{nextStep} : {map[nextStep.Y][nextStep.X]}");
                }
                Console.WriteLine();
            }
            Point lastPos = currentPos;
            currentPos = nextSteps.First(x => x != prevPos);
            path.Add(currentPos);
            prevPos = lastPos;
            
            stepCnt++;
        } while (currentPos != s);

        Console.WriteLine($"Steps: {stepCnt}");
        Console.WriteLine($"Part 1: {stepCnt/2}");

        var pathMap = new char[h][];
        for (var y = 0; y < h; y++)
        {
            pathMap[y] = new char[w];
            for (var x = 0; x < w; x++)
            {
                char mapTile = path.Contains(new Point(x, y)) ? map[y][x] : '.';
                if (mapTile == 'S') mapTile = sReplacement!.Value;
                pathMap[y][x] = mapTile;
                Console.Write(mapTile);
            }
            Console.WriteLine();
        }

        Console.WriteLine(" --- ");

        var insideCount = 0;
        for (var y = 0; y < h; y++)
        {
            char? glideStart = null;
            bool isInside = false;
            for (var x = 0; x < w; x++)
            {
                var mapTile = pathMap[y][x];
                if (mapTile == '.')
                {
                    if (isInside)
                    {
                        Console.Write('I');
                        insideCount++;
                    }
                    else Console.Write('O');
                }
                else
                {
                    Console.Write(Translate(mapTile));

                    if (glideStart == null)
                    {
                        if (mapTile == '|') isInside = !isInside;
                        else if ("FL".Contains(mapTile))
                        {
                            glideStart = mapTile;
                        }
                        else throw new InvalidOperationException("corrupted state!");
                    }
                    else
                    {
                        switch (mapTile)
                        {
                            case '-':
                                break;
                            case 'J':
                                switch (glideStart)
                                {
                                    case 'F':
                                        glideStart = null;
                                        isInside = !isInside;
                                        break;
                                    case 'L':
                                        glideStart = null;
                                        break;
                                    default:
                                        throw new InvalidOperationException("corrupted state!");
                                }
                                break;
                            case '7' when glideStart == 'F':
                                glideStart = null;
                                break;
                            case '7' when glideStart == 'L':
                                glideStart = null;
                                isInside = !isInside;
                                break;
                            case '7':
                                throw new InvalidOperationException("corrupted state!");
                            default:
                                throw new InvalidOperationException("corrupted state!");
                        }
                    }
                }
                //Console.Write(path.Contains(new Point(x, y)) ? map[y][x] : '.');
            }
            Console.WriteLine();
        }

        Console.WriteLine($"Part 2: {insideCount}");
    }

    private static char Translate(char mapTile)
    {
        return mapTile switch
        {
            '7' => '┐',
            'J' => '┘',
            'L' => '└',
            'F' => '┌',
            '-' => '─',
            '|' => '│',
            _ => mapTile
        };
    }

    private static char? DetermineTile(string[] map, Point s)
    {
        List<Point> nextSteps = GetPossibleDirections(map, s);
        if (nextSteps.Count != 2) return null;

        var diffNext = new Point(nextSteps[1].X - s.X, nextSteps[1].Y - s.Y);
        var diffPrev = new Point(nextSteps[0].X - s.X, nextSteps[0].Y - s.Y);

        return diffNext switch
        {
            { X: -1 } => diffPrev switch // left
            {
                { X: 1 } => '-',
                { Y: -1 } => 'J',
                { Y: 1 } => '7',
                _ => null
            },
            { X: 1 } => diffPrev switch // right
            {
                { X: -1 } => '-',
                { Y: -1 } => 'L',
                { Y: 1 } => 'F',
                _ => null
            },
            { Y: -1 } => diffPrev switch // up
            {
                { Y: 1 } => '|',
                { X: -1 } => 'J',
                { X: 1 } => 'L',
                _ => null
            },
            { Y: 1 } => diffPrev switch // down
            {
                { Y: -1 } => '|',
                { X: -1 } => '7',
                { X: 1 } => 'F',
                _ => null
            },
            _ => null
        };
    }

    private static List<Point> GetPossibleDirections(string[] map, Point s)
    {
        int w = map[0].Length;
        int h = map.Length;
        var nextSteps = new List<Point>();
        var c = map[s.Y][s.X];
        // top
        if (s.Y > 0 && "SJL|".Contains(c))
        {
            switch (map[s.Y - 1][s.X])
            {
                case '|':
                case '7':
                case 'F':
                case 'S':
                    nextSteps.Add(s with { Y = s.Y - 1 });
                    break;
            }
        }

        // bottom
        if (s.Y < h - 1 && "SF7|".Contains(c))
        {
            switch (map[s.Y + 1][s.X])
            {
                case '|':
                case 'J':
                case 'L':
                case 'S':
                    nextSteps.Add(s with { Y = s.Y + 1 });
                    break;
            }
        }

        // left
        if (s.X > 0 && "SJ7-".Contains(c))
        {
            switch (map[s.Y][s.X - 1])
            {
                case '-':
                case 'L':
                case 'F':
                case 'S':
                    nextSteps.Add(s with { X = s.X - 1 });
                    break;
            }
        }

        // right
        if (s.X < w - 1 && "SFL-".Contains(c))
        {
            switch (map[s.Y][s.X + 1])
            {
                case '-':
                case 'J':
                case '7':
                case 'S':
                    nextSteps.Add(s with { X = s.X + 1 });
                    break;
            }
        }

        return nextSteps;
    }
}