using Xunit;
using Xunit.Abstractions;

namespace Advent_of_Code._2025;

public class Day05(ITestOutputHelper output)
{
    private const string TestInput =
        """
        3-5
        10-14
        16-20
        12-18
        
        1
        5
        8
        11
        17
        32
        """;

    private readonly string[] _input = File.ReadAllLines(@"Inputs\Day05.txt");

    [Fact]
    public void Part1Test()
    {
        var cnt = CountFreshIngredients(TestInput.Split('\n',
            StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries));
        Assert.Equal(3, cnt);
    }

    [Fact]
    public void Part1()
    {
        var cnt = CountFreshIngredients(_input);
        Assert.Equal(707, cnt);
    }

    private int CountFreshIngredients(string[] input)
    {
        var ranges = new List<(long start, long end)>();
        var ingredients = new List<long>();
        foreach (var line in input)
        {
            if (line == "")
            {
                continue;
            }
            if (line.Contains('-'))
            {
                var splits = line.Split('-', StringSplitOptions.TrimEntries);
                long start = long.Parse(splits[0]);
                long end = long.Parse(splits[1]);
                ranges.Add((start, end));
            }
            else
            {
                ingredients.Add(long.Parse(line));
            }
        }
        
        int freshCount = 0;
        foreach (var ingredient in ingredients)
        {
            if(ranges.Any(r => ingredient >= r.start && ingredient <= r.end))
            {
                freshCount++;
            }
        }

        return freshCount;
    }

    [Fact]
    public void Part2Test()
    {
        var cnt = CountAllPossibleFreshIngredients(TestInput.Split('\n',
            StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries));
        Assert.Equal(14, cnt);
    }

    [Fact]
    public void Part2()
    {
        var cnt = CountAllPossibleFreshIngredients(_input);
        Assert.Equal(361615643045059, cnt);
    }

    private long CountAllPossibleFreshIngredients(string[] input)
    {
        var ranges = new List<(long start, long end)>();
        foreach (var line in input)
        {
            if (line.Contains('-'))
            {
                var splits = line.Split('-', StringSplitOptions.TrimEntries);
                long start = long.Parse(splits[0]);
                long end = long.Parse(splits[1]);
                ranges.Add((start, end));
            }
        }
        ranges = ranges.OrderBy(r => r.start).ToList();
        var mergedRanges = new List<(long start, long end)>();
        for(int i = 0; i < ranges.Count; i++)
        {
            var currentRange = ranges[i];
            var intersectingRanges = new List<(long start, long end)>{currentRange};
            for (int j = i + 1; j < ranges.Count; j++)
            {
                var nextRange = ranges[j];
                if (nextRange.start >= currentRange.start && nextRange.start <= intersectingRanges.Max(x=>x.end))
                {
                    intersectingRanges.Add(nextRange);
                }
                else
                {
                    break;
                }
            }
            mergedRanges.Add((
                intersectingRanges.Min(r => r.start),
                intersectingRanges.Max(r => r.end)
            ));
            i += intersectingRanges.Count - 1;
        }
        return mergedRanges.Sum(r => r.end - r.start + 1);
    }

}

