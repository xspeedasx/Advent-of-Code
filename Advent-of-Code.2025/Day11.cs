using Xunit;
using Xunit.Abstractions;

namespace Advent_of_Code._2025;

public class Day11(ITestOutputHelper output)
{
    private string TestInput =
        """
        aaa: you hhh
        you: bbb ccc
        bbb: ddd eee
        ccc: ddd eee fff
        ddd: ggg
        eee: out
        fff: out
        ggg: out
        hhh: ccc fff iii
        iii: out
        """;

    private string TestInput2 =
        """
        svr: aaa bbb
        aaa: fft
        fft: ccc
        bbb: tty
        tty: ccc
        ccc: ddd eee
        ddd: hub
        hub: fff
        eee: dac
        dac: fff
        fff: ggg hhh
        ggg: out
        hhh: out
        """;
    
    private readonly string[] _input = File.ReadAllLines(@"Inputs\Day11.txt");
    
    [Fact]
    public void Part1Test()
    {
        int ans = CalculatePathsFromYou(TestInput.Split("\n",
            StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries));
        Assert.Equal(5, ans);
    }
    
    [Fact]
    public void Part1()
    {
        int ans = CalculatePathsFromYou(_input);
        Assert.Equal(523, ans);
    }

    private class PathState
    {
        public string CurrentDeviceLabel { get; set; } = string.Empty;
        public HashSet<string> VisitedDevices { get; set; } = new();
    }
    
    private int CalculatePathsFromYou(string[] input)
    {
        Dictionary<string, Device> devices = ParseDevices(input);
        Device you = devices.First(x => x.Key == "you").Value;
        var uniquePaths = new HashSet<string>();
        var queue = new Queue<PathState>();
        queue.Enqueue(new PathState
        {
            CurrentDeviceLabel = you.Label,
            VisitedDevices = new HashSet<string> { you.Label }
        });

        while (queue.Count > 0)
        {
            PathState state = queue.Dequeue();
            Device currentDevice = devices[state.CurrentDeviceLabel];
            foreach (string outputLabel in currentDevice.Outputs)
            {
                if (outputLabel == "out")
                {
                    uniquePaths.Add(string.Join("->", state.VisitedDevices) + "->out");
                    continue;
                }

                if (state.VisitedDevices.Contains(outputLabel))
                {
                    continue;
                }

                var newVisited = new HashSet<string>(state.VisitedDevices) { outputLabel };
                queue.Enqueue(new PathState
                {
                    CurrentDeviceLabel = outputLabel,
                    VisitedDevices = newVisited
                });
            }
        }
        return uniquePaths.Count;
    }
    
    [Fact]
    public void Part2Test()
    {
        int ans = CalculatePathsViaFftDac(TestInput2.Split("\n",
            StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries));
        Assert.Equal(2, ans);
    }
    
    [Fact]
    public void Part2()
    {
        int ans = CalculatePathsViaFftDac(_input);
        Assert.Equal(437, ans);
    }
    
    private class PathStateFftDac
    {
        public string CurrentDeviceLabel { get; set; } = string.Empty;
        public HashSet<string> VisitedDevices { get; set; } = new();
        public bool FftVisited { get; set; } = false;
        public bool DacVisited { get; set; } = false;
    }
    
    private int CalculatePathsViaFftDac(string[] input)
    {
        Dictionary<string, Device> devices = ParseDevices(input);
        Device svr = devices.First(x => x.Key == "svr").Value;
        var knownPaths = new Dictionary<string, int>();

        int cnt = Traverse(svr, new HashSet<string>());
        return cnt;

        int Traverse(Device current, HashSet<string> visited)
        {
            var devCnt = 0;
            foreach (string outputLabel in current.Outputs)
            {
                if(outputLabel == "out")
                {
                    devCnt += visited.Contains("fft") && visited.Contains("dac") ? 1 : 0;
                    continue;
                }

                if (knownPaths.TryGetValue(outputLabel, out int knownCnt))
                {
                    devCnt += knownCnt;
                    continue;
                }
                if (visited.Contains(outputLabel))
                {
                    continue;
                }
                devCnt += Traverse(devices[outputLabel], [..visited, outputLabel]);
            }
            knownPaths[current.Label] = devCnt;
            return devCnt;
        }
    }

    private Dictionary<string, Device> ParseDevices(string[] input)
    {
        var devices = new Dictionary<string, Device>();
        
        foreach (string line in input)
        {
            string[] parts = line.Split(':', StringSplitOptions.TrimEntries);
            if (parts.Length != 2)
                continue;
                
            string label = parts[0].Trim();
            List<string> outputs = parts[1]
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList();
            
            devices[label] = new Device
            {
                Label = label,
                Outputs = outputs
            };
        }
        
        return devices;
    }
    
    private class Device
    {
        public string Label { get; set; } = string.Empty;
        public List<string> Outputs { get; set; } = new();
    }
}
