public static partial class AoCSolution
{
    public static Func<Task> Day8 => async () =>
          {
              Console.WriteLine("===== Day 8 =====");
              var testInput = await File.ReadAllLinesAsync("TestInputs/day8_test.txt");
              var testInput2 = await File.ReadAllLinesAsync("TestInputs/day8_test2.txt");
              var mainInput = await File.ReadAllLinesAsync("Inputs/day8.txt");

              Console.WriteLine("Part 1 solution for single line test input:");
              SolvePart1ForInputs(testInput);
              Console.WriteLine("Part 1 solution for test inputs:");
              SolvePart1ForInputs(testInput2);
              Console.WriteLine("Part 1 solution for puzzle inputs:");
              SolvePart1ForInputs(mainInput);

              Console.WriteLine("Part 2 solution for single line test input:");
              SolvePart2ForInputs(testInput, true);
              Console.WriteLine("Part 2 solution for test inputs:");
              SolvePart2ForInputs(testInput2, true);
              Console.WriteLine("Part 2 solution for puzzle inputs:");
              SolvePart2ForInputs(mainInput, false);

              void SolvePart1ForInputs(string[] entries)
              {
                  var outputCount = 0;
                  foreach (var entry in entries)
                  {
                      var splits = entry.Split("|", StringSplitOptions.TrimEntries);
                      var uniquePatterns = splits[0].Split(" ", StringSplitOptions.RemoveEmptyEntries);
                      var output = splits[1].Split(" ", StringSplitOptions.RemoveEmptyEntries);
                      outputCount += output.Count(x => x.Length is < 5 or > 6);
                  }
                  Console.WriteLine($"\tAnswer is: {outputCount}");
              }

              void SolvePart2ForInputs(string[] entries, bool isTest)
              {
                  var outputSum = 0L;
                  foreach (var entry in entries)
                  {
                      var knownDigits = new Dictionary<string, int>();

                      var splits = entry.Split("|", StringSplitOptions.TrimEntries);
                      var uniquePatterns = splits[0].Split(" ", StringSplitOptions.RemoveEmptyEntries);
                      var output = splits[1].Split(" ", StringSplitOptions.RemoveEmptyEntries);

                      var one = uniquePatterns.First(x => x.Length == 2);
                      var seven = uniquePatterns.First(x => x.Length == 3);
                      var four = uniquePatterns.First(x => x.Length == 4);
                      var eight = uniquePatterns.First(x => x.Length == 7);

                      var twoThreeFive = uniquePatterns.Where(x => x.Length == 5).ToList();
                      var two = twoThreeFive.First(t => four.Except(t).Count() == 2);
                      var three = twoThreeFive.First(x => two.Except(x).Count() == 1);
                      var five = twoThreeFive.Except(new[] { two, three }).First();

                      var zeroSixNine = uniquePatterns.Where(x => x.Length == 6).ToList();
                      var six = zeroSixNine.First(x => one.Except(x).Count() == 1);
                      var zero = zeroSixNine.Except(new[] { six }).First(x => three.Except(x).Count() == 1);
                      var nine = zeroSixNine.Except(new[] { six, zero }).First();

                      knownDigits.Add(OrderString(one), 1);
                      knownDigits.Add(OrderString(seven), 7);
                      knownDigits.Add(OrderString(four), 4);
                      knownDigits.Add(OrderString(eight), 8);
                      knownDigits.Add(OrderString(two), 2);
                      knownDigits.Add(OrderString(three), 3);
                      knownDigits.Add(OrderString(five), 5);
                      knownDigits.Add(OrderString(six), 6);
                      knownDigits.Add(OrderString(zero), 0);
                      knownDigits.Add(OrderString(nine), 9);

                      outputSum +=
                          knownDigits[OrderString(output[0])] * 1000 +
                          knownDigits[OrderString(output[1])] * 100 +
                          knownDigits[OrderString(output[2])] * 10 +
                          knownDigits[OrderString(output[3])];
                  }

                  Console.WriteLine($"\tAnswer is: {outputSum}");

                  string OrderString(string s) => string.Join("", s.OrderBy(x => x));
              }
          };
}