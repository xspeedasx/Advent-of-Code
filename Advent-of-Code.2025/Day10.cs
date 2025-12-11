using System.Drawing;
using System.Numerics;
using System.Text.RegularExpressions;
using Xunit;
using Xunit.Abstractions;

namespace Advent_of_Code._2025;

public class Day10(ITestOutputHelper output)
{
    private string TestInput =
        """
        [.##.] (3) (1,3) (2) (2,3) (0,2) (0,1) {3,5,4,7}
        [...#.] (0,2,3,4) (2,3) (0,4) (0,1,2) (1,2,3,4) {7,5,12,7,2}
        [.###.#] (0,1,2,3,4) (0,3,4) (0,1,2,4,5) (1,2) {10,11,11,5,10,5}
        """;
    private readonly string[] _input = File.ReadAllLines(@"Inputs\Day10.txt");
    
    [Fact]
    public void Part1Test()
    {
        long ans = FindAllFewestLightPresses(TestInput.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries));
        Assert.Equal(7, ans);
    }
    
    [Fact]
    public void Part1()
    {
        long ans = FindAllFewestLightPresses(_input);
        Assert.Equal(477, ans);
    }

    private long FindAllFewestLightPresses(string[] input)
    {
        var machines = input.Select(ParseMachine).ToList();
        return machines.Select(GetFewestLightPresses).Sum();
    }

    private record MachineState(bool[] Lights, List<int> ButtonsPressed);
    
    private int GetFewestLightPresses(Machine machine, int machineIdx)
    {
        var queue = new Queue<MachineState>();
        queue.Enqueue(new MachineState(new bool[machine.LightCount], []));

        while (queue.Count > 0)
        {
            var state = queue.Dequeue();
            if(state.ButtonsPressed.Count >= 10)
                continue;
            
            for (var i = 0; i < machine.Buttons.Length; i++)
            {
                var stateLights = state.Lights.ToArray();
                var stateButtonsPressed = state.ButtonsPressed.ToList();
                
                if(state.ButtonsPressed.Count > 0 && state.ButtonsPressed[^1] == i)
                    continue;
                foreach (int j in machine.Buttons[i])
                {
                    stateLights[j] = !stateLights[j];
                }
                stateButtonsPressed.Add(i);
                if (stateLights.SequenceEqual(machine.LightTarget))
                {
                    return stateButtonsPressed.Count;
                }
                queue.Enqueue(new MachineState(stateLights, stateButtonsPressed));
            }
        }

        return 0;
    }

    private static Regex LightRegex = new(@"\[(?<lights>[.#]+)\]");
    private static Regex ButtonRegex = new(@"\((?<button>[\d,]+)\)");
    private static Regex JoltageRegex = new(@"\{(?<joltages>[\d,]+)\}");
    
    private Machine ParseMachine(string arg)
    {
        var machine = new Machine();
        var lightMatch = LightRegex.Match(arg);
        machine.LightTarget = lightMatch.Groups["lights"].Value
            .Select(c => c == '#').ToArray();
        machine.LightCount = machine.LightTarget.Length;
        var buttonMatches = ButtonRegex.Matches(arg);
        machine.Buttons = buttonMatches.Select(m =>
            m.Groups["button"].Value.Split(',', StringSplitOptions.TrimEntries)
                .Select(int.Parse).ToArray()).ToArray();
        var joltageMatch = JoltageRegex.Match(arg);
        machine.JoltTarget = joltageMatch.Groups["joltages"].Value
            .Split(',', StringSplitOptions.TrimEntries)
            .Select(int.Parse).ToArray();
        return machine;
    }

    [Fact]
    public void Part2Test()
    {
        long ans = FindAllFewestJoltPresses(TestInput.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries));
        Assert.Equal(33, ans);
    }

    [Fact]
    public void Part2()
    {
        long ans = FindAllFewestJoltPresses(_input);
        Assert.Equal(1571016172, ans);
    }
    
    private long FindAllFewestJoltPresses(string[] input)
    {
        var machines = input.Select(ParseMachine).ToList();
        return machines.Select(GetFewestJoltPresses).Sum();
    }

    private record MachineJoltState(byte[] Joltage, int ButtonsPressed);
    
    private int GetFewestJoltPresses(Machine machine, int machineIdx)
    {
        output.WriteLine($"Processing machine {machineIdx}...");
        var states = new List<MachineJoltState>{ new(new byte[machine.JoltTarget.Length], 0) };
        var previousStates = new HashSet<string>();
        var answer = -1;

        while (answer == -1 && states.Count > 0)
        {
            var newStates = new List<MachineJoltState>();
            output.WriteLine($"  States to process: {states.Count}, previous unique states: {previousStates.Count}");

            Parallel.ForEach(states, state =>
            {
                foreach (int[] button in machine.Buttons)
                {
                    byte[] stateJolts = state.Joltage.ToArray();
                    int stateButtonsPressed = state.ButtonsPressed;
                    
                    foreach (int j in button)
                    {
                        stateJolts[j]++;
                    }
                    stateButtonsPressed++;
                    
                    bool matchesTarget = true;
                    for (int i = 0; i < stateJolts.Length; i++)
                    {
                        if (stateJolts[i] != machine.JoltTarget[i])
                            matchesTarget = false;
                    }
                    if (matchesTarget)
                    {
                        answer = stateButtonsPressed;
                        return;
                    }
                    
                    if(!ValidJoltage(state.Joltage, machine.JoltTarget))
                        continue;

                    lock (previousStates)
                    {
                        if (!previousStates.Add(string.Join(",", stateJolts)))
                        {
                            continue;
                        }
                    }
                    
                    lock (newStates)
                    {
                        newStates.Add(new MachineJoltState(stateJolts, stateButtonsPressed));
                    }
                }
            });
            states = newStates.DistinctBy(s => string.Join(",", s.Joltage)).ToList();
            continue;

            bool ValidJoltage(byte[] stateJoltage, int[] machineJoltTarget)
            {
                for (int i = 0; i < stateJoltage.Length; i++)
                {
                    if (stateJoltage[i] > machineJoltTarget[i])
                        return false;
                }
                return true;
            }
        }

        return answer;
    }

    private class Machine
    {
        public bool[] LightTarget { get; set; }
        public int LightCount { get; set; }
        public int[][] Buttons { get; set; }
        public int[] JoltTarget { get; set; }
    }
}
