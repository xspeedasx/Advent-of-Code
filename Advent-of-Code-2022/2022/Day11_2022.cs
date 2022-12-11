#pragma warning disable CS0162
// ReSharper disable HeuristicUnreachableCode
using System.Diagnostics;
using System.Numerics;
using System.Text.RegularExpressions;

namespace Advent_of_Code_2022._2022;

public static class Day11_2022
{
    public const bool DAY11_DEBUG = false; 
    public const bool DAY11_DEBUG_PART2 = true; 
    public static void Run(string testInputPath, string challengeInputPath)
    {
        Solve(File.ReadAllLines(testInputPath), 20);
        Solve(File.ReadAllLines(challengeInputPath), 20);
        // Solve(File.ReadAllLines(testInputPath), 10000, false);
        // Solve(File.ReadAllLines(challengeInputPath), 10000, false);
    }

    private static void Solve(string[] lines, int rounds, bool relief = true)
    {
        Dictionary<int, Monkey> monkeys = ParseMonkeys(lines.Concat(new[] { "" }));

        var inspections = new long[monkeys.Count];
        
        for (var round = 1; round <= rounds; round++)
        {
            if(DAY11_DEBUG)
                Console.WriteLine($":: ROUND {round} ::");

            foreach ((var monkeyId, Monkey monkey) in monkeys)
            {
                if(DAY11_DEBUG)
                    Console.WriteLine($"Monkey {monkeyId}:");

                while (monkey.Items.Count > 0)
                {
                    inspections[monkeyId]++;
                    var item = monkey.Items.Dequeue();

                    if(DAY11_DEBUG)
                        Console.WriteLine($"  Monkey inspects an item with a worry level of {item}.");

                    var sw = Stopwatch.StartNew();
                    var afterInspect = monkey.Operation(item);
                    sw.Stop();
                    if(sw.ElapsedMilliseconds > 1000)
                        Console.WriteLine($"WARNING: Operation took {sw.ElapsedMilliseconds} ms");
                    var aferBored = relief ? BigInteger.Divide(afterInspect, 3) : afterInspect;
                    if(DAY11_DEBUG)
                        Console.WriteLine($"    Monkey gets bored with item. Worry level is divided by 3 to {aferBored}.");
                    
                    sw.Restart();
                    var isTrueTarget = monkey.Test(aferBored);
                    sw.Stop();
                    if(sw.ElapsedMilliseconds > 1000)
                        Console.WriteLine($"WARNING: Test took {sw.ElapsedMilliseconds} ms");
                    
                    var target = isTrueTarget
                        ? monkey.TrueTarget
                        : monkey.FalseTarget;
                    
                    if(DAY11_DEBUG)
                        Console.WriteLine($"    Item with worry level {aferBored} is thrown to monkey {target}.");

                    if (!monkeys.TryGetValue(target, out var targetMonkey))
                    {
                        throw new Exception($"Monkey {target} was out of bounds");
                    }
                    
                    targetMonkey.Items.Enqueue(aferBored);
                }
            }

            if (DAY11_DEBUG)
            {
                Console.WriteLine($"After round {round}, the monkeys are holding items with these worry levels:");
                foreach ((int monkeyId, Monkey monkey) in monkeys)
                {
                    Console.WriteLine($"Monkey {monkeyId}: {String.Join(", ", monkey.Items)}");
                }
            }

            //Console.WriteLine($"Round {round} ended.");
            if(DAY11_DEBUG_PART2 && (round is 1 or 20 || (round > 0 && round % 1000 == 0)))
            {
                Console.WriteLine($"== After round {round} ==");
                for(var i = 0; i < inspections.Length; i++)
                {
                    Console.WriteLine($"Monkey {i} inspected items {inspections[i]} times.");
                }
            }
        }
        
        if (DAY11_DEBUG)
        {
            for(var i = 0; i < inspections.Length; i++)
            {
                Console.WriteLine($"Monkey {i} inspected items {inspections[i]} times.");
            }
        }

        var monkeyBusiness = inspections.OrderByDescending(x => x).Take(2).Aggregate((a, b) => a * b);
        Console.WriteLine($"Monkey business: {monkeyBusiness}");
    }

