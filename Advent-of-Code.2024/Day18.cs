using System.Drawing;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace Advent_of_Code._2024;

public class Day18(ITestOutputHelper output)
{
    private readonly Point[] _input = ParsePoints(File.ReadAllLines("Inputs/Day18.txt"));
    private readonly Point[] _testInput = ParsePoints(File.ReadAllLines("Inputs/TestInputs/Test18.txt"));
    private static Size[] Directions =
    [
        new(0, -1), // N
        new(1, 0), // E
        new(0, 1), // S
        new(-1, 0) // W
    ]; 
    
    private static Point[] ParsePoints(string[] input)
    {
        return input.Select(line =>
        {
            string[] parts = line.Split(",");
            return new Point(int.Parse(parts[0]), int.Parse(parts[1]));
        }).ToArray();
    }

    private int FindShortestPath(int mapw, int maph, IEnumerable<Point> obstacles)
    {
        var start = new Point(0,0);
        var end = new Point(mapw - 1, maph - 1);
        
        var visited = new Dictionary<Point, int>();
        var queue = new Queue<(Point current, int score)>();
        queue.Enqueue((start, 0));
        
        // for(var y = 0; y < maph; y++)
        // {
        //     var sb = new StringBuilder();
        //     for(var x = 0; x < mapw; x++)
        //     {
        //         if (obstacles.Contains(new Point(x, y)))
        //         {
        //             sb.Append('#');
        //         }
        //         else
        //         {
        //             sb.Append('.');
        //         }
        //     }
        //     output.WriteLine(sb.ToString());
        // }
        // output.WriteLine("---");
        
        while (queue.Count > 0)
        {
            var (currentPos, currentScore) = queue.Dequeue();
            if (currentPos == end)
            {
                break;
            }

            foreach (Size direction in Directions)
            {
                Point next = currentPos + direction;
                if (next.X < 0 || next.X >= mapw || next.Y < 0 || next.Y >= maph)
                {
                    continue;
                }
                if (obstacles.Contains(next))
                {
                    continue;
                }
                if (visited.TryGetValue(next, out int nextScore) && nextScore <= currentScore + 1)
                {
                    continue;
                }
                visited[next] = currentScore + 1;
                if(next != end)
                {
                    queue.Enqueue((next, currentScore + 1));
                }
            }
        }
        
        return visited.GetValueOrDefault(end, -1);
    }

    [Fact]
    public void Part1Test()
    {
        Assert.Equal(22, FindShortestPath(7, 7, _testInput.Take(12)));
    }
    
    [Fact]
    public void Part1()
    {
        Assert.Equal(288, FindShortestPath(71, 71, _input.Take(1024)));
    }

    private string FindLastToCutOff(int mapw, int maph, Point[] obstacles, int startat)
    {
        var dropped = startat;
        while(FindShortestPath(mapw, maph, obstacles.Take(dropped)) > 0 && dropped < obstacles.Length)
        {
            output.WriteLine($"At {obstacles[dropped].X},{obstacles[dropped].Y} still accessible... [{dropped}/{obstacles.Length}]");
            dropped++;
        }

        var last = obstacles[dropped-1];
        return last.X + "," + last.Y; 
    }
    
    [Fact]
    public void Part2Test()
    {
        Assert.Equal("6,1", FindLastToCutOff(7, 7, _testInput, 12));
    }
    
    [Fact]
    public void Part2()
    {
        Assert.Equal("52,5", FindLastToCutOff(71, 71, _input, 1024));
    }
}