using Xunit;

namespace Advent_of_Code._2015;

public class Day02
{
    private readonly string[] _input = File.ReadAllLines(@"Inputs\Day02.txt");

    private int CalculatePaper(string dimensions)
    {
        string[] splits = dimensions.Split('x');
        int l = int.Parse(splits[0]);
        int w = int.Parse(splits[1]);
        int h = int.Parse(splits[2]);
        int total = 2 * l * w + 2 * w * h + 2 * h * l;
        var sides = new[] { l, w, h }.Order().ToArray();
        var slack = sides[0] * sides[1];
        return total + slack;
    }
    
    [Fact]
    public void Part1Test()
    {
        Assert.Equal(58, CalculatePaper("2x3x4"));
        Assert.Equal(43, CalculatePaper("1x1x10"));
    }
    
    [Fact]
    public void Part1()
    {
        var sum = _input.Select(CalculatePaper).Sum();
        Assert.Equal(1606483, sum);
    }
    
    private int CalculateRibbon(string dimensions)
    {
        var splits = dimensions.Split('x');
        var l = int.Parse(splits[0]);
        var w = int.Parse(splits[1]);
        var h = int.Parse(splits[2]);
        var sides = new[] { l, w, h }.Order().ToArray();
        var ribbon = 2 * sides[0] + 2 * sides[1];
        var bow = l * w * h;
        return ribbon + bow;
    }
    
    [Fact]
    public void Part2Test()
    {
        Assert.Equal(34, CalculateRibbon("2x3x4"));
        Assert.Equal(14, CalculateRibbon("1x1x10"));
    }
    
    [Fact]
    public void Part2()
    {
        var sum = _input.Select(CalculateRibbon).Sum();
        Assert.Equal(3842356, sum);
    }
}