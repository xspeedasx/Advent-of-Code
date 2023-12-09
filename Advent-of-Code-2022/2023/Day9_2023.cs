using System.Diagnostics;

namespace Advent_of_Code_2022._2023;

public static class Day9_2023
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
        var sum = 0;
        var sum2 = 0;
        foreach (string line in lines)
        {
            int[] nums = line.Split(" ").Select(int.Parse).ToArray();
            int[] diffs = GetDiffs(nums);

            var diffTable = new List<List<int>> { nums.ToList(), diffs.ToList() };
            while (diffs.Any(x => x != 0))
            {
                diffs = GetDiffs(diffs);
                diffTable.Add(diffs.ToList());
            }

            var indent = 0;
            foreach (List<int> diff in diffTable)
            {
                Console.WriteLine(new string(' ', indent) + string.Join(" ", diff));
                indent += 1;
            }

            int ans = diffTable.Sum(x => x[^1]);
            var ans2 = 0;
            for (int i = diffTable.Count - 1; i > 0; i--)
            {
                List<int> numLine = diffTable[i - 1];
                ans2 = numLine[0] - ans2;
            }

            Console.WriteLine($"answer: {ans}");
            Console.WriteLine($"answer2: {ans2}");

            Console.WriteLine("---");
            sum += ans;
            sum2 += ans2;
        }

        Console.WriteLine($"Part 1: {sum}");
        Console.WriteLine($"Part 2: {sum2}");
    }

    private static int[] GetDiffs(int[] nums)
    {
        var ans = new int[nums.Length - 1];
        for (var i = 0; i < nums.Length - 1; i++) ans[i] = nums[i + 1] - nums[i];

        return ans;
    }
}