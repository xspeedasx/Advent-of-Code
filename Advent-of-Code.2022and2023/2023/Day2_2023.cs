using System.Text.RegularExpressions;

namespace Advent_of_Code._2023;

public static class Day2_2023
{
    public static void Run(string testInputPath, string challengeInputPath)
    {
        // part 1
        Solve(File.ReadAllLines(testInputPath));
        Solve(File.ReadAllLines(challengeInputPath));

        // part 2
        Solve2(File.ReadAllLines(testInputPath));
        Solve2(File.ReadAllLines(challengeInputPath));
    }
    private static void Solve(string[] games)
    {
        var bag = (red: 12, green: 13, blue: 14);

        var sum = 0;
        foreach (string game in games)
        {
            var valid = true;
            Match match = Regex.Match(game, @"Game (\d+):");
            int id = int.Parse(match.Groups[1].Value);
            string[] draws = game[(match.Length + 1)..].Split(";", StringSplitOptions.TrimEntries);
            foreach (string draw in draws)
            {
                string[] cubeCounts = draw.Split(",", StringSplitOptions.TrimEntries);
                foreach (string cubeCount in cubeCounts)
                {
                    string[] splits = cubeCount.Split(" ");
                    int cnt = int.Parse(splits[0]);
                    string color = splits[1];
                    valid = color switch
                    {
                        "red" => cnt <= bag.red,
                        "green" => cnt <= bag.green,
                        "blue" => cnt <= bag.blue,
                        _ => false
                    };
                    if (!valid)
                    {
                        break;
                    }
                }
                if (!valid)
                {
                    break;
                }
            }

            if (valid)
            {
                sum += id;
            }
        }
        Console.WriteLine($"part 1 answer: {sum}");
    }
    
    private static void Solve2(string[] games)
    {
        var sum = 0;
        foreach (string game in games)
        {
            var gameBag = (red: 0, green: 0, blue: 0);
            Match match = Regex.Match(game, @"Game (\d+):");
            int id = int.Parse(match.Groups[1].Value);
            string[] draws = game[(match.Length + 1)..].Split(";", StringSplitOptions.TrimEntries);
            foreach (string draw in draws)
            {
                string[] cubeCounts = draw.Split(",", StringSplitOptions.TrimEntries);
                foreach (string cubeCount in cubeCounts)
                {
                    string[] splits = cubeCount.Split(" ");
                    int cnt = int.Parse(splits[0]);
                    string color = splits[1];
                    switch (color)
                    {
                        case "red":
                            if (gameBag.red < cnt)
                            {
                                gameBag.red = cnt;
                            }
                            break;
                        case "green":
                            if (gameBag.green < cnt)
                            {
                                gameBag.green = cnt;
                            }
                            break;
                        case "blue":
                            if (gameBag.blue < cnt)
                            {
                                gameBag.blue = cnt;
                            }
                            break;
                    }
                }
            }

            int power = gameBag.red * gameBag.green * gameBag.blue;
            sum += power;
        }
        Console.WriteLine($"part 2 answer: {sum}");
    }
}