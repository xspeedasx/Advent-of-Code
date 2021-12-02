public static partial class AoCSolution
{
    public static Func<Task> Day2 => async () =>
      {
          Console.WriteLine("===== Day 2 =====");
          var testInput = await File.ReadAllLinesAsync("TestInputs/day2_test.txt");
          var mainInput = await File.ReadAllLinesAsync("Inputs/day2.txt");

          Console.WriteLine("Part 1 solution for test inputs:");
          SolvePart1ForInputs(testInput);
          Console.WriteLine("Part 1 solution for puzzle inputs:");
          SolvePart1ForInputs(mainInput);

          Console.WriteLine("Part 2 solution for test inputs:");
          SolvePart2ForInputs(testInput);
          Console.WriteLine("Part 2 solution for puzzle inputs:");
          SolvePart2ForInputs(mainInput);

          void SolvePart1ForInputs(string[] report)
          {
              var pos = 0;
              var depth = 0;
              var commands = report.Select(line => (dir: line[0], x: line.Split(" ")[1]));
              foreach (var command in commands)
              {
                  var x = int.Parse(command.x);
                  switch (command.dir)
                  {
                      case 'f':
                          pos += x;
                          break;

                      case 'd':
                          depth += x;
                          break;

                      case 'u':
                          depth -= x;
                          break;
                  }
              }
              Console.WriteLine($"\tpos: {pos}, depth: {depth}, answer (pos*depth) = {pos * depth}");
          }

          void SolvePart2ForInputs(string[] report)
          {
              var aim = 0;
              var pos = 0;
              var depth = 0;
              var commands = report.Select(line => (dir: line[0], x: line.Split(" ")[1]));
              foreach (var command in commands)
              {
                  var x = int.Parse(command.x);
                  switch (command.dir)
                  {
                      case 'f':
                          pos += x;
                          depth += aim * x;
                          break;

                      case 'd':
                          aim += x;
                          break;

                      case 'u':
                          aim -= x;
                          break;
                  }
              }
              Console.WriteLine($"\tpos: {pos}, depth: {depth}, answer (pos*depth) = {pos * depth}");
          }
      };
}