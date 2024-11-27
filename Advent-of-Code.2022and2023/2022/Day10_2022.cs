namespace Advent_of_Code._2022;

public static class Day10_2022
{
    public static void Run(string testInputPath, string challengeInputPath)
    {
        // Part 1
        // Solve(File.ReadAllLines(testInputPath), 1);
        // Solve(File.ReadAllLines(challengeInputPath), 1);
        
        // Part 2
        //Solve(File.ReadAllLines(testInputPath), 2);
        Solve(File.ReadAllLines(challengeInputPath), 2);
    }

    private static void Solve(string[] lines, int part)
    {
        switch (part)
        {
            case 1:
                MeasureSignalStrengths(lines);
                break;
            case 2:
                DrawPixels(lines);
                break;
        }
        
    }

    private static void DrawPixels(string[] lines)
    {
        var cpu = new Processor(lines);

        var pixelPos = 0;

        void Action(int regX, int ci)
        {
            var dist = Math.Abs(pixelPos - regX);
            Console.Write(dist < 2 ? '#' : '.');
            pixelPos++;
            if (pixelPos == 40)
            {
                Console.WriteLine();
                pixelPos = 0;
            }
        }

        while (cpu.Clock(Action)){}
    }

    private static void MeasureSignalStrengths(string[] lines)
    {
        var signalSum = 0;
        var measurePoints = new[] { 20, 60, 100, 140, 180, 220 };
        var measureIndex = 0;

        var cpu = new Processor(lines);

        for (int i = 1; i <= measurePoints[^1]; i++)
        {
            cpu.Clock((regX, clock) =>
            {
                if (clock != measurePoints[measureIndex])
                    return;

                var signalStrength = regX * clock;
                Console.WriteLine($"Adding signal strength: {signalStrength}");
                signalSum += signalStrength;

                measureIndex++;
            });
            if (measureIndex == measurePoints.Length) break;
        }

        Console.WriteLine($"signal sum: {signalSum}");
    }

    private class Processor
    {
        enum InstructionType { NOOP, ADDX }

        private int _regX = 1;
        private int _clockIndex = 1;
        private int _instructionIndex = 0;
        private Instruction? _currentInstruction = null;
        private Instruction[] _instructions;
        
        public Processor(string[] instructions)
        {
            _instructions = instructions
                .Select(x => x == "noop"
                    ? new Instruction(InstructionType.NOOP, 0)
                    : new Instruction(InstructionType.ADDX, int.Parse(x[5..])))
                .ToArray();
        }

        public bool Clock(Action<int, int> measure)
        {
            if (_instructionIndex == _instructions.Length) return false;
            measure(_regX, _clockIndex);
            _clockIndex++;
            
            if (_currentInstruction != null)
            {
                _regX += _currentInstruction.Value.Value;
                _currentInstruction = null;
                _instructionIndex++;
            }
            else
            {
                _currentInstruction = _instructions[_instructionIndex];
                if (_currentInstruction.Value.Type != InstructionType.NOOP) return true;
                _currentInstruction = null;
                _instructionIndex++;
            }

            return true;
        }

        record struct Instruction(InstructionType Type, int Value);
    }
}
