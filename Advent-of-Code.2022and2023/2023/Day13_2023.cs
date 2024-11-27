using System.Diagnostics;

namespace Advent_of_Code._2023;

public static class Day13_2023
{
    public static void Run(string testInputPath, string challengeInputPath)
    {
        var sw = new Stopwatch();
        sw.Start();

        //Solve(File.ReadAllLines(testInputPath));
        //Solve(File.ReadAllLines(challengeInputPath));
        
        //Solve(File.ReadAllLines(testInputPath), 1);
        Solve(File.ReadAllLines(challengeInputPath), 1);

        sw.Stop();
        Console.WriteLine($"Solution took: {sw.ElapsedMilliseconds} ms");
    }

    private static void Solve(string[] lines, int errCnt = 0)
    {
        long sum = 0;
        List<string> pattern = new List<string>();
        foreach (var line in lines)
        {
            if (string.IsNullOrEmpty(line))
            {
                sum += ProcessPattern(pattern.ToArray(), errCnt);
                pattern.Clear();
                continue;
            }
            pattern.Add(line);
        }
        sum += ProcessPattern(pattern.ToArray(), errCnt);
        
        Console.WriteLine($"Answer: {sum}");
    }

    private static long ProcessPattern(string[] rows, int errCnt)
    {
        var h = rows.Length;
        var w = rows[0].Length;
        var pattern = new char[h, w];
        for (var y = 0; y < h; y++)
            for (var x = 0; x < w; x++)
            {
                pattern[y, x] = rows[y][x];
            }

        long answer = 0;

        // scan columns
        Parallel.For(1, w, xOffset =>
        {
            var errors = 0;
            var windowSize = Math.Min(xOffset, w - xOffset);
            for (var x = 0; x < windowSize; x++)
            {
                var leftX = xOffset - 1 - x;
                var rightX = xOffset + x;
                for (var y = 0; y < h; y++)
                {
                    if (pattern[y, leftX] != pattern[y, rightX])
                    {
                        errors++;
                        
                        if (errors > errCnt)
                        {
                            break;
                        }
                    }
                }
            }

            if(errors == errCnt)
            {
                Interlocked.Add(ref answer, xOffset);
            }
        });

        // scan rows
        Parallel.For(1, h, yOffset =>
        {
            var errors = 0;
            var windowSize = Math.Min(yOffset, h - yOffset);
            for (var y = 0; y < windowSize; y++)
            {
                var topY = yOffset - 1 - y;
                var bottomY = yOffset + y;
                for (var x = 0; x < w; x++)
                {
                    if (pattern[topY, x] != pattern[bottomY, x])
                    {
                        errors++;
                        
                        if (errors > errCnt)
                        {
                            break;
                        }
                    }
                }
            }

            if(errors == errCnt)
            {
                Interlocked.Add(ref answer, yOffset * 100);
            }
        });

        if (answer == 0)
        {
            Console.WriteLine("ANSWER WAS 0... pattern:");
            foreach (var row in rows)
            {
                Console.WriteLine(row);
            }
        }
        
        return answer;
    }
}