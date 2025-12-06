using Xunit;
using Xunit.Abstractions;

namespace Advent_of_Code._2025;

public class Day06(ITestOutputHelper output)
{
    private string[] TestInput = new List<string>
    {
        "123 328  51 64 ",
        " 45 64  387 23 ",
        "  6 98  215 314",
        "*   +   *   +  ",
    }.ToArray();

    private readonly string[] _input = File.ReadAllLines(@"Inputs\Day06.txt");

    [Fact]
    public void Part1Test()
    {
        var ans = SolvePart1(TestInput);
        Assert.Equal(4277556, ans);
    }

    [Fact]
    public void Part1()
    {
        var ans = SolvePart1(_input);
        Assert.Equal(8108520669952, ans);
    }

    private class MathGroup
    {
        public List<long> Numbers { get; set; } = [];
        public char Operation { get; set; } = ' ';
        
        public long Calculate()
        {
            long result = Operation switch
            {
                '+' => Numbers.Sum(),
                '*' => Numbers.Aggregate(1L, (acc, val) => acc * val),
                _ => 0L
            };
            return result;
        }
    }

    private long SolvePart1(string[] input)
    {
        var groups = new Dictionary<int, MathGroup>();
        foreach (string line in input)
        {
            string[] splits = line.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            for(int i = 0; i < splits.Length; i++)
            {
                if (!groups.ContainsKey(i))
                {
                    groups[i] = new MathGroup();
                }

                if (splits[i] == "*" || splits[i] == "+")
                {
                    groups[i].Operation = splits[i][0];
                }
                else
                {
                    groups[i].Numbers.Add(long.Parse(splits[i]));
                }
            }
        }

        return groups.Values.Select(g => g.Calculate()).Sum();
    }

    [Fact]
    public void Part2Test()
    {
        var ans = SolvePart2(TestInput);
        Assert.Equal(3263827, ans);
    }

    [Fact]
    public void Part2()
    {
        var ans = SolvePart2(_input);
        Assert.Equal(11708563470209, ans);
    }

    private class AdvancedMathGroup
    {
        public List<string> Numbers { get; set; } = [];
        public char Operation { get; set; } = ' ';
        
        public long Calculate(ITestOutputHelper output)
        {
            var actualNumbers = new List<long>();
            for (int i = 0; i < Numbers[0].Length; i++)
            {
                var current = 0;
                foreach (var num in Numbers)
                {
                    if (num[i] != ' ')
                    {
                        current *= 10;
                        current += num[i] - '0';
                    }
                }
                actualNumbers.Add(current);
            }

            var result = Operation == '*' ? 1L : 0L;
            foreach (var n in actualNumbers)
            {
                if(Operation == '*')
                {
                    result *= n;
                }
                else
                {
                    result += n;
                }
            };
            output.WriteLine($"Calculating group: {string.Join(" " + Operation + " ", actualNumbers)} = {result}");
            return result;
        }
    }
    private long SolvePart2(string[] input)
    {
        var groups = new List<AdvancedMathGroup>();

        var currentGroup = new AdvancedMathGroup
        {
            Numbers = Enumerable.Repeat("", input.Length - 1).ToList()
        };
        
        for(int i = 0; i < input[0].Length; i++)
        {
            if (input.All(line => line[i] == ' '))
            {
                groups.Add(currentGroup);
                currentGroup = new AdvancedMathGroup
                {
                    Numbers = Enumerable.Repeat("", input.Length - 1).ToList()
                };
                continue;
            }
            if(input[^1][i] != ' ') 
            {
                currentGroup.Operation = input[^1][i];
            }

            for (int j = 0; j < input.Length - 1; j++)
            {
                currentGroup.Numbers[j] += input[j][i];
            }
        }
        groups.Add(currentGroup);
        
        //return groups.Select(g => g.Calculate(output)).Sum();
        var sum = 0L;
        foreach(var group in groups)
        {
            sum += group.Calculate(output);
        }
        return sum;
    }
}
