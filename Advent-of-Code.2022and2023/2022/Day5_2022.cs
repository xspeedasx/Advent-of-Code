using System.Text;
using System.Text.RegularExpressions;

namespace Advent_of_Code._2022;

public static class Day5_2022
{
    private static Regex rxMove = new Regex(@"move (?<cnt>\d+) from (?<from>\d+) to (?<to>\d+)");
    private enum CraneType { CrateMover_9000, CrateMover_9001 }
    public static void Run(string testInputPath, string challengeInputPath)
    {
        // Part 1
        Solve(File.ReadAllLines(testInputPath), CraneType.CrateMover_9000);
        Solve(File.ReadAllLines(challengeInputPath), CraneType.CrateMover_9000);
        // Part 2
        Solve(File.ReadAllLines(testInputPath), CraneType.CrateMover_9001);
        Solve(File.ReadAllLines(challengeInputPath), CraneType.CrateMover_9001);
    }

    private static void Solve(string[] lines, CraneType craneType)
    {
        int numberOfStacks = (lines[0].Length + 1) / 4;
        Stack<char>[] reverseStacks = Enumerable.Range(0, numberOfStacks).Select(x => new Stack<char>()).ToArray();
        List<string> moveLines = new();

        foreach (string line in lines)
        {
            if (line.Length > 0 && !line.StartsWith("move") && !Char.IsDigit(line[1]))
            {
                for (var i = 0; i < numberOfStacks; i++)
                {
                    char letter = line[i * 4 + 1];
                    if (letter != ' ')
                    {
                        reverseStacks[i].Push(letter);
                    }
                }
            }

            if (line.StartsWith("move"))
            {
                moveLines.Add(line);
            }
        }

        Stack<char>[] stacks = reverseStacks.Select(x => new Stack<char>(x)).ToArray();
        
        foreach (string moveLine in moveLines)
        {
            Match m = rxMove.Match(moveLine);
            int cnt = Int32.Parse(m.Groups["cnt"].Value);
            int from = Int32.Parse(m.Groups["from"].Value);
            int to = Int32.Parse(m.Groups["to"].Value);

            if (craneType == CraneType.CrateMover_9000)
            {
                for (var i = 0; i < cnt; i++)
                {
                    char c = stacks[from - 1].Pop();
                    stacks[to - 1].Push(c);
                }
            }
            else
            {
                Stack<char> heap = new();
                for (var i = 0; i < cnt; i++)
                {
                    heap.Push(stacks[from - 1].Pop());
                }
                for (var i = 0; i < cnt; i++)
                {
                    stacks[to - 1].Push(heap.Pop());
                }
            }
        }

        var ans = new StringBuilder();
        foreach (Stack<char> stack in stacks)
        {
            char topChar = stack.Peek();
            ans.Append(topChar);
        }

        Console.WriteLine($"Answer ({craneType}): {ans}");
    }
}
