public static partial class AoCSolution
{
    public static Func<Task> Day10 => async () =>
          {
              Console.WriteLine("===== Day 10 =====");
              var testInput = await File.ReadAllLinesAsync("TestInputs/day10_test.txt");
              var mainInput = await File.ReadAllLinesAsync("Inputs/day10.txt");

              Console.WriteLine("Part 1 solution for test inputs:");
              SolvePart1ForInputs(testInput);
              Console.WriteLine("Part 1 solution for puzzle inputs:");
              SolvePart1ForInputs(mainInput);

              Console.WriteLine("Part 2 solution for test inputs:");
              SolvePart2ForInputs(testInput, true);
              Console.WriteLine("Part 2 solution for puzzle inputs:");
              SolvePart2ForInputs(mainInput, false);

              void SolvePart1ForInputs(string[] entries)
              {
                  var pointsTable = new Dictionary<char, int>
                  {
                      {')', 3 },
                      {']', 57 },
                      {'}', 1197 },
                      {'>', 25137 }
                  };

                  var errorSum = 0;
                  foreach (var entry in entries)
                  {
                      var stack = new Stack<char>();
                      for (int i = 0; i < entry.Length; i++)
                      {
                          var c = entry[i];
                          if(!isLegal(stack, c))
                          {
                              errorSum += pointsTable[c];
                              break;
                          }
                      }
                  }

                  Console.WriteLine($"\tAnswer is: {errorSum}");
              }

              void SolvePart2ForInputs(string[] entries, bool isTest)
              {
                  var pointsTable = new Dictionary<char, int>
                  {
                      {'(', 1 },
                      {'[', 2 },
                      {'{', 3 },
                      {'<', 4 }
                  };

                  var lineScores = new List<long>();
                  foreach (var entry in entries)
                  {
                      var legal = true;
                      var stack = new Stack<char>();
                      for (int i = 0; i < entry.Length; i++)
                      {
                          var c = entry[i];
                          if (!isLegal(stack, c))
                          {
                              legal = false;
                              break;
                          }
                      }

                      if (legal)
                      {
                          var lineScore = 0L;
                          while(stack.Count > 0)
                          {
                              lineScore *= 5;
                              lineScore += pointsTable[stack.Pop()];
                          }
                          lineScores.Add(lineScore);
                      }
                  }

                  var sorted= lineScores.OrderBy(x => x).ToList();
                  var middle = sorted[sorted.Count / 2];
                  Console.WriteLine($"\tAnswer is: {middle}");
              }

              bool isLegal(Stack<char> stack, char c)
              {
                  if (c == '(' || c == '[' || c == '<' || c == '{')
                  {
                      stack.Push(c);
                      return true;
                  }

                  if (c == ')' || c == ']' || c == '>' || c == '}')
                  {
                      var last = stack.Pop();
                      //would probably be better to match by dictionary
                      if (c == ')' && last != '(' ||
                          c == ']' && last != '[' ||
                          c == '>' && last != '<' ||
                          c == '}' && last != '{')
                      {
                          return false;
                      }
                  }
                  return true;
              }
          };
}