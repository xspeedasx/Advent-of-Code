using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Advent_of_Code._2023;

public static class Day15_2023
{
    public static void Run(string testInputPath, string challengeInputPath)
    {
        var sw = new Stopwatch();
        sw.Start();

        //Solve(File.ReadAllLines(testInputPath));
        //Solve(File.ReadAllLines(challengeInputPath));
        
        //Solve2(File.ReadAllLines(testInputPath));
        Solve2(File.ReadAllLines(challengeInputPath));

        sw.Stop();
        Console.WriteLine($"Solution took: {sw.ElapsedMilliseconds} ms");
    }

    record Lens(string Label, int FocalLength);

    private static void Solve2(string[] lines)
    {
        var splits = lines[0].Split(",");
        var boxes = Enumerable.Range(0, 256).Select(_ => new List<Lens>()).ToArray();

        foreach (var split in splits)
        {
            Match match = Regex.Match(split, @"(\w+)([=-])(\d?)");
            var label = match.Groups[1].Value;
            var op = match.Groups[2].Value;
            var num = string.IsNullOrEmpty(match.Groups[3].Value) ? 0 : int.Parse(match.Groups[3].Value);
            
            var boxId = Hash(label);
            var box = boxes[boxId];

            if (op == "-")
            {
                box.RemoveAll(x => x.Label == label);
            }
            else if (op == "=")
            {
                var oldLens = box.FirstOrDefault(x => x.Label == label);
                if (oldLens != null)
                {
                    var index = box.IndexOf(oldLens);
                    box[index] = oldLens with { FocalLength = num };
                }
                else
                {
                    box.Add(new Lens(label, num));
                }
            }
            else
            {
                throw new Exception($"unsupported operation: {op}");
            }

            if(Debugger.IsAttached)
            {
                Console.WriteLine($"After \"{split}\":");
                for (int i = 0; i < boxes.Length; i++)
                {
                    var b = boxes[i];
                    if (b.Count == 0) continue;
                    Console.WriteLine($"Box {i}: {string.Join(" ", b.Select(x => $"[{x.Label} {x.FocalLength}]"))}");
                }
                Console.WriteLine();
            }
        }

        long sum = 0;
        for (int boxIndex = 0; boxIndex < boxes.Length; boxIndex++)
        {
            var box = boxes[boxIndex];
            for (int lensIndex = 0; lensIndex < box.Count; lensIndex++)
            {
                var lens = box[lensIndex];
                var focusingPower = (boxIndex + 1) * (lensIndex + 1) * lens.FocalLength;
                sum += focusingPower;

                if (Debugger.IsAttached)
                {
                    Console.WriteLine($"{lens.Label}: {boxIndex+1} (box {boxIndex}) * {lensIndex+1} (slot) * {lens.FocalLength} (focal length) = {focusingPower}");
                }
            }
        }

        Console.WriteLine($"Answer: {sum}");
    }
    
    private static void Solve(string[] lines)
    {
        var splits = lines[0].Split(",");
        long sum = 0;
        foreach (var split in splits)
        {
            var hash = Hash(split);
            sum += hash;
            //Console.WriteLine($"{split} becomes {hash}");
        }

        Console.WriteLine($"Answer: {sum}");
    }

    private static int Hash(string input)
    {
        var sum = 0;
        foreach (var c in input)
        {
            sum = ((sum + c) * 17) % 256;
        }

        return sum;
    }
}