using System.Drawing;
using System.Text.RegularExpressions;

namespace Advent_of_Code_2022._2022;

public static class Day15_2022
{
    public static void Run(string testInputPath, string challengeInputPath)
    {
        //Solve(File.ReadAllLines(testInputPath), 10);
        //Solve(File.ReadAllLines(challengeInputPath), 2_000_000);
        
        //Solve2(File.ReadAllLines(testInputPath), 20);
        Solve2(File.ReadAllLines(challengeInputPath), 4_000_000);
    }

    private static void Solve2(string[] lines, int mapSize)
    {
        List<(Point sensor, Point beacon)> sensors = ParseSensors(lines).ToList();
        List<(Point sensor, Point beacon, int dist)> sensorsWithDist = sensors
            .Select(s => (s.sensor, s.beacon, Manhattan(s.sensor, s.beacon)))
            .ToList();
        
        for (int y = 0; y < mapSize; y++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                var currentPoint = new Point(x, y);
                var sensor = sensorsWithDist.FirstOrDefault(s => Manhattan(s.sensor, currentPoint) <= s.dist);
                
                if (sensor == default)
                {
                    Console.WriteLine($"Found: {x},{y}. Signal: {(x * 4_000_000L + y)}");
                    return;
                }
                else
                {
                    var dist = Manhattan(new Point(sensor.sensor.X, currentPoint.Y), sensor.sensor);
                    x = sensor.sensor.X + sensor.dist - dist;
                }
            }
            
            //Console.WriteLine($"y={y}");
        }
    }
    
    private static void Solve(string[] lines, int targetY)
    {
        List<(Point sensor, Point beacon)> sensors = ParseSensors(lines).ToList();

        var ranges = new List<(long start, long end)>();

        foreach ((Point sensor, Point beacon) in sensors)
        {
            int distToBeacon = Manhattan(sensor, beacon);
            int distToTargetLine = Math.Abs(sensor.Y - targetY);

            if (distToBeacon > distToTargetLine)
            {
                int overlappedDistance = distToBeacon - distToTargetLine;
                Console.WriteLine($"S[{sensor.X},{sensor.Y}] Overlaps line {targetY} by {overlappedDistance}. Dist to B[{beacon.X},{beacon.Y}]: {distToBeacon}");
                int oStart = sensor.X - overlappedDistance;
                int oEnd = sensor.X + overlappedDistance;
                Console.WriteLine($"  Overlap is at X: {oStart}..{oEnd}. Size: {oEnd - oStart + 1}");

                ranges.Add((oStart, oEnd));
            }
        }
        
        Console.WriteLine($"All overlaps ({ranges.Count}): {ranges.Sum(x => x.end - x.start)}");
        ranges = ranges.OrderBy(x => x.start).ToList();

        var merged = true;
        while (merged)
        {
            List<(long start, long end)> newRanges = MergeRanges(ranges);
            merged = newRanges.Count != ranges.Count;
            ranges = newRanges;
        }

        Console.WriteLine($"All merged: ({ranges.Count}): {ranges.Sum(x => x.end - x.start)}");
    }

    private static List<(long start, long end)> MergeRanges(List<(long start, long end)> ranges)
    {
        if (ranges.Count == 1)
            return ranges;

        var newRanges = new List<(long start, long end)>();
        
        // take pairs 
        for (var i = 0; i < ranges.Count - 1; i++)
        {
            (long start, long end) r1 = ranges[i];
            (long start, long end) r2 = ranges[i+1];
            bool overlaps = r1.start <= r2.end && r1.end >= r2.start;
            if (!overlaps)
            {
                newRanges.Add(r1);
            }
            else
            {
                newRanges.Add((Math.Min(r1.start, r2.start), Math.Max(r1.end, r2.end)));
                for (int j = i + 2; j < ranges.Count; j++)
                {
                    newRanges.Add(ranges[j]);
                }
                break;
            }
        }

        return newRanges;
    }

    private static int Manhattan(Point a, Point b)
    {
        return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
    }

    private static IEnumerable<(Point sensor, Point beacon)> ParseSensors(string[] lines)
    {
        var pattern =
            new Regex(@"Sensor at x=(?<sx>-?\d+), y=(?<sy>-?\d+): closest beacon is at x=(?<bx>-?\d+), y=(?<by>-?\d+)");
        foreach (string line in lines)
        {
            Match matches = pattern.Match(line);
            yield return (
                new Point(
                    Int32.Parse(matches.Groups["sx"].Value),
                    Int32.Parse(matches.Groups["sy"].Value)
                ),
                new Point(
                    Int32.Parse(matches.Groups["bx"].Value),
                    Int32.Parse(matches.Groups["by"].Value)
                ));
        }
    }
}
