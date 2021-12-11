public static partial class AoCSolution
{
    public static Func<Task> Day11 => async () =>
{
    Console.WriteLine("===== Day 11 =====");
    var testInput = await File.ReadAllLinesAsync("TestInputs/day11_test.txt");
    var mainInput = await File.ReadAllLinesAsync("Inputs/day11.txt");

    Console.WriteLine("Solution for test inputs:");
    SolvePart1ForInputs(testInput);
    Console.WriteLine("Solution for puzzle inputs:");
    SolvePart1ForInputs(mainInput);

    void SolvePart1ForInputs(string[] entries, bool drawVisual = false)
    {
        var map = new int[100];
        for(int y = 0; y < 10; y++)
        {
            for(int x = 0; x < 10; x++)
            {
                map[y * 10 + x] = int.Parse("" + entries[y][x]);
            }
        }

        if (drawVisual)
        {
            Console.WriteLine("Before any steps:");
            DrawMap(map);
        }

        var allFlashStep = 0;
        var totalFlashes = 0;
        for (int i = 1;; i++)
        {
            // increment by 1
            map = map.Select(x => x + 1).ToArray();

            //flashes
            var flashed = true;
            while (flashed)
            {
                flashed = false;
                for (int y = 0; y < 10; y++)
                {
                    for (int x = 0; x < 10; x++)
                    {
                        var currIdx = y * 10 + x;

                        if (map[currIdx] > 9)
                        {
                            map[currIdx] = 0;
                            for(var dy = -1; dy <= 1; dy++)
                            {
                                for(var dx = -1; dx <= 1; dx++)
                                {
                                    if (y + dy < 0 || y + dy > 9 || x + dx < 0 || x + dx > 9)
                                        continue;
                                    var neighborIdx = (y + dy) * 10 + (x + dx);
                                    if(map[neighborIdx] > 0)
                                    {
                                        map[neighborIdx]++;
                                    }
                                }
                            }

                            flashed = true;
                            if(i <= 100)
                                totalFlashes++;
                        }
                    }
                }
            }

            if (map.All(x => x == 0))
            {
                allFlashStep = i;
                break;
            }

            if (drawVisual && (i <= 10 || i % 10 == 0))
            {
                Console.WriteLine($"After step {i}: total flashes: {totalFlashes}");
                DrawMap(map);
            }
        }

        Console.WriteLine($"\tPart 1 Answer: Total flashes in 100 steps {totalFlashes}");
        Console.WriteLine($"\tPart 2 answer: All flashed at step {allFlashStep}");
    }

    void DrawMap(int[] map)
    {
        for(int y = 0; y< 10; y++)
        {
            Console.WriteLine(String.Join("", map[(y * 10)..((y + 1) * 10)]));
        }
    }
};
}