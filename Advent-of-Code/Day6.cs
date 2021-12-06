public static partial class AoCSolution
{
    public static Func<Task> Day6 => async () =>
      {
          Console.WriteLine("===== Day 6 =====");
          var testInput = await File.ReadAllTextAsync("TestInputs/day6_test.txt");
          var mainInput = await File.ReadAllTextAsync("Inputs/day6.txt");

          Console.WriteLine("Part 1 solution for test inputs:");
          SolvePart1ForInputs(testInput);
          Console.WriteLine("Part 1 solution for puzzle inputs:");
          SolvePart1ForInputs(mainInput);

          Console.WriteLine("Part 2 solution for test inputs:");
          SolvePart2ForInputs(testInput);
          Console.WriteLine("Part 2 solution for puzzle inputs:");
          SolvePart2ForInputs(mainInput);

          void SolvePart1ForInputs(string fishString)
          {
              var currentFish = fishString.Split(",").Select(x => int.Parse(x)).ToList();

              for(int i = 0; i < 80; i++)
              {
                  var newFish = new List<int>();
                  foreach(var f in currentFish)
                  {
                      if(f == 0)
                      {
                          newFish.Add(6);
                          newFish.Add(8);
                      }
                      else
                      {
                          newFish.Add(f - 1);
                      }
                  }
                  currentFish = newFish;
              }

              Console.WriteLine($"\tAnswer is: {currentFish.Count}");
          }

          //the numbers are way too big to simulate using Lists
          //group counts by 'days remaining' and apply aging to whole group
          void SolvePart2ForInputs(string fishString)
          {
              var currentFish = fishString.Split(",").Select(x => int.Parse(x)).ToList();
              var counts = new long[9];
              foreach(var f in currentFish)
              {
                  counts[f]++;
              }

              for (int i = 0; i < 256; i++)
              {
                  var newCounts = new long[9];
                  for(int c = 8; c >= 0; c--)
                  {
                      if (c == 0)
                      {
                          newCounts[8] = counts[0];
                          newCounts[6] += counts[0];
                      }
                      else
                      {
                          newCounts[c - 1] = counts[c];
                      }
                  }
                  counts = newCounts;
              }

              Console.WriteLine($"\tAnswer is: {counts.Sum()}");
          }
      };
}