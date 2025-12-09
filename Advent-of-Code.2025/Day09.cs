using System.Drawing;
using System.Numerics;
using Xunit;
using Xunit.Abstractions;

namespace Advent_of_Code._2025;

public class Day09(ITestOutputHelper output)
{
    private string TestInput =
        """
        7,1
        11,1
        11,7
        9,7
        9,5
        2,5
        2,3
        7,3
        """;
    private readonly string[] _input = File.ReadAllLines(@"Inputs\Day09.txt");
    
    [Fact]
    public void Part1Test()
    {
        long ans = CalculateLargestArea(TestInput.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries));
        Assert.Equal(50, ans);
    }
    
    [Fact]
    public void Part1()
    {
        long ans = CalculateLargestArea(_input);
        Assert.Equal(4786902990, ans);
    }

    private long CalculateLargestArea(string[] input)
    {
        var points = input.Select(line =>
        {
            var splits = line.Split(',', StringSplitOptions.TrimEntries);
            return new Point(int.Parse(splits[0]), int.Parse(splits[1]));
        }).ToArray();

        long maxArea = 0;
        for (int i = 0; i < points.Length - 1; i++)
        {
            Point a = points[i];
            for (int j = i + 1; j < points.Length; j++)
            {
                Point b = points[j];
                var xdiff = (long)Math.Abs(a.X - b.X) + 1;
                var ydiff = (long)Math.Abs(a.Y - b.Y) + 1;
                var area = xdiff * ydiff;
                if (area > maxArea)
                {
                    maxArea = area;
                }
            }
        }
        return maxArea;
    }
    
    [Fact]
    public void Part2Test()
    {
        long ans = CalculateLargestInternalArea(TestInput.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries));
        Assert.Equal(24, ans);
    }

    [Fact]
    public void Part2()
    {
        long ans = CalculateLargestInternalArea(_input);
        Assert.Equal(1571016172, ans);
    }
    
    private long CalculateLargestInternalArea(string[] input)
    {
        var points = input.Select(line =>
        {
            var splits = line.Split(',', StringSplitOptions.TrimEntries);
            return new Point(int.Parse(splits[0]), int.Parse(splits[1]));
        }).ToArray();

        var horizontalLines = new List<(Point, Point)>();
        var verticalLines = new List<(Point, Point)>();
        for (int i = 0; i < points.Length; i++)
        {
            var a = points[i];
            var b = points[(i + 1) % points.Length];
            if(a.Y == b.Y)
                horizontalLines.Add((a, b));
            if(a.X == b.X)
                verticalLines.Add((a, b));
        }

        var areas = new List<TileArea>();
        for (int i = 0; i < points.Length - 1; i++)
        {
            Point a = points[i];
            for (int j = i + 1; j < points.Length; j++)
            {
                Point b = points[j];
                areas.Add(new TileArea(a, b));
            }
        }

        foreach (var tileArea in areas.OrderByDescending(x => x.Area))
        {
            var minX = Math.Min(tileArea.A.X, tileArea.B.X);
            var maxX = Math.Max(tileArea.A.X, tileArea.B.X);
            var minY = Math.Min(tileArea.A.Y, tileArea.B.Y);
            var maxY = Math.Max(tileArea.A.Y, tileArea.B.Y);

            if(points.Any(x => x.X > minX && x.X < maxX && x.Y > minY && x.Y < maxY))
                continue;

            var isValid = true;
            foreach (var line in horizontalLines)
            {
                if(line.Item1.Y <= minY || line.Item1.Y >= maxY)
                    continue;
                var lineMinX = Math.Min(line.Item1.X, line.Item2.X);
                var lineMaxX = Math.Max(line.Item1.X, line.Item2.X);
                
                if (lineMinX >= maxX || lineMaxX <= minX)
                {
                    continue;
                }
                isValid = false;
                break;
            }
            foreach (var line in verticalLines)
            {
                if(line.Item1.X <= minX || line.Item1.X >= maxX)
                    continue;
                var lineMinY = Math.Min(line.Item1.Y, line.Item2.Y);
                var lineMaxY = Math.Max(line.Item1.Y, line.Item2.Y);
                if (lineMinY >= maxY || lineMaxY <= minY)
                {
                    continue;
                }
                isValid = false;
                break;
            }
            
            if(!isValid)
                continue;
            
            return tileArea.Area;
        }

        return 0;
    }
}

internal class TileArea
{
    public Point A { get; set; }
    public Point B { get; set; }
    public long Area { get; set; }

    public TileArea(Point a, Point b)
    {
        A = a;
        B = b;
        var xdiff = (long)Math.Abs(a.X - b.X) + 1;
        var ydiff = (long)Math.Abs(a.Y - b.Y) + 1;
        Area = xdiff * ydiff;
    }
}