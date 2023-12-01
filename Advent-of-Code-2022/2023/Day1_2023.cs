using System.Text.RegularExpressions;

namespace Advent_of_Code_2022._2023;

public static class Day1_2023
{
    public static void Run(string testInputPath, string challengeInputPath)
    {
        // part 1
        Solve(File.ReadAllLines(testInputPath));
        Solve(File.ReadAllLines(challengeInputPath));

        // part 2
        var testpath2 = Path.Combine(
            Path.GetDirectoryName(testInputPath)!,
            Path.GetFileNameWithoutExtension(testInputPath) + "_2.txt");
        Solve2(File.ReadAllLines(testpath2));
        Solve2(File.ReadAllLines(challengeInputPath));
        Solve2Regex(File.ReadAllLines(testpath2));
        Solve2Regex(File.ReadAllLines(challengeInputPath));
    }

    private static void Solve2Regex(string[] lines)
    {
        var sum = 0;
        foreach (var line in lines)
        {
            var matches = Regex.Matches(line, @"(?=(\d|one|two|three|four|five|six|seven|eight|nine))");
            var calValue = ParseMatch(matches[0].Groups[1].Value) * 10 + ParseMatch(matches[^1].Groups[1].Value);
            sum += calValue;
        }

        Console.WriteLine(sum);
        
        static int ParseMatch(string m)
        {
            return m switch
            {
                "one" => 1,
                "two" => 2,
                "three" => 3,
                "four" => 4,
                "five" => 5,
                "six" => 6,
                "seven" => 7,
                "eight" => 8,
                "nine" => 9,
                _ => int.Parse(m)
            };
        }
    }
    
    private static void Solve2(string[] lines)
    {
        var sum = 0;
        foreach (var line in lines)
        {
            var matches = GetTokens(line);
            
            var minKey = matches.Keys.Min();
            var maxKey = matches.Keys.Max();
            var calValue = matches[minKey] * 10 + matches[maxKey];
            sum += calValue;
        }

        Console.WriteLine(sum);
        return;

        static Dictionary<int, int> GetTokens(string line)
        {
            var tokens = new Dictionary<int, int>();
            for (var i = 0; i < line.Length; i++)
            {
                var token = MatchKey(line[i..]);
                if (token != null)
                {
                    tokens[i] = token.Value;
                }
            }

            return tokens;

            int? MatchKey(string s)
            {
                if (char.IsDigit(s[0]))
                {
                    return int.Parse(s[0] + "");
                }

                if (s[..Math.Min(3, s.Length)].Equals("one", StringComparison.OrdinalIgnoreCase)) return 1;
                if (s[..Math.Min(3, s.Length)].Equals("two", StringComparison.OrdinalIgnoreCase)) return 2;
                if (s[..Math.Min(5, s.Length)].Equals("three", StringComparison.OrdinalIgnoreCase)) return 3;
                if (s[..Math.Min(4, s.Length)].Equals("four", StringComparison.OrdinalIgnoreCase)) return 4;
                if (s[..Math.Min(4, s.Length)].Equals("five", StringComparison.OrdinalIgnoreCase)) return 5;
                if (s[..Math.Min(3, s.Length)].Equals("six", StringComparison.OrdinalIgnoreCase)) return 6;
                if (s[..Math.Min(5, s.Length)].Equals("seven", StringComparison.OrdinalIgnoreCase)) return 7;
                if (s[..Math.Min(5, s.Length)].Equals("eight", StringComparison.OrdinalIgnoreCase)) return 8;
                if (s[..Math.Min(4, s.Length)].Equals("nine", StringComparison.OrdinalIgnoreCase)) return 9;
                return null;
            }
        }
    }

    private static void Solve(string[] lines)
    {
        var sum = 0;
        foreach (var line in lines)
        {
            var matches = Regex.Matches(line, @"(\d)");
            var calValue = int.Parse(matches[0].Value) * 10 + int.Parse(matches[^1].Value);
            sum += calValue;
        }

        Console.WriteLine(sum);
    }
}