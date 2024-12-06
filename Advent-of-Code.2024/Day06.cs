using System.Drawing;
using System.Text.RegularExpressions;
using Xunit;
using Xunit.Abstractions;

namespace Advent_of_Code._2024;

public class Day06(ITestOutputHelper output)
{
    private class LoopException : Exception;
    private readonly string[] _input = File.ReadAllLines("Inputs/Day06.txt");
    private readonly string[] _testInput = File.ReadAllLines("Inputs/TestInputs/Test06.txt");

    private enum Direction
    {
        UP,
        DOWN,
        LEFT,
        RIGHT
    };

    [Fact]
    public void Part1Test()
    {
        Assert.Equal(41, GetVisitedPositionsCount(_testInput));
    }
    
    [Fact]
    public void Part1()
    {
        Assert.Equal(4711, GetVisitedPositionsCount(_input));
    }
    
    private static Point FindStartPosition(string[] map)
    {
        Point start = default;
        for (var y = 0; y < map.Length; y++)
        {
            string line = map[y];
            Match m = Regex.Match(line, @"\^");
            if(!m.Success) continue;
            start = new Point(m.Index, y);
            break;
        }

        return start;
    }
    
    private static HashSet<Point> GetVisitedPositions(string[] map, Point current, Direction direction)
    {
        var visited = new HashSet<Point>();
        visited.Add(current);
        
        var turns = new HashSet<(Point, Direction)>();

        while (true)
        {
            var next = new Point(current.X, current.Y) + direction switch
            {
                Direction.UP => new Size(0, -1),
                Direction.DOWN => new Size(0, 1),
                Direction.LEFT => new Size(-1, 0),
                Direction.RIGHT => new Size(1, 0),
                _ => throw new ArgumentOutOfRangeException()
            };
            
            if(next.X < 0 || next.Y < 0 || next.Y >= map.Length || next.X >= map[next.Y].Length)
            {
                break;
            }
            
            if(map[next.Y][next.X] == '#')
            {
                if (!turns.Add((current, direction)))
                {
                    throw new LoopException();
                }
                direction = direction switch
                {
                    Direction.UP => Direction.RIGHT,
                    Direction.RIGHT => Direction.DOWN,
                    Direction.DOWN => Direction.LEFT,
                    Direction.LEFT => Direction.UP,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
            else
            {
                current = next;
                visited.Add(current);
            }
        }
        return visited;
    }

    private static int GetVisitedPositionsCount(string[] map)
    {
        return GetVisitedPositions(map, FindStartPosition(map), Direction.UP).Count;
    }

    private static int GetObstacleThatMakesLoopCount(string[] map)
    {
        Point startPosition = FindStartPosition(map);
        HashSet<Point> visited = GetVisitedPositions(map, startPosition, Direction.UP);
        var obstacleCount = 0;
        
        foreach (var point in visited)
        {
            var modifiedMap = map.ToArray();
            modifiedMap[point.Y] = modifiedMap[point.Y][..point.X] + "#" + modifiedMap[point.Y][(point.X + 1)..];

            try
            {
                GetVisitedPositions(modifiedMap, startPosition, Direction.UP);
            }
            catch (LoopException)
            {
                obstacleCount++;
            }
        }
        
        return obstacleCount;
    }
    
    [Fact]
    public void Part2Test()
    {
        Assert.Equal(6, GetObstacleThatMakesLoopCount(_testInput));
    }
    
    [Fact]
    public void Part2()
    {
        Assert.Equal(1562, GetObstacleThatMakesLoopCount(_input));
    }
}

