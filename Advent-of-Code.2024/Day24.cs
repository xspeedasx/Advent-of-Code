using Xunit;

namespace Advent_of_Code._2024;

public class Day24
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day24.txt");
    private readonly string[] _testInput = File.ReadAllLines("Inputs/TestInputs/Test24.txt");

    enum GateType
    {
        AND,
        OR,
        XOR
    }

    private class Gate
    {
        public GateType Type { get; set; }
        public string Input1 { get; set; }
        public string Input2 { get; set; }
        public string Output { get; set; }

        public Gate(string input)
        {
            string[] splits = input.Split(" ");
            Input1 = splits[0];
            Input2 = splits[2];
            Output = splits[4];
            Type = splits[1] switch
            {
                "AND" => GateType.AND,
                "OR" => GateType.OR,
                "XOR" => GateType.XOR,
                _ => throw new Exception("Invalid gate type")
            };
        }
    }

    private class GateResolver
    {
        public Dictionary<string, bool> Outputs = new();
        public List<Gate> Gates = [];
        
        public GateResolver(string[] input)
        {
            var isOutput = true;
            foreach (string line in input)
            {
                if (line == "")
                {
                    isOutput = false;
                    continue;
                }

                if (isOutput)
                {
                    string[] parts = line.Split(':', StringSplitOptions.TrimEntries);
                    Outputs[parts[0]] = parts[1] == "1";
                    continue;
                }
                
                Gates.Add(new Gate(line));
            }
        }

        public bool GetOutput(string outputName)
        {
            if (Outputs.TryGetValue(outputName, out bool output))
                return output;
            
            Gate gate = Gates.First(g => g.Output == outputName);

            bool input1 = GetOutput(gate.Input1);
            bool input2 = GetOutput(gate.Input2);
            Outputs[gate.Output] = gate.Type switch
            {
                GateType.AND => input1 & input2,
                GateType.OR => input1 | input2,
                GateType.XOR => input1 ^ input2,
                _ => throw new Exception("Invalid gate type")
            };
            return Outputs[gate.Output];
        }
    }
    
    public long SimulateGates(string[] input)
    {
        var resolver = new GateResolver(input);
        IOrderedEnumerable<Gate> zGates = resolver.Gates
            .Where(g => g.Output.StartsWith('z'))
            .OrderBy(g => g.Output);

        var sum = 0L;
        foreach (Gate zGate in zGates)
        {
            int shift = int.Parse(zGate.Output[1..]);
            sum += (long)(resolver.GetOutput(zGate.Output) ? 1 : 0) << shift;
        }

        return sum;
    }
    
    [Fact]
    void Part1Test()
    {
        Assert.Equal(2024, SimulateGates(_testInput));
    }
    
    [Fact]
    void Part1()
    {
        Assert.Equal(38869984335432, SimulateGates(_input));
    }
}