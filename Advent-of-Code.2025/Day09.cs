using System.Drawing;
using System.Numerics;
using Xunit;
using Xunit.Abstractions;

namespace Advent_of_Code._2025;

public class Day09(ITestOutputHelper output)
{
    private string TestInput =
        """
        7,1
        11,1
        11,7
        9,7
        9,5
        2,5
        2,3
        7,3
        """;
    private readonly string[] _input = File.ReadAllLines(@"Inputs\Day09.txt");
    
    [Fact]
    public void Part1Test()
    {
        long ans = CalculateLargestArea(TestInput.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries));
        Assert.Equal(50, ans);
    }
    
    [Fact]
    public void Part1()
    {
        long ans = CalculateLargestArea(_input);
        Assert.Equal(4786902990, ans);
    }

    private long CalculateLargestArea(string[] input)
    {
        var points = input.Select(line =>
        {
            var splits = line.Split(',', StringSplitOptions.TrimEntries);
            return new Point(int.Parse(splits[0]), int.Parse(splits[1]));
        }).ToArray();

        long maxArea = 0;
        for (int i = 0; i < points.Length - 1; i++)
        {
            Point a = points[i];
            for (int j = i + 1; j < points.Length; j++)
            {
                Point b = points[j];
                var xdiff = (long)Math.Abs(a.X - b.X) + 1;
                var ydiff = (long)Math.Abs(a.Y - b.Y) + 1;
                var area = xdiff * ydiff;
                if (area > maxArea)
                {
                    maxArea = (long)area;
                }
            }
        }
        return maxArea;
    }
}