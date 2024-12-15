using System.Drawing;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace Advent_of_Code._2024;

public class Day15(ITestOutputHelper output)
{
    class MapInput
    {
        public string[] Map { get; set; }
        public string Moves { get; set; }
    }

    private static MapInput ParseInput(string[] lines)
    {
        var testInput = new MapInput();
        var mapLines = new List<string>();
        var isMap = true;

        foreach (string line in lines)
        {
            if (line == "")
            {
                isMap = false;
                continue;
            }

            if (isMap)
            {
                mapLines.Add(line);
            }
            else
            {
                testInput.Moves += line;
            }
        }
        testInput.Map = mapLines.ToArray();
        return testInput;
    }
    
    private readonly MapInput _testInput1 = ParseInput(File.ReadAllLines("Inputs/TestInputs/Test15_1.txt"));
    private readonly MapInput _testInput2 = ParseInput(File.ReadAllLines("Inputs/TestInputs/Test15_2.txt"));
    private readonly MapInput _testInput3 = ParseInput(File.ReadAllLines("Inputs/TestInputs/Test15_3.txt"));
    private readonly MapInput _input = ParseInput(File.ReadAllLines("Inputs/Day15.txt"));
    
    private Dictionary<Point, char> ProcessInput(MapInput input)
    {
        Dictionary<Point, char> map = GetDict(input.Map);
        int mapw = input.Map[0].Length;
        int maph = input.Map.Length;
        
        var current = Point.Empty;
        
        var walls = new HashSet<Point>();
        
        for (var y = 0; y < maph; y++)
        {
            for (var x = 0; x < mapw; x++)
            {
                var point = new Point(x, y);
                char c = map[point];
                switch (c)
                {
                    case '@':
                        current = point;
                        map[point] = '.';
                        break;
                    case '#':
                        walls.Add(point);
                        break;
                }
            }
        }
        
        var moveIndex = 0;
        
        DrawDebugMap(map, mapw, maph, current, moveIndex, input.Moves);
        
        while(moveIndex++ < input.Moves.Length)
        {
            char move = input.Moves[moveIndex-1];
            
            Point next = move switch
            {
                '^' => current with { Y = current.Y - 1 },
                '>' => current with { X = current.X + 1 },
                'v' => current with { Y = current.Y + 1 },
                '<' => current with { X = current.X - 1 },
                _ => throw new Exception("Invalid move")
            };
            
            char nextChar = map[next];
            
            if(walls.Contains(next))
            {
                DrawDebugMap(map, mapw, maph, current, moveIndex, input.Moves);
                continue;
            }
            if(nextChar == '.')
            {
                current = next;
                DrawDebugMap(map, mapw, maph, current, moveIndex, input.Moves);
                continue;
            }

            var direction = new Size(next.X - current.X, next.Y - current.Y);
            Point nextNext = next + direction;
            while (true)
            {
                if(!map.TryGetValue(nextNext, out char nextNextChar))
                {
                    throw new Exception("Out of bounds");
                }
                if (nextNextChar == '#')
                {
                    break;
                }
                if (nextNextChar == '.')
                {
                    map[nextNext] = 'O';
                    map[next] = '.';
                    current = next;
                    break;
                }
                nextNext += direction;
            }
            DrawDebugMap(map, mapw, maph, current, moveIndex, input.Moves);
        }
        return map;
    }

    private void DrawDebugMap(Dictionary<Point,char> map, int mapw, int maph, Point current, int moveIndex, string moves)
    {
        if(moveIndex >= 0) 
            return;
        
        output.WriteLine(moveIndex == 0 ? "Initial state:" : $"Move ({moveIndex}) {moves[moveIndex - 1]}:");
        for (var y = 0; y < maph; y++)
        {
            var row = new StringBuilder();
            for (var x = 0; x < mapw; x++)
            {
                var point = new Point(x, y);
                if (point == current)
                {
                    row.Append('@');
                }
                else
                {
                    row.Append(map[point]);
                }
            }
            output.WriteLine(row.ToString());
        }
        output.WriteLine("");
    }

    private Dictionary<Point, char> GetDict(string[] inputMap)
    {
        var map = new Dictionary<Point, char>();
        for (var y = 0; y < inputMap.Length; y++)
        {
            for (var x = 0; x < inputMap[y].Length; x++)
            {
                map[new Point(x, y)] = inputMap[y][x];
            }
        }
        return map;
    }
    
    private long CalculateBoxIndex(MapInput input)
    {
        Dictionary<Point, char> map = ProcessInput(input);
        var sum = 0L;
        foreach ((Point point, char c) in map)
        {
            if (c == 'O')
            {
                sum += point.X + point.Y * 100;
            }
        }
        output.WriteLine($"sum of all boxes' GPS coordinates: {sum}");
        return sum;
    }

    [Fact]
    public void Part1Test1()
    {
        Assert.Equal(2028, CalculateBoxIndex(_testInput1));
    }
    
    [Fact]
    public void Part1Test2()
    {
        Assert.Equal(10092, CalculateBoxIndex(_testInput2));
    }
    
    [Fact]
    public void Part1()
    {
        Assert.Equal(1476771, CalculateBoxIndex(_input));
    }
    
    private long CalculateWideBoxIndex(MapInput input)
    {
        Dictionary<Point, char> map = ProcessWideInput(input);
        var sum = 0L;
        foreach ((Point point, char c) in map)
        {
            if (c == '[')
            {
                sum += point.X + point.Y * 100;
            }
        }
        output.WriteLine($"sum of all boxes' GPS coordinates: {sum}");
        return sum;
    }

