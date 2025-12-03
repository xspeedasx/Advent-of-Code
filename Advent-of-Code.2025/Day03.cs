using Xunit;
using Xunit.Abstractions;

namespace Advent_of_Code._2025;

public class Day03(ITestOutputHelper output)
{
    private const string TestInput =
        """
        987654321111111
        811111111111119
        234234234234278
        818181911112111
        """;

    private readonly string[] _input = File.ReadAllLines(@"Inputs\Day03.txt");

    [Fact]
    public void Part1Test()
    {
        long ans = GetSumOfBiggestJoltagesFrom2(TestInput.Split('\n',
            StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries));
        Assert.Equal(357, ans);
    }

    [Fact]
    public void Part1()
    {
        long ans = GetSumOfBiggestJoltagesFrom2(_input);
        Assert.Equal(17100, ans);
    }

    private long GetSumOfBiggestJoltagesFrom2(string[] input)
    {
        var sum = 0L;

        foreach (var line in input)
        {
            var leftBiggest = line[0] - '0';
            var leftIndex = 0;
            for (int i = 1; i < line.Length - 1; i++)
            {
                var current = line[i] - '0';
                if (current > leftBiggest)
                {
                    leftBiggest = current;
                    leftIndex = i;
                }
            }
            var rightBiggest = line[leftIndex+1] - '0';
            var rightIndex = leftIndex + 1;
            for (int i = leftIndex + 2; i < line.Length; i++)
            {
                var current = line[i] - '0';
                if (current > rightBiggest)
                {
                    rightBiggest = current;
                    rightIndex = i;
                }
            }

            int jolts = leftBiggest * 10 + rightBiggest;
            output.WriteLine($"Got joltage: {jolts} from {leftBiggest} at index {leftIndex} and {rightBiggest} at index {rightIndex}");
            sum += jolts;
        }


        return sum;
    }


    [Fact]
    public void Part2Test()
    {
        long ans = GetSumOfBiggestJoltagesFrom12(TestInput.Split('\n',
            StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries));
        Assert.Equal(3121910778619, ans);
    }

    [Fact]
    public void Part2()
    {
        long ans = GetSumOfBiggestJoltagesFrom12(_input);
        Assert.Equal(170418192256861, ans);
    }

    private long GetSumOfBiggestJoltagesFrom12(string[] input)
    {
        var sum = 0L;

        foreach (var line in input)
        {
            var joltSum = 0L;

            var biggestIndex = -1;
            for (int i = 11; i >= 0; i--)
            {
                var currentBiggest = line[biggestIndex + 1] - '0';
                var currentIndex = biggestIndex + 1;
                for (int j = biggestIndex + 2; j < line.Length - i; j++)
                {
                    var current = line[j] - '0';
                    if (current > currentBiggest)
                    {
                        currentBiggest = current;
                        currentIndex = j;
                    }
                }
                biggestIndex = currentIndex;
                joltSum *= 10;
                joltSum += currentBiggest;
            }
            output.WriteLine($"Got joltage: {joltSum} from line {line}");
            sum += joltSum;
        }
        return sum;
    }
}
