using Xunit;
using Xunit.Abstractions;

namespace Advent_of_Code._2024;

public class Day23(ITestOutputHelper output)
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day23.txt");
    private readonly string[] _testInput = File.ReadAllLines("Inputs/TestInputs/Test23.txt");

    private int FindSetsWithT(string[] input)
    {
        Dictionary<string, List<string>> dict = MakeAddressBook(input);

        var sets = new HashSet<string>();
        
        foreach ((string computer, List<string> neighbors) in dict)
        {
            foreach (string a in neighbors)
            {
                List<string> aNeighbors = dict[a];
                foreach (string b in aNeighbors)
                {
                    List<string> bNeighbors = dict[b];
                    if(bNeighbors.Contains(computer))
                    {
                        if(a[0] != 't' && b[0] != 't' && computer[0] != 't')
                            continue;
                        sets.Add(string.Join(",", new[] { computer, a, b }.OrderBy(x => x)));
                    }
                }
            }
        }
        output.WriteLine(string.Join("\n", sets));
        return sets.Count;
    }

    private static Dictionary<string, List<string>> MakeAddressBook(string[] input)
    {
        var dict = new Dictionary<string, List<string>>();
        foreach (string line in input)
        {
            string[] parts = line.Split("-");
            string a = parts[0];
            string b = parts[1];
            dict.TryAdd(a, new List<string>());
            dict[a].Add(b);
            dict.TryAdd(b, new List<string>());
            dict[b].Add(a);
        }

        return dict;
    }

    [Fact]
    void Part1Test()
    {
        Assert.Equal(7, FindSetsWithT(_testInput));
    }
    
    [Fact]
    void Part1()
    {
        Assert.Equal(1330, FindSetsWithT(_input));
    }

    private string GetLANPartyPassword(string[] input)
    {
        var dict = MakeAddressBook(input);
        var parties = new HashSet<string>();
        
        foreach ((string computer, List<string> neighbors) in dict)
        {
            foreach (string a in neighbors)
            {
                var commonNeighbors = dict[a].Intersect(neighbors).ToList();
                if (commonNeighbors.Count > 0)
                {
                    parties.Add(string.Join(",", new[] { computer, a }.Concat(commonNeighbors).OrderBy(x => x)));
                } 
            }
        }

        parties = parties.Where(party =>
        {
            var computers = party.Split(",");
            return computers.All(computer =>
            {
                var neighbors = dict[computer];
                return computers.Except([computer]).All(neighbors.Contains);
            });
        }).ToHashSet();
        
        if(parties.Count == 0)
            return "No LAN party";
        return parties.OrderByDescending(x => x.Length).First();
    }
    
    [Fact]
    void Part2Test()
    {
        Assert.Equal("co,de,ka,ta", GetLANPartyPassword(_testInput));
    }
    
    [Fact]
    void Part2()
    {
        Assert.Equal("hl,io,ku,pk,ps,qq,sh,tx,ty,wq,xi,xj,yp", GetLANPartyPassword(_input));
    }
}