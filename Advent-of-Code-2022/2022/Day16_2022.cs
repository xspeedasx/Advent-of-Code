using System.Text.RegularExpressions;

namespace Advent_of_Code_2022._2022;

public static class Day16_2022
{
    private static Regex LineRx =
        new Regex(@"Valve (?<name>\w+) has flow rate=(?<rate>\d+); tunnels? leads? to valves? (?<neighbors>.+)$");
    private static Dictionary<string, Valve> valves = new();
    public static void Run(string testInputPath, string challengeInputPath)
    {
        Solve(File.ReadAllLines(testInputPath));
        //Solve(File.ReadAllLines(challengeInputPath));
    }

    private static void Solve(string[] lines)
    {
        valves.Clear();
        foreach (string line in lines)
        {
            Match m = LineRx.Match(line);
            string[] neighbors = m.Groups["neighbors"].Value.Split(',', StringSplitOptions.TrimEntries);
            string name = m.Groups["name"].Value;
            var valve = new Valve
            {
                Name = name,
                Rate = int.Parse(m.Groups["rate"].Value),
                Neighbors = neighbors,
                Actions = new Action[neighbors.Length+1]
            };
            
            valve.Actions[0] = new Action(ActionType.OPEN, name);
            for (int i = 0; i < neighbors.Length; i++)
            {
                valve.Actions[i + 1] = new Action(ActionType.MOVE, neighbors[i]);
            }

            valves.Add(name, valve);
            Console.WriteLine($"Added: {name}");
        }

        Console.WriteLine($"max possible release: {valves.Sum(x=> x.Value.Rate*30)}");

        int score = Traverse(new Path(
            "AA",
            0,
            Array.Empty<string>(),
            Array.Empty<Action>()
        )).GetAwaiter().GetResult();
        Console.WriteLine($"Score: {score}");
    }

    private static async Task<int> Traverse(Path path)
    {
        if (path.Actions.Length == 20) return path.Score;

        int remaining = 29 - path.Actions.Length;

        string location = path.Location;
        Valve valve = valves[location];
        Action[] actions = valve.Actions;

        var tasks = new List<Task<int>>();
        
        foreach (Action action in actions)
        {
            switch (action.Type)
            {
                case ActionType.OPEN when !path.OpenValves.Contains(location):
                {
                    tasks.Add(Traverse(new Path(
                        location,
                        path.Score + remaining * valve.Rate,
                        path.OpenValves.Append(location).ToArray(),
                        path.Actions.Append(action).ToArray()
                    )));

                    break;
                }
                case ActionType.MOVE:
                    tasks.Add(Traverse(path with
                    {
                        Location = action.Target,
                        Actions = path.Actions.Append(action).ToArray()
                    }));
                    break;
            }
        }

        int[] results = await Task.WhenAll(tasks);
        return results.Max();
    }

    private record struct Path(
        string Location,
        int Score,
        string[] OpenValves,
        Action[] Actions
    );

    private enum ActionType { MOVE, OPEN }

    private record struct Action(ActionType Type, string Target);

    private class Valve
    {
        public string Name { get; init; } = null!;
        public int Rate { get; init; }
        public string[] Neighbors { get; init; } = null!;
        public Action[] Actions { get; init; } = null!;
    }
}