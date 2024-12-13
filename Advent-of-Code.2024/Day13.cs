using System.Numerics;
using System.Text.RegularExpressions;
using Xunit;
using Xunit.Abstractions;

namespace Advent_of_Code._2024;

public class Day13(ITestOutputHelper output)
{
    private readonly Arcade[] _input = ParseInput(File.ReadAllLines("Inputs/Day13.txt"));
    private readonly Arcade[] _testInput = ParseInput(File.ReadAllLines("Inputs/TestInputs/Test13.txt"));

    private static Arcade[] ParseInput(string[] lines)
    {
        var arcades = new List<Arcade>();

        PointL a = PointL.Zero;
        PointL b = PointL.Zero;
        PointL prize = PointL.Zero;

        for (var i = 0; i < lines.Length; i++)
        {
            Match match = Regex.Match(lines[i], @".+: X.(\d+), Y.(\d+)");
            switch (i % 4)
            {
                case 0:
                    a = new PointL(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value));
                    break;
                case 1:
                    b = new PointL(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value));
                    break;
                case 2:
                    prize = new PointL(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value));
                    break;
                default:
                    arcades.Add(new Arcade(a, b, prize));
                    break;
            }
        }

        arcades.Add(new Arcade(a, b, prize));

        return arcades.ToArray();
    }

    private static int CalculatePrice(Arcade arcade)
    {
        var apresses = 0;
        var bpresses = 100;

        while (apresses <= 100 && bpresses >= 0)
        {
            PointL currentVector = apresses * arcade.A + bpresses * arcade.B;
            if (currentVector.X == arcade.Prize.X && currentVector.Y == arcade.Prize.Y)
            {
                return 3 * apresses + bpresses;
            }

            if (currentVector.X > arcade.Prize.X || currentVector.Y > arcade.Prize.Y)
            {
                bpresses--;
            }
            else
            {
                apresses++;
            }
        }

        return 0;
    }

    [Fact]
    public void Part1Test1()
    {
        Assert.Equal(480, _testInput.Sum(CalculatePrice));
    }

    [Fact]
    public void Part1()
    {
        Assert.Equal(33921, _input.Sum(CalculatePrice));
    }

    private long CalculatePrice2(Arcade arcade)
    {
        var prize = new PointL(
            arcade.Prize.X + 10_000_000_000_000,
            arcade.Prize.Y + 10_000_000_000_000
        );
        // var apresses = 0L;
        var bpresses = Math.Max(prize.X / arcade.B.X, prize.Y / arcade.B.Y);
        var bloc = bpresses * arcade.B;
        output.WriteLine($".  prize at {prize.X:n0}, {prize.Y:n0}");
        output.WriteLine($"would be at {bloc.X:n0}, {bloc.Y:n0}");
        var adiff = arcade.A - arcade.B;
        output.WriteLine($"adiff: {adiff.X:n0}, {adiff.Y:n0}");
        

        return 0;
    }
    
    [Fact]
    public void Part2Test1()
    {
        Assert.Equal(0, CalculatePrice2(_testInput[1]));
        // Assert.Equal(480, _testInput.Sum(CalculatePrice2));
    }

    private class PointL(long x, long y)
    {
        public long X { get; set; } = x;
        public long Y { get; set; } = y;
        
        public static PointL operator +(PointL a, PointL b) => new PointL(a.X + b.X, a.Y + b.Y);
        public static PointL operator -(PointL a, PointL b) => new PointL(a.X - b.X, a.Y - b.Y);
        public static PointL operator *(long a, PointL b) => new PointL(a * b.X, a * b.Y);
        public static PointL operator *(PointL a, long b) => new PointL(a.X * b, a.Y * b);
        
        public static PointL Zero => new PointL(0, 0);
    }

    private record struct Arcade(PointL A, PointL B, PointL Prize);
}