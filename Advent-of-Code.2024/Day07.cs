using Xunit;

namespace Advent_of_Code._2024;

public class Day07
{
    private readonly string[] _testInput = File.ReadAllLines("Inputs/TestInputs/Test07.txt");
    private readonly string[] _input = File.ReadAllLines("Inputs/Day07.txt");
    
    [Fact]
    public void TestIfLongsAndIntsAreUsed()
    {
        foreach (string line in _input)
        {
            string[] parts = line.Split(": ");
            Assert.Equal(long.Parse(parts[0]).ToString(), parts[0]);
            
            string[] numbers = parts[1]
                .Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (string number in numbers)
            {
                Assert.Equal(int.Parse(number).ToString(), number);
            }
        }
    }

    private static long ValidateLine(string line, bool useConcatenation = false)
    {
        string[] parts = line.Split(": ");
        long total = long.Parse(parts[0]);
        int[] numbers = parts[1]
            .Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(int.Parse)
            .ToArray();

        var queue = new Queue<CurrentTotal>();
        queue.Enqueue(new CurrentTotal(numbers[0], 0));
        
        while(queue.Count > 0)
        {
            CurrentTotal current = queue.Dequeue();
            if(current.Total == total && current.Index == numbers.Length - 1) return total;
            if(current.Total > total) continue;
            
            if(current.Index == numbers.Length - 1) continue;
            queue.Enqueue(new CurrentTotal(current.Total + numbers[current.Index + 1], current.Index + 1));
            queue.Enqueue(new CurrentTotal(current.Total * numbers[current.Index + 1], current.Index + 1));
            if(useConcatenation)
            {
                long concatenated = long.Parse(current.Total.ToString() + numbers[current.Index + 1]);
                queue.Enqueue(new CurrentTotal(concatenated, current.Index + 1));
            }
        }
        
        return 0;
    }
    
    record struct CurrentTotal(long Total, int Index);
    
    [Fact]
    public void Part1Test()
    {
        Assert.Equal(3749, _testInput.Sum(x => ValidateLine(x)));
    }
    
    [Fact]
    public void Part1()
    {
        Assert.Equal(12553187650171, _input.Sum(x => ValidateLine(x)));
    }
    
    [Fact]
    public void Part2Test()
    {
        Assert.Equal(11387, _testInput.Sum(x => ValidateLine(x, true)));
    }
    
    [Fact]
    public void Part2()
    {
        Assert.Equal(96779702119491, _input.Sum(x => ValidateLine(x, true)));
    }
}