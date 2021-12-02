public static partial class AoCSolution
{
    public static Func<Task> Day1 => async () =>
      {
          Console.WriteLine("===== Day 1 =====");
          var testInput = await File.ReadAllLinesAsync("TestInputs/day1_test.txt");
          var mainInput = await File.ReadAllLinesAsync("Inputs/day1.txt");

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
              var measurements = report.Select(line => int.Parse(line));
              var increasedCount = 0;
              int? previous = null;
              foreach (var measurement in measurements)
              {
                  if (previous != null && measurement > previous)
                  {
                      increasedCount++;
                  }
                  previous = measurement;
              }
              Console.WriteLine($"\tAnswer is: {increasedCount}");
          }

          void SolvePart2ForInputs(string[] report)
          {
              var measurements = report.Select(line => int.Parse(line)).ToArray();
              var increasedCount = 0;
              for (int i = 0; i < measurements.Length - 3; i++)
              {
                  if (measurements[i] < measurements[i + 3]) increasedCount++;
              }
              Console.WriteLine($"\tAnswer is: {increasedCount}");
          }
      };
}