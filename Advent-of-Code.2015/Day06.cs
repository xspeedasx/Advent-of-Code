using System.Drawing;
using System.Text.RegularExpressions;
using Xunit;
using Xunit.Abstractions;

namespace Advent_of_Code._2015;

public class Day06
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly string[] _input = File.ReadAllLines(@"Inputs\Day06.txt");

    public Day06(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    enum CommandType { ON, OFF, TOGGLE }

    [Fact]
    public void Part1Test()
    {
        var list = new List<Rectangle>();
        
        // list.Add(new Rectangle(1,1,3,1));
        // list.Add(new Rectangle(2,1,1,3));
        
        ProcessCommand("turn on 1,2 through 3,2", ref list);
        ProcessCommand("toggle 2,1 through 2,3", ref list);

        Assert.Equal(4, list.Sum(x => x.Width * x.Height));
    }

    [Fact]
    public void Part1()
    {
        var list = new List<Rectangle>();
        foreach (string line in _input)
        {
            ProcessCommand(line, ref list);
        }
        
        Assert.Equal(0, list.Sum(x => x.Width * x.Height));        
    }

    private void ProcessCommand(string command, ref List<Rectangle> list)
    {
        var match = Regex.Match(command, @"([\w ]+) (\d+,\d+) through (\d+,\d+)");
        var type = match.Groups[1].Value switch
        {
            "turn on" => CommandType.ON,
            "turn off" => CommandType.OFF,
            "toggle" => CommandType.TOGGLE,
            _ => throw new Exception("Unsupported type")
        };
        var leftTop = match.Groups[2].Value.Split(",").Select(int.Parse).ToArray();
        var rightBott = match.Groups[3].Value.Split(",").Select(int.Parse).ToArray();
        _testOutputHelper.WriteLine($"{type} : {leftTop[0]},{leftTop[1]} - {rightBott[0]},{rightBott[1]}");

        //var rect = new Rectangle(rectXY[0], rectXY[1], rectXY2[0] - rectXY[0] + 1, rectXY2[1] - rectXY[1] + 1);
        var rect = Rectangle.FromLTRB(leftTop[0], leftTop[1], rightBott[0], rightBott[1]);
        var intersects = list.Where(x => x.IntersectsWith(rect)).ToArray();
        if (intersects.Length == 0)
        {
            if(type is CommandType.ON or CommandType.TOGGLE)
            {
                list.Add(rect);
            }
            return;
        }

        foreach (Rectangle intersect in intersects)
        {
            list.Remove(intersect);
            Rectangle intrnal = Rectangle.Intersect(intersect, rect);
            //Rectangle
        }
    }
}

class LightsRect
{
    public int X { get; set; }
    public int Y { get; set; }
    public int W { get; set; }
    public int H { get; set; }

    public int Area => W * H;
}