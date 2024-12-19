using System.Collections.Concurrent;
using Xunit;
using Xunit.Abstractions;

namespace Advent_of_Code._2024;

public class Day19
{
    private static ITestOutputHelper? _output;
    private readonly string[] _input = File.ReadAllLines("Inputs/Day19.txt");
    private readonly string[] _testInput = File.ReadAllLines("Inputs/TestInputs/Test19.txt");

    public Day19(ITestOutputHelper output)
    {
        _output = output;
    }
    private static int CountPossible(string[] input)
    {
        string[] availablePatterns = input[0].Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return input[2..].Count(x => CheckPossible2(availablePatterns, x));
    }

    private static bool CheckPossible2(string[] availablePatterns, string result)
    {
        var set = new HashSet<string>(availablePatterns);
        var minLen = availablePatterns.Min(x => x.Length);
        var maxLen = availablePatterns.Max(x => x.Length);
        var alreadyProcessedIndexes = new HashSet<int>();
        var indexes = new HashSet<int> {0};
        
        while (indexes.Count > 0)
        {
            var nextIndexes = new HashSet<int>();
            foreach (int index in indexes)
            {
                if(alreadyProcessedIndexes.Contains(index))
                {
                    continue;
                }
                
                for(int i = minLen; i <= maxLen; i++)
                {
                    if (index + i > result.Length)
                    {
                        break;
                    }
                    var subString = result[index..(index + i)];
                    if (set.Contains(subString))
                    {
                        if (index + i == result.Length)
                        {
                            return true;
                        }
                        nextIndexes.Add(index + i);
                    }
                }
                alreadyProcessedIndexes.Add(index);
            }

            indexes = nextIndexes;
        }
        return false;
    }
    
    private static bool CheckPossible(string[] availablePatterns, string result)
    {
        var queue = new List<string>();
        foreach (string pattern in availablePatterns)
        {
            if(result.StartsWith(pattern))
            {
                queue.Add(pattern);
            }
        }

        var found = 0;
        
        while (queue.Count > 0 && found == 0)
        {
            var nextList = new ConcurrentBag<string>();

            Parallel.ForEach(queue, (currentPattern, state) =>
            {
                if (currentPattern == result)
                {
                    Interlocked.Increment(ref found);
                }

                if (currentPattern != result[..currentPattern.Length])
                {
                    return;
                }

                if (currentPattern.Length > result.Length)
                {
                    return;
                }

                foreach (string pattern in availablePatterns)
                {
                    string nextPattern = currentPattern + pattern;
                    if (nextPattern == result)
                    {
                        Interlocked.Increment(ref found);
                        state.Break();
                        break;
                    }
                    if (result.StartsWith(nextPattern) && nextPattern.Length <= result.Length)
                    {
                        nextList.Add(nextPattern);
                    }
                }
            });
            
            queue = nextList.ToList();
        }
        _output!.WriteLine($"Pattern possible? {result}: {found > 0}");;
        return found > 0;
    }
    
    [Fact]
    public void Part1Test()
    {
        Assert.Equal(6, CountPossible(_testInput));
    }
    
    [Fact]
    public void Part1()
    {
        Assert.Equal(209, CountPossible(_input));
    }

    private static long CountPermutations(string[] input)
    {
        string[] availablePatterns = input[0].Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        
        var possible = input[2..].Where(x => CheckPossible2(availablePatterns, x)).ToList();
        long sum = 0;
        foreach (string s in possible)
        {
            long permutations = GetPermutations(availablePatterns, s);
            _output!.WriteLine($"Permutations for {s}: {permutations}");
            sum += permutations;
        }
        return sum;
    }

    private static long GetPermutations(string[] availablePatterns, string s)
    {
        var set = new HashSet<string>(availablePatterns);
        var minLen = availablePatterns.Min(x => x.Length);
        var maxLen = availablePatterns.Max(x => x.Length);
        long finishCounts = 0;
        var indexes = new List<(int key, long multiplier)> {(0, 1)};

        while (indexes.Count > 0)
        {
            var nextIndexes = new ConcurrentBag<(int key, long multiplier)>();
            // foreach (int index in indexes)
            var indArr = indexes.GroupBy(x => x.key).Select(x => (x.Key, x.Sum(c => c.multiplier))).ToArray();
            _output!.WriteLine($"Indexes count: {indArr.Length}");
            Parallel.ForEach(indArr, indMult =>
            {
                var index = indMult.Key;
                var multiplier = indMult.Item2;
                for (int i = minLen; i <= maxLen; i++)
                {
                    int nextI = index + i;
                    if (nextI > s.Length)
                    {
                        break;
                    }

                    var subString = s[index..nextI];

                    bool setContains = false;
                    lock(set)
                    {
                        setContains = set.Contains(subString);
                    }
                    if (setContains)
                    {
                        if (nextI == s.Length)
                        {
                            // finishCounts++;
                            Interlocked.Add(ref finishCounts, multiplier);
                            break;
                        }

                        nextIndexes.Add((nextI, multiplier));
                    }
                }
            });
            indexes = nextIndexes.ToList();
        }

        return finishCounts;
    }
    
    [Fact]
    public void Part2Test()
    {
        Assert.Equal(16, CountPermutations(_testInput));
    }
    
    [Fact]
    public void Part2()
    {
        Assert.Equal(777669668613191, CountPermutations(_input));
    }
}