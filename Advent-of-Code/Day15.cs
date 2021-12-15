using System.Collections.Concurrent;

public static partial class AoCSolution
{
    public static Func<Task> Day15 => async () =>
    {
        Console.WriteLine("===== Day 15 =====");
        var testInput = File.ReadAllLines("TestInputs/day15_test.txt");
        var mainInput = File.ReadAllLines("Inputs/day15.txt");

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
            var w = entries[0].Length;
            var h = entries.Length;
            var map = new int[entries.Length * entries[0].Length];
            for (int y = 0; y < entries.Length; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    map[y * w + x] = (byte)int.Parse("" + entries[y][x]);
                }
            }

            var scores = new int[w * h];
            Array.Fill(scores, int.MaxValue);

            Traverse(0, 0, -map[0]);

            Console.WriteLine($"\tAnswer is: {scores[w * h - 1]}");

            void Traverse(int x, int y, int parentScore)
            {
                var curr = map[y * w + x];
                var currScore = parentScore + curr;

                if (scores[y * w + x] <= currScore) return;
                scores[y * w + x] = currScore;

                if (x > 0)
                {
                    var left = scores[y * w + (x - 1)];
                    if (left > currScore) Traverse(x - 1, y, currScore);
                }
                if (x < w - 1)
                {
                    var right = scores[y * w + (x + 1)];
                    if (right > currScore) Traverse(x + 1, y, currScore);
                }

                if (y > 0)
                {
                    var up = scores[(y - 1) * w + x];
                    if (up > currScore) Traverse(x, y - 1, currScore);
                }
                if (y < h - 1)
                {
                    var down = scores[(y + 1) * w + x];
                    if (down > currScore) Traverse(x, y + 1, currScore);
                }
            }
        }

        void SolvePart2ForInputs(string[] entries)
        {
            var initialw = entries[0].Length;
            var initialh = entries.Length;
            var initialmap = new int[initialh * initialw];
            for (int y = 0; y < initialh; y++)
            {
                for (int x = 0; x < initialw; x++)
                {
                    initialmap[y * initialw + x] = (byte)int.Parse("" + entries[y][x]);
                }
            }

            var w = initialw * 5;
            var h = initialh * 5;
            var map = new int[w * h];

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    var increment = x / initialw + y / initialh;
                    var newVal = initialmap[(y % initialh) * initialw + (x % initialw)];
                    map[y * w + x] = newVal + increment + (newVal + increment > 9 ? -9 : 0);
                }
            }

            var scores = new long[w * h];
            Array.Fill(scores, int.MaxValue/2);
            scores[0] = 0;

            var iteration = 0;
            var changed = true;
            while (changed)
            {
                changed = false;
                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        var curr = map[y * w + x];
                        var currScore = scores[y * w + x];

                        scores[y * w + x] = currScore;

                        if (x > 0)
                        {
                            var left = scores[y * w + (x - 1)];
                            if (left + curr < currScore)
                            {
                                scores[y * w + x] = left + curr;
                                changed = true;
                            }
                        }
                        if (y > 0)
                        {
                            var up = scores[(y - 1) * w + x];
                            if (up + curr < currScore)
                            {
                                scores[y * w + x] = up + curr;
                                changed = true;
                            }
                        }

                        if (x < w - 1)
                        {
                            var right = scores[y * w + (x + 1)];
                            if (right + curr < currScore)
                            {
                                scores[y * w + x] = right + curr;
                                changed = true;
                            }
                        }
                        if (y < h - 1)
                        {
                            var down = scores[(y + 1) * w + x];
                            if (down + curr < currScore)
                            {
                                scores[y * w + x] = down + curr;
                                changed = true;
                            }
                        }
                    }
                }
                //Console.WriteLine($"Iteration: {++iteration}, score: {scores[w*h-1]}");
            }

            Console.WriteLine($"\tAnswer is: {scores[w * h - 1]}");
        }
    };
}