using Xunit;
using Xunit.Abstractions;

namespace Advent_of_Code._2024;

public class Day17
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day17.txt");
    private readonly string[] _testInput = File.ReadAllLines("Inputs/TestInputs/Test17.txt");
    private static ITestOutputHelper _output;
    private static int maxEquals = 1;

    public Day17(ITestOutputHelper output)
    {
        _output = output;
    }

    private class ProgramState
    {
        private readonly int[] _program;
        private int _instructionPointer;
        private long _regA;
        private long _regB;
        private long _regC;

        public ProgramState(long regA, long regB, long regC, int[] program)
        {
            _regA = regA;
            _regB = regB;
            _regC = regC;
            _program = program;
        }
        public ProgramState(string[] input)
        {
            _regA = int.Parse(input[0].Split(":", StringSplitOptions.TrimEntries)[1]);
            _regB = int.Parse(input[1].Split(":", StringSplitOptions.TrimEntries)[1]);
            _regC = int.Parse(input[2].Split(":", StringSplitOptions.TrimEntries)[1]);

            _program = input[4].Split(":")[1].Split(",").Select(int.Parse).ToArray();
        }

        private List<int> Output { get; } = new();

        public void Run(int[]? target = null)
        {
            long initialA = _regA;
            var loopCounter = 100000;
            while (_instructionPointer < _program.Length - 1 && loopCounter-- > 0)
            {
                if (loopCounter == 1)
                {
                    _output.WriteLine($"looped too many times at {initialA:n0}");
                    return;
                }
                int opCode = _program[_instructionPointer];
                int operand = _program[_instructionPointer + 1];
                switch (opCode)
                {
                    case 0: // adv
                        _regA = _regA / (int)Math.Pow(2, Combo(operand));
                        break;
                    case 1: // bxl
                        _regB = _regB ^ operand;
                        break;
                    case 2: // bst
                        _regB = Combo(operand) % 8;
                        break;
                    case 3: // jnz
                        if (_regA != 0)
                        {
                            _instructionPointer = operand - 2;
                        }
                        break;
                    case 4: // bxc
                        _regB = _regB ^ _regC;
                        break;
                    case 5: // out
                        Output.Add((int)(Combo(operand) % 8));
                        if(target != null && Output.Count > target.Length)
                        {
                            _output.WriteLine("too long: " + string.Join(",", Output));
                            return;
                        }
                        if(target != null && !Output.SequenceEqual(target[..Output.Count]))
                        {
                            // _output.WriteLine("not equal: " + string.Join(",", Output));
                            if(Output.Count > maxEquals)
                            {
                                _output.WriteLine("not equal: " + string.Join(",", Output) + $" at {initialA:n0}");
                                maxEquals = Output.Count;
                            }
                            return;
                        }
                        break;
                    case 6: // bdv
                        _regB = _regA / (int)Math.Pow(2, Combo(operand));
                        break;
                    case 7: // cdv
                        _regC = _regA / (int)Math.Pow(2, Combo(operand));
                        break;
                }
                _instructionPointer += 2;
            }

            return;

            long Combo(int operand)
            {
                if (operand <= 3)
                {
                    return operand;
                }

                return operand switch
                {
                    4 => _regA,
                    5 => _regB,
                    6 => _regC,
                    _ => throw new Exception("Invalid operand")
                };
            }
        }
        
        public string GetOutput()
        {
            Run();
            return string.Join(",", Output);
        }
        
        public int[] GetIntOutput(int[] target)
        {
            Run(target);
            return Output.ToArray();
        }
    }

    [Fact]
    public void Part1Test()
    {
        Assert.Equal("4,6,3,5,6,3,5,2,1,0", new ProgramState(_testInput).GetOutput());
    }
    
    [Fact]
    public void Part1()
    {
        Assert.Equal("7,4,2,5,1,4,6,0,4", new ProgramState(_input).GetOutput());
    }

    private static long UnCorruptProgram(string[] input)
    {
        int regB = int.Parse(input[1].Split(":", StringSplitOptions.TrimEntries)[1]);
        int regC = int.Parse(input[2].Split(":", StringSplitOptions.TrimEntries)[1]);
        int[] program = input[4].Split(":")[1].Split(",").Select(int.Parse).ToArray();
        long answer = -1;
        var stride = 10_000_000;
        long initial = 1 * (long)Math.Pow(8, program.Length - 1);
        long max = 8 * (long)Math.Pow(8, program.Length - 1);
        for(long i = initial; i < max; i += stride)
        {
            Parallel.For(i, i+stride, (a, state) =>
            {
                if (!program.SequenceEqual(new ProgramState(a, regB, regC, program).GetIntOutput(program)))
                {
                    return;
                }

                answer = a;
                state.Break();
            });
            if(answer != -1)
            {
                break;
            }
        }
        return answer;
    }
    
    [Fact]
    public void Part2Test()
    {
        _testInput[4] = "Program: 0,3,5,4,3,0";
        Assert.Equal(117440, UnCorruptProgram(_testInput));
    }
    
    [Fact]
    public void Part2()
    {
        //Assert.Equal(0, UnCorruptProgram(_input));
        int[] program = _input[4].Split(":")[1].Split(",").Select(int.Parse).ToArray();
        var ans = ReverseEngineerProgram(program);
        Assert.Equal(164278764924605, ans);
    }
    
    private long ReverseEngineerProgram(int[] program)
    {
        var traversals = new Queue<long>();
        
        for(int i = 0; i < 8; i++)
        {
            traversals.Enqueue(i);
        }
        
        var possibleFinishes = new List<long>();
        
        while(traversals.Count > 0)
        {
            long current = traversals.Dequeue();
            int[] output = new ProgramState(current, 0, 0, program).GetIntOutput(null!);
            
            if (output.Length == program.Length && output.SequenceEqual(program))
            {
                possibleFinishes.Add(current);
                continue;
            }
            
            var targetOutput = program[^output.Length..];
            if(output.SequenceEqual(targetOutput))
            {
                //_output.WriteLine($"found: {current:n0}");
                for(int i = 0; i < 8; i++)
                {
                    traversals.Enqueue(current * 8 + i);
                }
                _output.WriteLine($"found: {current:n0}, test: {string.Join(',', output)}");
                continue;
            }
        }
        
        return possibleFinishes.Min();
    }
}