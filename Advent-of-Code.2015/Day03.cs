using System.Drawing;
using Xunit;

namespace Advent_of_Code._2015;

public class Day03
{
    private readonly string _input = File.ReadAllText(@"Inputs\Day03.txt");
    
    private int CountHousesVisited(string directions)
    {
        var current = new Point(0, 0);
        var visited = new Dictionary<Point, int> { { current, 1 } };
        
        foreach(var direction in directions)
        {
            current = GetNewDirection(direction, current);

            if (!visited.ContainsKey(current))
            {
                visited[current] = 1;
            }
            visited[current]++;
        }
        return visited.Count;
    }

    [Fact]
    public void Part1Test()
    {
        Assert.Equal(2, CountHousesVisited(">"));
        Assert.Equal(4, CountHousesVisited("^>v<"));
        Assert.Equal(2, CountHousesVisited("^v^v^v^v^v"));
    }

    [Fact]
    public void Part1()
    {
        var count = CountHousesVisited(_input);
        Assert.Equal(2572, count);
    }

    private int CountHousesVisited2(string directions)
    {
        var santa = new Point(0, 0);
        var robo = new Point(0, 0);

        var visited = new HashSet<Point> { santa };

        var isSantasTurn = true;
        foreach (var direction in directions)
        {
            var newDir = GetNewDirection(direction, isSantasTurn ? santa : robo);
            visited.Add(newDir);
            if (isSantasTurn) santa = newDir;
            else robo = newDir;
            isSantasTurn = !isSantasTurn;
        }

        return visited.Count;
    }

    [Fact]
    public void Part2Test()
    {
        Assert.Equal(3, CountHousesVisited2("^v"));
        Assert.Equal(3, CountHousesVisited2("^>v<"));
        Assert.Equal(11, CountHousesVisited2("^v^v^v^v^v"));
    }

    private static Point GetNewDirection(char direction, Point current)
    {
        switch (direction)
        {
            case '^':
                current = current with { Y = current.Y - 1 };
                break;
            case '>':
                current = current with { X = current.X + 1 };
                break;
            case 'v':
                current = current with { Y = current.Y + 1 };
                break;
            case '<':
                current = current with { X = current.X - 1 };
                break;
        }

        return current;
    }   

    [Fact]
    public void Part2()
    {
        var count = CountHousesVisited2(_input);
        Assert.Equal(2631, count);
    }
}