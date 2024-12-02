using Xunit;

namespace Advent_of_Code._2024;

public class Day02
{
    private const string TestInput =
        """
        7 6 4 2 1
        1 2 7 8 9
        9 7 6 2 1
        1 3 2 4 5
        8 6 4 4 1
        1 3 6 7 9
        """;

    private readonly string[] _input = File.ReadAllLines(@"Inputs\Day02.txt");

    private bool IsSafe(int[] report)
    {
        var lineGradient = GetGradient(report[0], report[1]);

        for (int i = 1; i < report.Length; i++)
        {
            int gradient = GetGradient(report[i - 1], report[i]);
            if (gradient != lineGradient)
            {
                return false;
            }

            int magnitude = Math.Abs(report[i] - report[i - 1]);
            if (magnitude is 0 or > 3)
            {
                return false;
            }
        }

        return true;

        int GetGradient(int a, int b)
        {
            return b - a > 0 ? 1 : -1;
        }
    }

    private int[] ParseLine(string line)
    {
        return line.Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(int.Parse)
            .ToArray();
    }

    [Fact]
    public void Part1Test()
    {
        string[] lines = TestInput.Split("\r\n");
        Assert.Equal(2, lines.Count(x => IsSafe(ParseLine(x))));
    }

    [Fact]
    public void Part1()
    {
        Assert.Equal(502, _input.Count(x => IsSafe(ParseLine(x))));
    }

    private bool IsSafeWhenIgnore1(int[] report)
    {
        for (int i = 0; i < report.Length; i++)
        {
            if (IsSafe(report[..i].Concat(report[(i + 1)..]).ToArray()))
            {
                return true;
            }
        }
        return false;
    }

    [Fact]
    public void Part2Test()
    {
        string[] lines = TestInput.Split("\r\n");
        Assert.Equal(4, lines.Count(x => IsSafeWhenIgnore1(ParseLine(x))));
    }

    [Fact]
    public void Part2()
    {
        Assert.Equal(544, _input.Count(x => IsSafeWhenIgnore1(ParseLine(x))));
    }
}