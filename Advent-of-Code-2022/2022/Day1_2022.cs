namespace Advent_of_Code_2022._2022;

public static class Day1_2022
{
    public static void Run(string testInputPath, string challengeInputPath)
    {
        Solve(File.ReadAllLines(testInputPath));
        Solve(File.ReadAllLines(challengeInputPath));
    }

    private static void Solve(string[] lines)
    {
        var elves = new Dictionary<int, int>();
        var elfIdx = 0;
        foreach (string line in lines)
        {
            if (line.Length == 0)
            {
                elfIdx++;
                continue;
            }
            
            if (!elves.ContainsKey(elfIdx))
                elves[elfIdx] = 0;
            
            int elf = elves[elfIdx];            

            elf += int.Parse(line);
            elves[elfIdx] = elf;
        }

        KeyValuePair<int, int> maxElf = elves.MaxBy(x => x.Value);
        Console.WriteLine($"Max elf: {maxElf.Key}, carries: {maxElf.Value} cal");

        List<KeyValuePair<int, int>> max3 = elves.OrderByDescending(x => x.Value).Take(3).ToList();
        
        Console.WriteLine($"Max 3: (total: {max3.Sum(x => x.Value)})");
        foreach (KeyValuePair<int,int> elf in max3)
        {
            Console.WriteLine($"  elf: {elf.Key}, carries: {elf.Value} cal");
        }
    }
}
