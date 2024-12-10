using System.Drawing;
using Xunit;

namespace Advent_of_Code._2024;

public class Day10
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day10.txt");
    private readonly string[][] _testInput;

    public Day10()
    {
        string[] lines = File.ReadAllLines("Inputs/TestInputs/Test10.txt");
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

    public int CalculateTrailheadScores(string[] input)
    {
        var maph = input.Length;
        var mapw = input[0].Length;
        var visitedFromStart = new HashSet<(Point start, Point tile)>();
        var trails = new Queue<(Point start, Point current)>();
        var finishes = new HashSet<(Point start, Point finish)>();
        
        for (var y = 0; y < input.Length; y++)
        for (var x = 0; x < input[y].Length; x++)
            if (input[y][x] == '0')
                trails.Enqueue((new Point(x, y), new Point(x, y)));

        while (trails.Count > 0)
        {
            var (start, current) = trails.Dequeue();
            var currentTile = input[current.Y][current.X] - '0';
            
            // N
            CheckAndAddNextPoint(new Point(current.X, current.Y - 1));
            // E
            CheckAndAddNextPoint(new Point(current.X + 1, current.Y));
            // S
            CheckAndAddNextPoint(new Point(current.X, current.Y + 1));
            // W
            CheckAndAddNextPoint(new Point(current.X - 1, current.Y));

            void CheckAndAddNextPoint(Point next)
            {
                if (next.X < 0 || next.Y < 0 || next.X >= mapw || next.Y >= maph) return;
                int nextTile = input[next.Y][next.X] - '0';
                if(nextTile - currentTile != 1) return;                
                if (!visitedFromStart.Add((start, next))) return;
                if (nextTile == 9) finishes.Add((start, next));
                else trails.Enqueue((start, next));
            }
        }

        return finishes.Count;
    }
    
    [Fact]
    public void Part1Test1()
    {
        Assert.Equal(2, CalculateTrailheadScores(_testInput[0]));
    }
    
    [Fact]
    public void Part1Test2()
    {
        Assert.Equal(4, CalculateTrailheadScores(_testInput[1]));
    }
    
    [Fact]
    public void Part1Test3()
    {
        Assert.Equal(3, CalculateTrailheadScores(_testInput[2]));
    }
    
    [Fact]
    public void Part1Test4()
    {
        Assert.Equal(36, CalculateTrailheadScores(_testInput[3]));
    }
    
    [Fact]
    public void Part1()
    {
        Assert.Equal(674, CalculateTrailheadScores(_input));
    }

    public int CalculateTrailheadRatings(string[] input)
    {
        var maph = input.Length;
        var mapw = input[0].Length;
        var trails = new Queue<(Point start, Point current, string path)>();
        var finishes = new HashSet<(Point start, Point finish, string path)>();
        
        for (var y = 0; y < input.Length; y++)
        for (var x = 0; x < input[y].Length; x++)
            if (input[y][x] == '0')
                trails.Enqueue((new Point(x, y), new Point(x, y), $"{x},{y}"));

        while (trails.Count > 0)
        {
            var (start, current, path) = trails.Dequeue();
            var currentTile = input[current.Y][current.X] - '0';
            
            // N
            CheckAndAddNextPoint(new Point(current.X, current.Y - 1));
            // E
            CheckAndAddNextPoint(new Point(current.X + 1, current.Y));
            // S
            CheckAndAddNextPoint(new Point(current.X, current.Y + 1));
            // W
            CheckAndAddNextPoint(new Point(current.X - 1, current.Y));

            void CheckAndAddNextPoint(Point next)
            {
                if (next.X < 0 || next.Y < 0 || next.X >= mapw || next.Y >= maph) return;
                int nextTile = input[next.Y][next.X] - '0';
                if(nextTile - currentTile != 1) return;
                var nextPath = $"{path}|{next.X},{next.Y}";
                if (nextTile == 9) finishes.Add((start, next, nextPath));
                else trails.Enqueue((start, next, nextPath));
            }
        }

        return finishes.Count;
    }
    
    [Fact]
    public void Part2Test1()
    {
        Assert.Equal(3, CalculateTrailheadRatings(_testInput[4]));
    }
    
    [Fact]
    public void Part2Test2()
    {
        Assert.Equal(13, CalculateTrailheadRatings(_testInput[1]));
    }
    
    [Fact]
    public void Part2Test3()
    {
        Assert.Equal(227, CalculateTrailheadRatings(_testInput[5]));
    }
    
    [Fact]
    public void Part2Test4()
    {
        Assert.Equal(81, CalculateTrailheadRatings(_testInput[3]));
    }
    
    [Fact]
    public void Part2()
    {
        Assert.Equal(1372, CalculateTrailheadRatings(_input));
    }
}