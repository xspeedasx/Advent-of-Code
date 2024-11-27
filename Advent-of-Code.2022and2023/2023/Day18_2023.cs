using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Text;
using static Advent_of_Code.Directions;

namespace Advent_of_Code._2023;

public static class Day18_2023
{
    public static void Run(string testInputPath, string challengeInputPath)
    {
        var sw = new Stopwatch();
        sw.Start();

        //Solve(File.ReadAllLines(testInputPath));
        //Solve(File.ReadAllLines(challengeInputPath));
        //Solve2(File.ReadAllLines(testInputPath));
        Solve2(File.ReadAllLines(challengeInputPath));

        sw.Stop();
        Console.WriteLine($"Solution took: {sw.ElapsedMilliseconds} ms");
    }

    record Line(Point Start, Point End)
    {
        public int MinY => Math.Min(Start.Y, End.Y);
        public int MaxY => Math.Max(Start.Y, End.Y);
        public int MinX => Math.Min(Start.X, End.X);
        public int MaxX => Math.Max(Start.X, End.X);
    };
    
    private static void Solve2(string[] inputLines)
    {
        var current = Point.Empty;
        var max = new Size();
        var min = new Size();
        var lines = new List<Line>();
        
        foreach (string inputLine in inputLines)
        {
            string[] splits = inputLine.Split(' ');
            string distStr = splits[2][2..^2];
            Direction dir = splits[2][^2..^1] switch
            {
                "0" => Direction.RIGHT,
                "1" => Direction.DOWN,
                "2" => Direction.LEFT,
                "3" => Direction.UP,
                _ => throw new ArgumentOutOfRangeException()
            };
            
            int dist = int.Parse(distStr, NumberStyles.HexNumber);
            
            // PART 1:
            
            // Direction dir = splits[0][0] switch
            // {
            //     'R' => Direction.RIGHT,
            //     'L' => Direction.LEFT,
            //     'U' => Direction.UP,
            //     'D' => Direction.DOWN,
            //     _ => throw new ArgumentOutOfRangeException()
            // };
            // int dist = int.Parse(splits[1]);
            // string color = splits[2][1..^1];

            Size offset = Offsets[dir];

            Point next = Point.Add(current, offset * dist);
            lines.Add(new Line(current, next));
            current = next;

            //Console.WriteLine($"dir: {dir}, dist: {dist}, location: {current}");
            
            if (current.X > max.Width)
            {
                max.Width = current.X;
            }
            if (current.Y > max.Height)
            {
                max.Height = current.Y;
            }

            if (current.X < min.Width)
            {
                min.Width = current.X;
            }
            if (current.Y < min.Height)
            {
                min.Height = current.Y;
            }
        }

        Console.WriteLine($"min: {min}");
        Console.WriteLine($"max: {max}");

        // debug SVG:
        var sb = new StringBuilder();
        var zoom = 1000;
        sb.Append(
            $@"<svg viewBox=""{min.Width/zoom - 1} {min.Height/zoom - 1} {(max.Width-min.Width)/zoom + 2 } {(max.Height-min.Height)/zoom + 2}""  xmlns=""http://www.w3.org/2000/svg"">");
        
        foreach (Line line in lines)
        {
            sb.Append(
                $"<line x1=\"{line.Start.X/zoom}\" y1=\"{line.Start.Y/zoom}\" x2=\"{line.End.X/zoom}\" y2=\"{line.End.Y/zoom}\" stroke=\"black\" />");
        }
        
        sb.Append("</svg>");
        
        File.WriteAllText("2023_18.svg", sb.ToString());

        Line[] horizontals = lines.Where(x => x.Start.Y == x.End.Y).OrderBy(x => x.Start.Y).ToArray();
        Line[] verticals = lines.Where(x => x.Start.Y != x.End.Y).ToArray();
        int currY = min.Height;
        int lastY = min.Height;

        long sum = 0;
        Line[] firstHoriz = horizontals.Where(x => currY == x.Start.Y).ToArray();
        Console.WriteLine($"{firstHoriz.Length} horizontals added at y={currY} total {firstHoriz.Sum(x => Math.Abs(x.Start.X - x.End.X) + 1)}");
        sum += firstHoriz.Sum(x => Math.Abs(x.Start.X - x.End.X) + 1);
        
        while (currY <= max.Height)
        {
            //Line[] nextLines = horizontals.Where(x => x.Start.Y == currY).ToArray();
            var nextY = horizontals.FirstOrDefault(h => h.Start.Y > currY)?.Start.Y;
            // if(nextLines.Length == 0){break;}
            if(nextY == null) {break;}

            var nextLines = horizontals.Where(x => x.Start.Y == nextY).ToArray();
            currY = nextY.Value;

            var crossings = verticals.Where(v =>
            {
                int minY = Math.Min(v.Start.Y, v.End.Y);
                int maxY = Math.Max(v.Start.Y, v.End.Y);
                return minY <= lastY && maxY >= currY;
            }).OrderBy(x => x.Start.X).ToArray();

            while (crossings.Any())
            {
                Line left = crossings[0];
                Line right = crossings[1];
                long w = right.Start.X - left.Start.X + 1;
                long h = currY - lastY - 1;
                sum += w * h;
                crossings = crossings[2..];
                Console.WriteLine($"rect added: {left.Start.X},{lastY+1} {right.Start.X},{currY-1}. area: {w*h}");
            }

            long horizontalSum = 0;
            
            var allVerticalsAtY = verticals.Where(x => x.MinY <= currY && x.MaxY >= currY);
            var isInside = false;
            Line? lastVert = null;
            foreach (Line vert in allVerticalsAtY.OrderBy(x => x.Start.X))
            {
                horizontalSum++;
                if(lastVert == null)
                {
                    lastVert = vert;
                    if (lastVert.MinY != currY && lastVert.MaxY != currY)
                    {
                        isInside = true;
                    }
                    continue;
                }
                
                // are connected?
                Line? connectedLine = horizontals.SingleOrDefault(x =>
                    x.Start.Y == currY && x.MinX == lastVert.Start.X && x.MaxX == vert.Start.X);
                if (connectedLine is { })
                {
                    horizontalSum += connectedLine.MaxX - connectedLine.MinX - 1;

                    int above = (lastVert.MinY < currY ? 1 : 0) + (vert.MinY < currY ? 1 : 0);
                    if (above == 1)
                    {
                        isInside = !isInside;
                    }
                }
                else
                {
                    if (isInside)
                    {
                        horizontalSum += vert.Start.X - lastVert.Start.X - 1;
                    }

                    if (vert.MinY != currY && vert.MaxY != currY)
                    {
                        isInside = !isInside;
                    }
                }

                lastVert = vert;
            }
            
            Console.WriteLine($"horizontals added at y={currY} total {horizontalSum}");
            sum += horizontalSum;
            
            lastY = currY;
        }

        Console.WriteLine($"answer: {sum}");
    }
    
