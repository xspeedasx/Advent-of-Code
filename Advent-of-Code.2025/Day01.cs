using Xunit;
using Xunit.Abstractions;

namespace Advent_of_Code._2025;

public class Day01(ITestOutputHelper output)
{
    private const string TestInput =
        """
        L68
        L30
        R48
        L5
        R60
        L55
        L1
        L99
        R14
        L82
        """;

    private readonly string[] _input = File.ReadAllLines(@"Inputs\Day01.txt");

    [Fact]
    public void Part1Test()
    {
        var cnt = CalcDialStopAt0(TestInput.Split('\n',
            StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries));
        Assert.Equal(3, cnt);
    }

    [Fact]
    public void Part1()
    {
        var cnt = CalcDialStopAt0(_input);
        Assert.Equal(1043, cnt);
    }

    private int CalcDialStopAt0(string[] input)
    {
        var count = 0;
        var current = 50;
        foreach (var line in input)
        {
            var dir = line[0];
            var value = int.Parse(line[1..]);
            if (dir == 'L')
            {
                current -= value;
            }
            else if (dir == 'R')
            {
                current += value;
            }

            while (current < 0)
            {
                current += 100;
            }
            
            while (current >= 100)
            {
                current -= 100;
            }
            
            if (current == 0)
            {
                count++;
            }
            output.WriteLine($"Line: {line}, parsed: {dir}{value} Current: {current}, Count: {count}");
        }
        return count;
    }

    [Fact]
    public void Part2Test()
    {
        var cnt = CalcDialPass0(TestInput.Split('\n',
            StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries));
        Assert.Equal(6, cnt);
    }

    [Fact]
    public void Part2()
    {
        var cnt = CalcDialPass0(_input);
        Assert.Equal(5956, cnt);
    }

    private int CalcDialPass0(string[] input)
    {
        var count = 0;
        var current = 50;
        foreach (var line in input)
        {
            var dir = line[0];
            var value = int.Parse(line[1..]);
            while (value > 100)
            {
                count++;
                value -= 100;
            }

            for (int i = 0; i < value; i++)
            {
                if (dir == 'L')
                {
                    current--;
                }
                else if (dir == 'R')
                {
                    current++;
                }
                if(current >= 100) current = 0;
                if(current == -1) current = 99;
                
                if (current == 0)
                {
                    count++;
                }
            }

            var prev = current;
            var wcurrent = current + dir == 'L' ? -value : value;
            
            if(current < 0 && prev >=0)
            {
                count++;
                current += 100;
            }
            
            while (current >= 100)
            {
                current -= 100;
                count++;
            }
            while (current < 0)
            {
                current += 100;
                count++;
            }
            
            output.WriteLine($"Line: {line}, parsed: {dir}{value} Current: {current}, Count: {count}");
        }
        return count;
    }
}

