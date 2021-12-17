using System.Collections.Concurrent;
using System.Text.RegularExpressions;

public static partial class AoCSolution
{
    public static Func<Task> Day17 => async () =>
    {
        Console.WriteLine("===== Day 17 =====");
        var testInput = File.ReadAllLines("TestInputs/day17_test.txt");
        var mainInput = File.ReadAllLines("Inputs/day17.txt");

        Console.WriteLine("Part 1 solution for test inputs:");
        SolvePart1ForInputs(testInput);
        Console.WriteLine("Part 1 solution for puzzle inputs:");
        SolvePart1ForInputs(mainInput);

        Console.WriteLine("Part 2 solution for test inputs:");
        SolvePart2ForInputs(testInput);
        Console.WriteLine("Part 2 solution for puzzle inputs:");
        SolvePart2ForInputs(mainInput);
        (int minX, int maxX, int minY, int maxY) GetTargets(string entry)
        {
            var groups = new Regex(@"target area: x=(-?\d+)\.\.(-?\d+), y=(-?\d+)\.\.(-?\d+)").Match(entry).Groups;
            return (int.Parse(groups[1].Value), int.Parse(groups[2].Value), int.Parse(groups[3].Value), int.Parse(groups[4].Value));
        }

        (bool hit, int maxY) Simulate(int initialX, int initialY, int minX, int maxX, int minY, int maxY, bool trackProgress = true)
        {
            var poses = new List<(int x, int y)> { (0, 0) };
            var pos = (x: 0, y: 0);
            var vel = (x: initialX, y: initialY);
            var maxYReached = 0;

            var hit = false;

            while(pos.y > minY && pos.x < maxX)
            {
                pos = (pos.x + vel.x, pos.y + vel.y);
                var newX = vel.x > 0
                    ? vel.x - 1
                    : vel.x < 0
                        ? vel.x + 1
                        : vel.x;
                vel = (newX, vel.y - 1);

                if (trackProgress)
                {
                    if (pos.y > maxYReached) maxYReached = pos.y;
                    poses.Add(pos);
                }

                if (pos.x >= minX && pos.x <= maxX && pos.y >= minY && pos.y <= maxY) { hit = true; break; }
                if (pos.x > maxX || pos.y < minY) break;
            }

            if (trackProgress && initialX == 6 && initialY == 9)
            {
                Console.WriteLine();
                for (var y = maxYReached; y >= minY; y--)
                {
                    for (var x = 0; x <= maxX; x++)
                    {
                        if (x == 0 && y == 0) Console.Write("S");
                        else if (poses.Contains((x, y))) Console.Write("#");
                        else if (x >= minX && x <= maxX && y >= minY && y <= maxY) Console.Write("T");
                        else Console.Write(".");
                    }
                    Console.WriteLine();
                }
            }

            return (hit, maxYReached);
        }

        void SolvePart1ForInputs(string[] entries)
        {
            var (minX, maxX, minY, maxY) = GetTargets(entries[0]);

            var minInitialX = 0;
            var maxReachedX = 0;
            while(maxReachedX < minX)
            {
                var reachedX = 0;
                minInitialX++;
                for(int i = minInitialX; i > 0; i--)
                {
                    reachedX += i;
                }
                if (reachedX > maxReachedX) 
                    maxReachedX = reachedX;
            }
            var maxInitialY = -minY - 1;
            Console.WriteLine($"  Min initial X to reach {maxReachedX}: {minInitialX}");
            Console.WriteLine($"  Max initial Y to hit target: {maxInitialY}");

            var maxYReached = -10;
            var maxYInitialVel = (x:0, y:0);
            for(int y = 0; y <= maxInitialY; y++)
            {
                Console.Write($"    simulating for: {minInitialX},{y}.. ");
                var (hit, mY) = Simulate(minInitialX, y, minX, maxX, minY, maxY);
                if (hit && mY > maxYReached)
                {
                    maxYReached = mY;
                    maxYInitialVel = (minInitialX, y);
                }
                Console.WriteLine(hit ? "HIT!" : "missed..");
            }


            Console.WriteLine($"\tAnswer is: {maxYInitialVel.x},{maxYInitialVel.y} to reach Y {maxYReached}");
        }

        void SolvePart2ForInputs(string[] entries)
        {
            var (minX, maxX, minY, maxY) = GetTargets(entries[0]);

            var minInitialX = 0;
            var maxReachedX = 0;
            while (maxReachedX < minX)
            {
                var reachedX = 0;
                minInitialX++;
                for (int i = minInitialX; i > 0; i--)
                {
                    reachedX += i;
                }
                if (reachedX > maxReachedX)
                    maxReachedX = reachedX;
            }
            var maxInitialX = maxX;

            var minInitialY = minY;
            var maxInitialY = -minY - 1;
            Console.WriteLine($"  Min initial X to reach {maxReachedX}: {minInitialX}");
            Console.WriteLine($"  Max initial Y to hit target: {maxInitialY}");

            var maxYReached = -10;
            var maxYInitialVel = (x: 0, y: 0);

            var hitCnt = 0;
            for (int y = minInitialY; y <= maxInitialY; y++)
            {
                var tempHits = hitCnt;
                for (int x = minInitialX; x <= maxInitialX; x++)
                {
                    var (hit, mY) = Simulate(x, y, minX, maxX, minY, maxY, false);
                    if (hit) hitCnt++;
                }
                //Console.WriteLine($"simulated for y = {y}, new hits: {hitCnt - tempHits}");
            }
            Console.WriteLine($"\tAnswer is: {hitCnt}");
        }
    };
}