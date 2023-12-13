using System.Diagnostics;
using System.Text;
using Newtonsoft.Json;

namespace Advent_of_Code_2022._2023;

public static class Day12_2023
{
    public static void Run(string testInputPath, string challengeInputPath)
    {
        var sw = new Stopwatch();
        sw.Start();

        // var x = IsPossible("....#.##.##..?", new[] { 1, 2, 2 }, 5);
        // Console.WriteLine(x);
        // TestIsPossibles();
        //Solve(new[] { "?.????????#??? 1,2,2" });
        //return;
        
        //Solve(File.ReadAllLines(testInputPath));
        Solve(File.ReadAllLines(challengeInputPath));

        sw.Stop();
        Console.WriteLine($"Solution took: {sw.ElapsedMilliseconds} ms");
    }

    record Progress(Queue<string> Unprocessed, Dictionary<string, long> Processed);
    private static void Solve(string[] springs)
    {
        long sum = 0;
        long doneCnt = 0;
       // Progress progress = new Progress(new Queue<string>(springs), new Dictionary<string, long>());

        Progress progress = null;
        var progressJson = "2023_12-progress.json";
        if (!File.Exists(progressJson))
        {
            progress = new Progress(new Queue<string>(springs), new Dictionary<string, long>());
            File.WriteAllText(progressJson, JsonConvert.SerializeObject(progress, Formatting.Indented));
        }
        else
        {
            progress = JsonConvert.DeserializeObject<Progress>(File.ReadAllText(progressJson))!;
        }

        while (progress.Unprocessed.Count > 0)
        {
            string s = progress.Unprocessed.Dequeue();
            string[] splits = s.Split(" ");
            string springLine = splits[0];
            int[] groups = splits[1].Split(",").Select(int.Parse).ToArray();
            long cnt = GetArrangementsCntSmartest(
                string.Join("?", springLine, springLine, springLine, springLine, springLine),
                groups.Concat(groups).Concat(groups).Concat(groups).Concat(groups).ToArray());
            // long cnt = GetArrangementsCntSmartest(springLine, groups);
            progress.Processed[s] = cnt;
            File.WriteAllText(progressJson, JsonConvert.SerializeObject(progress, Formatting.Indented));
            Console.WriteLine($"{progress.Processed.Count} / {springs.Length} done.");
        }

        sum = progress.Processed.Values.Sum();
        
        //ThreadPool.SetMaxThreads(12, 12);
        // var tasks = springs.Select(s => Task.Run(() =>
        // {
        //     string[] splits = s.Split(" ");
        //     string springLine = splits[0];
        //     int[] groups = splits[1].Split(",").Select(int.Parse).ToArray();
        //     long cnt = GetArrangementsCntSmartest(
        //         string.Join("?", springLine, springLine, springLine, springLine, springLine),
        //         groups.Concat(groups).Concat(groups).Concat(groups).Concat(groups).ToArray());
        //     //long cnt = GetArrangementsCntSmartest(springLine, groups);
        //     
        //     Interlocked.Add(ref sum, cnt);
        //     Interlocked.Increment(ref doneCnt);
        //     Console.WriteLine($"Line {doneCnt} / {springs.Length} done");
        // })).ToArray();
        // var results = Task.WhenAll(tasks).GetAwaiter().GetResult();
        // foreach (Task task in tasks)
        // {
        //     task.Start();
        // }
        // Task.WaitAll(tasks);
        Console.WriteLine($"Answer: {sum}");

        /*var sum1 = 0L;
        var sum2 = 0L;
        var lineCnt = 1;
        foreach (string spring in springs)
        {
            string[] splits = spring.Split(" ");
            string springLine = splits[0];
            int[] groups = splits[1].Split(",").Select(int.Parse).ToArray();

            // var cnt1 = GetArrangementsCnt(springLine, groups);
            // //Console.WriteLine("---");
            // var cnt2 = GetArrangementsCntSmartest(springLine, groups);
            // if (cnt1 != cnt2)
            // {
            //     Console.WriteLine($"{cnt1} != {cnt2}");
            // }

            //sum1 += GetArrangementsCnt(springLine, groups);
            //sum1 += GetArrangementsCntSmarter(springLine, groups);

            //sum1 += GetArrangementsCntSmartest(springLine, groups);

            sum2 += GetArrangementsCntSmartest(string.Join("?", springLine, springLine, springLine, springLine, springLine), groups.Concat(groups).Concat(groups).Concat(groups).Concat(groups).ToArray());
            Console.WriteLine($"Line {lineCnt} / {springs.Length} done");
            lineCnt++;
        }
        Console.WriteLine($"Part 1: {sum1}");
        Console.WriteLine($"Part 2: {sum2}");*/
    }