    private static void Solve(string[] lines)
    {
        var current = new Point(0, 0);
        var painted = new HashSet<Point>();
        var insides = new HashSet<Point>();
        painted.Add(current);

        var max = new Size();
        var min = new Size();
        
        foreach (string line in lines)
        {
            string[] splits = line.Split(' ');
            Direction dir = splits[0][0] switch
            {
                'R' => Direction.RIGHT,
                'L' => Direction.LEFT,
                'U' => Direction.UP,
                'D' => Direction.DOWN,
                _ => throw new ArgumentOutOfRangeException()
            };
            int dist = int.Parse(splits[1]);
            string color = splits[2][1..^1];

            Size offset = Offsets[dir];
            for (int i = 0; i < dist; i++)
            {
                current = Point.Add(current, offset);
                if (!painted.Add(current))
                {
                    Console.WriteLine($"Crossing path at {current}");
                }

                if (current.X > max.Width)
                {
                    max.Width = current.X;
                }
                if (current.Y > max.Height)
                {
                    max.Height = current.Y;
                }

                if (current.X < min.Width)
                {
                    min.Width = current.X;
                }
                if (current.Y < min.Height)
                {
                    min.Height = current.Y;
                }
            }
        }

        var printLines = new List<string>();
        
        for (int y = min.Height; y <= max.Height; y++)
        {
            var sb = new StringBuilder();
            bool isInside = false;
            char? lastCorner = null;
            for (int x = min.Width; x <= max.Width; x++)
            {
                var pt = new Point(x, y);
                //Console.Write(painted.Contains(pt) ? '#' : ' ');
                bool contains = painted.Contains(pt);

                if (contains)
                {
                    Point upper = pt with { Y = pt.Y - 1 };
                    Point lower = pt with { Y = pt.Y + 1 };
                    if (painted.Contains(upper) && painted.Contains(lower))
                    {
                        isInside = !isInside;
                        lastCorner = null;
                    }
                    else if (painted.Contains(upper))
                    {
                        if (painted.Contains(lower))
                        {
                            isInside = !isInside;
                            lastCorner = null;
                        }
                        else
                        {
                            if(lastCorner is null)
                            {
                                lastCorner = 'u';
                            }
                            else if (lastCorner == 'd')
                            {
                                isInside = !isInside;
                            }
                            else
                            {
                                // same corner = no change
                            }
                        }
                    }
                    else
                    {
                        if (painted.Contains(lower))
                        {
                            if (lastCorner == null)
                            {
                                lastCorner = 'd';
                            }
                            else if (lastCorner == 'u')
                            {
                                isInside = !isInside;
                            }
                            else
                            {
                                // same corner = no change
                            }
                        }
                        else
                        {
                            
                        }
                    }
                }
                else
                {
                    if (isInside)
                    {
                        insides.Add(pt);
                    }
                }
                
                sb.Append(contains ? '#' : isInside ? '.' : ' ');
            }

            //Console.WriteLine();
            printLines.Add(sb.ToString());
        }

        File.WriteAllLines("2023_18.txt", printLines);
        Console.WriteLine("done");
        Console.WriteLine($"answer: {insides.Count + painted.Count}");
    }
}