using System.Diagnostics;
using System.Drawing;
using System.Text.RegularExpressions;

namespace Advent_of_Code._2023;

public static class Day3_2023
{
    public static void Run(string testInputPath, string challengeInputPath)
    {
        // part 1 & 2
        Solve(File.ReadAllLines(testInputPath));
        Solve(File.ReadAllLines(challengeInputPath));
    }
    
    private static void Solve(string[] lines)
    {
        var sum = 0;
        var numbers = new Dictionary<Point, int>();
        var symbols = new Dictionary<Point, (char type, List<int> parts)>();
        var mapWidth = lines[0].Length;
        var mapHeight = lines.Length;

        for (int y = 0; y < lines.Length; y++)
        {
            string line = lines[y];

            var matches = Regex.Matches(line, @"\d+");
            foreach (Match match in matches)
            {
                var num = int.Parse(match.Value);
                var x = match.Index;
                var pos = new Point(x, y);
                numbers.Add(pos, num);
                WriteLine($"Found for number {num} {pos} at {x},{y}");
            }

            var symbolMatches = Regex.Matches(line, @"[^\.\d]{1}");
            foreach (Match match in symbolMatches)
            {
                symbols.Add(new Point(match.Index, y), (match.Value[0], new List<int>()));
            }
        }

        var notFounds = new List<(int num, Point pos)>();
        foreach (var (pos, num) in numbers)
        {
            var numLen = NumberLength(num);
            // search area dimensions
            var minX = Math.Max(0, pos.X - 1);
            var maxX = Math.Min(mapWidth - 1, pos.X + numLen);
            var minY = Math.Max(0, pos.Y - 1);
            var maxY = Math.Min(mapHeight - 1, pos.Y + 1);

            var found = false;
            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    if (symbols.TryGetValue(new Point(x, y), out var symbol))
                    {
                        sum += num;
                        WriteLine($"Found for number {num} {pos} at {x},{y}");
                        found = true;
                        symbol.parts.Add(num);
                        
                        break;
                    }
                }
                if(found) break;
            }

            if (!found)
            {
                WriteLine($" ===== NOT FOUND for for number {num} {pos} ===== ");
                for (int y = minY; y <= maxY; y++)
                {
                    for (int x = minX; x <= maxX; x++)
                    {
                        Write(lines[y][x]);
                    }
                    WriteLine();
                }
            }
        }

        Console.WriteLine($"part 1 answer: {sum}");

        var sum2 = 0;
        foreach (var (type, parts) in symbols.Values)
        {
            if(type != '*') continue;
            if(parts.Count != 2) continue;

            sum2 += parts[0] * parts[1];
        }
        Console.WriteLine($"part 2 answer: {sum2}");
    }

    private static int NumberLength(int num)
    {
        return (int)Math.Floor(Math.Log10(num)) + 1;
    }

    private static void Write(char @char) => Write(@char.ToString());
    private static void Write(string message = "")
    {
        if(!Debugger.IsAttached)
        {
            return;
        }
        Console.Write(message);
    }
    
    private static void WriteLine(string message = "")
    {
        if(!Debugger.IsAttached)
        {
            return;
        }
        Console.WriteLine(message);
    }
}