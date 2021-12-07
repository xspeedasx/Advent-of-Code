public static partial class AoCSolution
{
    public static Func<Task> Day7 => async () =>
      {
          Console.WriteLine("===== Day 7 =====");
          var testInput = await File.ReadAllTextAsync("TestInputs/day7_test.txt");
          var mainInput = await File.ReadAllTextAsync("Inputs/day7.txt");

          Console.WriteLine("Part 1 solution for test inputs:");
          SolvePart1ForInputs(testInput);
          Console.WriteLine("Part 1 solution for puzzle inputs:");
          SolvePart1ForInputs(mainInput);

          Console.WriteLine("Part 2 solution for test inputs:");
          SolvePart2ForInputs(testInput);
          Console.WriteLine("Part 2 solution for puzzle inputs:");
          SolvePart2ForInputs(mainInput);

          void SolvePart1ForInputs(string positionsString)
          {
              var positions = positionsString.Split(',').Select(x => int.Parse(x)).ToList();

              var min = positions.Min();
              var max = positions.Max();
              var minF = int.MaxValue;

              for(var i = min; i < max; i++)
              {
                  var f = positions.Sum(x => Math.Abs(x - i));
                  if (f < minF) minF = f;
              }

              Console.WriteLine($"\tAnswer is: {minF}");
          }

          //the numbers are way too big to simulate using Lists
          //group counts by 'days remaining' and apply aging to whole group
          void SolvePart2ForInputs(string positionsString)
          {
              var positions = positionsString.Split(',').Select(x => int.Parse(x)).ToList();

              var min = positions.Min();
              var max = positions.Max();
              var minF = int.MaxValue;

              for (var i = min; i < max; i++)
              {
                  var f = positions.Sum(x => 
                  {
                      var dist = Math.Abs(x - i);
                      return dist*(dist+1)/2;
                  });
                  if (f < minF) minF = f;
              }

              Console.WriteLine($"\tAnswer is: {minF}");
          }
      };
}