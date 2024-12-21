using System.Drawing;
using Xunit;
using Xunit.Abstractions;

namespace Advent_of_Code._2024;

public class Day20(ITestOutputHelper output)
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day20.txt");
    private readonly string[] _testInput = File.ReadAllLines("Inputs/TestInputs/Test20.txt");
    
    private static Point[] FindBestPath(string[] map)
    {
        var start = Point.Empty;
        var end = Point.Empty;
        for(var y = 0; y < map.Length; y++)
        {
            if(start == Point.Empty)
            {
                int sIdx = map[y].IndexOf('S');
                if (sIdx != -1)
                {
                    start = new Point(sIdx, y);
                }
            }
            
            if(end == Point.Empty)
            {
                int eIdx = map[y].IndexOf('E');
                if (eIdx != -1)
                {
                    end = new Point(eIdx, y);
                }
            }
        }
        
        int mapw = map[0].Length;
        int maph = map.Length;
        
        Point current = start;
        var path = new List<Point> { current };

        while(current != end)
        {
            var moved = false;
            foreach(Size dir in DirUtil.Directions)
            {
                Point next = current + dir;
                if (next.X < 0 || next.X >= mapw || next.Y < 0 || next.Y >= maph)
                {
                    continue;
                }
                if (map[next.Y][next.X] == '#')
                {
                    continue;
                }

                if (path.Count >= 2 &&path[^2] == next)
                {
                    continue;
                }
                
                path.Add(next);
                current = next;
                moved = true;
                break;
            }
            if(!moved)
            {
                break;
            }
        }
        return path.ToArray();
    }
    
    [Fact]
    public void Part1Test1()
    {
        Point[] path = FindBestPath(_testInput);
        Assert.Equal(85, path.Length);
    }

    private Dictionary<int, int> FindBestCheats(string[] map)
    {
        int mapw = map[0].Length;
        int maph = map.Length;
        var cheats = new Dictionary<int, int>();
        Point[] path = FindBestPath(map);
        var tileIndices = new Dictionary<Point, int>();
        for (var i = 0; i < path.Length; i++)
        {
            tileIndices[path[i]] = i;
        }

        foreach (Point current in path)
        {
            int curentIdx = tileIndices[current];
            foreach (Size dir in DirUtil.Directions)
            {
                Point next = current + dir + dir;
                if(next.X <= 0 || next.X >= mapw || next.Y <= 0 || next.Y >= maph)
                {
                    continue;
                }
                if (!tileIndices.TryGetValue(next, out int nextIdx))
                {
                    continue;
                }
                int diff = nextIdx - curentIdx - 2;
                if (diff > 0)
                {
                    cheats[diff] = cheats.GetValueOrDefault(diff) + 1;
                }
            }
        }
        return cheats;
    }
    
    [Fact]
    public void Part1Test2()
    {
        var cheats = FindBestCheats(_testInput);
        Assert.Equal(14, cheats[2]);
        Assert.Equal(14,cheats[4]);
        Assert.Equal(2,cheats[6]);
        Assert.Equal(4,cheats[8]);
        Assert.Equal(2,cheats[10]);
        Assert.Equal(3,cheats[12]);
        Assert.Equal(1,cheats[20]);
        Assert.Equal(1,cheats[36]);
        Assert.Equal(1,cheats[38]);
        Assert.Equal(1,cheats[40]);
        Assert.Equal(1,cheats[64]);
    }
    
    [Fact]
    public void Part1()
    {
        var cheats = FindBestCheats(_input);
        Assert.Equal(1422, cheats.Where(c => c.Key >= 100).Sum(c => c.Value));
    }
    
    private Dictionary<int, int> FindBestestCheats(string[] map)
    {
        int mapw = map[0].Length;
        int maph = map.Length;
        var cheats = new Dictionary<int, int>();
        Point[] path = FindBestPath(map);
        var tileIndices = new Dictionary<Point, int>();
        for (var i = 0; i < path.Length; i++)
        {
            tileIndices[path[i]] = i;
        }

        foreach (Point current in path)
        {
            FindCheatsFromPoint(current);
            // foreach(var cheat in cheats.Where(c => c.Key >= 50).OrderByDescending(c => c.Key))
            // {
            //     output.WriteLine($"Cheat: {cheat.Key} - {cheat.Value}");
            // }
        }
        return cheats;
        
        void FindCheatsFromPoint(Point current)
        {
            //var possibleEndings = new HashSet<Point>();
            //var cheatDistances = new Dictionary<int, int>();
            int currentIdx = tileIndices[current];
            
            var visitedScores = new Dictionary<Point, int>();
            var queue = new Queue<(Point p, int dist)>();
            queue.Enqueue((current, 0));

            while (queue.Count > 0)
            {
                (Point point, int dist) = queue.Dequeue();
                int nextDist = dist+1;

                foreach (Size dir in DirUtil.Directions)
                {
                    Point next = point + dir;
                    if(next.X <= 0 || next.X >= mapw || next.Y <= 0 || next.Y >= maph)
                    {
                        continue;
                    }

                    if(visitedScores.TryGetValue(next, out int nextScore))
                    {
                        if (nextScore <= nextDist)
                        {
                            continue;
                        }
                    }
                    visitedScores[next] = nextDist;
                    if (nextDist < 20)
                    {
                        queue.Enqueue((next, nextDist));
                    }
                }
            }
            
            foreach (var (point, dist) in visitedScores)
            {
                if(!tileIndices.TryGetValue(point, out int pointIdx))
                {
                    continue;
                }
                int diff = pointIdx - currentIdx - dist;
                cheats[diff] = cheats.GetValueOrDefault(diff) + 1;
            }
        }
    }
    
    [Fact]
    public void Part2Test()
    {
        var cheats = FindBestestCheats(_testInput);
        
        Assert.Equal(32, cheats[50]);
        Assert.Equal(31, cheats[52]);
        Assert.Equal(29, cheats[54]);
        Assert.Equal(39, cheats[56]);
        Assert.Equal(25, cheats[58]);
        Assert.Equal(23, cheats[60]);
        Assert.Equal(20, cheats[62]);
        Assert.Equal(19, cheats[64]);
        Assert.Equal(12, cheats[66]);
        Assert.Equal(14, cheats[68]);
        Assert.Equal(12, cheats[70]);
        Assert.Equal(22, cheats[72]);
        Assert.Equal(4, cheats[74]);
        Assert.Equal(3, cheats[76]);
    }

    [Fact]
    public void Part2()
    {
        var cheats = FindBestestCheats(_input);
        Assert.Equal(1009299, cheats.Where(c => c.Key >= 100).Sum(c => c.Value));
    }
}