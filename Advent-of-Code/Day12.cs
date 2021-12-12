using System.Collections.Concurrent;

public static partial class AoCSolution
{
    public static Func<Task> Day12 => async () =>
      {
          Console.WriteLine("===== Day 12 =====");
          var testInput = await File.ReadAllLinesAsync("TestInputs/day12_test.txt");
          var mainInput = await File.ReadAllLinesAsync("Inputs/day12.txt");

          Console.WriteLine("Part 1 solution for test inputs:");
          SolvePart1ForInputs(testInput);
          Console.WriteLine("Part 1 solution for puzzle inputs:");
          SolvePart1ForInputs(mainInput);

          Console.WriteLine("Part 2 solution for test inputs:");
          SolvePart2ForInputs(testInput);
          Console.WriteLine("Part 2 solution for puzzle inputs:");
          SolvePart2ForInputs(mainInput);

          void SolvePart1ForInputs(string[] entries)
          {
              var NodeMap = new ConcurrentDictionary<string, CaveNode>();

              foreach (var entry in entries)
              {
                  var splits = entry.Split('-');
                  var a = NodeMap.GetOrAdd(splits[0], new CaveNode { Name = splits[0] });
                  var b = NodeMap.GetOrAdd(splits[1], new CaveNode { Name = splits[1] });
                  a.Links.Add(b);
                  b.Links.Add(a);
              }

              var finishedPaths = new List<List<string>>();
              var traversalQueue = new Queue<List<string>>();

              traversalQueue.Enqueue(new List<string> { "start" });
              while (traversalQueue.Count > 0)
              {
                  var path = traversalQueue.Dequeue();
                  var current = NodeMap[path.Last()];

                  foreach (var link in current.Links)
                  {
                      if (!link.IsBig && path.Contains(link.Name)) continue;
                      var nextPath = new List<string>(path);
                      nextPath.Add(link.Name);
                      if (link.Name == "end")
                      {
                          finishedPaths.Add(nextPath);
                      }
                      else
                      {
                          traversalQueue.Enqueue(nextPath);
                      }
                  }
              }

              Console.WriteLine($"\tAnswer is: {finishedPaths.Count}");
          }

          void SolvePart2ForInputs(string[] entries)
          {
              var NodeMap = new ConcurrentDictionary<string, CaveNode>();

              foreach (var entry in entries)
              {
                  var splits = entry.Split('-');
                  var a = NodeMap.GetOrAdd(splits[0], new CaveNode { Name = splits[0] });
                  var b = NodeMap.GetOrAdd(splits[1], new CaveNode { Name = splits[1] });
                  a.Links.Add(b);
                  b.Links.Add(a);
              }

              var finishedPaths = new List<ComplexPath>();
              var traversalQueue = new Queue<ComplexPath>();
              traversalQueue.Enqueue(new ComplexPath("start"));
              while (traversalQueue.Count > 0)
              {
                  var path = traversalQueue.Dequeue();
                  var current = NodeMap[path.Path.Last()];

                  foreach (var link in current.Links)
                  {
                      if (!link.IsBig && path.VisitedSmallCaves.Contains(link.Name) && path.SecondVisitedSmallCave) continue;
                      if (link.Name == "start") continue;
                      var nextPath = new ComplexPath(path);
                      nextPath.Path.Add(link.Name);
                      if (link.Name == "end")
                      {
                          finishedPaths.Add(nextPath);
                      }
                      else
                      {
                          if (!link.IsBig)
                          {
                              if (!nextPath.VisitedSmallCaves.Add(link.Name))
                                  nextPath.SecondVisitedSmallCave = true;
                          }
                          traversalQueue.Enqueue(nextPath);
                      }
                  }
              }

              Console.WriteLine($"\tAnswer is: {finishedPaths.Count()}");
          }
      };

    private class ComplexPath
    {
        public bool SecondVisitedSmallCave { get; set; }
        public HashSet<string> VisitedSmallCaves { get; set; } = new();
        public List<string> Path { get; set; } = new();

        public ComplexPath(string startNode)
        {
            Path.Add(startNode);
            VisitedSmallCaves.Add(startNode);
        }

        public ComplexPath(ComplexPath other)
        {
            Path = new List<string>(other.Path);
            VisitedSmallCaves = new HashSet<string>(other.VisitedSmallCaves);
            SecondVisitedSmallCave = other.SecondVisitedSmallCave;
        }
    }

    private class CaveNode
    {
        public string Name { get; set; }
        public bool IsBig => Name.Any(x => char.IsUpper(x));
        public List<CaveNode> Links { get; set; } = new();
    }
}