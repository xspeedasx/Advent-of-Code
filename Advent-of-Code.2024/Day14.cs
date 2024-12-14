using System.Drawing;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace Advent_of_Code._2024;

public class Day14(ITestOutputHelper output)
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day14.txt");
    private readonly string[] _testInput = File.ReadAllLines("Inputs/TestInputs/Test14.txt");
    
    public class Robot(Point position, Point velocity)
    {
        public Point Position { get; set; } = position;
        public Point Velocity { get; set; } = velocity;
    }

    private List<Robot> ParseRobots(string[] input)
    {
        return input.Select(line =>
        {
            string[] parts = line.Split(" ");
            string[] posSplits = parts[0][2..].Split(",");
            var position = new Point(int.Parse(posSplits[0]), int.Parse(posSplits[1]));
            string[] velSplits = parts[1][2..].Split(",");
            var velocity = new Point(int.Parse(velSplits[0]), int.Parse(velSplits[1]));
            return new Robot(position, velocity);
        }).ToList();
    }

    private List<Point> SimulateRobots(List<Robot> robots, int seconds, Size mapSize)
    {
        return robots.Select(robot =>
        {
            int x = robot.Position.X + robot.Velocity.X * seconds;
            x %= mapSize.Width;
            if (x < 0)
            {
                x += mapSize.Width;
            } 
            
            int y = robot.Position.Y + robot.Velocity.Y * seconds;
            y %= mapSize.Height;
            if (y < 0)
            {
                y += mapSize.Height;
            }
            
            return new Point(x, y);
        }).ToList();
    }

    [Fact]
    public void Part1Test1()
    {
        List<Robot> robots = ParseRobots(["p=2,4 v=2,-3"]);
        List<Point> positions = SimulateRobots(robots, 1, new Size(11, 7));
        Assert.Equal(new Point(4, 1), positions[0]);
        positions = SimulateRobots(robots, 2, new Size(11, 7));
        Assert.Equal(new Point(6, 5), positions[0]);
        positions = SimulateRobots(robots, 3, new Size(11, 7));
        Assert.Equal(new Point(8, 2), positions[0]);
        positions = SimulateRobots(robots, 4, new Size(11, 7));
        Assert.Equal(new Point(10, 6), positions[0]);
        positions = SimulateRobots(robots, 5, new Size(11, 7));
        Assert.Equal(new Point(1, 3), positions[0]);
    }
    
    private long CalculateSafetyScore(string[] input, int seconds, Size mapSize)
    {
        List<Robot> robots = ParseRobots(input);
        List<Point> positions = SimulateRobots(robots, seconds, mapSize);
        
        
        
        // debug
        output.WriteLine($"initial positions");
        for(var y = 0; y < mapSize.Height; y++)
        {
            var row = new StringBuilder();
            for(var x = 0; x < mapSize.Width; x++)
            {
                var count = robots.Count(r => r.Position == new Point(x, y));
                if (count > 0)
                {
                    row.Append(count);
                } else
                {
                    row.Append('.');
                }
            }
            output.WriteLine(row.ToString());
        }
        
        Dictionary<Point, int> pointCounts = new();
        foreach (Point position in positions)
        {
            if (!pointCounts.TryAdd(position, 1))
            {
                pointCounts[position]++;
            }
        }
        output.WriteLine($"after {seconds} seconds");
        for(var y = 0; y < mapSize.Height; y++)
        {
            var row = new StringBuilder();
            for(var x = 0; x < mapSize.Width; x++)
            {
                if (pointCounts.TryGetValue(new Point(x, y), out int count))
                {
                    row.Append(count);
                } else
                {
                    row.Append('.');
                }
            }
            output.WriteLine(row.ToString());
        }
        foreach (var p in positions.OrderBy(p => p.Y).ThenBy(p => p.X))
        {
            output.WriteLine($"({p.X}, {p.Y})");
        }
        // end debug
        
        var quadrants = new int[4];
        foreach (Point position in positions)
        {
            if (position.X < mapSize.Width / 2 && position.Y < mapSize.Height / 2)
            {
                quadrants[0]++;
            } else if (position.X > mapSize.Width / 2 && position.Y < mapSize.Height / 2)
            {
                quadrants[1]++;
            } else if (position.X < mapSize.Width / 2 && position.Y > mapSize.Height / 2)
            {
                quadrants[2]++;
            } else if (position.X > mapSize.Width / 2 && position.Y > mapSize.Height / 2)
            {
                quadrants[3]++;
            }
        }
        
        return quadrants[0] * quadrants[1] * quadrants[2] * quadrants[3];
    }

    [Fact]
    public void Part1Test2()
    {
        Assert.Equal(12, CalculateSafetyScore(_testInput, 100, new Size(11, 7)));
    }
    
    [Fact]
    public void Part1()
    {
        Assert.Equal(228421332, CalculateSafetyScore(_input, 100, new Size(101, 103)));
    }

    [Fact]
    public void Part2()
    {
        List<Robot> robots = ParseRobots(_input);
        var mapSize = new Size(101, 103);
        var seconds = 0;
        for(var i = 1; i < 10_000; i++)
        {
            var positions = SimulateRobots(robots, i, mapSize);
            
            if(DetectDiagonals(positions, 5))
            {
                output.WriteLine($"After {i} seconds");
                DrawMap(positions, mapSize);
                seconds = i;
                break;
            }
        }
        output.WriteLine("done");
        Assert.Equal(7790, seconds);
    }

    private bool DetectDiagonals(List<Point> positions, int cnt)
    {
        // x lines in a ^ shape
        return positions.Any(p =>
        {
            for (var i = 1; i <= cnt; i++)
            {
                var p1 = new Point(p.X + i, p.Y + i);
                var p2 = new Point(p.X - i, p.Y + i);
                if (!positions.Contains(p1) || !positions.Contains(p2))
                {
                    return false;
                }
            }
            
            return true;
        });
    }

    private void DrawMap(List<Point> positions, Size mapSize)
    {
        for(var y = 0; y < mapSize.Height; y++)
        {
            var row = new StringBuilder();
            for(var x = 0; x < mapSize.Width; x++)
            {
                var count = positions.Count(p => p == new Point(x, y));
                if (count > 0)
                {
                    row.Append(count);
                } else
                {
                    row.Append('.');
                }
            }
            output.WriteLine(row.ToString());
        }
    }
}