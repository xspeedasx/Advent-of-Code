using System.Drawing;
using Xunit;
using Xunit.Abstractions;

namespace Advent_of_Code._2024;

public class Day21
{
    private static ITestOutputHelper _output;
    private readonly string[] _input = File.ReadAllLines("Inputs/Day21.txt");
    private readonly string[] _testInput = File.ReadAllLines("Inputs/TestInputs/Test21.txt");

    public Day21(ITestOutputHelper output)
    {
        _output = output;
    }

    private static readonly (Size, char)[] DirectionArrows = DirUtil.Directions
        .Select((dir, idx) => (dir, idx switch
        {
            0 => '^',
            1 => '>',
            2 => 'v',
            3 => '<',
            _ => throw new ArgumentOutOfRangeException()
        }))
        .ToArray();

    private static List<char[]> GenericPadPossibleMoves(char[] code, Dictionary<char, Point> positions, Point invalidPosition, char startLetter = 'A')
    {
        var possiblePaths = new List<char[]>();
        
        Point[] codeTargets = code.Select(c => positions[c]).ToArray();
        var queue = new Queue<(Point current, char[] currentPath, Point[] targets)>();
        queue.Enqueue((positions[startLetter], [], codeTargets));
        
        while (queue.Count > 0)
        {
            (Point current, char[] currentPath, Point[] targets) = queue.Dequeue();
            Point currentTarget = targets[0];
            
            var diff = new Size(currentTarget.X - current.X, currentTarget.Y - current.Y);
            if (current == currentTarget)
            {
                char[] nextPath = currentPath
                    .Append('A')
                    .ToArray();
                if(targets.Length == 1)
                {
                    possiblePaths.Add(nextPath);
                    continue;
                }
                targets = targets[1..];
                queue.Enqueue((current, nextPath, targets));
                continue;
            }
            
            (Size, char)[] possibleDirections = DirectionArrows
                .Where(da =>
                {
                    Size dir = da.Item1;
                    if(diff.Width == 0 && dir.Width != 0)
                    {
                        return false;
                    }
                    if(diff.Height == 0 && dir.Height != 0)
                    {
                        return false;
                    }
                    return dir.Width * diff.Width >= 0 && dir.Height * diff.Height >= 0;
                })
                .ToArray();
            
            foreach ((Size dir, char arrow) in possibleDirections)
            {
                Point next = current + dir;
                if (next.X < 0 || next.X >= 3 || next.Y < 0 || next.Y >= 4 || next == invalidPosition)
                {
                    continue;
                }
                
                char[] nextPath = currentPath
                    .Append(arrow)
                    .ToArray();

                if (next == currentTarget)
                {
                    nextPath = nextPath
                        .Append('A')
                        .ToArray();
                    if(targets.Length == 1)
                    {
                        possiblePaths.Add(nextPath);
                        continue;
                    }
                    targets = targets[1..];
                }
                
                queue.Enqueue((next, nextPath, targets));
            }
        }
        return possiblePaths;
    }

    private List<char[]> NumericPadPossibleMoves(char[] code)
    {
        /*  +---+---+---+
            | 7 | 8 | 9 |
            +---+---+---+
            | 4 | 5 | 6 |
            +---+---+---+
            | 1 | 2 | 3 |
            +---+---+---+
                | 0 | A |
                +---+---+ */

        var positions = new Dictionary<char, Point>
        {
            ['7'] = new(0, 0), ['8'] = new(1, 0), ['9'] = new(2, 0),
            ['4'] = new(0, 1), ['5'] = new(1, 1), ['6'] = new(2, 1),
            ['1'] = new(0, 2), ['2'] = new(1, 2), ['3'] = new(2, 2),
            /*               */['0'] = new(1, 3), ['A'] = new(2, 3)
        };
        var invalidPosition = new Point(0, 3);
        return GenericPadPossibleMoves(code, positions, invalidPosition);
    }

    [Fact]
    public void Part1Test1()
    {
        List<string> moves = NumericPadPossibleMoves(_testInput[0].ToCharArray()).Select(m => new string(m)).ToList();
        Assert.Contains("<A^A>^^AvvvA", moves);
        Assert.Contains("<A^A^>^AvvvA", moves);
        Assert.Contains("<A^A^^>AvvvA", moves);
    }
    
