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
        var apresses = 0L;
        long bpresses = Math.Max(prize.X / arcade.B.X, prize.Y / arcade.B.Y);
        PointL bloc = bpresses * arcade.B;
        output.WriteLine($"prize at {prize.X:n0}, {prize.Y:n0}");
        output.WriteLine($"would be at {bloc.X:n0}, {bloc.Y:n0}");
        PointL adiff = arcade.A - arcade.B;
        output.WriteLine($"adiff: {adiff.X:n0}, {adiff.Y:n0}");

        long xdiff = Math.Abs(prize.X - bloc.X);
        long ydiff = Math.Abs(prize.Y - bloc.Y);

        if (ydiff < xdiff)
        {
            // find lcd for Y coord
            var aCnt = 0;
            var bCnt = 0;
            var currentDiff = 0L;

            var iterations = 10000;

            while (currentDiff != ydiff && iterations-- > 0)
                if (currentDiff < ydiff)
                {
                    aCnt++;
                    currentDiff += arcade.A.Y;
                }
                else
                {
                    bCnt++;
                    currentDiff -= arcade.B.Y;
                }

            if (iterations == 0)
            {
                return 0;
            }

            output.WriteLine($"aCnt: {aCnt}, bCnt: {bCnt}");
            apresses = aCnt;
            bpresses -= bCnt;

            bloc = bloc + aCnt * arcade.A - bCnt * arcade.B;
            output.WriteLine($"after adjustment: {bloc.X:n0}, {bloc.Y:n0}");

            // find loop for Y coord
            aCnt = 0;
            bCnt = 1;
            currentDiff = -arcade.B.Y;

            iterations = 10000;
            while (currentDiff != 0 && iterations-- > 0)
                if (currentDiff < ydiff)
                {
                    aCnt++;
                    currentDiff += arcade.A.Y;
                }
                else
                {
                    bCnt++;
                    currentDiff -= arcade.B.Y;
                }

            if (iterations == 0)
            {
                return 0;
            }

            output.WriteLine($"to make loop: aCnt: {aCnt}, bCnt: {bCnt}");

            // how many a loops to match prize:
            if (aCnt == 0 || arcade.A.X == 0)
            {
                throw new Exception("aCnt == 0 || arcade.A.X == 0");
            }

            long loops = (bloc.X - prize.X) / (aCnt * arcade.A.X);

            output.WriteLine($"loops: {loops}");

            apresses += loops * aCnt;
            bpresses -= loops * bCnt;
        }
        else
        {
            // find lcd for X coord
            var aCnt = 0;
            var bCnt = 0;
            var currentDiff = 0L;

            var iterations = 10000;

            while (currentDiff != xdiff && iterations-- > 0)
                if (currentDiff < xdiff)
                {
                    aCnt++;
                    currentDiff += arcade.A.X;
                }
                else
                {
                    bCnt++;
                    currentDiff -= arcade.B.X;
                }

            if (iterations == 0)
            {
                return 0;
            }

            output.WriteLine($"aCnt: {aCnt}, bCnt: {bCnt}");
            apresses = aCnt;
            bpresses -= bCnt;

            bloc = bloc + aCnt * arcade.A - bCnt * arcade.B;
            output.WriteLine($"after adjustment: {bloc.X:n0}, {bloc.Y:n0}");

            // find loop for X coord
            aCnt = 0;
            bCnt = 1;
            currentDiff = -arcade.B.X;

            iterations = 10000;
            while (currentDiff != 0 && iterations-- > 0)
                if (currentDiff < xdiff)
                {
                    aCnt++;
                    currentDiff += arcade.A.X;
                }
                else
                {
                    bCnt++;
                    currentDiff -= arcade.B.X;
                }

            if (iterations == 0)
            {
                return 0;
            }

            output.WriteLine($"to make loop: aCnt: {aCnt}, bCnt: {bCnt}");

            // how many a loops to match prize:
            if (aCnt == 0 || arcade.A.Y == 0)
            {
                throw new Exception("aCnt == 0 || arcade.A.Y == 0");
            }

            long loops = (bloc.Y - prize.Y) / (aCnt * arcade.A.Y);

            output.WriteLine($"loops: {loops}");

            apresses += loops * aCnt;
            bpresses -= loops * bCnt;
        }

        if (apresses < 0 || bpresses < 0)
            //throw new Exception("apresses < 0 || bpresses < 0");
        {
            return 0;
        }

        return apresses * 3 + bpresses;
    }

    private long CalculatePriceWithMaths(Arcade arcade)
    {
        // arcade.A.X = x1, arcade.A.Y = y1
        // arcade.B.X = x2, arcade.B.Y = y2
        // arcade.Prize.X = px, arcade.Prize.Y = py
        // equation 1 = a * x1 + b * x2 = px;
        // equation 2 = a * y1 + b * y2 = py;
        // a * x1 + b * x2 - px = 0;
        // a * y1 + b * y2 - py = 0;
        // b = (px - a * x1) / x2;
        // a * y1 + ((px - a * x1) / x2) * y2 - py = 0;
        // a * y1 + (px * y2 - a * x1 * y2) / x2 - py = 0;
        // a * y1 * x2 + px * y2 - a * x1 * y2 - py * x2 = 0;
        // a * y1 * x2 - a * x1 * y2 = py * x2 - px * y2;
        // a * (y1 * x2 - x1 * y2) = py * x2 - px * y2;
        // a = (py * x2 - px * y2) / (y1 * x2 - x1 * y2);

        long aLeftPart = arcade.Prize.Y * arcade.B.X - arcade.Prize.X * arcade.B.Y;
        long aRightPart = arcade.A.Y * arcade.B.X - arcade.A.X * arcade.B.Y;
        long a = aLeftPart / aRightPart;
        if (aLeftPart % aRightPart != 0)
        {
            return 0;
        }

        long bLeftPart = arcade.Prize.X - a * arcade.A.X;
        long b = bLeftPart / arcade.B.X;
        if (bLeftPart % arcade.B.X != 0)
        {
            return 0;
        }

        return a * 3 + b;
    }

    private long CheckBothSides(Arcade arc)
    {
        long ans = CalculatePriceWithMaths(arc);
        return ans != 0
            ? ans
            : CalculatePriceWithMaths(arc with { A = arc.B, B = arc.A });
    }

    [Fact]
    public void Part2Test1()
    {
        Assert.Equal(480, _testInput.Sum(CheckBothSides));
    }

    [Fact]
    public void Part2()
    {
        Assert.Equal(82261957837868, _input
            .Select(a => a with
            {
                Prize = new PointL(
                    a.Prize.X + 10_000_000_000_000,
                    a.Prize.Y + 10_000_000_000_000
                )
            })
            .Sum(CheckBothSides));
    }

    private class PointL(long x, long y)
    {
        public long X { get; } = x;
        public long Y { get; } = y;

        public static PointL Zero => new(0, 0);

        public static PointL operator +(PointL a, PointL b)
        {
            return new PointL(a.X + b.X, a.Y + b.Y);
        }

        public static PointL operator -(PointL a, PointL b)
        {
            return new PointL(a.X - b.X, a.Y - b.Y);
        }

        public static PointL operator *(long a, PointL b)
        {
            return new PointL(a * b.X, a * b.Y);
        }

        public static PointL operator *(PointL a, long b)
        {
            return new PointL(a.X * b, a.Y * b);
        }
    }

    private record struct Arcade(PointL A, PointL B, PointL Prize);
}