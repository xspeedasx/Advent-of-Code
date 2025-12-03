using Xunit;
using Xunit.Abstractions;

namespace Advent_of_Code._2025;

public class Day02(ITestOutputHelper output)
{
    private const string TestInput =
        "11-22,95-115,998-1012,1188511880-1188511890,222220-222224,1698522-1698528,446443-446449,38593856-38593862,565653-565659,824824821-824824827,2121212118-2121212124";

    private readonly string _input = File.ReadAllText(@"Inputs\Day02.txt");

    [Fact]
    public void Part1Test()
    {
        long ans = GetSumOfInvalidIds(TestInput);
        Assert.Equal(1227775554, ans);
    }

    [Fact]
    public void Part1()
    {
        long ans = GetSumOfInvalidIds(_input);
        Assert.Equal(29818212493, ans);
    }

    private long GetSumOfInvalidIds(string input)
    {
        var ranges = input.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(x =>
            {
                var parts = x.Split('-', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                return (parts[0], parts[1]);
            }).ToList();
        long sum = 0;
        foreach (var (start, end) in ranges)
        {
            if (start.Length % 2 != 0 && end.Length % 2 != 0)
            {
                // length must be divisible by 2 to have 2 equal segments
                continue;
            }
            var min = long.Parse(start);
            var max = long.Parse(end);

            var currentHalf = start.Length % 2 == 0
                ? start[..(start.Length / 2)]
                : "1" + new String('0', end.Length / 2 - 1);
            var currentNum = long.Parse(currentHalf + currentHalf);
            var increment = Math.Pow(10, currentHalf.Length) + 1;

            output.WriteLine($"Checking range {start}-{end}, starting at half {currentHalf}, num {currentNum}, increment {increment}");

            while (true)
            {
                if(currentNum > max)
                    break;
                if (currentNum.ToString().Length % 2 != 0)
                {
                    break;
                }
                
                if (currentNum >= min && currentNum <= max)
                {
                    sum += currentNum;
                    output.WriteLine($"  Found invalid ID: {currentNum}");
                }
                currentNum += (long)increment;
            }
        }
        return sum;
    }


    [Fact]
    public void Part2Test()
    {

    }

    [Fact]
    public void Part2()
    {
    }
}
