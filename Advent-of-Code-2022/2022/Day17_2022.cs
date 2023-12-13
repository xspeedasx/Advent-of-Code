using System.Collections.Immutable;
using System.Drawing;

namespace Advent_of_Code_2022._2022;

public static class Day17_2022
{
    public static void Run(string testInputPath, string challengeInputPath)
    {
        Solve(File.ReadAllLines(testInputPath), 2022);
        //Solve(File.ReadAllLines(challengeInputPath), 1000000000000);

        // Solve2(File.ReadAllLines(testInputPath));
        // Solve2(File.ReadAllLines(challengeInputPath));
    }

    private static void Solve(string[] lines, long roundCount)
    {
        ImmutableArray<char> winds = lines[0].ToImmutableArray();
        var shapes = new[]
        {
            new Shape("####"),
            new Shape(
                ".#.",
                "###",
                ".#."),
            new Shape(
                "..#",
                "..#",
                "###"),
            new Shape(
                "#",
                "#",
                "#",
                "#"),
            new Shape(
                "##",
                "##")
        };

        var shapeIdx = 0;
        var windIdx = 0;
        var lowest = 0;
        
        var room = new HashSet<Point>();
        var roomWidth = 7;
        
        for (long round = 0; round < roundCount; round++)
        {
            Shape shape = shapes[shapeIdx++ % shapes.Length];
            Point shapePos = new Point(3, lowest + 4);
            
            var stopped = false;
            while (!stopped)
            {
                // wind
                //Render(shape, shapePos);
                char dir = winds[windIdx++ % winds.Length];
                //Console.WriteLine($"wind dir: {dir}");
                if (dir == '>')
                {
                    if (shape.CanMoveRight(shapePos, room, roomWidth))
                    {
                        shapePos = AddPoints(shapePos, Right);
                    }
                }
                else
                {
                    if (shape.CanMoveLeft(shapePos, room, roomWidth))
                    {
                        shapePos = AddPoints(shapePos, Left);
                    }
                }

                // fall
                //Render(shape, shapePos);

                if (shape.CanMoveDown(shapePos, room))
                {
                    shapePos = AddPoints(shapePos, Down);
                }
                else
                {
                    stopped = true;
                }
            }

            foreach (Point shapeCell in shape.Cells)
            {
                Point newPt = AddPoints(shapeCell, shapePos);
                if (newPt.Y > lowest)
                {
                    lowest = newPt.Y;
                }

                room.Add(newPt);
            }

            //Render(null, shapePos);
            Console.WriteLine($"Round {round+1} end. Lowest: {lowest}");
        }


        void Render(Shape? curr = null, Point? shapePos = null)
        {
            int shapeOffset = curr?.Height ?? 0;
            for (int y = lowest+4+shapeOffset; y >= 0; y--)
            {
                if (y == 0)
                {
                    Console.WriteLine("+-------+");
                }
                else
                {
                    for (int x = 0; x < roomWidth+2; x++)
                    {
                        if (x == 0 || x == roomWidth + 1)
                        {
                            Console.Write('|');
                        }
                        else
                        {
                            if(curr != null && curr.Cells.Contains(new Point(x-shapePos!.Value.X, y-shapePos!.Value.Y)))
                                Console.Write('@');
                            else
                                Console.Write(room.Contains(new Point(x, y)) ? '#' : '.');
                        }
                    }
                }

                Console.WriteLine();
            }
        }
    }

    class Shape
    {
        public int Height { get; set; }
        public int Width { get; set; }
        public Point[] Cells { get; set; }

        private Point[] rightmost { get; set; }
        private Point[] leftmost { get; set; }
        private Point[] bottom { get; set; }
        
        public Shape(params string[] lines)
        {
            Height = lines.Length;
            Width = lines[0].Length;
            var cells = new List<Point>();
            int y = lines.Length - 1;
            foreach (string line in lines)
            {
                for (int x = 0; x < line.Length; x++)
                {
                    if (line[x] == '#')
                    {
                        cells.Add(new Point(x, y));
                    }
                    
                }
                y--;
            }

            Cells = cells.ToArray();

            rightmost = cells.GroupBy(x => x.Y).Select(x => x.MaxBy(c => c.X)).ToArray();
            leftmost = cells.GroupBy(x => x.Y).Select(x => x.MinBy(c => c.X)).ToArray();
            bottom = cells.GroupBy(x => x.X).Select(x => x.MinBy(c => c.Y)).ToArray();
            
            Console.WriteLine("Shape:");
            foreach (string line in lines)
            {
                Console.WriteLine(line);
            }

            Console.WriteLine("Rightmost:");
            foreach (Point point in rightmost)
            {
                Console.WriteLine($"[{point.X},{point.Y}]");
            }
            Console.WriteLine("Leftmost:");
            foreach (Point point in leftmost)
            {
                Console.WriteLine($"[{point.X},{point.Y}]");
            }
            Console.WriteLine("Bottom:");
            foreach (Point point in bottom)
            {
                Console.WriteLine($"[{point.X},{point.Y}]");
            }
        }

        public bool CanMoveRight(Point pos, IReadOnlySet<Point> room, int roomWidth)
        {
            foreach (Point rm in rightmost)
            {
                Point nextPt = AddPoints(pos, rm, Right);
                if (nextPt.X == roomWidth + 1 || room.Contains(nextPt))
                {
                    return false;
                }
            }

            return true;
        }

        public bool CanMoveLeft(Point pos, IReadOnlySet<Point> room, int roomWidth)
        {
            foreach (Point lm in leftmost)
            {
                Point nextPt = AddPoints(pos, lm, Left);
                if (nextPt.X == 0 || room.Contains(nextPt))
                {
                    return false;
                }
            }

            return true;
        }

        public bool CanMoveDown(Point pos, IReadOnlySet<Point> room)
        {
            foreach (Point b in bottom)
            {
                Point nextPt = AddPoints(pos, b, Down);
                if (nextPt.Y == 0 || room.Contains(nextPt))
                {
                    return false;
                }
            }

            return true;
        }
    }

    public static Point Right => new Point(1, 0);
    public static Point Left => new Point(-1, 0);
    public static Point Down => new Point(0, -1);
    public static Point AddPoints(params Point[] points)
    {
        var x = 0;
        var y = 0;
        foreach (Point point in points)
        {
            x += point.X;
            y += point.Y;
        }

        return new Point(x, y);
    }
}