    private static List<char[]> DirectionalPadPossibleMoves(char[] code, char startLetter = 'A')
    {
        /*      +---+---+
                | ^ | A |
            +---+---+---+
            | < | v | > |
            +---+---+---+ */
        
        var positions = new Dictionary<char, Point>
        {
            /*               */['^'] = new(1, 0), ['A'] = new(2, 0),
            ['<'] = new(0, 1), ['v'] = new(1, 1), ['>'] = new(2, 1),
        };
        
        var invalidPosition = new Point(0, 0);
        return GenericPadPossibleMoves(code, positions, invalidPosition, startLetter);
    }
    
    [Fact]
    public void Part1Test2()
    {
        IEnumerable<string> moves = 
            NumericPadPossibleMoves(_testInput[0].ToCharArray())
                .SelectMany(x => DirectionalPadPossibleMoves(x))
                .Select(m => new string(m));
        Assert.Contains("v<<A>>^A<A>AvA<^AA>A<vAAA>^A", moves);
    }
    
    private List<char[]> AllPadsChained(char[] code)
    {
        List<char[]> movesLevel1 = NumericPadPossibleMoves(code);
        List<char[]> movesLevel2 = movesLevel1.SelectMany(x => DirectionalPadPossibleMoves(x)).ToList();
        int minMoves = movesLevel2.Min(m => m.Length);
        List<char[]> bestLevel2Moves = movesLevel2.Where(m => m.Length == minMoves).ToList();
        IEnumerable<char[]> movesLevel3 = bestLevel2Moves.SelectMany(x => DirectionalPadPossibleMoves(x));
        return movesLevel3.ToList();
    } 
        
    [Fact]
    public void Part1Test3()
    {
        Assert.Contains("<vA<AA>>^AvAA<^A>A<v<A>>^AvA^A<vA>^A<v<A>^A>AAvA^A<v<A>A>^AAAvA<^A>A", 
            AllPadsChained(_testInput[0].ToCharArray()).Select(m => new string(m)));
    }
    
    private int GetCodeScore(string code)
    {
        int codeNum = int.Parse(code[..^1]);
        List<char[]> moves = AllPadsChained(code.ToCharArray());
        return moves.Min(x => x.Length) * codeNum;
    }
    
    [Fact]
    public void Part1Test4()
    {
        int sum = _testInput.Sum(GetCodeScore);
        Assert.Equal(126384, sum);
    }
    
    [Fact]
    public void Part1()
    {
        int sum = _input.Sum(GetCodeScore);
        Assert.Equal(154208, sum);
    }

    private static Dictionary<(char, char), char[]> BestMovesDirectional;
    private static Dictionary<(char, char), int> BestMoveCountsDirectional;
    
    private static void InitBestMovesDirectional()
    {
        BestMovesDirectional = new Dictionary<(char, char), char[]>();
        BestMoveCountsDirectional = new Dictionary<(char, char), int>();
        var positions = new Dictionary<char, Point>
        {
            /*               */['^'] = new(1, 0), ['A'] = new(2, 0),
            ['<'] = new(0, 1), ['v'] = new(1, 1), ['>'] = new(2, 1),
        };
        
        var invalidPosition = new Point(0, 0);
        
        var toBake = new List<(char, char, List<char[]>)>();

        foreach ((char a, Point p1) in positions)
        {
            foreach ((char b, Point p2) in positions)
            {
                List<char[]> possiblePaths = DirectionalPadPossibleMoves([b], a);
                // _output.WriteLine($"{a} -> {b}: {string.Join(", ", possiblePaths.Select(p => new string(p)))}");
                if(possiblePaths.Count > 1)
                {
                    toBake.Add((a, b, possiblePaths));
                }
                else
                {
                    BestMovesDirectional[(a, b)] = possiblePaths[0];
                }
            }
        }
        
        foreach ((char a, char b, List<char[]> paths) in toBake)
        {
            var scores = new Dictionary<char[], int>();
            foreach (char[] path in paths)
            {
                var bestPaths = ConstructDirectionalPossibleMoves(path);
                for (int i = 0; i < 2; i++)
                {
                    bestPaths = bestPaths.SelectMany(ConstructDirectionalPossibleMoves).ToList();
                }
                scores[path] = bestPaths.Min(p => p.Length);
            }
            char[] bestPath = paths.First(p => scores[p] == scores.Values.Min());
            BestMovesDirectional[(a, b)] = bestPath;
        }
        
        foreach (((char, char) k, char[] v) in BestMovesDirectional)
        {
            BestMoveCountsDirectional[k] = v.Length;
        }
    }

