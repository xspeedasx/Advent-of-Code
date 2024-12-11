using System.Collections.Concurrent;
using Xunit;
using Xunit.Abstractions;

namespace Advent_of_Code._2024;

public class Day11(ITestOutputHelper output)
{
    private readonly string _input = File.ReadAllText(@"Inputs\Day11.txt");

    private IEnumerable<long> BlinkList(IEnumerable<long> stones)
    {
        var newStones = new List<long>();
        foreach (var stone in stones)
        {
            (long a, long? b) = BlinkStone(stone);
            newStones.Add(a);
            if (b.HasValue)
            {
                newStones.Add(b.Value);
            }
        }
        return newStones;
    }

    private static (long, long?) BlinkStone(long stone)
    {
        if(stone == 0)
        {
            return (1, null);
        }
        var digits = stone.ToString();
        if(digits.Length % 2 == 0)
        {
            return (
                long.Parse(digits[..(digits.Length / 2)]),
                long.Parse(digits[(digits.Length / 2)..])
            );
        }
        return (stone * 2024, null);
    }

    private int CountOfStonesAfterBlinking(IEnumerable<long> stones, int count)
    {
        for (var i = 0; i < count; i++)
        {
            stones = BlinkList(stones);
            // output.WriteLine($"[{DateTime.Now:s}] Completed {i + 1} blinks, count: {stones.Count()}");
        }
        return stones.Count();
    }
    
    private IEnumerable<long> ParseInput(string input)
    {
        return input
            .Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(long.Parse);
    }
    
    [Fact]
    public void Part1Test1()
    {
        var stones = ParseInput("0 1 10 99 999");
        Assert.Equal(7, CountOfStonesAfterBlinking(stones, 1));
    }
    
    [Fact]
    public void Part1Test2()
    {
        var stones = ParseInput("125 17");
        IEnumerable<long> enumerable = stones as long[] ?? stones.ToArray();
        Assert.Equal(22, CountOfStonesAfterBlinking(enumerable, 6));
        Assert.Equal(55312, CountOfStonesAfterBlinking(enumerable, 25)); 
    }
    
    [Fact]
    public void Part1()
    {
        var stones = ParseInput(_input);
        Assert.Equal(217812, CountOfStonesAfterBlinking(stones, 25));
    }
    
    private long CountOfStonesAfterBlinkingFaster(IEnumerable<long> stones, int maxBlinkCount)
    {
        long totalSum = 0; 
        ConcurrentBag<(long stone, int blinkCount)> bagOfStones = new();
        
        foreach (var stone in stones)
        {
            bagOfStones.Add((stone, 0));
        }
        
        while (bagOfStones.Count > 0)
        {
            var handfulOfStones = new List<(long stone, int blinkCount)>();
            while (handfulOfStones.Count < 1000 && bagOfStones.TryTake(out var stone))
            {
                handfulOfStones.Add(stone);
            }
            
            Parallel.ForEach(handfulOfStones, stone =>
            // foreach (var stone in handfulOfStones)
            {
                if (stone.blinkCount == maxBlinkCount)
                {
                    // continue;
                    return;
                }
                (long a, long? b) = BlinkStone(stone.stone);
                
                if(stone.blinkCount == maxBlinkCount - 1)
                {
                    // totalSum++;
                    Interlocked.Increment(ref totalSum);
                    if (b.HasValue)
                    {
                        // totalSum++;
                        
                        Interlocked.Increment(ref totalSum);
                    }
                    // continue;
                    return;
                }
                
                bagOfStones.Add((a, stone.blinkCount + 1));
                if (b.HasValue)
                {
                    bagOfStones.Add((b.Value, stone.blinkCount + 1));
                }
            }
            );
        }
        return totalSum;
    }
    
    [Fact]
    public void Part1Test1Faster()
    {
        var stones = ParseInput("0 1 10 99 999");
        Assert.Equal(7, CountOfStonesAfterBlinkingFaster(stones, 1));
    }
    
    [Fact]
    public void Part1Test2Faster()
    {
        var stones = ParseInput("125 17");
        IEnumerable<long> enumerable = stones as long[] ?? stones.ToArray();
        // Assert.Equal(22, CountOfStonesAfterBlinkingFaster(enumerable, 6));
        // Assert.Equal(55312, CountOfStonesAfterBlinkingFaster(enumerable, 25));
        
        for (var i = 1; i <= 25; i++)
        {
            int countOfStonesAfterBlinking = CountOfStonesAfterBlinking(enumerable, i);
            output.WriteLine($"after {i} blinks: {countOfStonesAfterBlinking}");
            Assert.Equal(countOfStonesAfterBlinking, CountOfStonesAfterBlinkingFaster(enumerable, i));
        }
    }

    [Fact]
    public void Part1Faster()
    {
        var stones = ParseInput(_input);
        Assert.Equal(217812, CountOfStonesAfterBlinkingFaster(stones, 25));
    }
    
    private long CountOfStonesDFS(IEnumerable<long> stones, int depth)
    {
        var cache = new Dictionary<(long, int), long>();
        return stones.Sum(stone => CountBlink(stone, depth));
        
        long CountBlink(long stone, int depth)
        {
            if(cache.TryGetValue((stone, depth), out var count))
            {
                return count;
            }
            (long a, long? b) = BlinkStone(stone);
            if (depth == 1)
            {
                return 1 + (b.HasValue ? 1 : 0);
            }
            cache[(stone, depth)] = CountBlink(a, depth - 1) + (b.HasValue ? CountBlink(b.Value, depth - 1) : 0);
            return CountBlink(a, depth - 1) + (b.HasValue ? CountBlink(b.Value, depth - 1) : 0);
        }
    }
    
    [Fact]
    public void Part1Test1DFS()
    {
        var stones = ParseInput("0 1 10 99 999");
        Assert.Equal(7, CountOfStonesDFS(stones, 1));
    }
    
    [Fact]
    public void Part1Test2DFS()
    {
        var stones = ParseInput("125 17");
        IEnumerable<long> enumerable = stones as long[] ?? stones.ToArray();
        Assert.Equal(22, CountOfStonesDFS(enumerable, 6));
        Assert.Equal(55312, CountOfStonesDFS(enumerable, 25));
    }
    
    [Fact]
    public void Part1DFS()
    {
        var stones = ParseInput(_input);
        Assert.Equal(217812, CountOfStonesDFS(stones, 25));
    }
    
    [Fact]
    public void Part2DFS()
    {
        var stones = ParseInput(_input);
        Assert.Equal(259112729857522, CountOfStonesDFS(stones, 75));
    }
}