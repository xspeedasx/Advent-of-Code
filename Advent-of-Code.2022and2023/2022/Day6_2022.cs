using System.Text.RegularExpressions;

namespace Advent_of_Code._2022;

public static class Day6_2022
{
    private static Regex startPattern = new Regex(@"(.)((?!\1).)((?!\1|\2).)((?!\1|\2|\3).)");
    public static void Run(string testInputPath, string challengeInputPath)
    {
        // Part 1
        Console.WriteLine("--- part rx ---");
        SolveRx(File.ReadAllLines(testInputPath));
        SolveRx(File.ReadAllLines(challengeInputPath));
        Console.WriteLine("--- part 1 ---");
        Solve(File.ReadAllLines(testInputPath), 4);
        Solve(File.ReadAllLines(challengeInputPath), 4);
        // Part 2
        Console.WriteLine("--- part 2 ---");
        Solve(File.ReadAllLines(testInputPath), 14);
        Solve(File.ReadAllLines(challengeInputPath), 14);
    }

    private static void SolveRx(string[] lines)
    {
        // I'll leave this as a solution with regex for fun since that's how I solved part 1 :)
        foreach (string line in lines)
        {
            Match match = startPattern.Match(line);
            Console.WriteLine($"marker: {match.Value}, marker end: {(match.Index+4)}");
        }
    }
    
    private static void Solve(string[] lines, int markerSize)
    {
        foreach (string line in lines)
        {
            int markerStart = FindMarker(line, markerSize);
            int markerEnd = markerStart + markerSize;
            Console.WriteLine($"marker: {line[markerStart..markerEnd]}, marker end: {markerEnd}");
        }
    }

    private static int FindMarker(string input, int markerSize)
    {
        var markerBuffer = new HashSet<char>();

        for (var i = 0; i < input.Length; i++)
        {
            char c = input[i];
            if (!markerBuffer.Add(c))
            {
                i = LastOccurrence(input[..i], c);
                markerBuffer.Clear();
            }

            if (markerBuffer.Count == markerSize)
                return i + 1 - markerSize;
        }

        return input.Length;
    }

    private static int LastOccurrence(string input, char c)
    {
        for (int i = input.Length - 1; i > 0; i--)
        {
            if (input[i] == c)
                return i;
        }

        return 0;
    }
}
