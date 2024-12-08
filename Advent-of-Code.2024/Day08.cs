using System.Drawing;
using Xunit;

namespace Advent_of_Code._2024;

public class Day08
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day08.txt");
    private readonly string[][] _testInput;

    public Day08()
    {
        string[] lines = File.ReadAllLines("Inputs/TestInputs/Test08.txt");
        var cnt = 0;
        var part = new List<string>();
        _testInput = new string[lines.Count(x => x == "") + 1][];
        foreach (string t in lines)
        {
            if (t == "")
            {
                _testInput[cnt] = part.ToArray();
                part.Clear();
                cnt++;
                continue;
            }

            part.Add(t);
        }
        _testInput[cnt] = part.ToArray();
    }

    private int CalculateAntinodes(string[] input)
    {
        int mapw = input[0].Length;
        int maph = input.Length;
        var nodes = new Dictionary<char, List<Point>>();
        var antiNodes = new HashSet<Point>();
        for (var y = 0; y < input.Length; y++)
        for (var x = 0; x < input[y].Length; x++)
        {
            char node = input[y][x];
            if (node == '.')
            {
                continue;
            }

            if (!nodes.ContainsKey(node))
            {
                nodes[node] = new List<Point>();
            }

            var b = new Point(x, y);
            if (nodes[node].Count > 0)
            {
                foreach (Point a in nodes[node])
                {
                    var d = new Size(b.X - a.X, b.Y - a.Y);
                    Point anti1 = a - d;
                    CheckAndAddNode(anti1);
                    Point anti2 = b + d;
                    CheckAndAddNode(anti2);
                    continue;

                    void CheckAndAddNode(Point n)
                    {
                        if (n == a || n == b)
                        {
                            throw new Exception("Invalid antinode");
                        }

                        if (n.X >= 0 && n.X < mapw && n.Y >= 0 && n.Y < maph)
                        {
                            antiNodes.Add(n);
                        }
                    }
                }
            }

            nodes[node].Add(b);
        }

        return antiNodes.Count;
    }

    [Fact]
    public void Part1Test1()
    {
        Assert.Equal(2, CalculateAntinodes(_testInput[0]));
    }

    [Fact]
    public void Part1Test2()
    {
        Assert.Equal(4, CalculateAntinodes(_testInput[1]));
    }
    
    [Fact]
    public void Part1Test3()
    {
        Assert.Equal(4, CalculateAntinodes(_testInput[2]));
    }
    
    [Fact]
    public void Part1Test4()
    {
        Assert.Equal(14, CalculateAntinodes(_testInput[3]));
    }
    
    [Fact]
    public void Part1()
    {
        Assert.Equal(423, CalculateAntinodes(_input));
    }
    
    private int CalculateAntinodes2(string[] input)
    {
        int mapw = input[0].Length;
        int maph = input.Length;
        var nodes = new Dictionary<char, List<Point>>();
        var antiNodes = new HashSet<Point>();
        for (var y = 0; y < input.Length; y++)
        for (var x = 0; x < input[y].Length; x++)
        {
            char node = input[y][x];
            if (node == '.')
            {
                continue;
            }

            if (!nodes.ContainsKey(node))
            {
                nodes[node] = new List<Point>();
            }

            var b = new Point(x, y);
            if (nodes[node].Count > 0)
            {
                foreach (Point a in nodes[node])
                {
                    var d = new Size(b.X - a.X, b.Y - a.Y);
                    var current = new Point(a.X, a.Y);
                    while (current.X >= 0 && current.X < mapw && current.Y >= 0 && current.Y < maph)
                    {
                        antiNodes.Add(current);
                        current -= d;
                    }

                    current = new Point(b.X, b.Y);
                    while (current.X >= 0 && current.X < mapw && current.Y >= 0 && current.Y < maph)
                    {
                        antiNodes.Add(current);
                        current += d;
                    }
                }
            }

            nodes[node].Add(b);
        }

        return antiNodes.Count;
    }
    
    [Fact]
    public void Part2Test1()
    {
        Assert.Equal(9, CalculateAntinodes2(_testInput[4]));
    }
    
    [Fact]
    public void Part2Test2()
    {
        Assert.Equal(34, CalculateAntinodes2(_testInput[3]));
    }
    
    [Fact]
    public void Part2()
    {
        Assert.Equal(1287, CalculateAntinodes2(_input));
    }
}