    private static List<char[]> ConstructDirectionalPossibleMoves(char[] code)
    {
        var resultingPaths = new List<char[]>();
        for(var i = 0; i < code.Length - 1; i++)
        {
            var nextPaths = new List<char[]>();
            char a = code[i];
            char b = code[i + 1];
            if (BestMovesDirectional.TryGetValue((a, b), out char[]? path))
            {
                nextPaths.Add(path);
            }
            else
            {
                List<char[]> bestPaths = DirectionalPadPossibleMoves([b], a);
                nextPaths.AddRange(bestPaths);
            }

            if(resultingPaths.Count == 0)
            {
                resultingPaths = nextPaths;
                continue;
            }
            resultingPaths =
                resultingPaths
                    .SelectMany(rp => nextPaths.Select(np => rp.Concat(np).ToArray()))
                    .ToList();
        }
        return resultingPaths;
    }

    [Fact]
    public void Part2Test1()
    {
        InitBestMovesDirectional();
        var possiblePaths = NumericPadPossibleMoves(_testInput[0].ToCharArray());
        
        var bestScore = possiblePaths.Min(p => GetPathSum(p, 2));
        Assert.Equal(68, bestScore);        
    }
    
    private static readonly Dictionary<int, Dictionary<(char, char), long>> CountsAtLevels = new();
    
    long GetCountAtLevel(int lvl, char a, char b)
    {
        if(lvl == 0)
        {
            return BestMoveCountsDirectional[(a, b)];
        }
        
        if(CountsAtLevels.TryGetValue(lvl, out Dictionary<(char, char), long>? counts))
        {
            if(counts!.TryGetValue((a, b), out long count))
            {
                return count;
            }
        }
        else
        {
            CountsAtLevels[lvl] = new Dictionary<(char, char), long>();
        }

        long countAtLevel = GetPathSum(BestMovesDirectional[(a, b)], lvl);
        CountsAtLevels[lvl][(a, b)] = countAtLevel;
        return countAtLevel;
    }

    long GetPathSum(char[] path, int lvl)
    {
        long sum = 0;
        for(var i = -1; i < path.Length - 1; i++)
        {
            if(i == -1)
            {
                sum += GetCountAtLevel(lvl - 1, 'A', path[0]);
                continue;
            }
            sum += GetCountAtLevel(lvl - 1, path[i], path[i + 1]);
        }
        return sum;
    }

    [Fact]
    public void Part2Test2()
    {
        InitBestMovesDirectional();
        var totalSum = 0L;

        foreach (var code in _testInput)
        {
            var possiblePaths = NumericPadPossibleMoves(code.ToCharArray());
            var bestScore = possiblePaths.Min(p => GetPathSum(p, 2));
            long score = bestScore * int.Parse(code[..^1]);
            totalSum += score;
            _output.WriteLine($"{code}: {bestScore} * {int.Parse(code[..^1])}");
        }

        Assert.Equal(126384, totalSum);
    }

    [Fact]
    public void Part2()
    {
        InitBestMovesDirectional();
        var totalSum = 0L;

        foreach (var code in _input)
        {
            var possiblePaths = NumericPadPossibleMoves(code.ToCharArray());
            var bestScore = possiblePaths.Min(p => GetPathSum(p, 25));
            long score = bestScore * int.Parse(code[..^1]);
            totalSum += score;
            _output.WriteLine($"{code}: {bestScore} * {int.Parse(code[..^1])}");
        }

        Assert.Equal(188_000_493_837_892, totalSum);
    }
}