    private string[] WidenMap(string[] map)
    {
        var newMap = new List<string>();
        foreach (string line in map)
        {
            var newLine = new StringBuilder();
            foreach (char c in line)
            {
                newLine.Append(c switch
                    {
                        '#' => "##",
                        'O' => "[]",
                        '.' => "..",
                        '@' => "@.",
                        _ => ""
                    }
                );
            }
            newMap.Add(newLine.ToString());
        }
        return newMap.ToArray();
    }
    
    private Dictionary<Point, char> ProcessWideInput(MapInput input)
    { 
        Dictionary<Point, char> map = GetDict(WidenMap(input.Map));
        int mapw = input.Map[0].Length * 2;
        int maph = input.Map.Length;
        
        var current = Point.Empty;
        
        var walls = new HashSet<Point>();
        var boxes = new List<Box>();
        
        for (var y = 0; y < maph; y++)
        {
            for (var x = 0; x < mapw; x++)
            {
                var point = new Point(x, y);
                char c = map[point];
                switch (c)
                {
                    case '@':
                        current = point;
                        map[point] = '.';
                        break;
                    case '#':
                        walls.Add(point);
                        break;
                    case '[':
                        boxes.Add(new Box([point, point with { X = x + 1 }]));
                        break;
                }
            }
        }
        
        var moveIndex = 0;
        
        DrawDebugMap(map, mapw, maph, current, moveIndex, input.Moves);
        
        while(moveIndex++ < input.Moves.Length)
        {
            char move = input.Moves[moveIndex-1];
            
            Point next = move switch
            {
                '^' => current with { Y = current.Y - 1 },
                '>' => current with { X = current.X + 1 },
                'v' => current with { Y = current.Y + 1 },
                '<' => current with { X = current.X - 1 },
                _ => throw new Exception("Invalid move")
            };
            
            char nextChar = map[next];
            
            if(walls.Contains(next))
            {
                DrawDebugMap(map, mapw, maph, current, moveIndex, input.Moves);
                continue;
            }
            if(nextChar == '.')
            {
                current = next;
                DrawDebugMap(map, mapw, maph, current, moveIndex, input.Moves);
                continue;
            }

            var direction = new Size(next.X - current.X, next.Y - current.Y);
            if (direction.Width != 0)
            {
                var boxesAffected = new List<Box>();;
                var nextNext = next;
                var nextNextChar = map[nextNext];
                while(nextNextChar != '#' && nextNextChar != '.')
                {
                    boxesAffected.Add(boxes.Single(box => box.IsHit(nextNext)));
                    nextNext += direction*2;
                    nextNextChar = map[nextNext];
                }
                if(nextNextChar == '#')
                {
                    DrawDebugMap(map, mapw, maph, current, moveIndex, input.Moves);
                    continue;
                }
                foreach (Box box in boxesAffected)
                {
                    box.Move(direction);
                    map[box.Coordinates[0]] = '[';
                    map[box.Coordinates[1]] = ']';
                }
                map[next] = '.';
                current = next;
            }
            else
            {
                var boxesAffected = new List<Box> { boxes.Single(box1 => box1.IsHit(next)) };
                List<Box> boxLine = boxesAffected;
                while (true)
                {
                    // does hit wall?
                    if (boxLine.Any(b =>
                        {
                            Point nextLeft = b.Coordinates[0] + direction;
                            if (map[nextLeft] == '#')
                            {
                                return true;
                            }

                            Point nextRight = b.Coordinates[1] + direction;
                            return map[nextRight] == '#';
                        }))
                    {
                        break;
                    }
                    
                    // does hit box?
                    List<Point> nextPossiblePoints = boxLine.SelectMany(b =>
                    {
                        Point nextLeft = b.Coordinates[0] + direction;
                        Point nextRight = b.Coordinates[1] + direction;
                        return new[] {nextLeft, nextRight};
                    }).ToList();
                    
                    List<Box> nextBoxes = boxes
                        .Where(b => nextPossiblePoints.Any(b.IsHit))
                        .ToList();
                    if (nextBoxes.Count > 0)
                    {
                        boxesAffected.AddRange(nextBoxes);
                        boxLine = nextBoxes;
                        continue;
                    }

                    // move boxes
                    foreach (Box box in boxesAffected)
                    {
                        if(!boxesAffected.Any(x => x!=box && x.IsHit(box.Coordinates[0])))
                        {
                            map[box.Coordinates[0]] = '.';
                        }
                        if(!boxesAffected.Any(x => x!=box && x.IsHit(box.Coordinates[1])))
                        {
                            map[box.Coordinates[1]] = '.';
                        }
                        box.Move(direction);
                        {
                            map[box.Coordinates[0]] = '[';
                        }
                        map[box.Coordinates[0]] = '[';
                        map[box.Coordinates[1]] = ']';
                    }
                    map[next] = '.';
                    current = next;
                    break;
                }
            }
            
            DrawDebugMap(map, mapw, maph, current, moveIndex, input.Moves);
        }
        return map;
    }
    
    [Fact]
    public void Part2Test1()
    {
        Assert.Equal(312, CalculateWideBoxIndex(_testInput3));
    }
    
    [Fact]
    public void Part2Test2()
    {
        Assert.Equal(9021, CalculateWideBoxIndex(_testInput2));
    }
    
    [Fact]
    public void Part2()
    {
        Assert.Equal(1468005, CalculateWideBoxIndex(_input));
    }

    private class Box(Point[] coordinates)
    {
        private Point _left = coordinates[0];
        private Point _right = coordinates[1];

        public Point[] Coordinates => [_left, _right];
        
        public void Move(Size direction)
        {
            _left += direction;
            _right += direction;
        }
        
        public bool IsHit(Point point)
        {
            return _left == point || _right == point;
        }
    }
}