    private static long GetArrangementsCntSmartest(string springLine, int[] groups)
    {
        var sw = new Stopwatch();
        sw.Start();
        Console.WriteLine($"{springLine} theoretic maximum values: {Math.Pow(2, springLine.Count(x => x == '?')):##,###}");
        int groupsSum = groups.Sum();
        var queue = new Stack<string>();
        queue.Push(springLine);
        //var arrangements = new List<string>();
        long arrangementsCnt = 0;
        long totalCnt = 0;
        while (queue.Count != 0)
        {
            var lines = new List<string>();
            for (int i = 0; i < THREAD_COUNT; i++)
            {
                if (queue.Count == 0) break;
                lines.Add(queue.Pop());
                totalCnt++;
            }

            IEnumerable<Task<List<string>>> tasks = lines.Select(x => Task.Run(() =>
            {
                string line = x;
                int idx = line.IndexOf('?');
                if (idx == -1)
                {
                    if (IsCorrect(line, groups))
                    {
                        // Console.WriteLine(line);
                        //arrangementsCnt++;
                        Interlocked.Increment(ref arrangementsCnt);
                        if (arrangementsCnt % 100000 == 0)
                        {
                            Console.WriteLine($"elapsed: {sw.Elapsed}. {arrangementsCnt} arrangements registered... {totalCnt:##,###} total");
                        }
                    }

                    return new List<string>();
                }

                var returns = new List<string>();
                string a = line[..idx] + '.' + line[(idx + 1)..];
                if (IsPossible(a, groups, groupsSum))
                {
                    returns.Add(a);
                }

                string b = line[..idx] + '#' + line[(idx + 1)..];
                if (IsPossible(b, groups, groupsSum))
                {
                    returns.Add(b);
                }

                return returns;
            }));

            List<string>[] returns = Task.WhenAll(tasks).GetAwaiter().GetResult();
            foreach (List<string> @return in returns)
            {
                foreach (string s in @return)
                {
                    queue.Push(s);
                }
            }

            //Console.WriteLine($"Stack size: {queue.Count}");

            /*string line = queue.Dequeue();
            
            if (!IsPossible(line, groups, groupsSum))
            {
                continue;
            }
            
            int idx = line.IndexOf('?');
            if (idx == -1)
            {
                if(IsCorrect(line, groups)){
                    // Console.WriteLine(line);
                    arrangementsCnt++;
                }
                continue;
            }
            
            queue.Enqueue(line[..idx] + '.' + line[(idx+1)..]);
            queue.Enqueue(line[..idx] + '#' + line[(idx+1)..]);*/
        }

        return arrangementsCnt;
    }

    private const int THREAD_COUNT = 120;

    private static bool IsPossible(string line, int[] groups, int groupsSum)
    {
        if (line.Count(c => c is '#' or '?') < groupsSum)
        {
            return false;
        }

        int idx = line.IndexOf('?');
        if (idx == -1)
        {
            return IsCorrect(line, groups);
        }

        var groupLen = 0;
        var groupIdx = 0;
        for (var i = 0; i < idx; i++)
        {
            if (line[i] == '.')
            {
                if (groupLen > 0)
                {
                    if (groupIdx >= groups.Length)
                    {
                        return false;
                    }
                    if (groupLen != groups[groupIdx])
                    {
                        return false;
                    }

                    groupIdx++;
                    groupLen = 0;
                }
            }
            else
            {
                groupLen++;
            }
        }

        return true;
    }

    private static long GetArrangementsCntSmarter(string springLine, int[] groups)
    {
        var unknownCount = springLine.Count(x => x == '?');
        var possibilities = (long)Math.Pow(2, unknownCount);
        Console.WriteLine($"[{DateTime.Now:s}] Line {springLine} {string.Join(",", groups)} : {unknownCount} unknowns!");
        long correctCnt = 0;
        Parallel.For(0, possibilities, state =>
        {
            if (state % (possibilities / 100) == 0)
            {
                Console.WriteLine($"[{DateTime.Now:s}] {100 * state / possibilities}% done.");
            }
            var sb = new StringBuilder();
            var qIndex = 0;
            foreach (char c in springLine)
            {
                if (c == '?')
                {
                    sb.Append((state >> qIndex) % 2 == 0 ? '.' : '#');
                    qIndex++;
                }
                else
                {
                    sb.Append(c);
                }
            }

            if (IsCorrect(sb.ToString(), groups))
            {
                Interlocked.Increment(ref correctCnt);
            }
        });
        return correctCnt;
    }
    
    private static int GetArrangementsCnt(string springLine, int[] groups)
    {
        var queue = new Queue<string>();
        queue.Enqueue(springLine);
        var arrangements = new List<string>();

        while (queue.Count != 0)
        {
            string line = queue.Dequeue();
            int idx = line.IndexOf('?');
            if (idx == -1)
            {
                arrangements.Add(line);
                continue;
            }

            queue.Enqueue(line[..idx] + '.' + line[(idx+1)..]);
            queue.Enqueue(line[..idx] + '#' + line[(idx+1)..]);
        }

        int correctArrangements = arrangements.Count(x => IsCorrect(x, groups));
        //Console.WriteLine($"total arrangements: {arrangements.Count}, correct arrangements: {correctArrangements}");
        //
        // if (springLine == "?.????????#???")
        // {
        //     foreach (string arrangement in arrangements)
        //     {
        //         if (IsCorrect(arrangement, groups))
        //         {
        //             Console.WriteLine(arrangement);
        //         }
        //     }
        // }

        return correctArrangements;
        
        
    }
    static bool IsCorrect(string line, int[] groups)
    {
        int[] lineGroups = line.Split(".", StringSplitOptions.RemoveEmptyEntries).Select(x => x.Length).ToArray();
        return lineGroups.SequenceEqual(groups);
    }
}