namespace Advent_of_Code._2022;

public static class Day3_2022
{
    public static void Run(string testInputPath, string challengeInputPath)
    {
        // Part 1
        Solve(File.ReadAllLines(testInputPath));
        Solve(File.ReadAllLines(challengeInputPath));
        // Part 2
        Solve2(File.ReadAllLines(testInputPath));
        Solve2(File.ReadAllLines(challengeInputPath));
    }

    private static void Solve(string[] lines)
    {
        var sum = 0;
        foreach (var line in lines)
        {
            var a = line[..(line.Length / 2)];
            var b = line[(line.Length / 2)..];

            Console.WriteLine($"a : {a}");
            Console.WriteLine($"b : {b}");

            var matchingChar = GetMatchingChar(new[] { ToSet(a), ToSet(b) });
            var priority = GetPriority(matchingChar);
            
            Console.WriteLine($"Match: {matchingChar}, priority: {priority}");
            sum += priority;
            Console.WriteLine();
        }

        Console.WriteLine($"Sum: {sum}");
    }

    private static void Solve2(string[] lines)
    {
        var sum = 0;
        foreach (var group in lines.Chunk(3))
        {
            var sets = group.Select(ToSet).ToArray();
            var matchingChar = GetMatchingChar(sets);
            var priority = GetPriority(matchingChar);
            Console.WriteLine($"Match: {matchingChar}, priority: {priority}");
            sum += priority;
            Console.WriteLine();
        }
        Console.WriteLine($"Sum: {sum}");
    }

    private static int GetPriority(char c) =>
        !char.IsLetter(c)
            ? 0
            : char.IsLower(c)
                ? 1 + c - 'a'
                : 27 + c - 'A';

    private static HashSet<char> ToSet(string a)
    {
        var aset = new HashSet<char>();
        foreach (var c in a)
        {
            aset.Add(c);
        }

        return aset;
    }

    private static char GetMatchingChar(HashSet<char>[] sets)
    {
        foreach (var c in sets[0])
        {
            if (!sets.All(s => s.Contains(c))) continue;
            return c;
        }
        return '\0';
    }
}
