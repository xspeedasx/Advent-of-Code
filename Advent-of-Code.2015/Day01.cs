using Xunit;

namespace Advent_of_Code._2015;

public class Day01
{
    private readonly string _input = File.ReadAllText(@"Inputs/Day01.txt");

    private int CountFloors(string input)
    {
        var floor = 0;
        foreach (char t in input)
        {
            switch (t)
            {
                case '(':
                    floor++;
                    break;
                case ')':
                    floor--;
                    break;
            }
        }

        return floor;
    }

    private int GetFirstBasement(string input)
    {
        var floor = 0;
        for (var i = 0; i < input.Length; i++)
        {
            switch (input[i])
            {
                case '(':
                    floor++;
                    break;
                case ')':
                    floor--;
                    break;
            }
            
            if(floor == -1)
            {
                return i + 1;
            }
        }

        return -1;
    }
    
    [Fact]
    public void Part1Test()
    {
        Assert.Equal(0, CountFloors("(())"));
        Assert.Equal(3, CountFloors("((("));
        Assert.Equal(3, CountFloors("(()(()("));
        Assert.Equal(3, CountFloors("))((((("));
        Assert.Equal(-1, CountFloors("())"));
        Assert.Equal(-1, CountFloors("))("));
        Assert.Equal(-3, CountFloors(")))"));
        Assert.Equal(-3, CountFloors(")())())"));
    }
    
    [Fact]
    public void Part1()
    {
        Assert.Equal(138, CountFloors(_input));
    }
    
    [Fact]
    public void Part2Test()
    {
        Assert.Equal(1, GetFirstBasement(")"));
        Assert.Equal(5, GetFirstBasement("()())"));
    }
    
    [Fact]
    public void Part2()
    {
        Assert.Equal(1771, GetFirstBasement(_input));
    }
}