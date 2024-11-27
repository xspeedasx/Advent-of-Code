using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace Advent_of_Code._2022;

public static class Day16_2022
{
    private static readonly Regex LineRx =
        new(@"Valve (?<name>\w+) has flow rate=(?<rate>\d+); tunnels? leads? to valves? (?<neighbors>.+)$");
    private static readonly Dictionary<string, Valve> Valves = new();
    private static readonly List<string> GoodValves = new();
    private static readonly Dictionary<string, Dictionary<string, int>> Distances = new();
    public static void Run(string testInputPath, string challengeInputPath)
    {
        // Solve(File.ReadAllLines(testInputPath));
        // Solve(File.ReadAllLines(challengeInputPath));
        
        //Solve2(File.ReadAllLines(testInputPath));
        Solve2(File.ReadAllLines(challengeInputPath));
    }

    private static void Solve2(string[] lines)
    {
        PrepareCave(lines);
        
        Console.WriteLine($"Total release with all valves open: {Valves.Sum(x=> x.Value.Rate*30)}");
        
        var maxScore = 0;
        var pathLenTerritory = 0;
        var queue = new ConcurrentQueue<DoubleMove>();
        
        queue.Enqueue(new DoubleMove("AA", "AA", 0, 26, 26, new HashSet<string>{ "AA" }));

        while (queue.Count > 0)
        {
            var nextQueue = new ConcurrentQueue<DoubleMove>();
            Parallel.ForEach(queue, m =>
            {
                //(string? currentLocation, int score, int remainingTime, HashSet<string> visited) = m;
                (string? currentLocation1, string? currentLocation2, int score, int remainingTime1, int remainingTime2, HashSet<string> visited) = m;
                
                if (visited.Count > pathLenTerritory)
                {
                    pathLenTerritory = visited.Count;
                    Console.WriteLine($"Visiting {pathLenTerritory} path length teritory!");
                }

                if (score > maxScore)
                {
                    maxScore = score;
                    Console.WriteLine(
                        $"Visited valves {String.Join(",", visited)}. Score: {score}, Rem. time: {remainingTime1}:{remainingTime2}");
                }
                
                foreach (string target1 in GoodValves)
                {
                    if(target1 == currentLocation1) continue;
                    if (visited.Contains(target1)) continue;
                    int targetFlow1 = Valves[target1].Rate;
                    if(targetFlow1 == 0) continue;
                    int dist1 = Distances[currentLocation1][target1];
                    
                    // foreach ((string target2, int dist2) in targets2)
                    foreach (string target2 in GoodValves)
                    {
                        if(target1 == target2) continue;
                        if(target2 == currentLocation2) continue;
                        if (visited.Contains(target2)) continue;
                        int targetFlow2 = Valves[target2].Rate;
                        if(targetFlow2 == 0) continue;
                        int dist2 = Distances[currentLocation2][target2];
                    
                        // walk there, then open
                        int remainTimeAfterWalkingAndOpening1 = remainingTime1 - dist1 - 1;
                        
                        // walk there, then open
                        int remainTimeAfterWalkingAndOpening2 = remainingTime2 - dist2 - 1;
                    
                        if (remainTimeAfterWalkingAndOpening1 <= 0 && remainTimeAfterWalkingAndOpening2 <= 0) continue;

                        if (remainTimeAfterWalkingAndOpening1 <= 0)
                        {
                            var nextScore = score + remainTimeAfterWalkingAndOpening2 * targetFlow2;
                            if(nextScore <= maxScore*0.9)
                                continue;
                            var nextMove = new DoubleMove(
                                currentLocation1,
                                target2,
                                nextScore,
                                remainingTime1,
                                remainTimeAfterWalkingAndOpening2,
                                visited.Append(target2).ToHashSet()
                            );
                            lock (nextQueue)
                            {
                                nextQueue.Enqueue(nextMove);
                            }
                        }
                        else if (remainTimeAfterWalkingAndOpening2 <= 0)
                        {
                            int nextScore = score + remainTimeAfterWalkingAndOpening1 * targetFlow1;
                            if(nextScore <= maxScore*0.9)
                                continue;
                            var nextMove = new DoubleMove(
                                target1,
                                currentLocation2,
                                nextScore,
                                remainTimeAfterWalkingAndOpening1,
                                remainingTime2,
                                visited.Append(target1).ToHashSet()
                            );
                            lock (nextQueue)
                            {
                                nextQueue.Enqueue(nextMove);
                            }
                        }
                        else
                        {
                            int nextScore = score + remainTimeAfterWalkingAndOpening1 * targetFlow1 +
                                                              remainTimeAfterWalkingAndOpening2 * targetFlow2;
                            if(nextScore <= maxScore*0.9)
                                continue;
                            var nextMove = new DoubleMove(
                                target1,
                                target2,
                                nextScore,
                                remainTimeAfterWalkingAndOpening1,
                                remainTimeAfterWalkingAndOpening2,
                                visited.Append(target1).Append(target2).ToHashSet()
                            );
                            lock (nextQueue)
                            {
                                nextQueue.Enqueue(nextMove);
                            }
                        }
                    }
                }
            });
            queue = nextQueue;
            Console.WriteLine($"Queue passed. Next: {nextQueue.Count} items.");
        }

        Console.WriteLine($"Done. Best score: {maxScore}");
        
    }

    private static void Solve(string[] lines)
    {
        PrepareCave(lines);

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

    private static void PrepareCave(string[] lines)
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
            if (valve.Rate > 0)
            {
                GoodValves.Add(name);
            }
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
    
    private record struct DoubleMove(
        string CurrentLocation1,
        string CurrentLocation2,
        int Score,
        int RemainingTime1,
        int RemainingTime2,
        HashSet<string> Visited
    );

    private class Valve
    {
        public string Name { get; init; } = null!;
        public int Rate { get; init; }
        public string[] Neighbors { get; init; } = null!;
    }
}