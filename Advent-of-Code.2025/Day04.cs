using Xunit;
using Xunit.Abstractions;

namespace Advent_of_Code._2025;

public class Day04(ITestOutputHelper output)
{
    private const string TestInput =
        """
        ..@@.@@@@.
        @@@.@.@.@@
        @@@@@.@.@@
        @.@@@@..@.
        @@.@@@@.@@
        .@@@@@@@.@
        .@.@.@.@@@
        @.@@@.@@@@
        .@@@@@@@@.
        @.@.@@@.@.
        """;

    private readonly string[] _input = File.ReadAllLines(@"Inputs\Day04.txt");

    [Fact]
    public void Part1Test()
    {
        var cnt = CountAccessibleRolls(TestInput.Split('\n',
            StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries));
        Assert.Equal(13, cnt);
    }

    [Fact]
    public void Part1()
    {
        var cnt = CountAccessibleRolls(_input);
        Assert.Equal(1464, cnt);
    }

    int CountAccessibleRolls(string[] input)
    {
        var w = input[0].Length;
        var h = input.Length;
        var accessibleCount = 0;

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                if (input[y][x] != '@')
                    continue;
                
                var nCount = 0;
                for(int ny = Math.Max(0, y - 1); ny <= Math.Min(h - 1, y + 1); ny++)
                {
                    for(int nx = Math.Max(0, x - 1); nx <= Math.Min(w - 1, x + 1); nx++)
                    {
                        if (nx == x && ny == y)
                            continue;
                        if(input[ny][nx] != '@')
                            continue;
                        nCount++;
                    }
                }
                if (nCount < 4)
                {
                    accessibleCount++;
                }
            }
        }
        return accessibleCount;
    }

    [Fact]
    public void Part2Test()
    {
        var cnt = CountRemovableRolls(TestInput.Split('\n',
            StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries));
        Assert.Equal(43, cnt);
    }

    [Fact]
    public void Part2()
    {
        var cnt = CountRemovableRolls(_input);
        Assert.Equal(8409, cnt);
    }

    private int CountRemovableRolls(string[] input)
    {
        var w = input[0].Length;
        var h = input.Length;
        var removedCount = 0;
        var map = input.Select(x => x.ToCharArray()).ToArray();
        
        while(true){
            var toRemove = new List<(int x, int y)>();
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    if (map[y][x] != '@')
                        continue;
                    
                    var nCount = 0;
                    for(int ny = Math.Max(0, y - 1); ny <= Math.Min(h - 1, y + 1); ny++)
                    {
                        for(int nx = Math.Max(0, x - 1); nx <= Math.Min(w - 1, x + 1); nx++)
                        {
                            if (nx == x && ny == y)
                                continue;
                            if(map[ny][nx] != '@')
                                continue;
                            nCount++;
                        }
                    }
                    if (nCount < 4)
                    {
                        toRemove.Add((x, y));
                    }
                }
            }
            if(toRemove.Count == 0)
                break;
            foreach (var (x, y) in toRemove)
            {
                map[y][x] = 'x';
                removedCount++;
            }
        }
        return removedCount;
    }
}

