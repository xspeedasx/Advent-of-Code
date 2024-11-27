using System.Diagnostics;

namespace Advent_of_Code._2023;

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

            var diffTable = new List<int[]> { nums, diffs };
            while (diffs.Any(x => x != 0))
            {
                diffs = GetDiffs(diffs);
                diffTable.Add(diffs);
            }

            var ans = 0;
            var ans2 = 0;
            for (int i = diffTable.Count - 2; i >= 0; i--)
            {
                ans += diffTable[i][^1];
                ans2 = diffTable[i][0] - ans2;
            }

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