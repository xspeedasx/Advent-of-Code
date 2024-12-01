using Xunit;

namespace Advent_of_Code._2024;

public class Day01
{
    private const string TestInput =
        """
        3   4
        4   3
        2   5
        1   3
        3   9
        3   3
        """;

    private readonly string[] _input = File.ReadAllLines(@"Inputs\Day01.txt");

    private int CalculateDistances(string[] input)
    {
        var list1 = new List<int>();
        var list2 = new List<int>();

        foreach (string line in input)
        {
            string[] split = line.Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            list1.Add(int.Parse(split[0]));
            list2.Add(int.Parse(split[1]));
        }

        list1.Sort();
        list2.Sort();
        return list1.Zip(list2).Sum(x => Math.Abs(x.First - x.Second));
    }

    [Fact]
    public void Part1Test()
    {
        Assert.Equal(11, CalculateDistances(TestInput.Split("\r\n")));
    }

    [Fact]
    public void Part1()
    {
        Assert.Equal(2066446, CalculateDistances(_input));
    }

    private int CalculateSimilarity(string[] input)
    {
        var appearances = new Dictionary<int, int>();
        var digits = new List<int>();
        foreach (string line in input)
        {
            string[] split = line.Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            digits.Add(int.Parse(split[0]));
            int key = int.Parse(split[1]);
            if (appearances.ContainsKey(key))
            {
                appearances[key]++;
            }
            else
            {
                appearances[key] = 1;
            }
        }

        return digits.Sum(x => appearances.TryGetValue(x, out int appearance)
            ? x * appearance
            : 0);
    }

    [Fact]
    public void Part2Test()
    {
        Assert.Equal(31, CalculateSimilarity(TestInput.Split("\r\n")));
    }

    [Fact]
    public void Part2()
    {
        Assert.Equal(24931009, CalculateSimilarity(_input));
    }
}