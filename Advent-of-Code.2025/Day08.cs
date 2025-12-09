using System.Numerics;
using Xunit;
using Xunit.Abstractions;

namespace Advent_of_Code._2025;

public class Day08(ITestOutputHelper output)
{
    private string TestInput =
        """
        162,817,812
        57,618,57
        906,360,560
        592,479,940
        352,342,300
        466,668,158
        542,29,236
        431,825,988
        739,650,466
        52,470,668
        216,146,977
        819,987,18
        117,168,530
        805,96,715
        346,949,466
        970,615,88
        941,993,340
        862,61,35
        984,92,344
        425,690,689
        """;

    private readonly string[] _input = File.ReadAllLines(@"Inputs\Day08.txt");

    [Fact]
    public void Part1Test()
    {
        long ans = JoinAndGetBiggestSizes(TestInput.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries), 10);
        Assert.Equal(40, ans);
    }

    [Fact]
    public void Part1()
    {
        long ans = JoinAndGetBiggestSizes(_input, 1000);
        Assert.Equal(47040, ans);
    }

    private long JoinAndGetBiggestSizes(string[] input, int connections)
    {
        Vector3[] vectors = GetVectors(input);
        output.WriteLine($"Loaded {vectors.Length} vectors");
        Dictionary<(Vector3, Vector3), float> distances = GetDistances(vectors);
        output.WriteLine($"Calculated {distances.Count} distances");
        var groups = vectors.Select(v => new List<Vector3> { v }).ToList();

        var minDist = 0f;
        for (int i = 0; i < connections; i++)
        {
            var nextMin = distances.Where(kv => kv.Value > minDist).MinBy(kv => kv.Value);
            minDist = nextMin.Value;
            var (a, b) = nextMin.Key;
            var groupA = groups.First(g => g.Contains(a));
            var groupB = groups.First(g => g.Contains(b));
            if(groupA == groupB)
                continue;
            groupA.AddRange(groupB);
            groups.Remove(groupB);
        }

        var biggest3 = groups.OrderByDescending(g => g.Count).Take(3).Select(g => g.Count);
        
        return biggest3.Aggregate(1, (acc, val) => acc * val);
    }

    private Dictionary<(Vector3, Vector3), float> GetDistances(Vector3[] vectors)
    {
        var distances = new Dictionary<(Vector3, Vector3), float>();
        
        for (int i = 0; i < vectors.Length - 1; i++)
        {
            Vector3 pointA = vectors[i];
            for (int j = i + 1; j < vectors.Length; j++)
            {
                Vector3 pointB = vectors[j];
                float dist = Vector3.Distance(pointA, pointB);
                distances.Add((pointA, pointB), dist);
                // distances.Add((pointB, pointA), dist);
            }
        }
        return distances;
    }

    private static Vector3[] GetVectors(string[] input)
    {
        Vector3[] vectors = input.Select(line =>
        {
            string[] splits = line.Split(',', StringSplitOptions.TrimEntries);
            return new Vector3(
                float.Parse(splits[0]),
                float.Parse(splits[1]),
                float.Parse(splits[2])
            );
        }).ToArray();
        return vectors;
    }

    [Fact]
    public void Part2Test()
    {
        long ans = JoinUntilAllAreConnected(TestInput.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries));
        Assert.Equal(25272, ans);
    }

    [Fact]
    public void Part2()
    {
        long ans = JoinUntilAllAreConnected(_input);
        Assert.Equal(4884971896, ans);
    }

    private long JoinUntilAllAreConnected(string[] input)
    {
        Vector3[] vectors = GetVectors(input);
        output.WriteLine($"Loaded {vectors.Length} vectors");
        Dictionary<(Vector3, Vector3), float> distances = GetDistances(vectors);
        output.WriteLine($"Calculated {distances.Count} distances");
        var groups = vectors.Select(v => new List<Vector3> { v }).ToList();

        var minDist = 0f;
        while(true)
        {
            var nextMin = distances.Where(kv => kv.Value > minDist).MinBy(kv => kv.Value);
            minDist = nextMin.Value;
            var (a, b) = nextMin.Key;
            var groupA = groups.First(g => g.Contains(a));
            var groupB = groups.First(g => g.Contains(b));
            if(groupA == groupB)
                continue;
            groupA.AddRange(groupB);
            groups.Remove(groupB);
            output.WriteLine($"Joined groups of {groupA.Count} and {groupB.Count}, now have {groups.Count} groups");
            if (groups.Count == 1)
            {
                return (long)a.X * (long)b.X;
                break;
            }
        }

        return 0;
    }
}