    private static Dictionary<int, Monkey> ParseMonkeys(IEnumerable<string> lines)
    {
        var idPattern = new Regex(@"Monkey (?<id>\d+):");
        var monkeys = new Dictionary<int, Monkey>();
        var currentMonkey = new Monkey { Id = -1 };
        foreach (var line in lines)
        {
            if (idPattern.Match(line) is { Success: true } idMatch)
            {
                currentMonkey.Id = int.Parse(idMatch.Groups["id"].Value);
            }
            else if (line.StartsWith("  Starting items:"))
            {
                var lineItems = line[18..];
                currentMonkey.Items = new Queue<BigInteger>(
                    lineItems
                        .Split(',', StringSplitOptions.TrimEntries)
                        .Select(x => new BigInteger(int.Parse(x)))
                );
            }
            else if (line.StartsWith("  Operation: "))
            {
                var operation = line[23..];
                var operand = operation[0];

                int? value = operation[2..] == "old"
                    ? null
                    : int.Parse(operation[2..]);

                currentMonkey.Operation = operand == '+'
                    ? value == null
                        ? worry =>
                        {
                            BigInteger addRes = worry + worry;
                            if (DAY11_DEBUG)
                                Console.WriteLine(
                                    $"    Worry level increases by {(value == null ? "itself" : value)} to {addRes}.");
                            return addRes;
                        }
                        : worry =>
                        {
                            BigInteger addRes = worry + value.Value;
                            if (DAY11_DEBUG)
                                Console.WriteLine(
                                    $"    Worry level increases by {(value == null ? "itself" : value)} to {addRes}.");
                            return addRes;
                        }
                    : value == null
                        ? worry =>
                        {
                            BigInteger addRes = worry * worry;
                            // reduce the result somehow?

                            if (DAY11_DEBUG)
                                Console.WriteLine(
                                    $"    Worry level increases by {(value == null ? "itself" : value)} to {addRes}.");
                            return addRes;
                        }
                        : worry =>
                        {
                            BigInteger multRes = worry * value.Value;
                            if (DAY11_DEBUG)
                                Console.WriteLine(
                                    $"    Worry level is multiplied by {(value == null ? "itself" : value)} to {multRes}.");
                            return multRes;
                        };
            }
            else if (line.StartsWith("  Test: "))
            {
                var divPart = line[21..];
                var divider = int.Parse(divPart);

                currentMonkey.Test = worry =>
                {
                    var result = worry % divider == 0;
                    if (DAY11_DEBUG)
                    {
                        Console.WriteLine(
                            $"    Current worry level is{(result ? "" : " not")} divisible by {divider}.");
                    }
                    return result;
                };
            }
            else if (line.StartsWith("    If true:"))
            {
                var trueTargetS = line[29..];
                currentMonkey.TrueTarget = int.Parse(trueTargetS);
            }
            else if (line.StartsWith("    If false:"))
            {
                var falseTargetS = line[30..];
                currentMonkey.FalseTarget = int.Parse(falseTargetS);
            }
            else
            {
                if (currentMonkey.Id != -1)
                {
                    monkeys[currentMonkey.Id] = currentMonkey;
                }

                currentMonkey = new Monkey { Id = -1 };
            }
        }

        return monkeys;
    }
    
    class Monkey
    {
        public int Id { get; set; }
        public Queue<BigInteger> Items { get; set; } = null!;

        /// <summary>
        /// Changes worry level of currently held item
        /// </summary>
        public Func<BigInteger, BigInteger> Operation { get; set; } = null!;

        /// <summary>
        /// Determines next target by testing worry level
        /// </summary>
        public Func<BigInteger, bool> Test { get; set; } = null!;

        public int TrueTarget { get; set; }
        public int FalseTarget { get; set; }
    }
}
