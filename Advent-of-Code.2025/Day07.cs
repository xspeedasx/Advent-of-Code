using System.Drawing;
using Xunit;
using Xunit.Abstractions;

namespace Advent_of_Code._2025;

public class Day07(ITestOutputHelper output)
{
    private string TestInput =
        """
        .......S.......
        ...............
        .......^.......
        ...............
        ......^.^......
        ...............
        .....^.^.^.....
        ...............
        ....^.^...^....
        ...............
        ...^.^...^.^...
        ...............
        ..^...^.....^..
        ...............
        .^.^.^.^.^...^.
        ...............
        """;

    private readonly string[] _input = File.ReadAllLines(@"Inputs\Day07.txt");

    [Fact]
    public void Part1Test()
    {
        var ans = CalcSplits(TestInput.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries));
        Assert.Equal(21, ans);
    }

    [Fact]
    public void Part1()
    {
        var ans = CalcSplits(_input);
        Assert.Equal(1587, ans);
    }

    private long CalcSplits(string[] input)
    {
        var splits = 0;
        var w = input[0].Length;
        
        var beams = new Queue<Point>();
        var beamStarts = new HashSet<Point>();
        var splittersTriggered = new HashSet<Point>();
        beams.Enqueue(new Point(input[0].IndexOf('S'), 0));
        while (beams.Count > 0)
        {
            var beam = beams.Dequeue();
            
            var itSplitAt = WhereItSplits(beam, input);
            if(itSplitAt == null)
            {
                output.WriteLine($"Beam from {beam} goes straight down");
                continue;
            }
            
            if(splittersTriggered.Add(itSplitAt.Value) == false)
            {
                output.WriteLine($"Beam at {beam} merges with another beam");
                continue;
            }
            
            output.WriteLine($"Beam splits at {itSplitAt}");
            splits++;
            
            var a = itSplitAt.Value with { X = itSplitAt.Value.X - 1 };
            if(a.X >= 0 && beamStarts.Add(a))
                beams.Enqueue(a);
            var b = itSplitAt.Value with { X = itSplitAt.Value.X + 1 };
            if(b.X < w && beamStarts.Add(b))
                beams.Enqueue(b);
        }
        
        for(int y = 0; y < input.Length; y++)
        {
            var line = "";
            for(int x = 0; x < input[0].Length; x++)
            {
                line += beamStarts.Contains(new Point(x, y)) ? 'o' : input[y][x];
            }
            output.WriteLine(line);
        }
        
        return splits;
    }

    private Point? WhereItSplits(Point beam, string[] input)
    {
        var w = input[0].Length;
        var h = input.Length;
        
        var current = beam;
        while (current.Y < h - 1)
        {
            current = current with { Y = current.Y + 1 };
            if (input[current.Y][current.X] == '^')
            {
                return current;
            }
        }
        return null;
    }

    [Fact]
    public void Part2Test()
    {
        var ans = CalculateTimelines(TestInput.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries));
        Assert.Equal(40, ans);
    }

    [Fact]
    public void Part2()
    {
        var ans = CalculateTimelines(_input);
        Assert.Equal(5748679033029, ans);
    }

    private long CalculateTimelines(string[] input)
    {
        var queue = new Queue<Beam>();
        long totalPaths = 0L;
        queue.Enqueue(new Beam
        {
            DifferentPaths = 1,
            Position = new Point(input[0].IndexOf('S'), 0)
        });
        
        while(true)
        {
            var newBeams = new List<Beam>();
            while (queue.Count > 0)
            {
                var beam = queue.Dequeue();
                // process beam
                var split = WhereItSplits(beam.Position, input);
                if (split == null)
                {
                    totalPaths += beam.DifferentPaths;
                    continue;
                }

                // split beam
                var leftBeam = new Beam
                {
                    Position = split.Value with { X = split.Value.X - 1 },
                    DifferentPaths = beam.DifferentPaths,
                };
                newBeams.Add(leftBeam);
                var right = new Beam
                {
                    Position = split.Value with { X = split.Value.X + 1 },
                    DifferentPaths = beam.DifferentPaths,
                };
                newBeams.Add(right);
            }
            if (newBeams.Count == 0)
                break;
            var grouped = newBeams
                .GroupBy(b => b.Position)
                .Select(g => new Beam
                {
                    Position = g.Key,
                    DifferentPaths = g.Sum(x => x.DifferentPaths)
                });
            foreach (var beam in grouped)
            {
                queue.Enqueue(beam);
            }
        }
        return totalPaths;
    }

    private class Beam
    {
        public long DifferentPaths { get; set; }
        public Point Position { get; set; }
    }
}
