using System.Drawing;
using Xunit;
using Xunit.Abstractions;

namespace Advent_of_Code._2024;

public class Day12
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day12.txt");
    private readonly string[][] _testInput;
    private readonly ITestOutputHelper _output;

    public Day12(ITestOutputHelper output)
    {
        _output = output;
        string[] lines = File.ReadAllLines("Inputs/TestInputs/Test12.txt");
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

    private int CalculateGardenPlots(string[] map)
    {
        List<Region> regions = GetRegions(map);

        return regions
            .Where(region => region.Perimeter != 0)
            .Sum(region => region.Perimeter * region.Points.Count);
    }

    private static List<Region> GetRegions(string[] map)
    {
        int mapw = map[0].Length;
        int maph = map.Length;

        var regions = new List<Region>();
        var regionIndex = new Dictionary<Point, int>();

        for (var y = 0; y < maph; y++)
        for (var x = 0; x < mapw; x++)
        {
            char c = map[y][x];
            var current = new Point(x, y);

            var touchingRegions = new List<Region>();
     
            Point above = current with { Y = y - 1 };
            int aboveIndex = regionIndex.GetValueOrDefault(above, -1);
            Region? aboveRegion = aboveIndex == -1 ? null : regions[aboveIndex];

            if (aboveRegion?.Label == c)
            {
                touchingRegions.Add(aboveRegion);
            }

            Point left = current with { X = x - 1 };
            int leftIndex = regionIndex.GetValueOrDefault(left, -1);
            Region? leftRegion = leftIndex == -1 ? null : regions[leftIndex];
            if (leftRegion?.Label == c)
            {
                touchingRegions.Add(leftRegion);
            }

            if (touchingRegions.Count == 0)
            {
                var newRegion = new Region(c, new HashSet<Point> { current }, 4);
                regions.Add(newRegion);
                regionIndex.Add(current, regions.Count - 1);
            }
            else if (touchingRegions.Count == 1)
            {
                touchingRegions[0].Points.Add(current);
                touchingRegions[0].Perimeter += 2;
                regionIndex[current] = leftRegion?.Label == c ? leftIndex : aboveIndex;
            }
            else
            {
                if (leftIndex == aboveIndex)
                {
                    touchingRegions[0].Points.Add(current);
                    regionIndex[current] = regionIndex[left];
                    continue;
                }

                var newRegion = new Region(c, new HashSet<Point>(), 4);
                var totalperimeter = 0;
                foreach (Region region in touchingRegions)
                {
                    newRegion.Points.UnionWith(region.Points);
                    totalperimeter += region.Perimeter;
                    region.Points.Clear();
                    region.Perimeter = 0;
                    region.Label = ' ';
                }

                newRegion.Points.Add(current);
                newRegion.Perimeter = totalperimeter;
                regions.Add(newRegion);
                foreach (Point point in newRegion.Points) regionIndex[point] = regions.Count - 1;
            }
        }

        return regions;
    }

    [Fact]
    public void Part1Test1()
    {
        Assert.Equal(28, CalculateGardenPlots(_testInput[0]));
    }

    [Fact]
    public void Part1Test2()
    {
        Assert.Equal(140, CalculateGardenPlots(_testInput[1]));
    }
    
    [Fact]
    public void Part1Test3()
    {
        Assert.Equal(772, CalculateGardenPlots(_testInput[2]));
    }
    
    [Fact]
    public void Part1Test4()
    {
        Assert.Equal(1930, CalculateGardenPlots(_testInput[3]));
    }
    
    [Fact]
    public void Part1()
    {
        Assert.Equal(1471452, CalculateGardenPlots(_input));
    }

    private static int CalculateGardenPlotsWithDiscount(string[] input)
    {
        List<Region> regions = GetRegions(input);
        
        return regions
            .Where(region => region.Perimeter != 0)
            .Sum(region => FindEdges(region.Points) * region.Points.Count);
    }

    private static int FindEdges(HashSet<Point> points)
    {
        var lines = points
            .GroupBy(p => p.Y)
            .OrderBy(y=>y.Key)
            .Select(g => g.OrderBy(p => p.X).ToArray()).ToArray();
        var edgeCount = 0;
        
        var lastRightFacingEdges = new List<int>();
        var lastLeftFacingEdges = new List<int>();
        
        foreach (var line in lines)
        {
            var rightFacingEdges = line
                .Where(p => 
                    !points.Contains(p with { X = p.X + 1 })
                )
                .Select(p => p.X)
                .ToList();
            
            foreach (int edge in rightFacingEdges)
            {
                if(lastRightFacingEdges.Contains(edge)) continue;
                edgeCount++;
            }
            lastRightFacingEdges = rightFacingEdges;
            
            var leftFacingEdges = line
                .Where(p => 
                    !points.Contains(p with { X = p.X - 1 })
                )
                .Select(p => p.X)
                .ToList();
            
            foreach (int edge in leftFacingEdges)
            {
                if(lastLeftFacingEdges.Contains(edge)) continue;
                edgeCount++;
            }
            lastLeftFacingEdges = leftFacingEdges;
        }
        
        var columns = points.GroupBy(p => p.X)
            .OrderBy(x=>x.Key)
            .Select(g => g.OrderBy(p => p.Y).ToArray()).ToArray();
        
        var lastUpFacingEdges = new List<int>();
        var lastDownFacingEdges = new List<int>();
        foreach (var column in columns)
        {
            var upFacingEdges = column
                .Where(p => 
                    !points.Contains(p with { Y = p.Y - 1 })
                )
                .Select(p => p.Y)
                .ToList();
            
            foreach (int edge in upFacingEdges)
            {
                if(lastUpFacingEdges.Contains(edge)) continue;
                edgeCount++;
            }
            lastUpFacingEdges = upFacingEdges;
            
            var downFacingEdges = column
                .Where(p => 
                    !points.Contains(p with { Y = p.Y + 1 })
                )
                .Select(p => p.Y)
                .ToList();
            
            foreach (int edge in downFacingEdges)
            {
                if(lastDownFacingEdges.Contains(edge)) continue;
                edgeCount++;
            }
            lastDownFacingEdges = downFacingEdges;
        }
        
        return edgeCount;
    }
    
    [Fact]
    public void Part2Test1()
    {
        Assert.Equal(22, CalculateGardenPlotsWithDiscount(_testInput[0]));
    }
    
    [Fact]
    public void Part2Test2()
    {
        Assert.Equal(80, CalculateGardenPlotsWithDiscount(_testInput[1]));
    }
    
    [Fact]
    public void Part2Test3()
    {
        Assert.Equal(436, CalculateGardenPlotsWithDiscount(_testInput[2]));
    }
    
    [Fact]
    public void Part2Test4()
    {
        Assert.Equal(1206, CalculateGardenPlotsWithDiscount(_testInput[3]));
    }
    
    [Fact]
    public void Part2()
    {
        Assert.Equal(863366, CalculateGardenPlotsWithDiscount(_input));
    }

    private class Region(char label, HashSet<Point> points, int perimeter)
    {
        public char Label { get; set; } = label;
        public HashSet<Point> Points { get; } = points;
        public int Perimeter { get; set; } = perimeter;
    }
}