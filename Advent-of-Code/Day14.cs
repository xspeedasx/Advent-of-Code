using System.Collections.Concurrent;
using System.Text;

public static partial class AoCSolution
{
    public static Func<Task> Day14 => async () =>
    {
        Console.WriteLine("===== Day 14 =====");
        var testInput = await File.ReadAllLinesAsync("TestInputs/day14_test.txt");
        var mainInput = await File.ReadAllLinesAsync("Inputs/day14.txt");

        //Console.WriteLine("Part 1 solution for test inputs:");
        //SolvePart1ForInputs(testInput);
        //Console.WriteLine("Part 1 solution for puzzle inputs:");
        //SolvePart1ForInputs(mainInput);

        Console.WriteLine("Part 2 solution for test inputs:");
        SolvePart2ForInputs(testInput);
        //Console.WriteLine("Part 2 solution for puzzle inputs:");
        //SolvePart2ForInputs(mainInput);

        void SolvePart1ForInputs(string[] entries)
        {
            var (polymer, map) = ReadInput(entries);
            Console.WriteLine($"Polymer: {polymer}");
            
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
            Console.WriteLine($"Most Common: {mostCommon.Key} : {mostCommon.Value}");
            Console.WriteLine($"Least Common: {leastCommon.Key} : {leastCommon.Value}");
            Console.WriteLine($"\tAnswer: {mostCommon.Value - leastCommon.Value}");
        }

        void SolvePart2ForInputs(string[] entries)
        {
            var (polymer, map) = ReadInput(entries);
            Console.WriteLine($"Polymer: {polymer}");

            var endCnt = 4L;
            for(int i = 0; i < 40; i++)
            {
                endCnt = endCnt * 2 - 1;
            }
            Console.WriteLine($"EndLen: {endCnt}");

        }

        (string, Dictionary<string, string>) ReadInput(string[] entries)
        {
            var polymer = entries[0];
            var map = entries[2..].Select(x => x.Split(" -> ")).ToDictionary(x => x[0], x => x[1]);
            return (polymer, map);
        }

        string ApplyPairInsertion(string polymer, Dictionary<string, string> map)
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