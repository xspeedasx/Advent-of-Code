using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Advent_of_Code._2023;

public static class Day8_2023
{
    public static void Run(string testInputPath, string challengeInputPath)
    {
        // part 1 & 2
        var sw = new Stopwatch();
        sw.Start();
        
        //Solve(File.ReadAllLines(testInputPath));
        //Solve(File.ReadAllLines(challengeInputPath));
        
        var testpath2 = Path.Combine(
            Path.GetDirectoryName(testInputPath)!,
            Path.GetFileNameWithoutExtension(testInputPath) + "_2.txt");
        //Solve2(File.ReadAllLines(testpath2));
        Solve2(File.ReadAllLines(challengeInputPath));
        
        sw.Stop();
        Console.WriteLine($"Solution took: {sw.ElapsedMilliseconds} ms");
    }

    private static void Solve(string[] lines)
    {
        (Dictionary<string, (string left, string right)> nodes, string moves) = ParseInput(lines);

        var current = "AAA";
        var moveIdx = 0;
        while (!string.Equals(current, "ZZZ", StringComparison.InvariantCultureIgnoreCase))
        {
            var (left, right) = nodes[current];
            switch (moves[moveIdx%moves.Length])
            {
                case 'L':
                    current = left;
                    break;
                case 'R':
                    current = right;
                    break;
            }
            moveIdx++;
        }

        Console.WriteLine($"Part 1: {moveIdx}");
    }

    private static void Solve2(string[] lines)
    {
        (Dictionary<string, (string left, string right)> nodes, string moves) = ParseInput(lines);

        //Solve2Stupid(nodes, moves); // don't even try...
        Solve2Smart(nodes, moves);
    }

    private static void Solve2Smart(Dictionary<string, (string left, string right)> nodes, string moves)
    {
        string[] aStarts = nodes.Keys.Where(x => x.EndsWith("A")).ToArray();
        string[] zEnds = nodes.Keys.Where(x => x.EndsWith("Z")).ToArray();
        Console.WriteLine($"z: {String.Join(",", zEnds)}");

        var sums = new List<long>();
        foreach (string aStart in aStarts)
        {
            string anode = aStart;
            var zCounts = new int[zEnds.Length];
            var moveIdx = 0;
            int? firstReach = null;
            while (zCounts.Any(x => x == 0))
            {
                char move = moves[moveIdx % moves.Length];
                anode = move == 'L' ? nodes[anode].left : nodes[anode].right;

                if (zEnds.Contains(anode))
                {
                    if (firstReach == null)
                    {
                        Console.WriteLine($"{aStart} reached Z end {anode} after {moveIdx} moves");
                        firstReach = moveIdx;
                        //sums.Add(moveIdx);
                    }
                    else
                    {
                        int loopLen = moveIdx - firstReach.Value;
                        Console.WriteLine($"next Z loop after {loopLen}");
                        sums.Add(loopLen);
                        break;
                    }
                }

                moveIdx++;
            }
        }

        Console.WriteLine("Got loops");

        var lcm = sums[0];
        foreach (long sum in sums[1..])
        {
            lcm = Lcm(lcm, sum);
        }

        Console.WriteLine(lcm);


        static long Gcd(long a, long b)
        {
            while (b != 0)
            {
                long temp = a;
                a = b;
                b = temp % b;
            }

            return a;
        }

        static long Lcm(long a, long b)
        {
            return a * b / Gcd(a, b);
        }
    }

    private static void Solve2Stupid(Dictionary<string, (string left, string right)> nodes, string moves)
    {
        string[] current = nodes.Keys.Where(x => x.EndsWith("A")).ToArray();
        Console.WriteLine($"[{DateTime.Now:s}] Starts: {string.Join(",", current)}");

        var moveIdx = 0;
        while (!AllOnZ(current))
        {
            char move = moves[moveIdx % moves.Length];
            Parallel.For(0, current.Length, i =>
            {
                (string left, string right) = nodes[current[i]];
                current[i] = move == 'L' ? left : right;
            });
            moveIdx++;
            if (moveIdx % 1000000 == 0)
            {
                Console.WriteLine($"[{DateTime.Now:s}] calculated {moveIdx} moves.");
            }
        }

        Console.WriteLine($"Part 2: {moveIdx}");
    }

    private static bool AllOnZ(string[] current)
    {
        foreach (string t in current)
        {
            if (t[2] != 'Z') return false;
        }

        return true;
    }

    private static (Dictionary<string, (string left, string right)> nodes, string moves) ParseInput(string[] lines)
    {
        var nodes = new Dictionary<string, (string left, string right)>();
        var moves = lines[0];

        foreach (string line in lines[2..])
        {
            var matches = Regex.Match(line, @"(\w+) = \((\w+), (\w+)\)");

            var node = matches.Groups[1].Value;
            var left = matches.Groups[2].Value;
            var right = matches.Groups[3].Value;

            nodes[node] = (left, right);
        }

        return (nodes, moves);
    }
}