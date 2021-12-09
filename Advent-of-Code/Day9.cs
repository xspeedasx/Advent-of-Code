public static partial class AoCSolution
{
    public static Func<Task> Day9 => async () =>
          {
              Console.WriteLine("===== Day 9 =====");
              var testInput = await File.ReadAllLinesAsync("TestInputs/day9_test.txt");
              var mainInput = await File.ReadAllLinesAsync("Inputs/day9.txt");

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
                  var lowsSum = 0;
                  for(int y = 0; y < entries.Length; y++)
                  {
                      for (int x = 0; x < entries[y].Length; x++)
                      {
                          var current = entries[y][x];
                          var isLow = true;
                          //up
                          if (y > 0 && current >= entries[y - 1][x])
                              continue;
                          //down
                          if (y < entries.Length - 1 && current >= entries[y + 1][x])
                              continue;
                          //left
                          if (x > 0 && current >= entries[y][x - 1])
                              continue;
                          //right
                          if (x < entries[y].Length - 1 && current >= entries[y][x + 1])
                              continue;

                          lowsSum += int.Parse("" + current) + 1;
                      }
                  }

                  Console.WriteLine($"\tAnswer is: {lowsSum}");
              }

              void SolvePart2ForInputs(string[] entries, bool isTest)
              {
                  var basinMap = new Dictionary<(int x, int y), int>();
                  var basinIndex = 1;

                  for (int y = 0; y < entries.Length; y++)
                  {
                      for (int x = 0; x < entries[y].Length; x++)
                      {
                          var current = entries[y][x];
                          if (current == '9') continue;

                          //up
                          if (y > 0) 
                          {
                              var upEntry = entries[y - 1][x];
                              if(upEntry != '9')
                              {
                                  var neighborBasin = basinMap[(x, y - 1)];
                                  basinMap.Add((x, y), neighborBasin);
                                  CheckNeighbor(x - 1, y, neighborBasin);
                                  continue;
                              }
                          }
                          //left
                          if (x > 0)
                          {
                              var leftEntry = entries[y][x - 1];
                              if (leftEntry != '9')
                              {
                                  var neighborBasin = basinMap[(x - 1, y)];
                                  basinMap.Add((x, y), neighborBasin);
                                  CheckNeighbor(x, y - 1, neighborBasin);
                                  continue;
                              }
                          }

                          basinMap.Add((x, y), basinIndex);
                          basinIndex++;
                      }
                  }

                  var biggestBasins = basinMap.GroupBy(x => x.Value).OrderByDescending(x => x.Count()).Take(3).ToList();
                  var biggestKeys = biggestBasins.Select(x => x.Key).ToList();
                  var basinSum = biggestBasins.Select(x => x.Count());
                  var answer = 1;
                  foreach(var basin in basinSum)
                  {
                      answer *= basin;
                      Console.WriteLine($"basin: {basin}, ans: {answer}");
                  }
                  Console.WriteLine($"\tAnswer is: {answer}");

                  // printing and verifying:
                  for (int y = 0; y < entries.Length; y++)
                  {
                      for (int x = 0; x < entries[y].Length; x++)
                      {
                          var current = basinMap.GetValueOrDefault((x, y));

                          if (current > 0)
                          {
                              //test neighboring;
                              //up
                              if (y > 0)
                              {
                                  var neighbor = basinMap.GetValueOrDefault((x, y + 1));
                                  if (neighbor > 0 && current != neighbor) throw new Exception($"{x},{y} found diff neighbor up");
                              }
                              //down
                              if (y < entries.Length - 1)
                              {
                                  var neighbor = basinMap.GetValueOrDefault((x, y - 1));
                                  if (neighbor > 0 && current != neighbor) throw new Exception($"{x},{y} found diff neighbor down");
                              }
                              //left
                              if (x > 0)
                              {
                                  var neighbor = basinMap.GetValueOrDefault((x + 1, y));
                                  if (neighbor > 0 && current != neighbor) throw new Exception($"{x},{y} found diff neighbor left");
                              }
                              //right
                              if (x < entries[y].Length - 1)
                              {
                                  var neighbor = basinMap.GetValueOrDefault((x - 1, y));
                                  if (neighbor > 0 && current != neighbor) throw new Exception($"{x},{y} found diff neighbor right");
                              }
                          }

                          var basin = basinMap.GetValueOrDefault((x, y));
                          if (basin == 0) Console.Write("*");
                          else if (biggestBasins.Any(x => x.Key == basin)) Console.Write((char)('A' + biggestKeys.IndexOf(basin)));
                          else Console.Write("" + basin%10);
                      }
                      Console.WriteLine();
                  }

                  void CheckNeighbor(int x, int y, int basin)
                  {
                      if (x < 0 || y < 0) return;
                      var neighborBasin = basinMap.GetValueOrDefault((x, y));
                      if (neighborBasin == 0 || neighborBasin == basin) return;
                      if (neighborBasin > basin) basinMap[(x, y)] = basin;
                      else basin = neighborBasin;
                      CheckNeighbor(x - 1, y, basin);
                      CheckNeighbor(x, y - 1, basin);
                      CheckNeighbor(x + 1, y, basin);
                      CheckNeighbor(x, y + 1, basin);
                  }
              }
          };
}