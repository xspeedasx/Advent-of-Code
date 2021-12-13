using System.Collections.Concurrent;

public static partial class AoCSolution
{
    public static Func<Task> Day13 => async () =>
    {
        Console.WriteLine("===== Day 13 =====");
        var testInput = await File.ReadAllLinesAsync("TestInputs/day13_test.txt");
        var mainInput = await File.ReadAllLinesAsync("Inputs/day13.txt");

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
            var (folds, dots) = GetInputs(entries);
            var fold = folds[0];
            var newDots = FoldMap(dots, fold);
            Console.WriteLine($"\tAnswer is: {newDots.Count}");
        }

        void SolvePart2ForInputs(string[] entries)
        {
            var (folds, dots) = GetInputs(entries);

            var newDots = new HashSet<(int x, int y)>();
            foreach (var fold in folds)
            {
                newDots = FoldMap(dots, fold);
                dots = newDots;
            }

            Console.WriteLine($"\tAnswer is:");
            var minX = folds.Where(x => x.axis == "x").Min(x => x.pos);
            var minY = folds.Where(x => x.axis == "y").Min(x => x.pos);
            for (int y = 0; y < minY; y++)
            {
                for (int x = 0; x < minX; x++)
                {
                    Console.Write(dots.Contains((x, y)) ? "#" : ".");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        HashSet<(int x, int y)> FoldMap(HashSet<(int x, int y)> dots, (string axis, int pos) fold)
        {
            var newDots = new HashSet<(int x, int y)>();
            foreach (var dot in dots)
            {
                var newPos = fold.axis == "x"
                    ? (dot.x < fold.pos ? dot.x : dot.x - 2 * (dot.x - fold.pos), dot.y)
                    : (dot.x, dot.y < fold.pos ? dot.y : dot.y - 2 * (dot.y - fold.pos));
                newDots.Add(newPos);
            }
            return newDots;
        }

        (List<(string axis, int pos)> folds, HashSet<(int x, int y)> dots) GetInputs(string[] entries)
        {
            var folds = new List<(string axis, int pos)>();
            var dots = new HashSet<(int x, int y)>();
            foreach (var entry in entries)
            {
                if (entry == "") continue;
                if (entry.StartsWith("fold"))
                {
                    var splits = entry.Split("=");
                    folds.Add((splits[0].Replace("fold along ", ""), int.Parse(splits[1])));
                }
                else
                {
                    var splits = entry.Split(",");
                    dots.Add((int.Parse(splits[0]), int.Parse(splits[1])));
                }
            }
            return (folds, dots);
        }
    };
}