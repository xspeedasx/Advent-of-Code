using System.Text.RegularExpressions;
using Xunit;

namespace Advent_of_Code._2015;

public class Day05
{
    private readonly string[] _input = File.ReadAllLines(@"Inputs\Day05.txt");

    private static readonly HashSet<char> Vowels = "aeiou".ToHashSet();
    private static readonly HashSet<string> BadStrings = ["ab", "cd", "pq", "xy"];

    private bool IsNice(string s)
    {
        var vowelCount = 0;
        var doubleLetter = false;
        var containsStrings = false;
        for (int i = 0; i < s.Length; i++)
        {
            var c = s[i];
            if (Vowels.Contains(c))
            {
                vowelCount++;
            }

            if (i > 0)
            {
                if(s[i - 1] == c)
                {
                    doubleLetter = true;
                }

                if (BadStrings.Contains($"{s[i - 1]}{c}"))
                {
                    containsStrings = true;
                }
            }
        }
        return vowelCount >= 3 && doubleLetter && !containsStrings;
    }
    
    [Fact]
    public void Part1Test()
    {
        Assert.True(IsNice("ugknbfddgicrmopn"));
        Assert.True(IsNice("aaa"));

        Assert.False(IsNice("jchzalrnumimnmhp"));
        Assert.False(IsNice("haegwjzuvuyypxyu"));
        Assert.False(IsNice("dvszwmarrgswjxmb"));
    }

    [Fact]
    public void Part1()
    {
        Assert.Equal(255, _input.Count(IsNice));
    }

    private static readonly Regex Part2Rule1 = new(@"(\w\w).*(\1)");
    private static readonly Regex Part2Rule2 = new(@"(\w).(\1)");
    

    private bool IsNice2(string s)
    {
        return Part2Rule1.IsMatch(s) && Part2Rule2.IsMatch(s);
    }

    [Fact]
    public void Part2Test()
    {
        Assert.True(IsNice2("qjhvhtzxzqqjkmpb"));
        Assert.True(IsNice2("xxyxx"));

        Assert.False(IsNice2("uurcxstgmygtbstg"));
        Assert.False(IsNice2("ieodomkazucvgmuy"));
    }

    [Fact]
    public void Part2()
    {
        Assert.Equal(55, _input.Count(IsNice2));
    }
    
}