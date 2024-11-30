using Xunit;

namespace Advent_of_Code._2024;

public class Day01
{
    private readonly string _input = File.ReadAllText(@"Inputs\Day01.txt");

    [Fact]
    public void Part1Test()
    {
        Assert.Equal("test", _input);
    }
}