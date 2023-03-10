using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace Advent_of_Code_2022._2022;

public static class Day16_2022
{
    private static readonly Regex LineRx =
        new Regex(@"Valve (?<name>\w+) has flow rate=(?<rate>\d+); tunnels? leads? to valves? (?<neighbors>.+)$");
    private static readonly Dictionary<string, Valve> Valves = new();
    private static readonly Dictionary<string, Dictionary<string, int>> Distances = new();
    public static void Run(string testInputPath, string challengeInputPath)
    {
        Solve(File.ReadAllLines(testInputPath));
        Solve(File.ReadAllLines(challengeInputPath));
    }

    private static void Solve(string[] lines)
    {
        Valves.Clear();
        foreach (string line in lines)
        {
            Match m = LineRx.Match(line);
            string[] neighbors = m.Groups["neighbors"].Value.Split(',', StringSplitOptions.TrimEntries);
            string name = m.Groups["name"].Value;
            var valve = new Valve
            {
                Name = name,
                Rate = int.Parse(m.Groups["rate"].Value),
                Neighbors = neighbors
            };

            Valves.Add(name, valve);
        }
        
        // draw graph:
        foreach ((string _, Valve v) in Valves)
        {
            foreach (string neighbor in v.Neighbors)
            {
                Console.WriteLine($"{v.Name} {neighbor}");
            }
        }
        
        Console.WriteLine("calculating distances");
        foreach ((string v1, Valve _) in Valves)
        {
            foreach ((string v2, Valve _) in Valves)
            {
                if (v1 == v2) continue;
                int d = FindDistance(Valves, v1, v2);
                
                if (!Distances.ContainsKey(v1))
                {
                    Distances[v1] = new Dictionary<string, int>();
                }
                Distances[v1][v2] = d;
                if (!Distances.ContainsKey(v2))
                {
                    Distances[v2] = new Dictionary<string, int>();
                }
                Distances[v2][v1] = d;
            }
        }

        Console.WriteLine($"Total release with all valves open: {Valves.Sum(x=> x.Value.Rate*30)}");
        
        var maxScore = 0;
        var pathLenTerritory = 0;
        var queue = new ConcurrentQueue<Move>();
        
        queue.Enqueue(new Move("AA", 0, 30, new HashSet<string>{ "AA" }));

        while (queue.Count > 0)
        {
            var nextQueue = new ConcurrentQueue<Move>();
            Parallel.ForEach(queue, m =>
            {
                (string? currentLocation, int score, int remainingTime, HashSet<string> visited) = m;
                
                if (visited.Count > pathLenTerritory)
                {
                    pathLenTerritory = visited.Count;
                    Console.WriteLine($"Visiting {pathLenTerritory} path length teritory!");
                }

                if (score > maxScore)
                {
                    maxScore = score;
                    Console.WriteLine(
                        $"Visited valves {String.Join(",", visited)}. Score: {score}, Rem. time: {remainingTime}");
                }

                Dictionary<string, int> targets = Distances[currentLocation];
                foreach ((string target, int dist) in targets)
                {
                    if (target == currentLocation) continue;
                    if (visited.Contains(target)) continue;
                    int targetFlow = Valves[target].Rate;
                    if(targetFlow == 0) continue;
                    
                    // walk there, then open
                    int remainTimeAfterWalkingAndOpening = remainingTime - dist - 1;
                    if (remainTimeAfterWalkingAndOpening <= 0) continue;

                    int scoreAfter = score + remainTimeAfterWalkingAndOpening * targetFlow;
                    
                    var nextMove = new Move(
                        target,
                        scoreAfter,
                        remainTimeAfterWalkingAndOpening,
                        visited.Append(target).ToHashSet()
                    );
                    lock (nextQueue)
                    {
                        nextQueue.Enqueue(nextMove);
                    }
                }
            });
            queue = nextQueue;
            Console.WriteLine($"Queue passed. Next: {nextQueue.Count} items.");
        }

        Console.WriteLine($"Done. Best score: {maxScore}");
    }

    private static int FindDistance(Dictionary<string, Valve> valves, string v1, string v2)
    {
        var seen = new HashSet<string>();
        var q = new Queue<(string, int)>();
        q.Enqueue((v1, 0));

        int sanity = valves.Count * valves.Count;
        while (q.Count > 0)
        {
            (string n, int d) = q.Dequeue();
            if(!seen.Add(n))
                continue;
            foreach (string neighbor in valves[n].Neighbors)
            {
                if (neighbor == v2)
                {
                    return d + 1;
                }
                q.Enqueue((neighbor, d + 1));
            }

            if (sanity-- > 0) continue;
            Console.WriteLine("WARNING: Sanity check reached!");
            break;
        }

        return 0;
    }

    private record struct Move(
        string CurrentLocation,
        int Score,
        int RemainingTime,
        HashSet<string> Visited
    );

    private class Valve
    {
        public string Name { get; init; } = null!;
        public int Rate { get; init; }
        public string[] Neighbors { get; init; } = null!;
    }
}