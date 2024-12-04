using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Advent_of_Code._2024;

public class Day04(ITestOutputHelper output)
{
    private readonly char[] _xmas = "XMAS".ToArray();
    private const string TestInput =
        """
        MMMSXXMASM
        MSAMXMSMSA
        AMXSXMAAMM
        MSAMASMSMX
        XMASAMXAMM
        XXAMMXXAMA
        SMSMSASXSS
        SAXAMASAAA
        MAMMMXMMMM
        MXMXAXMASX
        """;
    private readonly string[] _input = File.ReadAllLines(@"Inputs\Day04.txt");
    
    private string ReadMap(string[] input, out Size size)
    {
        size = new Size(input[0].Length, input.Length);
        return string.Join("", input);
    }

    private int CountXmas(string[] input)
    {
        var map = ReadMap(input, out var size);
        var enabledLocations = new bool[size.Width * size.Height];
        
        var regex = new Regex("X", RegexOptions.Compiled);
        
        var matches = regex.Matches(new string(map));
        var starts = matches.Select(m => m.Index).ToArray();
        
        var count = 0;
        foreach (var start in starts)
        {
            var startx = start % size.Width;
            var starty = start / size.Width;
            // N
            TraverseStraight(startx, starty, 0, -1);
            // NE
            TraverseStraight(startx, starty, 1, -1);
            // E
            TraverseStraight(startx, starty, 1, 0);
            // SE
            TraverseStraight(startx, starty, 1, 1);
            // S
            TraverseStraight(startx, starty, 0, 1);
            // SW
            TraverseStraight(startx, starty, -1, 1);
            // W
            TraverseStraight(startx, starty, -1, 0);
            // NW
            TraverseStraight(startx, starty, -1, -1);
            
            void TraverseStraight(int x, int y, int dirx, int diry, int index = 0)
            {
                if (x < 0 || x >= size.Width || y < 0 || y >= size.Height)
                    return;
                
                var c = map[y * size.Width + x];
                if (c == _xmas[index])
                {
                    int nextIndex = index + 1;
                    if (nextIndex == _xmas.Length)
                    {
                        count++;
                        while(nextIndex > 0)
                        {
                            nextIndex--;
                            enabledLocations[y * size.Width + x] = true;
                            x -= dirx;
                            y -= diry;
                        }
                        return;
                    }
                    
                    TraverseStraight(x + dirx, y + diry, dirx, diry, nextIndex);
                }
            }
        }
        
        var mapVisualizer = new StringBuilder();
        for (int y = 0; y < size.Height; y++)
        {
            for (int x = 0; x < size.Width; x++)
            {
                mapVisualizer.Append(enabledLocations[y * size.Width + x] ? map[y * size.Width + x] : '.');
            }
            mapVisualizer.AppendLine();
        }
        
        File.WriteAllText("map.txt", mapVisualizer.ToString());
        
        return count;
    }
    
    [Fact]
    public void Part1Test()
    {
        Assert.Equal(18, CountXmas(TestInput.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)));
    }
    
    [Fact]
    public void Part1()
    {
        Assert.Equal(2524, CountXmas(_input));
    }

    private int CountMas(string[] input)
    {
        var map = ReadMap(input, out var size);
        var enabledLocations = new bool[size.Width * size.Height];
        
        var regex = new Regex("A", RegexOptions.Compiled);
        
        var matches = regex.Matches(new string(map));
        var starts = matches.Select(m => m.Index).ToArray();
        
        var count = 0;
        foreach (var start in starts)
        {
            var x = start % size.Width;
            var y = start / size.Width;
            
            var mCount = 0;
            var sCount = 0;
            
            bool TryGetMapChar(int x, int y, out char c)
            {
                if (x < 0 || x >= size.Width || y < 0 || y >= size.Height)
                {
                    c = ' ';
                    return false;
                }

                c = map[y * size.Width + x];
                return true;
            }
            
            //NW
            if (!TryGetMapChar(x - 1, y - 1, out var nw) || !CheckDiagonal(nw))
            {
                continue;
            }
            //NE
            if (!TryGetMapChar(x + 1, y - 1, out var ne) || !CheckDiagonal(ne))
            {
                continue;
            }
            //SW
            if (!TryGetMapChar(x - 1, y + 1, out var sw) || !CheckDiagonal(sw, ne == 'M' ? 'S' : 'M'))
            {
                continue;
            }
            //SE
            if (!TryGetMapChar(x + 1, y + 1, out var se) || !CheckDiagonal(se, nw == 'M' ? 'S' : 'M'))
            {
                continue;
            }
            
            if(sCount == 2 && mCount == 2)
            {
                enabledLocations[y * size.Width + x] = true;
                
                //ne
                enabledLocations[(y - 1) * size.Width + x + 1] = true;
                //nw
                enabledLocations[(y - 1) * size.Width + x - 1] = true;
                //se
                enabledLocations[(y + 1) * size.Width + x + 1] = true;
                //sw
                enabledLocations[(y + 1) * size.Width + x - 1] = true;
                
                count++;
            }

            bool CheckDiagonal(char c, char? expected = null)
            {
                if(expected != null && c != expected)
                    return false;
                
                if (c == 'M')
                {
                    mCount++;
                    return true;
                }
                if (c == 'S')
                {
                    sCount++;
                    return true;
                }

                return false;
            }
        }
        
        var mapVisualizer = new StringBuilder();
        for (int y = 0; y < size.Height; y++)
        {
            for (int x = 0; x < size.Width; x++)
            {
                mapVisualizer.Append(enabledLocations[y * size.Width + x] ? map[y * size.Width + x] : '.');
            }
            mapVisualizer.AppendLine();
        }
        File.WriteAllText("map2.txt", mapVisualizer.ToString());

        return count;
    }
    
    [Fact]
    public void Part2Test()
    {
        Assert.Equal(9, CountMas(TestInput.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)));
    }
    
    [Fact]
    public void Part2()
    {
        Assert.Equal(1873, CountMas(_input));
    }
}