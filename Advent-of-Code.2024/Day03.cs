using System.Text.RegularExpressions;
using Xunit;

namespace Advent_of_Code._2024;

public class Day03
{
    private const string TestInput = "xmul(2,4)%&mul[3,7]!@^do_not_mul(5,5)+mul(32,64]then(mul(11,8)mul(8,5))";
    private const string TestInput2 = "xmul(2,4)&mul[3,7]!^don't()_mul(5,5)+mul(32,64](mul(11,8)undo()?mul(8,5))";

    private readonly string _input = File.ReadAllText(@"Inputs\Day03.txt");
    
    private readonly Regex _mulRegex = new(@"mul\((\d{1,3}),(\d{1,3})\)", RegexOptions.Compiled);
    private readonly Regex _doRegex = new(@"do\(\)|don't\(\)", RegexOptions.Compiled);

    private int FindAndAddMulResults(string s)
    {
        var matches = _mulRegex.Matches(s);
        int sum = 0;
        foreach (Match match in matches)
        {
            int a = int.Parse(match.Groups[1].Value);
            int b = int.Parse(match.Groups[2].Value);
            sum += a * b;
        }
        return sum;
    }
    
    [Fact]
    public void Part1Test()
    {
        Assert.Equal(161, FindAndAddMulResults(TestInput));
    }

    [Fact]
    public void Part1()
    {
        Assert.Equal(188192787, FindAndAddMulResults(_input));
    }
    
    private int FindAndAddMulResults2(string s)
    {
        var matches = _mulRegex.Matches(s);
        var muls = new List<(int i, int a, int b)>();
        foreach (Match match in matches)
        {
            int a = int.Parse(match.Groups[1].Value);
            int b = int.Parse(match.Groups[2].Value);
            muls.Add((match.Index, a, b));
        }
        
        var doMatches = _doRegex.Matches(s);
        var enabled = true;
        var disableStart = 0;
        for (int i = 0; i < doMatches.Count; i++)
        {
            if(doMatches[i].Value == "don't()" && enabled)
            {
                enabled = false;
                disableStart = doMatches[i].Index;
            }
            else if(doMatches[i].Value == "do()" && !enabled)
            {
                enabled = true;
                muls.RemoveAll(x => x.i >= disableStart && x.i <= doMatches[i].Index);
            }
        }
        return muls.Sum(x => x.a * x.b);
    }
    
    [Fact]
    public void Part2Test()
    {
        Assert.Equal(48, FindAndAddMulResults2(TestInput2));
    }
    
    [Fact]
    public void Part2()
    {
        Assert.Equal(113965544, FindAndAddMulResults2(_input));
    }
}