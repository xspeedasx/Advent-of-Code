using System.Collections.Concurrent;
using System.Text;

public static partial class AoCSolution
{
    public static Func<Task> Day14 => async () =>
    {
        Console.WriteLine("===== Day 14 =====");
        var testInput = await File.ReadAllLinesAsync("TestInputs/day14_test.txt");
        var mainInput = await File.ReadAllLinesAsync("Inputs/day14.txt");

        Console.WriteLine("Part 1 solution for test inputs:");
        SolvePart1ForInputs(testInput);
        Console.WriteLine("Part 1 solution for puzzle inputs:");
        SolvePart1ForInputs(mainInput);

        Console.WriteLine("Part 2 solution for test inputs:");
        SolvePart2ForInputs(testInput);
        Console.WriteLine("Part 2 solution for puzzle inputs:");
        SolvePart2ForInputs(mainInput);

        void SolvePart1ForInputs(string[] entries)
        {
            var (polymer, map) = ReadInput(entries);
            Console.WriteLine($"  Polymer: {polymer}");
            
            for(int i = 0; i < 10; i++)
            {
                polymer = ApplyPairInsertion(polymer, map);
            }

            var cCount = new ConcurrentDictionary<char, int>();
            foreach(var c in polymer)
            {
                cCount.AddOrUpdate(c, 1, (_, x) => x+1);
            }

            var mostCommon = cCount.MaxBy(x => x.Value);
            var leastCommon = cCount.MinBy(x => x.Value);
            Console.WriteLine($"    Most Common: {mostCommon.Key} : {mostCommon.Value}");
            Console.WriteLine($"    Least Common: {leastCommon.Key} : {leastCommon.Value}");
            Console.WriteLine($"\tAnswer: {mostCommon.Value - leastCommon.Value}");
        }

        void SolvePart2ForInputs(string[] entries)
        {
            var maxDepth = 40;
            var (polymer, map) = ReadInput(entries);
            
            Console.WriteLine($"  Polymer: {polymer}");

            var theBigCache = new Dictionary<(char a, char b, int depth), long[]>();

            var allUses = new long[26];
            for(int i = 0; i < polymer.Length-1; i++)
            {
                var uses = Traverse(polymer[i], polymer[i + 1], 0);
                for (int c = 0; c < 26; c++)
                {
                    if (uses[c] > 0) 
                    {
                        allUses[c]+=uses[c];
                    }
                }
            }
            allUses[polymer[^1] - 'A']++;

            var mostUses = 0L;
            var leastUses = long.MaxValue;
            for (int c = 0; c < 26; c++)
            {
                var uses = allUses[c];
                if (uses > 0) 
                { 
                    Console.WriteLine($"    {(char)(c + 'A')}: {allUses[c]} uses");
                    if(uses > mostUses) mostUses = uses;
                    if(uses < leastUses) leastUses = uses;
                }
            }

            Console.WriteLine($"\tAnswer is: {mostUses-leastUses}");

            long[] Traverse(char a, char b, int depth)
            {
                var usages = new long[26];
                var newB = map[""+a+b];
                if (depth == maxDepth)
                {
                    usages[a-'A']++;
                    return usages;
                }

                if(theBigCache.TryGetValue((a, b, depth), out var cachedUsages)) return cachedUsages;

                var usages1 = Traverse(a, newB, depth + 1);
                var usages2 = Traverse(newB, b, depth + 1);
                for(int i = 0; i < 26; i++)
                {
                    usages[i] += usages1[i] + usages2[i];
                }
                theBigCache[(a, b, depth)] = usages;
                return usages;
            }
        }

        (string, Dictionary<string, char>) ReadInput(string[] entries)
        {
            var polymer = entries[0];
            var map = entries[2..].Select(x => x.Split(" -> ")).ToDictionary(x => x[0], x => x[1][0]);
            return (polymer, map);
        }

        string ApplyPairInsertion(string polymer, Dictionary<string, char> map)
        {
            var newPol = new StringBuilder();
            for (int x = 0; x < polymer.Length - 1; x++)
            {
                var pair = polymer[x..(x + 2)];
                newPol.Append(polymer[x]);
                newPol.Append(map[pair]);
            }
            newPol.Append(polymer[^1]);
            return newPol.ToString();
        }
    };
}