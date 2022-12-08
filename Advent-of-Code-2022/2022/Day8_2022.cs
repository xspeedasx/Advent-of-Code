namespace Advent_of_Code_2022._2022;

public static class Day8_2022
{
    public static void Run(string testInputPath, string challengeInputPath)
    {
        Solve(File.ReadAllLines(testInputPath));
        Solve(File.ReadAllLines(challengeInputPath));
    }

    private static void Solve(string[] lines)
    {
        int size = lines.Length;

        var map = new byte[size * size];
        var visibilityMap = new bool[size * size];
        var scenicMap = new int[size * size];
        
        for(var y = 0; y < size; y++)
        {
            for (var x = 0; x < size; x++)
            {
                int idx = y * size + x;
                map[idx] = (byte) (lines[y][x] - '0');
                visibilityMap[idx] = x == 0 || x == size - 1 || y == 0 || y == size - 1;
            }
        }

        CalculateVisibilityMap(size, ref map, ref visibilityMap);
        //PrintVisibilityMap(size, visibilityMap);

        Console.WriteLine($"Visible: {visibilityMap.Count(x => x)}");
        Console.WriteLine();

        CalculateScenicMap(size, ref map, ref scenicMap);

        //PrintScenicMap(size, scenicMap);

        Console.WriteLine($"Best score: {scenicMap.Max()}");
        Console.WriteLine();
    }

    private static void CalculateScenicMap(int size, ref byte[] map, ref int[] scenicMap)
    {
        for (var y = 1; y < size - 1; y++)
        {
            int[] lineIndexes = GetLineIndexes(size, y);

            for (var x = 1; x < size; x++)
            {
                int[] colIndexes = GetColumnIndexes(size, x);
                
                int currIdx = lineIndexes[x];
                byte h = map[currIdx];
                // traverse left
                var leftScore = 0;
                for (int i = x-1; i >= 0; i--)
                {
                    leftScore++;
                    if(h <= map[lineIndexes[i]])
                        break;
                }
                // traverse right
                var rightScore = 0;
                for (int i = x+1; i < size; i++)
                {
                    rightScore++;
                    if(h <= map[lineIndexes[i]])
                        break;
                }
                // traverse top
                var topScore = 0;
                for (int i = y-1; i >= 0; i--)
                {
                    topScore++;
                    if(h <= map[colIndexes[i]])
                        break;
                }
                // traverse bottom
                var botScore = 0;
                for (int i = y+1; i < size; i++)
                {
                    botScore++;
                    if(h <= map[colIndexes[i]])
                        break;
                }

                scenicMap[currIdx] = leftScore * rightScore * topScore * botScore;
            }
        }
    }

    private static void PrintScenicMap(int size, int[] scenicMap)
    {
        for (var i = 0; i < size * size; i++)
        {
            if (i > 0 && i % size == 0)
                Console.WriteLine();

            int score = scenicMap[i];
            Console.Write(score > 99 ? "XX" : $"{score:D2}");
        }
        Console.WriteLine();
    }
    
    private static void CalculateVisibilityMap(int size, ref byte[] map, ref bool[] visibilityMap)
    {
        for (var i = 1; i < size - 1; i++)
        {
            // from left
            int hFromLeft = -1;
            int[] fromLeftIndexes = GetLineIndexes(size, i);
            foreach (int index in fromLeftIndexes)
            {
                if (map[index] <= hFromLeft) continue;
                hFromLeft = map[index];
                visibilityMap[index] = true;
            }
            
            // from right
            int hFromRight = -1;
            int[] fromRightIndexes = GetLineIndexes(size, i, true).ToArray();
            foreach (int index in fromRightIndexes)
            {
                if (map[index] <= hFromRight) continue;
                hFromRight = map[index];
                visibilityMap[index] = true;
            }

            // from top
            int hFromTop = -1;
            int[] fromTopIndexes = GetColumnIndexes(size, i);
            foreach (int index in fromTopIndexes)
            {
                if (map[index] <= hFromTop) continue;
                hFromTop = map[index];
                visibilityMap[index] = true;
            }

            // from bottom
            int hFromBottom = -1;
            int[] fromBottomIndexes = GetColumnIndexes(size, i, true);
            foreach (int index in fromBottomIndexes)
            {
                if (map[index] <= hFromBottom) continue;
                hFromBottom = map[index];
                visibilityMap[index] = true;
            }
        }
    }

    private static void PrintVisibilityMap(int size, bool[] visibilityMap)
    {
        for (var i = 0; i < size * size; i++)
        {
            if (i > 0 && i % size == 0)
                Console.WriteLine();
            Console.Write(visibilityMap[i] ? "1" : "0");
        }
        Console.WriteLine();
    }

    private static int[] GetLineIndexes(int size, int line, bool reverse = false)
    {
        return reverse 
            ? Calc().Reverse().ToArray()
            : Calc().ToArray();
        
        IEnumerable<int> Calc()
        {
            for (var i = 0; i < size; i++)
            {
                yield return size * line + i;
            }
        }
    }
    private static int[] GetColumnIndexes(int size, int col, bool reverse = false)
    {
        return reverse 
            ? Calc().Reverse().ToArray()
            : Calc().ToArray();
        
        IEnumerable<int> Calc()
        {
            for (var i = 0; i < size; i++)
            {
                yield return size * i + col;
            }
        }
    }
}
