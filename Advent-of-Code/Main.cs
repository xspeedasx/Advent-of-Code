using System.Diagnostics;
using static AoCSolution;
Console.Title = "Advent-of-Code 2021";
var solutions = new[]
{
    Day1,
    Day2,
    Day3,
    Day4,
    Day5,
    Day6,
    Day7,
    Day8,
    Day9,
    Day10,
    Day11,
    Day12,
    Day13,
    Day14,
    Day15,
    Day16,
};

for(;;)
{
    Console.WriteLine($"Choose day to view solution (1-{solutions.Length})");
    var ans = Console.ReadLine();
    if (int.TryParse(ans, out var day) && day >= 1 && day <= solutions.Length)
    {
        var sw = new Stopwatch();
        sw.Start();
        await solutions[day - 1]();
        sw.Stop();
        Console.WriteLine($"Solving day {day} took: {sw.ElapsedMilliseconds} ms");
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }
    Console.Clear();
}