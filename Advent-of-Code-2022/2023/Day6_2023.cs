using System.Diagnostics;

namespace Advent_of_Code_2022._2023;

public static class Day6_2023
{
    public static void Run(string testInputPath, string challengeInputPath)
    {
        // part 1 & 2
        var sw = new Stopwatch();
        sw.Start();
        
        //Solve(File.ReadAllLines(testInputPath));
        Solve(File.ReadAllLines(challengeInputPath));
        
        sw.Stop();
        Console.WriteLine($"Solution took: {sw.ElapsedMilliseconds} ms");
    }
    
    private static void Solve(string[] lines)
    {
        var times = lines[0][5..].Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
        var distances = lines[1][9..].Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
        var input = times.Zip(distances).Select(x => (time: x.First, distance: x.Second));

        long sum = 1;
        foreach (var (time, distance) in input)
        {
            var waysToWin = 0;
            for (int i = 0; i < time; i++)
            {
                var dist = CalcDistanceByMsHeld(i, time);
                if (dist > distance)
                {
                    waysToWin++;
                }
            }

            sum *= waysToWin;
        }

        Console.WriteLine($"Part 1: {sum}");

        var sTime = long.Parse(lines[0][5..].Replace(" ", ""));
        var sDistance = long.Parse(lines[1][9..].Replace(" ", ""));
        long waysToWin2 = 0;
        object waysToWin2o = new();
        Parallel.For(0L, sTime, i =>
        {
            if (i > 0 && i % 1000000 == 0)
            {
                Console.WriteLine($"{i} / {sTime}");
            }

            var dist = CalcDistanceByMsHeld(i, sTime);
            if (dist <= sDistance) return;
            lock (waysToWin2o)
            {
                waysToWin2++;    
            }
        });
        Console.WriteLine($"Part 2: {waysToWin2}");
    }

    private static long CalcDistanceByMsHeld(long held, long total)
    {
        var left = total - held;
        var dist = left * held;
        return dist;
    }
}