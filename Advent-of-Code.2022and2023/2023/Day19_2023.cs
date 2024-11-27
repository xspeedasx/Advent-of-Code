using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace Advent_of_Code._2023;

public static class Day19_2023
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

    record struct Part(int X, int M, int A, int S)
    {
        public int Sum => X + M + A + S;
    }
    
    
    private static void Solve(string[] lines)
    {
        long sum = 0;
        var workflows = new Dictionary<string, Func<Part, string>[]>();
        
        var isPart = false;
        foreach (string line in lines)
        {
            if (line == String.Empty)
            {
                isPart = true;
                continue;
            }

            if (!isPart)
            {
                var nameIdx = line.IndexOf('{');
                var name = line[..nameIdx];
                var ruleStrings = line[(nameIdx + 1)..^1].Split(',');

                var rules = ruleStrings.Select(MakeFunc).ToArray();
                workflows[name] = rules;
            }
            else
            {
                var splits = line[1..^1].Split(',');
                var part = new Part(
                    int.Parse(splits[0][2..]),
                    int.Parse(splits[1][2..]),
                    int.Parse(splits[2][2..]),
                    int.Parse(splits[3][2..])
                );
                
                var current = workflows["in"];
                Console.WriteLine($"workflow: in");
                var finished = false;
                while (!finished)
                {
                    string? result = null;
                    foreach (Func<Part,string?> func in current)
                    {
                        result = func(part);
                        if (result == null)
                        {
                            continue;
                        }
                        if (result == "A")
                        {
                            sum += part.Sum;
                            Console.WriteLine($"X={part.X} accepted. sum: {part.Sum}");
                            finished = true;
                            break;
                        }
                        else if (result == "R")
                        {
                            Console.WriteLine($"X={part.X} rejected. sum: {part.Sum}");
                            finished = true;
                            break;
                        }
                        else
                        {
                            Console.WriteLine($"workflow: {result}");
                            current = workflows[result];
                            break;
                        }
                    }
                }

                Console.WriteLine();
            }
        }

        Console.WriteLine($"Answer: {sum}");
    }

    private static Func<Part, string?> MakeFunc(string rule)
    {
        if (!rule.Contains(':'))
        {
            return _ => rule;
        }
        
        string[] splits = rule.Split(":");
        char prop = splits[0][0];
        char condition = splits[0][1];
        int value = int.Parse(splits[0][2..]);
        string result = splits[1];
        return (Part p) =>
        {
            int partVal = prop switch
            {
                'x' => p.X,
                'm' => p.M,
                'a' => p.A,
                's' => p.S
            };

            bool matches = condition switch
            {
                '<' => partVal < value,
                '>' => partVal > value
            };

            return matches ? result : null;
        };
    }
}