using System.Text.RegularExpressions;

namespace Advent_of_Code._2022;

public static class Day13_2022
{
    public static void Run(string testInputPath, string challengeInputPath)
    {
        //Solve(File.ReadAllLines(testInputPath));
        //Solve(File.ReadAllLines(challengeInputPath));
        //Solve2(File.ReadAllLines(testInputPath));
        Solve2(File.ReadAllLines(challengeInputPath));
    }

    private static void Solve2(string[] lines)
    {
        var lists = new Dictionary<string, string>();
        string[] packets = lines
            .Where(l => l.Length > 0)
            .Union(new[] { "[[2]]", "[[6]]" })
            .Select(x => ConvertList(x, lists))
            .ToArray();

        var ordered = true;
        while (ordered)
        {
            ordered = false;
            for (int i = 0; i < packets.Length - 1; i++)
            {
                var order = CheckOrder(packets[i], packets[i + 1], lists);
                if (order == false)
                {
                    (packets[i], packets[i + 1]) = (packets[i + 1], packets[i]);
                    ordered = true;
                    break;
                }
            }
        }

        string[] renderedPackets = packets.Select(x => RenderPacket(x, lists)).ToArray();

        foreach (var renderedPacket in renderedPackets)
        {
            Console.WriteLine(renderedPacket);
        }

        var index2 = Array.IndexOf(renderedPackets, "[[2]]")+1;
        var index6 = Array.IndexOf(renderedPackets, "[[6]]")+1;

        Console.WriteLine($"Index2: {index2}, Index6: {index6}. DecoderKey: {index2 * index6}");
    }

    private static string RenderPacket(string packet, Dictionary<string, string> lists)
    {
        var rendered = packet;
        while (rendered.Contains("L"))
        {
            var m = Regex.Match(rendered, @"(L\d+)");
            var lcontent = lists[m.Value];
            rendered = rendered[..m.Index] + "[" + lcontent + "]" + rendered[(m.Index + m.Length)..];
        }

        return rendered;
    }

    private static void Solve(string[] lines)
    {
        var pairIndex = 1;
        var indicesSum = 0;
        foreach (var pair in lines.Chunk(3))
        {
            var lists = new Dictionary<string, string>();
            var a = ConvertList(pair[0], lists);
            var b = ConvertList(pair[1], lists);

            bool? correctOrder = CheckOrder(a, b, lists);

            Console.WriteLine($"Pair {pairIndex} Correct order? {correctOrder}");
            if (correctOrder == true)
                indicesSum += pairIndex;
            
            pairIndex++;
        }

        Console.WriteLine($"Sum of indices: {indicesSum}");
    }

    private static string ConvertList(string line, Dictionary<string, string> lists)
    {
        while (line.Contains('['))
        {
            var startIdx = line.IndexOf('[');
            var closeIdx = FindEnding(line, startIdx);
            var listContent = ConvertList(line[(startIdx + 1)..closeIdx], lists);
            var listId = "L" + lists.Count;
            lists[listId] = listContent;
            line = line[..startIdx] + listId + line[(closeIdx + 1)..];
        }

        return line;
    }

    private static bool? CheckOrder(string a, string b, Dictionary<string,string> lists)
    {
        var itemsA = a.Split(',', StringSplitOptions.RemoveEmptyEntries);
        var itemsB = b.Split(',', StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < Math.Max(itemsA.Length, itemsB.Length); i++)
        {
            // left runs out of items - correct order
            if (i == itemsA.Length)
                return true;
            
            // right runs out of items - incorrect order
            if (i == itemsB.Length)
                return false;

            var itemA = itemsA[i];
            var itemB = itemsB[i];
            
            // not lists, compare values
            if (!itemA.StartsWith("L") && !itemB.StartsWith("L"))
            {
                var nA = int.Parse(itemA);
                var nB = int.Parse(itemB);
                if (nA < nB)
                    return true;
                else if (nA > nB)
                    return false;
                continue;
            }

            // get list values and compare them
            var order = CheckOrder(
                itemA.StartsWith("L") ? lists[itemA] : itemA,
                itemB.StartsWith("L") ? lists[itemB] : itemB,
                lists
            );
            if (order != null)
                return order;
        }

        return null;
    }

    static int FindEnding(string line, int start)
    {
        var depth = 0;
        for (int i = start; i < line.Length; i++)
        {
            var c = line[i];
            if (c == '[')
            {
                depth++;
            } else if (c == ']')
            {
                depth--;
                if (depth == 0)
                    return i;
            }
        }

        return line.Length - 1;
    }
}
