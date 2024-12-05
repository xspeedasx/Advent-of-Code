using Xunit;
using Xunit.Abstractions;

namespace Advent_of_Code._2024;

public class Day05(ITestOutputHelper output)
{
    private record ManualData
    {
        public List<(int, int)> Rules { get; set; } = new();
        public List<List<int>> Manuals { get; set; } = new();
    }
    
    private readonly ManualData _input = ParseManualData(File.ReadAllLines("Inputs/Day05.txt"));
    private readonly ManualData _testInput = ParseManualData(File.ReadAllLines("Inputs/TestInputs/Test05.txt"));

    private bool IsManualValid(List<(int, int)> rules, List<int> pages)
    {
        var pageIndexes = new Dictionary<int, int>();
        for (var i = 0; i < pages.Count; i++)
        {
            pageIndexes[pages[i]] = i;
        }

        var valid = true;
        foreach ((int a, int b) in rules)
        {
            if(!pageIndexes.ContainsKey(a) || !pageIndexes.ContainsKey(b))
                continue;
            
            var indexA = pageIndexes[a];
            var indexB = pageIndexes[b];
            if(indexA > indexB)
            {
                valid = false;
                break;
            }
        }
        return valid;
    }

    private int CalculateMiddlePagesOfValidManuals(ManualData manualData)
    {
        var sum = 0;
        foreach (var manualPages in manualData.Manuals)
        {
            if(IsManualValid(manualData.Rules, manualPages))
            {
                var middle = manualPages.Count / 2;
                sum += manualPages[middle];
            }
        }

        return sum;
    }

    [Fact]
    public void Part1Test()
    {
        Assert.Equal(143, CalculateMiddlePagesOfValidManuals(_testInput));
    }
    
    [Fact]
    public void Part1()
    {
        Assert.Equal(6051, CalculateMiddlePagesOfValidManuals(_input));
    }

    private List<int> ReorderManualPages(List<int> pages, List<(int, int)> rules)
    {
        var actualRules = rules.Where(r => pages.Contains(r.Item1) && pages.Contains(r.Item2)).ToList();
        
        // this was a shot in the dark, but it worked :D thanks for the creators being so dilligent with their inputs
        var appearances = new Dictionary<int, int>();
        foreach (var page in pages)
        {
            appearances[page] = 0;
        }
        foreach (var (a,b) in actualRules)
        {
            appearances[a]++;
        }
        
        return appearances.OrderByDescending(x => x.Value).Select(x => x.Key).ToList();
    }
    private int CalculateMiddlePagesOfReorderedManuals(ManualData manualData)
    {
        var sum = 0;
        var cnt = 0;
        foreach (var manualPages in manualData.Manuals)
        {
            output.WriteLine($"Processing manual {++cnt} / {manualData.Manuals.Count}");
            if(IsManualValid(manualData.Rules, manualPages))
            {
                output.WriteLine("Skipping valid manual");
                continue;
            }
            
            var ordered = ReorderManualPages(manualPages, manualData.Rules);
            if (ordered.Count != manualPages.Count)
            {
                throw new Exception("Invalid count");
            }
            var middle = ordered.Count / 2;
            sum += ordered[middle];
        }

        return sum;
    }

    [Fact]
    public void Part2Test1()
    {
        Assert.Equal(new[]{97,75,47,61,53}, ReorderManualPages(_testInput.Manuals[3], _testInput.Rules));;
    }
    
    [Fact]
    public void Part2TestFull()
    {
        Assert.Equal(123, CalculateMiddlePagesOfReorderedManuals(_testInput));
    }
    
    [Fact]
    public void Part2()
    {
        Assert.Equal(5093, CalculateMiddlePagesOfReorderedManuals(_input));
    }

    private static ManualData ParseManualData(string[] lines)
    {
        var manualData = new ManualData();
        var isRule = true;
        foreach (var line in lines)
        {
            if (line == "")
            {
                isRule = false;
                continue;
            }

            if (isRule)
            {
                var parts = line.Split("|", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                manualData.Rules.Add((int.Parse(parts[0]), int.Parse(parts[1])));
            }
            else
            {
                manualData.Manuals.Add(line.Split(",", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                    .Select(int.Parse).ToList());
            }
        }

        return manualData;
    }
}