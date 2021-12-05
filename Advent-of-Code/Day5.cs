using System.Text.RegularExpressions;

public static partial class AoCSolution
{
    public static Regex lineExtractRegex = new Regex(@"(\d+),(\d+) -> (\d+),(\d+)");

    public static Func<Task> Day5 => async () =>
      {
          Console.WriteLine("===== Day 5 =====");
          var testInput = await File.ReadAllLinesAsync("TestInputs/day5_test.txt");
          var mainInput = await File.ReadAllLinesAsync("Inputs/day5.txt");

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
              var map = new Dictionary<(int x, int y), int>();
              foreach (var input in report)
              {
                  var line = ParseLine(input);
                  AddLineToMap(map, line, includeDiagonal: false);
              }

              Console.WriteLine($"\tAnswer is: {map.Count(x => x.Value > 1)}");
          }

          void SolvePart2ForInputs(string[] report)
          {
              var map = new Dictionary<(int x, int y), int>();
              foreach (var input in report)
              {
                  var line = ParseLine(input);
                  AddLineToMap(map, line, includeDiagonal: true);
              }
              Console.WriteLine($"\tAnswer is: {map.Count(x => x.Value > 1)}");
          }

          (int x1, int y1, int x2, int y2) ParseLine(string line)
          {
              var m = lineExtractRegex.Match(line);
              if (m.Groups.Count != 5) return default;
              return (int.Parse(m.Groups[1].Value),
                      int.Parse(m.Groups[2].Value),
                      int.Parse(m.Groups[3].Value),
                      int.Parse(m.Groups[4].Value));
          }

          void AddPointToMap(Dictionary<(int x, int y), int> map, int x, int y)
          {
              var pt = (x: x, y: y);

              if (!map.ContainsKey(pt))
                  map.Add(pt, 0);
              map[pt]++;
          }

          void AddLineToMap(Dictionary<(int x, int y), int> map, (int x1, int y1, int x2, int y2) line, bool includeDiagonal)
          {
              if (line.x1 == line.x2)
              {
                  var dir = line.y1 < line.y2 ? 1 : -1;
                  var len = Math.Abs(line.y2 - line.y1) + 1;
                  var y = line.y1;
                  for (int i = 0; i < len; i++)
                  {
                      AddPointToMap(map, line.x1, y);
                      y += dir;
                  }
              }
              else if (line.y1 == line.y2)
              {
                  var dir = line.x1 < line.x2 ? 1 : -1;
                  var len = Math.Abs(line.x2 - line.x1) + 1;
                  var x = line.x1;
                  for (int i = 0; i < len; i++)
                  {
                      AddPointToMap(map, x, line.y1);
                      x += dir;
                  }
              }
              else if (includeDiagonal)
              {
                  var dirx = line.x1 < line.x2 ? 1 : -1;
                  var diry = line.y1 < line.y2 ? 1 : -1;
                  var len = Math.Abs(line.x2 - line.x1) + 1;

                  var x = line.x1;
                  var y = line.y1;
                  for (int i = 0; i < len; i++)
                  {
                      AddPointToMap(map, x, y);
                      x += dirx;
                      y += diry;
                  }
              }
          }
      };
}