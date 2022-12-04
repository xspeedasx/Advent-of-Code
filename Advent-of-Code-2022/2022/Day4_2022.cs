namespace Advent_of_Code_2022._2022;

public static class Day4_2022
{
    public static void Run(string testInputPath, string challengeInputPath)
    {
        Solve(File.ReadAllLines(testInputPath));
        Solve(File.ReadAllLines(challengeInputPath));
    }

    private static void Solve(string[] lines)
    {
        var fullyContainedSum = 0;
        var overlappingSum = 0;
        foreach (var line in lines)
        {
            var pairs = line.Split(',');
            var a = Section.Parse(pairs[0]);
            var b = Section.Parse(pairs[1]);
            
            fullyContainedSum += IsFullyContained(a, b) ? 1 : 0;
            overlappingSum += IsOverlapping(a, b) ? 1 : 0;
        }

        Console.WriteLine($"FullyContained: {fullyContainedSum} pairs");
        Console.WriteLine($"Overlapping: {overlappingSum} pairs");
    }

    private static bool IsFullyContained(Section a, Section b)
    {
        return
            // b in a?
            b.Start >= a.Start && b.Start <= a.End && b.End <= a.End ||
            // a in b?
            a.Start >= b.Start && a.Start <= b.End && a.End <= b.End;
    }

    private static bool IsOverlapping(Section a, Section b)
    {
        return
            a.Start <= b.End && b.Start <= a.End;
    }
    
    private record struct Section(int Start, int End)
    {
        public static Section Parse(string input)
        {
            var splits = input.Split("-");
            if (splits.Length != 2) throw new IndexOutOfRangeException($"{input} could not be parsed");
            return new Section(int.Parse(splits[0]), int.Parse(splits[1]));
        }
    }
}
