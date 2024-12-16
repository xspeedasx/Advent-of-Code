using System.Drawing;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace Advent_of_Code._2024;

public class Day16
{
    private readonly ITestOutputHelper _output;
    private readonly string[] _input = File.ReadAllLines("Inputs/Day16.txt");
    private readonly string[][] _testInput;
    
    private static Size[] Directions =
    [
        new(0, -1), // N
        new(1, 0), // E
        new(0, 1), // S
        new(-1, 0) // W
    ]; 

    public Day16(ITestOutputHelper output)
    {
        _output = output;
        string[] lines = File.ReadAllLines("Inputs/TestInputs/Test16.txt");
        var cnt = 0;
        var part = new List<string>();
        _testInput = new string[lines.Count(x => x == "") + 1][];
        foreach (string t in lines)
        {
            if (t == "")
            {
                _testInput[cnt] = part.ToArray();
                part.Clear();
                cnt++;
                continue;
            }

            part.Add(t);
        }

        _testInput[cnt] = part.ToArray();
    }

    public (int score, int tiles) MazeScore(string[] map)
    {
        int mapw = map[0].Length;
        int maph = map.Length;
        var start = new Point(1, maph - 2);
        var end = new Point(mapw - 2, 1);

        var visitedScores = new Dictionary<Point, CellScores?>();
        var queue = new Queue<TraversalState>();
        var finishes = new List<TraversalState>();
        queue.Enqueue(new TraversalState(0, [start with { X = start.X - 1 }, start]));

        while (queue.Count > 0)
        {
            TraversalState state = queue.Dequeue();
            Traverse(state);
        }

        var minFinish = finishes.MinBy(x => x.Score);
        var allMinFinishes = finishes.Where(x => x.Score == minFinish.Score).ToList();
        
        var visited = new HashSet<Point>();
        foreach (TraversalState finish in allMinFinishes)
        {
            foreach (Point point in finish.Path)
            {
                visited.Add(point);
            }
        }
        
        for(int y = 0; y < maph; y++)
        {
            var line = new StringBuilder();
            for(int x = 0; x < mapw; x++)
            {
                if (visited.Contains(new Point(x, y)))
                {
                    line.Append('O');
                }
                else
                {
                    line.Append(map[y][x]);
                }
            }
            _output.WriteLine(line.ToString());
        }
        
        return (minFinish.Score, visited.Count - 1); // adjusted for initial facing direction prev tile

        void Traverse(TraversalState state)
        {
            Point currentPos = state.Path[^1];
            Point prevPos = state.Path.Length > 1 ? state.Path[^2] : currentPos;

            var scoresUpdated = false;
            if (!visitedScores.TryGetValue(currentPos, out CellScores? cellScores))
            {
                cellScores = new CellScores(int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue);
                scoresUpdated = true;
            }

            foreach (Size dir in Directions)
            {
                Point nextPos = currentPos + dir;
                if (nextPos == prevPos)
                {
                    continue;
                }

                char c = map[nextPos.Y][nextPos.X];
                if (c == '#')
                {
                    continue;
                }
                
                //is turning
                int cost = state.Score + 1;
                if(Math.Abs(prevPos.X - nextPos.X) == 1 && Math.Abs(prevPos.Y - nextPos.Y) == 1)
                {
                    cost += 1000;
                }
                
                int oldScore = cellScores!.Scores[dir];
                if (cost > oldScore)
                {
                    continue;
                }

                cellScores.Scores[dir] = cost;
                
                if (c == 'E')
                {
                    finishes.Add(new TraversalState(cost, state.Path.Append(nextPos).ToArray()));
                }

                queue.Enqueue(new TraversalState(cost, state.Path.Append(nextPos).ToArray()));
                scoresUpdated = true;
            }
            
            if(scoresUpdated)
            {
                visitedScores[currentPos] = cellScores;
            }
        }
    }
    
    [Fact]
    public void Part1Test1()
    {
        Assert.Equal(7036, MazeScore(_testInput[0]).score);
    }
    
    [Fact]
    public void Part1Test2()
    {
        Assert.Equal(11048, MazeScore(_testInput[1]).score);
    }
    
    [Fact]
    public void Part1()
    {
        Assert.Equal(73404, MazeScore(_input).score);
    }
    
    [Fact]
    public void Part2Test1()
    {
        Assert.Equal(45, MazeScore(_testInput[0]).tiles);
    }
    
    [Fact]
    public void Part2Test2()
    {
        Assert.Equal(64, MazeScore(_testInput[1]).tiles);
    }
    
    [Fact]
    public void Part2()
    {
        Assert.Equal(449, MazeScore(_input).tiles);
    }

    private record struct TraversalState(int Score, Point[] Path);

    private class CellScores(int up, int down, int left, int right)
    {
        public int Up
        {
            get => Scores[Directions[0]];
            set => Scores[Directions[0]] = value;
        }
        
        public int Right
        {
            get => Scores[Directions[1]];
            set => Scores[Directions[1]] = value;
        }
        
        public int Down
        {
            get => Scores[Directions[2]];
            set => Scores[Directions[2]] = value;
        }
        
        public int Left
        {
            get => Scores[Directions[3]];
            set => Scores[Directions[3]] = value;
        }
        
        public readonly Dictionary<Size, int> Scores = new()
        {
            [Directions[0]] = up,
            [Directions[1]] = right,
            [Directions[2]] = down,
            [Directions[3]] = left
        };
    }
}