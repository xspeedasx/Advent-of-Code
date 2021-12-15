using System.Collections.Concurrent;

public static partial class AoCSolution
{
    public static Func<Task> Day15 => async () =>
    {
        var stackSize = 1000000000;
        Thread thread = new Thread(new ThreadStart(BigRecursion), stackSize);
        thread.Start();

        void BigRecursion()
        {
            Console.WriteLine("===== Day 15 =====");
            var testInput = File.ReadAllLines("TestInputs/day15_test.txt");
            var mainInput = File.ReadAllLines("Inputs/day15.txt");

            //Console.WriteLine("Part 1 solution for test inputs:");
            //SolvePart1ForInputs(testInput);
            //Console.WriteLine("Part 1 solution for puzzle inputs:");
            //SolvePart1ForInputs(mainInput);

            //Console.WriteLine("Part 2 solution for test inputs:");
            //SolvePart2ForInputs(testInput);
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

                DrawMap(map, w, h);


                var scores = new int[w * h];//.Select(x => int.MaxValue).ToArray();
                Array.Fill(scores, int.MaxValue);

                Traverse(0, 0, -map[0]);

                Console.WriteLine("Scores: ");
                DrawMap(scores, w, h, 3);


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

                void DrawMap(int[] m, int wi, int ht, int padding = 1)
                {
                    for (int y = 0; y < ht; y++)
                    {
                        for (int x = 0; x < wi; x++)
                        {
                            Console.Write(("" + m[y * wi + x]).PadLeft(padding, ' '));
                        }
                        Console.WriteLine();
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

                Console.WriteLine($"width: {w}, heighy: {h}");

                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        var increment = x / initialw + y / initialh;
                        var newVal = initialmap[(y % initialh) * initialw + (x % initialw)];
                        map[y * w + x] = newVal + increment + (newVal + increment > 9 ? -9 : 0);
                        //Console.Write(map[y * w + x]);
                    }
                    //Console.WriteLine();
                }


                var scores = new int[w * h];//.Select(x => int.MaxValue).ToArray();
                Array.Fill(scores, int.MaxValue);

                var maxx = 0;
                var maxy = 0;
                Traverse(0, 0, -map[0]);


                Console.WriteLine($"\tAnswer is: {scores[w * h - 1]}");

                void Traverse(int x, int y, int parentScore)
                {
                    var curr = map[y * w + x];
                    var currScore = parentScore + curr;

                    if (scores[y * w + x] <= currScore) return;
                    scores[y * w + x] = currScore;
                    Console.WriteLine($"Traversing {x}:{y}. score: {currScore}");

                    if (x < w - 1)
                    {
                        var right = scores[y * w + (x + 1)];
                        if (right > currScore) 
                            Traverse(x + 1, y, currScore);
                    }
                    if (y < h - 1)
                    {
                        var down = scores[(y + 1) * w + x];
                        if (down > currScore)
                            Traverse(x, y + 1, currScore);
                    }

                    if (y > 0)
                    {
                        var up = scores[(y - 1) * w + x];
                        if (up > currScore) 
                            Traverse(x, y - 1, currScore);
                    }
                    if (x > 0)
                    {
                        var left = scores[y * w + (x - 1)];
                        if (left > currScore) 
                            Traverse(x - 1, y, currScore);
                    }
                }
            }
        }
    };
}