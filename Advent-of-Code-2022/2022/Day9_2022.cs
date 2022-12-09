using System.Numerics;

namespace Advent_of_Code_2022._2022;

public static class Day9_2022
{
    public static void Run(string testInputPath, string challengeInputPath)
    {
        Console.WriteLine("Part 1 test");
        Solve(File.ReadAllLines(testInputPath), 2);
        Console.WriteLine("Part 1");
        Solve(File.ReadAllLines(challengeInputPath), 2);
        Console.WriteLine("Part 1 test");
        Solve(File.ReadAllLines(testInputPath), 10);
        Console.WriteLine("Part 1 test 2");
        Solve(File.ReadAllLines(testInputPath.Replace(".txt", "-2.txt")), 10);
        Console.WriteLine("Part 2");
        Solve(File.ReadAllLines(challengeInputPath), 10);
    }

    private static void Solve(string[] lines, int ropeLength)
    {
        var movements = new Dictionary<char, Vector2>
        {
            ['U'] = new(0, -1),
            ['D'] = new(0, 1),
            ['L'] = new(-1, 0),
            ['R'] = new(1, 0)
        };

        Vector2[] ropePos = new Vector2[10].Select(_ => new Vector2(0, 0)).ToArray();

        var positionsTailVisited = new HashSet<Vector2> { ropePos[^1] };

        foreach (var line in lines)
        {
            var splits = line.Split(" ");
            var dir = splits[0][0];
            var cnt = int.Parse(splits[1]);

            for (var i = 0; i < cnt; i++)
            {
                // move head
                Vector2 hmove = movements[dir];
                ropePos[0] += hmove;

                // adjust tail(s)
                for (var knot = 1; knot < ropeLength; knot++)
                {
                    Vector2 prev = ropePos[knot - 1];
                    Vector2 curr = ropePos[knot];

                    var dist = Vector2.Distance(prev, curr);
                    if (dist < 2) break;

                    Vector2 posDiff = prev - curr;

                    // normalize
                    var normalDiff = new Vector2(
                        Math.Clamp(posDiff.X, -1, 1),
                        Math.Clamp(posDiff.Y, -1, 1)
                    );

                    ropePos[knot] += normalDiff;
                }

                // check for new position
                positionsTailVisited.Add(ropePos[^1]);
            }
        }

        Console.WriteLine($"Total positions: {positionsTailVisited.Count}");
    }
}